using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace CoolerItemVisualEffect
{

    [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label")]
    public class ConfigurationSwoosh : ModConfig
    {
        #region Basic
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static ConfigurationSwoosh ConfigSwooshInstance => ModContent.GetInstance<ConfigurationSwoosh>();
        #endregion

        #region 同步 字段比较等
        public void SendData(int? whoami = null, int ignoreCilent = -1, int toCilent = -1, bool enter = false)
        {
            ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
            packet.Write((byte)(enter ? HandleNetwork.MessageType.EnterWorld : HandleNetwork.MessageType.Configs));
            if (whoami != null) packet.Write(whoami.Value);
            packet.Write((byte)coolerSwooshQuality);
            packet.Write(toolsNoUseNewSwooshEffect);
            packet.Write(isLighterDecider);
            packet.Write((byte)swooshColorType);
            packet.Write((byte)swooshSampler);
            packet.Write((byte)swooshFactorStyle);
            packet.Write((byte)swooshActionStyle);
            packet.Write(swooshSize);
            packet.Write(hueOffsetRange);
            packet.Write(hueOffsetValue);
            packet.Write(saturationScalar);
            packet.Write(luminosityRange);
            packet.Write(luminosityFactor);
            packet.Write(swingAttackTime);
            packet.Write(distortFactor);
            packet.Write(itemAdditive);
            packet.Write(itemHighLight);
            packet.Write(shake);
            packet.Write((byte)imageIndex);
            packet.Write(checkAir);
            packet.Write(gather);
            packet.Write(allowZenith);
            packet.Write(glowLight);
            packet.Write((byte)maxCount);
            packet.Write(directOfHeatMap);
            packet.Write((byte)swooshTimeLeft);
            packet.Write(onlyChangeSizeOfSwoosh);
            packet.Write((byte)fadeStyle);
            packet.Write((byte)growStyle);
            packet.Write((byte)animateIndex);
            packet.Write(distortSize);
            packet.Write(showHeatMap);
            packet.Write(actionOffsetSize);
            packet.Write(actionOffsetSpeed);
            packet.Write(actionModifyEffect);
            packet.Write(ignoreDamageType);
            var count = (byte)heatMapColors.Count;
            packet.Write(count);
            for (int n = 0; n < count; n++)
            {
                packet.Write(heatMapColors[n].PackedValue);
            }
            packet.Write(alphaFactor);
            packet.Write(dustQuantity);
            packet.Write((byte)heatMapCreateStyle);
            packet.Write(useWeaponDisplay);
            packet.Write(firstWeaponDisplay);
            packet.Write(weaponScale);
            packet.Write((byte)hitBoxStyle);
            packet.Write(DontChangeMyTitle);
            packet.Write(ItemDropEffectActive);
            packet.Write(ItemInventoryEffectActive);
            packet.Write(VanillaProjectileDrawModifyActive);
            packet.Write(TeleprotEffectActive);
            packet.Write(CelesteMoveAnimation);
            packet.Send(toCilent, ignoreCilent);
        }
        public static void SetData(BinaryReader reader, int whoami)
        {
            if (whoami < 0 || whoami > 255) throw new System.Exception("我抄，超范围辣");
            var config = Main.player[whoami].GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh;
            config.coolerSwooshQuality = (QualityType)reader.ReadByte();
            config.toolsNoUseNewSwooshEffect = reader.ReadBoolean();
            config.isLighterDecider = reader.ReadSingle();
            config.swooshColorType = (SwooshColorType)reader.ReadByte();
            config.swooshSampler = (SwooshSamplerState)reader.ReadByte();
            config.swooshFactorStyle = (SwooshFactorStyle)reader.ReadByte();
            config.swooshActionStyle = (SwooshAction)reader.ReadByte();
            config.swooshSize = reader.ReadSingle();
            config.hueOffsetRange = reader.ReadSingle();
            config.hueOffsetValue = reader.ReadSingle();
            config.saturationScalar = reader.ReadSingle();
            config.luminosityRange = reader.ReadSingle();
            config.luminosityFactor = reader.ReadSingle();
            config.swingAttackTime = reader.ReadSingle();
            config.distortFactor = reader.ReadSingle();
            config.itemAdditive = reader.ReadBoolean();
            config.itemHighLight = reader.ReadBoolean();
            config.shake = reader.ReadSingle();
            config.imageIndex = reader.ReadByte();
            config.checkAir = reader.ReadBoolean();
            config.gather = reader.ReadBoolean();
            config.allowZenith = reader.ReadBoolean();
            config.glowLight = reader.ReadSingle();
            config.maxCount = reader.ReadByte();
            config.directOfHeatMap = reader.ReadSingle();
            config.swooshTimeLeft = reader.ReadByte();
            config.onlyChangeSizeOfSwoosh = reader.ReadBoolean();
            config.fadeStyle = (SwooshFadeStyle)reader.ReadByte();
            config.growStyle = (SwooshGrowStyle)reader.ReadByte();
            config.animateIndex = reader.ReadByte();
            config.distortSize = reader.ReadSingle();
            config.showHeatMap = reader.ReadBoolean();
            config.actionOffsetSize = reader.ReadBoolean();
            config.actionOffsetSpeed = reader.ReadBoolean();
            config.actionModifyEffect = reader.ReadBoolean();
            config.ignoreDamageType = reader.ReadBoolean();
            config.heatMapColors.Clear();
            var count = reader.ReadByte();
            for (int n = 0; n < count; n++)
            {
                config.heatMapColors.Add(new Color() { PackedValue = reader.ReadUInt32() });
            }
            config.alphaFactor = reader.ReadSingle();
            config.dustQuantity = reader.ReadSingle();
            config.heatMapCreateStyle = (HeatMapCreateStyle)reader.ReadByte();
            config.useWeaponDisplay = reader.ReadBoolean();
            config.firstWeaponDisplay = reader.ReadBoolean();
            config.weaponScale = reader.ReadSingle();
            config.hitBoxStyle = (HitBoxStyle)reader.ReadByte();
            config.DontChangeMyTitle = reader.ReadBoolean();
            config.ItemDropEffectActive = reader.ReadBoolean();
            config.ItemInventoryEffectActive = reader.ReadBoolean();
            config.VanillaProjectileDrawModifyActive = reader.ReadBoolean();
            config.TeleprotEffectActive = reader.ReadBoolean();
            config.CelesteMoveAnimation = reader.ReadBoolean();
        }
        bool EqualValue(ConfigurationSwoosh config)
        {
            return
                CoolerSwooshActive == config.CoolerSwooshActive &&
                toolsNoUseNewSwooshEffect == config.toolsNoUseNewSwooshEffect &&
                isLighterDecider == config.isLighterDecider &&
                swooshColorType == config.swooshColorType &&
                swooshSampler == config.swooshSampler &&
                swooshFactorStyle == config.swooshFactorStyle &&
                swooshActionStyle == config.swooshActionStyle &&
                swooshSize == config.swooshSize &&
                hueOffsetRange == config.hueOffsetRange &&
                hueOffsetValue == config.hueOffsetValue &&
                saturationScalar == config.saturationScalar &&
                luminosityRange == config.luminosityRange &&
                luminosityFactor == config.luminosityFactor &&
                swingAttackTime == config.swingAttackTime &&
                distortFactor == config.distortFactor &&
                itemAdditive == config.itemAdditive &&
                itemHighLight == config.itemHighLight &&
                shake == config.shake &&
                imageIndex == config.imageIndex &&
                checkAir == config.checkAir &&
                gather == config.gather &&
                allowZenith == config.allowZenith &&
                glowLight == config.glowLight &&
                maxCount == config.maxCount &&
                directOfHeatMap == config.directOfHeatMap &&
                swooshTimeLeft == config.swooshTimeLeft &&
                onlyChangeSizeOfSwoosh == config.onlyChangeSizeOfSwoosh &&
                fadeStyle == config.fadeStyle &&
                growStyle == config.growStyle &&
                animateIndex == config.animateIndex &&
                distortSize == config.distortSize &&
                showHeatMap == config.showHeatMap &&
                actionOffsetSize == config.actionOffsetSize &&
                actionOffsetSpeed == config.actionOffsetSpeed &&
                actionModifyEffect == config.actionModifyEffect &&
                ignoreDamageType == config.ignoreDamageType &&
                heatMapColors.EqualValue(config.heatMapColors) &&
                alphaFactor == config.alphaFactor &&
                dustQuantity == config.dustQuantity &&
                heatMapCreateStyle == config.heatMapCreateStyle && true;
                //useWeaponDisplay == config.useWeaponDisplay &&
                //firstWeaponDisplay == config.firstWeaponDisplay &&
                //weaponScale == config.weaponScale &&
                //hitBoxStyle == config.hitBoxStyle &&
                //DontChangeMyTitle == config.DontChangeMyTitle &&
                //ItemDropEffectActive == config.ItemDropEffectActive &&
                //ItemInventoryEffectActive == config.ItemInventoryEffectActive &&
                //VanillaProjectileDrawModifyActive == config.VanillaProjectileDrawModifyActive &&
                //TeleprotEffectActive == config.TeleprotEffectActive &&
                //CelesteMoveAnimation == config.CelesteMoveAnimation;
        }
        #endregion

        #region 方便访问的属性
        [JsonIgnore]
        public QualityType coolerSwooshQuality
        {
            get => meleeSwooshConfigs.coolerSwooshQuality;
            set => meleeSwooshConfigs.coolerSwooshQuality = value;
        }

        [JsonIgnore]
        public bool CoolerSwooshActive => (byte)coolerSwooshQuality > 0;
        [JsonIgnore]
        public bool toolsNoUseNewSwooshEffect
        {
            get => meleeSwooshConfigs.toolsNoUseNewSwooshEffect;
            set => meleeSwooshConfigs.toolsNoUseNewSwooshEffect = value;
        }
        [JsonIgnore]
        public float isLighterDecider
        {
            get => drawConfigs.isLighterDecider;
            set => drawConfigs.isLighterDecider = value;
        }

        [JsonIgnore]
        public SwooshColorType swooshColorType
        {
            get => drawConfigs.swooshColorType;
            set => drawConfigs.swooshColorType = value;
        }

        [JsonIgnore]
        public SwooshSamplerState swooshSampler
        {
            get => drawConfigs.swooshSampler;
            set => drawConfigs.swooshSampler = value;
        }

        [JsonIgnore]
        public SwooshFactorStyle swooshFactorStyle
        {
            get => drawConfigs.swooshFactorStyle;
            set => drawConfigs.swooshFactorStyle = value;
        }

        [JsonIgnore]
        public SwooshAction swooshActionStyle
        {
            get => meleeSwooshConfigs.swooshActionStyle;
            set => meleeSwooshConfigs.swooshActionStyle = value;
        }

        [JsonIgnore]
        public float swooshSize
        {
            get => meleeSwooshConfigs.swooshSize;
            set => meleeSwooshConfigs.swooshSize = value;
        }

        [JsonIgnore]
        public float hueOffsetRange
        {
            get => heatMapConfigs.hueOffsetRange;
            set => heatMapConfigs.hueOffsetRange = value;
        }


        [JsonIgnore]
        public float hueOffsetValue
        {
            get => heatMapConfigs.hueOffsetValue;
            set => heatMapConfigs.hueOffsetValue = value;
        }

        [JsonIgnore]
        public float saturationScalar
        {
            get => heatMapConfigs.saturationScalar;
            set => heatMapConfigs.saturationScalar = value;
        }

        [JsonIgnore]
        public float luminosityRange
        {
            get => heatMapConfigs.luminosityRange;
            set => heatMapConfigs.luminosityRange = value;
        }

        [JsonIgnore]
        public float luminosityFactor
        {
            get => renderConfigs.luminosityFactor;
            set => renderConfigs.luminosityFactor = value;
        }

        [JsonIgnore]
        public float swingAttackTime
        {
            get => meleeSwooshConfigs.swingAttackTime;
            set => meleeSwooshConfigs.swingAttackTime = value;
        }

        [JsonIgnore]
        public float distortFactor
        {
            get => renderConfigs.distortFactor;
            set => renderConfigs.distortFactor = value;
        }

        [JsonIgnore]
        public bool itemAdditive
        {
            get => drawConfigs.itemAdditive;
            set => drawConfigs.itemAdditive = value;
        }

        [JsonIgnore]
        public bool itemHighLight
        {
            get => drawConfigs.itemHighLight;
            set => drawConfigs.itemHighLight = value;
        }

        [JsonIgnore]
        public float shake
        {
            get => meleeSwooshConfigs.shake;
            set => meleeSwooshConfigs.shake = value;
        }

        [JsonIgnore]
        public float imageIndex
        {
            get => meleeSwooshConfigs.imageIndex;
            set => meleeSwooshConfigs.imageIndex = value;
        }
        [JsonIgnore]
        public int ImageIndex => (int)MathHelper.Clamp(ConfigSwooshInstance.imageIndex, 0, 11);

        [JsonIgnore]
        public bool checkAir
        {
            get => meleeSwooshConfigs.checkAir;
            set => meleeSwooshConfigs.checkAir = value;
        }

        [JsonIgnore]
        public bool gather
        {
            get => meleeSwooshConfigs.gather;
            set => meleeSwooshConfigs.gather = value;
        }

        [JsonIgnore]
        public bool allowZenith
        {
            get => meleeSwooshConfigs.allowZenith;
            set => meleeSwooshConfigs.allowZenith = value;
        }

        [JsonIgnore]
        public float glowLight
        {
            get => meleeSwooshConfigs.glowLight;
            set => meleeSwooshConfigs.glowLight = value;
        }

        [JsonIgnore]
        public int maxCount
        {
            get => renderConfigs.maxCount;
            set => renderConfigs.maxCount = value;
        }

        [JsonIgnore]
        public float directOfHeatMap
        {
            get => heatMapConfigs.directOfHeatMap;
            set => heatMapConfigs.directOfHeatMap = value;
        }

        [JsonIgnore]
        public float swooshTimeLeft
        {
            get => meleeSwooshConfigs.swooshTimeLeft;
            set => meleeSwooshConfigs.swooshTimeLeft = value;
        }

        [JsonIgnore]
        public bool onlyChangeSizeOfSwoosh
        {
            get => meleeSwooshConfigs.onlyChangeSizeOfSwoosh;
            set => meleeSwooshConfigs.onlyChangeSizeOfSwoosh = value;
        }

        [JsonIgnore]
        public SwooshFadeStyle fadeStyle
        {
            get => meleeSwooshConfigs.fadeStyle;
            set => meleeSwooshConfigs.fadeStyle = value;
        }

        [JsonIgnore]
        public bool IsTransparentFade => fadeStyle == SwooshFadeStyle.逐渐透明GraduallyTransparent || fadeStyle == SwooshFadeStyle.全部Both;

        [JsonIgnore]
        public bool IsCloseAngleFade => fadeStyle == SwooshFadeStyle.角度收缩CloseTheAngle || fadeStyle == SwooshFadeStyle.全部Both;

        [JsonIgnore]
        public bool IsDarkFade => fadeStyle == SwooshFadeStyle.逐渐黯淡GraduallyFade || fadeStyle == SwooshFadeStyle.全部Both;

        [JsonIgnore]
        public SwooshGrowStyle growStyle
        {
            get => meleeSwooshConfigs.growStyle;
            set => meleeSwooshConfigs.growStyle = value;
        }

        [JsonIgnore]
        public bool IsExpandGrow => growStyle == SwooshGrowStyle.扩大Expand || growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;

        [JsonIgnore]
        public bool IsHorizontallyGrow => growStyle == SwooshGrowStyle.横向扩大ExpandHorizontally || growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;

        [JsonIgnore]
        public bool IsOffestGrow => growStyle == SwooshGrowStyle.平移Offest || growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;

        [JsonIgnore]
        public float animateIndex
        {
            get => meleeSwooshConfigs.animateIndex;
            set => meleeSwooshConfigs.animateIndex = value;
        }
        [JsonIgnore]
        public int AnimateIndex => (int)MathHelper.Clamp(ConfigSwooshInstance.animateIndex, 0, 5);

        [JsonIgnore]
        public float distortSize
        {
            get => renderConfigs.distortSize;
            set => renderConfigs.distortSize = value;
        }

        [JsonIgnore]
        public bool showHeatMap
        {
            get => heatMapConfigs.showHeatMap;
            set => heatMapConfigs.showHeatMap = value;
        }

        [JsonIgnore]
        public bool actionOffsetSize
        {
            get => meleeSwooshConfigs.actionOffsetSize;
            set => meleeSwooshConfigs.actionOffsetSize = value;
        }

        [JsonIgnore]
        public bool actionOffsetSpeed
        {
            get => meleeSwooshConfigs.actionOffsetSpeed;
            set => meleeSwooshConfigs.actionOffsetSpeed = value;
        }

        [JsonIgnore]
        public bool actionModifyEffect
        {
            get => meleeSwooshConfigs.actionModifyEffect;
            set => meleeSwooshConfigs.actionModifyEffect = value;
        }

        [JsonIgnore]
        public bool ignoreDamageType
        {
            get => meleeSwooshConfigs.ignoreDamageType;
            set => meleeSwooshConfigs.ignoreDamageType = value;
        }

        [JsonIgnore]
        public HeatMapFactorStyle heatMapFactorStyle
        {
            get => heatMapConfigs.heatMapFactorStyle;
            set => heatMapConfigs.heatMapFactorStyle = value;
        }

        [JsonIgnore]
        public List<Color> heatMapColors
        {
            get => heatMapConfigs.heatMapColors;
            set => heatMapConfigs.heatMapColors = value;
        }

        [JsonIgnore]
        public float alphaFactor
        {
            get => drawConfigs.alphaFactor;
            set => drawConfigs.alphaFactor = value;
        }

        [JsonIgnore]
        public float dustQuantity
        {
            get => otherConfigs.dustQuantity;
            set => otherConfigs.dustQuantity = value;
        }

        [JsonIgnore]
        public HeatMapCreateStyle heatMapCreateStyle
        {
            get => heatMapConfigs.heatMapCreateStyle;
            set => heatMapConfigs.heatMapCreateStyle = value;
        }

        [JsonIgnore]
        public bool useWeaponDisplay
        {
            get => otherConfigs.useWeaponDisplay;
            set => otherConfigs.useWeaponDisplay = value;
        }

        [JsonIgnore]
        public bool firstWeaponDisplay
        {
            get => otherConfigs.firstWeaponDisplay;
            set => otherConfigs.firstWeaponDisplay = value;
        }
        [JsonIgnore]
        public float weaponScale
        {
            get => otherConfigs.weaponScale;
            set => otherConfigs.weaponScale = value;
        }

        [JsonIgnore]
        public HitBoxStyle hitBoxStyle
        {
            get => otherConfigs.hitBoxStyle;
            set => otherConfigs.hitBoxStyle = value;
        }

        [JsonIgnore]
        public bool DontChangeMyTitle
        {
            get => otherConfigs.DontChangeMyTitle;
            set => otherConfigs.DontChangeMyTitle = value;
        }

        //[JsonIgnore]
        //public PreInstallSwoosh preInstallSwoosh
        //{
        //    get;
        //    set;
        //}
        [JsonIgnore]
        public bool ItemDropEffectActive
        {
            get => otherConfigs.ItemDropEffectActive;
            set => otherConfigs.ItemDropEffectActive = value;
        }

        [JsonIgnore]
        public bool ItemInventoryEffectActive
        {
            get => otherConfigs.ItemInventoryEffectActive;
            set => otherConfigs.ItemInventoryEffectActive = value;
        }

        [JsonIgnore]
        public bool VanillaProjectileDrawModifyActive
        {
            get => otherConfigs.VanillaProjectileDrawModifyActive;
            set => otherConfigs.VanillaProjectileDrawModifyActive = value;

        }

        [JsonIgnore]
        public bool TeleprotEffectActive
        {
            get => otherConfigs.TeleprotEffectActive;
            set => otherConfigs.TeleprotEffectActive = value;
        }

        [JsonIgnore]
        public bool CelesteMoveAnimation
        {
            get => otherConfigs.CelesteMoveAnimation;
            set => otherConfigs.CelesteMoveAnimation = value;
        }
        #endregion

        #region 子页实例
        [SeparatePage]
        [BackgroundColor(0, 255, 255, 127)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label1")]
        public MeleeSwooshConfigs meleeSwooshConfigs = new MeleeSwooshConfigs();


        [SeparatePage]
        [BackgroundColor(0, 128, 255, 127)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label2")]
        public DrawConfigs drawConfigs = new DrawConfigs();


        [SeparatePage]
        [BackgroundColor(0, 0, 255, 127)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label3")]
        public HeatMapConfigs heatMapConfigs = new HeatMapConfigs();


        [SeparatePage]
        [BackgroundColor(128, 0, 255, 127)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label4")]
        public RenderConfigs renderConfigs = new RenderConfigs();


        [SeparatePage]
        [BackgroundColor(255, 0, 255, 127)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label5")]
        public OtherConfigs otherConfigs = new OtherConfigs();
        #endregion

        #region 枚举
        public enum PreInstallSwoosh
        {
            普通Normal,
            飓风Hurricane,
            巨大Huge,
            夸张Exaggerate,
            明亮Bright,
            黑暗Dark,
            光滑Smooth,
            泰拉Terra_EspeciallyTerraBladeRecommended,
            神圣Holy_EspeciallyTrueExcaliburRecommended,
            永夜Evil_EspeciallyTrueNightsEdgeRecommended,
            旧日OldOnes_EspeciallyFlyingDragonRecommended,
            波涌Influx_EspeciallyInfluxWaverRecommended,
            黑白Grey,
            反相InverseHue,
            彩虹Rainbow,
            超级彩虹UltraRainbow,
            自定义UserDefined
        }
        public enum HitBoxStyle
        {
            原版Vanilla,
            矩形Rectangle,
            线状AABBLine,
            剑气UltraSwoosh,
            弹幕Projectile
        }
        public enum SwooshSamplerState : byte
        {
            各向异性,
            线性,
            点,
        }
        public enum SwooshAction : byte
        {
            左右横劈,
            左右横劈_后倾,
            重斩,
            上挑,
            腾云斩,
            旋风劈,
            流雨断,
            鸣雷刺,
            风暴灭却剑,
            左右横劈_后倾_旧,
            左右横劈_失败,

        }
        public enum SwooshFactorStyle : byte
        {
            每次开始时决定系数,
            系数中间插值
        }
        public enum SwooshColorType : byte
        {
            加权平均,
            单向渐变,
            武器贴图对角线,
            单向渐变与对角线混合,
            热度图,
        }
        public enum QualityType : byte
        {
            关off,
            低low,
            中medium,
            高high,
            极限ultra
        }
        public enum SwooshFadeStyle : byte
        {
            逐渐透明GraduallyTransparent,
            角度收缩CloseTheAngle,
            逐渐黯淡GraduallyFade,
            全部Both
        }
        public enum SwooshGrowStyle : byte
        {
            扩大Expand,
            横向扩大ExpandHorizontally,
            平移Offest,
            横向扩大与平移BothExpandHorizontallyAndOffest
        }
        public enum HeatMapFactorStyle : byte
        {
            线性Linear,
            分块Floor,
            二次Quadratic,
            平方根SquareRoot,
            柔和分块SmoothFloor
        }
        public enum HeatMapCreateStyle : byte
        {
            函数生成,
            贴图生成,
            指定
        }
        #endregion

        #region Config类
        public class MeleeSwooshConfigs
        {
            #region 基本设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D1")]
            [DrawTicks]
            [DefaultValue(QualityType.极限ultra)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.1")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.2")]
            [BackgroundColor(248, 0, 255, 255)] public QualityType coolerSwooshQuality = QualityType.极限ultra;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.3")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.4")]
            [BackgroundColor(242, 0, 255, 255)] public bool toolsNoUseNewSwooshEffect = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.43")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.44")]
            [BackgroundColor(236, 0, 255, 255)] public bool allowZenith = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.69")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.70")]
            [BackgroundColor(230, 0, 255, 255)] public bool actionOffsetSize = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.73")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.74")]
            [BackgroundColor(224, 0, 255, 255)] public bool actionOffsetSpeed = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.75")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.76")]
            [BackgroundColor(218, 0, 255, 255)] public bool actionModifyEffect = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.41")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.42")]
            [BackgroundColor(212, 0, 255, 255)] public bool gather = true;
            #endregion

            #region 样式设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D2")]
            [DrawTicks]
            [DefaultValue(SwooshAction.左右横劈_后倾)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.13")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.14")]
            [BackgroundColor(206, 0, 255, 255)] public SwooshAction swooshActionStyle = SwooshAction.左右横劈_后倾;

            [Increment(1f)]
            [DefaultValue(7f)]
            [Range(0, 11f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.37")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.38")]
            [BackgroundColor(200, 0, 255, 255)] public float imageIndex = 7;

            [Increment(1f)]
            [DefaultValue(3f)]
            [Range(0, 5f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.63")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.64")]
            [BackgroundColor(194, 0, 255, 255)] public float animateIndex = 3;

            [DrawTicks]
            [DefaultValue(SwooshFadeStyle.全部Both)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.59")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.60")]
            [BackgroundColor(188, 0, 255, 255)] public SwooshFadeStyle fadeStyle = SwooshFadeStyle.全部Both;

            [DrawTicks]
            [DefaultValue(SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.61")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.62")]
            [BackgroundColor(182, 0, 255, 255)] public SwooshGrowStyle growStyle = SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;
            #endregion

            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [Increment(0.05f)]
            [DefaultValue(1f)]
            [Range(0.5f, 3f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.15")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.16")]
            [BackgroundColor(176, 0, 255, 255)] public float swooshSize = 1f;

            [DefaultValue(30f)]
            [Range(0, 60f)]
            [Increment(1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.55")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.56")]
            [BackgroundColor(170, 0, 255, 255)] public float swooshTimeLeft = 30f;

            [Increment(0.05f)]
            [DefaultValue(0f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.35")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.36")]
            [BackgroundColor(164, 0, 255, 255)] public float shake = 0f;
            #endregion

            #region 细节设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D4")]
            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.39")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.40")]
            [BackgroundColor(158, 0, 255, 255)] public bool checkAir = true;

            [Increment(1f)]
            [DefaultValue(4f)]
            [Range(2f, 10f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.89")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.90")]
            [BackgroundColor(152, 0, 255, 255)] public float swingAttackTime = 3f;

            [Increment(0.05f)]
            [DefaultValue(0f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.45")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.46")]
            [BackgroundColor(146, 0, 255, 255)] public float glowLight = 0f;
            #endregion

            #region 试验性设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D5")]
            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.57")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.58")]
            [BackgroundColor(140, 0, 255, 255)] public bool onlyChangeSizeOfSwoosh = false;

            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.77")]
            [BackgroundColor(134, 0, 255, 255)]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.78")]
            public bool ignoreDamageType = false;
            #endregion
        }
        public class DrawConfigs
        {
            #region 样式设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D2")]
            [DrawTicks]
            [DefaultValue(SwooshColorType.热度图)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.7")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.8")]
            [BackgroundColor(112, 0, 255, 255)] public SwooshColorType swooshColorType = SwooshColorType.热度图;
            #endregion

            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [Increment(0.05f)]
            [DefaultValue(1.5f)]
            [Range(0f, 3f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.83")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.84")]
            [BackgroundColor(96, 0, 255, 255)] public float alphaFactor = 1.5f;

            [Increment(0.05f)]
            [Range(0f, 1f)]
            [DefaultValue(0.2f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.5")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.6")]
            [BackgroundColor(80, 0, 255, 255)] public float isLighterDecider = 0.2f;
            #endregion

            #region 细节设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D4")]
            [DrawTicks]
            [DefaultValue(SwooshSamplerState.线性)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.9")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.10")]
            [BackgroundColor(64, 0, 255, 255)] public SwooshSamplerState swooshSampler = SwooshSamplerState.线性;

            [DrawTicks]
            [DefaultValue(SwooshFactorStyle.每次开始时决定系数)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.11")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.12")]
            [BackgroundColor(48, 0, 255, 255)] public SwooshFactorStyle swooshFactorStyle = SwooshFactorStyle.每次开始时决定系数;
            #endregion

            #region 试验性设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D5")]
            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.31")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.32")]
            [BackgroundColor(32, 0, 255, 255)] public bool itemAdditive = false;

            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.33")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.34")]
            [BackgroundColor(16, 0, 255, 255)] public bool itemHighLight = false;
            #endregion
        }
        public class HeatMapConfigs
        {
            #region 样式设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D2")]
            [DrawTicks]
            [DefaultValue(HeatMapCreateStyle.函数生成)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.87")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.88")]
            [BackgroundColor(0, 13, 255, 255)] public HeatMapCreateStyle heatMapCreateStyle = HeatMapCreateStyle.函数生成;

            [DrawTicks]
            [DefaultValue(HeatMapFactorStyle.线性Linear)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.79")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.80")]
            [BackgroundColor(0, 26, 255, 255)] public HeatMapFactorStyle heatMapFactorStyle = HeatMapFactorStyle.线性Linear;
            #endregion

            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [DefaultValue(0.2f)]
            [Increment(0.01f)]
            [Range(-1f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.17")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.18")]
            [BackgroundColor(0, 39, 255, 255)] public float hueOffsetRange = 0.2f;

            [DefaultValue(0f)]
            [Increment(0.01f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.19")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.20")]
            [BackgroundColor(0, 42, 255, 255)] public float hueOffsetValue = 0f;

            [DefaultValue(5f)]
            [Increment(0.05f)]
            [Range(0f, 5f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.21")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.22")]
            [BackgroundColor(0, 64, 255, 255)] public float saturationScalar = 5f;

            [DefaultValue(0.2f)]
            [Increment(0.05f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.23")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.24")]
            [BackgroundColor(0, 77, 255, 255)] public float luminosityRange = 0.2f;

            [DefaultValue(3.1415f)]
            [Range(0, 6.283f)]
            [Increment(0.05f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.53")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.54")]
            [BackgroundColor(0, 90, 255, 255)] public float directOfHeatMap = MathHelper.Pi;

            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.81")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.82")]
            [BackgroundColor(0, 103, 255, 255)] public List<Color> heatMapColors = new List<Color>() { Color.Blue, Color.Green, Color.Yellow };
            #endregion

            #region 试验性设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D5")]
            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.67")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.68")]
            [BackgroundColor(0, 116, 255, 255)] public bool showHeatMap = false;
            #endregion
        }
        public class RenderConfigs
        {
            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [DefaultValue(0f)]
            [Increment(0.05f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.25")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.26")]
            [BackgroundColor(0, 153, 255, 255)] public float luminosityFactor = 0f;

            [Increment(0.05f)]
            [DefaultValue(0.25f)]
            [Range(-1f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.29")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.30")]
            [BackgroundColor(0, 178, 255, 255)] public float distortFactor = 0.05f;

            [Increment(0.05f)]
            [DefaultValue(1.5f)]
            [Range(0.5f, 3f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.65")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.66")]
            [BackgroundColor(0, 203, 255, 255)] public float distortSize = 1.5f;

            [DefaultValue(1)]
            [Range(1, 10)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.51")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.52")]
            [BackgroundColor(0, 228, 255, 255)] public int maxCount = 1;
            #endregion
        }
        public class OtherConfigs
        {
            #region 基本设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D1")]
            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.Config.Num1")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.Num2")]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool useWeaponDisplay = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.Config.Num3")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.Num4")]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool firstWeaponDisplay = true;

            [Increment(0.05f)]
            [Range(0.5f, 2f)]
            [DefaultValue(1f)]
            [Label("$Mods.CoolerItemVisualEffect.Config.11")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.12")]
            [Slider]
            [BackgroundColor(0, 255, 255, 255)] 
            public float weaponScale = 1f;

            [DefaultValue(HitBoxStyle.弹幕Projectile)]
            [DrawTicks]
            [Label("$Mods.CoolerItemVisualEffect.Config.33")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.34")]
            [BackgroundColor(0, 255, 255, 255)] 
            public HitBoxStyle hitBoxStyle = HitBoxStyle.弹幕Projectile;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.49")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.50")]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool DontChangeMyTitle = true;

            [Header("$Mods.CoolerItemVisualEffect.Config.24")]
            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.Config.25")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.26")]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool ItemDropEffectActive = false;

            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.Config.27")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.28")]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool ItemInventoryEffectActive = false;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.Config.29")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.30")]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool VanillaProjectileDrawModifyActive = true;

            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.Config.31")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.32")]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool TeleprotEffectActive = false;

            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.Config.35")]
            [Tooltip("$Mods.CoolerItemVisualEffect.Config.36")]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool CelesteMoveAnimation = false;
            #endregion

            #region 样式设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D2")]
            [JsonIgnore]
            int meanless;

            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.47")]

            [Label("[i:4]普通Normal")]
            [DefaultValue(true)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool NormalActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.普通Normal)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.普通Normal);
                }
            }
            [Label("[i:3852]飓风Hurricane")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool HurricaneActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.飓风Hurricane)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.飓风Hurricane);
                }
            }
            [Label("[i:426]巨大Huge")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool HugeActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.巨大Huge)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.巨大Huge);
                }
            }
            [Label("[i:4956]夸张Exaggerate")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool ExaggerateActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.夸张Exaggerate)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.夸张Exaggerate);
                }
            }
            [Label("[i:3768]明亮Bright")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool BrightActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.明亮Bright)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.明亮Bright);
                }
            }
            [Label("[i:1327]黑暗Dark")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool DarkActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.黑暗Dark)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.黑暗Dark);
                }
            }
            [Label("[i:3781]光滑Smooth")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool SmoothActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.光滑Smooth)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.光滑Smooth);
                }
            }
            [Label("[i:757]泰拉Terra_EspeciallyTerraBladeRecommended")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool TerraActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.泰拉Terra_EspeciallyTerraBladeRecommended)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.泰拉Terra_EspeciallyTerraBladeRecommended);
                }
            }
            [Label("[i:674]神圣Holy_EspeciallyTrueExcaliburRecommended")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool HolyActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.神圣Holy_EspeciallyTrueExcaliburRecommended)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.神圣Holy_EspeciallyTrueExcaliburRecommended);
                }
            }
            [Label("[i:675]永夜Evil_EspeciallyTrueNightsEdgeRecommended")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool EvilActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.永夜Evil_EspeciallyTrueNightsEdgeRecommended)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.永夜Evil_EspeciallyTrueNightsEdgeRecommended);
                }
            }
            [Label("[i:3827]旧日OldOnes_EspeciallyFlyingDragonRecommended")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool OldOnesActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.旧日OldOnes_EspeciallyFlyingDragonRecommended)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.旧日OldOnes_EspeciallyFlyingDragonRecommended);
                }
            }
            [Label("[i:2880]波涌Influx_EspeciallyInfluxWaverRecommended")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool InfluxActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.波涌Influx_EspeciallyInfluxWaverRecommended)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.波涌Influx_EspeciallyInfluxWaverRecommended);
                }
            }
            [Label("[i:389]黑白Grey")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool GreyActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.黑白Grey)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.黑白Grey);
                }
            }
            [Label("[i:1968]反相InverseHue")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool InverseActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.反相InverseHue)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.反相InverseHue);
                }
            }
            [Label("[i:3063]彩虹Rainbow")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool RainbowActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.彩虹Rainbow)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.彩虹Rainbow);
                }
            }
            [Label("[i:5005]超级彩虹UltraRainbow")]
            [DefaultValue(false)]
            [BackgroundColor(0, 255, 255, 255)] 
            public bool UltraRainbowActive
            {
                get { if (configurationSwoosh == null) return false; return configurationSwoosh.EqualValue(SetCSValue(new ConfigurationSwoosh(), PreInstallSwoosh.超级彩虹UltraRainbow)); }
                set
                {
                    if (configurationSwoosh != null && value)
                        SetCSValue(configurationSwoosh, PreInstallSwoosh.超级彩虹UltraRainbow);
                }
            }
            #endregion

            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [Increment(0.05f)]
            [DefaultValue(.75f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.85")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.86")]
            [BackgroundColor(0, 255, 255, 255)] 
            public float dustQuantity = .75f;
            #endregion

            [JsonIgnore]
            public ConfigurationSwoosh configurationSwoosh => ConfigurationSwoosh.ConfigSwooshInstance;
        }
        #endregion

        #region 其它函数
        public static ConfigurationSwoosh SetCSValue(ConfigurationSwoosh cs, PreInstallSwoosh preInstallSwoosh)
        {
            cs.coolerSwooshQuality = QualityType.极限ultra;
            //cs.toolsNoUseNewSwooshEffect = false;
            cs.isLighterDecider = 0.2f;
            cs.swooshColorType = SwooshColorType.热度图;
            cs.swooshSampler = SwooshSamplerState.线性;
            cs.swooshFactorStyle = SwooshFactorStyle.每次开始时决定系数;
            cs.swooshActionStyle = SwooshAction.左右横劈_后倾;
            cs.swooshSize = 1f;
            cs.hueOffsetRange = 0.2f;
            cs.hueOffsetValue = 0f;
            cs.saturationScalar = 5f;
            cs.luminosityRange = 0.2f;
            cs.luminosityFactor = 0.2f;
            //cs.swingAttackTime = 3f;
            cs.swingAttackTime = 4f;
            cs.distortFactor = 0.25f;
            cs.itemAdditive = false;
            cs.itemHighLight = false;
            cs.shake = 0f;
            cs.imageIndex = 7f;
            cs.checkAir = true;
            cs.gather = true;
            cs.allowZenith = true;
            cs.glowLight = 0f;
            cs.maxCount = 1;
            cs.directOfHeatMap = MathHelper.Pi;
            cs.swooshTimeLeft = 30;
            cs.onlyChangeSizeOfSwoosh = false;
            cs.fadeStyle = SwooshFadeStyle.全部Both;
            cs.growStyle = SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;
            cs.animateIndex = 3;
            cs.distortSize = 1.5f;
            cs.actionOffsetSize = true;
            cs.actionOffsetSpeed = true;
            cs.actionModifyEffect = true;
            cs.heatMapCreateStyle = HeatMapCreateStyle.函数生成;
            //cs.useWeaponDisplay = true;
            //cs.firstWeaponDisplay = true;
            //cs.weaponScale = 1f;
            //cs.hitBoxStyle = HitBoxStyle.弹幕Projectile;
            //cs.DontChangeMyTitle = true;
            //cs.ItemDropEffectActive = false;
            //cs.ItemInventoryEffectActive = false;
            //cs.VanillaProjectileDrawModifyActive = true;
            //cs.TeleprotEffectActive = false;
            //cs.CelesteMoveAnimation = false;
            switch (preInstallSwoosh)
            {
                //case PreInstallSwoosh.普通Normal: 
                //	{
                //		break; 
                //	}
                case PreInstallSwoosh.飓风Hurricane:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.shake = 0.3f;
                        cs.distortFactor = 1f;
                        cs.swooshSize = 1.5f;
                        cs.swooshActionStyle = SwooshAction.旋风劈;
                        cs.maxCount = 3;
                        cs.luminosityFactor = 0.4f;
                        break;
                    }
                case PreInstallSwoosh.巨大Huge:
                    {
                        cs.swooshSize = 3f;
                        break;
                    }
                case PreInstallSwoosh.夸张Exaggerate:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.swooshSize = 3f;
                        cs.distortFactor = 1f;
                        cs.shake = 1f;
                        cs.swooshFactorStyle = SwooshFactorStyle.系数中间插值;
                        cs.swooshActionStyle = SwooshAction.风暴灭却剑;
                        cs.maxCount = 3;
                        cs.luminosityFactor = 1f;
                        break;
                    }
                case PreInstallSwoosh.明亮Bright:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.isLighterDecider = 0;
                        cs.luminosityFactor = 0.6f;
                        cs.itemAdditive = true;
                        cs.maxCount = 2;
                        cs.glowLight = 1;
                        break;
                    }
                case PreInstallSwoosh.黑暗Dark:
                    {
                        cs.isLighterDecider = 1f;
                        cs.heatMapFactorStyle = HeatMapFactorStyle.二次Quadratic;
                        cs.luminosityRange = 1f;
                        break;
                    }
                case PreInstallSwoosh.光滑Smooth:
                    {
                        cs.swooshColorType = SwooshColorType.单向渐变;
                        cs.imageIndex = 0f;
                        break;
                    }
                case PreInstallSwoosh.泰拉Terra_EspeciallyTerraBladeRecommended:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.swooshColorType = SwooshColorType.热度图;
                        cs.swooshActionStyle = SwooshAction.风暴灭却剑;
                        cs.hueOffsetRange = -0.25f;
                        cs.hueOffsetValue = 0.15f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 4f;
                        cs.distortFactor = .4f;
                        cs.shake = 0.4f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        cs.heatMapFactorStyle = HeatMapFactorStyle.平方根SquareRoot;
                        break;
                    }
                case PreInstallSwoosh.神圣Holy_EspeciallyTrueExcaliburRecommended:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.swooshColorType = SwooshColorType.热度图;
                        cs.swooshActionStyle = SwooshAction.流雨断;
                        cs.hueOffsetRange = 0.25f;
                        cs.hueOffsetValue = 0.9f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 3f;
                        cs.distortFactor = .3f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        break;
                    }
                case PreInstallSwoosh.永夜Evil_EspeciallyTrueNightsEdgeRecommended:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.swooshColorType = SwooshColorType.热度图;
                        cs.swooshActionStyle = SwooshAction.旋风劈;
                        cs.hueOffsetRange = 0.25f;
                        cs.hueOffsetValue = 0.05f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 3f;
                        cs.distortFactor = .3f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        break;
                    }
                case PreInstallSwoosh.旧日OldOnes_EspeciallyFlyingDragonRecommended:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.swooshColorType = SwooshColorType.热度图;
                        cs.swooshActionStyle = SwooshAction.腾云斩;
                        cs.hueOffsetRange = 0.2f;
                        cs.hueOffsetValue = 0.95f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 3f;
                        cs.distortFactor = .5f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        cs.heatMapFactorStyle = HeatMapFactorStyle.平方根SquareRoot;
                        break;
                    }
                case PreInstallSwoosh.波涌Influx_EspeciallyInfluxWaverRecommended:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.swooshColorType = SwooshColorType.热度图;
                        cs.swooshActionStyle = SwooshAction.鸣雷刺;
                        cs.hueOffsetRange = 0.15f;
                        cs.hueOffsetValue = 0.05f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 3f;
                        cs.distortFactor = .25f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        cs.heatMapFactorStyle = HeatMapFactorStyle.二次Quadratic;

                        break;
                    }
                case PreInstallSwoosh.黑白Grey:
                    {
                        cs.swooshColorType = SwooshColorType.单向渐变;
                        cs.saturationScalar = 0;
                        break;
                    }
                case PreInstallSwoosh.反相InverseHue:
                    {
                        cs.swooshColorType = SwooshColorType.单向渐变;
                        cs.hueOffsetValue = 0.5f;
                        break;
                    }
                case PreInstallSwoosh.彩虹Rainbow:
                    {
                        cs.swooshColorType = SwooshColorType.单向渐变;
                        cs.hueOffsetRange = 1f;
                        break;
                    }
                case PreInstallSwoosh.超级彩虹UltraRainbow:
                    {
                        cs.coolerSwooshQuality = QualityType.极限ultra;
                        cs.swooshColorType = SwooshColorType.单向渐变;
                        cs.swooshActionStyle = SwooshAction.风暴灭却剑;
                        cs.hueOffsetRange = 1f;
                        cs.hueOffsetValue = 0f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 2f;
                        cs.distortFactor = 0.75f;
                        cs.shake = 0.1f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        cs.directOfHeatMap = MathHelper.Pi * 1.5f;
                        break;
                    }
            }
            return cs;
        }
        public static void Save(ModConfig config)
        {
            var ModConfigPath = Path.Combine(Main.SavePath, "ModConfigs");
            Directory.CreateDirectory(ModConfigPath);
            string filename = config.Mod.Name + "_" + config.Name + ".json";
            string path = Path.Combine(ModConfigPath, filename);
            string json = JsonConvert.SerializeObject(config, ConfigManager.serializerSettings);
            File.WriteAllText(path, json);
        }
        public override void OnChanged()
        {
            CoolerItemVisualEffect.WhenConfigSwooshChange();
            if (Main.netMode == NetmodeID.MultiplayerClient) SendData();
        }
        //public ConfigurationSwoosh()
        //{
        //    otherConfigs.configurationSwoosh = this;
        //}
        #endregion
    }
}