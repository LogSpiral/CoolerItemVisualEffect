using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.ConfigModification;
using LogSpiralLibrary.CodeLibrary.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using static System.Runtime.InteropServices.JavaScript.JSType;
using tModPorter;
using FullSerializer;
using Terraria.Localization;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;
using System.IO;

namespace CoolerItemVisualEffect
{
    public static class PreviewHelper
    {
        public static void DrawUltraSwoosh(SpriteBatch spriteBatch, Vector2 center, ConfigurationCIVE config, Texture2D heatMap = null, int? baseTex = null, int? aniTex = null, Vector3? alphaVector = null, bool? useRenderEffect = null, Action<UltraSwoosh> otherOperation = null)
        {
            float k = 1f;
            if (!Main.gameMenu) k = 1 / Main.UIScale;
            var adjustedClippingRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            VertexDrawInfo.UIDrawing = true;
            UltraSwoosh[] ultraSwooshes = new UltraSwoosh[1];
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            UltraSwoosh.NewUltraSwoosh(mplr?.mainColor ?? Main.DiscoColor, ultraSwooshes, 30, 80 * k, center * k, heatMap ?? (mplr?.heatMap ?? LogSpiralLibraryMod.HeatMap[1].Value), false, 0, 1, null, aniTex ?? config.animateIndexSwoosh, baseTex ?? config.baseIndexSwoosh, alphaVector ?? config.colorVector.AlphaVector, false);
            ultraSwooshes[0].weaponTex = MeleeModifyPlayer.GetWeaponTextureFromItem(mplr?.Player.HeldItem);
            ultraSwooshes[0].alphaFactor = config.alphaFactor;
            ultraSwooshes[0].heatRotation = config.directOfHeatMap;
            ultraSwooshes[0].ModityAllRenderInfo([[config.distortConfigs.effectInfo], [config.maskConfigs.maskEffectInfo, config.bloomConfigs.effectInfo]]);
            ultraSwooshes[0].Uptate();
            ultraSwooshes[0].timeLeft++;
            otherOperation?.Invoke(ultraSwooshes[0]);
            spriteBatch.End();
            VertexDrawInfo.DrawVertexInfo(ultraSwooshes, typeof(UltraSwoosh), spriteBatch, useRenderEffect ?? config.useRenderEffectPVInOtherConfig ? Main.graphics.GraphicsDevice : null, LogSpiralLibraryMod.Instance.Render, LogSpiralLibraryMod.Instance.Render_Swap);
            spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = UIElement.OverflowHiddenRasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, UIElement.OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
            VertexDrawInfo.UIDrawing = false;
        }

        public static void DrawUltraStab(SpriteBatch spriteBatch, Vector2 center, ConfigurationCIVE config, Texture2D heatMap = null, int? baseTex = null, int? aniTex = null, Vector3? alphaVector = null, bool? useRenderEffect = null, Action<UltraStab> otherOperation = null)
        {
            float k = 1f;
            if (!Main.gameMenu) k = 1 / Main.UIScale;
            var adjustedClippingRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            VertexDrawInfo.UIDrawing = true;
            UltraStab[] ultraStabs = new UltraStab[1];
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            UltraStab.NewUltraStab(mplr?.mainColor ?? Main.DiscoColor, ultraStabs, 30, 160 * k, center * k, heatMap ?? (mplr?.heatMap ?? LogSpiralLibraryMod.HeatMap[1].Value)
                , false, 0, 2, aniTex ?? config.animateIndexStab, baseTex ?? config.baseIndexStab, alphaVector ?? config.colorVector.AlphaVector, false);
            ultraStabs[0].weaponTex = MeleeModifyPlayer.GetWeaponTextureFromItem(mplr?.Player.HeldItem);
            ultraStabs[0].alphaFactor = config.alphaFactor;
            ultraStabs[0].heatRotation = config.directOfHeatMap;
            ultraStabs[0].Uptate();
            ultraStabs[0].timeLeft++;

            otherOperation?.Invoke(ultraStabs[0]);
            spriteBatch.End();
            ultraStabs[0].ModityAllRenderInfo([[config.distortConfigs.effectInfo], [config.maskConfigs.maskEffectInfo, config.bloomConfigs.effectInfo]]);
            VertexDrawInfo.DrawVertexInfo(ultraStabs, typeof(UltraStab), spriteBatch, useRenderEffect ?? config.useRenderEffectPVInOtherConfig ? Main.graphics.GraphicsDevice : null, LogSpiralLibraryMod.Instance.Render, LogSpiralLibraryMod.Instance.Render_Swap);
            spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = UIElement.OverflowHiddenRasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, UIElement.OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
            VertexDrawInfo.UIDrawing = false;
        }

