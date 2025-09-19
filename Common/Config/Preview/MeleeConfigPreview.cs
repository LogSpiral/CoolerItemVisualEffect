using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.Config.Datas.ColorVectorData;
using CoolerItemVisualEffect.Common.Config.Datas.CosineGenerateHeatMapData;
using CoolerItemVisualEffect.Common.Config.Datas.DesignateHeatMap;
using CoolerItemVisualEffect.Common.MeleeModify;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.ConfigModification;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingContents;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingEffects;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace CoolerItemVisualEffect.Common.Config.Preview;



public abstract class MeleePreview<T> : SimplePreview<T>
{
    public override bool UsePreview => MeleeConfig.Instance.UsePreview;
}

public class UsePVPreview : MeleePreview<bool>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, bool data, OptionMetaData metaData)
    {
    }
}

public class ActivePreview : MeleePreview<bool>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, bool data, OptionMetaData metaData)
    {
        spriteBatch.Draw(data ? TextureAssets.Npc[NPCID.SkeletronPrime].Value : TextureAssets.Npc[NPCID.SkeletronHead].Value, dimension.Center(), data ? new Rectangle(0, 0, 140, 156) : null, Color.White, 0, data ? new Vector2(70, 78) : new Vector2(40, 51), 1f, 0, 0);
    }
}

public class AnimationTexSwooshPreview : MeleePreview<int>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, int data, OptionMetaData metaData)
    {
        var tex = LogSpiralLibraryMod.AniTex_Swoosh[data].Value;                //20 + 90
        spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 110, dimension.Center().Y), new Vector2(180)), Color.White);
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, data, null);
    }
}

public class BaseTexSwooshPreview : MeleePreview<int>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, int data, OptionMetaData metaData)
    {
        var tex = LogSpiralLibraryMod.BaseTex_Swoosh[data].Value;                //20 + 90
        spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 110, dimension.Center().Y), new Vector2(180)), Color.White);
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), (MeleeConfig)metaData.config, null, data, null, null);
    }
}

public class AnimationTexStabPreview : MeleePreview<int>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, int data, OptionMetaData metaData)
    {
        var tex = LogSpiralLibraryMod.AniTex_Stab[data].Value;                //20 + 90
        spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 110, dimension.Center().Y), new Vector2(180)), Color.White);
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(dimension.X + dimension.Width - 180, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, data, null);
    }
}

public class BaseTexStabPreview : MeleePreview<int>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, int data, OptionMetaData metaData)
    {
        var tex = LogSpiralLibraryMod.BaseTex_Stab[data].Value;                //20 + 90
        spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 110, dimension.Center().Y), new Vector2(180)), Color.White);
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(dimension.X + dimension.Width - 180, dimension.Center().Y), (MeleeConfig)metaData.config, null, data, null, null);
    }
}

public class TimeLeftPreview : MeleePreview<int>
{
    private static int timer;
    private static int timerMax;
    private static int coolDown;

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, int data, OptionMetaData metaData)
    {
        var config = (MeleeConfig)metaData.config;

        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        void ModifyTimer(VertexDrawInfo v)
        {
            if (timer <= -60)
                timerMax = timer = config.swooshTimeLeft;
            v.timeLeftMax = timerMax;
            v.timeLeft = timer < 0 ? 0 : timer;
        }
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), config, otherOperation: ModifyTimer);
        PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(dimension.X + 20, dimension.Center().Y), config, otherOperation: ModifyTimer);
        timer--;
    }
}

public class WeaponExtraLightPreview : MeleePreview<float>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, float data, OptionMetaData metaData)
    {
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        Item item = Main.gameMenu ? null : Main.LocalPlayer.HeldItem;
        Texture2D texture = Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayerUtils.GetWeaponTextureFromItem(item);
        var nframe = Main.itemAnimations[Main.gameMenu ? ItemID.TerraBlade : item.type]?.GetFrame(texture);
        CustomVertexInfo[] c = DrawingMethods.GetItemVertexes(new Vector2(0.1f, 0.9f), -MathHelper.PiOver4, MathHelper.PiOver4, 0, texture, 1, 1, dimension.Center(), false, 1f, nframe);
        Effect ItemEffect = LogSpiralLibraryMod.ItemEffectEX;
        if (ItemEffect == null) return;
        SamplerState sampler = SamplerState.AnisotropicWrap;
        var projection = Main.gameMenu ? Matrix.CreateOrthographicOffCenter(0, Main.instance.Window.ClientBounds.Width, Main.instance.Window.ClientBounds.Height, 0, 0, 1) : Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
        var model = Matrix.CreateTranslation(default);
        var trans = Main.gameMenu ? Matrix.CreateScale(Main.instance.Window.ClientBounds.Width / (float)Main.screenWidth, Main.instance.Window.ClientBounds.Height / (float)Main.screenHeight, 1) : Matrix.Identity;
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
            glow = item.glowMask != -1 ? TextureAssets.GlowMask[item.glowMask].Value : item.flame ? TextureAssets.ItemFlame[item.type].Value : null;
        }
        Main.graphics.GraphicsDevice.Textures[3] = glow;

        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
        Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
        Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
        ItemEffect.CurrentTechnique.Passes[0].Apply();
        for (int n = 0; n < c.Length; n++) c[n].Color = (Main.gameMenu ? Color.DarkGreen : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().MainColor) * data;
        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, c, 0, c.Length / 3);
        Main.graphics.GraphicsDevice.RasterizerState = originalState;
        Main.spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
    }
}

