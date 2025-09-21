using CoolerItemVisualEffect.Common.MeleeModify;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingContents;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingEffects;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria.GameContent;
using Terraria.Localization;

namespace CoolerItemVisualEffect.Common.Config.Preview;

public static class PreviewHelper
{
    public static Vector3 DefaultHSL => new(0.3227513f, 0.25301206f, 0.4882353f);
    public static Texture2D PreviewAssistantHeatMap
    {
        get
        {
            CoolerItemVisualEffectHelper.CreateHeatMapIfNull(ref field);
            return field;
        }
    }
    public static float WeaponScalePVAssistant { get; set; }

    public static void DrawUltraSwoosh(SpriteBatch spriteBatch, Vector2 center, MeleeConfig config, Texture2D heatMap = null, int? baseTex = null, int? aniTex = null, Vector3? alphaVector = null, bool? useRenderEffect = null, Action<UltraSwoosh> otherOperation = null)
    {
        var mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
        if (PreviewAssistantHeatMap == null)
            MeleeModifyPlayerUtils.UpdateHeatMap(PreviewAssistantHeatMap, DefaultHSL, config, TextureAssets.Item[ItemID.TerraBlade].Value);//hsl使用铸炼的泰拉刃生成

        IRenderEffect[][] renderEffects = [[config.distortConfigs.EffectInstance], [config.maskConfigs.EffectInstance, config.dyeConfigs.EffectInstance, config.bloomConfigs.EffectInstance]];

        RenderingCanvas renderingCanvas = new(useRenderEffect ?? config.useRenderEffectPVInOtherConfig ? renderEffects : []) { IsUILayer = true };

        var content = new UltraSwoosh();
        content.timeLeft = content.timeLeftMax = 30;
        content.scaler = 80;
        content.center = center;
        content.angleRange = (-1.125f, 0.7125f);
        content.aniTexIndex = aniTex ?? config.animateIndexSwoosh;
        content.baseTexIndex = baseTex ?? config.baseIndexSwoosh;

        content.heatMap = heatMap ?? mplr?.HeatMap ?? PreviewAssistantHeatMap ?? LogSpiralLibraryMod.HeatMap[1].Value;
        content.ColorVector = alphaVector ?? config.colorVector.AlphaVector;

        content.weaponTex = MeleeModifyPlayerUtils.GetWeaponTextureFromItem(mplr?.Player.HeldItem);
        content.alphaFactor = config.alphaFactor;
        content.heatRotation = config.directOfHeatMap;
        content.Update();
        content.timeLeft++;
        otherOperation?.Invoke(content);

        renderingCanvas.Add(content);

        spriteBatch.End();
        renderingCanvas.DrawContents(spriteBatch, Main.graphics.GraphicsDevice);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
    }

    public static void DrawUltraStab(SpriteBatch spriteBatch, Vector2 center, MeleeConfig config, Texture2D heatMap = null, int? baseTex = null, int? aniTex = null, Vector3? alphaVector = null, bool? useRenderEffect = null, Action<UltraStab> otherOperation = null)
    {
        var mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
        if (PreviewAssistantHeatMap == null)
            MeleeModifyPlayerUtils.UpdateHeatMap(PreviewAssistantHeatMap, DefaultHSL, config, TextureAssets.Item[ItemID.TerraBlade].Value);//hsl使用铸炼的泰拉刃生成

        IRenderEffect[][] renderEffects = [[config.distortConfigs.EffectInstance], [config.maskConfigs.EffectInstance, config.dyeConfigs.EffectInstance, config.bloomConfigs.EffectInstance]];

        RenderingCanvas renderingCanvas = new(useRenderEffect ?? config.useRenderEffectPVInOtherConfig ? renderEffects : []) { IsUILayer = true };

        var content = new UltraStab();
        content.timeLeft = content.timeLeftMax = 30;
        content.scaler = 160;
        content.center = center;
        content.aniTexIndex = aniTex ?? config.animateIndexStab;
        content.baseTexIndex = baseTex ?? config.baseIndexStab;

        content.heatMap = heatMap ?? mplr?.HeatMap ?? PreviewAssistantHeatMap ?? LogSpiralLibraryMod.HeatMap[1].Value;
        content.ColorVector = alphaVector ?? config.colorVector.AlphaVector;

        content.weaponTex = MeleeModifyPlayerUtils.GetWeaponTextureFromItem(mplr?.Player.HeldItem);
        content.alphaFactor = config.alphaFactor;
        content.heatRotation = config.directOfHeatMap;
        content.Update();
        content.timeLeft++;
        otherOperation?.Invoke(content);

        renderingCanvas.Add(content);

        spriteBatch.End();
        renderingCanvas.DrawContents(spriteBatch, Main.graphics.GraphicsDevice);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
    }

    public static void DrawSorry(SpriteBatch spriteBatch, Rectangle rectangle)
    {
        var pos = rectangle.Left() + new Vector2(20, 0);
        spriteBatch.DrawString(FontAssets.MouseText.Value, Language.GetOrRegister("Mods.CoolerItemVisualEffect.Misc.UselessConfig").Value, pos, Color.White);
    }

    public static void DrawUnavailable(SpriteBatch spriteBatch, Rectangle rectangle)
    {
        var pos = rectangle.Left() + new Vector2(20, 0);
        spriteBatch.DrawString(FontAssets.MouseText.Value, Language.GetOrRegister("Mods.CoolerItemVisualEffect.Misc.UnavailableConfig").Value, pos, Color.White);
    }
}