        public static void DrawSorry(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            Vector2 pos = rectangle.Left() + new Vector2(20, 0);
            spriteBatch.DrawString(FontAssets.MouseText.Value, Language.GetOrRegister("Mods.CoolerItemVisualEffect.Misc.UselessConfig").Value, pos, Color.White);
        }
        public static void DrawUnavailable(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            Vector2 pos = rectangle.Left() + new Vector2(20, 0);
            spriteBatch.DrawString(FontAssets.MouseText.Value, Language.GetOrRegister("Mods.CoolerItemVisualEffect.Misc.UnavailableConfig").Value, pos, Color.White);
        }
    }
    public class ActivePreview : SimplePreview<bool>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle rectangle, bool data, ModConfig modConfig)
        {
            spriteBatch.Draw(data ? TextureAssets.Npc[NPCID.SkeletronPrime].Value : TextureAssets.Npc[NPCID.SkeletronHead].Value, rectangle.Center(), data ? new Rectangle(0, 0, 140, 156) : null, Color.White, 0, data ? new Vector2(70, 78) : new Vector2(40, 51), 1f, 0, 0);
        }
    }
    public class AnimationTexSwooshPreview : SimplePreview<int>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, int data, ModConfig modConfig)
        {
            var tex = LogSpiralLibraryMod.AniTex_Swoosh[data].Value;                //20 + 90
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 110, drawRange.Center().Y), new Vector2(180)), Color.White);
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)modConfig, null, null, data, null);
        }
    }
    public class BaseTexSwooshPreview : SimplePreview<int>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, int data, ModConfig modConfig)
        {
            var tex = LogSpiralLibraryMod.BaseTex_Swoosh[data].Value;                //20 + 90
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 110, drawRange.Center().Y), new Vector2(180)), Color.White);
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)modConfig, null, data, null, null);
        }
    }
    public class AnimationTexStabPreview : SimplePreview<int>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, int data, ModConfig modConfig)
        {
            var tex = LogSpiralLibraryMod.AniTex_Stab[data].Value;                //20 + 90
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 110, drawRange.Center().Y), new Vector2(180)), Color.White);
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 180, drawRange.Center().Y), (ConfigurationCIVE)modConfig, null, null, data, null);
        }
    }
    public class BaseTexStabPreview : SimplePreview<int>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, int data, ModConfig modConfig)
        {
            var tex = LogSpiralLibraryMod.BaseTex_Stab[data].Value;                //20 + 90
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 110, drawRange.Center().Y), new Vector2(180)), Color.White);
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 180, drawRange.Center().Y), (ConfigurationCIVE)modConfig, null, data, null, null);
        }
    }
    public class TimeLeftPreview : SimplePreview<int>
    {
        static int timer;
        static int timerMax;
        static int coolDown;
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, int data, ModConfig pendingConfig)
        {
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            void ModifyTimer(VertexDrawInfo v)
            {
                var config = (ConfigurationCIVE)pendingConfig;
                if (timer <= -30)
                    timerMax = timer = config.swooshTimeLeft;
                v.timeLeftMax = timerMax;
                v.timeLeft = timer < 0 ? 0 : timer;
            }
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, otherOperation: ModifyTimer);
            PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(drawRange.X + 20, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, otherOperation: ModifyTimer);
            timer--;

        }
    }
    public class WeaponExtraLightPreview : SimplePreview<float>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
            var adjustedClippingRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            float k = 1f;
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            else
                k = 1 / Main.UIScale;
            Item item = Main.gameMenu ? null : Main.LocalPlayer.HeldItem;
            Texture2D texture = Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayer.GetWeaponTextureFromItem(item);
            var nframe = Main.itemAnimations[Main.gameMenu ? ItemID.TerraBlade : item.type]?.GetFrame(texture);
            CustomVertexInfo[] c = DrawingMethods.GetItemVertexes(new Vector2(0.1f, 0.9f), 0, -MathHelper.PiOver2, texture, 1, k, drawRange.Center() * k, true, 1f, nframe);
            Effect ItemEffect = LogSpiralLibraryMod.ItemEffectEX;
            if (ItemEffect == null) return;
            SamplerState sampler = SamplerState.AnisotropicWrap;
            var projection = Main.gameMenu ? Matrix.CreateOrthographicOffCenter(0, Main.instance.Window.ClientBounds.Width, Main.instance.Window.ClientBounds.Height, 0, 0, 1) : Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(default);
            var trans = Main.UIScaleMatrix;
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            Matrix result = model * trans * projection;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
            ItemEffect.Parameters["uTransform"].SetValue(result);
            ItemEffect.Parameters["uTime"].SetValue((float)LogSpiralLibraryMod.ModTime / 60f % 1);
            ItemEffect.Parameters["uItemColor"].SetValue(Vector4.One);
            ItemEffect.Parameters["uItemGlowColor"].SetValue(Vector4.One);
            if (nframe != null)
            {
                Rectangle frame = nframe.Value;
                Vector2 size = texture.Size();
                ItemEffect.Parameters["uItemFrame"].SetValue(new Vector4(frame.TopLeft() / size, frame.Width / size.X, frame.Height / size.Y));
            }
            else
                ItemEffect.Parameters["uItemFrame"].SetValue(new Vector4(0, 0, 1, 1));
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.Misc[0].Value;
            Main.graphics.GraphicsDevice.Textures[2] = LogSpiralLibraryMod.BaseTex[15].Value;
            Texture2D glow = null;
            if (!Main.gameMenu && item.type != ItemID.None)
            {
                glow = item.glowMask != -1 ? TextureAssets.GlowMask[item.glowMask].Value : (item.flame ? TextureAssets.ItemFlame[item.type].Value : null);
            }
            Main.graphics.GraphicsDevice.Textures[3] = glow;

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
            ItemEffect.CurrentTechnique.Passes[0].Apply();
            for (int n = 0; n < c.Length; n++) c[n].Color = (Main.gameMenu ? Color.DarkGreen : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().mainColor) * data;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, c, 0, c.Length / 3);
            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            Main.spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;

            spriteBatch.GraphicsDevice.RasterizerState = UIElement.OverflowHiddenRasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, UIElement.OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
        }
    }
    public class ColorVectorPreview : SimplePreview<ColorVector>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, ColorVector data, ModConfig pendingConfig)
        {
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + 100, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, data.AlphaVector * Vector3.UnitX);
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + 150, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, data.AlphaVector * Vector3.UnitY);
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + 200, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, data.AlphaVector * Vector3.UnitZ);

            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, data.AlphaVector);

        }
    }
    public class AlphaScalerPreview : SimplePreview<float>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, drawRange.Center(), (ConfigurationCIVE)pendingConfig, null, null, null, null, null, u => u.alphaFactor = data);

        }
    }
    public class LightStandardPreview : SimplePreview<float>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
            PreviewHelper.DrawSorry(spriteBatch, drawRange);

        }
    }
    public class HeatMapCreatePreview : SimplePreview<ConfigurationCIVE.HeatMapCreateStyle>
    {
        static Texture2D[] curHeatMaps = new Texture2D[3];
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, ConfigurationCIVE.HeatMapCreateStyle data, ModConfig pendingConfig)
        {
            var config = (ConfigurationCIVE)pendingConfig;

            //Texture2D[] curHeatMaps = new Texture2D[3];
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            for (int n = 0; n < 3; n++)
            {
                config.heatMapCreateStyle = (ConfigurationCIVE.HeatMapCreateStyle)n;
                var tex = curHeatMaps[n];
                MeleeModifyPlayer.UpdateHeatMap(ref tex, Main.gameMenu ? new Vector3(0.5f, 1, 0.5f) : mplr.hsl, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                curHeatMaps[n] = tex;
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + (n - 1) * 60), new Vector2(180, 40)), Color.White);
                if (n == (int)data)
                {
                    PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), config, tex, null, null, null);
                    if (mplr != null)
                        MeleeModifyPlayer.UpdateHeatMap(ref mplr.heatMap, mplr.hsl, config, MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                }
                //tex.Dispose();
            }
            spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(drawRange.X + 20, drawRange.Center().Y + ((int)data - 1) * 60), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
            config.heatMapCreateStyle = data;

        }
    }
    public class HeatMapFactorPreview : SimplePreview<ConfigurationCIVE.HeatMapFactorStyle>
    {
        static Texture2D[] curHeatMaps = new Texture2D[5];
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, ConfigurationCIVE.HeatMapFactorStyle data, ModConfig pendingConfig)
        {
            var config = (ConfigurationCIVE)pendingConfig;

            //Texture2D[] curHeatMaps = new Texture2D[5];
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            for (int n = 0; n < 5; n++)
            {
                config.heatMapFactorStyle = (ConfigurationCIVE.HeatMapFactorStyle)n;
                var tex = curHeatMaps[n];
                MeleeModifyPlayer.UpdateHeatMap(ref tex, Main.gameMenu ? new Vector3(0.5f, 1, 0.5f) : mplr.hsl, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                curHeatMaps[n] = tex;
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + (n - 2) * 45), new Vector2(180, 20)), Color.White);
                if (n == (int)data)
                {
                    PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), config, tex, null, null, null);
                    if (mplr != null)
                        MeleeModifyPlayer.UpdateHeatMap(ref mplr.heatMap, mplr.hsl, config, MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                }
                //tex.Dispose();
            }
            spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(drawRange.X + 20, drawRange.Center().Y + ((int)data - 2) * 45), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
            config.heatMapFactorStyle = data;

        }
    }
    public class HueOffsetRangePreview : SimplePreview<float>
    {
        static Texture2D[] curHeatMaps = new Texture2D[60];
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
            var config = (ConfigurationCIVE)pendingConfig;
            if (config.heatMapCreateStyle != ConfigurationCIVE.HeatMapCreateStyle.ByFunction)
            {
                PreviewHelper.DrawUnavailable(spriteBatch, drawRange);
                return;
            }
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            bool havntDraw = true;
            for (int n = 0; n < 60; n++)
            {
                float r = config.hueOffsetRange = MathHelper.Lerp(-1, 1, n / 59f);
                var tex = curHeatMaps[n];
                MeleeModifyPlayer.UpdateHeatMap(ref tex, Main.gameMenu ? new Vector3(0.5f, 1, 0.5f) : mplr.hsl, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                curHeatMaps[n] = tex;
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + (n - 29.5f) * 2f - 30), new Vector2(180, 2)), Color.White);
                if (Math.Abs(r - data) <= 0.02f && havntDraw)
                {
                    havntDraw = false;
                    PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), config, tex, null, null, null);
                    if (mplr != null)
                        MeleeModifyPlayer.UpdateHeatMap(ref mplr.heatMap, mplr.hsl, config, MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));

                    spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + 70), new Vector2(180, 30)), Color.White);

                }
                //tex.Dispose();

            }

            spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(drawRange.X + 20, drawRange.Center().Y + MathHelper.Lerp(-90, 30, Utils.GetLerpValue(-1, 1, data))), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
            config.hueOffsetRange = data;
        }
    }
    public class HueOffsetPreview : SimplePreview<float>
    {
        static Texture2D[] curHeatMaps = new Texture2D[60];
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
            var config = (ConfigurationCIVE)pendingConfig;
            if (config.heatMapCreateStyle != ConfigurationCIVE.HeatMapCreateStyle.ByFunction)
            {
                PreviewHelper.DrawUnavailable(spriteBatch, drawRange);
                return;
            }
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            bool havntDraw = true;
            for (int n = 0; n < 60; n++)
            {
                float r = config.hueOffsetValue = MathHelper.Lerp(0, 1, n / 59f);
                var tex = curHeatMaps[n];
                MeleeModifyPlayer.UpdateHeatMap(ref tex, Main.gameMenu ? new Vector3(0.5f, 1, 0.5f) : mplr.hsl, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                curHeatMaps[n] = tex;
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + (n - 29.5f) * 2f - 30), new Vector2(180, 2)), Color.White);
                if (Math.Abs(r - data) <= 0.01f && havntDraw)
                {
                    havntDraw = false;
                    PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), config, tex, null, null, null);
                    if (mplr != null)
                        MeleeModifyPlayer.UpdateHeatMap(ref mplr.heatMap, mplr.hsl, config, MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));

                    spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + 70), new Vector2(180, 30)), Color.White);
                }
                //tex.Dispose();

            }
            spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(drawRange.X + 20, drawRange.Center().Y + MathHelper.Lerp(-90, 30, Utils.GetLerpValue(0, 1, data))), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
            config.hueOffsetValue = data;
        }
    }
    public class SaturationScalarPreview : SimplePreview<float>
    {
        static Texture2D[] curHeatMaps = new Texture2D[60];
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
            var config = (ConfigurationCIVE)pendingConfig;
            if (config.heatMapCreateStyle != ConfigurationCIVE.HeatMapCreateStyle.ByFunction)
            {
                PreviewHelper.DrawUnavailable(spriteBatch, drawRange);
                return;
            }
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            bool havntDraw = true;
            for (int n = 0; n < 60; n++)
            {
                float r = config.saturationScalar = MathHelper.Lerp(0, 5, n / 59f);
                var tex = curHeatMaps[n];
                MeleeModifyPlayer.UpdateHeatMap(ref tex, Main.gameMenu ? new Vector3(0.5f, 1, 0.5f) : mplr.hsl, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                curHeatMaps[n] = tex;
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + (n - 29.5f) * 2f - 30), new Vector2(180, 2)), Color.White);
                if (Math.Abs(r - data) <= 0.05f && havntDraw)
                {
                    havntDraw = false;
                    PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), config, tex, null, null, null);
                    if (mplr != null)
                        MeleeModifyPlayer.UpdateHeatMap(ref mplr.heatMap, mplr.hsl, config, MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));

                    spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + 70), new Vector2(180, 30)), Color.White);

                }
                //tex.Dispose();

            }
            spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(drawRange.X + 20, drawRange.Center().Y + MathHelper.Lerp(-90, 30, Utils.GetLerpValue(0, 5, data))), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
            config.saturationScalar = data;
        }
    }
    public class LuminosityRangePreview : SimplePreview<float>
    {
        static Texture2D[] curHeatMaps = new Texture2D[60];
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
            var config = (ConfigurationCIVE)pendingConfig;

            if (config.heatMapCreateStyle != ConfigurationCIVE.HeatMapCreateStyle.ByFunction)
            {
                PreviewHelper.DrawUnavailable(spriteBatch, drawRange);
                return;
            }
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            bool havntDraw = true;
            for (int n = 0; n < 60; n++)
            {
                float r = config.luminosityRange = MathHelper.Lerp(0, 1, n / 59f);
                var tex = curHeatMaps[n];
                MeleeModifyPlayer.UpdateHeatMap(ref tex, Main.gameMenu ? new Vector3(0.5f, 1, 0.5f) : mplr.hsl, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                curHeatMaps[n] = tex;
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + (n - 29.5f) * 2f - 30), new Vector2(180, 2)), Color.White);
                if (Math.Abs(r - data) <= 0.01f && havntDraw)
                {
                    havntDraw = false;
                    PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), config, tex, null, null, null);
                    if (mplr != null)
                        MeleeModifyPlayer.UpdateHeatMap(ref mplr.heatMap, mplr.hsl, config, MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                    spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + 70), new Vector2(180, 30)), Color.White);

                }
                //tex.Dispose();
            }
            spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(drawRange.X + 20, drawRange.Center().Y + MathHelper.Lerp(-90, 30, Utils.GetLerpValue(0, 1, data))), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
            config.luminosityRange = data;
        }
    }
    public class DirectionOfHeatMapPreview : SimplePreview<float>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, null, null, u => u.heatRotation = data);
            PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(drawRange.X + 20, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, null, null, u => u.heatRotation = data);

            //PreviewHelper.DrawSorry(spriteBatch, drawRange);
        }
    }
    public class ColorListPreview : SimplePreview<List<Color>>//List<Color> //因为ConfigElement其实没有直接List<Color>类型的
    {
        Texture2D heatMap;
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, List<Color> data, ModConfig modConfig)//List<Color>
        {
            var config = (ConfigurationCIVE)modConfig;
            if (config.heatMapCreateStyle != ConfigurationCIVE.HeatMapCreateStyle.Designate)
            {
                PreviewHelper.DrawUnavailable(spriteBatch, drawRange);
                return;
            }
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            MeleeModifyPlayer.UpdateHeatMap(ref heatMap, Main.gameMenu ? new Vector3(0.5f, 1, 0.5f) : mplr.hsl, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayer.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
            var tex = heatMap;
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y), new Vector2(240, 40)), Color.White);
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)modConfig, heatMap, null, null, null);
        }
    }
    public class RenderEffectPreview : SimplePreview<object> //因为那几个通用的所以干脆obj得了
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, object data, ModConfig pendingConfig)
        {
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, null, true);
            PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(drawRange.X + 20, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, null, true);
        }
    }
    public class UseRenderPVPreivew : SimplePreview<bool>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, bool data, ModConfig pendingConfig)
        {
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, null, data);
            PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(drawRange.X + 20, drawRange.Center().Y), (ConfigurationCIVE)pendingConfig, null, null, null, null, data);
        }
    }
}
