using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using System.Collections.Generic;
using System;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using System.Linq;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using static CoolerItemVisualEffect.CoolerItemVisualEffectMethods;
using static CoolerItemVisualEffect.CoolerItemVisualEffect;
using static CoolerItemVisualEffect.ConfigurationSwoosh_Advanced;
using Terraria.GameContent;
using CoolerItemVisualEffect;
using System.IO;

namespace CoolerItemVisualEffect.Weapons
{
    public class PureFractal : ModItem
    {
        //public override void SetStaticDefaults()
        //{
        //    DisplayName.SetDefault("纯粹分形");
        //    Tooltip.SetDefault("远古纯粹的自然魔法洗礼之后的天顶之锋，展露吧，你最初的锋芒。");

        //}
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var time = ((float)Math.Sin(ModTime / 60f * MathHelper.TwoPi) + 1) * .5f;
            Color color;
            if (time < 0.5f) color = Color.Lerp(Color.Cyan, Color.Green, time * 2f);
            else color = Color.Lerp(Color.Green, Color.Yellow, time * 2f - 1);
            tooltips.Add(new TooltipLine(Mod, "PureSuggestion", Language.GetTextValue("Mods.CoolerItemVisualEffect.FinalFractalTip.0")) { OverrideColor = color });//"这甚至还不是它们的最终形态"
        }