public class ColorVectorPreview : MeleePreview<ColorVector>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, ColorVector data, OptionMetaData metaData)
    {
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + 100, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, data.AlphaVector * Vector3.UnitX);
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + 150, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, data.AlphaVector * Vector3.UnitY);
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + 200, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, data.AlphaVector * Vector3.UnitZ);

        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, data.AlphaVector);
    }
}

public class AlphaScalerPreview : MeleePreview<float>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, float data, OptionMetaData metaData)
    {
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraSwoosh(spriteBatch, dimension.Center(), (MeleeConfig)metaData.config, null, null, null, null, null, u => u.alphaFactor = data);
    }
}

public class LightStandardPreview : MeleePreview<float>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, float data, OptionMetaData metaData)
    {
        PreviewHelper.DrawSorry(spriteBatch, dimension.ToRectangle());
    }
}

public class HeatMapCreatePreview : MeleePreview<MeleeConfig.HeatMapCreateStyle>
{
    private static Texture2D[] curHeatMaps = new Texture2D[5];

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, MeleeConfig.HeatMapCreateStyle data, OptionMetaData metaData)
    {
        var config = (MeleeConfig)metaData.config;

        //Texture2D[] curHeatMaps = new Texture2D[3];
        MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        for (int n = 0; n < 5; n++)
        {
            config.heatMapCreateStyle = (MeleeConfig.HeatMapCreateStyle)n;
            var tex = curHeatMaps[n];
            CoolerItemVisualEffectHelper.CreateHeatMapIfNull(ref tex);
            MeleeModifyPlayerUtils.UpdateHeatMap(tex, Main.gameMenu ? PreviewHelper.DefaultHSL : mplr.WeaponHSL, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value : MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
            curHeatMaps[n] = tex;
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 130, dimension.Center().Y + (n - 2) * 45), new Vector2(180, 20)), Color.White);
            if (n == (int)data)
            {
                PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), config, tex, null, null, null);
                if (mplr != null)
                    MeleeModifyPlayerUtils.UpdateHeatMap(mplr.HeatMap, mplr.WeaponHSL, config, MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                else
                    MeleeModifyPlayerUtils.UpdateHeatMap(PreviewHelper.PreviewAssistantHeatMap, PreviewHelper.DefaultHSL, config, TextureAssets.Item[ItemID.TerraBlade].Value);
            }
            //tex.Dispose();
        }
        spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(dimension.X + 20, dimension.Center().Y + ((int)data - 2) * 45), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
        config.heatMapCreateStyle = data;
    }
}

public class HeatMapFactorPreview : MeleePreview<MeleeConfig.HeatMapFactorStyle>
{
    private static Texture2D[] curHeatMaps = new Texture2D[5];

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, MeleeConfig.HeatMapFactorStyle data, OptionMetaData metaData)
    {
        var config = (MeleeConfig)metaData.config;

        //Texture2D[] curHeatMaps = new Texture2D[5];
        MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        for (int n = 0; n < 5; n++)
        {
            config.heatMapFactorStyle = (MeleeConfig.HeatMapFactorStyle)n;
            var tex = curHeatMaps[n];
            CoolerItemVisualEffectHelper.CreateHeatMapIfNull(ref tex);
            MeleeModifyPlayerUtils.UpdateHeatMap(tex, Main.gameMenu ? PreviewHelper.DefaultHSL : mplr.WeaponHSL, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value :  MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
            curHeatMaps[n] = tex;
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 130, dimension.Center().Y + (n - 2) * 45), new Vector2(180, 20)), Color.White);
            if (n == (int)data)
            {
                PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), config, tex, null, null, null);
                if (mplr != null)
                    MeleeModifyPlayerUtils.UpdateHeatMap(mplr.HeatMap, mplr.WeaponHSL, config, MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                else
                    MeleeModifyPlayerUtils.UpdateHeatMap(PreviewHelper.PreviewAssistantHeatMap, PreviewHelper.DefaultHSL, config, TextureAssets.Item[ItemID.TerraBlade].Value);
            }
            //tex.Dispose();
        }
        spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(dimension.X + 20, dimension.Center().Y + ((int)data - 2) * 45), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
        config.heatMapFactorStyle = data;
    }
}

