using LogSpiralLibrary.CodeLibrary.ConfigModification;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using NetSimplified.Syncing;
using NetSimplified;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;
using LogSpiralLibrary.CodeLibrary.UIGenericConfig;
using Terraria.ModLoader.UI;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingEffects;
using LogSpiralLibrary.CodeLibrary.Utilties.BaseClasses;

namespace CoolerItemVisualEffect.Config
{
    public class SyncMeleeConfig : NetModule
    {
        public int plrIndex;
        public MeleeConfig configuration;
        public static SyncMeleeConfig Get(int plrIndex, MeleeConfig configurationCIVE)
        {
            var result = NetModuleLoader.Get<SyncMeleeConfig>();
            result.plrIndex = plrIndex;
            result.configuration = configurationCIVE;
            return result;
        }
        public override void Send(ModPacket p)
        {
            p.Write((byte)plrIndex);
            string content = JsonConvert.SerializeObject(configuration);
            p.Write(content);
            base.Send(p);
        }
        public override void Read(BinaryReader r)
        {
            plrIndex = r.ReadByte();
            string content = r.ReadString();
            configuration = new MeleeConfig();
            configuration.designateData?.colors.Clear();
            JsonConvert.PopulateObject(content, configuration);
            //configuration = (ConfigurationCIVE)JsonConvert.DeserializeObject(content);
            base.Read(r);
        }
        public override void Receive()
        {
            var plr = Main.player[plrIndex];
            var MMPlr = plr.GetModPlayer<MeleeModifyPlayer>();
            MMPlr.ConfigurationSwoosh = configuration;
            if (MMPlr.heatMap != null && MMPlr.hsl != default)
                MeleeModifyPlayer.UpdateHeatMap(ref MMPlr.heatMap, MMPlr.hsl, MMPlr.ConfigurationSwoosh, MeleeModifyPlayer.GetWeaponTextureFromItem(plr.HeldItem));
            if (Main.dedServ)
            {
                Get(plrIndex, configuration).Send(-1, plrIndex);
            }
        }
    }
    [AutoSync]
    public class SyncMeleeModifyActive : NetModule
    {
        public int plrIndex;
        public bool active;
        public static SyncMeleeModifyActive Get(int plrIndex, bool active)
        {
            var result = NetModuleLoader.Get<SyncMeleeModifyActive>();
            result.plrIndex = plrIndex;
            result.active = active;
            return result;
        }
        public override void Receive()
        {
            var plr = Main.player[plrIndex];
            var MMPlr = plr.GetModPlayer<MeleeModifyPlayer>();
            MMPlr.ConfigurationSwoosh.SwordModifyActive = active;
            if (MMPlr.heatMap != null && MMPlr.hsl != default)
                MeleeModifyPlayer.UpdateHeatMap(ref MMPlr.heatMap, MMPlr.hsl, MMPlr.ConfigurationSwoosh, MeleeModifyPlayer.GetWeaponTextureFromItem(plr.HeldItem));
            if (Main.dedServ)
            {
                Get(plrIndex, active).Send(-1, plrIndex);
            }
        }
    }
    [HorizonOverflowEnable]
    [RenderDrawingPreviewNeeded]
    public class MeleeConfig : ModConfig
    {
        [DefaultValue(true)]
        public bool UsePreview;

        [DefaultValue(false)]
        [CustomPreview<UseRenderPVPreivew>]
        public bool useRenderEffectPVInOtherConfig = false;

