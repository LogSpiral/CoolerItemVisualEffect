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

namespace CoolerItemVisualEffect
{
    public static class PreviewHelper
    {
        public static void DrawUltraSwoosh(SpriteBatch spriteBatch, Vector2 center, ConfigurationCIVE config, Texture2D heatMap = null, int? baseTex = null, int? aniTex = null, Vector3? alphaVector = null)
        {
            var adjustedClippingRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            VertexDrawInfo.UIDrawing = true;
            UltraSwoosh[] ultraSwooshes = new UltraSwoosh[1];
            MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            UltraSwoosh.NewUltraSwoosh(mplr?.mainColor ?? Main.DiscoColor, ultraSwooshes, 30, 80, center, heatMap ?? (mplr?.heatMap ?? LogSpiralLibraryMod.HeatMap[1].Value), false, 0, 1, null, aniTex ?? config.animateIndex, baseTex ?? config.imageIndex, alphaVector ?? config.colorVector.AlphaVector, false);
            ultraSwooshes[0].weaponTex = TextureAssets.Item[mplr?.Player.HeldItem.type ?? ItemID.TerraBlade].Value;
            ultraSwooshes[0].Uptate();
            spriteBatch.End();
            if (Main.gameMenu || !LogSpiralLibraryMod.CanUseRender)
                VertexDrawInfo.DrawVertexInfo(ultraSwooshes, typeof(UltraSwoosh), spriteBatch, null, null, null);
            else
                VertexDrawInfo.DrawVertexInfo(ultraSwooshes, typeof(UltraSwoosh), spriteBatch, null, null, null);



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
    public class AnimationTexPreview : SimplePreview<int>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, int data, ModConfig modConfig)
        {
            var tex = LogSpiralLibraryMod.AniTex[data + 11].Value;                //20 + 90
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 110, drawRange.Center().Y), new Vector2(180)), Color.White);
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)modConfig, null, null, data, null);
        }
    }
    public class BaseTexPreview : SimplePreview<int>
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, int data, ModConfig modConfig)
        {
            var tex = LogSpiralLibraryMod.BaseTex[data].Value;                //20 + 90
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 110, drawRange.Center().Y), new Vector2(180)), Color.White);
            if (Main.gameMenu)
                LogSpiralLibrarySystem.ModTime += .33f;
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)modConfig, null, data, null, null);
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
            PreviewHelper.DrawSorry(spriteBatch, drawRange);

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
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + (n - 29.5f) * 2f - 30) , new Vector2(180, 2)), Color.White);
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
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y + (n - 29.5f) * 2f- 30), new Vector2(180, 2)), Color.White);
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
            PreviewHelper.DrawSorry(spriteBatch, drawRange);
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
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(drawRange.X + 130, drawRange.Center().Y), new Vector2(240,40)), Color.White);
            PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(drawRange.X + drawRange.Width - 110, drawRange.Center().Y), (ConfigurationCIVE)modConfig, heatMap, null, null, null);
        }
    }
}
