using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Terraria.UI;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.GameContent;
using Terraria.UI.Chat;
using System.Reflection;
using Terraria.ModLoader.UI;
using System.Linq;
using ReLogic.Graphics;
using Terraria.GameContent.UI.Elements;
using static CoolerItemVisualEffect.ConfigurationCIVE;
using Terraria.GameContent.UI.States;
using System.Collections;
//using CoolerItemVisualEffect.ConfigSLer;
using LogSpiralLibrary.CodeLibrary.DataStructures;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures;
using NetSimplified;
using NetSimplified.Syncing;

namespace CoolerItemVisualEffect
{
    public class SyncConfigCIVE : NetModule
    {
        public int plrIndex;
        public ConfigurationCIVE configuration;
        public static SyncConfigCIVE Get(int plrIndex, ConfigurationCIVE configurationCIVE)
        {
            var result = NetModuleLoader.Get<SyncConfigCIVE>();
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
            configuration = new ConfigurationCIVE();
            configuration.heatMapColors.Clear();
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
    public class ConfigurationCIVE : ModConfig
    {
        public static ConfigurationCIVE ConfigCIVEInstance => ModContent.GetInstance<ConfigurationCIVE>();

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
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SyncConfigCIVE.Get(Main.myPlayer, this).Send();
            }
            base.OnChanged();
        }

        [Header("MeleeModifyPart")]
        [DefaultValue(true)]
        public bool SwordModifyActive = true;

        [CustomModConfigItem(typeof(SequenceDefinitionElement<MeleeAction>))]
        [TypeConverter(typeof(ToFromStringConverter<SequenceDefinition<MeleeAction>>))]
        public SequenceDefinition<MeleeAction> swooshActionStyle = new SequenceDefinition<MeleeAction>(nameof(CoolerItemVisualEffect), nameof(CIVESword));

        [DefaultValue(7)]
        [Range(0, 11)]
        [Slider]
        public int imageIndex = 7;//改

        [DefaultValue(3)]
        [Range(0, 5)]
        [Slider]
        public int animateIndex = 3;//改

        [DefaultValue(10)]
        [Range(0, 60)]
        [Slider]
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
        [DefaultValue(1f)]
        [Range(0f, 1f)]
        public float dustQuantity = 1f;

        //[SeparatePage]
        [Header("RenderingPart")]
        public ColorVector colorVector = new ColorVector();//{ heatMapAlpha = 1, normalize = true }


        [Increment(0.05f)]
        [DefaultValue(1.5f)]
        [Range(0f, 3f)]
        public float alphaFactor = 1.5f;//暂定预览

        [Increment(0.05f)]
        [Range(0f, 1f)]
        [DefaultValue(0.2f)]
        public float isLighterDecider = 0.2f;//黑白名单形式?


        /*[DefaultValue(false)]
        public bool itemAdditive = false;

        [DefaultValue(false)]
        public bool itemHighLight
        {
            get => highLight;
            set
            {
                highLight = value;
            }
        }
        [JsonIgnore]
        [DefaultValue(false)]
        bool highLight = false;*/

        public enum HeatMapCreateStyle
        {
            ByFunction,
            FromTexturePixel,
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
        public HeatMapCreateStyle heatMapCreateStyle = HeatMapCreateStyle.ByFunction;

        [DrawTicks]
        [DefaultValue(HeatMapFactorStyle.Linear)]
        public HeatMapFactorStyle heatMapFactorStyle = HeatMapFactorStyle.Linear;//改为拖动曲线?


        [DefaultValue(0.2f)]
        [Increment(0.01f)]
        [Range(-1f, 1f)]
        public float hueOffsetRange = 0.2f;

        [DefaultValue(0f)]
        [Increment(0.01f)]
        [Range(0f, 1f)]
        public float hueOffsetValue = 0f;

        [DefaultValue(5f)]
        [Increment(0.05f)]
        [Range(0f, 5f)]
        public float saturationScalar = 5f;

        [DefaultValue(0.2f)]
        [Increment(0.05f)]
        [Range(0f, 1f)]
        public float luminosityRange = 0.2f;