        Item item => Item;
        public Texture2D tex => TextureAssets.Item[item.type].Value;
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            item.ShaderItemEffectInventory(spriteBatch, position, origin, GetTexture("ItemEffectTex_0"), Color.Lerp(new Color(0, 162, 232), new Color(34, 177, 76), (float)Math.Sin(MathHelper.Pi / 60 * ModTime) / 2 + 0.5f), scale);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.ShaderItemEffectInWorld(spriteBatch, GetTexture("ItemEffectTex_0"), Color.Lerp(new Color(0, 162, 232), new Color(34, 177, 76), (float)Math.Sin(MathHelper.Pi / 60 * ModTime) / 2 + 0.5f), rotation);
        }
        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.Swing;
            item.width = 56;
            item.height = 56;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.DamageType = DamageClass.Melee;
            item.DamageType = DamageClass.Melee;
            item.shoot = ProjectileType<PureFractalProj>();
            item.useAnimation = 30;
            item.useTime = item.useAnimation / 3;
            item.shootSpeed = 16f;
            item.damage = 240;
            item.knockBack = 6.5f;
            item.value = Item.sellPrice(0, 20, 0, 0);
            item.crit = 10;
            item.rare = ItemRarityID.Purple;
            item.noUseGraphic = true;
            item.noMelee = true;
        }
        //public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Zenith).AddIngredient<FirstFractal_CIVE>().AddTile(TileID.LunarCraftingStation).Register();
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.QuickAddIngredient(ItemID.Zenith);
            recipe.AddIngredient<FirstFractal_CIVE>();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
        public override bool CanUseItem(Player player)
        {
            //if (player.name == "FFT")
            //{
            //	item.useAnimation = 20;
            //	item.useTime = item.useAnimation / 5;
            //}
            //else 
            //{
            //	item.useAnimation = 30;
            //	item.useTime = item.useAnimation / 3;
            //}
            return true;
        }
        //public override Color? GetAlpha(Color lightColor)
        //{
        //	return new Color(255, 255, 255, (int)lightColor.A - item.alpha);
        //}
        public static bool GetZenithTarget(Vector2 searchCenter, float maxDistance, Player player, out int npcTargetIndex)
        {
            npcTargetIndex = 0;
            int? num = null;
            float num2 = maxDistance;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(player, false))
                {
                    float num3 = Vector2.Distance(searchCenter, npc.Center);
                    if (num2 > num3)
                    {
                        num = new int?(i);
                        num2 = num3;
                    }
                }
            }
            if (num == null)
            {
                return false;
            }
            npcTargetIndex = num.Value;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true, true);
            float num6 = Main.mouseX + Main.screenPosition.X - vector.X;
            float num7 = Main.mouseY + Main.screenPosition.Y - vector.Y;
            int num166 = (player.itemAnimationMax - player.itemAnimation) / player.itemTime;
            Vector2 velocity_ = new Vector2(num6, num7);
            Vector2 value7 = Main.MouseWorld - player.MountedCenter;
            if (num166 == 1 || num166 == 2)
            {
                int num168;
                bool zenithTarget = GetZenithTarget(Main.MouseWorld, 400f, player, out num168);
                if (zenithTarget)
                {
                    value7 = Main.npc[num168].Center - player.MountedCenter;
                }
                bool flag8 = num166 == 2;
                if (num166 == 1 && !zenithTarget)
                {
                    flag8 = true;
                }
                if (flag8)
                {
                    value7 += Main.rand.NextVector2Circular(150f, 150f);
                }
            }
            velocity_ = value7 / 2f;
            float ai5 = Main.rand.Next(-100, 101);//
            //if(player.ownedProjectileCounts[type] < 1)
            var proj = Projectile.NewProjectileDirect(source, player.Center, velocity_, type, damage, knockback, player.whoAmI, ai5);
            proj.frame = Main.rand.Next(26);
            proj.netUpdate = true;
            proj.netUpdate2 = true;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Instance.GetPacket();
                packet.Write((byte)HandleNetwork.MessageType.PureFractal);
                packet.Write((short)proj.whoAmI);
                packet.Write((byte)proj.frame);
                packet.Send(-1, -1);
            }
            //for (int n = 0; n < 600; n++) 
            //{
            //    Projectile.NewProjectile(source, player.Center + new Vector2(48 * n - 14400, -560), new Vector2(0, 48), ProjectileID.PureSpray, 0, 0, player.whoAmI);
            //}
            return false;
        }
    }
    public class PureFractalProj : ModProjectile
    {
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)projectile.frame);
            //writer.Write(newColor.PackedValue);
            //writer.Write(airFactor);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.frame = reader.ReadByte();
            //newColor.PackedValue = reader.ReadUInt32();
            //airFactor = reader.ReadSingle();
        }
        Projectile projectile => Projectile;
        Color newColor => CoolerItemVisualEffect.PureFractalColors[Projectile.frame];
        float airFactor => CoolerItemVisualEffect.PureFractalAirFactorss[Projectile.frame];
        public override void PostAI()
        {
            Vector2 value1 = Main.player[projectile.owner].position - Main.player[projectile.owner].oldPosition;
            for (int num31 = projectile.oldPos.Length - 1; num31 > 0; num31--)
            {
                projectile.oldPos[num31] = projectile.oldPos[num31 - 1];
                projectile.oldRot[num31] = projectile.oldRot[num31 - 1];
                projectile.oldSpriteDirection[num31] = projectile.oldSpriteDirection[num31 - 1];
                if (projectile.numUpdates == 0 && projectile.oldPos[num31] != Vector2.Zero)
                {
                    projectile.oldPos[num31] += value1;
                }
            }
            projectile.oldPos[0] = projectile.Center;
            projectile.oldRot[0] = projectile.rotation;
            projectile.oldSpriteDirection[0] = projectile.spriteDirection;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 mountedCenter = player.MountedCenter;//坐骑上玩家的中心
            float lerpValue = Utils.GetLerpValue(900f, 0f, projectile.velocity.Length() * 2f, true);//获取线性插值的t值
            //Main.NewText((1 - projectile.velocity.Length() * 2 / 900f, lerpValue));
            float num = MathHelper.Lerp(0.7f, 2f, lerpValue);//速度的模长的两倍越接近900，这个值越接近0.7f
            projectile.localAI[0] += num;
            Main.projFrames[projectile.type] = 25;
            //projectile.frame = (int)projectile.ai[1];
            if (projectile.localAI[0] >= 120f)
            {
                projectile.Kill();
                return;
            }
            //目标离你越近，剑气的飞行时间越短
            float lerpValue2 = Utils.GetLerpValue(0f, 1f, projectile.localAI[0] / 60f, true);//??这不就单纯clamp了一下
            float num3 = projectile.ai[0];
            float num4 = projectile.velocity.ToRotation();
            float num5 = 3.14159274f;
            float num6 = projectile.velocity.X > 0f ? 1 : -1;
            float num7 = num5 + num6 * lerpValue2 * 6.28318548f;
            float num8 = projectile.velocity.Length() + Utils.GetLerpValue(0.5f, 1f, lerpValue2, true) * 40f;
            float num9 = 60f;
            if (num8 < num9)
            {
                num8 = num9;//保证半长轴最短是60
            }
            Vector2 value = mountedCenter + projectile.velocity;//椭圆中心
            Vector2 spinningpoint = new Vector2(1f, 0f).RotatedBy(num7, default) * new Vector2(num8, num3 * MathHelper.Lerp(2f, 1f, lerpValue));//插值生成椭圆轨迹
            Vector2 value2 = value + spinningpoint.RotatedBy(num4, default);//加上弹幕自身旋转量
            Vector2 value3 = (1f - Utils.GetLerpValue(0f, 0.5f, lerpValue2, true)) * new Vector2((projectile.velocity.X > 0f ? 1 : -1) * -num8 * 0.1f, -projectile.ai[0] * 0.3f);//坐标修改偏移量
            float num10 = num7 + num4;
            projectile.rotation = num10 + 1.57079637f;//弹幕绘制旋转量
            projectile.Center = value2 + value3;
            projectile.spriteDirection = projectile.direction = projectile.velocity.X > 0f ? 1 : -1;
            if (num3 < 0f)//小于零就反向
            {
                projectile.rotation = num5 + num6 * lerpValue2 * -6.28318548f + num4;
                projectile.rotation += 1.57079637f;
                projectile.spriteDirection = projectile.direction = projectile.velocity.X > 0f ? -1 : 1;
            }
            projectile.Opacity = Utils.GetLerpValue(0f, 5f, projectile.localAI[0], true) * Utils.GetLerpValue(120f, 115f, projectile.localAI[0], true);//修改透明度
        }
        //public override bool PreDraw(ref Color lightColor)
        public void DrawSword() 
        {
            var max = 0;
            Texture2D currentTex = GetPureFractalProjTexs(projectile.frame);
            float _scaler = currentTex.Size().Length();
            var multiValue = 1 - projectile.localAI[0] / 120f;
            var spEffect = projectile.ai[0] * projectile.velocity.X > 0 ? 0 : SpriteEffects.FlipHorizontally;

            for (int n = 0; n < Projectile.oldPos.Length; n++)
            {
                if (projectile.oldPos[n] == default) { max = n; break; }
            }
            for (int n = 15; n < max; n += 15)
            {
                var _fac = 1 - (float)n / max;
                //_fac *= _fac * _fac;
                var _color = newColor * _fac;//newColor 
                _color.A = 0;
                Main.spriteBatch.Draw(currentTex, projectile.oldPos[n - 1] - Main.screenPosition, null, _color * multiValue, projectile.oldRot[n - 1] - MathHelper.PiOver4 + (spEffect == 0 ? 0 : MathHelper.PiOver2), currentTex.Size() * new Vector2(spEffect == 0 ? 0 : 1, 1), ConfigSwooshInstance.swooshSize, spEffect, 0);
                DrawPrettyStarSparkle(projectile, 0, projectile.oldPos[n - 1] + (projectile.oldRot[n - 1] - MathHelper.PiOver2).ToRotationVector2() * _scaler * ConfigSwooshInstance.swooshSize - Main.screenPosition, Color.White, _color, Main.spriteBatch);

            }
            Main.spriteBatch.Draw(currentTex, projectile.oldPos[0] - Main.screenPosition, null, Color.White * multiValue, projectile.oldRot[0] - MathHelper.PiOver4 + (spEffect == 0 ? 0 : MathHelper.PiOver2), currentTex.Size() * new Vector2(spEffect == 0 ? 0 : 1, 1), ConfigSwooshInstance.swooshSize, spEffect, 0);
            DrawPrettyStarSparkle(projectile, 0, projectile.oldPos[0] + (projectile.oldRot[0] - MathHelper.PiOver2).ToRotationVector2() * _scaler * ConfigSwooshInstance.swooshSize - Main.screenPosition, Color.White, newColor, Main.spriteBatch);
        }
        public void DrawSwoosh()
        {
            int max = 60;
            Texture2D currentTex = GetPureFractalProjTexs(projectile.frame);
            //Main.spriteBatch.DrawLine(projectile.Center - Main.screenPosition, projectile.velocity, newColor, 16, true);
            var hsl = Main.rgbToHsl(newColor);
            for (int n = 0; n < Projectile.oldPos.Length; n++)
            {
                if (projectile.oldPos[n] == default) { max = n; break; }
            }

            if (!Main.gamePaused && projectile.localAI[0] < 60f)
            {
                //Vector2 value4 = (projectile.rotation - 1.57079637f).ToRotationVector2();
                Vector2 center = projectile.oldPos[0];
                int num11 = 1 + (int)(projectile.velocity.Length() / 100f);
                var lerpValue2 = MathHelper.Clamp(projectile.localAI[0] / 60f, 0, 1);
                num11 = (int)((float)num11 * Utils.GetLerpValue(0f, 0.5f, lerpValue2, true) * Utils.GetLerpValue(1f, 0.5f, lerpValue2, true));
                if (num11 < 1)
                {
                    num11 = 1;
                }
                Player player = Main.player[projectile.owner];
                var unit = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2();

                for (int i = 0; i < num11 + 5; i++)
                {

                    if (Main.rand.NextBool(9))
                    {
                        int _num = Main.rand.Next(1, 4);
                        for (int k = 0; k < _num; k++)
                        {
                            Dust dust = Dust.NewDustPerfect(center + unit * ConfigSwooshInstance.swooshSize * .5f * 50, 278, null, 100, Color.Lerp(newColor, Color.White, Main.rand.NextFloat() * 0.3f), 1f);
                            dust.scale = 0.4f;
                            dust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.3f;
                            dust.noGravity = true;
                            dust.velocity += unit * (3f + Main.rand.NextFloat() * 4f);
                        }
                    }
                    Vector3 value5 = Vector3.Lerp(Vector3.One, newColor.ToVector3(), 0.7f);
                    Lighting.AddLight(projectile.Center, newColor.ToVector3() * 0.5f * projectile.Opacity);
                    Lighting.AddLight(player.MountedCenter, value5 * projectile.Opacity * 0.15f);
                    //Dust.NewDustPerfect(center + value4 * finalFractalProfile.trailWidth * MathHelper.Lerp(0.5f, 1f, Main.rand.NextFloat()), projectile.rotation - 1.57079637f + 1.57079637f * (float)projectile.spriteDirection, player.velocity);
                }
            }

            #region 快乐顶点绘制_1(在原来的基础上叠加，亮瞎了)
            if (ShaderSwooshEX == null) return;//false
            if (DistortEffect == null) return;
            if (Main.GameViewMatrix == null) return;
            //var drawPlayer = Main.player[Projectile.owner];
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            //var modPlayer = drawPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>(); var fac = modPlayer.factorGeter;
            //fac = modPlayer.negativeDir ? 1 - fac : fac;
            //var drawCen = drawPlayer.gravDir == -1 ? new Vector2(drawPlayer.Center.X, (2 * (Main.screenPosition + new Vector2(960, 560)) - drawPlayer.Center - new Vector2(0, 96)).Y) : drawPlayer.Center;
            //float rotVel = instance.swooshActionStyle == SwooshAction.两次普通斩击一次高速旋转 && modPlayer.swingCount % 3 == 2 ? instance.rotationVelocity : 1;
            //var theta = (1.2375f * fac * rotVel - 1.125f) * MathHelper.Pi; CustomVertexInfo[] c = new CustomVertexInfo[6]; var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value; float xScaler = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, fac) : modPlayer.kValue; float scaler = itemTex.Size().Length() * drawPlayer.GetAdjustedItemScale(drawPlayer.HeldItem) / xScaler * 0.5f * trans.M11 * instance.swooshSize * airFactor; var cos = (float)Math.Cos(theta) * scaler;
            //var sin = (float)Math.Sin(theta) * scaler; var rotator = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, fac) : modPlayer.rotationForShadow;
            //var u = new Vector2(xScaler * (cos - sin), -cos - sin).RotatedBy(rotator);
            //var v = new Vector2(-xScaler * (cos + sin), sin - cos).RotatedBy(rotator); 
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            //List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            //var swooshAniFac = modPlayer.negativeDir ? 4 * fac - 3 : 4 * fac; swooshAniFac = MathHelper.Clamp(swooshAniFac, 0, 1);
            //var theta3 = (1.2375f * swooshAniFac * rotVel - 1.125f) * MathHelper.Pi; float xScaler3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, swooshAniFac) : modPlayer.kValue;
            //var rotator3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, swooshAniFac) : modPlayer.rotationForShadow;
            //var realColor = newColor;
            //for (int i = 0; i < 45; i++)
            //{
            //    var f = i / 44f; var theta2 = f.Lerp(theta3, theta, true); var xScaler2 = (instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? f : 1).Lerp(xScaler3, xScaler, true);
            //    var rotator2 = (instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? f : 1).Lerp(rotator3, rotator, true);
            //    var cos2 = (float)Math.Cos(theta2) * scaler;
            //    var sin2 = (float)Math.Sin(theta2) * scaler;
            //    var u2 = new Vector2(xScaler2 * (cos2 - sin2), -cos2 - sin2).RotatedBy(rotator2);
            //    var v2 = new Vector2(-xScaler2 * (cos2 + sin2), sin2 - cos2).RotatedBy(rotator2);
            //    var newVec = u2 + v2; var alphaLight = 0.6f;
            //    if (instance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 || instance.swooshColorType == SwooshColorType.色调处理与对角线混合)
            //    {
            //        float h = (hsl.X + instance.hueOffsetValue + instance.hueOffsetRange * (2 * f - 1)) % 1;
            //        float s = MathHelper.Clamp(hsl.Y * instance.saturationScalar, 0, 1);
            //        float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + instance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - instance.luminosityRange), 0, 1);
            //        realColor = Main.hslToRgb(h, s, l);
            //    }
            //    var _f = 6 * f / (3 * f + 1); _f = MathHelper.Clamp(_f, 0, 1);
            //    realColor.A = (byte)(_f * 255); bars.Add(new CustomVertexInfo(drawCen + newVec, realColor, new Vector3(1 - f, 1, alphaLight))); realColor.A = 0; bars.Add(new CustomVertexInfo(drawCen, realColor, new Vector3(0, 0, alphaLight)));
            //}
            SamplerState sampler;
            switch (ConfigSwooshInstance.swooshSampler)
            {
                default:
                case SwooshSamplerState.各向异性: sampler = SamplerState.AnisotropicClamp; break;
                case SwooshSamplerState.线性: sampler = SamplerState.LinearClamp; break;
                case SwooshSamplerState.点: sampler = SamplerState.PointClamp; break;
            }
            //List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
            //if (bars.Count > 2)
            //{
            //    for (int i = 0; i < bars.Count - 2; i += 2)
            //    {
            //        triangleList.Add(bars[i]);
            //        triangleList.Add(bars[i + 2]);
            //        triangleList.Add(bars[i + 1]);
            //        triangleList.Add(bars[i + 1]);
            //        triangleList.Add(bars[i + 2]);
            //        triangleList.Add(bars[i + 3]);
            //    }
            //    bool useRender = instance.distortFactor != 0 && Lighting.Mode != Terraria.Graphics.Light.LightMode.Retro && Lighting.Mode != Terraria.Graphics.Light.LightMode.Trippy;
            //    var gd = Main.graphics.GraphicsDevice;
            //    var sb = Main.spriteBatch;
            //    var passCount = 0;
            //    switch (instance.swooshColorType)
            //    {
            //        case SwooshColorType.函数生成热度图: passCount = 2; break;
            //        case SwooshColorType.武器贴图对角线: passCount = 1; break;
            //        case SwooshColorType.色调处理与对角线混合: passCount = 3; break;
            //    }
            //    if (useRender)
            //    {
            //        #region MyRegion
            //        #endregion
            //        sb.End();
            //        gd.SetRenderTarget(Instance.Render);
            //        gd.Clear(Color.Transparent);
            //        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity); ShaderSwooshEX.Parameters["uTransform"].SetValue(model * projection);
            //        ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
            //        ShaderSwooshEX.Parameters["uTime"].SetValue(0); ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir); ShaderSwooshEX.Parameters["airFactor"].SetValue(airFactor);
            //        ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather);
            //        Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + (int)MathHelper.Clamp(instance.ImageIndex, 0, 7)); Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
            //        Main.graphics.GraphicsDevice.Textures[2] = itemTex;
            //        if (instance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = GetPureFractalHeatMaps(Projectile.frame);
            //        Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
            //        Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
            //        Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
            //        Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
            //        ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
            //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
            //        Main.graphics.GraphicsDevice.RasterizerState = originalState;
            //        for (int n = 0; n < instance.maxCount; n++)
            //        {
            //            sb.End();
            //            gd.SetRenderTarget(Main.screenTargetSwap);
            //            gd.Clear(Color.Transparent); sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //            DistortEffect.CurrentTechnique.Passes[0].Apply(); DistortEffect.Parameters["tex0"].SetValue(Instance.Render); DistortEffect.Parameters["offset"].SetValue((u + v) * -0.002f * (1 - 2 * Math.Abs(0.5f - fac)) * instance.distortFactor); DistortEffect.Parameters["invAlpha"].SetValue(0);
            //            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            //            sb.End();
            //            gd.SetRenderTarget(Main.screenTarget);
            //            gd.Clear(Color.Transparent);
            //            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            //            sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, 0));
            //        }
            //    }
            //    else
            //    {
            //        sb.End();
            //        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans); ShaderSwooshEX.Parameters["uTransform"].SetValue(model * projection);
            //        ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
            //        ShaderSwooshEX.Parameters["uTime"].SetValue(0); ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir);
            //        ShaderSwooshEX.Parameters["airFactor"].SetValue(airFactor);
            //        ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather);
            //        Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + (int)MathHelper.Clamp(instance.ImageIndex, 0, 7)); Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
            //        Main.graphics.GraphicsDevice.Textures[2] = itemTex;
            //        if (instance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = modPlayer.colorBar.tex;
            //        Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
            //        Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
            //        Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
            //        Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
            //        ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
            //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
            //        Main.graphics.GraphicsDevice.RasterizerState = originalState;
            //    }
            //}
            #endregion
            #region 快乐顶点绘制_2(现在才进入正题)
            float _scaler = currentTex.Size().Length() * airFactor;
            var bars = new List<CustomVertexInfo>();
            var realColor = newColor;
            var multiValue = 1 - projectile.localAI[0] / 120f;
            multiValue = 4 * multiValue / (3 * multiValue + 1);
            for (int i = 0; i < max; i++)
            {
                //if (i >= 15) break;
                //var f = 1 - (i / (max - 1f)) * 4 % 1;
                int _i = i % 15;
                var _value = max / 15 * 15;
                var f = i > _value ? _i / (max - _value - 1f) : (_i / 14f);
                f = 1 - f;
                var alphaLight = 0.6f;
                if (ConfigSwooshInstance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 || ConfigSwooshInstance.swooshColorType == SwooshColorType.色调处理与对角线混合)
                {
                    float h = (hsl.X + ConfigSwooshInstance.hueOffsetValue + ConfigSwooshInstance.hueOffsetRange * (2 * f - 1)) % 1;
                    float s = MathHelper.Clamp(hsl.Y * ConfigSwooshInstance.saturationScalar, 0, 1);
                    float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + ConfigSwooshInstance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - ConfigSwooshInstance.luminosityRange), 0, 1);
                    realColor = Main.hslToRgb(h, s, l);
                }
                var _f = f * f;//6 * f / (3 * f + 1) /(float)Math.Pow(f,instance.maxCount)
                _f = MathHelper.Clamp(_f, 0, 1);
                realColor.A = (byte)(_f * 255 * (float)Math.Pow(1 - i / 15 / 5f, 2));
                bars.Add(new CustomVertexInfo(projectile.oldPos[i] + (projectile.oldRot[i] - MathHelper.PiOver2).ToRotationVector2() * _scaler * ConfigSwooshInstance.swooshSize, realColor * multiValue, new Vector3(1 - f, 1, alphaLight)));
                realColor.A = 0;
                bars.Add(new CustomVertexInfo(projectile.oldPos[i], realColor * multiValue, new Vector3(0, 0, alphaLight)));
            }
            List<CustomVertexInfo> _triangleList = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    _triangleList.Add(bars[i]);
                    _triangleList.Add(bars[i + 2]);
                    _triangleList.Add(bars[i + 1]);
                    _triangleList.Add(bars[i + 1]);
                    _triangleList.Add(bars[i + 2]);
                    _triangleList.Add(bars[i + 3]);
                }
                //bool useRender = instance.distortFactor != 0 && CoolerItemVisualEffect.CanUseRender;
                //var gd = Main.graphics.GraphicsDevice;
                //var sb = Main.spriteBatch;
                var passCount = 0;
                switch (ConfigSwooshInstance.swooshColorType)
                {
                    case SwooshColorType.函数生成热度图: passCount = 2; break;
                    case SwooshColorType.武器贴图对角线: passCount = 1; break;
                    case SwooshColorType.色调处理与对角线混合: passCount = 3; break;
                }
                //if (false)//useRender
                //{
                //    #region MyRegion
                //    #endregion
                //    sb.End();
                //    gd.SetRenderTarget(Instance.Render);
                //    gd.Clear(Color.Transparent);
                //    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                //    ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                //    ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
                //    ShaderSwooshEX.Parameters["uTime"].SetValue(0); ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir);
                //    ShaderSwooshEX.Parameters["airFactor"].SetValue(airFactor);
                //    ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather);
                //    Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + (int)MathHelper.Clamp(instance.ImageIndex, 0, 7));
                //    Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
                //    Main.graphics.GraphicsDevice.Textures[2] = currentTex;
                //    if (instance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = GetPureFractalHeatMaps(Projectile.frame);
                //    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                //    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                //    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                //    Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
                //    ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                //    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleList.ToArray(), 0, _triangleList.Count / 3);
                //    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                //    for (int n = 0; n < instance.maxCount; n++)
                //    {
                //        sb.End();
                //        gd.SetRenderTarget(Main.screenTargetSwap);
                //        gd.Clear(Color.Transparent);
                //        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                //        DistortEffect.CurrentTechnique.Passes[0].Apply();
                //        DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
                //        DistortEffect.Parameters["offset"].SetValue((projectile.oldRot[0] - MathHelper.PiOver2).ToRotationVector2() * -0.01f * instance.distortFactor);
                //        DistortEffect.Parameters["invAlpha"].SetValue(0);
                //        sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                //        sb.End();
                //        gd.SetRenderTarget(Main.screenTarget);
                //        gd.Clear(Color.Transparent);
                //        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                //        sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                //        sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, 0));

                //        //sb.End();
                //        //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                //        //DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                //        //DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
                //        //DistortEffect.Parameters["position"].SetValue(new Vector2(0, 3));
                //        //DistortEffect.Parameters["tier2"].SetValue(0.2f);
                //        //for (int i = 0; i < 1; i++)
                //        //{
                //        //    gd.SetRenderTarget(Main.screenTargetSwap);
                //        //    gd.Clear(Color.Transparent);
                //        //    DistortEffect.CurrentTechnique.Passes[7].Apply();
                //        //    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);



                //        //    gd.SetRenderTarget(Main.screenTarget);
                //        //    gd.Clear(Color.Transparent);
                //        //    DistortEffect.CurrentTechnique.Passes[6].Apply();
                //        //    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                //        //}
                //        //DistortEffect.Parameters["position"].SetValue(new Vector2(0, 3));
                //        //DistortEffect.Parameters["ImageSize"].SetValue(Vector2.Normalize(projectile.velocity) * -0.002f * instance.distortFactor);
                //        //for (int i = 0; i < 1; i++)
                //        //{
                //        //    gd.SetRenderTarget(Main.screenTargetSwap);
                //        //    gd.Clear(Color.Transparent);
                //        //    DistortEffect.CurrentTechnique.Passes[5].Apply();
                //        //    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

                //        //    gd.SetRenderTarget(Main.screenTarget);
                //        //    gd.Clear(Color.Transparent);
                //        //    DistortEffect.CurrentTechnique.Passes[4].Apply();
                //        //    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                //        //}
                //        //sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                //        //sb.Draw(Instance.Render, Vector2.Zero, Color.White);
                //    }
                //}
                //else
                //{
                //    sb.End();
                //    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                //    ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                //    ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
                //    ShaderSwooshEX.Parameters["uTime"].SetValue(0); ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir);
                //    ShaderSwooshEX.Parameters["airFactor"].SetValue(airFactor);
                //    ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather);
                //    Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + (int)MathHelper.Clamp(instance.ImageIndex, 0, 7));
                //    Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
                //    Main.graphics.GraphicsDevice.Textures[2] = currentTex;
                //    if (instance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = GetPureFractalHeatMaps(Projectile.frame);
                //    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                //    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                //    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                //    Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
                //    ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                //    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleList.ToArray(), 0, _triangleList.Count / 3);
                //    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                //}
                //sb.End();
                //sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                //ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
                ShaderSwooshEX.Parameters["uTime"].SetValue(0); ShaderSwooshEX.Parameters["checkAir"].SetValue(ConfigSwooshInstance.checkAir);
                ShaderSwooshEX.Parameters["airFactor"].SetValue(airFactor);
                ShaderSwooshEX.Parameters["gather"].SetValue(ConfigSwooshInstance.gather);
                Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + (int)MathHelper.Clamp(ConfigSwooshInstance.ImageIndex, 0, 8));
                Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
                Main.graphics.GraphicsDevice.Textures[2] = currentTex;
                if (ConfigSwooshInstance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = GetPureFractalHeatMaps(Projectile.frame);
                Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
                ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleList.ToArray(), 0, _triangleList.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
            #endregion

            //max = Projectile.oldPos.Length;
            //for (int n = 0; n < max; n += 3)
            //{
            //    var _fac = 1 - (float)n / max;
            //    _fac *= _fac * _fac;
            //    Main.spriteBatch.Draw(currentTex, projectile.oldPos[n] - Main.screenPosition, null, (n == 0 ? Color.White : newColor) * _fac, projectile.oldRot[n] - MathHelper.PiOver4, currentTex.Size() * new Vector2(0, 1), 1f, 0, 0);
            //}

            // Main.spriteBatch.DrawString(FontAssets.MouseText.Value, multiValue.ToString(), projectile.Center - Main.screenPosition, Color.Red);
            //Main.NewText(multiValue);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
            //for (int n = 15; n < max; n += 15)
            //{
            //    var _fac = 1 - (float)n / max;
            //    //_fac *= _fac * _fac;
            //    var _color = newColor * _fac;
            //    _color.A = 0;
            //    Main.spriteBatch.Draw(currentTex, projectile.oldPos[n - 1] - Main.screenPosition, null, _color * multiValue, projectile.oldRot[n - 1] - MathHelper.PiOver4 + (spEffect == 0 ? 0 : MathHelper.PiOver2), currentTex.Size() * new Vector2(spEffect == 0 ? 0 : 1, 1), instance.swooshSize, spEffect, 0);
            //    DrawPrettyStarSparkle(projectile, 0, projectile.oldPos[n - 1] + (projectile.oldRot[n - 1] - MathHelper.PiOver2).ToRotationVector2() * _scaler * instance.swooshSize - Main.screenPosition, Color.White, _color, Main.spriteBatch);

            //}
            //Main.spriteBatch.Draw(currentTex, projectile.oldPos[0] - Main.screenPosition, null, Color.White * multiValue, projectile.oldRot[0] - MathHelper.PiOver4 + (spEffect == 0 ? 0 : MathHelper.PiOver2), currentTex.Size() * new Vector2(spEffect == 0 ? 0 : 1, 1), instance.swooshSize, spEffect, 0);
            //DrawPrettyStarSparkle(projectile, 0, projectile.oldPos[0] + (projectile.oldRot[0] - MathHelper.PiOver2).ToRotationVector2() * _scaler * instance.swooshSize - Main.screenPosition, Color.White, newColor, Main.spriteBatch);
            return;// false
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
        private void DrawPrettyStarSparkle(Projectile proj, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, SpriteBatch spriteBatch)
        {
            Texture2D value = GetTexture("FinalFractalLight");
            Color color = shineColor * proj.Opacity * 0.5f;
            color.A = 0;
            Vector2 origin = value.Size() / 2f;
            Color color2 = drawColor * 0.5f;
            float num = Utils.GetLerpValue(15f, 30f, proj.localAI[0], true) * Utils.GetLerpValue(45f, 30f, proj.localAI[0], true);
            Vector2 vector = new Vector2(0.5f, 5f) * num * ConfigSwooshInstance.swooshSize;
            Vector2 vector2 = new Vector2(0.5f, 2f) * num * ConfigSwooshInstance.swooshSize;
            color *= num;
            color2 *= num;
            spriteBatch.Draw(value, drawpos, null, color, 1.57079637f, origin, vector, dir, 0);
            spriteBatch.Draw(value, drawpos, null, color, 0f, origin, vector2, dir, 0);
            spriteBatch.Draw(value, drawpos, null, color2, 1.57079637f, origin, vector * 0.6f, dir, 0);
            spriteBatch.Draw(value, drawpos, null, color2, 0f, origin, vector2 * 0.6f, dir, 0);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle _lanceHitboxBounds = new Rectangle(0, 0, 300, 300);
            float num2 = 0f;
            float scaleFactor = 40f;
            for (int i = 14; i < projectile.oldPos.Length; i += 15)
            {
                float num3 = projectile.localAI[0] - i;
                if (num3 >= 0f && num3 <= 60f)
                {
                    Vector2 vector2 = projectile.oldPos[i];
                    Vector2 value2 = (projectile.oldRot[i] + 1.57079637f).ToRotationVector2();
                    _lanceHitboxBounds.X = (int)vector2.X - _lanceHitboxBounds.Width / 2;
                    _lanceHitboxBounds.Y = (int)vector2.Y - _lanceHitboxBounds.Height / 2;
                    if (_lanceHitboxBounds.Intersects(targetHitbox) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), vector2 - value2 * scaleFactor, vector2 + value2 * scaleFactor, 20f, ref num2))
                    {
                        return true;
                    }
                }
            }
            Vector2 value3 = (projectile.rotation + 1.57079637f).ToRotationVector2();
            _lanceHitboxBounds.X = (int)projectile.position.X - _lanceHitboxBounds.Width / 2;
            _lanceHitboxBounds.Y = (int)projectile.position.Y - _lanceHitboxBounds.Height / 2;
            return _lanceHitboxBounds.Intersects(targetHitbox) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - value3 * scaleFactor, projectile.Center + value3 * scaleFactor, 20f, ref num2);
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.DamageType = DamageClass.Melee;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.manualDirectionChange = true;
            projectile.localNPCHitCooldown = 3;
            projectile.penetrate = -1;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            lightColor = Color.White * projectile.Opacity;
            return lightColor;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 60;
        }
    }
}
