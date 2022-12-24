﻿using CoolerItemVisualEffect.Weapons;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader.Config;
using Terraria.Utilities;
using static CoolerItemVisualEffect.CoolerItemVisualEffect;
using static Terraria.Utils;
namespace CoolerItemVisualEffect
{
    public class LightTree
    {
        public class Node
        {
            public float rad, size, length;
            public List<Node> children;
            public Texture2D leafTex;
            public bool drawLeaf;
            public Rectangle leafFrame;
            public Node(float rad, float size, float length)
            {
                this.rad = rad;
                this.size = size;
                this.length = length;
                this.children = new List<Node>();
            }
        };
        public Node root;
        private UnifiedRandom random;
        public LightTree(UnifiedRandom random)
        {
            cnt = 0;
            root = null;
            this.random = random;
        }
        public LightTree(Node node, UnifiedRandom random)
        {
            cnt = 0;
            root = node;
            this.random = random;
        }
        private Vector2 target;
        private int cnt;
        public List<Vector2> keyPoints;

        public void SpawnProjectile(Projectile projectile, Vector2 position, Vector2 velocity, Node node, float chance = .2f)
        {
            float r = velocity.ToRotation();
            Vector2 unit = (r + node.rad).ToRotationVector2();
            if (Main.rand.NextFloat(0, 1) < chance)
            {
                bool flag = false;
                if (projectile.type == ModContent.ProjectileType<WitheredTree>())
                    flag = (int)projectile.ai[0] / 2 == 1;

                var rand = flag ? Main.rand.Next(4) : 0;
                int index = -1;
                if (rand != 0 && Main.rand.NextBool(3))
                {
                    foreach (var npc in Main.npc)
                    {
                        if (npc.active && npc.CanBeChasedBy() && !npc.friendly && (npc.Center - position).Length() <= 768)
                        {
                            index = npc.whoAmI;
                            break;
                        }
                    }
                }
                var proj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), position, new Vector2(this.rand() * 2 - 1, 0) * 4, ModContent.ProjectileType<WitheredWood>(), projectile.damage / 4, projectile.knockBack * .5f, projectile.owner, rand, index);
                proj.rotation = r + node.rad;//this.rand() * MathHelper.TwoPi
                if (proj.ModProjectile is WitheredWood wood)
                {
                    wood.tree = new LightTree(node, random);
                }
            }
            else
                foreach (var child in node.children)
                {
                    SpawnProjectile(projectile, position + unit * node.length, unit, child, chance + Main.rand.NextFloat(-.05f, .15f));
                }
        }
        public void Generate(Vector2 pos, Vector2 vel, Vector2 target, bool hasLeaf)
        {
            // 根节点生成，朝向0，粗细1，长度随机50中选
            root = new Node(0, 1f, (rand() * .25f + .75f) * 128);
            keyPoints = new List<Vector2>();
            this.target = target;
            root = _build(root, pos, vel, true, hasLeaf);
            // Main.NewText($"生成了一个{cnt}个节点的树状结构");
        }
        public void Generate(Vector2 pos, Vector2 vel, Vector2 target, float lengthStart, bool hasLeaf)
        {
            // 根节点生成，朝向0，粗细1，长度随机50中选
            root = new Node(0, 1f, lengthStart);
            keyPoints = new List<Vector2>();
            this.target = target;
            root = _build(root, pos, vel, true, hasLeaf);
            // Main.NewText($"生成了一个{cnt}个节点的树状结构");
        }
        public void Generate(Vector2 pos, Vector2 vel, Vector2 target, float lengthStart, float minSize, float minLength, float minDistance, float randAngleMain, float randAngleBranch, float chance, float decreaseSize, float decreaseLength, float decreaseSizeB, float decreaseLengthB, bool hasLeaf)
        {
            // 根节点生成，朝向0，粗细1，长度随机50中选
            root = new Node(0, 1f, lengthStart);
            keyPoints = new List<Vector2>();
            this.target = target;
            root = _build(root, pos, vel, true, minSize, minLength, minDistance, randAngleMain, randAngleBranch, chance, decreaseSize, decreaseLength, decreaseSizeB, decreaseLengthB, hasLeaf);
            // Main.NewText($"生成了一个{cnt}个节点的树状结构");
        }
        private Node _build(Node node, Vector2 pos, Vector2 vel, bool root, bool hasLeaf)
        {
            keyPoints.Add(pos);
            cnt++;
            if (node.size < 0.1f || node.length < 1 || Vector2.Distance(pos, target) < 10)
            {
                if (hasLeaf && !Main.rand.NextBool(3))
                {
                    try
                    {
                        Main.instance.LoadTiles(5);
                        Main.instance.LoadGore(385);
                        Main.instance.LoadGore(384);
                    }
                    catch { }
                    int index = Main.rand.Next(3);
                    node.leafTex = index switch
                    {
                        0 => TextureAssets.TreeBranch[9].Value,
                        1 => TextureAssets.Gore[385].Value,
                        _ or 2 => TextureAssets.Gore[384].Value
                    };
                    node.leafFrame = index switch
                    {
                        0 => new Rectangle(42, 0, 42, 42),
                        1 => new Rectangle(0, 0, 36, 34),
                        _ or 2 => new Rectangle(0, 0, 40, 28)
                    };
                }
                return node;
            }
            var r2 = (target - pos).ToRotation() - vel.ToRotation();
            var r = r2 * .5f + rand(MathHelper.Pi / 4f) * 1.5f;
            //var r = rand(MathHelper.Pi / 4f);
            var unit = (vel.ToRotation() + r).ToRotationVector2();
            Node rchild = new Node(r, node.size * .9f, node.length * (cnt == 1 ? .25f : .975f));
            // 闪电树主节点（树干）
            node.children.Add(_build(rchild, pos + unit * node.length, unit, root, hasLeaf));
            if (root || rand() > 0.35f * node.size * (1 - node.size) * 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (rand() > 0.75f)
                    {

                        r = rand(MathHelper.Pi / 3f);
                        unit = (vel.ToRotation() + r).ToRotationVector2();
                        Node child = new Node(r, (.5f + rand() * .5f) * node.size * 0.65f, node.length * (cnt == 1 ? .25f : .6f));
                        node.children.Add(_build(child, pos + unit * node.length, unit, false, hasLeaf));
                    }
                }
            }
            return node;
        }
        private Node _build(Node node, Vector2 pos, Vector2 vel, bool root, float minSize, float minLength, float minDistance, float randAngleMain, float randAngleBranch, float chance, float decreaseSize, float decreaseLength, float decreaseSizeB, float decreaseLengthB, bool hasLeaf)
        {
            keyPoints.Add(pos);
            cnt++;
            if (node.size < minSize || node.length < minLength || Vector2.Distance(pos, target) < minDistance)
            {
                if (hasLeaf && !Main.rand.NextBool(3))
                {
                    try
                    {
                        Main.instance.LoadTiles(5);
                        Main.instance.LoadGore(385);
                        Main.instance.LoadGore(384);
                    }
                    catch { }
                    int index = Main.rand.Next(3);
                    node.leafTex = index switch
                    {
                        0 => TextureAssets.TreeBranch[9].Value,
                        1 => TextureAssets.Gore[385].Value,
                        _ or 2 => TextureAssets.Gore[384].Value
                    };
                    node.leafFrame = index switch
                    {
                        0 => new Rectangle(42, 0, 42, 42),
                        1 => new Rectangle(0, 0, 36, 34),
                        _ or 2 => new Rectangle(0, 0, 40, 28)
                    };
                }
                return node;
            }
            var r2 = (target - pos).ToRotation() - vel.ToRotation();
            var r = r2 * .5f + rand(randAngleMain) * 1.5f;
            //var r = rand(MathHelper.Pi / 4f);
            var unit = (vel.ToRotation() + r).ToRotationVector2();
            Node rchild = new Node(r, node.size * decreaseSize, node.length * (cnt == 1 ? .25f : decreaseLength));
            // 闪电树主节点（树干）
            node.children.Add(_build(rchild, pos + unit * node.length, unit, root, minSize, minLength, minDistance, randAngleMain, randAngleBranch, chance, decreaseSize, decreaseLength, decreaseSizeB, decreaseLengthB, hasLeaf));
            if (root || rand() > chance * node.size * (1 - node.size) * 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (rand() > 0.75f)
                    {

                        r = rand(randAngleBranch);
                        unit = (vel.ToRotation() + r).ToRotationVector2();
                        Node child = new Node(r, (.5f + rand() * .5f) * node.size * decreaseSizeB, node.length * (cnt == 1 ? .25f : decreaseLengthB));
                        node.children.Add(_build(child, pos + unit * node.length, unit, false, minSize, minLength, minDistance, randAngleMain, randAngleBranch, chance, decreaseSize, decreaseLength, decreaseSizeB, decreaseLengthB, hasLeaf));
                    }
                }
            }
            return node;
        }
        //private Node _build2(Node node, Vector2 pos, Vector2 vel, bool isMain, Vector2 target) {
        //    cnt++;
        //    keyPoints.Add(pos);
        //    // 终止条件：树枝太细了，或者太短了
        //    if (node.size < 0.1f || node.length < 1) return node;
        //    var r2 = (target - pos).ToRotation() - vel.ToRotation();
        //    var r = r2 + rand(MathHelper.Pi / 4f);
        //    Vector2 unit = (vel.ToRotation() + r).ToRotationVector2();
        //    Node main = new Node(rand(r), node.size * 0.95f, node.length);
        //    node.children.Add(_build(main, pos + unit * node.length, unit, isMain, target));
        //    // 只有较小的几率出分支
        //    if (rand() > 0.9f) {
        //        // 生成分支的时候长度变化不大，但是大小变化很大
        //        r = rand(MathHelper.Pi / 3f);
        //        unit = (vel.ToRotation() + r).ToRotationVector2();
        //        Node child = new Node(r, node.size * 0.6f, node.length);
        //        node.children.Add(_build(child, pos + unit * node.length, unit, false, target));
        //    }
        //    return node;
        //}


        //private Node _build(Node node, Vector2 pos, Vector2 vel, bool root) {
        //    keyPoints.Add(pos);
        //    cnt++;
        //    if (node.size < 0.1f || node.length < 1) return node;
        //    var r2 = (target - pos).ToRotation() - vel.ToRotation();
        //    var r = r2 + rand(MathHelper.Pi / 4f);
        //    var unit = (vel.ToRotation() + r).ToRotationVector2();
        //    Node rchild = new Node(r, node.size * 0.9f, node.length);
        //    // 闪电树主节点（树干）
        //    node.children.Add(_build(rchild, pos + unit * node.length, unit, root));
        //    if (root) {
        //        if (rand() > 0.8f) {
        //            for (int i = 0; i < 1; i++) {
        //                r = rand(MathHelper.Pi / 3f);
        //                unit = (vel.ToRotation() + r).ToRotationVector2();
        //                Node child = new Node(r, rand() * node.size * 0.6f, node.length * 0.6f);
        //                node.children.Add(_build(child, pos + unit * node.length, unit, false));
        //            }
        //        }
        //    }
        //    return node;
        //}




        public float rand()
        {
            double u = -2 * Math.Log(random.NextDouble());
            double v = 2 * Math.PI * random.NextDouble();
            return (float)Math.Max(0, Math.Sqrt(u) * Math.Cos(v) * 0.3 + 0.5);
        }

        private float rand(float range)
        {
            return random.NextFloatDirection() * range;
        }

        //private Node _build(Node node, Vector2 pos, Vector2 vel, bool root) {
        //    keyPoints.Add(pos);
        //    cnt++;
        //    if (node.size < 0.1f || node.length < 1) return node;
        //    var r2 = (target - pos).ToRotation() - vel.ToRotation();
        //    var r = r2 + rand(MathHelper.Pi / 4f);
        //    var unit = (vel.ToRotation() + r).ToRotationVector2();
        //    Node rchild = new Node(r, node.size * 0.9f, node.length);
        //    闪电树主节点（树干）
        //    node.children.Add(_build(rchild, pos + unit * node.length, unit, root));
        //    if (root) {
        //        if (rand() > 0.8f) {
        //            for (int i = 0; i < 1; i++) {
        //                r = rand(MathHelper.Pi / 3f);
        //                unit = (vel.ToRotation() + r).ToRotationVector2();
        //                Node child = new Node(r, rand() * node.size * 0.6f, node.length * 0.6f);
        //                node.children.Add(_build(child, pos + unit * node.length, unit, false));
        //            }
        //        }
        //    }
        //    return node;
        //}

        public void Draw(SpriteBatch sb, Vector2 pos, Vector2 vel, float factor)
        {
            //sb.End();
            //sb.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            //_draw(sb, pos, vel, root, Color.Cyan * 0.4f, 8f, factor);
            _draw(sb, pos, vel, root, Color.White, 16f, factor);// * 0.6f
            //sb.End();
            //sb.Begin();
        }
        public void Draw(SpriteBatch sb, Texture2D tex, Rectangle frame, float width, Texture2D branch, Vector2 pos, Vector2 vel, float factor)
        {
            _draw(sb, tex, frame, width, branch, pos, vel, root, Color.White, factor);
        }
        public void Draw(SpriteBatch sb, Texture2D branch, Vector2 pos, Vector2 vel, Color c, float width, float factor)
        {
            _draw(sb, branch, pos, vel, root, c, width, factor);
        }
        public void Draw(SpriteBatch sb, Vector2 pos, Vector2 vel, Color c, float width, float factor)
        {
            _draw(sb, pos, vel, root, c, width, factor);
        }
        public void SpawnDust(Vector2 pos, Vector2 vel)
        {
            _dust(pos, vel, root);
        }
        private void _draw(SpriteBatch sb, Texture2D branch, Vector2 pos, Vector2 vel, Node node, Color c, float width, float factor)
        {
            // 树枝实际的方向向量
            Vector2 unit = (vel.ToRotation() + node.rad).ToRotationVector2();
            // 类似激光的线性绘制方法，绘制出树枝

            //TODO 绘制树

            //for (float i = 0; i <= node.length; i += 0.3f)
            //    sb.Draw(Main.magicPixel, pos + unit * i, new Rectangle(0, 0, 1, 1), c, 0,
            //        new Vector2(0.5f, 0.5f), Math.Max(node.size * factor, 0.3f), SpriteEffects.None, 0f);
            // 递归到子节点进行绘制
            var _fac = MathHelper.Clamp(factor, 0, 1);
            try
            {
                if (!TextureAssets.Tile[5].IsLoaded) Main.instance.LoadTiles(5);

            }
            catch { }
            var tex = TextureAssets.Tile[5].Value;
            sb.Draw(tex, pos + unit * node.length * .5f * _fac, new Rectangle(0, 0, 16, 20), c, vel.ToRotation() + node.rad + MathHelper.PiOver2, new Vector2(8, 10 * _fac), new Vector2(width * MathF.Sqrt(node.size) / 16f, node.length / 20f * 1.05f) * _fac, 0, 0);
            //var tex = TextureAssets.Item[664].Value;
            //sb.Draw(tex, pos + unit * node.length * .5f * _fac, null, c, vel.ToRotation() + node.rad, new Vector2(8 * _fac, 8), new Vector2(node.length / 16f, width * node.size / 16f) * _fac, 0, 0);
            //sb.Draw(TextureAssets.MagicPixel.Value, pos + unit * node.length * .5f * _fac, new Rectangle(0, 0, 1, 1), c, vel.ToRotation() + node.rad, new Vector2(.5f * _fac, .5f), new Vector2(node.length, width * node.size) * _fac, 0, 0);
            if (branch != null && (node.children == null || node.children.Count == 0))
            {
                _fac = MathHelper.Clamp(factor - 1, 0, 1);
                //tex = branch ?? TextureAssets.TreeBranch[9].Value;
                sb.Draw(branch, pos + unit * node.length, new Rectangle(42, 0, 42, 42), c, vel.ToRotation() + node.rad, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 4f, 0, 0);
                sb.Draw(branch, pos + unit * node.length, new Rectangle(42, 0, 42, 42), c, vel.ToRotation() + node.rad + MathHelper.Pi / 6, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 8f, 0, 0);
                sb.Draw(branch, pos + unit * node.length, new Rectangle(42, 0, 42, 42), c, vel.ToRotation() + node.rad - MathHelper.Pi / 6, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 8f, 0, 0);

            }
            else
                foreach (var child in node.children)
                {
                    // 传递给子节点真实的位置和方向向量
                    if (factor > 1)
                        _draw(sb, branch, pos + unit * node.length, unit, child, c, width, factor - 1);//.RotatedBy(MathF.Cos((float)Main.time / 24f) * MathHelper.PiOver4 / 3)
                    factor -= .1f;
                }
        }
        private void _draw(SpriteBatch sb, Texture2D tex, Rectangle frame, float width, Texture2D branch, Vector2 pos, Vector2 vel, Node node, Color c, float factor)
        {
            // 树枝实际的方向向量
            Vector2 unit = (vel.ToRotation() + node.rad).ToRotationVector2();
            // 类似激光的线性绘制方法，绘制出树枝

            //TODO 绘制树

            //for (float i = 0; i <= node.length; i += 0.3f)
            //    sb.Draw(Main.magicPixel, pos + unit * i, new Rectangle(0, 0, 1, 1), c, 0,
            //        new Vector2(0.5f, 0.5f), Math.Max(node.size * factor, 0.3f), SpriteEffects.None, 0f);
            // 递归到子节点进行绘制
            var _fac = MathHelper.Clamp(factor, 0, 1);
            //var tex = TextureAssets.Tile[5].Value;
            sb.Draw(tex, pos + unit * node.length * .5f * _fac, frame, c, vel.ToRotation() + node.rad + MathHelper.PiOver2, new Vector2(frame.Width * .5f, frame.Height * .5f * _fac), new Vector2(width * MathF.Sqrt(node.size) / frame.Width, node.length / frame.Height * 1.05f) * _fac, 0, 0);
            //var tex = TextureAssets.Item[664].Value;
            //sb.Draw(tex, pos + unit * node.length * .5f * _fac, null, c, vel.ToRotation() + node.rad, new Vector2(8 * _fac, 8), new Vector2(node.length / 16f, width * node.size / 16f) * _fac, 0, 0);
            //sb.Draw(TextureAssets.MagicPixel.Value, pos + unit * node.length * .5f * _fac, new Rectangle(0, 0, 1, 1), c, vel.ToRotation() + node.rad, new Vector2(.5f * _fac, .5f), new Vector2(node.length, width * node.size) * _fac, 0, 0);
            if (node.children == null || node.children.Count == 0)
            {
                _fac = MathHelper.Clamp(factor - 1, 0, 1);
                sb.Draw(branch, pos + unit * node.length, new Rectangle(42, 0, 42, 42), c, vel.ToRotation() + node.rad, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 4f, 0, 0);
                sb.Draw(branch, pos + unit * node.length, new Rectangle(42, 0, 42, 42), c, vel.ToRotation() + node.rad + MathHelper.Pi / 6, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 8f, 0, 0);
                sb.Draw(branch, pos + unit * node.length, new Rectangle(42, 0, 42, 42), c, vel.ToRotation() + node.rad - MathHelper.Pi / 6, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 8f, 0, 0);
            }
            else
                foreach (var child in node.children)
                {
                    // 传递给子节点真实的位置和方向向量
                    if (factor > 1)
                        _draw(sb, tex, frame, width, branch, pos + unit * node.length, unit, child, c, factor - 1);//
                    factor -= .1f;
                }
        }

        private void _draw(SpriteBatch sb, Vector2 pos, Vector2 vel, Node node, Color c, float width, float factor)
        {
            //// 树枝实际的方向向量
            //Vector2 unit = (vel.ToRotation() + node.rad).ToRotationVector2();
            //// 类似激光的线性绘制方法，绘制出树枝

            ////TODO 绘制树

            ////for (float i = 0; i <= node.length; i += 0.3f)
            ////    sb.Draw(Main.magicPixel, pos + unit * i, new Rectangle(0, 0, 1, 1), c, 0,
            ////        new Vector2(0.5f, 0.5f), Math.Max(node.size * factor, 0.3f), SpriteEffects.None, 0f);
            //// 递归到子节点进行绘制
            //var _fac = MathHelper.Clamp(factor, 0, 1);
            ////var tex = TextureAssets.Tile[5].Value;
            ////sb.Draw(tex, pos + unit * node.length * .5f * _fac, new Rectangle(0, 0, 16, 20), c, vel.ToRotation() + node.rad + MathHelper.PiOver2, new Vector2(8, 10 * _fac), new Vector2(width * MathF.Sqrt(node.size) / 16f, node.length / 20f * 1.05f) * _fac, 0, 0);
            ////var tex = TextureAssets.Item[664].Value;
            ////sb.Draw(tex, pos + unit * node.length * .5f * _fac, null, c, vel.ToRotation() + node.rad, new Vector2(8 * _fac, 8), new Vector2(node.length / 16f, width * node.size / 16f) * _fac, 0, 0);
            //sb.Draw(TextureAssets.MagicPixel.Value, pos + unit * node.length * .5f * _fac, new Rectangle(0, 0, 1, 1), c, vel.ToRotation() + node.rad, new Vector2(.5f * _fac, .5f), new Vector2(node.length, width * node.size) * _fac, 0, 0);
            ////if (node.children == null || node.children.Count == 0)
            ////{
            ////    _fac = MathHelper.Clamp(factor - 1, 0, 1);
            ////    tex = TextureAssets.TreeBranch[9].Value;

            ////    sb.Draw(tex, pos + unit * node.length, new Rectangle(42, 0, 42, 42), c, vel.ToRotation() + node.rad, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 8f, 0, 0);

            ////}
            ////else
            //foreach (var child in node.children)
            //{
            //    // 传递给子节点真实的位置和方向向量
            //    if (factor > 1)
            //        _draw(sb, pos + unit * node.length, unit, child, c, width, factor - 1);//
            //    factor -= .1f;
            //}

            // 树枝实际的方向向量
            Vector2 unit = (vel.ToRotation() + node.rad).ToRotationVector2();
            var _fac = MathHelper.Clamp(factor, 0, 1);
            var tex = TextureAssets.Tile[5].Value;
            var branch = node.leafTex;
            sb.Draw(tex, pos + unit * node.length * .5f * _fac, new Rectangle(0, 0, 16, 20), c, vel.ToRotation() + node.rad + MathHelper.PiOver2, new Vector2(8, 10 * _fac), new Vector2(width * MathF.Sqrt(node.size) / 16f, node.length / 20f * 1.05f) * _fac, 0, 0);
            if (branch != null && (node.children == null || node.children.Count == 0))
            {
                _fac = MathHelper.Clamp(factor - .33f, 0, 1);
                //tex = branch ?? TextureAssets.TreeBranch[9].Value;
                sb.Draw(branch, pos + unit * node.length, node.leafFrame, c, vel.ToRotation() + node.rad, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 4f, 0, 0);
                sb.Draw(branch, pos + unit * node.length, node.leafFrame, c, vel.ToRotation() + node.rad + MathHelper.Pi / 6, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 8f, 0, 0);
                sb.Draw(branch, pos + unit * node.length, node.leafFrame, c, vel.ToRotation() + node.rad - MathHelper.Pi / 6, new Vector2(2, 22), width * MathF.Sqrt(node.size) * _fac / 8f, 0, 0);

            }
            else
                foreach (var child in node.children)
                {
                    // 传递给子节点真实的位置和方向向量
                    if (factor > 1)
                        _draw(sb, pos + unit * node.length, unit, child, c, width, factor - 1);//.RotatedBy(MathF.Cos((float)Main.time / 24f) * MathHelper.PiOver4 / 3)
                    factor -= .1f;
                }
        }

        private void _dust(Vector2 pos, Vector2 vel, Node node)
        {
            float r = vel.ToRotation();
            Vector2 unit = (r + node.rad).ToRotationVector2();
            for (float i = 0; i <= node.length; i += 4f)
            {
                var dust = Dust.NewDustDirect(pos + unit * i, 0, 0,
                    MyDustId.Wood, 0, 0, 100, Color.White, 1.5f);
                //dust.noGravity = true;
                dust.velocity *= 0.25f;
                dust.velocity += new Vector2(rand() * 2 - 1, 2);

                dust.position = pos + unit * i;
            }
            foreach (var child in node.children)
            {
                _dust(pos + unit * node.length, unit, child);
            }
        }
        public bool Check(Rectangle hitbox, Vector2 pos, Vector2 vel, float factor)
        {
            return _check(hitbox, pos, vel, root, factor);
        }
        private bool _check(Rectangle hitbox, Vector2 pos, Vector2 vel, Node node, float factor)
        {
            Vector2 unit = (vel.ToRotation() + node.rad).ToRotationVector2();
            var _fac = MathHelper.Clamp(factor, 0, 1);
            if (hitbox.Contains(pos.ToPoint()) || hitbox.Contains((pos + unit * node.length * .5f * _fac).ToPoint())) return true;
            else
                foreach (var child in node.children)
                {
                    if (factor < 1) return false;
                    if (_check(hitbox, pos + unit * node.length, unit.RotatedBy(MathF.Cos((float)Main.time / 24f) * MathHelper.PiOver4 / 3), child, factor - 1)) return true;
                    factor -= .1f;
                }
            return false;
        }
        public bool Check(Rectangle hitbox)
        {
            foreach (var pt in keyPoints)
                if (hitbox.Contains(pt.ToPoint())) return true;
            return false;
        }
    }
    public class VertexTriangle
    {
        public Vector2 position0;
        public Vector2 position1;
        public Vector2 position2;
        public void GenerateFractal(List<VertexTriangle> list, int counter)
        {
            if (counter > 0)
            {
                var center = position0 + position1 + position2;
                center /= 3f;

                var _011 = position0 * 2 / 3f + position1 * 1 / 3f;
                var _012 = position0 * 1 / 3f + position1 * 2 / 3f;

                var _121 = position1 * 2 / 3f + position2 * 1 / 3f;
                var _122 = position1 * 1 / 3f + position2 * 2 / 3f;

                var _201 = position2 * 2 / 3f + position0 * 1 / 3f;
                var _202 = position2 * 1 / 3f + position0 * 2 / 3f;

                list.Add(new VertexTriangle(position0, _011, _202));
                list.Add(new VertexTriangle(center, _011, _202));
                list.Add(new VertexTriangle(position1, _012, _121));
                list.Add(new VertexTriangle(center, _012, _121));
                list.Add(new VertexTriangle(position2, _122, _201));
                list.Add(new VertexTriangle(center, _122, _201));

                new VertexTriangle(_011, _012, center.Symmetric(_011, _012)).GenerateFractal(list, counter - 1);
                new VertexTriangle(_121, _122, center.Symmetric(_121, _122)).GenerateFractal(list, counter - 1);
                new VertexTriangle(_201, _202, center.Symmetric(_201, _202)).GenerateFractal(list, counter - 1);

            }
            else
            {
                list.Add(this);
            }
        }
        public void GenerateFractal(List<VertexTriangle> list, float counter)
        {
            if (counter > 0)
            {
                var center = position0 + position1 + position2;
                center /= 3f;

                var _011 = position0 * 2 / 3f + position1 * 1 / 3f;
                var _012 = position0 * 1 / 3f + position1 * 2 / 3f;

                var _121 = position1 * 2 / 3f + position2 * 1 / 3f;
                var _122 = position1 * 1 / 3f + position2 * 2 / 3f;

                var _201 = position2 * 2 / 3f + position0 * 1 / 3f;
                var _202 = position2 * 1 / 3f + position0 * 2 / 3f;

                list.Add(new VertexTriangle(position0, _011, _202));
                list.Add(new VertexTriangle(center, _011, _202));
                list.Add(new VertexTriangle(position1, _012, _121));
                list.Add(new VertexTriangle(center, _012, _121));
                list.Add(new VertexTriangle(position2, _122, _201));
                list.Add(new VertexTriangle(center, _122, _201));

                if (counter > 1)
                {
                    new VertexTriangle(_011, _012, center.Symmetric(_011, _012)).GenerateFractal(list, counter - 1);
                    new VertexTriangle(_121, _122, center.Symmetric(_121, _122)).GenerateFractal(list, counter - 1);
                    new VertexTriangle(_201, _202, center.Symmetric(_201, _202)).GenerateFractal(list, counter - 1);
                }
                else
                {
                    var fac = (1 - MathF.Cos(MathHelper.Pi * (1 - counter))) * .5f;
                    list.Add(new VertexTriangle(_011, _012, Vector2.Lerp(center.Symmetric(_011, _012), center, fac)));
                    list.Add(new VertexTriangle(_121, _122, Vector2.Lerp(center.Symmetric(_121, _122), center, fac)));
                    list.Add(new VertexTriangle(_201, _202, Vector2.Lerp(center.Symmetric(_201, _202), center, fac)));
                }

            }
            else
            {
                list.Add(this);
            }
        }
        public VertexTriangle(Vector2 vector0, Vector2 vector1, Vector2 vector2)
        {
            position0 = vector0;
            position1 = vector1;
            position2 = vector2;
        }
    }

    /// <summary>
    /// DXTsT自制的粒子ID表
    /// 制作时间：2017/1/31
    /// 版权所有：DXTsT & 四十九落星制作组
    /// 
    /// 说明：以下字段带有（！）标识符的说明此粒子效果会在黑暗中自发光
    /// 带有（.）标识符说明此粒子效果会高亮显示但是不会发光
    /// 其余Dust全部都不会发光！
    /// </summary>
    public static class MyDustId
    {
        /// <summary>
        /// brown dirt
        /// </summary>
        public const int BrownDirt = 0;
        /// <summary>
        /// grey stone
        /// </summary>
        public const int GreyStone = 1;
        /// <summary>
        /// thick green grass
        /// </summary>
        public const int GreenGrass = 2;
        /// <summary>
        /// thin green grass
        /// </summary>
        public const int ThinGreenGrass = 3;
        /// <summary>
        /// grey pebbles
        /// </summary>
        public const int GreyPebble = 4;
        /// <summary>
        /// red blood
        /// </summary>
        public const int RedBlood = 5;
        /// <summary>
        /// (!)orange fire, emits orange light !WARNING
        /// </summary>
        public const int Fire = 6;
        /// <summary>
        /// brown wood
        /// </summary>
        public const int Wood = 7;
        /// <summary>
        /// purple gems
        /// </summary>
        public const int PurpleGems = 8;
        /// <summary>
        /// orange gems
        /// </summary>
        public const int OrangeGems = 9;
        /// <summary>
        /// yellow gems
        /// </summary>
        public const int YellowGems = 10;
        /// <summary>
        /// white gems
        /// </summary>
        public const int WhiteGems = 11;
        /// <summary>
        /// red gems
        /// </summary>
        public const int RedGems = 12;
        /// <summary>
        /// cyan gems
        /// </summary>
        public const int CyanGems = 13;
        /// <summary>
        /// purple corruption particle with no gravity
        /// </summary>
        public const int CorruptionParticle = 14;
        /// <summary>
        /// (!)white amd blue magic fx, emits pale blue light
        /// </summary>
        public const int BlueMagic = 15;
        /// <summary>
        /// bluish white clouds like hermes boots
        /// </summary>
        public const int WhiteClouds = 16;
        /// <summary>
        /// thin grey material
        /// </summary>
        public const int ThinGrey = 17;
        /// <summary>
        /// thin sickly green material
        /// </summary>
        public const int SicklyGreen = 18;
        /// <summary>
        /// thin yellow material
        /// </summary>
        public const int ThinYellow = 19;
        /// <summary>
        /// (!)white lingering, emits cyan light
        /// </summary>
        public const int WhiteLingering = 20;
        /// <summary>
        /// (!)purple lingering, emits purple light
        /// </summary>
        public const int PurpleLingering = 21;
        /// <summary>
        /// brown material
        /// </summary>
        public const int Brown = 22;
        /// <summary>
        /// orange material
        /// </summary>
        public const int orange = 23;
        /// <summary>
        /// thin brown material
        /// </summary>
        public const int ThinBrown = 24;
        /// <summary>
        /// copper
        /// </summary>
        public const int Copper = 25;
        /// <summary>
        /// iron
        /// </summary>
        public const int iron = 26;
        /// <summary>
        /// (!)purple fx, emits bright purple light
        /// </summary>
        public const int PurpleLight = 27;
        /// <summary>
        /// dull copper
        /// </summary>
        public const int DullCopper = 28;
        /// <summary>
        /// (!)dark blue, emits pale pink light !WARNING
        /// </summary>
        public const int DarkBluePinkLight = 29;
        /// <summary>
        /// silver material
        /// </summary>
        public const int Silver = 30;
        /// <summary>
        /// yellowish white cloud material
        /// </summary>
        public const int Smoke = 31;
        /// <summary>
        /// yellow sand
        /// </summary>
        public const int Sand = 32;
        /// <summary>
        /// water, highly transparent
        /// </summary>
        public const int Water = 33;
        /// <summary>
        /// (!)red fx, emits red light !WARNING
        /// </summary>
        public const int RedLight = 35;
        /// <summary>
        /// muddy pale material
        /// </summary>
        public const int MuddyPale = 36;
        /// <summary>
        /// dark grey material
        /// </summary>
        public const int DarkGrey = 37;
        /// <summary>
        /// muddy brown material
        /// </summary>
        public const int MuddyBrown = 38;
        /// <summary>
        /// bright green jungle grass
        /// </summary>
        public const int JungleGrass = 39;
        /// <summary>
        /// bright green thin grass
        /// </summary>
        public const int ThinGrass = 40;
        /// <summary>
        /// (!)dark blue wandering circles, emits bright cyan light !WARNING
        /// </summary>
        public const int BlueCircle = 41;
        /// <summary>
        /// thin teal material
        /// </summary>
        public const int ThinTeal = 42;
        /// <summary>
        /// (!)bright green spores that lingers for a while, emits light green light
        /// </summary>
        public const int GreenSpores = 44;
        /// <summary>
        /// (!)light blue circles, emits purple light
        /// </summary>
        public const int LightBlueCircle = 45;
        /// <summary>
        /// green material with no gravity
        /// </summary>
        public const int GreenMaterial = 46;
        /// <summary>
        /// thin cyan grass
        /// </summary>
        public const int CyanGrass = 47;
        /// <summary>
        /// pink water, highly transparent
        /// </summary>
        public const int PinkWater = 52;
        /// <summary>
        /// grey material
        /// </summary>
        public const int GreyMaterial = 53;
        /// <summary>
        /// black material
        /// </summary>
        public const int BlackMaterial = 54;
        /// <summary>
        /// (!)bright orange thick fx, emits yellow light
        /// </summary>
        public const int OrangeFx = 55;
        /// <summary>
        /// (!)cyan fx, emits pale blue light
        /// </summary>
        public const int CyanFx = 56;
        /// <summary>
        /// (!)small yellow hallowed fx, emis yellow light
        /// </summary>
        public const int YellowHallowFx = 57;
        /// <summary>
        /// (!)hot and pale pink magic fx, emits pink light
        /// </summary>
        public const int PinkMagic = 58;
        /// <summary>
        /// (!)blue torch, emits pure blue light !WARNING
        /// </summary>
        public const int BlueTorch = 59;
        /// <summary>
        /// (!)red torch, emits pure red light !WARNING
        /// </summary>
        public const int RedTorch = 60;
        /// <summary>
        /// (!)green torch, emits pure green light !WARNING
        /// </summary>
        public const int GreenTorch = 61;
        /// <summary>
        /// (!)purple torch, emits purple light !WARNING
        /// </summary>
        public const int PurpleTorch = 62;
        /// <summary>
        /// (!)white torch, emits bright white light !WARNING
        /// </summary>
        public const int WhiteTorch = 63;
        /// <summary>
        /// (!)yellow torch, emits deep yellow light !WARNING
        /// </summary>
        public const int YellowTorch = 64;
        /// <summary>
        /// (!)demon torch, emits pulsating pink/purple light !WARNING
        /// </summary>
        public const int DemonTorch = 65;
        /// <summary>
        /// (!)White transparent !WARNING
        /// </summary>
        public const int WhiteTransparent = 66;
        /// <summary>
        /// (!)cyan ice crystals, emits cyan light
        /// </summary>
        public const int CyanIce = 67;
        /// <summary>
        /// (.)dark cyan ice crystals, emits very faint blue light, glows in disabled gravity
        /// </summary>
        public const int DarkCyanIce = 68;
        /// <summary>
        /// thin pink material
        /// </summary>
        public const int ThinPink = 69;
        /// <summary>
        /// (.)thin transparent purple material, emits faint purple light, glows in disabled gravity
        /// </summary>
        public const int TransparentPurple = 70;
        /// <summary>
        /// (!)transparent pink fx, emits faint pink light
        /// </summary>
        public const int TransparentPinkFx = 71;
        /// <summary>
        /// (!)solid pink fx, emits faint pink light
        /// </summary>
        public const int SolidPinkFx = 72;
        /// <summary>
        /// (!)solid bright pink fx, emits pink light
        /// </summary>
        public const int BrightPinkFx = 73;
        /// <summary>
        /// (!)solid bright green fx, emits green light
        /// </summary>
        public const int BrightGreenFx = 74;
        /// <summary>
        /// (!)green cursed torch !WARNING
        /// </summary>
        public const int CursedFire = 75;
        /// <summary>
        /// snowfall, lasts a long time
        /// </summary>
        public const int Snow = 76;

        /// <summary>
        /// thin grey material
        /// </summary>
        public const int ThinGrey1 = 77;
        /// <summary>
        /// thin copper material
        /// </summary>
        public const int ThinCopper = 78;
        /// <summary>
        /// thin yellow material
        /// </summary>
        public const int ThinYellow1 = 79;
        /// <summary>
        /// ice block material
        /// </summary>
        public const int IceBlock = 80;
        /// <summary>
        /// iron material
        /// </summary>
        public const int Iron = 81;
        /// <summary>
        /// silty material
        /// </summary>
        public const int Silty = 82;
        /// <summary>
        /// sickly green material
        /// </summary>
        public const int SicklyGreen1 = 83;
        /// <summary>
        /// bluish grey material
        /// </summary>
        public const int BluishGrey = 84;
        /// <summary>
        /// thin sandy materiial
        /// </summary>
        public const int ThinSandy = 85;
        /// <summary>
        /// (!)transparent pink material, emits pink light
        /// </summary>
        public const int PinkTrans = 86;
        /// <summary>
        /// (!)transparent yellow material, emits yellow light
        /// </summary>
        public const int YellowTrans = 87;
        /// <summary>
        /// (!)transparent blue material, emits blue light
        /// </summary>
        public const int BlueTrans = 88;
        /// <summary>
        /// (!)transparent green material, emits green light
        /// </summary>
        public const int GreenTrans = 89;
        /// <summary>
        /// (!)transparent red material, emits red light
        /// </summary>
        public const int RedTrans = 90;
        /// <summary>
        /// (!)transparent white material, emits white light
        /// </summary>
        public const int WhiteTrans = 91;
        /// <summary>
        /// (!)transparent cyan material, emits cyan light
        /// </summary>
        public const int CyanTrans = 92;
        /// <summary>
        /// thin dark green grass
        /// </summary>
        public const int DarkGrass = 93;
        /// <summary>
        /// thin pale dark green grass
        /// </summary>
        public const int PaleDarkGrass = 94;
        /// <summary>
        /// thin dark red grass
        /// </summary>
        public const int DarkRedGrass = 95;
        /// <summary>
        /// thin blackish green grass
        /// </summary>
        public const int BlackGreenGrass = 96;
        /// <summary>
        /// thin dark red grass
        /// </summary>
        public const int DarkRedGrass1 = 97;
        /// <summary>
        /// purple water, highly transparent
        /// </summary>
        public const int PurpleWater = 98;
        /// <summary>
        /// cyan water, highly transparent
        /// </summary>
        public const int CyanWater = 99;
        /// <summary>
        /// pink water, highly transparent
        /// </summary>
        public const int PinkWater1 = 100;
        /// <summary>
        /// cyan water, highly transparent
        /// </summary>
        public const int CyanWater1 = 101;
        /// <summary>
        /// orange water, highly transparent
        /// </summary>
        public const int OrangeWater = 102;
        /// <summary>
        /// dark blue water, highly transparent
        /// </summary>
        public const int DarkBlueWater = 103;
        /// <summary>
        /// hot pink water, highly transparent
        /// </summary>
        public const int HotPinkWater = 104;
        /// <summary>
        /// red water, highly transparent
        /// </summary>
        public const int RedWater = 105;
        /// <summary>
        /// (.)transparent red/green/blue material, glows in the dark
        /// </summary>
        public const int RgbMaterial = 106;
        /// <summary>
        /// (!)short green powder, emits green light
        /// </summary>
        public const int GreenFXPowder = 107;
        /// <summary>
        /// light pale purple round material
        /// </summary>
        public const int PurpleRound = 108;
        /// <summary>
        /// black material
        /// </summary>
        public const int BlackMaterial1 = 109;
        /// <summary>
        /// (.)bright green bubbles, emits very faint green light
        /// </summary>
        public const int GreenBubble = 110;
        /// <summary>
        /// (.)bright cyan bubbles, emits very faint cyan light
        /// </summary>
        public const int CyanBubble = 111;
        /// <summary>
        /// (.)bright pink bubbles, emits very faint pink light
        /// </summary>
        public const int PinkBubble = 112;
        /// <summary>
        /// (.)blue ice crystals, glows in the dark
        /// </summary>
        public const int BlueIce = 113;
        /// <summary>
        /// (.)bright pink/yellow bubbles, emits very faint pink light
        /// </summary>
        public const int PinkYellowBubble = 114;
        /// <summary>
        /// red grass
        /// </summary>
        public const int RedGrass = 115;
        /// <summary>
        /// blueish green grass
        /// </summary>
        public const int BlueGreenGrass = 116;
        /// <summary>
        /// red grass
        /// </summary>
        public const int RedGrass1 = 117;
        /// <summary>
        /// purple gems
        /// </summary>
        public const int PurpleGems1 = 118;
        /// <summary>
        /// pink gems
        /// </summary>
        public const int PinkGems = 119;
        /// <summary>
        /// pale pink gems
        /// </summary>
        public const int PalePinkGems = 120;
        /// <summary>
        /// thin grey material
        /// </summary>
        public const int ThinGrey2 = 121;
        /// <summary>
        /// thin iron material
        /// </summary>
        public const int ThinIron = 122;
        /// <summary>
        /// hot pink bubble material
        /// </summary>
        public const int HotPinkBubble = 123;
        /// <summary>
        /// yellowish white bubbles
        /// </summary>
        public const int YellowWhiteBubble = 124;
        /// <summary>
        /// thin red material
        /// </summary>
        public const int ThinRed = 125;
        /// <summary>
        /// thin grey material
        /// </summary>
        public const int ThinGrey3 = 126;
        /// <summary>
        /// (!)reddish orange fire, emits orange light
        /// </summary>
        public const int OrangeFire = 127;
        /// <summary>
        /// green gems
        /// </summary>
        public const int GreenGems = 128;
        /// <summary>
        /// thin brown material
        /// </summary>
        public const int ThinBrown1 = 129;
        /// <summary>
        /// (!)trailing red falling fireworks, emits red light
        /// </summary>
        public const int TrailingRed = 130;
        /// <summary>
        /// (!)trailing green rising fireworks, emits green light
        /// </summary>
        public const int TrailingGreen = 131;
        /// <summary>
        /// (!)trailing cyan falling fireworks, emits cyan light
        /// </summary>
        public const int TrailingCyan = 132;
        /// <summary>
        /// (!)trailing yellow falling fireworks, emits cyan light
        /// </summary>
        public const int TrailingYellow = 133;
        /// <summary>
        /// trailing pink falling fireworks
        /// </summary>
        public const int TrailingPink = 134;
        /// <summary>
        /// (!)cyan ice torch, emits cyan light !WARNING
        /// </summary>
        public const int IceTorch = 135;
        /// <summary>
        /// red material
        /// </summary>
        public const int Red = 136;
        /// <summary>
        /// bright blue/cyan material
        /// </summary>
        public const int BrightCyan = 137;
        /// <summary>
        /// bright orange/brown material
        /// </summary>
        public const int BrightOrange = 138;
        /// <summary>
        /// cyan confetti
        /// </summary>
        public const int CyanConfetti = 139;
        /// <summary>
        /// green confetti
        /// </summary>
        public const int GreenConfetti = 140;
        /// <summary>
        /// pink confetti
        /// </summary>
        public const int PinkConfetti = 141;
        /// <summary>
        /// yellow confetti
        /// </summary>
        public const int YellowConfetti = 142;
        /// <summary>
        /// light grey stone
        /// </summary>
        public const int LightGreyStone = 143;
        /// <summary>
        /// vivid copper stone
        /// </summary>
        public const int CopperStone = 144;
        /// <summary>
        /// pink stone
        /// </summary>
        public const int PinkStone = 145;
        /// <summary>
        /// green/brown material mix
        /// </summary>
        public const int GreenBrown = 146;
        /// <summary>
        /// orange material
        /// </summary>
        public const int Orange = 147;
        /// <summary>
        /// desaturated red material
        /// </summary>
        public const int RedDesaturated = 148;
        /// <summary>
        /// white material
        /// </summary>
        public const int White = 149;
        /// <summary>
        /// black/yellow/bluishwhite material
        /// </summary>
        public const int BlackYellowBluishwhite = 150;
        /// <summary>
        /// thin white material
        /// </summary>
        public const int ThinWhite = 151;
        /// <summary>
        /// (!)bright orange bubbles !WARNING
        /// </summary>
        public const int OrangeBubble = 152;
        /// <summary>
        /// bright orange bubble material
        /// </summary>
        public const int OrangeBubbleMaterial = 153;
        /// <summary>
        /// pale blue thin material
        /// </summary>
        public const int BlueThin = 154;
        /// <summary>
        /// thin dark brown material
        /// </summary>
        public const int DarkBrown = 155;
        /// <summary>
        /// (!)bright blue/white bubble material, emits pale blue light
        /// </summary>
        public const int BlueWhiteBubble = 156;
        /// <summary>
        /// (.)thin green fx, glows in the dark
        /// </summary>
        public const int GreenFx = 157;
        /// <summary>
        /// (!)orange fire, emits orange light !WARNING
        /// </summary>
        public const int OrangeFire1 = 158;
        /// <summary>
        /// (!)flickering yellow fx, emits yellow light !WARNING
        /// </summary>
        public const int YellowFx = 159;
        /// <summary>
        /// (!)shortlived cyan fx, emits bright cyan light
        /// </summary>
        public const int CyanShortFx = 160;
        /// <summary>
        /// cyan material
        /// </summary>
        public const int CyanMaterial = 161;
        /// <summary>
        /// (!)shortlived orange fx, emits bright orange light
        /// </summary>
        public const int OrangeShortFx = 162;
        /// <summary>
        /// (.)bright green thin material, glows in the dark
        /// </summary>
        public const int BrightGreen = 163;
        /// <summary>
        /// (!)flickering pink fx, emits hot pink light !WARNING
        /// </summary>
        public const int PinkFx = 164;
        /// <summary>
        /// white/blue bubble material
        /// </summary>
        public const int WhiteBlueBubble = 165;
        /// <summary>
        /// thin bright pink material
        /// </summary>
        public const int PinkThinBright = 166;
        /// <summary>
        /// thin green material
        /// </summary>
        public const int ThinGreen = 167;
        /// <summary>
        /// !bright pink bubbles !WARNING
        /// </summary>
        public const int PinkBrightBubble = 168;
        /// <summary>
        /// (!)yellow fx, emits deep yellow light !WARNING
        /// </summary>
        public const int YellowFx1 = 169;
        /// <summary>
        /// (.)thin orange fx, emits faint white light
        /// </summary>
        public const int Ichor = 170;
        /// <summary>
        /// bright purple bubble material
        /// </summary>
        public const int PurpleBubble = 171;
        /// <summary>
        /// (.)light blue particles, emits faint blue light
        /// </summary>
        public const int BlueParticle = 172;
        /// <summary>
        /// (!)shortlived purple fx, emits bright purple light
        /// </summary>
        public const int PurpleShortFx = 173;
        /// <summary>
        /// (!)bright orange bubble material, emits reddish orange light
        /// </summary>
        public const int OrangeFire2 = 174;
        /// <summary>
        /// (.)shortlived white fx, glows in the dark
        /// </summary>
        public const int WhiteShortFx = 175;
        /// <summary>
        /// light blue particles
        /// </summary>
        public const int LightBlueParticle = 176;
        /// <summary>
        /// light pink particles
        /// </summary>
        public const int LightPinkParticle = 177;
        /// <summary>
        /// light green particles
        /// </summary>
        public const int LightGreenParticle = 178;
        /// <summary>
        /// light purple particles
        /// </summary>
        public const int LightPurpleParticle = 179;
        /// <summary>
        /// (!)light cyan particles, glows in the dark
        /// </summary>
        public const int LightCyanParticle = 180;
        /// <summary>
        /// (.)light cyan/pink bubble material, glows in the dark
        /// </summary>
        public const int CyanPinkBubble = 181;
        /// <summary>
        /// (.)light red bubble material, barely emits red light
        /// </summary>
        public const int RedBubble = 182;
        /// <summary>
        /// (.)transparent red bubble material, glows in the dark
        /// </summary>
        public const int RedTransBubble = 183;
        /// <summary>
        /// sickly pale greenish grey particles that stay in place
        /// </summary>
        public const int GreenishGreyParticle = 184;
        /// <summary>
        /// (!)light cyan crystal material, emits cyan light
        /// </summary>
        public const int CyanCrystal = 185;
        /// <summary>
        /// pale dark blue smoke
        /// </summary>
        public const int DarkBlueSmoke = 186;
        /// <summary>
        /// (!)light cyan particles, emits cyan light
        /// </summary>
        public const int LightCyanParticle1 = 187;
        /// <summary>
        /// bright green bubbles
        /// </summary>
        public const int GreenBubble1 = 188;
        /// <summary>
        /// thin orange material
        /// </summary>
        public const int OrangeMaterial = 189;
        /// <summary>
        /// thin gold material
        /// </summary>
        public const int GoldMaterial = 190;
        /// <summary>
        /// black flakes
        /// </summary>
        public const int BlackFlakes = 191;
        /// <summary>
        /// snow material
        /// </summary>
        public const int SnowMaterial = 192;
        /// <summary>
        /// green material
        /// </summary>
        public const int GreenMaterial1 = 193;
        /// <summary>
        /// thin brown material
        /// </summary>
        public const int BrownMaterial = 194;
        /// <summary>
        /// thin black material
        /// </summary>
        public const int BlackMaterial2 = 195;
        /// <summary>
        /// thin green material
        /// </summary>
        public const int ThinGreen1 = 196;
        /// <summary>
        /// (.)thin bright cyan material, glows in the dark
        /// </summary>
        public const int BrightCyanMaterial = 197;
        /// <summary>
        /// black/white particles
        /// </summary>
        public const int BlackWhiteParticle = 198;
        /// <summary>
        /// pale purple/black/grey particles
        /// </summary>
        public const int PurpleBlackGrey = 199;
        /// <summary>
        /// pink particles
        /// </summary>
        public const int PinkParticle = 200;
        /// <summary>
        /// light pink particles
        /// </summary>
        public const int LightPinkParticle1 = 201;
        /// <summary>
        /// light cyan particles
        /// </summary>
        public const int LightCyanParticle2 = 202;
        /// <summary>
        /// grey particles
        /// </summary>
        public const int GreyParticle = 203;
        /// <summary>
        /// (.)white particles, glows in the dark
        /// </summary>
        public const int WhiteParticle = 204;
        /// <summary>
        /// (.)thin pink material, barely emits pink light
        /// </summary>
        public const int ThinPinkMaterial = 205;
        /// <summary>
        /// (!)shortlived cyan fx, emits bright blue light
        /// </summary>
        public const int CyanShortFx1 = 206;
        /// <summary>
        /// thin brown material
        /// </summary>
        public const int BrownMaterial1 = 207;
        /// <summary>
        /// orange stone
        /// </summary>
        public const int OrangeStone = 208;
        /// <summary>
        /// pale green stone
        /// </summary>
        public const int PaleGreenStone = 209;
        /// <summary>
        /// off white material
        /// </summary>
        public const int OffWhite = 210;
        /// <summary>
        /// bright blue particles
        /// </summary>
        public const int BrightBlueParticle = 211;
        /// <summary>
        /// white particles
        /// </summary>
        public const int WhiteParticle1 = 212;
        /// <summary>
        /// (.)shortlived tiny white fx, barely emits white light
        /// </summary>
        public const int WhiteShortFx1 = 213;
        /// <summary>
        /// thin pale brown material
        /// </summary>
        public const int Thin = 214;
        /// <summary>
        /// thin khaki material
        /// </summary>
        public const int ThinKhaki = 215;
        /// <summary>
        /// pale pink material
        /// </summary>
        public const int Pale = 216;
        /// <summary>
        /// cyan particles
        /// </summary>
        public const int Cyan = 217;
        /// <summary>
        /// hot pink particles
        /// </summary>
        public const int Hot = 218;
        /// <summary>
        /// (!)trailing red flying fireworks, emits orange light
        /// </summary>
        public const int TrailingRed1 = 219;
        /// <summary>
        /// (!)trailing green flying fireworks, emits green light
        /// </summary>
        public const int TrailingGreen1 = 220;
        /// <summary>
        /// (!)trailing blue flying fireworks, emits pale blue light
        /// </summary>
        public const int TrailingBlue = 221;
        /// <summary>
        /// (!)trailing yellow flying fireworks, emits yellow light
        /// </summary>
        public const int TrailingYellow1 = 222;
        /// <summary>
        /// (.)trailing red flying fireworks, glows in the dark
        /// </summary>
        public const int TrailingRed2 = 223;
        /// <summary>
        /// thin blue material
        /// </summary>
        public const int ThinBlue = 224;
        /// <summary>
        /// orange material
        /// </summary>
        public const int OrangeMaterial1 = 225;
        /// <summary>
        /// 
        /// </summary>
        public const int ElectricCyan = 226;

        /// <summary>
        /// (!)Lunar fire!!!
        /// </summary>
        public const int CyanLunarFire = 229;
        /// <summary>
        /// (!)flickering Purple fx, emits Purple light !WARNING
        /// </summary>
        public const int PurpleFx = 230;
    }
    public static class CoolerItemVisualEffectMethods
    {
        public static float Luminance(this Color color) => (Math.Max(Math.Max(color.R, color.G), color.B) + Math.Min(Math.Min(color.R, color.G), color.B)) / 2;
        public static float UpAndDown(this float t) => MathF.Acos(MathF.Sin(MathHelper.Pi * (2 * t + 0.5f))) / MathHelper.Pi;
        public static Vector2 Symmetric(this Vector2 target, Vector2 lineStart, Vector2 lineEnd)
        {
            var n = lineStart - lineEnd;
            n = new Vector2(-n.Y, n.X);
            return target + 2 * Vector2.Dot(n, lineStart - target) / n.LengthSquared() * n;
            //return lineStart + lineEnd - target;
        }
        public static float SmoothFloor(this float t)
        {
            t += .5f;
            var f = (int)Math.Floor(t);
            var g = MathF.Sin(MathHelper.TwoPi * t) / MathHelper.TwoPi + t - .5f;
            return MathHelper.Lerp(f, g, Math.Abs(f - g) * 2);
        }
        public static bool EqualValue<T>(this IList<T> list, IList<T> target)
        {
            var count = list.Count;
            if (count != target.Count) return false;
            for (int n = 0; n < count; n++)
            {
                if (!list[n].Equals(target[n])) return false;
            }
            return true;
        }
        public static Color GetLerpArrayValue(this float factor, params Color[] values)
        {
            if (factor <= 0)
            {
                return values[0];
            }
            else if (factor >= 1)
            {
                return values[values.Length - 1];
            }
            else
            {
                int c = values.Length - 1;
                int tier = (int)(c * factor);
                return Color.Lerp(values[tier], values[tier + 1], c * factor % 1);
            }
        }
        public static float DistanceColor(this Color mainColor, Color target, int style) => style switch { 0 => (mainColor.ToVector3() - target.ToVector3()).Length(), 1 => DistanceColor(mainColor, target), _ => 0 };
        public static float DistanceColor(this Vector3 mainColor, Vector3 target)
        {
            float hueDistance;
            float saturationDistance = Math.Max(mainColor.Y, target.Y) - Math.Min(mainColor.Y, target.Y);
            float luminosityDistance = Math.Max(mainColor.Z, target.Z) - Math.Min(mainColor.Z, target.Z);
            #region 色调处理
            {
                hueDistance = Math.Max(mainColor.X, target.X);
                float helper = Math.Min(mainColor.X, target.X);
                hueDistance = Math.Min(hueDistance - helper, helper + 1 - hueDistance);
            }
            #endregion
            hueDistance *= mainColor.Y * 2 * MathF.Sqrt(mainColor.Z * (1 - mainColor.Z));
            return hueDistance * 8 + saturationDistance + luminosityDistance;
        }
        public static float DistanceColor(this Color mainColor, Color target) => DistanceColor(Main.rgbToHsl(mainColor), Main.rgbToHsl(target));

        public static CustomVertexInfo[] TailVertexFromProj(this Projectile projectile, Vector2 Offset = default, float Width = 30, float alpha = 1, bool VeloTri = false, Color? mainColor = null)
        {
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            int indexMax = -1;
            for (int n = 0; n < projectile.oldPos.Length; n++) if (projectile.oldPos[n] == Vector2.Zero) { indexMax = n; break; }
            //if(!Main.gamePaused)
            //Main.NewText(projectile.oldPos[0]);
            if (indexMax == -1) indexMax = projectile.oldPos.Length;
            Offset += projectile.velocity;
            var _mainColor = (mainColor ?? Color.Purple);
            for (int i = 1; i < indexMax; ++i)
            {
                if (projectile.oldPos[i] == Vector2.Zero)
                {
                    break;
                }
                var normalDir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
                var factor = i / (float)indexMax;
                var w = 1 - factor;
                bars.Add(new CustomVertexInfo(projectile.oldPos[i] + Offset + normalDir * Width, _mainColor * w, new Vector3((float)Math.Sqrt(factor), 1, alpha * .6f)));//w * 
                bars.Add(new CustomVertexInfo(projectile.oldPos[i] + Offset + normalDir * -Width, _mainColor * w, new Vector3((float)Math.Sqrt(factor), 0, alpha * .6f)));//w * 
            }
            List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                if (VeloTri)
                {
                    triangleList.Add(bars[0]);
                    var vertex = new CustomVertexInfo((bars[0].Position + bars[1].Position) * 0.5f + Vector2.Normalize(projectile.velocity) * 30, _mainColor,
                        new Vector3(0, 0.5f, alpha * .8f));
                    triangleList.Add(bars[1]);
                    triangleList.Add(vertex);
                }

                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }
            }
            return triangleList.ToArray();
        }
        public static void DrawShaderTail(this SpriteBatch spriteBatch, Projectile projectile, Texture2D heatMap, Texture2D aniTex, Texture2D baseTex, float Width = 30, Vector2 Offset = default, float alpha = 1, bool VeloTri = false, bool additive = false, Color? mainColor = null)
        {
            var triangleList = projectile.TailVertexFromProj(Offset, Width, alpha, VeloTri, mainColor);
            if (triangleList.Length < 3) return;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            //var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            //var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            //IllusionBoundMod.DefaultEffect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            //IllusionBoundMod.DefaultEffect.Parameters["uTime"].SetValue(-(float)IllusionBoundMod.ModTime * 0.03f);
            //Main.graphics.GraphicsDevice.Textures[0] = heatMap;
            //Main.graphics.GraphicsDevice.Textures[1] = baseTex;
            //Main.graphics.GraphicsDevice.Textures[2] = aniTex;
            //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            //Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            //Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
            //IllusionBoundMod.DefaultEffect.CurrentTechnique.Passes[0].Apply();
            //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList, 0, triangleList.Length / 3);
            //Main.graphics.GraphicsDevice.RasterizerState = originalState;
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            ShaderSwooshEX.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            ShaderSwooshEX.Parameters["uTime"].SetValue(-(float)CoolerSystem.ModTime * 0.03f);

            ShaderSwooshEX.Parameters["uLighter"].SetValue(0);
            //CoolerItemVisualEffect.ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
            ShaderSwooshEX.Parameters["checkAir"].SetValue(false);
            ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
            ShaderSwooshEX.Parameters["gather"].SetValue(false);
            ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
            ShaderSwooshEX.Parameters["distortScaler"].SetValue(0);
            ShaderSwooshEX.Parameters["alphaFactor"].SetValue(ConfigurationSwoosh_Advanced.ConfigSwooshInstance.alphaFactor);
            ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(ConfigurationSwoosh_Advanced.ConfigSwooshInstance.alphaFactor == 0);

            Main.graphics.GraphicsDevice.Textures[0] = baseTex;
            Main.graphics.GraphicsDevice.Textures[1] = aniTex;
            Main.graphics.GraphicsDevice.Textures[2] = GetWeaponDisplayImage("BaseTex_8");
            Main.graphics.GraphicsDevice.Textures[3] = heatMap;

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.PointClamp;

            ShaderSwooshEX.CurrentTechnique.Passes[mainColor == null ? 2 : 0].Apply();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList, 0, triangleList.Length / 3);
            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, additive ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static Vector2[] CatMullRomCurve(this Vector2[] vecs, int extraLength)
        {
            int l = vecs.Length;
            extraLength += l;
            Vector2[] scVecs = new Vector2[extraLength];
            for (int n = 0; n < extraLength; n++)
            {
                float t = n / (float)extraLength;
                float k = (l - 1) * t;
                int i = (int)k;
                float vk = k % 1;
                if (i == 0)
                {
                    scVecs[n] = Vector2.CatmullRom(2 * vecs[0] - vecs[1], vecs[0], vecs[1], vecs[2], vk);
                }
                else if (i == l - 2)
                {
                    scVecs[n] = Vector2.CatmullRom(vecs[l - 3], vecs[l - 2], vecs[l - 1], 2 * vecs[l - 1] - vecs[l - 2], vk);
                }
                else
                {
                    scVecs[n] = Vector2.CatmullRom(vecs[i - 1], vecs[i], vecs[i + 1], vecs[i + 2], vk);
                }
            }
            return scVecs;
        }
        public static Vector2[] CatMullRomCurve(this Vector2[] vecs, int extraLength, (int start, int end) range)
        {
            if (range.start >= range.end)
            {
                throw new Exception("你丫的找茬是吧，起点下标(start)必须小于终点下标(end)");
            }

            var (s, e) = range;
            int l = e - s;
            extraLength += l;
            Vector2[] scVecs = new Vector2[extraLength];
            for (int n = 0; n < extraLength; n++)
            {
                float t = n / (float)extraLength;
                float k = (l - 1) * t;
                int i = (int)k;
                float vk = k % 1;
                if (i == 0)
                {
                    scVecs[n] = Vector2.CatmullRom(2 * vecs[s] - vecs[1 + s], vecs[s], vecs[1 + s], vecs[2 + s], vk);
                }
                else if (i == l - 2)
                {
                    scVecs[n] = Vector2.CatmullRom(vecs[l - 3 + s], vecs[l - 2 + s], vecs[l - 1 + s], 2 * vecs[l - 1 + s] - vecs[l - 2 + s], vk);
                }
                else
                {
                    scVecs[n] = Vector2.CatmullRom(vecs[i - 1 + s], vecs[i + s], vecs[i + 1 + s], vecs[i + 2 + s], vk);
                }
            }
            return scVecs;
        }
        public static void DrawQuadraticLaser_PassColorBar(this SpriteBatch spriteBatch, Vector2 start, Vector2 unit, Texture2D colorBar, Texture2D style, float length = 3200, float width = 512, float shakeRadMax = 0, float light = 4, bool timeOffset = false, float maxFactor = 0.5f, bool autoAdditive = true, (float x1, float y1, float x2, float y2) texcoord = default, float alpha = 1)
        {

            Effect effect = EightTrigramsFurnaceEffect; if (effect == null) return;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
            if (shakeRadMax > 0)
            {
                unit = unit.RotatedBy(Main.rand.NextFloat(-shakeRadMax, shakeRadMax));
            }

            Vector2 unit2 = new Vector2(-unit.Y, unit.X);
            if (texcoord == default) texcoord = (0, 0, 1, 1);
            bars1.Add(new CustomVertexInfo(start + unit2 * width, alpha, new Vector3(texcoord.x1, texcoord.y1, light)));
            bars1.Add(new CustomVertexInfo(start - unit2 * width, alpha, new Vector3(texcoord.x1, texcoord.y2, light)));
            bars1.Add(new CustomVertexInfo(start + unit2 * width + length * unit, alpha, new Vector3(texcoord.x2, texcoord.y1, 0)));
            bars1.Add(new CustomVertexInfo(start - unit2 * width + length * unit, alpha, new Vector3(texcoord.x2, texcoord.y2, 0)));
            List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
            if (bars1.Count > 2)
            {
                for (int i = 0; i < bars1.Count - 2; i += 2)
                {
                    triangleList1.Add(bars1[i]);
                    triangleList1.Add(bars1[i + 2]);
                    triangleList1.Add(bars1[i + 1]);
                    triangleList1.Add(bars1[i + 1]);
                    triangleList1.Add(bars1[i + 2]);
                    triangleList1.Add(bars1[i + 3]);
                }
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                effect.Parameters["maxFactor"].SetValue(maxFactor);
                effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
                Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
                Main.graphics.GraphicsDevice.Textures[1] = style;
                Main.graphics.GraphicsDevice.Textures[2] = colorBar;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                if (timeOffset)
                {
                    effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect_ColorBar_TimeOffset"].Apply();
                }
                else
                {
                    effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect_ColorBar"].Apply();
                }

                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawQuadraticLaser_PassColorBar(this SpriteBatch spriteBatch, (Vector2 start, Vector2 unit)[] startAndUnits, Texture2D colorBar, Texture2D style, float length = 3200, float width = 512, float shakeRadMax = 0, float light = 4, bool timeOffset = false, float maxFactor = 0.5f, bool autoAdditive = true, float alpha = 1)
        {
            Effect effect = EightTrigramsFurnaceEffect; if (effect == null) return;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            effect.Parameters["maxFactor"].SetValue(maxFactor);
            effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
            Main.graphics.GraphicsDevice.Textures[1] = style;
            Main.graphics.GraphicsDevice.Textures[2] = colorBar;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
            if (timeOffset)
            {
                effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect_ColorBar_TimeOffset"].Apply();
            }
            else
            {
                effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect_ColorBar"].Apply();
            }

            foreach (var (start, _unit) in startAndUnits)
            {
                List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
                var unit = _unit;
                if (shakeRadMax > 0)
                {
                    unit = unit.RotatedBy(Main.rand.NextFloat(-shakeRadMax, shakeRadMax));
                }

                Vector2 unit2 = new Vector2(-unit.Y, unit.X);
                bars1.Add(new CustomVertexInfo(start + unit2 * width, alpha, new Vector3(0, 0, light)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width, alpha, new Vector3(0, 1, light)));
                bars1.Add(new CustomVertexInfo(start + unit2 * width + length * unit, alpha, new Vector3(1, 0, 0)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width + length * unit, alpha, new Vector3(1, 1, 0)));
                List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
                if (bars1.Count > 2)
                {
                    for (int i = 0; i < bars1.Count - 2; i += 2)
                    {
                        triangleList1.Add(bars1[i]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 3]);
                    }
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                }
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawQuadraticLaser_PassHeatMap(this SpriteBatch spriteBatch, Vector2 start, Vector2 unit, Texture2D heatMap, Texture2D style, float length = 3200, float width = 512, float shakeRadMax = 0, float light = 4, bool timeOffset = false, float maxFactor = 0.5f, bool autoAdditive = true, (float x1, float y1, float x2, float y2) texcoord = default, float alpha = 1)
        {
            Effect effect = EightTrigramsFurnaceEffect; if (effect == null) return;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
            if (shakeRadMax > 0)
            {
                unit = unit.RotatedBy(Main.rand.NextFloat(-shakeRadMax, shakeRadMax));
            }

            Vector2 unit2 = new Vector2(-unit.Y, unit.X);
            if (texcoord == default) texcoord = (0, 0, 1, 1);
            bars1.Add(new CustomVertexInfo(start + unit2 * width, alpha, new Vector3(texcoord.x1, texcoord.y1, light)));
            bars1.Add(new CustomVertexInfo(start - unit2 * width, alpha, new Vector3(texcoord.x1, texcoord.y2, light)));
            bars1.Add(new CustomVertexInfo(start + unit2 * width + length * unit, alpha, new Vector3(texcoord.x2, texcoord.y1, 0)));
            bars1.Add(new CustomVertexInfo(start - unit2 * width + length * unit, alpha, new Vector3(texcoord.x2, texcoord.y2, 0)));
            List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
            if (bars1.Count > 2)
            {
                for (int i = 0; i < bars1.Count - 2; i += 2)
                {
                    triangleList1.Add(bars1[i]);
                    triangleList1.Add(bars1[i + 2]);
                    triangleList1.Add(bars1[i + 1]);
                    triangleList1.Add(bars1[i + 1]);
                    triangleList1.Add(bars1[i + 2]);
                    triangleList1.Add(bars1[i + 3]);
                }
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                effect.Parameters["maxFactor"].SetValue(maxFactor);
                effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
                Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
                Main.graphics.GraphicsDevice.Textures[1] = style;
                Main.graphics.GraphicsDevice.Textures[2] = heatMap;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                if (timeOffset)
                {
                    effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect_HeatMap_TimeOffset"].Apply();
                }
                else
                {
                    effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect_HeatMap"].Apply();
                }

                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawQuadraticLaser_PassHeatMap(this SpriteBatch spriteBatch, (Vector2 start, Vector2 unit)[] startAndUnits, Texture2D heatMap, Texture2D style, float length = 3200, float width = 512, float shakeRadMax = 0, float light = 4, bool timeOffset = false, float maxFactor = 0.5f, bool autoAdditive = true, float alpha = 1)
        {
            Effect effect = EightTrigramsFurnaceEffect; if (effect == null) return;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            effect.Parameters["maxFactor"].SetValue(maxFactor);
            effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
            Main.graphics.GraphicsDevice.Textures[1] = style;
            Main.graphics.GraphicsDevice.Textures[2] = heatMap;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
            if (timeOffset)
            {
                effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect_HeatMap_TimeOffset"].Apply();
            }
            else
            {
                effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect_HeatMap"].Apply();
            }

            foreach (var (start, _unit) in startAndUnits)
            {
                List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
                var unit = _unit;
                if (shakeRadMax > 0)
                {
                    unit = unit.RotatedBy(Main.rand.NextFloat(-shakeRadMax, shakeRadMax));
                }

                Vector2 unit2 = new Vector2(-unit.Y, unit.X);
                bars1.Add(new CustomVertexInfo(start + unit2 * width, alpha, new Vector3(0, 0, light)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width, alpha, new Vector3(0, 1, light)));
                bars1.Add(new CustomVertexInfo(start + unit2 * width + length * unit, alpha, new Vector3(1, 0, 0)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width + length * unit, alpha, new Vector3(1, 1, 0)));
                List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
                if (bars1.Count > 2)
                {
                    for (int i = 0; i < bars1.Count - 2; i += 2)
                    {
                        triangleList1.Add(bars1[i]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 3]);
                    }
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                }
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawQuadraticLaser_PassNormal(this SpriteBatch spriteBatch, Vector2 start, Vector2 unit, Color color, Texture2D style, float length = 3200, float width = 512, float shakeRadMax = 0, float light = 4, float maxFactor = 0.5f, bool autoAdditive = true)
        {
            Effect effect = EightTrigramsFurnaceEffect; if (effect == null) return;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
            if (shakeRadMax > 0)
            {
                unit = unit.RotatedBy(Main.rand.NextFloat(-shakeRadMax, shakeRadMax));
            }

            Vector2 unit2 = new Vector2(-unit.Y, unit.X);
            bars1.Add(new CustomVertexInfo(start + unit2 * width, color, new Vector3(0, 0, light)));
            bars1.Add(new CustomVertexInfo(start - unit2 * width, color, new Vector3(0, 1, light)));
            bars1.Add(new CustomVertexInfo(start + unit2 * width + length * unit, color, new Vector3(1, 0, 0)));
            bars1.Add(new CustomVertexInfo(start - unit2 * width + length * unit, color, new Vector3(1, 1, 0)));
            List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
            if (bars1.Count > 2)
            {
                for (int i = 0; i < bars1.Count - 2; i += 2)
                {
                    triangleList1.Add(bars1[i]);
                    triangleList1.Add(bars1[i + 2]);
                    triangleList1.Add(bars1[i + 1]);
                    triangleList1.Add(bars1[i + 1]);
                    triangleList1.Add(bars1[i + 2]);
                    triangleList1.Add(bars1[i + 3]);
                }
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                effect.Parameters["maxFactor"].SetValue(maxFactor);
                effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
                Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
                Main.graphics.GraphicsDevice.Textures[1] = style;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect"].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawQuadraticLaser_PassNormal(this SpriteBatch spriteBatch, (Vector2 start, Vector2 unit)[] startAndUnits, Color color, Texture2D style, float length = 3200, float width = 512, float shakeRadMax = 0, float light = 4, float maxFactor = 0.5f, bool autoAdditive = true)
        {
            Effect effect = EightTrigramsFurnaceEffect; if (effect == null) return;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            effect.Parameters["maxFactor"].SetValue(maxFactor);
            effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
            Main.graphics.GraphicsDevice.Textures[1] = style;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes["EightTrigramsFurnaceEffect"].Apply();
            foreach (var (start, _unit) in startAndUnits)
            {
                List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
                var unit = _unit;
                if (shakeRadMax > 0)
                {
                    unit = unit.RotatedBy(Main.rand.NextFloat(-shakeRadMax, shakeRadMax));
                }

                Vector2 unit2 = new Vector2(-unit.Y, unit.X);
                bars1.Add(new CustomVertexInfo(start + unit2 * width, color, new Vector3(0, 0, light)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width, color, new Vector3(0, 1, light)));
                bars1.Add(new CustomVertexInfo(start + unit2 * width + length * unit, color, new Vector3(1, 0, 0)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width + length * unit, color, new Vector3(1, 1, 0)));
                List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
                if (bars1.Count > 2)
                {
                    for (int i = 0; i < bars1.Count - 2; i += 2)
                    {
                        triangleList1.Add(bars1[i]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 3]);
                    }
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                }
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawEffectLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 _unit, Color color, Texture2D style, float startLight = 1, float endLight = 0, float length = 3200, float width = 512, bool autoAdditive = true)
        {
            try
            {
                Effect effect = ShaderSwooshEffect;
                if (effect == null)
                {
                    return;
                }

                if (autoAdditive)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
                Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
                Main.graphics.GraphicsDevice.Textures[1] = style;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                effect.CurrentTechnique.Passes[0].Apply();
                List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
                var unit = _unit;
                Vector2 unit2 = new Vector2(-unit.Y, unit.X);
                bars1.Add(new CustomVertexInfo(start + unit2 * width, color, new Vector3(0, 0, startLight)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width, color, new Vector3(0, 1, startLight)));
                bars1.Add(new CustomVertexInfo(start + unit2 * width + length * unit, color, new Vector3(1, 0, endLight)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width + length * unit, color, new Vector3(1, 1, endLight)));
                List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
                if (bars1.Count > 2)
                {
                    for (int i = 0; i < bars1.Count - 2; i += 2)
                    {
                        triangleList1.Add(bars1[i]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 3]);
                    }
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                }
                if (autoAdditive)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }
            catch (Exception e)
            {
                Main.NewText(e);
            }
        }
        public static void DrawEffectLine(this SpriteBatch spriteBatch, (Vector2 start, Vector2 unit)[] startAndUnits, Color color, Texture2D style, float startLight = 1, float endLight = 0, float length = 3200, float width = 512, bool autoAdditive = true)
        {
            Effect effect = ShaderSwooshEffect;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            //effect.Parameters["maxFactor"].SetValue(maxFactor);
            effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
            Main.graphics.GraphicsDevice.Textures[1] = style;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[0].Apply();
            foreach (var (start, _unit) in startAndUnits)
            {
                List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
                var unit = _unit;
                Vector2 unit2 = new Vector2(-unit.Y, unit.X);
                bars1.Add(new CustomVertexInfo(start + unit2 * width, color, new Vector3(0, 0, startLight)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width, color, new Vector3(0, 1, startLight)));
                bars1.Add(new CustomVertexInfo(start + unit2 * width + length * unit, color, new Vector3(1, 0, endLight)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width + length * unit, color, new Vector3(1, 1, endLight)));
                List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
                if (bars1.Count > 2)
                {
                    for (int i = 0; i < bars1.Count - 2; i += 2)
                    {
                        triangleList1.Add(bars1[i]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 3]);
                    }
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                }
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawEffectLine_StartAndEnd(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, Texture2D style, float startLight = 1, float endLight = 0, float width = 512, bool autoAdditive = true)
        {
            Effect effect = ShaderSwooshEffect;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
            Main.graphics.GraphicsDevice.Textures[1] = style;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[1].Apply();
            List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
            var unit = Vector2.Normalize(end - start);
            //unit.Normalize();
            Vector2 unit2 = new Vector2(-unit.Y, unit.X);
            bars1.Add(new CustomVertexInfo(start + unit2 * width, color, new Vector3(0, 0, startLight)));
            bars1.Add(new CustomVertexInfo(start - unit2 * width, color, new Vector3(0, 1, startLight)));
            bars1.Add(new CustomVertexInfo(end + unit2 * width, color, new Vector3(1, 0, endLight)));
            bars1.Add(new CustomVertexInfo(end - unit2 * width, color, new Vector3(1, 1, endLight)));
            List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
            if (bars1.Count > 2)
            {
                for (int i = 0; i < bars1.Count - 2; i += 2)
                {
                    triangleList1.Add(bars1[i]);
                    triangleList1.Add(bars1[i + 2]);
                    triangleList1.Add(bars1[i + 1]);
                    triangleList1.Add(bars1[i + 1]);
                    triangleList1.Add(bars1[i + 2]);
                    triangleList1.Add(bars1[i + 3]);
                }
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public static void DrawEffectLine_StartAndEnd(this SpriteBatch spriteBatch, (Vector2 start, Vector2 end)[] startAndEnds, Color color, Texture2D style, float startLight = 1, float endLight = 0, float width = 512, bool autoAdditive = true)
        {
            Effect effect = ShaderSwooshEffect;
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            effect.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_8");
            Main.graphics.GraphicsDevice.Textures[1] = style;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[1].Apply();
            foreach (var (start, end) in startAndEnds)
            {
                List<CustomVertexInfo> bars1 = new List<CustomVertexInfo>();
                var unit = Vector2.Normalize(end - start);
                //unit.Normalize();
                Vector2 unit2 = new Vector2(-unit.Y, unit.X);
                bars1.Add(new CustomVertexInfo(start + unit2 * width, color, new Vector3(0, 0, startLight)));
                bars1.Add(new CustomVertexInfo(start - unit2 * width, color, new Vector3(0, 1, startLight)));
                bars1.Add(new CustomVertexInfo(end + unit2 * width, color, new Vector3(1, 0, endLight)));
                bars1.Add(new CustomVertexInfo(end - unit2 * width, color, new Vector3(1, 1, endLight)));
                List<CustomVertexInfo> triangleList1 = new List<CustomVertexInfo>();
                if (bars1.Count > 2)
                {
                    for (int i = 0; i < bars1.Count - 2; i += 2)
                    {
                        triangleList1.Add(bars1[i]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 1]);
                        triangleList1.Add(bars1[i + 2]);
                        triangleList1.Add(bars1[i + 3]);
                    }
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList1.ToArray(), 0, triangleList1.Count / 3);
                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                }
            }
            if (autoAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        /// <summary>
        /// 阿汪超喜欢用的插值函数，获得一个先上后下的插值
        /// </summary>
        /// <param name="value">丢进去的变量，取值范围一般是[0,2*center]</param>
        /// <param name="center">中间值，或者说最大值点</param>
        /// <param name="whenGetMax">决定丢进去的值与最大值的比值为多少时第一次达到最大值(1)，一般取(0,0.5f]</param>
        /// <returns>自己画函数图像去，不是三角形就是梯形(</returns>
        public static float SymmetricalFactor2(this float value, float center, float whenGetMax)
        {
            //return Clamp((center - Math.Abs(center - value)) / center / whenGetMax, 0, 1);
            return value.SymmetricalFactor(center, whenGetMax * center * 2);
        }

        /// <summary>
        /// 阿汪超喜欢用的插值函数，获得一个先上后下的插值
        /// </summary>
        /// <param name="value">丢进去的变量，取值范围一般是[0,2*center]</param>
		/// <param name="center">中间值，或者说最大值点</param>
		/// <param name="whenGetMax">决定丢进去的值为多少时第一次达到最大值(1)，一般取(0,center]</param>
		/// <returns>自己画函数图像去，不是三角形就是梯形(</returns>
        public static float SymmetricalFactor(this float value, float center, float whenGetMax)
        {
            return MathHelper.Clamp((center - Math.Abs(center - value)) / whenGetMax, 0, 1);
        }
        /// <summary>
        /// 阿汪超喜欢用的插值函数，获得一个先迅速增加再慢慢变小的插值
        /// </summary>
        /// <param name="value">丢进去的变量，取值范围一般是[0,maxTimeWhen]</param>
        /// <param name="maxTimeWhen">什么时候插值结束呢</param>
        /// <returns>自己画函数图像去，真的像是一个小山丘一样(</returns>
        public static float HillFactor2(this float value, float maxTimeWhen = 1)
        {
            //return Clamp((center - Math.Abs(center - value)) / center / whenGetMax, 0, 1);
            return (1 - (float)Math.Cos(MathHelper.TwoPi * Math.Sqrt(value / maxTimeWhen))) * 0.5f;
        }
        public static void DrawPrettyStarSparkle(this Projectile projectile, SpriteBatch spriteBatch, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor)
        {
            Texture2D value = GetTexture("FinalFractalLight");
            Color color = shineColor * projectile.Opacity * 0.5f;
            color.A = 0;
            Vector2 origin = value.Size() / 2f;
            Color color2 = drawColor * 0.5f;
            float num = GetLerpValue(15f, 30f, projectile.localAI[0], true) * GetLerpValue(45f, 30f, projectile.localAI[0], true);
            Vector2 vector = new Vector2(0.5f, 5f) * num;
            Vector2 vector2 = new Vector2(0.5f, 2f) * num;
            color *= num;
            color2 *= num;
            spriteBatch.Draw(value, drawpos, null, color, 1.57079637f, origin, vector, dir, 0);
            spriteBatch.Draw(value, drawpos, null, color, 0f, origin, vector2, dir, 0);
            spriteBatch.Draw(value, drawpos, null, color2, 1.57079637f, origin, vector * 0.6f, dir, 0);
            spriteBatch.Draw(value, drawpos, null, color2, 0f, origin, vector2 * 0.6f, dir, 0);
        }
        public static void DrawProjWithStarryTrail(this Projectile projectile, SpriteBatch spriteBatch, float drawColor, Color projectileColor, SpriteEffects dir)
        {
            //GameTime gameTime = new GameTime();
            Color color = new Color(255, 255, 255, (int)projectileColor.A - projectile.alpha);
            Vector2 vector = projectile.velocity;
            Color value = Color.Blue * 0.1f;
            Vector2 spinningpoint = new Vector2(0f, -4f);
            float num = 0f;
            float t = vector.Length();
            float scale = GetLerpValue(3f, 5f, t, true);
            bool flag = true;
            vector = projectile.position - projectile.oldPos[1];
            float num2 = vector.Length();
            if (num2 == 0f)
            {
                vector = Vector2.UnitY;
            }
            else
            {
                vector *= 5f / num2;
            }
            Vector2 origin = new Vector2(projectile.ai[0], projectile.ai[1]);
            Vector2 center = Main.player[projectile.owner].Center;
            float num3 = GetLerpValue(0f, 120f, Vector2.Distance(origin, center), true);
            float num4 = 90f;
            num4 = 60f;
            flag = false;
            float num5 = GetLerpValue(num4, num4 * 0.8333333f, projectile.localAI[0], true);
            float lerpValue = GetLerpValue(0f, 120f, Vector2.Distance(projectile.Center, center), true);
            num3 *= lerpValue;
            num5 *= GetLerpValue(0f, 15f, projectile.localAI[0], true);
            value = Color.HotPink * 0.15f * (num5 * num3);
            value = Main.hslToRgb(drawColor, 1f, 0.5f) * 0.15f * (num5 * num3);
            spinningpoint = new Vector2(0f, -2f);
            float num6 = GetLerpValue(num4, num4 * 0.6666667f, projectile.localAI[0], true);
            num6 *= GetLerpValue(0f, 20f, projectile.localAI[0], true);
            num = -0.3f * (1f - num6);
            num += -1f * GetLerpValue(15f, 0f, projectile.localAI[0], true);
            num *= num3;
            scale = num5 * num3;
            Vector2 value2 = projectile.Center + vector;
            Texture2D value3 = TextureAssets.Projectile[projectile.type].Value;
            //new Microsoft.Xna.Framework.Rectangle(0, 0, value3.Width, value3.Height).Size() /= 2f;
            Texture2D value4 = GetTexture("FinalFractalTail");
            Rectangle rectangle = Utils.Frame(value4, 1, 1, 0, 0, 0, 0);
            Vector2 origin2 = new Vector2((float)rectangle.Width / 2f, 10f);
            //Microsoft.Xna.Framework.Color.Cyan * 0.5f * scale;
            Vector2 value5 = new Vector2(0f, projectile.gfxOffY);
            float num7 = (float)Main.time / 60f;
            Vector2 value6 = value2 + vector * 0.5f;
            Color value7 = Color.White * 0.5f * scale;
            value7.A = 0;
            Color color2 = value * scale;
            color2.A = 0;
            Color color3 = value * scale;
            color3.A = 0;
            Color color4 = value * scale;
            color4.A = 0;
            float num8 = vector.ToRotation();
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.5f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 2.09439516f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color3, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.1f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 4.18879032f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color4, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.3f + num, SpriteEffects.None, 0);
            Vector2 value8 = value2 - vector * 0.5f;
            for (float num9 = 0f; num9 < 1f; num9 += 0.5f)
            {
                float num10 = num7 % 0.5f / 0.5f;
                num10 = (num10 + num9) % 1f;
                float num11 = num10 * 2f;
                if (num11 > 1f)
                {
                    num11 = 2f - num11;
                }
                spriteBatch.Draw(value4, value8 - Main.screenPosition + value5, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * num11 * .5f * projectile.Opacity, projectile.velocity.ToRotation() + 1.57079637f, origin2, 0.3f + num10 * 0.5f, SpriteEffects.None, 0);
            }
            if (flag)
            {
                float rotation = projectile.rotation + projectile.localAI[1];
                //float num12 = (float)Main.time / 240f;
                //float globalTimeWrappedHourly = (float)(gameTime.TotalGameTime.TotalSeconds % 3600.0);
                /*float num13 = (float)(gameTime.TotalGameTime.TotalSeconds % 3600.0);
                num13 %= 5f;
                num13 /= 2.5f;
                if (num13 >= 1f)
                {
                    num13 = 2f - num13;
                }
                num13 = num13 * 0.5f + 0.5f;*/
                Vector2 position = projectile.Center - Main.screenPosition;
                //Main.instance.LoadItem(75);
                Texture2D value9 = GetTexture("FinalFractalTail2");
                Rectangle rectangle2 = Utils.Frame(value9, 1, 8, 0, 0, 0, 0);
                Vector2 origin3 = rectangle2.Size() / 2f;
                spriteBatch.Draw(value9, position, new Microsoft.Xna.Framework.Rectangle?(rectangle2), color, rotation, origin3, projectile.scale, SpriteEffects.None, 0);
            }
        }

        //public static Vector2 Projectile(this Vector3 vec, float height)
        //{
        //    return height / (height - vec.Z) * new Vector2(vec.X, vec.Y);
        //}
        //public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width = 2)
        //{
        //    spriteBatch.Draw(TextureAssets.MagicPixel.Value, (start + end) / 2f - Main.screenPosition, new Rectangle(0, 0, 1, 1), color, (end - start).ToRotation(), new Vector2(0.5f, 0.5f), new Vector2((end - start).Length(), width), 0, 0);
        //}
        public static bool SpecialCheck(this int type) => new int[] { ItemID.Zenith, ModContent.ItemType<Weapons.FirstFractal_CIVE>(), ModContent.ItemType<Weapons.PureFractal_Old>(), ModContent.ItemType<Weapons.FirstZenith_Old>() }.Contains(type);
        public static void DrawHammer(this SpriteBatch spriteBatch, IHammerProj hammerProj)
        {
            Vector2 origin = hammerProj.DrawOrigin;
            float rotation = hammerProj.Rotation;
            var flip = hammerProj.flip;
            if (hammerProj.Player.gravDir == -1)
            {
                rotation = MathHelper.PiOver2 - rotation;
                if (flip == 0) flip = SpriteEffects.FlipHorizontally;
                else if (flip == SpriteEffects.FlipHorizontally) flip = 0;
            }
            switch (flip)
            {
                case SpriteEffects.FlipHorizontally:
                    origin.X = hammerProj.projTex.Size().X / hammerProj.FrameMax.X - origin.X;
                    rotation += MathHelper.PiOver2;

                    break;
                case SpriteEffects.FlipVertically:
                    origin.Y = hammerProj.projTex.Size().Y / hammerProj.FrameMax.Y - origin.Y;
                    break;
            }
            spriteBatch.Draw(hammerProj.projTex, hammerProj.projCenter - Main.screenPosition, hammerProj.frame, hammerProj.color, rotation, origin, hammerProj.scale, flip, 0);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width = 4f, bool offset = false, Vector2 drawOffset = default)
        {
            if (offset) end += start;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, (start + end) * .5f + drawOffset, new Rectangle(0, 0, 1, 1), color, (end - start).ToRotation(), new Vector2(.5f, .5f), new Vector2((start - end).Length(), width), 0, 0);
        }
        public static Texture2D GetTexture(string path, bool autoPath = true) => ModContent.Request<Texture2D>((autoPath ? "CoolerItemVisualEffect/Shader/" : "") + path).Value;
        public static void ShaderItemEffectInWorld(this Item item, SpriteBatch spriteBatch, Texture2D effectTex, Color c, float rotation, float light = 2)
        {
            if (ItemEffect == null) return;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null);
            CustomVertexInfo[] triangleArry = new CustomVertexInfo[6];
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            //Color c = Main.hslToRgb(CoolerSystem.ModTime / 60 % 1, 1f, 0.75f);
            var texture = TextureAssets.Item[item.type].Value;
            Vector2 scale = texture.Size();
            //triangleArry[0] = new CustomVertexInfo(item.position, c, new Vector3(0, 0, light));
            //triangleArry[1] = new CustomVertexInfo(item.position + new Vector2(scale.X, 0), c, new Vector3(1, 0, light));
            //triangleArry[2] = new CustomVertexInfo(item.position + scale, c, new Vector3(1, 1, light));
            //triangleArry[3] = triangleArry[2];
            //triangleArry[4] = new CustomVertexInfo(item.position + new Vector2(0, scale.Y), c, new Vector3(0, 1, light));
            //triangleArry[5] = triangleArry[0];
            var ani = Main.itemAnimations[item.type];
            var texCoordStart = new Vector2(0);
            var texCoordEnd = new Vector2(1);

            if (ani != null)
            {
                var frame = ani.GetFrame(texture);
                texCoordStart = frame.TopLeft() / scale;
                texCoordEnd = frame.BottomRight() / scale;
                scale = frame.Size();
            }
            scale *= 0.5f;
            var center = item.Center;//item.position + scale
            triangleArry[0] = new CustomVertexInfo(center - scale.RotatedBy(rotation), c, new Vector3(texCoordStart, light));
            triangleArry[1] = new CustomVertexInfo(center + new Vector2(scale.X, -scale.Y).RotatedBy(rotation), c, new Vector3(new Vector2(texCoordEnd.X, texCoordStart.Y), light));
            triangleArry[2] = new CustomVertexInfo(center + scale.RotatedBy(rotation), c, new Vector3(texCoordEnd, light));
            triangleArry[3] = triangleArry[2];
            triangleArry[4] = new CustomVertexInfo(center - new Vector2(scale.X, -scale.Y).RotatedBy(rotation), c, new Vector3(new Vector2(texCoordStart.X, texCoordEnd.Y), light));
            triangleArry[5] = triangleArry[0];
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            ItemEffect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
            ItemEffect.Parameters["uTime"].SetValue(ModTime / 60f);//CoolerSystem.ModTime / 60
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.Textures[1] = effectTex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            ItemEffect.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleArry, 0, 2);
            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void ShaderItemEffectInventory(this Item item, SpriteBatch spriteBatch, Vector2 position, Vector2 origin, Texture2D effectTex, Color c, float Scale, float light = 2)
        {
            if (ItemEffect == null) return;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            var ani = Main.itemAnimations[item.type];
            CustomVertexInfo[] triangleArry = new CustomVertexInfo[6];
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            Texture2D texture = TextureAssets.Item[item.type].Value;
            Vector2 scale = texture.Size();
            //float offsetX = texture.Width * Scale;
            //float offsetY = texture.Height * Scale;
            //Color c = Main.hslToRgb(0f, 1f, 0.75f);
            //triangleArry[0] = new CustomVertexInfo(position + Main.screenPosition - new Vector2(offsetX, offsetY) + origin, c, new Vector3(0, 0, light));
            //triangleArry[1] = new CustomVertexInfo(position + Main.screenPosition + new Vector2(offsetX, -offsetY) + origin, c, new Vector3(1, 0, light));
            //triangleArry[2] = new CustomVertexInfo(position + Main.screenPosition + new Vector2(offsetX, offsetY) + origin, c, new Vector3(1, 1, light));
            //triangleArry[3] = triangleArry[2];
            //triangleArry[4] = new CustomVertexInfo(position + Main.screenPosition - new Vector2(offsetX, -offsetY) + origin, c, new Vector3(0, 1, light));
            //triangleArry[5] = triangleArry[0];
            //Vector2 offset = ItemID.Sets.ItemIconPulse[item.type] ? default : new Vector2(-2, -2);
            //var texCoordYstart = 0f;
            //var texCoordYend = 1f;
            //if (ani != null)
            //{
            //    offsetY /= ani.FrameCount;
            //    texCoordYend = 1f / ani.FrameCount;
            //    texCoordYstart = texCoordYend * ani.Frame;
            //    texCoordYend += texCoordYstart;
            //}
            var texCoordStart = new Vector2(0);
            var texCoordEnd = new Vector2(1);

            if (ani != null)
            {
                var frame = ani.GetFrame(texture);
                texCoordStart = frame.TopLeft() / scale;
                texCoordEnd = frame.BottomRight() / scale;
                scale = frame.Size();
            }
            scale *= Scale;
            position -= origin * Scale;
            triangleArry[0] = new CustomVertexInfo(position + Main.screenPosition, c, new Vector3(texCoordStart, light));
            triangleArry[1] = new CustomVertexInfo(position + Main.screenPosition + new Vector2(scale.X, 0), c, new Vector3(new Vector2(texCoordEnd.X, texCoordStart.Y), light));
            triangleArry[2] = new CustomVertexInfo(position + Main.screenPosition + scale, c, new Vector3(texCoordEnd, light));
            triangleArry[3] = triangleArry[2];
            triangleArry[4] = new CustomVertexInfo(position + Main.screenPosition + new Vector2(0, scale.Y), c, new Vector3(new Vector2(texCoordStart.X, texCoordEnd.Y), light));
            triangleArry[5] = triangleArry[0];
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            ItemEffect.Parameters["uTransform"].SetValue(model * projection);
            ItemEffect.Parameters["uTime"].SetValue(ModTime / 60f % 1);
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.Textures[1] = effectTex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            ItemEffect.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleArry, 0, 2);
            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static float Lerp(this float t, float from, float to, bool clamp = false)
        {
            if (clamp) t = MathHelper.Clamp(t, 0, 1);
            return (1 - t) * from + t * to;
        }
        //public static T Lerp<T>(this float t, T from, T to,bool clamp = false) 
        //{
        //    if (clamp) t = MathHelper.Clamp(t, 0, 1);
        //    return (1 - t) * from + t * to;
        //}

        //public static void DrawPath(this SpriteBatch spriteBatch, Func<float, Vector2> vectorFunc, Func<float, Color> colorFunc, Effect effect, Texture2D baseTex, Texture2D aniTex, Vector2 offest = default, int counts = 25, float min = 0, float max = 1, float width = 16, float kOfX = 1, bool looped = false, Func<float, float> factorFunc = null, Func<float, float> widthFunc = null, Func<float, float> lightFunc = null, Func<float> timeFunc = null, string pass = default, Action<Vector2> doSth = null, bool alwaysDoSth = false, bool beginImmediately = false)
        //{
        //    if (vectorFunc == null || colorFunc == null || effect == null || counts < 3)
        //    {
        //        return;
        //    }
        //    var positions = new Vector2[counts];
        //    var bars = new CustomVertexInfo[counts * 2];
        //    for (int n = 0; n < counts; n++)
        //    {
        //        var factor = (float)n / (counts - 1);
        //        if (factorFunc != null)
        //        {
        //            factor = factorFunc.Invoke(factor);
        //        }
        //        var position = vectorFunc.Invoke(factor.Lerp(min, max)) + offest;
        //        positions[n] = position;
        //        if ((!Main.gamePaused || alwaysDoSth) && doSth != null)
        //        {
        //            doSth.Invoke(position);
        //        }
        //    }
        //    for (int i = 0; i < counts; ++i)
        //    {
        //        var normalDir = i == 0 ? (looped ? positions[0] - positions[counts - 1] : positions[1] - positions[0]) : positions[i] - positions[i - 1];
        //        normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
        //        var factor = i / (counts - 1f);
        //        if (factorFunc != null)
        //        {
        //            factor = factorFunc.Invoke(factor);
        //        }
        //        var color = colorFunc.Invoke(factor);
        //        var w = widthFunc == null ? width : widthFunc.Invoke(factor);
        //        var l = lightFunc == null ? factor : lightFunc.Invoke(factor);
        //        bars[2 * i] = (new CustomVertexInfo(positions[i] + normalDir * w, color, new Vector3(factor * kOfX, 1, l)));
        //        bars[2 * i + 1] = (new CustomVertexInfo(positions[i] + normalDir * -w, color, new Vector3(factor * kOfX, 0, l)));
        //    }
        //    List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
        //    for (int i = 0; i < bars.Length - 2; i += 2)
        //    {
        //        triangleList.Add(bars[i]);
        //        triangleList.Add(bars[i + 2]);
        //        triangleList.Add(bars[i + 1]);
        //        triangleList.Add(bars[i + 1]);
        //        triangleList.Add(bars[i + 2]);
        //        triangleList.Add(bars[i + 3]);
        //    }
        //    spriteBatch.End();
        //    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        //    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
        //    //RasterizerState rasterizerState = new RasterizerState();
        //    //rasterizerState.CullMode = CullMode.None;
        //    //rasterizerState.FillMode = FillMode.WireFrame;
        //    //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;
        //    Main.graphics.GraphicsDevice.RasterizerState.CullMode = CullMode.None;
        //    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
        //    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
        //    effect.Parameters["uTransform"].SetValue(model * projection);
        //    effect.Parameters["uTime"].SetValue(timeFunc == null ? -(float)Main.time * 0.03f : timeFunc.Invoke());
        //    Main.graphics.GraphicsDevice.Textures[0] = baseTex;
        //    Main.graphics.GraphicsDevice.Textures[1] = aniTex;
        //    Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        //    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        //    if (pass != null) { effect.CurrentTechnique.Passes[pass].Apply(); } else { effect.CurrentTechnique.Passes[0].Apply(); }
        //    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
        //    Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //    spriteBatch.End();
        //    spriteBatch.Begin(beginImmediately ? SpriteSortMode.Immediate : SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        //}
    }
    public static class CoolerItemVisualRecipeMethods
    {
        public static void QuickSpawnItem(this Player player, IEntitySource source, params int[] ingredients)
        {
            foreach (var item in ingredients)
            {
                player.QuickSpawnItem(source, item);
            }
        }
        public static Recipe QuickAddIngredient(this Recipe recipe, params int[] ingredients)
        {
            foreach (var item in ingredients)
            {
                recipe.AddIngredient(item);
            }
            return recipe;
        }
        public static Recipe QuickAddIngredient(this Recipe recipe, params (int, int)[] ingredients)
        {
            foreach (var item in ingredients)
            {
                recipe.AddIngredient(item.Item1, item.Item2);
            }
            return recipe;
        }
        public static Recipe QuickAddIngredient(this Recipe recipe, params ModItem[] ingredients)
        {
            foreach (var item in ingredients)
            {
                recipe.AddIngredient(item);
            }
            return recipe;
        }
        public static Recipe QuickAddIngredient(this Recipe recipe, params (ModItem, int)[] ingredients)
        {
            foreach (var item in ingredients)
            {
                recipe.AddIngredient(item.Item1, item.Item2);
            }
            return recipe;
        }


        public static bool FindSharpTearsOpening(int x, int y, bool acceptLeft, bool acceptRight, bool acceptUp, bool acceptDown)
        {
            if (acceptLeft && !WorldGen.SolidTile(x - 1, y))
                return true;

            if (acceptRight && !WorldGen.SolidTile(x + 1, y))
                return true;

            if (acceptUp && !WorldGen.SolidTile(x, y - 1))
                return true;

            if (acceptDown && !WorldGen.SolidTile(x, y + 1))
                return true;

            return false;
        }
        public static Point FindSharpTearsSpot(this Player player, Vector2 targetSpot)
        {
            Point point = targetSpot.ToTileCoordinates();
            Vector2 center = player.Center;
            Vector2 endPoint = targetSpot;
            int samplesToTake = 3;
            float samplingWidth = 4f;
            Collision.AimingLaserScan(center, endPoint, samplingWidth, samplesToTake, out Vector2 vectorTowardsTarget, out float[] samples);
            float num = float.PositiveInfinity;
            for (int i = 0; i < samples.Length; i++)
            {
                if (samples[i] < num)
                    num = samples[i];
            }

            targetSpot = center + vectorTowardsTarget.SafeNormalize(Vector2.Zero) * num;
            point = targetSpot.ToTileCoordinates();
            Rectangle value = new Rectangle(point.X, point.Y, 1, 1);
            value.Inflate(6, 16);
            Rectangle value2 = new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY);
            value2.Inflate(-40, -40);
            value = Rectangle.Intersect(value, value2);
            List<Point> list = new List<Point>();
            List<Point> list2 = new List<Point>();
            for (int j = value.Left; j <= value.Right; j++)
            {
                for (int k = value.Top; k <= value.Bottom; k++)
                {
                    if (!WorldGen.SolidTile(j, k))
                        continue;

                    Vector2 value3 = new Vector2(j * 16 + 8, k * 16 + 8);
                    if (!(Vector2.Distance(targetSpot, value3) > 200f))
                    {
                        if (FindSharpTearsOpening(j, k, j > point.X, j < point.X, k > point.Y, k < point.Y))
                            list.Add(new Point(j, k));
                        else
                            list2.Add(new Point(j, k));
                    }
                }
            }

            if (list.Count == 0 && list2.Count == 0)
                list.Add((player.Center.ToTileCoordinates().ToVector2() + Main.rand.NextVector2Square(-2f, 2f)).ToPoint());

            List<Point> list3 = list;
            if (list3.Count == 0)
                list3 = list2;

            int index = Main.rand.Next(list3.Count);
            return list3[index];
        }
    }
}