public class HeatMapRelatedDatePreview : MeleePreview<float>
{
    private static Texture2D[] curHeatMaps = new Texture2D[60];

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, float data, OptionMetaData metaData)
    {
        var config = (MeleeConfig)metaData.config;
        if (config.heatMapCreateStyle != MeleeConfig.HeatMapCreateStyle.ByFunction)
        {
            PreviewHelper.DrawUnavailable(spriteBatch, dimension.ToRectangle());
            return;
        }
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
        (float Min, float Max) range = (0f, 1f);
        var rangeAttribute = metaData.GetAttribute<RangeAttribute>();
        if (rangeAttribute != null)
            range = ((float)rangeAttribute.Min, (float)rangeAttribute.Max);
        bool havntDraw = true;
        float stp = (range.Max - range.Min) / 60f;
        for (int n = 0; n < 60; n++)
        {
            float r = MathHelper.Lerp(range.Min, range.Max, n / 59f);
            metaData.Value = r;
            var tex = curHeatMaps[n];
            CoolerItemVisualEffectHelper.CreateHeatMapIfNull(ref tex);
            MeleeModifyPlayerUtils.UpdateHeatMap(tex, Main.gameMenu ? PreviewHelper.DefaultHSL : mplr.WeaponHSL, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value :  MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
            curHeatMaps[n] = tex;
            spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 130, dimension.Center().Y + (n - 29.5f) * 2f - 30), new Vector2(180, 2)), Color.White);
            if (Math.Abs(r - data) <= stp && havntDraw)
            {
                havntDraw = false;
                PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), config, tex, null, null, null);
                if (mplr != null)
                    MeleeModifyPlayerUtils.UpdateHeatMap(mplr.HeatMap, mplr.WeaponHSL, config, MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
                else
                    MeleeModifyPlayerUtils.UpdateHeatMap(PreviewHelper.PreviewAssistantHeatMap, PreviewHelper.DefaultHSL, config, TextureAssets.Item[ItemID.TerraBlade].Value);
                spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 130, dimension.Center().Y + 70), new Vector2(180, 30)), Color.White);
            }
            //tex.Dispose();
        }

        spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons").Value, new Vector2(dimension.X + 20, dimension.Center().Y + MathHelper.Lerp(-90, 30, Utils.GetLerpValue(range.Min, range.Max, data))), new Rectangle(32, 32, 32, 32), Color.White, 0, new Vector2(16), 1, 0, 0);
        metaData.Value = data;
    }
}

