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
using static CoolerItemVisualEffect.ConfigurationCIVE;
using LogSpiralLibrary;
using System.IO;
using NetSimplified;
using System.Reflection;
using Terraria.ModLoader.Config.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
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
        public override void Load()
        {
            AddContent<NetModuleLoader>();

            base.Load();
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