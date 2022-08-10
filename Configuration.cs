using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect
{
    [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label_2")]
    public class ConfigurationPreInstall : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [DefaultValue(PreInstallSwoosh.普通Normal)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.47")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.48")]
        [DrawTicks]
        public PreInstallSwoosh preInstallSwoosh { get; set; }
        public ConfigurationSwoosh SetCSValue(ConfigurationSwoosh cs)
        {
            cs.CoolerSwooshActive = true;
            cs.ToolsNoUseNewSwooshEffect = false;
            cs.IsLighterDecider = 0.2f;
            cs.swooshColorType = ConfigurationSwoosh.SwooshColorType.色调处理与对角线混合;
            cs.swooshSampler = ConfigurationSwoosh.SwooshSamplerState.线性;
            cs.swooshFactorStyle = ConfigurationSwoosh.SwooshFactorStyle.每次开始时决定系数;
            cs.swooshActionStyle = ConfigurationSwoosh.SwooshAction.向后倾一定角度后重击;
            cs.swooshSize = 1f;
            cs.hueOffsetRange = 0.2f;
            cs.hueOffsetValue = 0f;
            cs.saturationScalar = 5f;
            cs.luminosityRange = 0.2f;
            cs.luminosityFactor = 0f;
            cs.rotationVelocity = 3f;
            cs.distortFactor = 0.25f;
            cs.ItemAdditive = false;
            cs.ItemHighLight = true;
            cs.Shake = 0f;
            cs.ImageIndex = 7f;
            cs.checkAir = true;
            cs.gather = true;
            cs.allowZenith = true;
            cs.glowLight = 0f;
            cs.maxCount = 1;
            switch (preInstallSwoosh)
            {
                //case PreInstallSwoosh.普通Normal: 
                //	{
                //		break; 
                //	}
                case PreInstallSwoosh.飓风Hurricane:
                    {
                        cs.Shake = 0.3f;
                        cs.distortFactor = 1f;
                        cs.swooshSize = 1.5f;
                        cs.swooshActionStyle = ConfigurationSwoosh.SwooshAction.两次普通斩击一次高速旋转;
                        cs.maxCount = 3;
                        break;
                    }
                case PreInstallSwoosh.巨大Huge:
                    {
                        cs.swooshSize = 3f;
                        break;
                    }
                case PreInstallSwoosh.夸张Exaggerate:
                    {
                        cs.swooshSize = 3f;
                        cs.distortFactor = 1f;
                        cs.Shake = 1f;
                        cs.swooshFactorStyle = ConfigurationSwoosh.SwooshFactorStyle.系数中间插值;
                        cs.swooshActionStyle = ConfigurationSwoosh.SwooshAction.两次普通斩击一次高速旋转;
                        cs.maxCount = 5;
                        break;
                    }
                case PreInstallSwoosh.明亮Bright:
                    {
                        cs.IsLighterDecider = 0;
                        cs.ItemAdditive = true;
                        cs.glowLight = 1;
                        break;
                    }
                case PreInstallSwoosh.怪异Strange:
                    {
                        cs.swooshColorType = ConfigurationSwoosh.SwooshColorType.函数生成热度图;
                        break;
                    }
                case PreInstallSwoosh.光滑Smooth:
                    {
                        cs.swooshColorType = ConfigurationSwoosh.SwooshColorType.加权平均_饱和与色调处理;
                        cs.ImageIndex = 0f;
                        break;
                    }
                case PreInstallSwoosh.黑白Grey:
                    {
                        cs.swooshColorType = ConfigurationSwoosh.SwooshColorType.加权平均_饱和与色调处理;
                        cs.saturationScalar = 0;
                        break;
                    }
                case PreInstallSwoosh.反相InverseHue:
                    {
                        cs.swooshColorType = ConfigurationSwoosh.SwooshColorType.加权平均_饱和与色调处理;
                        cs.hueOffsetValue = 0.5f;
                        break;
                    }
                case PreInstallSwoosh.彩虹Rainbow:
                    {
                        cs.swooshColorType = ConfigurationSwoosh.SwooshColorType.加权平均_饱和与色调处理;
                        cs.hueOffsetRange = 1f;
                        break;
                    }
            }
            return cs;
        }
        public override void OnChanged()
        {
            if (preInstallSwoosh == PreInstallSwoosh.自定义UserDefined) return;
            var cs = ConfigurationSwoosh.instance;
            if (cs == null) return;
            SetCSValue(cs);
            ConfigurationSwoosh.Save(cs);
            CoolerItemVisualEffect.ChangeAllPureHeatMap();
            if (Main.netMode == NetmodeID.MultiplayerClient) ConfigurationSwoosh.instance.SendData();
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
            黑白Grey,
            反相InverseHue,
            彩虹Rainbow,
            自定义UserDefined
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
        public float WeaponScale;

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigurationServer.HitboxName")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigurationServer.HitboxTooltip")]
        public bool UseHitbox;

        [Range(1, 20)]
        [Increment(1)]
        [DefaultValue(4)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigurationServer.AttackablesName")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigurationServer.AttackablesTooltip")]
        [Slider]
        public int ItemAttackCD;

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.49")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.50")]
        public bool DontChangeMyTitle;
        public static ConfigurationPreInstall instance => ModContent.GetInstance<ConfigurationPreInstall>();

    }

    [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.Label")]
    public class ConfigurationSwoosh : ModConfig
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
            packet.Write(CoolerSwooshActive);
            packet.Write(ToolsNoUseNewSwooshEffect);
            packet.Write(IsLighterDecider);
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
            packet.Write(ItemAdditive);
            packet.Write(ItemHighLight);
            packet.Write(Shake);
            packet.Write(ImageIndex);
            packet.Write(checkAir);
            packet.Write(gather);
            packet.Write(allowZenith);
            packet.Write(glowLight);
            packet.Write((byte)maxCount);

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
            config.CoolerSwooshActive = reader.ReadBoolean();
            config.ToolsNoUseNewSwooshEffect = reader.ReadBoolean();
            config.IsLighterDecider = reader.ReadSingle();
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
            config.ItemAdditive = reader.ReadBoolean();
            config.ItemHighLight = reader.ReadBoolean();
            config.Shake = reader.ReadSingle();
            config.ImageIndex = reader.ReadSingle();
            config.checkAir = reader.ReadBoolean();
            config.gather = reader.ReadBoolean();
            config.allowZenith = reader.ReadBoolean();
            config.glowLight = reader.ReadSingle();
            config.maxCount = reader.ReadByte();
            //Main.NewText("向 " + Main.player[whoami] + "设置数据");

        }
        //[JsonIgnore]
        //public static int MagicConfigCounter;
        bool EqualValue(ConfigurationSwoosh config)
        {
            return
                CoolerSwooshActive == config.CoolerSwooshActive &&
                ToolsNoUseNewSwooshEffect == config.ToolsNoUseNewSwooshEffect &&
                IsLighterDecider == config.IsLighterDecider &&
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
                ItemAdditive == config.ItemAdditive &&
                ItemHighLight == config.ItemHighLight &&
                Shake == config.Shake &&
                ImageIndex == config.ImageIndex &&
                checkAir == config.checkAir &&
                gather == config.gather &&
                allowZenith == config.allowZenith &&
                glowLight == config.glowLight &&
                maxCount == config.maxCount;
        }
        public override void OnChanged()
        {
            if (!EqualValue(ConfigurationPreInstall.instance.SetCSValue(new ConfigurationSwoosh()))) 
            {
                ConfigurationPreInstall.instance.preInstallSwoosh = ConfigurationPreInstall.PreInstallSwoosh.自定义UserDefined;
                Save(ConfigurationPreInstall.instance);
            }
            CoolerItemVisualEffect.ChangeAllPureHeatMap();
            //MagicConfigCounter++;
            if (Main.netMode == NetmodeID.MultiplayerClient) SendData();
        }
        public override void OnLoaded()
        {
            //if (Main.netMode == NetmodeID.MultiplayerClient) SendData();
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static ConfigurationSwoosh instance => ModContent.GetInstance<ConfigurationSwoosh>();

        //[DefaultValue(true)]
        //[Label("更帅的拔刀")]
        //[Tooltip("你厌倦了原版近战的挥动方式了吗？")]
        //[BackgroundColor(0, 0, 255, 127)]
        //public bool CoolerSwooshActive;
        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.1")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.2")]
        [BackgroundColor(0, 0, 255, 127)]
        public bool CoolerSwooshActive { get; set; }

        [DefaultValue(false)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.3")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.4")]
        [BackgroundColor(15, 0, 255, 127)]
        public bool ToolsNoUseNewSwooshEffect { get; set; }
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
        public float IsLighterDecider { get; set; }

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
        [DefaultValue(SwooshAction.向后倾一定角度后重击)]
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
        [Range(0f, 1f)]
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
        public bool ItemAdditive { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.33")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.34")]
        [BackgroundColor(240, 0, 255, 127)]
        public bool ItemHighLight { get; set; }

        [Increment(0.05f)]
        [DefaultValue(0f)]
        [Range(0f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.35")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.36")]
        [BackgroundColor(255, 0, 255, 127)]
        public float Shake { get; set; }

        [Increment(1f)]
        [DefaultValue(7f)]
        [Range(0, 8f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.37")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.38")]
        [BackgroundColor(255, 127, 255, 127)]
        public float ImageIndex { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.39")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.40")]
        [BackgroundColor(127, 255, 255, 127)]
        public bool checkAir { get; set; }

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.41")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.42")]
        [BackgroundColor(255, 255, 127, 127)]
        public bool gather { get; set; }//

        [DefaultValue(true)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.43")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.44")]
        [BackgroundColor(255, 255, 255, 127)]
        public bool allowZenith { get; set; }//

        [Increment(0.05f)]
        [DefaultValue(0f)]
        [Range(0f, 1f)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.45")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.46")]
        [BackgroundColor(127, 127, 127, 127)]
        public float glowLight { get; set; }//

        [DefaultValue(1)]
        [Range(1, 10)]
        [Label("$Mods.CoolerItemVisualEffect.ConfigSwoosh.51")]
        [Tooltip("$Mods.CoolerItemVisualEffect.ConfigSwoosh.52")]
        [BackgroundColor(102, 153, 204, 127)]
        public int maxCount { get; set; }//

        public enum SwooshSamplerState
        {
            各向异性,
            线性,
            点,
        }
        public enum SwooshAction
        {
            正常挥砍,
            向后倾一定角度后重击,
            两次普通斩击一次高速旋转
        }
        public enum SwooshFactorStyle
        {
            每次开始时决定系数,
            系数中间插值
        }
        public enum SwooshColorType
        {
            加权平均,
            加权平均_饱和与色调处理,
            函数生成热度图,
            武器贴图对角线,
            色调处理与对角线混合
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