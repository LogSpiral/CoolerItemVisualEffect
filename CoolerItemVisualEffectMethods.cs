using CoolerItemVisualEffect.Weapons;
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
    public static class CoolerItemVisualEffectMethods
    {
        public static bool SpecialCheck(this int type) => new int[] { ItemID.Zenith, ModContent.ItemType<Weapons.FirstFractal_CIVE>(), ModContent.ItemType<Weapons.PureFractal_Old>(), ModContent.ItemType<Weapons.FirstZenith_Old>() }.Contains(type);
        public static Texture2D GetTexture(string path, bool autoPath = true) => ModContent.Request<Texture2D>((autoPath ? "CoolerItemVisualEffect/Shader/" : "") + path).Value;
        public static void QuickSpawnItem(this Player player, IEntitySource source, params int[] ingredients)
        {
            foreach (var item in ingredients)
            {
                player.QuickSpawnItem(source, item);
            }
        }
    }
}
