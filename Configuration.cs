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
    [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label_2")]
    public class ConfigurationNormal : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public ConfigurationSwoosh_Advanced SetCSValue(ConfigurationSwoosh_Advanced cs)
        {
            cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
            cs.toolsNoUseNewSwooshEffect = false;
            cs.isLighterDecider = 0.2f;
            cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.热度图;
            cs.swooshSampler = ConfigurationSwoosh_Advanced.SwooshSamplerState.线性;
            cs.swooshFactorStyle = ConfigurationSwoosh_Advanced.SwooshFactorStyle.每次开始时决定系数;
            cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.左右横劈_后倾;
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
            cs.itemHighLight = true;
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
            cs.fadeStyle = ConfigurationSwoosh_Advanced.SwooshFadeStyle.全部Both;
            cs.growStyle = ConfigurationSwoosh_Advanced.SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;
            cs.animateIndex = 3;
            cs.distortSize = 1.5f;
            cs.actionOffsetSize = true;
            cs.actionOffsetSpeed = true;
            cs.actionModifyEffect = true;
            cs.heatMapCreateStyle = ConfigurationSwoosh_Advanced.HeatMapCreateStyle.函数生成;
            switch (preInstallSwoosh)
            {
                //case PreInstallSwoosh.普通Normal: 
                //	{
                //		break; 
                //	}
                case PreInstallSwoosh.飓风Hurricane:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.shake = 0.3f;
                        cs.distortFactor = 1f;
                        cs.swooshSize = 1.5f;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.旋风劈;
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
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshSize = 3f;
                        cs.distortFactor = 1f;
                        cs.shake = 1f;
                        cs.swooshFactorStyle = ConfigurationSwoosh_Advanced.SwooshFactorStyle.系数中间插值;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.风暴灭却剑;
                        cs.maxCount = 3;
                        cs.luminosityFactor = 1f;
                        break;
                    }
                case PreInstallSwoosh.明亮Bright:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.isLighterDecider = 0;
                        cs.luminosityFactor = 0.6f;
                        cs.itemAdditive = true;   
                        cs.maxCount = 2;
                        cs.glowLight = 1;
                        break;
                    }
                case PreInstallSwoosh.怪异Strange:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.热度图;
                        break;
                    }
                case PreInstallSwoosh.光滑Smooth:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.单向渐变;
                        cs.imageIndex = 0f;
                        break;
                    }
                case PreInstallSwoosh.泰拉Terra_EspeciallyTerraBladeRecommended:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.风暴灭却剑;
                        cs.hueOffsetRange = -0.25f;
                        cs.hueOffsetValue = 0.15f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 4f;
                        cs.distortFactor = .4f;
                        cs.shake = 0.4f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        cs.heatMapFactorStyle = ConfigurationSwoosh_Advanced.HeatMapFactorStyle.平方根SquareRoot;
                        break;
                    }
                case PreInstallSwoosh.神圣Holy_EspeciallyTrueExcaliburRecommended:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.流雨断;
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
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.旋风劈;
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
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.腾云斩;
                        cs.hueOffsetRange = 0.2f;
                        cs.hueOffsetValue = 0.95f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 3f;
                        cs.distortFactor = .5f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        cs.heatMapFactorStyle = ConfigurationSwoosh_Advanced.HeatMapFactorStyle.平方根SquareRoot;
                        break;
                    }
                case PreInstallSwoosh.波涌Influx_EspeciallyInfluxWaverRecommended:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.鸣雷刺;
                        cs.hueOffsetRange = 0.15f;
                        cs.hueOffsetValue = 0.05f;
                        cs.luminosityFactor = 1f;
                        cs.swingAttackTime = 3f;
                        cs.distortFactor = .25f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        cs.heatMapFactorStyle = ConfigurationSwoosh_Advanced.HeatMapFactorStyle.二次Quadratic;

                        break;
                    }
                case PreInstallSwoosh.黑白Grey:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.单向渐变;
                        cs.saturationScalar = 0;
                        break;
                    }
                case PreInstallSwoosh.反相InverseHue:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.单向渐变;
                        cs.hueOffsetValue = 0.5f;
                        break;
                    }
                case PreInstallSwoosh.彩虹Rainbow:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.单向渐变;
                        cs.hueOffsetRange = 1f;
                        break;
                    }
                case PreInstallSwoosh.超级彩虹UltraRainbow:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.单向渐变;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.风暴灭却剑;
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
        public override void OnChanged()
        {
            if (preInstallSwoosh == PreInstallSwoosh.自定义UserDefined) return;
            var cs = ConfigurationSwoosh_Advanced.ConfigSwooshInstance;
            if (cs == null) return;
            SetCSValue(cs);
            ConfigurationSwoosh_Advanced.Save(cs);
            CoolerItemVisualEffect.WhenConfigSwooshChange();
            if (Main.netMode == NetmodeID.MultiplayerClient) ConfigurationSwoosh_Advanced.ConfigSwooshInstance.SendData();
        }
        public enum PreInstallSwoosh
        {
            普通Normal,
            飓风Hurricane,
            巨大Huge,
            夸张Exaggerate,
            明亮Bright,
            怪异Strange,
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
            剑气UltraSwoosh
        }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.Config.Num1")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.Num2")]
        public bool useWeaponDisplay { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.Config.Num3")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.Num4")]
        public bool firstWeaponDisplay { get; set; }

        [Increment(0.05f)]
        [Range(0.5f, 2f)]
        [DefaultValue(1f)]
        [Label("$Mods.CoolerItemVisualEffect.Config.11")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.12")]
        [Slider]
        public float weaponScale { get; set; }

        [DefaultValue(HitBoxStyle.剑气UltraSwoosh)]
        [DrawTicks]
        [Label("$Mods.CoolerItemVisualEffect.Config.33")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.34")]
        public HitBoxStyle hitBoxStyle { get; set; }

        [JsonIgnore]
        public bool UseHitbox { get; set; }
        //[Range(1, 20)]
        //[Increment(1)]
        //[DefaultValue(4)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigurationServer.AttackablesName")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigurationServer.AttackablesTooltip")]
        //[Slider]
        //public int ItemAttackCD;

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.49")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.50")]
        public bool DontChangeMyTitle { get; set; }

        [DefaultValue(PreInstallSwoosh.普通Normal)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.47")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.48")]
        [DrawTicks]
        public PreInstallSwoosh preInstallSwoosh { get; set; }

        [Header("$Mods.CoolerItemVisualEffect.Config.24")]
        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.Config.25")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.26")]
        public bool ItemDropEffectActive { get; set; }

        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.Config.27")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.28")]
        public bool ItemInventoryEffectActive { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.Config.29")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.30")]
        public bool VanillaProjectileDrawModifyActive { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.Config.31")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.32")]
        public bool TeleprotEffectActive { get; set; }
        public static ConfigurationNormal instance => ModContent.GetInstance<ConfigurationNormal>();

    }

    [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label")]
    public class ConfigurationSwoosh_Advanced : ModConfig
    {
        public static void Save(ModConfig config)
        {
            var ModConfigPath = Path.Combine(Main.SavePath, "ModConfigs");
            Directory.CreateDirectory(ModConfigPath);
            string filename = config.Mod.Name + "_" + config.Name + ".json";
            string path = Path.Combine(ModConfigPath, filename);
            string json = JsonConvert.SerializeObject(config, ConfigManager.serializerSettings);
            File.WriteAllText(path, json);
        }
        public void SendData(int? whoami = null, int ignoreCilent = -1, int toCilent = -1, bool enter = false) //ModPacket packet, int? playerIndex = null
        {
            ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
            packet.Write((byte)(enter ? HandleNetwork.MessageType.EnterWorld : HandleNetwork.MessageType.Configs));
            //packet.Write(playerIndex ?? Main.myPlayer);
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
            //packet.Write
            packet.Send(toCilent, ignoreCilent);
            //if (whoami != -1)
            //    Main.NewText("由 " + Main.player[whoami] + "发射数据");
            //else Main.NewText("发射数据!");
        }
        public static void SetData(BinaryReader reader, int whoami)
        {
            if (whoami < 0 || whoami > 255) throw new System.Exception("我抄，超范围辣");
            var config = Main.player[whoami].GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh;
            //if (config == null) Main.player[whoami].GetModPlayer<WeaponDisplayPlayer>().configurationSwoosh = new ConfigurationSwoosh();
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
            //Main.NewText("向 " + Main.player[whoami] + "设置数据");

        }
        //[JsonIgnore]
        //public static int MagicConfigCounter;
        bool EqualValue(ConfigurationSwoosh_Advanced config)
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
                heatMapCreateStyle == config.heatMapCreateStyle;
        }
        public override void OnChanged()
        {
            if (!EqualValue(ConfigurationNormal.instance.SetCSValue(new ConfigurationSwoosh_Advanced())))
            {
                ConfigurationNormal.instance.preInstallSwoosh = ConfigurationNormal.PreInstallSwoosh.自定义UserDefined;
                Save(ConfigurationNormal.instance);
            }
            CoolerItemVisualEffect.WhenConfigSwooshChange();
            //MagicConfigCounter++;
            if (Main.netMode == NetmodeID.MultiplayerClient) SendData();
        }
        public override void OnLoaded()
        {
            //if (Main.netMode == NetmodeID.MultiplayerClient) SendData();
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static ConfigurationSwoosh_Advanced ConfigSwooshInstance => ModContent.GetInstance<ConfigurationSwoosh_Advanced>();

        //TODO Config

        //[DefaultValue(true)]
        //[Label("更帅的拔刀")]
        //[Tooltip("你厌倦了原版近战的挥动方式了吗？")]
        //[BackgroundColor(0, 0, 255, 127)]
        //public bool CoolerSwooshActive;
        //[DrawTicks]
        //[DefaultValue(QualityType.极限ultra)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.1")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.2")]
        //[BackgroundColor(0, 0, 255, 127)]
        [JsonIgnore]
        public QualityType coolerSwooshQuality
        {
            get => meleeSwooshConfigs.coolerSwooshQuality;
            set => meleeSwooshConfigs.coolerSwooshQuality = value;
        }

        [JsonIgnore]
        public bool CoolerSwooshActive => (byte)coolerSwooshQuality > 0;

        //[DefaultValue(false)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.3")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.4")]
        [JsonIgnore]
        //[BackgroundColor(15, 0, 255, 127)]
        public bool toolsNoUseNewSwooshEffect
        {
            get => meleeSwooshConfigs.toolsNoUseNewSwooshEffect;
            set => meleeSwooshConfigs.toolsNoUseNewSwooshEffect = value;
        }
        //[Increment(0.05f)]
        //[Range(0f, 1f)]
        //[DefaultValue(0.2f)]
        //[Label("挥砍效果亮度")]
        //[Tooltip("亮度大于这个值就会使用高亮式挥砍")]
        //[Slider]
        //[BackgroundColor(15, 0, 255, 127)]
        //public float IsLighterDecider{ get; set; }
        //[Increment(0.05f)]
        //[Range(0f, 1f)]
        //[DefaultValue(0.2f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.5")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.6")]
        [JsonIgnore]
        //[Slider]
        //[BackgroundColor(30, 0, 255, 127)]
        public float isLighterDecider
        {
            get => drawConfigs.isLighterDecider;
            set => drawConfigs.isLighterDecider = value;
        }

        //[DefaultValue(true)]
        //[Label("挥砍效果颜色")]
        //[Tooltip("是, 以开启武器贴图对角线颜色关联挥砍效果颜色, 否, 使用单色. 默认为是")]
        //public bool UseItemTexForSwoosh{ get; set; }
        //[DrawTicks]
        //[DefaultValue(SwooshColorType.色调处理与对角线混合)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.7")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.8")]
        [JsonIgnore]
        //[BackgroundColor(45, 0, 255, 127)]
        public SwooshColorType swooshColorType
        {
            get => drawConfigs.swooshColorType;
            set => drawConfigs.swooshColorType = value;
        }

        //[DrawTicks]
        //[DefaultValue(SwooshSamplerState.线性)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.9")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.10")]
        [JsonIgnore]
        //[BackgroundColor(60, 0, 255, 127)]
        public SwooshSamplerState swooshSampler
        {
            get => drawConfigs.swooshSampler;
            set => drawConfigs.swooshSampler = value;
        }

        //[DrawTicks]
        //[DefaultValue(SwooshFactorStyle.每次开始时决定系数)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.11")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.12")]
        [JsonIgnore]
        //[BackgroundColor(75, 0, 255, 127)]
        public SwooshFactorStyle swooshFactorStyle
        {
            get => drawConfigs.swooshFactorStyle;
            set => drawConfigs.swooshFactorStyle = value;
        }

        //[DrawTicks]
        //[DefaultValue(SwooshAction.左右横劈_后倾)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.13")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.14")]
        [JsonIgnore]
        //[BackgroundColor(60, 0, 255, 127)]
        public SwooshAction swooshActionStyle
        {
            get => meleeSwooshConfigs.swooshActionStyle;
            set => meleeSwooshConfigs.swooshActionStyle = value;
        }

        //[Increment(0.05f)]
        //[DefaultValue(1f)]
        //[Range(0.5f, 3f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.15")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.16")]
        [JsonIgnore]
        //[BackgroundColor(105, 0, 255, 127)]
        public float swooshSize
        {
            get => meleeSwooshConfigs.swooshSize;
            set => meleeSwooshConfigs.swooshSize = value;
        }

        //[DefaultValue(0.2f)]
        //[Increment(0.01f)]
        //[Range(-1f, 1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.17")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.18")]
        [JsonIgnore]
        //[BackgroundColor(120, 0, 255, 127)]
        public float hueOffsetRange
        {
            get => heatMapConfigs.hueOffsetRange;
            set => heatMapConfigs.hueOffsetRange = value;
        }


        //[DefaultValue(0f)]
        //[Increment(0.01f)]
        //[Range(0f, 1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.19")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.20")]
        [JsonIgnore]
        //[BackgroundColor(135, 0, 255, 127)]
        public float hueOffsetValue
        {
            get => heatMapConfigs.hueOffsetValue;
            set => heatMapConfigs.hueOffsetValue = value;
        }

        //[DefaultValue(5f)]
        //[Increment(0.05f)]
        //[Range(0f, 5f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.21")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.22")]
        [JsonIgnore]
        //[BackgroundColor(150, 0, 255, 127)]
        public float saturationScalar
        {
            get => heatMapConfigs.saturationScalar;
            set => heatMapConfigs.saturationScalar = value;
        }

        //[DefaultValue(0.2f)]
        //[Increment(0.05f)]
        //[Range(0f, 1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.23")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.24")]
        [JsonIgnore]
        //[BackgroundColor(165, 0, 255, 127)]
        public float luminosityRange
        {
            get => heatMapConfigs.luminosityRange;
            set => heatMapConfigs.luminosityRange = value;
        }

        //[DefaultValue(0f)]
        //[Increment(0.05f)]
        //[Range(0f, 1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.25")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.26")]
        [JsonIgnore]
        //[BackgroundColor(180, 0, 255, 127)]
        public float luminosityFactor
        {
            get => renderConfigs.luminosityFactor;
            set => renderConfigs.luminosityFactor = value;
        }

        //[Increment(0.05f)]
        //[DefaultValue(3f)]
        //[Range(1f, 3f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.27")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.28")]
        [JsonIgnore]
        //[BackgroundColor(195, 0, 255, 127)]
        public float swingAttackTime
        {
            get => meleeSwooshConfigs.swingAttackTime;
            set => meleeSwooshConfigs.swingAttackTime = value;
        }

        //[Increment(0.05f)]
        //[DefaultValue(0.25f)]
        //[Range(-1f, 1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.29")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.30")]
        [JsonIgnore]
        //[BackgroundColor(210, 0, 255, 127)]
        public float distortFactor
        {
            get => renderConfigs.distortFactor;
            set => renderConfigs.distortFactor = value;
        }

        //[DefaultValue(false)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.31")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.32")]
        //[BackgroundColor(225, 0, 255, 127)]
        //[BackgroundColor(0,0,0,0)]
        [JsonIgnore]
        //[ColorHSLSlider(true)]
        public bool itemAdditive
        {
            get => drawConfigs.itemAdditive;
            set => drawConfigs.itemAdditive = value;
        }

        //[DefaultValue(true)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.33")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.34")]
        [JsonIgnore]
        //[BackgroundColor(240, 0, 255, 127)]
        public bool itemHighLight
        {
            get => drawConfigs.itemHighLight;
            set => drawConfigs.itemHighLight = value;
        }

        //[Increment(0.05f)]
        //[DefaultValue(0f)]
        //[Range(0f, 1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.35")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.36")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 255, 127)]
        public float shake
        {
            get => meleeSwooshConfigs.shake;
            set => meleeSwooshConfigs.shake = value;
        }

        //[Increment(1f)]
        //[DefaultValue(7f)]
        //[Range(0, 11f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.37")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.38")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 240, 127)]
        public float imageIndex
        {
            get => meleeSwooshConfigs.imageIndex;
            set => meleeSwooshConfigs.imageIndex = value;
        }
        [JsonIgnore]
        public int ImageIndex => (int)MathHelper.Clamp(ConfigSwooshInstance.imageIndex, 0, 11);

        //[DefaultValue(true)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.39")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.40")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 225, 127)]
        public bool checkAir
        {
            get => meleeSwooshConfigs.checkAir;
            set => meleeSwooshConfigs.checkAir = value;
        }

        //[DefaultValue(true)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.41")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.42")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 210, 127)]
        public bool gather
        {
            get => meleeSwooshConfigs.gather;
            set => meleeSwooshConfigs.gather = value;
        }

        //[DefaultValue(true)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.43")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.44")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 195, 127)]
        public bool allowZenith
        {
            get => meleeSwooshConfigs.allowZenith;
            set => meleeSwooshConfigs.allowZenith = value;
        }

        //[Increment(0.05f)]
        //[DefaultValue(0f)]
        //[Range(0f, 1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.45")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.46")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 180, 127)]
        public float glowLight
        {
            get => meleeSwooshConfigs.glowLight;
            set => meleeSwooshConfigs.glowLight = value;
        }

        //[DefaultValue(1)]
        //[Range(1, 10)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.51")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.52")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 165, 127)]
        public int maxCount
        {
            get => renderConfigs.maxCount;
            set => renderConfigs.maxCount = value;
        }

        //[DefaultValue(3.1415f)]
        //[Range(0, 6.283f)]
        //[Increment(0.05f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.53")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.54")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 150, 127)]
        public float directOfHeatMap
        {
            get => heatMapConfigs.directOfHeatMap;
            set => heatMapConfigs.directOfHeatMap = value;
        }

        //[DefaultValue(30f)]
        //[Range(0, 60f)]
        //[Increment(1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.55")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.56")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 135, 127)]
        public float swooshTimeLeft
        {
            get => meleeSwooshConfigs.swooshTimeLeft;
            set => meleeSwooshConfigs.swooshTimeLeft = value;
        }

        //[DefaultValue(false)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.57")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.58")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 120, 127)]
        public bool onlyChangeSizeOfSwoosh
        {
            get => meleeSwooshConfigs.onlyChangeSizeOfSwoosh;
            set => meleeSwooshConfigs.onlyChangeSizeOfSwoosh = value;
        }

        //[DrawTicks]
        //[DefaultValue(SwooshFadeStyle.全部Both)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.59")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.60")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 105, 127)]
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

        //[DrawTicks]
        //[DefaultValue(SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.61")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.62")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 90, 127)]
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

        //[Increment(1f)]
        //[DefaultValue(3f)]
        //[Range(0, 5f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.63")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.64")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 75, 127)]
        public float animateIndex
        {
            get => meleeSwooshConfigs.animateIndex;
            set => meleeSwooshConfigs.animateIndex = value;
        }
        [JsonIgnore]
        public int AnimateIndex => (int)MathHelper.Clamp(ConfigSwooshInstance.animateIndex, 0, 5);

        //[Increment(0.05f)]
        //[DefaultValue(1.5f)]
        //[Range(0.5f, 3f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.65")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.66")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 60, 127)]
        public float distortSize
        {
            get => renderConfigs.distortSize;
            set => renderConfigs.distortSize = value;
        }

        //[DefaultValue(false)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.67")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.68")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 45, 127)]
        public bool showHeatMap
        {
            get => heatMapConfigs.showHeatMap;
            set => heatMapConfigs.showHeatMap = value;
        }

        //[DefaultValue(true)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.69")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.70")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 30, 127)]
        public bool actionOffsetSize
        {
            get => meleeSwooshConfigs.actionOffsetSize;
            set => meleeSwooshConfigs.actionOffsetSize = value;
        }

        //[DefaultValue(true)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.73")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.74")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 15, 127)]
        public bool actionOffsetSpeed
        {
            get => meleeSwooshConfigs.actionOffsetSpeed;
            set => meleeSwooshConfigs.actionOffsetSpeed = value;
        }

        //[DefaultValue(true)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.75")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.76")]
        [JsonIgnore]
        //[BackgroundColor(255, 0, 0, 127)]
        public bool actionModifyEffect
        {
            get => meleeSwooshConfigs.actionModifyEffect;
            set => meleeSwooshConfigs.actionModifyEffect = value;
        }

        //[DefaultValue(false)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.77")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.78")]
        [JsonIgnore]
        //[BackgroundColor(255, 15, 0, 127)]
        public bool ignoreDamageType
        {
            get => meleeSwooshConfigs.ignoreDamageType;
            set => meleeSwooshConfigs.ignoreDamageType = value;
        }

        //[DrawTicks]
        //[DefaultValue(HeatMapFactorStyle.线性Linear)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.79")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.80")]
        //[BackgroundColor(255, 30, 0, 127)]
        [JsonIgnore]
        public HeatMapFactorStyle heatMapFactorStyle
        {
            get => heatMapConfigs.heatMapFactorStyle;
            set => heatMapConfigs.heatMapFactorStyle = value;
        }

        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.81")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.82")]
        //[BackgroundColor(255, 45, 0, 127)]
        [JsonIgnore]
        public List<Color> heatMapColors
        {
            get => heatMapConfigs.heatMapColors;
            set => heatMapConfigs.heatMapColors = value;
        }

        //[Increment(0.05f)]
        //[DefaultValue(1.5f)]
        //[Range(0f, 3f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.83")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.84")]
        //[BackgroundColor(255, 60, 0, 127)]
        [JsonIgnore]
        public float alphaFactor
        {
            get => drawConfigs.alphaFactor;
            set => drawConfigs.alphaFactor = value;
        }

        //[Increment(0.05f)]
        //[DefaultValue(.75f)]
        //[Range(0f, 1f)]
        //[Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.85")]
        //[Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.86")]
        //[BackgroundColor(255, 75, 0, 127)]
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

            //摇摆重击
        }
        public enum SwooshFactorStyle : byte
        {
            每次开始时决定系数,
            系数中间插值
        }
        public enum SwooshColorType : byte
        {
            //加权平均,
            //加权平均_饱和与色调处理,
            //函数生成热度图,
            //武器贴图对角线,
            //色调处理与对角线混合,
            //贴图生成热度图,
            //指定热度图
            加权平均,
            单向渐变,
            武器贴图对角线,
            单向渐变与对角线混合,
            热度图,
            //贴图生成热度图,
            //指定热度图
        }
        public enum QualityType : byte
        {
            关off,
            低low,
            中medium,
            高high,
            极限ultra
        }
        public enum SwooshFadeStyle:byte
        {
            逐渐透明GraduallyTransparent,
            角度收缩CloseTheAngle,
            逐渐黯淡GraduallyFade,
            全部Both
        }
        public enum SwooshGrowStyle:byte
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
        public class MeleeSwooshConfigs
        {
            #region 基本设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D1")]
            [DrawTicks]
            [DefaultValue(QualityType.极限ultra)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.1")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.2")]
            //[BackgroundColor(0, 248, 255, 255)]//127
            [BackgroundColor(248, 0, 255, 255)]//127
            public QualityType coolerSwooshQuality = QualityType.极限ultra;

            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.3")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.4")]
            //[BackgroundColor(0, 242, 255, 255)]//127
            [BackgroundColor(242, 0, 255, 255)]//127
            public bool toolsNoUseNewSwooshEffect = false;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.43")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.44")]
            //[BackgroundColor(0, 236, 255, 255)]//127
            [BackgroundColor(236, 0, 255, 255)]//127
            public bool allowZenith = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.69")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.70")]
            //[BackgroundColor(0, 230, 255, 255)]//127
            [BackgroundColor(230, 0, 255, 255)]//127
            public bool actionOffsetSize = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.73")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.74")]
            //[BackgroundColor(0, 224, 255, 255)]//127
            [BackgroundColor(224, 0, 255, 255)]//127
            public bool actionOffsetSpeed = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.75")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.76")]
            //[BackgroundColor(0, 218, 255, 255)]//127
            [BackgroundColor(218, 0, 255, 255)]//127
            public bool actionModifyEffect = true;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.41")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.42")]
            //[BackgroundColor(0, 212, 255, 255)]//127
            [BackgroundColor(212, 0, 255, 255)]//127
            public bool gather = true;
            #endregion

            #region 样式设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D2")]
            [DrawTicks]
            [DefaultValue(SwooshAction.左右横劈_后倾)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.13")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.14")]
            //[BackgroundColor(0, 206, 255, 255)]//127
            [BackgroundColor(206, 0, 255, 255)]//127
            public SwooshAction swooshActionStyle = SwooshAction.左右横劈_后倾;

            [Increment(1f)]
            [DefaultValue(7f)]
            [Range(0, 11f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.37")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.38")]
            //[BackgroundColor(0, 200, 255, 255)]//127
            [BackgroundColor(200, 0, 255, 255)]//127
            public float imageIndex = 7;

            [Increment(1f)]
            [DefaultValue(3f)]
            [Range(0, 5f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.63")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.64")]
            //[BackgroundColor(0, 194, 255, 255)]//127
            [BackgroundColor(194, 0, 255, 255)]//127
            public float animateIndex = 3;

            [DrawTicks]
            [DefaultValue(SwooshFadeStyle.全部Both)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.59")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.60")]
            //[BackgroundColor(0, 188, 255, 255)]//127
            [BackgroundColor(188, 0, 255, 255)]//127
            public SwooshFadeStyle fadeStyle = SwooshFadeStyle.全部Both;

            [DrawTicks]
            [DefaultValue(SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.61")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.62")]
            //[BackgroundColor(0, 182, 255, 255)]//127
            [BackgroundColor(182, 0, 255, 255)]//127
            public SwooshGrowStyle growStyle = SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;
            #endregion

            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [Increment(0.05f)]
            [DefaultValue(1f)]
            [Range(0.5f, 3f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.15")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.16")]
            //[BackgroundColor(0, 176, 255, 255)]//127
            [BackgroundColor(176, 0, 255, 255)]//127
            public float swooshSize = 1f;

            [DefaultValue(30f)]
            [Range(0, 60f)]
            [Increment(1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.55")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.56")]
            //[BackgroundColor(0, 170, 255, 255)]//127
            [BackgroundColor(170, 0, 255, 255)]//127
            public float swooshTimeLeft = 30f;

            [Increment(0.05f)]
            [DefaultValue(0f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.35")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.36")]
            //[BackgroundColor(0, 164, 255, 255)]//127
            [BackgroundColor(164, 0, 255, 255)]//127
            public float shake = 0f;
            #endregion

            #region 细节设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D4")]
            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.39")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.40")]
            //[BackgroundColor(0, 158, 255, 255)]//127
            [BackgroundColor(158, 0, 255, 255)]//127
            public bool checkAir = true;

            //[Increment(0.05f)]
            //[DefaultValue(3f)]
            //[Range(1f, 3f)]
            [Increment(1f)]
            [DefaultValue(4f)]
            [Range(2f, 10f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.89")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.90")]
            //[BackgroundColor(0, 152, 255, 255)]//127
            [BackgroundColor(152, 0, 255, 255)]//127
            public float swingAttackTime = 3f;

            [Increment(0.05f)]
            [DefaultValue(0f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.45")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.46")]
            //[BackgroundColor(0, 146, 255, 255)]//127
            [BackgroundColor(146, 0, 255, 255)]//127
            public float glowLight = 0f;
            #endregion

            #region 试验性设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D5")]
            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.57")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.58")]
            //[BackgroundColor(0, 140, 255, 255)]//127
            [BackgroundColor(140, 0, 255, 255)]//127
            public bool onlyChangeSizeOfSwoosh = false;

            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.77")]
            //[BackgroundColor(0, 134, 255, 255)]//127
            [BackgroundColor(134, 0, 255, 255)]//127
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
            //[BackgroundColor(0, 112, 255, 255)]//127
            [BackgroundColor(112, 0, 255, 255)]//127
            public SwooshColorType swooshColorType = SwooshColorType.热度图;
            #endregion

            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [Increment(0.05f)]
            [DefaultValue(1.5f)]
            [Range(0f, 3f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.83")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.84")]
            //[BackgroundColor(0, 96, 255, 255)]//127
            [BackgroundColor(96, 0, 255, 255)]//127
            public float alphaFactor = 1.5f;

            [Increment(0.05f)]
            [Range(0f, 1f)]
            [DefaultValue(0.2f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.5")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.6")]
            //[BackgroundColor(0, 80, 255, 255)]//127
            [BackgroundColor(80, 0, 255, 255)]//127
            public float isLighterDecider = 0.2f;
            #endregion

            #region 细节设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D4")]
            [DrawTicks]
            [DefaultValue(SwooshSamplerState.线性)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.9")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.10")]
            //[BackgroundColor(0, 64, 255, 255)]//127
            [BackgroundColor(64, 0, 255, 255)]//127
            public SwooshSamplerState swooshSampler = SwooshSamplerState.线性;

            [DrawTicks]
            [DefaultValue(SwooshFactorStyle.每次开始时决定系数)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.11")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.12")]
            //[BackgroundColor(0, 48, 255, 255)]//127
            [BackgroundColor(48, 0, 255, 255)]//127
            public SwooshFactorStyle swooshFactorStyle = SwooshFactorStyle.每次开始时决定系数;
            #endregion

            #region 试验性设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D5")]
            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.31")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.32")]
            //[BackgroundColor(0, 32, 255, 255)]//127
            [BackgroundColor(32, 0, 255, 255)]//127
            public bool itemAdditive = false;

            [DefaultValue(true)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.33")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.34")]
            //[BackgroundColor(0, 16, 255, 255)]//127
            [BackgroundColor(16, 0, 255, 255)]//127
            public bool itemHighLight = true;
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
            //[BackgroundColor(13, 0, 255, 255)]//127
            [BackgroundColor(0, 13, 255, 255)]//127
            public HeatMapCreateStyle heatMapCreateStyle = HeatMapCreateStyle.函数生成;

            [DrawTicks]
            [DefaultValue(HeatMapFactorStyle.线性Linear)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.79")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.80")]
            //[BackgroundColor(26, 0, 255, 255)]//127
            [BackgroundColor(0, 26, 255, 255)]//127
            public HeatMapFactorStyle heatMapFactorStyle = HeatMapFactorStyle.线性Linear;
            #endregion

            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [DefaultValue(0.2f)]
            [Increment(0.01f)]
            [Range(-1f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.17")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.18")]
            //[BackgroundColor(39, 0, 255, 255)]//127
            [BackgroundColor(0, 39, 255, 255)]//127
            public float hueOffsetRange = 0.2f;

            [DefaultValue(0f)]
            [Increment(0.01f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.19")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.20")]
            //[BackgroundColor(42, 0, 255, 255)]//127
            [BackgroundColor(0, 42, 255, 255)]//127
            public float hueOffsetValue = 0f;

            [DefaultValue(5f)]
            [Increment(0.05f)]
            [Range(0f, 5f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.21")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.22")]
            //[BackgroundColor(64, 0, 255, 255)]//127
            [BackgroundColor(0, 64, 255, 255)]//127
            public float saturationScalar = 5f;

            [DefaultValue(0.2f)]
            [Increment(0.05f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.23")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.24")]
            //[BackgroundColor(77, 0, 255, 255)]//127
            [BackgroundColor(0, 77, 255, 255)]//127
            public float luminosityRange = 0.2f;

            [DefaultValue(3.1415f)]
            [Range(0, 6.283f)]
            [Increment(0.05f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.53")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.54")]
            //[BackgroundColor(90, 0, 255, 255)]//127
            [BackgroundColor(0, 90, 255, 255)]//127
            public float directOfHeatMap = MathHelper.Pi;

            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.81")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.82")]
            //[BackgroundColor(103, 0, 255, 255)]//127
            [BackgroundColor(0, 103, 255, 255)]//127
            public List<Color> heatMapColors = new List<Color>() { Color.Blue, Color.Green, Color.Yellow };
            #endregion

            #region 试验性设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D5")]
            [DefaultValue(false)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.67")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.68")]
            //[BackgroundColor(116, 0, 255, 255)]//127
            [BackgroundColor(0, 116, 255, 255)]//127
            public bool showHeatMap = false;
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
            //[BackgroundColor(153, 0, 255, 255)]//127
            [BackgroundColor(0, 153, 255, 255)]//127
            public float luminosityFactor = 0f;

            [Increment(0.05f)]
            [DefaultValue(0.25f)]
            [Range(-1f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.29")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.30")]
            //[BackgroundColor(178, 0, 255, 255)]//127
            [BackgroundColor(0, 178, 255, 255)]//127
            public float distortFactor = 0.05f;

            [Increment(0.05f)]
            [DefaultValue(1.5f)]
            [Range(0.5f, 3f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.65")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.66")]
            //[BackgroundColor(203, 0, 255, 255)]//127
            [BackgroundColor(0, 203, 255, 255)]//127
            public float distortSize = 1.5f;

            [DefaultValue(1)]
            [Range(1, 10)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.51")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.52")]
            //[BackgroundColor(228, 0, 255, 255)]//127
            [BackgroundColor(0, 228, 255, 255)]//127
            public int maxCount = 1;
            #endregion
        }
        public class OtherConfigs
        {
            #region 参数设置
            [Header("$Mods.CoolerItemVisualEffect.ConfigSwoosh.D3")]
            [Increment(0.05f)]
            [DefaultValue(.75f)]
            [Range(0f, 1f)]
            [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.85")]
            [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.86")]
            //[BackgroundColor(255, 0, 255, 255)]//127
            [BackgroundColor(0, 255, 255, 255)]//127
            public float dustQuantity = .75f;
            #endregion
        }
    }
    //[Label("$Mods.CoolerItemVisualEffect.Config.Label")]
    //public class Configuration : ModConfig
    //{
    //    public override ConfigScope Mode => ConfigScope.ClientSide;

    //    [DefaultValue(true)]
    //    [Label("$Mods.CoolerItemVisualEffect.Config.Num1")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.Config.Num2")]
    //    public bool UseWeaponDisplay;

    //    [DefaultValue(true)]
    //    [Label("$Mods.CoolerItemVisualEffect.Config.Num3")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.Config.Num4")]
    //    public bool FirstWeaponDisplay;

    //    [Header("$Mods.CoolerItemVisualEffect.Config.head1")]
    //    [Label("$Mods.CoolerItemVisualEffect.Config.1")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.Config.2")]
    //    [DefaultValue(DyeSlot.None)]
    //    [DrawTicks]
    //    public DyeSlot DyeUsed;

    //    [DefaultValue(false)]
    //    [Label("$Mods.CoolerItemVisualEffect.Config.3")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.Config.4")]
    //    public bool ShowGlow;

    //    [Increment(0.05f)]
    //    [Range(0f, 1f)]
    //    [DefaultValue(0.8f)]
    //    [Label("$Mods.CoolerItemVisualEffect.Config.5")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.Config.6")]
    //    [Slider]
    //    public float GlowLighting;

    //    [Increment(0.05f)]
    //    [Range(0.5f, 2f)]
    //    [DefaultValue(1f)]
    //    [Label("$Mods.CoolerItemVisualEffect.Config.11")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.Config.12")]
    //    [Slider]
    //    public float WeaponScale;

    //    [Header("$Mods.CoolerItemVisualEffect.Config.head2")]
    //    [DefaultValue(true)]
    //    [Label("$Mods.CoolerItemVisualEffect.Config.9")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.Config.10")]
    //    public bool LightItem;

    //    [Increment(1)]
    //    [Range(0, 2)]
    //    [DefaultValue(0)]
    //    [Label("$Mods.CoolerItemVisualEffect.Config.23")]
    //    [DrawTicks]
    //    public int LightItemNum;

    //    //[Header("$Mods.CoolerItemVisualEffect.Config.head3")]
    //    //[DefaultValue(true)]
    //    //[Label("$Mods.CoolerItemVisualEffect.Config.7")]
    //    //[Tooltip("$Mods.CoolerItemVisualEffect.Config.8")]
    //    //public bool CoolerSwooshActive;

    //    //[Increment(0.05f)]
    //    //[Range(0f, 1f)]
    //    //[DefaultValue(0.5f)]
    //    //[Label("$Mods.CoolerItemVisualEffect.Config.13")]
    //    //[Tooltip("$Mods.CoolerItemVisualEffect.Config.14")]
    //    //[Slider]
    //    //public float IsLighterDecider;

    //    //[DefaultValue(true)]
    //    //[Label("$Mods.CoolerItemVisualEffect.Config.15")]
    //    //[Tooltip("$Mods.CoolerItemVisualEffect.Config.16")]
    //    //public bool UseItemTexForSwoosh;

    //    //[DefaultValue(false)]
    //    //[Label("$Mods.CoolerItemVisualEffect.Config.17")]
    //    //[Tooltip("$Mods.CoolerItemVisualEffect.Config.18")]
    //    ////[BackgroundColor(0,0,0,0)]
    //    ////[ColorHSLSlider(true)]
    //    //public bool ItemAdditive;

    //    //[DefaultValue(true)]
    //    //[Label("$Mods.CoolerItemVisualEffect.Config.19")]
    //    //[Tooltip("$Mods.CoolerItemVisualEffect.Config.20")]
    //    //public bool ToolsNoUseNewSwooshEffect;

    //    //public override void OnChanged() {
    //    //    WeaponDisplay.IsLighterDecider = IsLighterDecider;
    //    //    WeaponDisplay.UseItemTexForSwoosh = UseItemTexForSwoosh;
    //    //    WeaponDisplay.ItemAdditive = ItemAdditive;
    //    //    WeaponDisplay.ToolsNoUse = ToolsNoUseNewSwooshEffect;
    //    //}
    //}

    //[Label("$Mods.CoolerItemVisualEffect.ConfigurationServer.Label")]
    //public class ConfigurationServer : ModConfig
    //{
    //    public override ConfigScope Mode => ConfigScope.ServerSide;

    //    [Header("$Mods.CoolerItemVisualEffect.ConfigurationServer.SlashSettings")]

    //    [DefaultValue(true)]
    //    [Label("$Mods.CoolerItemVisualEffect.ConfigurationServer.HitboxName")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.ConfigurationServer.HitboxTooltip")]
    //    public bool UseHitbox;

    //    [Range(1, 20)]
    //    [Increment(1)]
    //    [DefaultValue(4)]
    //    [Label("$Mods.CoolerItemVisualEffect.ConfigurationServer.AttackablesName")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.ConfigurationServer.AttackablesTooltip")]
    //    [Slider]
    //    public int ItemAttackCD;

    //    [Header("$Mods.CoolerItemVisualEffect.ConfigurationServer.Parry")]

    //    [DefaultValue(true)]
    //    [Label("$Mods.CoolerItemVisualEffect.ConfigurationServer.ParryName")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.ConfigurationServer.ParryTooltip")]
    //    public bool CanParry;

    //    [Label("$Mods.CoolerItemVisualEffect.ConfigurationServer.ParryListName")]
    //    [Tooltip("$Mods.CoolerItemVisualEffect.ConfigurationServer.ParryListTooltip")]
    //    public List<ItemDefinition> ParryItems = new List<ItemDefinition> {
    //        new(ModContent.ItemType<旧日支配者>()),
    //        new(ModContent.ItemType<真旧日支配者>())
    //    }; // 默认值直接在这里写
    //}

    //public enum DyeSlot
    //{
    //    None,
    //    Head,
    //    Body,
    //    Leg
    //}
}