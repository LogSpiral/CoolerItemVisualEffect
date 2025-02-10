global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.DataStructures;
global using Terraria.ID;
global using Terraria.ModLoader;
global using LogSpiralLibrary.CodeLibrary;
global using MeleeSequence = LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Sequence<LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Melee.MeleeAction>;

using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using static CoolerItemVisualEffect.Config.MeleeConfig;
using LogSpiralLibrary;
using System.IO;
using NetSimplified;
using System.Reflection;
using Terraria.ModLoader.Config.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System.Collections.Generic;
using Terraria.ModLoader.Config;
using CoolerItemVisualEffect.Config;
using System.Linq;
namespace CoolerItemVisualEffect
{
    public class CoolerItemVisualEffectMod : Mod
    {
        #region Effects
        internal static Effect ShaderSwooshEX => LogSpiralLibraryMod.ShaderSwooshEX;
        internal static Effect ShaderSwooshUL => LogSpiralLibraryMod.ShaderSwooshUL;
        internal static Effect RenderEffect => LogSpiralLibraryMod.RenderEffect;
        #endregion
        #region 辅助函数
        public override object Call(params object[] args)
        {
            if (args.Length == 0 || args[0] is not string str)
                return base.Call(args);
            switch (str)
            {
                case "RegisterModifyWeaponTex":
                    {
                        try
                        {

                            MeleeModifyPlayer.weaponGetFunctions.Add(((Func<Item, Texture2D> func, float priority))(args[1], args[2]));
                            MeleeModifyPlayer.RefreshWeaponTexFunc = true;
                        }
                        catch
                        {
                            return null;
                        }
                        return null;
                    }
                default: return null;
            }
        }
        #endregion
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetModule.ReceiveModule(reader, whoAmI);

