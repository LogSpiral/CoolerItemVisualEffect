using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using CoolerItemVisualEffect;
using static CoolerItemVisualEffect.CoolerItemVisualEffectMethods;
using static Terraria.Utils;
using Terraria.GameContent;
using System.IO;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures;

namespace CoolerItemVisualEffect.FinalFractal
{
    public class FinalFractalProjectile : ModProjectile
    {
        Projectile projectile => Projectile;

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.DamageType = DamageClass.Melee;
            projectile.penetrate = 1;
            projectile.light = 0.5f;
            projectile.timeLeft = 180;
            projectile.alpha = 255;
            projectile.friendly = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 30;
        }
        private float drawColor
        {
            get => projectile.ai[0];
            set { projectile.ai[0] = value; }
        }
        public override void AI()
        {
            projectile.localAI[0]++;
            if (projectile.localAI[0] >= 50)
            {
                projectile.localAI[0] = 50;
            }
            if (drawColor == 0)
            {
                drawColor = Main.rand.Next(1, 7);
            }
            switch (drawColor)
            {
                case 6:
                    drawColor = Main.rgbToHsl(new Color(108, 134, 230)).X;
                    break;
                case 1:
                    drawColor = Main.rgbToHsl(new Color(178, 165, 226)).X;
                    break;
                case 2:
                    drawColor = Main.rgbToHsl(new Color(182, 109, 190)).X;
                    break;
                case 3:
                    drawColor = Main.rgbToHsl(new Color(99, 74, 187)).X;
                    break;
                case 4:
                    drawColor = Main.rgbToHsl(new Color(173, 214, 193)).X;
                    break;
                case 5:
                    drawColor = Main.rgbToHsl(new Color(100, 203, 189)).X;
                    break;
                default:

                    break;
            }
            float num = projectile.light;
            float num2 = projectile.light;
            float num3 = projectile.light;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item60, projectile.position);
            }
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 0.785f;
            num *= 0.2f;
            num3 *= 0.6f;
            Lighting.AddLight((int)((projectile.position.X + (float)(projectile.width / 2)) / 16f), (int)((projectile.position.Y + (float)(projectile.height / 2)) / 16f), num, num2, num3);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.localAI[1] >= 15f)
            {
                return new Color(255, 255, 255, projectile.alpha);
            }
            if (projectile.localAI[1] < 5f)
            {
                return Color.Transparent;
            }
            int num7 = (int)((projectile.localAI[1] - 5f) / 10f * 255f);
            return new Color(num7, num7, num7, num7);
        }
        public override void Kill(int timeLeft)
        {
            for (int n = 0; n < 3; n++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), projectile.Center + new Vector2(64, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 3 * n + MathHelper.Pi), new Vector2(-1, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 3 * n + MathHelper.Pi), ProjectileType<FinalFractalShadow>(), projectile.damage, 0, projectile.owner, 1f, 3f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (CoolerItemVisualEffectMod.FinalFractalTailEffect == null) return false; if (CoolerItemVisualEffectMod.ColorfulEffect == null) return false;

            SpriteBatch spriteBatch = Main.spriteBatch;
            var tex = TextureAssets.Projectile[projectile.type].Value;
            DrawProjWithStarryTrail(spriteBatch);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, tex.Frame(), Color.White, projectile.rotation, tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            //{
            //    SpriteEffects spriteEffects = SpriteEffects.None;
            //    if (projectile.spriteDirection == -1)
            //    {
            //        spriteEffects = SpriteEffects.FlipHorizontally;
            //    }
            //    Vector2 vector57 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            //    Texture2D value108 = TextureAssets.Projectile[projectile.type].Value;
            //    Microsoft.Xna.Framework.Color alpha8 = projectile.GetAlpha(Color.White);
            //    Vector2 origin17 = new Vector2((float)value108.Width, (float)value108.Height) / 2f;
            //    float num319 = projectile.rotation;
            //    Vector2 vector58 = Vector2.One * projectile.scale;
            //    float lerpValue6 = Tools.GetLerpValue(0f, 8f, projectile.velocity.Length(), true);
            //    num319 *= lerpValue6;
            //    vector58 *= 0.6f;
            //    vector58.Y *= MathHelper.Lerp(1f, 0.8f, lerpValue6);
            //    vector58.X *= MathHelper.Lerp(1f, 1.5f, lerpValue6);
            //    Microsoft.Xna.Framework.Color color82 = new Microsoft.Xna.Framework.Color(80, 80, 80, 0);
            //    Vector2 vector67 = vector58 + vector58 * (float)Math.Cos((double)(projectile.localAI[0] / 60 * 6.28318548f)) * 0.4f;
            //    Vector2 spinningpoint23 = new Vector2(2f * vector67.X, 0f);
            //    double radians27 = (double)num319;
            //    Vector2 center = default(Vector2);
            //    Vector2 vector68 = spinningpoint23.RotatedBy(radians27, center);
            //    for (float num334 = 0f; num334 < 1f; num334 += 0.25f)
            //    {
            //        Texture2D texture14 = value108;
            //        Vector2 value123 = vector57;
            //        Vector2 spinningpoint24 = vector68;
            //        double radians28 = (double)(num334 * 6.28318548f);
            //        center = default(Vector2);
            //        spriteBatch.Draw(texture14, value123 + spinningpoint24.RotatedBy(radians28, center), null, color82, num319, origin17, vector67, spriteEffects, 0);
            //    }
            //    spriteBatch.Draw(value108, vector57, null, alpha8, num319, origin17, vector58, spriteEffects, 0);

            //}
            if (CoolerItemVisualEffectMod.FinalFractalTailEffect != null)
            {
                List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

                // 把所有的点都生成出来，按照顺序
                for (int i = 1; i < projectile.oldPos.Length; ++i)
                {
                    if (projectile.oldPos[i] == Vector2.Zero) break;
                    //spriteBatch.Draw(TextureAssets.MagicPixel.Value, projectile.oldPos[i] - Main.screenPosition,
                    //    new Rectangle(0, 0, 1, 1), Color.White, 0f, new Vector2(0.5f, 0.5f), 5f, SpriteEffects.None, 0f);

                    //int width = 30;
                    var normalDir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                    normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

                    var factor = i / (float)projectile.oldPos.Length;
                    var color = Color.Lerp(Color.White, Color.Red, factor);
                    var w = MathHelper.Lerp(1f, 0.05f, factor);
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + normalDir * 30, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + normalDir * -15, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
                }

                List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();

                if (bars.Count > 2)
                {

                    // 按照顺序连接三角形
                    triangleList.Add(bars[0]);
                    var vertex = new CustomVertexInfo((bars[0].Position + bars[1].Position) * 0.5f + Vector2.Normalize(projectile.velocity) * 30, Color.White,
                        new Vector3(0, 0.5f, 1));
                    triangleList.Add(bars[1]);
                    triangleList.Add(vertex);
                    for (int i = 0; i < bars.Count - 2; i += 2)
                    {
                        triangleList.Add(bars[i]);
                        triangleList.Add(bars[i + 2]);
                        triangleList.Add(bars[i + 1]);

                        triangleList.Add(bars[i + 1]);
                        triangleList.Add(bars[i + 2]);
                        triangleList.Add(bars[i + 3]);
                    }


                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                    // 干掉注释掉就可以只显示三角形栅格
                    //RasterizerState rasterizerState = new RasterizerState();
                    //rasterizerState.CullMode = CullMode.None;
                    //rasterizerState.FillMode = FillMode.WireFrame;
                    //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));

                    // 把变换和所需信息丢给shader
                    CoolerItemVisualEffectMod.FinalFractalTailEffect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                    CoolerItemVisualEffectMod.FinalFractalTailEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.03f);
                    //CoolerItemVisualEffect.ColorfulEffect.Parameters["defaultColor"].SetValue(Main.hslToRgb(drawColor, 1f, 0.5f).ToVector4());
                    Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.HeatMap[7].Value;
                    Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.BaseTex[13].Value;
                    Main.graphics.GraphicsDevice.Textures[2] = LogSpiralLibraryMod.AniTex[5].Value;
                    Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                    //Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                    //Main.graphics.GraphicsDevice.Textures[1] = TextureAssets.MagicPixel.Value;
                    //Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;
                    /*if (isCyan)
     {
         CoolerItemVisualEffect.CleverEffect.CurrentTechnique.Passes["Clever"].Apply();
     }*/
                    CoolerItemVisualEffectMod.FinalFractalTailEffect.CurrentTechnique.Passes[0].Apply();


                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                    spriteBatch.End();
                    spriteBatch.Begin();
                }
            }
            return false;
        }
        private void DrawProjWithStarryTrail(SpriteBatch spriteBatch)
        {
            Color color = new Color(255, 255, 255, (int)Color.White.A - projectile.alpha);
            Vector2 vector = projectile.velocity;
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
            float num4 = 60f;
            bool flag = false;
            float num5 = GetLerpValue(num4, num4 * 0.8333333f, projectile.localAI[0], true);
            float lerpValue = GetLerpValue(0f, 120f, Vector2.Distance(projectile.Center, center), true);
            num3 *= lerpValue;
            num5 *= GetLerpValue(0f, 15f, projectile.localAI[0], true);
            Color value = Main.hslToRgb(drawColor, 1f, 0.5f) * 0.15f * (num5 * num3);
            var spinningpoint = new Vector2(0f, -2f);
            float num6 = GetLerpValue(num4, num4 * 0.6666667f, projectile.localAI[0], true);
            num6 *= GetLerpValue(0f, 20f, projectile.localAI[0], true);
            var num = -0.3f * (1f - num6);
            num += -1f * GetLerpValue(15f, 0f, projectile.localAI[0], true);
            num *= num3;
            var scale = num5 * num3;
            Vector2 value2 = projectile.Center + vector;
            Texture2D value4 = LogSpiralLibraryMod.Misc[16].Value;
            Rectangle rectangle = value4.Frame(1, 1, 0, 0, 0, 0);
            Vector2 origin2 = new Vector2(rectangle.Width / 2f, 10f);
            Vector2 value5 = new Vector2(0f, projectile.gfxOffY);
            float num7 = (float)Main.time / 60f;
            Vector2 value6 = value2 + projectile.velocity * projectile.scale;
            Color color2 = value * scale;
            color2.A = 0;
            Color color3 = value * scale;
            color3.A = 0;
            Color color4 = value * scale;
            color4.A = 0;
            Vector2 value8 = value2 - vector * 0.5f;
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.5f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 2.09439516f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color3, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.1f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 4.18879032f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color4, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.3f + num, SpriteEffects.None, 0);
            for (float num9 = 0f; num9 < 1f; num9 += 0.5f)
            {
                float num10 = num7 % 0.5f / 0.5f;
                num10 = (num10 + num9) % 1f;
                float num11 = num10 * 2f;
                if (num11 > 1f)
                {
                    num11 = 2f - num11;
                }
                //spriteBatch.Draw(value4, value8 - Main.screenPosition + value5, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * num11, projectile.velocity.ToRotation() + 1.57079637f, origin2, 0.3f + num10 * 0.5f, SpriteEffects.None, 0);
            }
            if (flag)
            {
                float rotation = projectile.rotation + projectile.localAI[1];
                Vector2 position = projectile.Center - Main.screenPosition;
                Texture2D value9 = LogSpiralLibraryMod.Misc[15].Value;
                Rectangle rectangle2 = value9.Frame(1, 8, 0, 0, 0, 0);
                Vector2 origin3 = rectangle2.Size() / 2f;
                spriteBatch.Draw(value9, position, new Rectangle?(rectangle2), color, rotation, origin3, projectile.scale, SpriteEffects.None, 0);
            }
        }
    }
    public class FinalFractalShadow : ModProjectile
    {
        Projectile projectile => Projectile;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[projectile.owner] = 0;
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if(info.PvP) target.immune = false;
            base.OnHitPlayer(target, info);
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.DamageType = DamageClass.Melee;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.friendly = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 30;
        }
        private const int startTime = 5;
        private const int flyTime = 5;
        private Vector2 oldvec
        {
            get => Projectile.oldVelocity;
            set { projectile.oldVelocity = value; }
        }
        private int timer;
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }
        public override bool ShouldUpdatePosition()
        {
            return timer > startTime;
        }
        private float drawColor
        {
            get => projectile.ai[0];
            set { projectile.ai[0] = value; }
        }
        public override void AI()
        {
            float[] array = projectile.localAI;
            int num4 = 0;
            float num5 = array[num4] + 1f;
            array[num4] = num5;
            if (projectile.localAI[0] >= 50)
            {
                projectile.localAI[0] = 50;
            }
            if (drawColor == 0)
            {
                drawColor = Main.rand.Next(1, 7);
            }
            switch (drawColor)
            {
                case 6:
                    drawColor = Main.rgbToHsl(new Color(108, 134, 230)).X;
                    break;
                case 1:
                    drawColor = Main.rgbToHsl(new Color(178, 165, 226)).X;
                    break;
                case 2:
                    drawColor = Main.rgbToHsl(new Color(182, 109, 190)).X;
                    break;
                case 3:
                    drawColor = Main.rgbToHsl(new Color(99, 74, 187)).X;
                    break;
                case 4:
                    drawColor = Main.rgbToHsl(new Color(173, 214, 193)).X;
                    break;
                case 5:
                    drawColor = Main.rgbToHsl(new Color(100, 203, 189)).X;
                    break;
                default:

                    break;
            }
            timer++;

            if (projectile.velocity != Vector2.Zero)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 4;
            }
            if (timer == startTime)
            {
                //oldvec = projectile.velocity;
                //oldvec.Normalize();
                //projectile.velocity = oldvec * 32;
                projectile.velocity = projectile.velocity.SafeNormalize(default) * 32;
                //Main.NewText(projectile.velocity);
            }
            if (timer == startTime + 5)
            {
                Vector2 vec = projectile.velocity;
                vec.Normalize();
                projectile.velocity = vec * 0;
            }
            if (timer == startTime + flyTime && projectile.ai[1] > 0)
            {
                for (int n = 0; n < 3; n++)
                {
                    //Projectile.NewProjectile(Projectile.GetSource_FromThis(),projectile.Center + 64 * oldvec.RotatedBy(MathHelper.TwoPi / 3 * n), 16 * oldvec.RotatedBy(MathHelper.TwoPi / 3 * n), ProjectileType<FinalFractalShadow>(), projectile.damage, 0, projectile.owner, 0.3f * (projectile.ai[1] - 1), projectile.ai[1] - 1);
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), projectile.Center + new Vector2(64, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 3 * n), new Vector2(-1, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 3 * n), ProjectileType<FinalFractalShadow>(), projectile.damage, 0, projectile.owner, 0, projectile.ai[1] - 1).scale = 0.3f * (projectile.ai[1] - 1);
                    //Projectile.NewProjectile(Projectile.GetSource_FromThis(),projectile.Center + new Vector2(64, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 3 * n + MathHelper.Pi / 12), new Vector2(-1, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 3 * n + MathHelper.Pi / 12), ProjectileType<FinalFractalShadow>(), projectile.damage, 0, projectile.owner, 0.3f * (projectile.ai[1] - 1), projectile.ai[1] - 1);
                }
            }
            if (timer == startTime + flyTime + projectile.ai[1] * (startTime + flyTime))
            {
                projectile.velocity = projectile.rotation.ToRotationVector2() * 32;
            }
            if (timer > startTime + flyTime + projectile.ai[1] * (startTime + flyTime) && (Main.player[projectile.owner].name == "WitherStorm" || Main.player[projectile.owner].name == "FFT"))
            {
                projectile.velocity = projectile.velocity.RotatedBy(MathHelper.TwoPi / 60 * projectile.timeLeft / 300f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (CoolerItemVisualEffectMod.FinalFractalTailEffect == null) return false; if (CoolerItemVisualEffectMod.ColorfulEffect == null) return false;

            SpriteBatch spriteBatch = Main.spriteBatch;

            //{
            //    SpriteEffects spriteEffects = SpriteEffects.None;
            //    if (projectile.spriteDirection == -1)
            //    {
            //        spriteEffects = SpriteEffects.FlipHorizontally;
            //    }
            //    Vector2 vector57 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            //    Texture2D value108 = TextureAssets.Projectile[projectile.type].Value;
            //    Microsoft.Xna.Framework.Color alpha8 = projectile.GetAlpha(Color.White);
            //    Vector2 origin17 = new Vector2((float)value108.Width, (float)value108.Height) / 2f;
            //    float num319 = projectile.rotation;
            //    Vector2 vector58 = Vector2.One * projectile.scale;
            //    float lerpValue6 = Tools.GetLerpValue(0f, 8f, projectile.velocity.Length(), true);
            //    num319 *= lerpValue6;
            //    vector58 *= 0.6f;
            //    vector58.Y *= MathHelper.Lerp(1f, 0.8f, lerpValue6);
            //    vector58.X *= MathHelper.Lerp(1f, 1.5f, lerpValue6);
            //    Microsoft.Xna.Framework.Color color82 = new Microsoft.Xna.Framework.Color(80, 80, 80, 0);
            //    Vector2 vector67 = vector58 + vector58 * (float)Math.Cos((double)(projectile.localAI[0] / 60 * 6.28318548f)) * 0.4f;
            //    Vector2 spinningpoint23 = new Vector2(2f * vector67.X, 0f);
            //    double radians27 = (double)num319;
            //    Vector2 center = default(Vector2);
            //    Vector2 vector68 = spinningpoint23.RotatedBy(radians27, center);
            //    for (float num334 = 0f; num334 < 1f; num334 += 0.25f)
            //    {
            //        Texture2D texture14 = value108;
            //        Vector2 value123 = vector57;
            //        Vector2 spinningpoint24 = vector68;
            //        double radians28 = (double)(num334 * 6.28318548f);
            //        center = default(Vector2);
            //        spriteBatch.Draw(texture14, value123 + spinningpoint24.RotatedBy(radians28, center), null, color82, num319, origin17, vector67, spriteEffects, 0);
            //    }
            //    spriteBatch.Draw(value108, vector57, null, alpha8, num319, origin17, vector58, spriteEffects, 0);

            //}

            {
                List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

                // 把所有的点都生成出来，按照顺序
                for (int i = 1; i < projectile.oldPos.Length; ++i)
                {
                    if (projectile.oldPos[i] == Vector2.Zero) break;
                    //spriteBatch.Draw(TextureAssets.MagicPixel.Value, projectile.oldPos[i] - Main.screenPosition,
                    //    new Rectangle(0, 0, 1, 1), Color.White, 0f, new Vector2(0.5f, 0.5f), 5f, SpriteEffects.None, 0f);

                    //int width = 30;
                    var normalDir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                    normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

                    var factor = i / (float)projectile.oldPos.Length;
                    var color = Color.Lerp(Color.White, Color.Red, factor);
                    var w = MathHelper.Lerp(1f, 0.05f, factor);
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + normalDir * 30, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + normalDir * -15, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
                }

                List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();

                if (bars.Count > 2)
                {

                    // 按照顺序连接三角形
                    triangleList.Add(bars[0]);
                    var vertex = new CustomVertexInfo((bars[0].Position + bars[1].Position) * 0.5f + Vector2.Normalize(projectile.velocity) * 30, Color.White,
                        new Vector3(0, 0.5f, 1));
                    triangleList.Add(bars[1]);
                    triangleList.Add(vertex);
                    for (int i = 0; i < bars.Count - 2; i += 2)
                    {
                        triangleList.Add(bars[i]);
                        triangleList.Add(bars[i + 2]);
                        triangleList.Add(bars[i + 1]);

                        triangleList.Add(bars[i + 1]);
                        triangleList.Add(bars[i + 2]);
                        triangleList.Add(bars[i + 3]);
                    }


                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                    // 干掉注释掉就可以只显示三角形栅格
                    //RasterizerState rasterizerState = new RasterizerState();
                    //rasterizerState.CullMode = CullMode.None;
                    //rasterizerState.FillMode = FillMode.WireFrame;
                    //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));

                    // 把变换和所需信息丢给shader
                    if (Main.player[projectile.owner].name == "FFT")
                    {
                        CoolerItemVisualEffectMod.FinalFractalTailEffect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                        CoolerItemVisualEffectMod.FinalFractalTailEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.03f);
                        //CoolerItemVisualEffect.ColorfulEffect.Parameters["defaultColor"].SetValue(Main.hslToRgb(drawColor, 1f, 0.5f).ToVector4());
                        Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.HeatMap[7].Value;
                        Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.BaseTex[13].Value;
                        Main.graphics.GraphicsDevice.Textures[2] = LogSpiralLibraryMod.AniTex[5].Value;
                        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                        //Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                        //Main.graphics.GraphicsDevice.Textures[1] = TextureAssets.MagicPixel.Value;
                        //Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;
                        /*if (isCyan)
                        {
                            CoolerItemVisualEffect.CleverEffect.CurrentTechnique.Passes["Clever"].Apply();
                        }*/
                        CoolerItemVisualEffectMod.FinalFractalTailEffect.CurrentTechnique.Passes[0].Apply();
                    }
                    else
                    {
                        CoolerItemVisualEffectMod.ColorfulEffect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                        CoolerItemVisualEffectMod.ColorfulEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.03f);
                        CoolerItemVisualEffectMod.ColorfulEffect.Parameters["defaultColor"].SetValue(Main.hslToRgb(drawColor, 1f, 0.5f).ToVector4());
                        Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.HeatMap[7].Value;
                        Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.BaseTex[13].Value;
                        Main.graphics.GraphicsDevice.Textures[2] = LogSpiralLibraryMod.AniTex[5].Value;
                        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                        //Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                        //Main.graphics.GraphicsDevice.Textures[1] = TextureAssets.MagicPixel.Value;
                        //Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;
                        /*if (isCyan)
                        {
                            CoolerItemVisualEffect.CleverEffect.CurrentTechnique.Passes["Clever"].Apply();
                        }*/
                        CoolerItemVisualEffectMod.ColorfulEffect.CurrentTechnique.Passes[0].Apply();
                    }


                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                    spriteBatch.End();
                    spriteBatch.Begin();
                }
            }
            var tex = TextureAssets.Projectile[projectile.type].Value;
            DrawProjWithStarryTrail(spriteBatch);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, tex.Frame(), Color.White, projectile.rotation, tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        private void DrawProjWithStarryTrail(SpriteBatch spriteBatch)
        {
            Color color = new Color(255, 255, 255, (int)Color.White.A - projectile.alpha);
            Vector2 vector = projectile.velocity;
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
            float num4 = 60f;
            bool flag = false;
            float num5 = GetLerpValue(num4, num4 * 0.8333333f, projectile.localAI[0], true);
            float lerpValue = GetLerpValue(0f, 120f, Vector2.Distance(projectile.Center, center), true);
            num3 *= lerpValue;
            num5 *= GetLerpValue(0f, 15f, projectile.localAI[0], true);
            Color value = Main.hslToRgb(drawColor, 1f, 0.5f) * 0.15f * (num5 * num3);
            var spinningpoint = new Vector2(0f, -2f);
            float num6 = GetLerpValue(num4, num4 * 0.6666667f, projectile.localAI[0], true);
            num6 *= GetLerpValue(0f, 20f, projectile.localAI[0], true);
            var num = -0.3f * (1f - num6);
            num += -1f * GetLerpValue(15f, 0f, projectile.localAI[0], true);
            num *= num3;
            var scale = num5 * num3;
            Vector2 value2 = projectile.Center + vector;
            Texture2D value4 = LogSpiralLibraryMod.Misc[16].Value;
            Rectangle rectangle = value4.Frame(1, 1, 0, 0, 0, 0);
            Vector2 origin2 = new Vector2(rectangle.Width / 2f, 10f);
            Vector2 value5 = new Vector2(0f, projectile.gfxOffY);
            float num7 = (float)Main.time / 60f;
            Vector2 value6 = value2 + projectile.velocity * projectile.scale;
            Color color2 = value * scale;
            color2.A = 0;
            Color color3 = value * scale;
            color3.A = 0;
            Color color4 = value * scale;
            color4.A = 0;
            Vector2 value8 = value2 - vector * 0.5f;
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.rotation + MathHelper.PiOver4, origin2, 1.5f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 2.09439516f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color3, projectile.rotation + MathHelper.PiOver4, origin2, 1.1f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 4.18879032f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color4, projectile.rotation + MathHelper.PiOver4, origin2, 1.3f + num, SpriteEffects.None, 0);
            /*spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.rotation + MathHelper.PiOver4, origin2, (1.5f + num) * projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 2.09439516f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color3, projectile.rotation + MathHelper.PiOver4, origin2, (1.1f + num) * projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 4.18879032f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color4, projectile.rotation + MathHelper.PiOver4, origin2, (1.3f + num) * projectile.scale, SpriteEffects.None, 0);*/
            for (float num9 = 0f; num9 < 1f; num9 += 0.5f)
            {
                float num10 = num7 % 0.5f / 0.5f;
                num10 = (num10 + num9) % 1f;
                float num11 = num10 * 2f;
                if (num11 > 1f)
                {
                    num11 = 2f - num11;
                }
                //spriteBatch.Draw(value4, value8 - Main.screenPosition + value5, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * num11, projectile.velocity.ToRotation() + 1.57079637f, origin2, 0.3f + num10 * 0.5f, SpriteEffects.None, 0);
            }
            if (flag)
            {
                float rotation = projectile.rotation + projectile.localAI[1];
                Vector2 position = projectile.Center - Main.screenPosition;
                Texture2D value9 = LogSpiralLibraryMod.Misc[15].Value;
                Rectangle rectangle2 = value9.Frame(1, 8, 0, 0, 0, 0);
                Vector2 origin3 = rectangle2.Size() / 2f;
                spriteBatch.Draw(value9, position, new Rectangle?(rectangle2), color, rotation, origin3, projectile.scale, SpriteEffects.None, 0);
            }
        }
    }
    public class FinalFractalDimensionalSwoosh : ModProjectile
    {
        Projectile projectile => Projectile;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[projectile.owner] = 0;
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.PvP) target.immune = false;
            base.OnHitPlayer(target, info);
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.DamageType = DamageClass.Melee;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.manualDirectionChange = true;
            projectile.penetrate = -1;
            //projectile.hide = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 30;
        }
        private int timer = 0;
        private int timer2 = 0;
        private float drawColor;
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            timer2 = reader.ReadInt32();
            drawColor = reader.ReadSingle();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(timer2);
            writer.Write(drawColor);
        }
        public override void AI()
        {
            switch (Projectile.frame)
            {
                case 0:
                    drawColor = 0.0664557f;
                    break;
                case 1:
                    drawColor = 0.9329502f;
                    break;
                case 2:
                    drawColor = 0.6392276f;
                    break;
                case 3:
                    drawColor = 0.7272727f;
                    break;
                case 4:
                    drawColor = 0.6984127f;
                    break;
                case 5:
                    drawColor = 0.9936204f;
                    break;
                case 6:
                    drawColor = 0.6428571f;
                    break;
                case 7:
                    drawColor = 0.2643678f;
                    break;
                case 8:
                    drawColor = 0.05653451f;
                    break;
                case 9:
                    drawColor = 0.6860465f;
                    break;
                case 10:
                    drawColor = 0.1390169f;
                    break;
                case 11:
                    drawColor = 0.7558479f;
                    break;
                case 12:
                    drawColor = 0.84573f;
                    break;
                case 13:
                    drawColor = 0.4966667f;
                    break;
                case 14:
                    drawColor = 0.2311828f;
                    break;
                case 15:
                    drawColor = 0.09360731f;
                    break;
                case 16:
                    drawColor = 0.5325521f;
                    break;
                case 17:
                    drawColor = 0.9331396f;
                    break;
                case 18:
                    drawColor = 0.5114943f;
                    break;
                case 19:
                    drawColor = 0.6161616f;
                    break;
                case 20:
                    drawColor = 0.7021858f;
                    break;
                case 21:
                    drawColor = 0.5621212f;
                    break;
                case 22:
                    drawColor = 0.06666667f;
                    break;
                case 23:
                    drawColor = 0.3822844f;
                    break;
                case 24:
                    drawColor = 0.6319444f;
                    break;
                default:

                    break;
            }
            timer++;
            Main.projFrames[projectile.type] = 25;
            float num = 60f;
            float[] array = projectile.localAI;
            int num2 = 0;
            float num3 = array[num2] + 1f;
            array[num2] = num3;
            if (timer >= num)
            {
                projectile.Kill();
                return;
            }
            if (timer2 == 0)
            {
                timer2 = 1;
                projectile.frame = (int)projectile.ai[1];
            }
            projectile.velocity = projectile.velocity.RotatedBy((double)projectile.ai[0], default);
            projectile.Opacity = GetLerpValue(0f, 12f, projectile.localAI[0], true) * GetLerpValue(num, num - 12f, projectile.localAI[0], true);
            projectile.direction = projectile.velocity.X > 0f ? 1 : -1;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = 0.7853982f * projectile.spriteDirection + projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation += 3.14159274f;
            }
            if (projectile.localAI[0] > 7f)
            {
                //int num4 = 5;
                //projectile.Center -= new Vector2((float)num4);
                if (Main.rand.NextBool(15))
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, MyDustId.CyanBubble, null, 100, Color.Lerp(Main.hslToRgb(drawColor, 1f, 0.5f), Color.White, Main.rand.NextFloat() * 0.3f), 1f);
                    dust.scale = 0.7f;
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                    dust.velocity += projectile.velocity * 2f;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (CoolerItemVisualEffectMod.FinalFractalTailEffect == null) return false; if (CoolerItemVisualEffectMod.ColorfulEffect == null) return false;

            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 vector71 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D4 = TextureAssets.Projectile[projectile.type].Value;
            Rectangle rectangle29 = texture2D4.Frame(25, 1, 0, projectile.frame, 0, 0);
            Color color84 = Color.White;
            Vector2 origin21 = rectangle29.Size() / 2f;
            projectile.DrawProjWithStarryTrail(spriteBatch, drawColor, Color.White);
            color84 = Color.White * projectile.Opacity * 0.9f;
            color84.A /= 2;
            rectangle29 = texture2D4.Frame(25, 1, projectile.frame, 0, 0, 0);
            origin21 = rectangle29.Size() / 2f;
            projectile.DrawPrettyStarSparkle(spriteBatch, spriteEffects, vector71, color84, Main.hslToRgb(drawColor, 1f, 0.5f));
            spriteBatch.Draw(texture2D4, vector71, new Microsoft.Xna.Framework.Rectangle?(rectangle29), color84, projectile.rotation, origin21, projectile.scale, spriteEffects, 0);
            if (Main.player[projectile.owner].name == "FFT")
            {
                List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

                // 把所有的点都生成出来，按照顺序
                for (int i = 1; i < projectile.oldPos.Length; ++i)
                {
                    if (projectile.oldPos[i] == Vector2.Zero) break;
                    //spriteBatch.Draw(TextureAssets.MagicPixel.Value, projectile.oldPos[i] - Main.screenPosition,
                    //    new Rectangle(0, 0, 1, 1), Color.White, 0f, new Vector2(0.5f, 0.5f), 5f, SpriteEffects.None, 0f);

                    //int width = 30;
                    var normalDir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                    normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

                    var factor = i / (float)projectile.oldPos.Length;
                    var color = Color.Lerp(Color.White, Color.Red, factor);
                    var w = MathHelper.Lerp(1f, 0.05f, factor);
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + normalDir * 30, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + normalDir * -30, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
                }

                List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();

                if (bars.Count > 2)
                {

                    // 按照顺序连接三角形
                    triangleList.Add(bars[0]);
                    var vertex = new CustomVertexInfo((bars[0].Position + bars[1].Position) * 0.5f + Vector2.Normalize(projectile.velocity) * 30, Color.White,
                        new Vector3(0, 0.5f, 1));
                    triangleList.Add(bars[1]);
                    triangleList.Add(vertex);
                    for (int i = 0; i < bars.Count - 2; i += 2)
                    {
                        triangleList.Add(bars[i]);
                        triangleList.Add(bars[i + 2]);
                        triangleList.Add(bars[i + 1]);

                        triangleList.Add(bars[i + 1]);
                        triangleList.Add(bars[i + 2]);
                        triangleList.Add(bars[i + 3]);
                    }


                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                    // 干掉注释掉就可以只显示三角形栅格
                    //RasterizerState rasterizerState = new RasterizerState();
                    //rasterizerState.CullMode = CullMode.None;
                    //rasterizerState.FillMode = FillMode.WireFrame;
                    //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));

                    // 把变换和所需信息丢给shader
                    CoolerItemVisualEffectMod.FinalFractalTailEffect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                    CoolerItemVisualEffectMod.FinalFractalTailEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.03f);
                    //CoolerItemVisualEffect.FinalFractalTailEffect.Parameters["defaultColor"].SetValue(new Vector4(finalFractalProfile.trailColor.ToVector3(), finalFractalProfile.trailColor.A));
                    Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.HeatMap[7].Value;
                    Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.BaseTex[13].Value;
                    Main.graphics.GraphicsDevice.Textures[2] = LogSpiralLibraryMod.AniTex[5].Value;
                    Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                    //Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                    //Main.graphics.GraphicsDevice.Textures[1] = TextureAssets.MagicPixel.Value;
                    //Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;
                    /*if (isCyan)
					{
						CoolerItemVisualEffect.CleverEffect.CurrentTechnique.Passes["Clever"].Apply();
					}*/
                    CoolerItemVisualEffectMod.FinalFractalTailEffect.CurrentTechnique.Passes[0].Apply();


                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                    spriteBatch.End();
                    spriteBatch.Begin();
                }
            }
            else
            {
                List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

                // 把所有的点都生成出来，按照顺序
                for (int i = 1; i < projectile.oldPos.Length; ++i)
                {
                    if (projectile.oldPos[i] == Vector2.Zero) break;
                    //spriteBatch.Draw(TextureAssets.MagicPixel.Value, projectile.oldPos[i] - Main.screenPosition,
                    //    new Rectangle(0, 0, 1, 1), Color.White, 0f, new Vector2(0.5f, 0.5f), 5f, SpriteEffects.None, 0f);

                    //int width = 30;
                    var normalDir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                    normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

                    var factor = i / (float)projectile.oldPos.Length;
                    var color = Color.Lerp(Color.White, Color.Red, factor);
                    var w = MathHelper.Lerp(1f, 0.05f, factor);
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + normalDir * 30, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + normalDir * -15, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
                }

                List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();

                if (bars.Count > 2)
                {

                    // 按照顺序连接三角形
                    triangleList.Add(bars[0]);
                    var vertex = new CustomVertexInfo((bars[0].Position + bars[1].Position) * 0.5f + Vector2.Normalize(projectile.velocity) * 30, Color.White,
                        new Vector3(0, 0.5f, 1));
                    triangleList.Add(bars[1]);
                    triangleList.Add(vertex);
                    for (int i = 0; i < bars.Count - 2; i += 2)
                    {
                        triangleList.Add(bars[i]);
                        triangleList.Add(bars[i + 2]);
                        triangleList.Add(bars[i + 1]);

                        triangleList.Add(bars[i + 1]);
                        triangleList.Add(bars[i + 2]);
                        triangleList.Add(bars[i + 3]);
                    }


                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                    // 干掉注释掉就可以只显示三角形栅格
                    //RasterizerState rasterizerState = new RasterizerState();
                    //rasterizerState.CullMode = CullMode.None;
                    //rasterizerState.FillMode = FillMode.WireFrame;
                    //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));

                    // 把变换和所需信息丢给shader
                    CoolerItemVisualEffectMod.ColorfulEffect.Parameters["uTransform"].SetValue(model * Main.GameViewMatrix.TransformationMatrix * projection);
                    CoolerItemVisualEffectMod.ColorfulEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.03f);
                    CoolerItemVisualEffectMod.ColorfulEffect.Parameters["defaultColor"].SetValue(Main.hslToRgb(drawColor, 1f, 0.5f).ToVector4());
                    Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.HeatMap[7].Value;
                    Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.BaseTex[13].Value;
                    Main.graphics.GraphicsDevice.Textures[2] = LogSpiralLibraryMod.AniTex[5].Value;
                    Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                    //Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                    //Main.graphics.GraphicsDevice.Textures[1] = TextureAssets.MagicPixel.Value;
                    //Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;
                    /*if (isCyan)
                    {
                        CoolerItemVisualEffect.CleverEffect.CurrentTechnique.Passes["Clever"].Apply();
                    }*/
                    CoolerItemVisualEffectMod.ColorfulEffect.CurrentTechnique.Passes[0].Apply();


                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                    spriteBatch.End();
                    spriteBatch.Begin();
                }
            }
            return false;
        }
    }
    public class FinalFractalDimensionalSwoosh2 : ModProjectile
    {
        Projectile projectile => Projectile;

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.DamageType = DamageClass.Melee;
            projectile.penetrate = 3;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.friendly = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[projectile.owner] = 0;
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.PvP) target.immune = false;
            base.OnHitPlayer(target, info);
        }
        private int timer = 0;
        public override void AI()
        {
            Main.projFrames[projectile.type] = 24;
            if (timer == 0)
            {
                timer = 1;
                projectile.frame = Main.rand.Next(24);
            }
            float num = projectile.light;
            float num2 = projectile.light;
            float num3 = projectile.light;
            if (projectile.localAI[1] > 7f)
            {
                int dustID = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X * 4f + 2f,
                    projectile.position.Y + 2f - projectile.velocity.Y * 4f), 8, 8, MyDustId.RedBubble, projectile.oldVelocity.X,
                    projectile.oldVelocity.Y, 100, default(Color), 1.25f);
                Dust dust = Main.dust[dustID];
                dust.velocity *= -0.25f;
                dust.noGravity = true;
                dustID = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X * 4f + 2f,
                    projectile.position.Y + 2f - projectile.velocity.Y * 4f), 8, 8, MyDustId.RedBubble, projectile.oldVelocity.X,
                    projectile.oldVelocity.Y, 100, default(Color), 1.25f);
                dust = Main.dust[dustID];
                dust.noGravity = true;
                dust.velocity *= -0.25f;
                dust = Main.dust[dustID];
                dust.noGravity = true;
                dust.position -= projectile.velocity * 0.5f;
                /*Dust dust1 = Dust.NewDustDirect(projectile.position - projectile.velocity * 4f, 8, 8,
                MyDustId.GreenFXPowder, projectile.oldVelocity.X,
                projectile.oldVelocity.Y, 100, default(Color), 1f);
                dust1.velocity *= -0.25f;
                Dust dust2 = Dust.NewDustDirect(projectile.position - projectile.velocity * 4f, 8, 8,
                    MyDustId.GreenFXPowder, projectile.oldVelocity.X,
                    projectile.oldVelocity.Y, 100, default(Color), 2f);
                dust2.velocity *= -0.25f;
                dust2.noGravity = true;
                dust2.position -= projectile.velocity * 0.5f;*/
            }
            if (projectile.localAI[1] < 15f)
            {
                projectile.localAI[1] += 1f;
            }
            else
            {
                if (projectile.localAI[0] == 0f)
                {
                    projectile.scale -= 0.02f;
                    projectile.alpha += 30;
                    if (projectile.alpha >= 250)
                    {
                        projectile.alpha = 255;
                        projectile.localAI[0] = 1f;
                    }
                }
                else if (projectile.localAI[0] == 1f)
                {
                    projectile.scale += 0.02f;
                    projectile.alpha -= 30;
                    if (projectile.alpha <= 0)
                    {
                        projectile.alpha = 0;
                        projectile.localAI[0] = 0f;
                    }
                }
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item60, projectile.position);
            }
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 0.785f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
            num *= 0.2f;
            num3 *= 0.6f;
            Lighting.AddLight((int)((projectile.position.X + (float)(projectile.width / 2)) / 16f), (int)((projectile.position.Y + (float)(projectile.height / 2)) / 16f), num, num2, num3);
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, projectile.position);
            int num3;
            for (int num798 = 4; num798 < 31; num798 = num3 + 1)
            {
                float num799 = projectile.oldVelocity.X * (30f / num798);
                float num800 = projectile.oldVelocity.Y * (30f / num798);
                int num801 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num799, projectile.oldPosition.Y - num800), 8, 8, MyDustId.RedBubble, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default(Color), 1.8f);
                Main.dust[num801].noGravity = true;
                Dust dust = Main.dust[num801];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                num801 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num799, projectile.oldPosition.Y - num800), 8, 8, MyDustId.RedBubble, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default(Color), 1.4f);
                dust = Main.dust[num801];
                dust.velocity *= 0.05f;
                dust.noGravity = true;
                num3 = num798;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.localAI[1] >= 15f)
            {
                return new Color(255, 255, 255, projectile.alpha);
            }
            if (projectile.localAI[1] < 5f)
            {
                return Color.Transparent;
            }
            int num7 = (int)((projectile.localAI[1] - 5f) / 10f * 255f);
            return new Color(num7, num7, num7, num7);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (CoolerItemVisualEffectMod.FinalFractalTailEffect == null) return false; if (CoolerItemVisualEffectMod.ColorfulEffect == null) return false;

            SpriteBatch spriteBatch = Main.spriteBatch;
            /*spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value,
                new Vector2(projectile.position.X - Main.screenPosition.X + (float)(projectile.width / 2),
                projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2)),
                new Rectangle(0, 0, TextureAssets.Projectile[projectile.type].Value.Width, TextureAssets.Projectile[projectile.type].Value.Height),
                Color.White, projectile.rotation,
                new Vector2(TextureAssets.Projectile[projectile.type].Value.Width, 0f),
                projectile.scale, SpriteEffects.None, 0f);
            return false;*/
            var tex = TextureAssets.Projectile[projectile.type].Value;
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, new Rectangle(0, 84 * projectile.frame, 90, 84), Color.White, projectile.rotation, new Vector2(0, 84), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class Zenith_FirstFractal : ModProjectile
    {
        Projectile projectile => Projectile;

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.DamageType = DamageClass.Melee;
            projectile.penetrate = 1;
            projectile.light = 0.5f;
            projectile.timeLeft = 180;
            projectile.alpha = 255;
            projectile.friendly = true;
        }
        private float drawColor;
        private int timer;
        public override void AI()
        {
            float[] array = projectile.localAI;
            int num4 = 0;
            float num5 = array[num4] + 1f;
            array[num4] = num5;
            projectile.localAI[0] = 50;
            if (timer == 0)
            {
                timer = Main.rand.Next(1, 7);
            }
            switch (timer)
            {
                case 6:
                    drawColor = Main.rgbToHsl(new Color(108, 134, 230)).X;
                    break;
                case 1:
                    drawColor = Main.rgbToHsl(new Color(178, 165, 226)).X;
                    break;
                case 2:
                    drawColor = Main.rgbToHsl(new Color(182, 109, 190)).X;
                    break;
                case 3:
                    drawColor = Main.rgbToHsl(new Color(99, 74, 187)).X;
                    break;
                case 4:
                    drawColor = Main.rgbToHsl(new Color(173, 214, 193)).X;
                    break;
                case 5:
                    drawColor = Main.rgbToHsl(new Color(100, 203, 189)).X;
                    break;
                default:

                    break;
            }
            float num = projectile.light;
            float num2 = projectile.light;
            float num3 = projectile.light;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item60, projectile.position);
            }
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 0.785f;
            num *= 0.2f;
            num3 *= 0.6f;
            Lighting.AddLight((int)((projectile.position.X + (float)(projectile.width / 2)) / 16f), (int)((projectile.position.Y + (float)(projectile.height / 2)) / 16f), num, num2, num3);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.localAI[1] >= 15f)
            {
                return new Color(255, 255, 255, projectile.alpha);
            }
            if (projectile.localAI[1] < 5f)
            {
                return Color.Transparent;
            }
            int num7 = (int)((projectile.localAI[1] - 5f) / 10f * 255f);
            return new Color(num7, num7, num7, num7);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int n = 0; n < 6; n++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), projectile.Center, new Vector2(32, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 6 * n), ProjectileType<Zenith_FirstFractals>(), projectile.damage, 0, projectile.owner);
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int n = 0; n < 6; n++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), projectile.Center + new Vector2(64, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 6 * n), new Vector2(32, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 6 * n + MathHelper.TwoPi / 3), ProjectileType<Zenith_FirstFractals>(), projectile.damage, 0, projectile.owner);
            }
            target.immune[projectile.owner] = 0;
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.PvP) 
            {
                for (int n = 0; n < 6; n++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), projectile.Center + new Vector2(64, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 6 * n), new Vector2(32, 0).RotatedBy(projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / 6 * n + MathHelper.TwoPi / 3), ProjectileType<Zenith_FirstFractals>(), projectile.damage, 0, projectile.owner);
                }
                target.immune = false;
            }
            base.OnHitPlayer(target, info);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (CoolerItemVisualEffectMod.FinalFractalTailEffect == null) return false; if (CoolerItemVisualEffectMod.ColorfulEffect == null) return false;

            SpriteBatch spriteBatch = Main.spriteBatch;

            //{
            //    SpriteEffects spriteEffects = SpriteEffects.None;
            //    if (projectile.spriteDirection == -1)
            //    {
            //        spriteEffects = SpriteEffects.FlipHorizontally;
            //    }
            //    Vector2 vector57 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            //    Texture2D value108 = TextureAssets.Projectile[projectile.type].Value;
            //    Microsoft.Xna.Framework.Color alpha8 = projectile.GetAlpha(Color.White);
            //    Vector2 origin17 = new Vector2((float)value108.Width, (float)value108.Height) / 2f;
            //    float num319 = projectile.rotation;
            //    Vector2 vector58 = Vector2.One * projectile.scale;
            //    float lerpValue6 = Tools.GetLerpValue(0f, 8f, projectile.velocity.Length(), true);
            //    num319 *= lerpValue6;
            //    vector58 *= 0.6f;
            //    vector58.Y *= MathHelper.Lerp(1f, 0.8f, lerpValue6);
            //    vector58.X *= MathHelper.Lerp(1f, 1.5f, lerpValue6);
            //    Microsoft.Xna.Framework.Color color82 = new Microsoft.Xna.Framework.Color(80, 80, 80, 0);
            //    Vector2 vector67 = vector58 + vector58 * (float)Math.Cos((double)(projectile.localAI[0] / 60 * 6.28318548f)) * 0.4f;
            //    Vector2 spinningpoint23 = new Vector2(2f * vector67.X, 0f);
            //    double radians27 = (double)num319;
            //    Vector2 center = default(Vector2);
            //    Vector2 vector68 = spinningpoint23.RotatedBy(radians27, center);
            //    for (float num334 = 0f; num334 < 1f; num334 += 0.25f)
            //    {
            //        Texture2D texture14 = value108;
            //        Vector2 value123 = vector57;
            //        Vector2 spinningpoint24 = vector68;
            //        double radians28 = (double)(num334 * 6.28318548f);
            //        center = default(Vector2);
            //        spriteBatch.Draw(texture14, value123 + spinningpoint24.RotatedBy(radians28, center), null, color82, num319, origin17, vector67, spriteEffects, 0);
            //    }
            //    spriteBatch.Draw(value108, vector57, null, alpha8, num319, origin17, vector58, spriteEffects, 0);

            //}
            var tex = TextureAssets.Projectile[projectile.type].Value;
            DrawProjWithStarryTrail(spriteBatch);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, tex.Frame(), Color.White, projectile.rotation, tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        private void DrawProjWithStarryTrail(SpriteBatch spriteBatch)
        {
            Color color = new Color(255, 255, 255, (int)Color.White.A - projectile.alpha);
            Vector2 vector = projectile.velocity;
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
            float num4 = 60f;
            bool flag = false;
            float num5 = GetLerpValue(num4, num4 * 0.8333333f, projectile.localAI[0], true);
            float lerpValue = GetLerpValue(0f, 120f, Vector2.Distance(projectile.Center, center), true);
            num3 *= lerpValue;
            num5 *= GetLerpValue(0f, 15f, projectile.localAI[0], true);
            Color value = Main.hslToRgb(drawColor, 1f, 0.5f) * 0.15f * (num5 * num3);
            var spinningpoint = new Vector2(0f, -2f);
            float num6 = GetLerpValue(num4, num4 * 0.6666667f, projectile.localAI[0], true);
            num6 *= GetLerpValue(0f, 20f, projectile.localAI[0], true);
            var num = -0.3f * (1f - num6);
            num += -1f * GetLerpValue(15f, 0f, projectile.localAI[0], true);
            num *= num3;
            var scale = num5 * num3;
            Vector2 value2 = projectile.Center + vector;
            Texture2D value4 = LogSpiralLibraryMod.Misc[17].Value;
            Rectangle rectangle = value4.Frame(1, 1, 0, 0, 0, 0);
            Vector2 origin2 = new Vector2(rectangle.Width / 2f, 10f);
            Vector2 value5 = new Vector2(0f, projectile.gfxOffY);
            float num7 = (float)Main.time / 60f;
            Vector2 value6 = value2 + projectile.velocity * projectile.scale;
            Color color2 = value * scale;
            color2.A = 0;
            Color color3 = value * scale;
            color3.A = 0;
            Color color4 = value * scale;
            color4.A = 0;
            Vector2 value8 = value2 - vector * 0.5f;
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.5f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 2.09439516f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color3, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.1f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 4.18879032f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color4, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.3f + num, SpriteEffects.None, 0);
            for (float num9 = 0f; num9 < 1f; num9 += 0.5f)
            {
                float num10 = num7 % 0.5f / 0.5f;
                num10 = (num10 + num9) % 1f;
                float num11 = num10 * 2f;
                if (num11 > 1f)
                {
                    num11 = 2f - num11;
                }
                //spriteBatch.Draw(value4, value8 - Main.screenPosition + value5, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * num11, projectile.velocity.ToRotation() + 1.57079637f, origin2, 0.3f + num10 * 0.5f, SpriteEffects.None, 0);
            }
            if (flag)
            {
                float rotation = projectile.rotation + projectile.localAI[1];
                Vector2 position = projectile.Center - Main.screenPosition;
                Texture2D value9 = LogSpiralLibraryMod.Misc[15].Value;
                Rectangle rectangle2 = value9.Frame(1, 8, 0, 0, 0, 0);
                Vector2 origin3 = rectangle2.Size() / 2f;
                spriteBatch.Draw(value9, position, new Rectangle?(rectangle2), color, rotation, origin3, projectile.scale, SpriteEffects.None, 0);
            }
        }
    }
    public class Zenith_FirstFractals : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.DamageType = DamageClass.Melee;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.light = 0.5f;
            projectile.timeLeft = 180;
            projectile.alpha = 255;
            projectile.friendly = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[projectile.owner] = 0;
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.PvP) target.immune = false;
            base.OnHitPlayer(target, info);
        }

        private float drawColor
        {
            get => projectile.ai[0];
            set { projectile.ai[0] = value; }
        }
        public override void AI()
        {
            float[] array = projectile.localAI;
            int num4 = 0;
            float num5 = array[num4] + 1f;
            array[num4] = num5;
            projectile.localAI[0] = 50;
            if (drawColor == 0)
            {
                drawColor = Main.rand.Next(1, 7);
            }
            switch (drawColor)
            {
                case 6:
                    drawColor = Main.rgbToHsl(new Color(108, 134, 230)).X;
                    break;
                case 1:
                    drawColor = Main.rgbToHsl(new Color(178, 165, 226)).X;
                    break;
                case 2:
                    drawColor = Main.rgbToHsl(new Color(182, 109, 190)).X;
                    break;
                case 3:
                    drawColor = Main.rgbToHsl(new Color(99, 74, 187)).X;
                    break;
                case 4:
                    drawColor = Main.rgbToHsl(new Color(173, 214, 193)).X;
                    break;
                case 5:
                    drawColor = Main.rgbToHsl(new Color(100, 203, 189)).X;
                    break;
                default:

                    break;
            }
            float num = projectile.light;
            float num2 = projectile.light;
            float num3 = projectile.light;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item60, projectile.position);
            }
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 0.785f;
            num *= 0.2f;
            num3 *= 0.6f;
            Lighting.AddLight((int)((projectile.position.X + (float)(projectile.width / 2)) / 16f), (int)((projectile.position.Y + (float)(projectile.height / 2)) / 16f), num, num2, num3);
        }
        Projectile projectile => Projectile;

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.localAI[1] >= 15f)
            {
                return new Color(255, 255, 255, projectile.alpha);
            }
            if (projectile.localAI[1] < 5f)
            {
                return Color.Transparent;
            }
            int num7 = (int)((projectile.localAI[1] - 5f) / 10f * 255f);
            return new Color(num7, num7, num7, num7);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (CoolerItemVisualEffectMod.FinalFractalTailEffect == null) return false; if (CoolerItemVisualEffectMod.ColorfulEffect == null) return false;

            SpriteBatch spriteBatch = Main.spriteBatch;
            //{
            //    SpriteEffects spriteEffects = SpriteEffects.None;
            //    if (projectile.spriteDirection == -1)
            //    {
            //        spriteEffects = SpriteEffects.FlipHorizontally;
            //    }
            //    Vector2 vector57 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            //    Texture2D value108 = TextureAssets.Projectile[projectile.type].Value;
            //    Microsoft.Xna.Framework.Color alpha8 = projectile.GetAlpha(Color.White);
            //    Vector2 origin17 = new Vector2((float)value108.Width, (float)value108.Height) / 2f;
            //    float num319 = projectile.rotation;
            //    Vector2 vector58 = Vector2.One * projectile.scale;
            //    float lerpValue6 = Tools.GetLerpValue(0f, 8f, projectile.velocity.Length(), true);
            //    num319 *= lerpValue6;
            //    vector58 *= 0.6f;
            //    vector58.Y *= MathHelper.Lerp(1f, 0.8f, lerpValue6);
            //    vector58.X *= MathHelper.Lerp(1f, 1.5f, lerpValue6);
            //    Microsoft.Xna.Framework.Color color82 = new Microsoft.Xna.Framework.Color(80, 80, 80, 0);
            //    Vector2 vector67 = vector58 + vector58 * (float)Math.Cos((double)(projectile.localAI[0] / 60 * 6.28318548f)) * 0.4f;
            //    Vector2 spinningpoint23 = new Vector2(2f * vector67.X, 0f);
            //    double radians27 = (double)num319;
            //    Vector2 center = default(Vector2);
            //    Vector2 vector68 = spinningpoint23.RotatedBy(radians27, center);
            //    for (float num334 = 0f; num334 < 1f; num334 += 0.25f)
            //    {
            //        Texture2D texture14 = value108;
            //        Vector2 value123 = vector57;
            //        Vector2 spinningpoint24 = vector68;
            //        double radians28 = (double)(num334 * 6.28318548f);
            //        center = default(Vector2);
            //        spriteBatch.Draw(texture14, value123 + spinningpoint24.RotatedBy(radians28, center), null, color82, num319, origin17, vector67, spriteEffects, 0);
            //    }
            //    spriteBatch.Draw(value108, vector57, null, alpha8, num319, origin17, vector58, spriteEffects, 0);

            //}
            var tex = TextureAssets.Projectile[projectile.type].Value;
            DrawProjWithStarryTrail(spriteBatch);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, tex.Frame(), Color.White, projectile.rotation, tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        private void DrawProjWithStarryTrail(SpriteBatch spriteBatch)
        {
            Color color = new Color(255, 255, 255, (int)Color.White.A - projectile.alpha);
            Vector2 vector = projectile.velocity;
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
            float num4 = 60f;
            bool flag = false;
            float num5 = GetLerpValue(num4, num4 * 0.8333333f, projectile.localAI[0], true);
            float lerpValue = GetLerpValue(0f, 120f, Vector2.Distance(projectile.Center, center), true);
            num3 *= lerpValue;
            num5 *= GetLerpValue(0f, 15f, projectile.localAI[0], true);
            Color value = Main.hslToRgb(drawColor, 1f, 0.5f) * 0.15f * (num5 * num3);
            var spinningpoint = new Vector2(0f, -2f);
            float num6 = GetLerpValue(num4, num4 * 0.6666667f, projectile.localAI[0], true);
            num6 *= GetLerpValue(0f, 20f, projectile.localAI[0], true);
            var num = -0.3f * (1f - num6);
            num += -1f * GetLerpValue(15f, 0f, projectile.localAI[0], true);
            num *= num3;
            var scale = num5 * num3;
            Vector2 value2 = projectile.Center + vector;
            Texture2D value4 = LogSpiralLibraryMod.Misc[17].Value;
            Rectangle rectangle = value4.Frame(1, 1, 0, 0, 0, 0);
            Vector2 origin2 = new Vector2(rectangle.Width / 2f, 10f);
            Vector2 value5 = new Vector2(0f, projectile.gfxOffY);
            float num7 = (float)Main.time / 60f;
            Vector2 value6 = value2 + projectile.velocity * projectile.scale;
            Color color2 = value * scale;
            color2.A = 0;
            Color color3 = value * scale;
            color3.A = 0;
            Color color4 = value * scale;
            color4.A = 0;
            Vector2 value8 = value2 - vector * 0.5f;
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.5f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 2.09439516f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color3, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.1f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 4.18879032f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color4, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.3f + num, SpriteEffects.None, 0);
            for (float num9 = 0f; num9 < 1f; num9 += 0.5f)
            {
                float num10 = num7 % 0.5f / 0.5f;
                num10 = (num10 + num9) % 1f;
                float num11 = num10 * 2f;
                if (num11 > 1f)
                {
                    num11 = 2f - num11;
                }
                //spriteBatch.Draw(value4, value8 - Main.screenPosition + value5, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * num11, projectile.velocity.ToRotation() + 1.57079637f, origin2, 0.3f + num10 * 0.5f, SpriteEffects.None, 0);
            }
            if (flag)
            {
                float rotation = projectile.rotation + projectile.localAI[1];
                Vector2 position = projectile.Center - Main.screenPosition;
                Texture2D value9 = LogSpiralLibraryMod.Misc[15].Value;
                Rectangle rectangle2 = value9.Frame(1, 8, 0, 0, 0, 0);
                Vector2 origin3 = rectangle2.Size() / 2f;
                spriteBatch.Draw(value9, position, new Rectangle?(rectangle2), color, rotation, origin3, projectile.scale, SpriteEffects.None, 0);
            }
        }
    }
    public class FinalFractalDimensionalSwoosh3 : ModProjectile
    {
        private Vector2 P;
        private Vector2 M;
        private float T;
        private const float MT = 180;
        private const float d = 128;
        private float h;
        public override void SetDefaults()
        {
            projectile.DamageType = DamageClass.Melee;
            projectile.width = 16;
            projectile.height = 16;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            //projectile.alpha = 255;
            projectile.friendly = true;
            projectile.extraUpdates = 3;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[projectile.owner] = 0;
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.PvP) target.immune = false;
            base.OnHitPlayer(target, info);
        }
        private float drawColor;
        private int timer;
        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.localAI[1] >= 15f)
            {
                return new Color(255, 255, 255, projectile.alpha);
            }
            if (projectile.localAI[1] < 5f)
            {
                return Color.Transparent;
            }
            int num7 = (int)((projectile.localAI[1] - 5f) / 10f * 255f);
            return new Color(num7, num7, num7, num7);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (CoolerItemVisualEffectMod.FinalFractalTailEffect == null) return false; if (CoolerItemVisualEffectMod.ColorfulEffect == null) return false;

            SpriteBatch spriteBatch = Main.spriteBatch;
            var tex = TextureAssets.Projectile[projectile.type].Value;
            DrawProjWithStarryTrail(spriteBatch);
            var rectangle29 = tex.Frame(25, 1, projectile.frame, 0, 0, 0);
            Vector2 vector71 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            //spriteBatch.Draw(tex, vector71, rectangle29, new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha), projectile.rotation, tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        Projectile projectile => Projectile;

        private void DrawProjWithStarryTrail(SpriteBatch spriteBatch)
        {
            Color color = new Color(255, 255, 255, (int)Color.White.A - projectile.alpha);
            Vector2 vector = projectile.velocity;
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
            float num4 = 60f;
            bool flag = false;
            float num5 = GetLerpValue(num4, num4 * 0.8333333f, projectile.localAI[0], true);
            float lerpValue = GetLerpValue(0f, 120f, Vector2.Distance(projectile.Center, center), true);
            num3 *= lerpValue;
            num5 *= GetLerpValue(0f, 15f, projectile.localAI[0], true);
            Color value = Main.hslToRgb(drawColor, 1f, 0.5f) * 0.15f * (num5 * num3);
            var spinningpoint = new Vector2(0f, -2f);
            float num6 = GetLerpValue(num4, num4 * 0.6666667f, projectile.localAI[0], true);
            num6 *= GetLerpValue(0f, 20f, projectile.localAI[0], true);
            var num = -0.3f * (1f - num6);
            num += -1f * GetLerpValue(15f, 0f, projectile.localAI[0], true);
            num *= num3;
            var scale = num5 * num3;
            Vector2 value2 = projectile.Center + vector;
            Texture2D value4 = LogSpiralLibraryMod.Misc[17].Value;
            Rectangle rectangle = value4.Frame(1, 1, 0, 0, 0, 0);
            Vector2 origin2 = new Vector2(rectangle.Width / 2f, 10f);
            Vector2 value5 = new Vector2(0f, projectile.gfxOffY);
            float num7 = (float)Main.time / 60f;
            Vector2 value6 = value2 - projectile.velocity * projectile.scale;
            Color color2 = value * scale;
            color2.A = 0;
            Color color3 = value * scale;
            color3.A = 0;
            Color color4 = value * scale;
            color4.A = 0;
            Vector2 value8 = value2 - vector * 0.5f;
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.5f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 2.09439516f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color3, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.1f + num, SpriteEffects.None, 0);
            spriteBatch.Draw(value4, value6 - Main.screenPosition + value5 + spinningpoint.RotatedBy((double)(6.28318548f * num7 + 4.18879032f), default(Vector2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color4, projectile.velocity.ToRotation() + 1.57079637f, origin2, 1.3f + num, SpriteEffects.None, 0);
            for (float num9 = 0f; num9 < 1f; num9 += 0.5f)
            {
                float num10 = num7 % 0.5f / 0.5f;
                num10 = (num10 + num9) % 1f;
                float num11 = num10 * 2f;
                if (num11 > 1f)
                {
                    num11 = 2f - num11;
                }
                //spriteBatch.Draw(value4, value8 - Main.screenPosition + value5, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * num11, projectile.velocity.ToRotation() + 1.57079637f, origin2, 0.3f + num10 * 0.5f, SpriteEffects.None, 0);
            }
            if (flag)
            {
                float rotation = projectile.rotation + projectile.localAI[1];
                Vector2 position = projectile.Center - Main.screenPosition;
                Texture2D value9 = LogSpiralLibraryMod.Misc[15].Value;
                Rectangle rectangle2 = value9.Frame(1, 8, 0, 0, 0, 0);
                Vector2 origin3 = rectangle2.Size() / 2f;
                spriteBatch.Draw(value9, position, new Rectangle?(rectangle2), color, rotation, origin3, projectile.scale, SpriteEffects.None, 0);
            }
        }
        public override void AI()
        {
            projectile.frame = (int)projectile.ai[1];
            if (timer == 0)
            {
                timer = Main.rand.Next(0, 25);
            }
            switch (timer)
            {
                case 0:
                    drawColor = 0.0664557f;
                    break;
                case 1:
                    drawColor = 0.9329502f;
                    break;
                case 2:
                    drawColor = 0.6392276f;
                    break;
                case 3:
                    drawColor = 0.7272727f;
                    break;
                case 4:
                    drawColor = 0.6984127f;
                    break;
                case 5:
                    drawColor = 0.9936204f;
                    break;
                case 6:
                    drawColor = 0.6428571f;
                    break;
                case 7:
                    drawColor = 0.2643678f;
                    break;
                case 8:
                    drawColor = 0.05653451f;
                    break;
                case 9:
                    drawColor = 0.6860465f;
                    break;
                case 10:
                    drawColor = 0.1390169f;
                    break;
                case 11:
                    drawColor = 0.7558479f;
                    break;
                case 12:
                    drawColor = 0.84573f;
                    break;
                case 13:
                    drawColor = 0.4966667f;
                    break;
                case 14:
                    drawColor = 0.2311828f;
                    break;
                case 15:
                    drawColor = 0.09360731f;
                    break;
                case 16:
                    drawColor = 0.5325521f;
                    break;
                case 17:
                    drawColor = 0.9331396f;
                    break;
                case 18:
                    drawColor = 0.5114943f;
                    break;
                case 19:
                    drawColor = 0.6161616f;
                    break;
                case 20:
                    drawColor = 0.7021858f;
                    break;
                case 21:
                    drawColor = 0.5621212f;
                    break;
                case 22:
                    drawColor = 0.06666667f;
                    break;
                case 23:
                    drawColor = 0.3822844f;
                    break;
                case 24:
                    drawColor = 0.6319444f;
                    break;
                default:

                    break;
            }
            float[] array = projectile.localAI;
            int num4 = 0;
            float num5 = array[num4] + 1f;
            array[num4] = num5;
            projectile.localAI[0] = 50;
            projectile.alpha = (int)Math.Abs(255f - T / MT * 510f);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver4;
            T++;
            Player player = Main.player[projectile.owner];
            if (T <= 1)
            {
                M = Main.MouseWorld + new Vector2(Main.rand.NextFloat(0, 64), 0).RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi));
                P = player.Center;
                h = Vector2.Distance(M, P);
            }
            float x = T - MT / 2 + projectile.ai[0];
            Vector2 TargetPos = new Vector2(-h / (float)Math.Pow(d, 2) * (float)Math.Pow(x, 2) + h, x).RotatedBy((M - P).ToRotation()) + P;
            projectile.velocity = TargetPos - projectile.Center;
            if (T > MT)
            {
                projectile.Kill();
            }
        }
    }
}
