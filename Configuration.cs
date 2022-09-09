using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect
{
    [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label_2")]
    public class ConfigurationNormal : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [DefaultValue(PreInstallSwoosh.普通Normal)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.47")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.48")]
        [DrawTicks]
        public PreInstallSwoosh preInstallSwoosh { get; set; }
        public ConfigurationSwoosh_Advanced SetCSValue(ConfigurationSwoosh_Advanced cs)
        {
            cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.中medium;
            cs.toolsNoUseNewSwooshEffect = false;
            cs.isLighterDecider = 0.2f;
            cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.色调处理与对角线混合;
            cs.swooshSampler = ConfigurationSwoosh_Advanced.SwooshSamplerState.线性;
            cs.swooshFactorStyle = ConfigurationSwoosh_Advanced.SwooshFactorStyle.每次开始时决定系数;
            cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.左右横劈_后倾;
            cs.swooshSize = 1f;
            cs.hueOffsetRange = 0.2f;
            cs.hueOffsetValue = 0f;
            cs.saturationScalar = 5f;
            cs.luminosityRange = 0.2f;
            cs.luminosityFactor = 0.2f;
            cs.rotationVelocity = 3f;
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
            cs.onlyChangeSizeOfSwoosh = true;
            cs.fadeStyle = ConfigurationSwoosh_Advanced.SwooshFadeStyle.全部Both;
            cs.growStyle = ConfigurationSwoosh_Advanced.SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;
            cs.animateIndex = 3;
            cs.distortSize = 1.5f;
            cs.actionOffsetSize = true;
            cs.actionOffsetSpeed = true;
            cs.actionModifyEffect = true;
            switch (preInstallSwoosh)
            {
                //case PreInstallSwoosh.普通Normal: 
                //	{
                //		break; 
                //	}
                case PreInstallSwoosh.飓风Hurricane:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.高high;
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
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.高high;
                        cs.swooshSize = 3f;
                        cs.distortFactor = 1f;
                        cs.shake = 1f;
                        cs.swooshFactorStyle = ConfigurationSwoosh_Advanced.SwooshFactorStyle.系数中间插值;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.旋风劈;
                        cs.maxCount = 5;
                        cs.luminosityFactor = 1f;
                        break;
                    }
                case PreInstallSwoosh.明亮Bright:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.高high;
                        cs.isLighterDecider = 0;
                        cs.luminosityFactor = 0.6f;
                        cs.itemAdditive = true;
                        cs.glowLight = 1;
                        break;
                    }
                case PreInstallSwoosh.怪异Strange:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.函数生成热度图;
                        break;
                    }
                case PreInstallSwoosh.光滑Smooth:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.加权平均_饱和与色调处理;
                        cs.imageIndex = 0f;
                        break;
                    }
                case PreInstallSwoosh.泰拉Terra_EspeciallyTerraBladeRecommended: 
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.函数生成热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.风暴灭却剑;
                        cs.hueOffsetRange = -0.25f;
                        cs.hueOffsetValue = 0.15f;
                        cs.luminosityFactor = 1f;
                        cs.rotationVelocity = 3f;
                        cs.distortFactor = 1f;
                        cs.shake = 0.4f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        break;
                    }
                case PreInstallSwoosh.神圣Holy_EspeciallyTrueExcaliburRecommended:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.函数生成热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.流雨断;
                        cs.hueOffsetRange = 0.25f;
                        cs.hueOffsetValue = 0.9f;
                        cs.luminosityFactor = 1f;
                        cs.rotationVelocity = 3f;
                        cs.distortFactor = 1f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        break;
                    }
                case PreInstallSwoosh.永夜Evil_EspeciallyTrueNightsEdgeRecommended:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.函数生成热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.旋风劈;
                        cs.hueOffsetRange = 0.25f;
                        cs.hueOffsetValue = 0.05f;
                        cs.luminosityFactor = 1f;
                        cs.rotationVelocity = 3f;
                        cs.distortFactor = 1f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        break;
                    }
                case PreInstallSwoosh.旧日OldOnes_EspeciallyFlyingDragonRecommended:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.函数生成热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.腾云斩;
                        cs.hueOffsetRange = 0.2f;
                        cs.hueOffsetValue = 0.95f;
                        cs.luminosityFactor = 1f;
                        cs.rotationVelocity = 3f;
                        cs.distortFactor = 1f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        break;
                    }
                case PreInstallSwoosh.波涌Influx_EspeciallyInfluxWaverRecommended: 
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.函数生成热度图;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.鸣雷刺;
                        cs.hueOffsetRange = 0.15f;
                        cs.hueOffsetValue = 0.05f;
                        cs.luminosityFactor = 1f;
                        cs.rotationVelocity = 3f;
                        cs.distortFactor = 1f;
                        cs.shake = 0.2f;
                        cs.glowLight = 1f;
                        cs.maxCount = 2;
                        break;
                    }
                case PreInstallSwoosh.黑白Grey:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.加权平均_饱和与色调处理;
                        cs.saturationScalar = 0;
                        break;
                    }
                case PreInstallSwoosh.反相InverseHue:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.加权平均_饱和与色调处理;
                        cs.hueOffsetValue = 0.5f;
                        break;
                    }
                case PreInstallSwoosh.彩虹Rainbow:
                    {
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.加权平均_饱和与色调处理;
                        cs.hueOffsetRange = 1f;
                        break;
                    }
                case PreInstallSwoosh.超级彩虹UltraRainbow:
                    {
                        cs.coolerSwooshQuality = ConfigurationSwoosh_Advanced.QualityType.极限ultra;
                        cs.swooshColorType = ConfigurationSwoosh_Advanced.SwooshColorType.加权平均_饱和与色调处理;
                        cs.swooshActionStyle = ConfigurationSwoosh_Advanced.SwooshAction.风暴灭却剑;
                        cs.hueOffsetRange = 1f;
                        cs.hueOffsetValue = 0f;
                        cs.luminosityFactor = 1f;
                        cs.rotationVelocity = 2f;
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
        public bool useWeaponDisplay;

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.Config.Num3")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.Num4")]
        public bool firstWeaponDisplay;

        [Increment(0.05f)]
        [Range(0.5f, 2f)]
        [DefaultValue(1f)]
        [Label("$Mods.CoolerItemVisualEffect.Config.11")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.12")]
        [Slider]
        public float weaponScale;

        [DefaultValue(HitBoxStyle.线状AABBLine)]
        [DrawTicks]
        [Label("$Mods.CoolerItemVisualEffect.Config.11")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.12")]
        public HitBoxStyle hitBoxStyle;

        [JsonIgnore]
        public bool UseHitbox;
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
        public bool DontChangeMyTitle;

        [Header("$Mods.CoolerItemVisualEffect.Config.24")]
        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.Config.25")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.26")]
        public bool ItemDropEffectActive;

        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.Config.27")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.28")]
        public bool ItemInventoryEffectActive;

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.Config.29")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.30")]
        public bool VanillaProjectileDrawModifyActive;

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.Config.31")]
        [Tooltip("$Mods.CoolerItemVisualEffect.Config.32")]
        public bool TeleprotEffectActive;
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
            packet.Write(rotationVelocity);
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
            config.rotationVelocity = reader.ReadSingle();
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
                rotationVelocity == config.rotationVelocity &&
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
                ignoreDamageType == config.ignoreDamageType;
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

        //[DefaultValue(true)]
        //[Label("更帅的拔刀")]
        //[Tooltip("你厌倦了原版近战的挥动方式了吗？")]
        //[BackgroundColor(0, 0, 255, 127)]
        //public bool CoolerSwooshActive;
        [DrawTicks]
        [DefaultValue(QualityType.中medium)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.1")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.2")]
        [BackgroundColor(0, 0, 255, 127)]
        public QualityType coolerSwooshQuality { get; set; }

        [JsonIgnore]
        public bool CoolerSwooshActive => (byte)coolerSwooshQuality > 0;

        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.3")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.4")]
        [BackgroundColor(15, 0, 255, 127)]
        public bool toolsNoUseNewSwooshEffect { get; set; }
        //[Increment(0.05f)]
        //[Range(0f, 1f)]
        //[DefaultValue(0.2f)]
        //[Label("挥砍效果亮度")]
        //[Tooltip("亮度大于这个值就会使用高亮式挥砍")]
        //[Slider]
        //[BackgroundColor(15, 0, 255, 127)]
        //public float IsLighterDecider{ get; set; }
        [Increment(0.05f)]
        [Range(0f, 1f)]
        [DefaultValue(0.2f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.5")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.6")]
        //[Slider]
        [BackgroundColor(30, 0, 255, 127)]
        public float isLighterDecider { get; set; }

        //[DefaultValue(true)]
        //[Label("挥砍效果颜色")]
        //[Tooltip("是, 以开启武器贴图对角线颜色关联挥砍效果颜色, 否, 使用单色. 默认为是")]
        //public bool UseItemTexForSwoosh{ get; set; }
        [DrawTicks]
        [DefaultValue(SwooshColorType.色调处理与对角线混合)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.7")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.8")]
        [BackgroundColor(45, 0, 255, 127)]
        public SwooshColorType swooshColorType { get; set; }

        [DrawTicks]
        [DefaultValue(SwooshSamplerState.线性)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.9")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.10")]
        [BackgroundColor(60, 0, 255, 127)]
        public SwooshSamplerState swooshSampler { get; set; }

        [DrawTicks]
        [DefaultValue(SwooshFactorStyle.每次开始时决定系数)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.11")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.12")]
        [BackgroundColor(75, 0, 255, 127)]
        public SwooshFactorStyle swooshFactorStyle { get; set; }

        [DrawTicks]
        [DefaultValue(SwooshAction.左右横劈_后倾)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.13")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.14")]
        [BackgroundColor(60, 0, 255, 127)]
        public SwooshAction swooshActionStyle { get; set; }

        [Increment(0.05f)]
        [DefaultValue(1f)]
        [Range(0.5f, 3f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.15")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.16")]
        [BackgroundColor(105, 0, 255, 127)]
        public float swooshSize { get; set; }

        [DefaultValue(0.2f)]
        [Increment(0.01f)]
        [Range(-1f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.17")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.18")]
        [BackgroundColor(120, 0, 255, 127)]
        public float hueOffsetRange { get; set; }


        [DefaultValue(0f)]
        [Increment(0.01f)]
        [Range(0f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.19")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.20")]
        [BackgroundColor(135, 0, 255, 127)]
        public float hueOffsetValue { get; set; }

        [DefaultValue(5f)]
        [Increment(0.05f)]
        [Range(0f, 5f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.21")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.22")]
        [BackgroundColor(150, 0, 255, 127)]
        public float saturationScalar { get; set; }

        [DefaultValue(0.2f)]
        [Increment(0.05f)]
        [Range(0f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.23")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.24")]
        [BackgroundColor(165, 0, 255, 127)]
        public float luminosityRange { get; set; }

        [DefaultValue(0f)]
        [Increment(0.05f)]
        [Range(0f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.25")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.26")]
        [BackgroundColor(180, 0, 255, 127)]
        public float luminosityFactor { get; set; }

        [Increment(0.05f)]
        [DefaultValue(3f)]
        [Range(1f, 3f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.27")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.28")]
        [BackgroundColor(195, 0, 255, 127)]
        public float rotationVelocity { get; set; }

        [Increment(0.05f)]
        [DefaultValue(0.25f)]
        [Range(-1f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.29")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.30")]
        [BackgroundColor(210, 0, 255, 127)]
        public float distortFactor { get; set; }

        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.31")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.32")]
        [BackgroundColor(225, 0, 255, 127)]
        //[BackgroundColor(0,0,0,0)]
        //[ColorHSLSlider(true)]
        public bool itemAdditive { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.33")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.34")]
        [BackgroundColor(240, 0, 255, 127)]
        public bool itemHighLight { get; set; }

        [Increment(0.05f)]
        [DefaultValue(0f)]
        [Range(0f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.35")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.36")]
        [BackgroundColor(255, 0, 255, 127)]
        public float shake { get; set; }

        [Increment(1f)]
        [DefaultValue(7f)]
        [Range(0, 11f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.37")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.38")]
        [BackgroundColor(255, 0, 240, 127)]
        public float imageIndex { get; set; }
        [JsonIgnore]
        public int ImageIndex => (int)MathHelper.Clamp(ConfigSwooshInstance.imageIndex, 0, 11);

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.39")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.40")]
        [BackgroundColor(255, 0, 225, 127)]
        public bool checkAir { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.41")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.42")]
        [BackgroundColor(255, 0, 210, 127)]
        public bool gather { get; set; }//

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.43")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.44")]
        [BackgroundColor(255, 0, 195, 127)]
        public bool allowZenith { get; set; }//

        [Increment(0.05f)]
        [DefaultValue(0f)]
        [Range(0f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.45")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.46")]
        [BackgroundColor(255, 0, 180, 127)]
        public float glowLight { get; set; }//

        [DefaultValue(1)]
        [Range(1, 10)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.51")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.52")]
        [BackgroundColor(255, 0, 165, 127)]
        public int maxCount { get; set; }//

        [DefaultValue(3.1415f)]
        [Range(0, 6.283f)]
        [Increment(0.05f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.53")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.54")]
        [BackgroundColor(255, 0, 150, 127)]
        public float directOfHeatMap { get; set; }

        [DefaultValue(30f)]
        [Range(0, 60f)]
        [Increment(1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.55")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.56")]
        [BackgroundColor(255, 0, 135, 127)]
        public float swooshTimeLeft { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.57")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.58")]
        [BackgroundColor(255, 0, 120, 127)]
        public bool onlyChangeSizeOfSwoosh { get; set; }

        [DrawTicks]
        [DefaultValue(SwooshFadeStyle.全部Both)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.59")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.60")]
        [BackgroundColor(255, 0, 105, 127)]
        public SwooshFadeStyle fadeStyle { get; set; }

        [JsonIgnore]
        public bool IsTransparentFade => fadeStyle == SwooshFadeStyle.逐渐透明GraduallyTransparent || fadeStyle == SwooshFadeStyle.全部Both;

        [JsonIgnore]
        public bool IsCloseAngleFade => fadeStyle == SwooshFadeStyle.角度收缩CloseTheAngle || fadeStyle == SwooshFadeStyle.全部Both;

        [JsonIgnore]
        public bool IsDarkFade => fadeStyle == SwooshFadeStyle.逐渐黯淡GraduallyFade || fadeStyle == SwooshFadeStyle.全部Both;

        [DrawTicks]
        [DefaultValue(SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.61")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.62")]
        [BackgroundColor(255, 0, 90, 127)]
        public SwooshGrowStyle growStyle { get; set; }

        [JsonIgnore]
        public bool IsExpandGrow => growStyle == SwooshGrowStyle.扩大Expand || growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;

        [JsonIgnore]
        public bool IsHorizontallyGrow => growStyle == SwooshGrowStyle.横向扩大ExpandHorizontally || growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;

        [JsonIgnore]
        public bool IsOffestGrow => growStyle == SwooshGrowStyle.平移Offest || growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest;

        [Increment(1f)]
        [DefaultValue(3f)]
        [Range(0, 5f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.63")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.64")]
        [BackgroundColor(255, 0, 75, 127)]
        public float animateIndex { get; set; }
        [JsonIgnore]
        public int AnimateIndex => (int)MathHelper.Clamp(ConfigSwooshInstance.animateIndex, 0, 5);

        [Increment(0.05f)]
        [DefaultValue(1.5f)]
        [Range(0.5f, 3f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.65")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.66")]
        [BackgroundColor(255, 0, 60, 127)]
        public float distortSize { get; set; }

        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.67")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.68")]
        [BackgroundColor(255, 0, 45, 127)]
        public bool showHeatMap { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.69")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.70")]
        [BackgroundColor(255, 0, 30, 127)]
        public bool actionOffsetSize { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.73")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.74")]
        [BackgroundColor(255, 0, 15, 127)]
        public bool actionOffsetSpeed { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.75")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.76")]
        [BackgroundColor(255, 0, 0, 127)]
        public bool actionModifyEffect { get; set; }

        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.77")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.78")]
        [BackgroundColor(255, 15, 0, 127)]
        public bool ignoreDamageType { get; set; }
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
            加权平均,
            加权平均_饱和与色调处理,
            函数生成热度图,
            武器贴图对角线,
            色调处理与对角线混合
        }
        public enum QualityType : byte
        {
            关off,
            低low,
            中medium,
            高high,
            极限ultra
        }
        public enum SwooshFadeStyle 
        {
            逐渐透明GraduallyTransparent,
            角度收缩CloseTheAngle,
            逐渐黯淡GraduallyFade,
            全部Both
        }
        public enum SwooshGrowStyle
        {
            扩大Expand,
            横向扩大ExpandHorizontally,
            平移Offest,
            横向扩大与平移BothExpandHorizontallyAndOffest
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