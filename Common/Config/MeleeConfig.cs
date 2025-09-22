using CoolerItemVisualEffect.Common.Config.Data.ByFuncHeatMap;
using CoolerItemVisualEffect.Common.Config.Data.ColorVectorData;
using CoolerItemVisualEffect.Common.Config.Data.CosineGenerateHeatMapData;
using CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap;
using CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.Vanilla;
using CoolerItemVisualEffect.Common.Config.NetSync;
using CoolerItemVisualEffect.Common.Config.Preview;
using CoolerItemVisualEffect.Common.MeleeModify;
using CoolerItemVisualEffect.MeleeModify;
using LogSpiralLibrary.CodeLibrary.ConfigModification;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingEffects;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.Definition;
using LogSpiralLibrary.CodeLibrary.Utilties.BaseClasses;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace CoolerItemVisualEffect.Common.Config;

[HorizonOverflowEnable]
[RenderDrawingPreviewNeeded]
public class MeleeConfig : ModConfig
{
    [DefaultValue(true)] 
    public bool UsePreview = true;

    [DefaultValue(false)]
    [CustomPreview<UseRenderPVPreivew>]
    public bool useRenderEffectPVInOtherConfig = false;

    public static MeleeConfig Instance { get; private set; }
    public override void OnLoaded()
    {
        Instance = this;
        base.OnLoaded();
    }
    public override ConfigScope Mode => ConfigScope.ClientSide;

    public override void OnChanged()
    {
        if (Main.player != null && Main.LocalPlayer != null && Main.LocalPlayer.active)
        {
            var plr = Main.LocalPlayer;
            var mplr = plr.GetModPlayer<MeleeModifyPlayer>();
            MeleeModifyPlayerUtils.UpdateHeatMap(mplr);
            mplr.RefreshConfigEffects();
        }
        if (GetHashCode() == Interface.modConfig?.modConfig?.GetHashCode())
            SyncMeleeConfig.Get(Main.myPlayer, this).Send();
        base.OnChanged();
    }

    [Header("MeleeModifyPart")]
    [DefaultValue(true)]
    [CustomPreview<ActivePreview>]
    public bool SwordModifyActive = true;

    [CustomModConfigItem(typeof(SequenceDefinitionElement<MeleeAction>))]
    [TypeConverter(typeof(ToFromStringConverter<SequenceDefinition<MeleeAction>>))]
    public SequenceDefinition<MeleeAction> swooshActionStyle = new(nameof(CoolerItemVisualEffect), nameof(CIVESword));

    [DefaultValue(7)]
    [Range(0, 13)]
    [Slider]
    [CustomPreview<BaseTexSwooshPreview>]
    [DrawTicks]
    public int baseIndexSwoosh = 7;

    [DefaultValue(3)]
    [Range(0, 6)]
    [Slider]
    [CustomPreview<AnimationTexSwooshPreview>]
    [DrawTicks]
    public int animateIndexSwoosh = 3;

    [DefaultValue(0)]
    [Range(0, 2)]
    [Slider]
    [CustomPreview<BaseTexStabPreview>]
    [DrawTicks]
    public int baseIndexStab = 0;

    [DefaultValue(9)]
    [Range(0, 11)]
    [Slider]
    [CustomPreview<AnimationTexStabPreview>]
    [DrawTicks]
    public int animateIndexStab = 5;

    [DefaultValue(10)]
    [Range(0, 60)]
    [Slider]
    [CustomPreview<TimeLeftPreview>]
    public int swooshTimeLeft = 10;//改

    [Increment(0.05f)]
    [DefaultValue(1f)]
    [Range(0f, 1f)]
    public float shake = 1f;//改

    /*[Increment(1f)]
    [DefaultValue(6f)]
    [Range(2f, 10f)]
    public float swingAttackTime = 6f;//改 */

    /*[Increment(0.05f)]
    [DefaultValue(0.1f)]
    [Range(0f, 1f)]
    public float glowLight = 0.1f;*/  //改

    [Increment(0.05f)]
    [DefaultValue(0.35f)]
    [Range(0f, 1f)]
    public float dustQuantity = 0.35f;

    //[SeparatePage]
    [Header("RenderingPart")]
    [Increment(0.05f)]
    [DefaultValue(0.35f)]
    [Range(0f, 1f)]
    [CustomPreview<WeaponExtraLightPreview>]
    public float weaponExtraLight = 0.35f;

    [CustomPreview<ColorVectorPreview>]
    public ColorVector colorVector = new();

    [Increment(0.05f)]
    [DefaultValue(1.5f)]
    [Range(0f, 3f)]
    [CustomPreview<AlphaScalerPreview>]
    public float alphaFactor = 1.5f;//暂定预览

    public enum HeatMapCreateStyle
    {
        ByFunction,
        FromTexturePixel,
        CosineGenerate_RGB,
        CosineGenerate_HSL,
        Designate
    }

    public enum HeatMapFactorStyle
    {
        Linear,
        Floor,
        Quadratic,
        SquareRoot,
        SmoothFloor,
    }

    [DrawTicks]
    [DefaultValue(HeatMapCreateStyle.ByFunction)]
    [CustomPreview<HeatMapCreatePreview>]
    public HeatMapCreateStyle heatMapCreateStyle = HeatMapCreateStyle.ByFunction;

    [DrawTicks]
    [DefaultValue(HeatMapFactorStyle.Linear)]
    [CustomPreview<HeatMapFactorPreview>]
    public HeatMapFactorStyle heatMapFactorStyle = HeatMapFactorStyle.Linear;//改为拖动曲线?

    [Expand(false)]
    public ByFuncHeatMapData byFuncData = new();

    [Expand(false)]
    [CustomPreview<CosinePreview>]
    public CosineGenerateHeatMapData_RGB rgbData = new();

    [Expand(false)]
    [CustomPreview<CosinePreview>]
    public CosineGenerateHeatMapData_HSL hslData = new();

    [Expand(false)]
    [CustomModConfigItem(typeof(DesignateColorConfigElement))]
    [CustomPreview<DesignedColorPreview>]
    public DesignateHeatMapData designateData = new();

    [DefaultValue(3.1415f)]
    [Range(0, 6.283f)]
    [Increment(0.05f)]
    [CustomPreview<DirectionOfHeatMapPreview>]
    public float directOfHeatMap = MathHelper.Pi;

    [Header("EffectPart")]
    [CustomModConfigItem(typeof(AvailableConfigElement))]
    [CustomPreview<RenderEffectPreview>]
    public AirDistortConfigs distortConfigs = new();

    [CustomModConfigItem(typeof(AvailableConfigElement))]
    [CustomPreview<RenderEffectPreview>]
    public MaskConfigs maskConfigs = new();

    [CustomModConfigItem(typeof(AvailableConfigElement))]
    [CustomPreview<RenderEffectPreview>]
    public DyeConfigs dyeConfigs = new();

    [CustomModConfigItem(typeof(AvailableConfigElement))]
    [CustomPreview<RenderEffectPreview>]
    public BloomConfigs bloomConfigs = new();
}

