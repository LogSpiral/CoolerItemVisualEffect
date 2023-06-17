using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.Utilities;
using static CoolerItemVisualEffect.ConfigSLer.ConfigSLHelper;
using static CoolerItemVisualEffect.ConfigurationSwoosh;
namespace CoolerItemVisualEffect.ConfigSLer
{
    public class ConfigSLUI : UIState
    {
        public static bool Visible { get; private set; }
        public bool CacheSetupConfig; // 缓存，在下一帧Setup
        public bool CacheSetupConfigInfos; // 缓存，在下一帧Setup
        public string CacheConfigInfoPath;

        private int oldScreenHeight;

        public Asset<Texture2D> RefreshTexture;
        public Asset<Texture2D> BackTexture;
        public Asset<Texture2D> ButtonBackgroundTexture;

        private SUIPanel BasePanel; // 背景板
        public CoolerUIScrollbar Scrollbar; // 拖动条
        public UIList UIList; // 明细列表
        public CoolerImageButton RefreshButton; // 刷新/回退按钮
                                                // 当前结构的信息
        /// <summary>
        /// 初始化
        /// </summary>
        public override void OnInitialize()
        {
            #region 贴图加载
            //var saveTexture = GetTexture("UI/Construct/Save");
            //var loadTexture = GetTexture("UI/Construct/Load");
            //var explodeAndPlaceTexture = GetTexture("UI/Construct/ExplodeAndPlace");
            //var placeOnlyTexture = GetTexture("UI/Construct/PlaceOnly");
            RefreshTexture = GetTexture("UI/Construct/Refresh");
            BackTexture = GetTexture("UI/Construct/Back");
            ButtonBackgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
            #endregion
            #region 面板初始化
            BasePanel = new()
            {
                Top = StyleDimension.FromPixels(256f),
                HAlign = 0.2f
            };
            //BasePanel.SetPos(new Vector2(256, 256));
            BasePanel.SetSize(600f, 0f, precentHeight: 0.75f).SetPadding(12f);
            BasePanel.Draggable = true;
            Append(BasePanel);
            #endregion
            #region 列表初始化
            UIList = new UIList
            {
                Width = StyleDimension.FromPixelsAndPercent(0, 1f),
                Height = StyleDimension.FromPixelsAndPercent(-60, 1f),
                PaddingBottom = 4f,
                PaddingTop = 4f,
                ListPadding = 24f,
                Top = StyleDimension.FromPixels(60)
            };
            UIList.SetPadding(2f);
            UIList.ManualSortMethod = _ => { }; // 阻止他自动排序
            BasePanel.Append(UIList);
            #endregion
            #region 滑动条初始化
            Scrollbar = new();
            Scrollbar.Left.Set(600f, 0f);
            Scrollbar.Top.Set(8f, 0f);
            Scrollbar.Height.Set(0, 0.975f);
            Scrollbar.Width.Set(32, 0);
            //UIList.SetScrollbar(Scrollbar); // 用自己的代码
            SetupScrollBar();
            BasePanel.Append(Scrollbar);
            #endregion
            #region 刷新按钮初始化
            //TODO: hjson
            RefreshButton = QuickButton(RefreshTexture, "{$Mods.ImproveGame.Common.Refresh}");
            RefreshButton.SetPos(new(-286f, 8), 0.5f, 0f);
            RefreshButton.OnMouseDown += (_, _) =>
            {
                CachedConfigDatas.Clear();
                SetupConfigList();
            };
            BasePanel.Append(RefreshButton);
            #endregion
            #region 文件夹开启按钮初始化
            var folderButton = QuickButton(GetTexture("UI/Construct/Folder"), "{$LegacyInterface.110}");
            folderButton.SetPos(new(-220f, 8), 0.5f, 0f);
            folderButton.OnMouseDown += (_, _) => Utils.OpenFolder(SavePath);
            BasePanel.Append(folderButton);
            #endregion
            #region 关闭按钮初始化
            var closeButton = QuickButton(GetTexture("UI/Construct/Close"), "{$LegacyInterface.71}");
            closeButton.SetPos(new(243, 8), 0.5f, 0f);
            closeButton.OnMouseDown += (_, _) => Close();
            BasePanel.Append(closeButton);
            #endregion
        }