        [DefaultValue(3.1415f)]
        [Range(0, 6.283f)]
        [Increment(0.05f)]
        public float directOfHeatMap = MathHelper.Pi;

        public List<Color> heatMapColors = new List<Color>() { Color.Blue, Color.Green, Color.Yellow };


        [DefaultValue(false)]
        public bool showHeatMap = false;

        [Header("EffectPart")]
        [CustomModConfigItem(typeof(AvailableConfigElement))]
        public AirDistortConfigs distortConfigs = new AirDistortConfigs();
        [CustomModConfigItem(typeof(AvailableConfigElement))]
        public BloomConfigs bloomConfigs = new BloomConfigs();
        [CustomModConfigItem(typeof(AvailableConfigElement))]
        public MaskConfigs maskConfigs = new MaskConfigs();
        public class AirDistortConfigs : IAvailabilityChangableConfig
        {
            public bool Available { get; set; } = true;
            [Range(0, 10f)]
            [DefaultValue(6f)]
            public float intensity = 6f;
            [Range(0, MathHelper.TwoPi)]
            public float rotation;
            [Range(0, 1)]
            [DefaultValue(0.5f)]
            public float colorOffset = .5f;
            //[Range(0, 2)]
            //public int tier = 1;
            [JsonIgnore]
            public AirDistortEffectInfo effectInfo => !Available ? default : new AirDistortEffectInfo(intensity, rotation, colorOffset);
        }
        public class BloomConfigs : IAvailabilityChangableConfig
        {
            public bool Available { get; set; } = true;
            [Range(0, 1f)]
            public float threshold = 0f;
            [Range(0, 1f)]
            public float intensity = 0.35f;
            [Range(1f, 12f)]
            public float range = 1;
            [Range(1, 5)]
            public int times = 3;
            [JsonIgnore]
            public bool additive = true;
            [JsonIgnore]
            public BloomEffectInfo effectInfo => !Available ? default : new BloomEffectInfo(threshold, intensity, range, times, additive);// - 4 + 8 * Main.GlobalTimeWrappedHourly.CosFactor()
        }
        public class MaskConfigs : IAvailabilityChangableConfig
        {
            public bool Available { get; set; } = false;
            [Range(0, 5)]
            public int SkyStyle = 1;
            public Color glowColor = new Color(152, 74, 255);//166,17,240//255,55,225//255,153,240
            [Range(0, 1f)]
            public float tier1 = 0.2f;
            [Range(0, 1f)]
            public float tier2 = 0.25f;
            //public bool lightAsAlpha = true;
            //public bool inverse;
            [JsonIgnore]
            public MaskEffectInfo maskEffectInfo => !Available ? default :
                new MaskEffectInfo(LogSpiralLibraryMod.Misc[20 + SkyStyle].Value, glowColor, tier1, tier2, default, true, false);

        }


        [DefaultValue(true)]

        [Header("MiscPart")]
        public bool useWeaponDisplay = true;

        [DefaultValue(true)]


        public bool firstWeaponDisplay = true;

        [Increment(0.05f)]
        [Range(0.5f, 2f)]
        [DefaultValue(1f)]
        [Slider]
        public float weaponScale = 1f;

        [DefaultValue(false)]


        public bool ItemDropEffectActive = false;

        [DefaultValue(false)]


        public bool ItemInventoryEffectActive = false;

        [DefaultValue(true)]


        public bool VanillaProjectileDrawModifyActive = true;

        [DefaultValue(false)]


        public bool TeleprotEffectActive = false;



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

        public override bool Equals(object obj)
        {
            if (obj is ColorVector vec)
                return this == vec;
            return false;
            //if (ReferenceEquals(this, obj))
            //{
            //    return true;
            //}

            //if (ReferenceEquals(obj, null))
            //{
            //    return false;
            //}

            //throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return mapColorAlpha.GetHashCode() ^ weaponColorAlpha.GetHashCode() ^ heatMapAlpha.GetHashCode() ^ normalize.GetHashCode();
        }
    }
}