            base.HandlePacket(reader, whoAmI);
        }
        public void RegisterCategory(Mod qot, List<KeyValuePair<string, ModConfig>> variables, int itemIconID = 0, 
            Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
            => qot.Call("RegisterCategory", this, variables, itemIconID, getIconTexture, getLabel, getTooltip);

        public void RegisterCategory(Mod qot, ModConfig modConfig, List<string> variables, int itemIconID = 0,
            Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
            => RegisterCategory(qot, (from name in variables select new KeyValuePair<string, ModConfig>(name, modConfig)).ToList(),
                itemIconID, getIconTexture, getLabel, getTooltip);

        public void RegisterCategory(Mod qot, List<(ModConfig, List<string>)> variables, int itemIconID = 0, Func<Texture2D> getIconTexture = null,
            Func<string> getLabel = null, Func<string> getTooltip = null)
        {
            List<KeyValuePair<string, ModConfig>> list = [];
            foreach (var pair in variables)
                list.AddRange((from name in pair.Item2 select new KeyValuePair<string, ModConfig>(name, pair.Item1)).ToList());
            RegisterCategory(qot, list, itemIconID, getIconTexture, getLabel, getTooltip);
        }


        public override void Load()
        {
            AddContent<NetModuleLoader>();

            if (ModLoader.TryGetMod("ImproveGame", out var qot))
            {
                RegisterCategory(qot, [
                    (SeverConfig.Instance,[nameof(SeverConfig.meleeModifyLevel)]),
                (MeleeConfig.Instance,
                [
                     nameof(MeleeConfig.SwordModifyActive),
                     nameof(MeleeConfig.swooshActionStyle),
                     nameof(MeleeConfig.swooshActionStyle),
                     nameof(MeleeConfig.baseIndexSwoosh),
                     nameof(MeleeConfig.animateIndexSwoosh),
                     nameof(MeleeConfig.baseIndexStab),
                     nameof(MeleeConfig.animateIndexStab),
                     nameof(MeleeConfig.swooshTimeLeft),
                     nameof(MeleeConfig.shake),
                     nameof(MeleeConfig.dustQuantity)
                ])],
                ItemID.TitaniumSword, null, () => "近战设置", () => "拜托这早就不只是视觉上的修改了");

                RegisterCategory(qot, MeleeConfig.Instance,
                [
                     nameof(MeleeConfig.weaponExtraLight),
                     nameof(MeleeConfig.colorVector),
                     nameof(MeleeConfig.alphaFactor),
                     nameof(MeleeConfig.heatMapCreateStyle),
                     nameof(MeleeConfig.heatMapFactorStyle),
                     nameof(MeleeConfig.byFuncData),
                     nameof(MeleeConfig.rgbData),
                     nameof(MeleeConfig.hslData),
                     nameof(MeleeConfig.designateData),
                     nameof(MeleeConfig.directOfHeatMap)
                ],
                ItemID.RainbowWallpaper, null, () => "渲染设置", () => "说实在的没有预览功能再怎么好看的配置面板也没用(");

                RegisterCategory(qot, MeleeConfig.Instance,
                [
                     nameof(MeleeConfig.distortConfigs),
                     nameof(MeleeConfig.maskConfigs),
                     nameof(MeleeConfig.dyeConfigs),
                     nameof(MeleeConfig.bloomConfigs)
                ],
                ItemID.LastPrism, null, () => "特效设置", () => "亮瞎眼了啊喂，显卡要冒烟了啊喂");

                RegisterCategory(qot, MiscConfig.Instance,
                [
                     nameof(MiscConfig.useWeaponDisplay),
                     nameof(MiscConfig.firstWeaponDisplay),
                     nameof(MiscConfig.weaponScale),
                     nameof(MiscConfig.ItemDropEffectActive),
                     nameof(MiscConfig.ItemInventoryEffectActive),
                     nameof(MiscConfig.VanillaProjectileDrawModifyActive),
                     nameof(MiscConfig.TeleprotEffectActive)
                ],
                ItemID.Cog, null, () => "杂项设置", () => "非常水");
                qot.Call("SetAboutPage", this, () => "非常酷大剑转转转的配置中心！！！", (int)ItemID.IronShortsword, null, () => "关于大剑", () => "酷酷酷酷酷");
            }

            base.Load();
        }
        public override void Unload()
        {
            if (ModLoader.TryGetMod("ImproveGame", out var qot))
            {
                qot.Call("RemoveCategory", this);
                qot.Call("RemoveAboutPage", this);

            }
            base.Unload();
        }
    }
    /*public class CoolerItemVisualEffectSystem : ModSystem 
    {
        public override void Load()
        {
            var populateConfigsMethod = typeof(UIModConfig).GetMethod(nameof(UIModConfig.OnActivate),
    BindingFlags.Public | BindingFlags.Instance);
            MonoModHooks.Add(populateConfigsMethod, AddPreviewWindow);
            base.Load();
        }
        private static void AddPreviewWindow(Action<UIModConfig> orig, UIModConfig self)
        {
            orig.Invoke(self);
            if (self.modConfig.Name != "ConfigurationCIVE" || Main.gameMenu)
                return;
            if (previewPanel == null) 
            {
                previewPanel = new UIPanel();
                previewPanel.Left.Set(40, 1);
                previewPanel.Top.Set(0, 0);
                previewPanel.Width.Set(0, 1f);
                previewPanel.Height.Set(0, 1);


                var drawer = new PreviewDrawer();
                drawer.Width.Set(0, 1);
                drawer.Height.Set(0, 1);
                previewPanel.Append(drawer);
            }
            if (previewPanel.Parent != null)
                previewPanel.Remove();
            self.uIPanel.Append(previewPanel);
            previewPanel.MaxWidth.Set(Main.screenWidth / 2 - 200 * Main.UIScale,0);
        }
        public static UIPanel previewPanel;
    }
    public class PreviewDrawer : UIElement 
    {
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            ConfigurationCIVE config = ConfigCIVEInstance;
            MeleeModifyPlayer mplr = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            var dimension = GetDimensions();
            spriteBatch.Draw(mplr.heatMap, dimension.Position(), null, Color.White, 0, default, new Vector2(dimension.Width / 300f,50), 0, 0);
            base.DrawSelf(spriteBatch);
        }
    }*/
}