        /// <summary>
        /// 快速实例化一个按钮
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="hoverText"></param>
        /// <returns></returns>
        private CoolerImageButton QuickButton(Asset<Texture2D> texture, string hoverText)
        {
            var button = new CoolerImageButton(texture, Color.Cyan with { A = 0 } * .75f, default);
            button.SetSize(44, 44);
            button.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
            button.HoverText = hoverText;
            button.configTexStyle = currentStyle;
            return button;
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            if (BasePanel.GetOuterDimensions().Contains(evt.MousePosition.ToPoint()) || Scrollbar.GetOuterDimensions().Contains(evt.MousePosition.ToPoint()))
                Scrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }

        /// <summary>
        /// 绘制这边更新滑动条的位置(?
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            var innerList = UIList.GetType().GetField("_innerList", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(UIList) as UIElement;
            if (Scrollbar is not null && innerList is not null)
            {
                innerList.Top.Set(-Scrollbar.ViewPosition, 0);
            }
            UIList.Recalculate();
            base.DrawSelf(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (CacheSetupConfig)
            {
                SetupConfigList();
                CacheSetupConfig = false;
            }
            if (CacheSetupConfigInfos)
            {
                SetupCurrentConfigList();
                CacheSetupConfigInfos = false;
            }

            Recalculate();
            base.Update(gameTime);

            if (BasePanel.IsMouseHovering || (Scrollbar.IsMouseHovering && Scrollbar.Visible))
            {
                if (Scrollbar.Visible)
                {
                    PlayerInput.LockVanillaMouseScroll("CoolerItemVisualEffect: Config GUI");
                }
                Main.LocalPlayer.mouseInterface = true;
            }

            if (oldScreenHeight != Main.screenHeight)
            {
                SetupScrollBar(false);
            }

            oldScreenHeight = Main.screenHeight;
        }
        /// <summary>
        /// <br>加载当前设置的详细信息</br>
        /// <br>未完工，乐</br>
        /// </summary>
        public void SetupCurrentConfigList()
        {
            if (string.IsNullOrEmpty(CacheConfigInfoPath) || !File.Exists(CacheConfigInfoPath))
            {
                SetupConfigList();
                return;
            }

            UIList.Clear();

            UIList.Add(QuickTitleText(GetText("NotFinishYet"), 0.5f));

            RefreshButton.SetImage(BackTexture);
            RefreshButton.HoverText = "{$UI.Back}";

            Recalculate();
            SetupScrollBar();
        }
        private void SetupScrollBar(bool resetViewPosition = true)
        {
            float height = UIList.GetInnerDimensions().Height;
            float totalHeight = UIList.GetTotalHeight();
            Scrollbar.SetView(height, totalHeight);
            if (resetViewPosition)
                Scrollbar.ViewPosition = 0f;

            Scrollbar.Visible = true;
            if (height >= totalHeight)
            {
                Scrollbar.Visible = false;
            }
        }
        /// <summary>
        /// 加载设置项并且设置滑动条
        /// </summary>
        public void SetupConfigList()
        {
            UIList.Clear();//清空

            var filePaths = Directory.GetFiles(SavePath);
            foreach (string path in filePaths)
            {
                if (Path.GetExtension(path) == Extension)
                {
                    UIList.Add(new ConfigItemPanel(path));//如果有就添加目标
                }
            }
            RefreshButton.SetImage(RefreshTexture);
            RefreshButton.HoverText = "{$Mods.ImproveGame.Common.Refresh}";

            Recalculate();
            SetupScrollBar();
        }
        /// <summary>
        /// 开启ui,加载设置表
        /// </summary>
        public void Open()
        {
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            SetupConfigList();
        }
        public void Close()
        {
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
        private static UIText QuickTitleText(string text, float originY, float originX = 0.5f) => new(text, 0.6f, true)
        {
            Height = StyleDimension.FromPixels(50f),
            Width = StyleDimension.FromPercent(1f),
            TextOriginX = originX,
            TextOriginY = originY
        };
    }
    public static class ConfigSLHelper
    {
        public static ConfigSLUI configSLUI => ConfigSLSystem.Instance.configSLUI;

        public static Dictionary<string, TagCompound> CachedConfigDatas = new();

        public static Asset<Texture2D> GetTexture(string path)
        {
            return ModContent.Request<Texture2D>($"CoolerItemVisualEffect/ConfigSLer/{path}", AssetRequestMode.ImmediateLoad);
        }
        public static string ConvertLeftRight(string text)
        {
            // 支持输入<left>和<right>，就和ItemTooltip一样（原版只有Tooltip支持）
            if (text.Contains("<right>"))
            {
                InputMode inputMode = InputMode.XBoxGamepad;
                if (PlayerInput.UsingGamepad)
                    inputMode = InputMode.XBoxGamepadUI;

                if (inputMode == InputMode.XBoxGamepadUI)
                {
                    KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[inputMode];
                    string input = PlayerInput.BuildCommand("", true, keyConfiguration.KeyStatus["MouseRight"]);
                    input = input.Replace(": ", "");
                    text = text.Replace("<right>", input);
                }
                else
                {
                    text = text.Replace("<right>", Language.GetTextValue("Controls.RightClick"));
                }
            }
            if (text.Contains("<left>"))
            {
                InputMode inputMode2 = InputMode.XBoxGamepad;
                if (PlayerInput.UsingGamepad)
                    inputMode2 = InputMode.XBoxGamepadUI;

                if (inputMode2 == InputMode.XBoxGamepadUI)
                {
                    KeyConfiguration keyConfiguration2 = PlayerInput.CurrentProfile.InputModes[inputMode2];
                    string input = PlayerInput.BuildCommand("", true, keyConfiguration2.KeyStatus["MouseLeft"]);
                    input = input.Replace(": ", "");
                    text = text.Replace("<left>", input);
                }
                else
                {
                    text = text.Replace("<left>", Language.GetTextValue("Controls.LeftClick"));
                }
            }
            return text;
        }
        /// <summary>
        /// 获取 HJson 文字
        /// </summary>
        public static string GetText(string str, params object[] arg)
        {
            string text = Language.GetTextValue($"Mods.CoolerItemVisualEffect.ConfigSLer.{str}", arg);
            return ConvertLeftRight(text);
        }
        public static UIElement SetSize(this UIElement uie, Vector2 size, float precentWidth = 0, float precentHeight = 0)
        {
            uie.SetSize(size.X, size.Y, precentWidth, precentHeight);
            return uie;
        }
        public static UIElement SetSize(this UIElement uie, float width, float height, float precentWidth = 0, float precentHeight = 0)
        {
            uie.Width.Set(width, precentWidth);
            uie.Height.Set(height, precentHeight);
            return uie;
        }
        public static UIElement SetPos(this UIElement uie, Vector2 position, float precentX = 0, float precentY = 0)
        {
            uie.SetPos(position.X, position.Y, precentX, precentY);
            return uie;
        }

        public static UIElement SetPos(this UIElement uie, float x, float y, float precentX = 0, float precentY = 0)
        {
            uie.Left.Set(x, precentX);
            uie.Top.Set(y, precentY);
            return uie;
        }
        public static bool Contains(this CalculatedStyle calculatedStyle, Vector2 position)
        {
            if (calculatedStyle.X <= position.X && position.X < calculatedStyle.X + calculatedStyle.Width &&
                calculatedStyle.Y <= position.Y)
            {
                return position.Y < calculatedStyle.Y + calculatedStyle.Height;
            }

            return false;
        }

        public static bool Contains(this CalculatedStyle calculatedStyle, Point position)
        {
            if (calculatedStyle.X <= position.X && position.X < calculatedStyle.X + calculatedStyle.Width &&
                calculatedStyle.Y <= position.Y)
            {
                return position.Y < calculatedStyle.Y + calculatedStyle.Height;
            }

            return false;
        }

        public static Vector2 Size(this CalculatedStyle calculatedStyle)
        {
            return new Vector2(calculatedStyle.Width, calculatedStyle.Height);
        }
        public static string SavePath => Path.Combine(ModLoader.ModPath, "CoolerItemVisualEffect");
        public static string Extension => ".civeConfig";
        public static void Save(ModConfig config)
        {
            var ModConfigPath = SavePath;
            Directory.CreateDirectory(ModConfigPath);
            string filename = GetText("DefaultName");
            string resultName = filename + Extension;
            string thisPath = Path.Combine(SavePath, resultName);
            int maxCount = 30;
            bool sameDefault = false;
            if (File.Exists(thisPath))
            {
                for (int i = 2; i <= maxCount; i++)
                {
                    resultName = $"{filename} ({i}){Extension}";
                    thisPath = Path.Combine(SavePath, resultName);
                    if (!File.Exists(thisPath))
                    {
                        sameDefault = i == 8;
                        break;
                    }
                    else if (i == maxCount)
                    {
                        Main.NewText($"[c/FF0000:{GetText("TooManySameName")}]");
                        return;
                    }
                }
            }
            if (sameDefault)
            {
                Main.NewText($"[c/00FFFF:{GetText("SameDefault")}]");
            }
            string json = JsonConvert.SerializeObject(config, ConfigManager.serializerSettings);
            File.WriteAllText(thisPath, json);

            Main.NewText(GetText("SavedAs") + thisPath, Color.Yellow);

            CachedConfigDatas.Clear();
            if (ConfigSLUI.Visible && configSLUI is not null)
            {
                configSLUI.CacheSetupConfig = true;
                if (ConfigSLSystem.ConstructFilePath == thisPath)
                    ConfigSLSystem.ConstructFilePath = string.Empty;
            }
        }
        public static void Load(ModConfig config, string filename, bool autoPath = false)
        {

            string path;
            if (autoPath)
            {
                filename += Extension;
                path = Path.Combine(SavePath, filename);
            }
            else path = filename;

            if (config.Mode == ConfigScope.ServerSide && ModNet.NetReloadActive)
            { // #999: Main.netMode isn't 1 at this point due to #770 fix.
                string netJson = ModNet.pendingConfigs.Single(x => x.modname == config.Mod.Name && x.configname == config.Name).json;
                JsonConvert.PopulateObject(netJson, config, ConfigManager.serializerSettingsCompact);
                return;
            }

            bool jsonFileExists = File.Exists(path);
            string json = jsonFileExists ? File.ReadAllText(path) : "{}";

            try
            {
                JsonConvert.PopulateObject(json, config, ConfigManager.serializerSettings);
                Main.NewText(GetText("Successed"), Color.Yellow);
                //Main.NewText((config as ConfigurationSwoosh).hueOffsetValue, Color.Yellow);
                //Main.NewText(ConfigurationSwoosh.ConfigSwooshInstance.hueOffsetValue, Color.Yellow);
                //ConfigManager.Save(ConfigurationSwoosh.ConfigSwooshInstance);
                CoolerItemVisualEffect.WhenConfigSwooshChange();
                SoundEngine.PlaySound(SoundID.Unlock);
            }
            catch (Exception e) when (jsonFileExists && (e is JsonReaderException || e is JsonSerializationException))
            {
                Logging.tML.Warn($"Then config file {config.Name} from the mod {config.Mod.Name} located at {path} failed to load. The file was likely corrupted somehow, so the defaults will be loaded and the file deleted.");
                File.Delete(path);
                //JsonConvert.PopulateObject("{}", config, ConfigManager.serializerSettings);
                CachedConfigDatas.Clear();
                configSLUI.SetupConfigList();
                Main.NewText(GetText("Failed"), Color.Red);
                SoundEngine.PlaySound(SoundID.Thunder);
            }
        }
    }
    public class ConfigSLSystem : ModSystem
    {
        public ConfigSLUI configSLUI;
        public UserInterface configSLInterface;
        public static ConfigSLSystem Instance;
        public static string ConstructFilePath;
        public override void Load()
        {
            configSLUI = new ConfigSLUI();
            configSLInterface = new UserInterface();
            configSLUI.Activate();
            configSLInterface.SetState(configSLUI);
            Instance = this;
            base.Load();
        }
        public override void Unload()
        {
            configSLUI = null;
            configSLInterface = null;
            Instance = null;
            base.Unload();
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (ConfigSLUI.Visible)
                configSLInterface?.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Structure GUI", () =>
                {
                    if (ConfigSLUI.Visible)
                        configSLUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
    public class ConfigSLer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.itemAnimation == player.itemAnimationMax)
            {
                if (player.altFunctionUse != 2)
                {
                    ConfigSLHelper.Save(ConfigSwooshInstance);
                }
                else
                {
                    if (ConfigSLUI.Visible)
                        ConfigSLSystem.Instance.configSLUI.Close();
                    else
                        ConfigSLSystem.Instance.configSLUI.Open();
                }
            }
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe().Register();
        }
    }
}