        public static MeleeConfig Instance => ModContent.GetInstance<MeleeConfig>();

        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override void OnChanged()
        {
            if (Main.player != null && Main.LocalPlayer != null && Main.LocalPlayer.active)
            {
                var plr = Main.LocalPlayer;
                var MMPlr = plr.GetModPlayer<MeleeModifyPlayer>();
                MeleeModifyPlayer.UpdateHeatMap(ref MMPlr.heatMap, MMPlr.hsl, MMPlr.ConfigurationSwoosh, MeleeModifyPlayer.GetWeaponTextureFromItem(plr.HeldItem));
                //foreach (var plr in Main.player) 
                //{
                //    if (plr == null || !plr.active) continue;
                //    var MMPlr = plr.GetModPlayer<MeleeModifyPlayer>();
                //    MeleeModifyPlayer.UpdateHeatMap(ref MMPlr.heatMap, MMPlr.hsl, MMPlr.ConfigurationSwoosh, MeleeModifyPlayer.GetWeaponTextureFromItem(plr.HeldItem));
                //}
                MMPlr.RefreshConfigEffects();
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
        [CustomGenericConfigItem<GenericSequenceDefinitionElement<MeleeAction>>]//为自己的UI里编辑Config做手脚
        [TypeConverter(typeof(ToFromStringConverter<SequenceDefinition<MeleeAction>>))]
        public SequenceDefinition<MeleeAction> swooshActionStyle = new(nameof(CoolerItemVisualEffect), nameof(CIVESword));

        [DefaultValue(7)]
        [Range(0, 11)]
        [Slider]
        [CustomPreview<BaseTexSwooshPreview>]
        [DrawTicks]
        public int baseIndexSwoosh = 7;

        [DefaultValue(3)]
        [Range(0, 5)]
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
        public ColorVector colorVector = new();//{ heatMapAlpha = 1, normalize = true }


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
        [CustomGenericConfigItem<DesignateColorConfigElement_Generic>]
        [CustomPreview<DesignedColorPreview>]
        public DesignateHeatMapData designateData = new();

        [DefaultValue(3.1415f)]
        [Range(0, 6.283f)]
        [Increment(0.05f)]
        [CustomPreview<DirectionOfHeatMapPreview>]
        public float directOfHeatMap = MathHelper.Pi;

        [Header("EffectPart")]
        [CustomModConfigItem(typeof(AvailableConfigElement))]
        [CustomGenericConfigItem<GenericAvailableConfigElement>]
        [CustomPreview<RenderEffectPreview>]
        public AirDistortConfigs distortConfigs = new ();

        [CustomModConfigItem(typeof(AvailableConfigElement))]
        [CustomGenericConfigItem<GenericAvailableConfigElement>]
        [CustomPreview<RenderEffectPreview>]
        public MaskConfigs maskConfigs = new ();

        [CustomModConfigItem(typeof(AvailableConfigElement))]
        [CustomGenericConfigItem<GenericAvailableConfigElement>]
        [CustomPreview<RenderEffectPreview>]
        public DyeConfigs dyeConfigs = new();

        [CustomModConfigItem(typeof(AvailableConfigElement))]
        [CustomGenericConfigItem<GenericAvailableConfigElement>]
        [CustomPreview<RenderEffectPreview>]
        public BloomConfigs bloomConfigs = new();
    }
    public class ColorVector
    {
        [Range(0, 1f)]
        [DefaultValue(1f)]
        public float mapColorAlpha = 1f;

        [Range(0, 1f)]
        [DefaultValue(0.25f)]
        public float weaponColorAlpha = .25f;

        [Range(0, 1f)]
        [DefaultValue(1f)]
        public float heatMapAlpha = 1f;

        [DefaultValue(true)]
        public bool normalize = true;
        [JsonIgnore]
        public Vector3 AlphaVector
        {
            get
            {
                var result = new Vector3(mapColorAlpha, weaponColorAlpha, heatMapAlpha);
                if (normalize)
                {
                    float sum = Vector3.Dot(Vector3.One, result);
                    if (sum == 0) return Vector3.One * .33f;
                    return result / sum;
                }
                else
                {
                    return result;
                }
            }
        }

        //
        //
        //
        //public List<int> useLessValues;

        public static bool operator ==(ColorVector v1, ColorVector v2)
        {
            return
                v1.mapColorAlpha == v2.mapColorAlpha &&
                v1.weaponColorAlpha == v2.weaponColorAlpha &&
                v1.heatMapAlpha == v2.heatMapAlpha &&
                v1.normalize == v2.normalize;
        }
        public static bool operator !=(ColorVector v1, ColorVector v2)
        {
            return !v1.Equals(v2);
        }

        public override bool Equals(object obj)=> obj is ColorVector vec && this == vec;

        public override int GetHashCode()
        {
            return mapColorAlpha.GetHashCode() ^ weaponColorAlpha.GetHashCode() ^ heatMapAlpha.GetHashCode() ^ normalize.GetHashCode();
        }
    }
}