public class CosinePreview : MeleePreview<ICosineData>
{
    private static Texture2D CurrentHeatMap 
    {
        get 
        {
            CoolerItemVisualEffectHelper.CreateHeatMapIfNull(ref field);
            return field;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, ICosineData data, OptionMetaData metaData)
    {
        var config = (MeleeConfig)metaData.config;
        if (data is CosineGenerateHeatMapData_RGB && config.heatMapCreateStyle != MeleeConfig.HeatMapCreateStyle.CosineGenerate_RGB || data is CosineGenerateHeatMapData_HSL && config.heatMapCreateStyle != MeleeConfig.HeatMapCreateStyle.CosineGenerate_HSL)
        {
            PreviewHelper.DrawUnavailable(spriteBatch, dimension.ToRectangle());
            return;
        }
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;

        int w = (int)dimension.Width;
        int subW = w / 2 - 30;
        float k = 1f / (subW - 1f);
        float height = dimension.Height - 70;
        float r = MathF.Min(subW, height) * .5f;
        Vector2 topLeft = dimension.Position() + new Vector2(20, 20);
        Vector2 centerP = topLeft + new Vector2(20 + subW * 1.5f, height * .5f);
        int counter = 0;
        Func<float, CosineInfo, Color>[] methods = data.LineColorMethods;
        Color[] colors = data.LineColors;

        foreach (var cosineInfo in data.Cosines)
        {
            Func<float, CosineInfo, Color> func = null;
            if (methods != null)
                func = methods[counter];
            Color color = Color.White;
            if (colors != null)
                color = colors[counter];
            Vector2 prev = default;
            Vector2 prevP = default;

            for (int n = 0; n < subW; n++)
            {
                float t = n * k;
                float value = MathHelper.Clamp(cosineInfo.GetValue(t), 0, 1f);
                Vector2 current = topLeft + new Vector2(n, (1 - value) * height);
                Vector2 currentP = centerP + (t * MathHelper.TwoPi).ToRotationVector2() * r * value;
                if (n > 0)
                {
                    spriteBatch.DrawLine(prev, current, func?.Invoke(t, cosineInfo) ?? color, 2, false, default);
                    spriteBatch.DrawLine(prevP, currentP, func?.Invoke(t, cosineInfo) ?? color, 2, false, default);
                }
                prev = current;
                prevP = currentP;
            }
            counter++;
        }

        MeleeModifyPlayerUtils.UpdateHeatMap(CurrentHeatMap, default, config, TextureAssets.Item[ItemID.TerraBlade].Value);

        spriteBatch.Draw(CurrentHeatMap, dimension.ToRectangle().BottomLeft() + new Vector2(20, -40), null, Color.White, 0, default, new Vector2(subW / 300f, 20f), 0, 0);

        spriteBatch.Draw(CurrentHeatMap, dimension.ToRectangle().BottomLeft() + new Vector2(40 + subW, -40), null, Color.White, 0, default, new Vector2(subW / 300f, 20f), 0, 0);
    }
}

public class DesignedColorPreview : MeleePreview<DesignateHeatMapData>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, DesignateHeatMapData data, OptionMetaData metaData)
    {
        var config = (MeleeConfig)metaData.config;
        if (config.heatMapCreateStyle != MeleeConfig.HeatMapCreateStyle.Designate)
        {
            PreviewHelper.DrawUnavailable(spriteBatch, dimension.ToRectangle());
            return;
        }
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
        if (mplr != null)
            MeleeModifyPlayerUtils.UpdateHeatMap(mplr.HeatMap, mplr.WeaponHSL, config, MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
        else
            MeleeModifyPlayerUtils.UpdateHeatMap(PreviewHelper.PreviewAssistantHeatMap, PreviewHelper.DefaultHSL, config, TextureAssets.Item[ItemID.TerraBlade].Value);
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, null);
        PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(dimension.X + 20, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, null);
    }
}

public class DirectionOfHeatMapPreview : MeleePreview<float>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, float data, OptionMetaData metaData)
    {
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, null, null, u => u.heatRotation = data);
        PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(dimension.X + 20, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, null, null, u => u.heatRotation = data);

        //PreviewHelper.DrawSorry(spriteBatch, dimension);
    }
}

public class ColorListPreview : MeleePreview<List<Color>>
{
    private Texture2D HeatMap 
    {
        get 
        {
            CoolerItemVisualEffectHelper.CreateHeatMapIfNull(ref field);
            return field;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, List<Color> data, OptionMetaData metaData)//List<Color>
    {
        var config = (MeleeConfig)metaData.config;
        if (config.heatMapCreateStyle != MeleeConfig.HeatMapCreateStyle.Designate)
        {
            PreviewHelper.DrawUnavailable(spriteBatch, dimension.ToRectangle());
            return;
        }
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        MeleeModifyPlayer mplr = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
        MeleeModifyPlayerUtils.UpdateHeatMap(HeatMap, Main.gameMenu ? PreviewHelper.DefaultHSL : mplr.WeaponHSL, config, Main.gameMenu ? TextureAssets.Item[ItemID.TerraBlade].Value :  MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Main.LocalPlayer.HeldItem));
        var tex = HeatMap;
        spriteBatch.Draw(tex, Utils.CenteredRectangle(new Vector2(dimension.X + 130, dimension.Center().Y), new Vector2(240, 40)), Color.White);
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), (MeleeConfig)metaData.config, HeatMap, null, null, null);
    }
}

public class RenderEffectPreview : MeleePreview<object> //因为那几个通用的所以干脆obj得了
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, object data, OptionMetaData metaData)
    {
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, null, true);
        PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(dimension.X + 20, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, null, true);
    }
}

public class UseRenderPVPreivew : MeleePreview<bool>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, bool data, OptionMetaData metaData)
    {
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33f;
        PreviewHelper.DrawUltraSwoosh(spriteBatch, new Vector2(dimension.X + dimension.Width - 110, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, null, data);
        PreviewHelper.DrawUltraStab(spriteBatch, new Vector2(dimension.X + 20, dimension.Center().Y), (MeleeConfig)metaData.config, null, null, null, null, data);
    }
}