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
namespace CoolerItemVisualEffect.ConfigSLer
{
    //下面几个都抄写或魔改自qol，感谢Cyril和局长
    public class SUIPanel : UIElement
    {
        internal bool Shaded;
        internal float ShadowThickness;
        internal Color ShadowColor;
        internal bool Draggable;
        internal bool Dragging;
        internal Vector2 Offset;

        public float round;
        public float border;
        public Color borderColor;
        public Color backgroundColor;
        public bool CalculateBorder;

        public SUIPanel(Color borderColor, Color backgroundColor, float round = 12, float border = 3, bool CalculateBorder = true)
        {
            SetPadding(10f);
            ShadowThickness = 50f;
            ShadowColor = new Color(0, 0, 0, 0.25f);
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.round = round;
            this.border = border;
            this.CalculateBorder = CalculateBorder;
            OnMouseDown += DragStart;
            OnMouseUp += DragEnd;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimenstions = GetDimensions();
            var rect = dimenstions.ToRectangle();
            if (CalculateBorder)
            {
                rect = Utils.CenteredRectangle(rect.Center.ToVector2(), rect.Size() + new Vector2(border * 2));
            }
            ConfigurationSwoosh.DrawCoolerPanel(spriteBatch, rect, borderColor, 0, ConfigurationSwoosh.currentStyle);
        }

        // 可拖动界面
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            // 当点击的是子元素不进行移动
            //Main.NewText((Draggable, evt.Target == this));
            if (Draggable && evt.Target == this)
            {
                Offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
                Dragging = true;
            }
        }

        // 可拖动/调整大小界面
        private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            Dragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (Dragging)
            {
                Left.Set(Main.mouseX - Offset.X, 0f);
                Top.Set(Main.mouseY - Offset.Y, 0f);
                Recalculate();
                OnDrag?.Invoke(this);
            }
        }

        public event ElementEvent OnDrag;
    }
    public class ConfigItemPanel : SUIPanel
    {
        public string FilePath { get; private set; }
        public string Name { get; private set; }

        private string _inputName;  // 用于先装输入缓存的
        private bool _renaming;
        private int _cursorTimer;
        private bool _oldMouseLeft;
        private string _selectedButtonName = "";

        public static readonly Color BorderSelectedColor = new(89, 116, 213);
        public static readonly Color BorderUnselectedColor = new(39, 46, 100);
        public static readonly Color SelectedColor = new(73, 94, 171);
        public static readonly Color UnselectedColor = new(62, 80, 146);

        public UIText NameText;
        public UIText PathText;
        public UIImageButton RenameButton;
        public SUIPanel PathPanel;

        public ConfigItemPanel(string filePath) : base(BorderUnselectedColor, UnselectedColor, CalculateBorder: false)
        {
            FilePath = filePath;

            _oldMouseLeft = true;

            Width = StyleDimension.FromPixels(580f);

            string name = FilePath.Split('\\').Last();
            name = name[..^Extension.Length]; // name.Substring(0, name.Length - FileOperator.Extension.Length)
            Name = name;
            NameText = new(name, 1.05f)
            {
                Left = StyleDimension.FromPixels(2f),
                Height = StyleDimension.FromPixels(24f)
            };
            Append(NameText);

            var buttonNameText = new UIText("")
            {
                Left = StyleDimension.FromPercent(1f),
                Top = StyleDimension.FromPixels(4f),
                Height = StyleDimension.FromPixels(20f)
            };
            buttonNameText.OnUpdate += (_) =>
            {
                string text = Language.GetTextValue(_selectedButtonName);
                var font = FontAssets.MouseText.Value;
                buttonNameText.SetText(text);
                buttonNameText.Left = new StyleDimension(-font.MeasureString(text).X, 1f);
                _selectedButtonName = "";
            };
            Append(buttonNameText);

            UIHorizontalSeparator separator = new()
            {
                Top = StyleDimension.FromPixels(NameText.Height.Pixels - 2f),
                Width = StyleDimension.FromPercent(1f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            };
            Append(separator);

            var detailButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"))
            {
                Top = new StyleDimension(separator.Height.Pixels + separator.Top.Pixels + 3f, 0f),
                Left = new StyleDimension(-20f, 1f)
            };
            detailButton.SetSize(24f, 24f);
            detailButton.OnClick += DetailButtonClick;
            detailButton.OnUpdate += (_) =>
            {
                if (detailButton.IsMouseHovering)
                {
                    _selectedButtonName = "tModLoader.ModsMoreInfo";
                }
            };
            Append(detailButton);

            var deleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"))
            {
                Top = detailButton.Top,
                Left = new StyleDimension(detailButton.Left.Pixels - 24f, 1f)
            };
            deleteButton.SetSize(24f, 24f);
            deleteButton.OnClick += DeleteButtonClick;
            deleteButton.OnUpdate += (_) =>
            {
                if (deleteButton.IsMouseHovering)
                {
                    _selectedButtonName = "UI.Delete";
                }
            };
            Append(deleteButton);

            RenameButton = new(Main.Assets.Request<Texture2D>("Images/UI/ButtonRename"))
            {
                Top = detailButton.Top,
                Left = new StyleDimension(deleteButton.Left.Pixels - 24f, 1f)
            };
            RenameButton.SetSize(24f, 24f);
            RenameButton.OnUpdate += (_) =>
            {
                if (RenameButton.IsMouseHovering)
                {
                    _selectedButtonName = "UI.Rename";
                }
            };
            Append(RenameButton);

            var loadConfigButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonFavoriteActive"))
            {
                Top = detailButton.Top,
                Left = new StyleDimension(RenameButton.Left.Pixels - 24f, 1f)
            };
            loadConfigButton.SetSize(24f, 24f);
            loadConfigButton.OnClick += (ev, e) =>
            {
                Load(ConfigurationSwoosh.ConfigSwooshInstance, FilePath);
            };
            loadConfigButton.OnUpdate += (_) =>
            {
                if (loadConfigButton.IsMouseHovering)
                {
                    _selectedButtonName = "Mods.CoolerItemVisualEffect.ConfigSLer.LoadThisConfig";
                }
            };
            Append(loadConfigButton);

            PathPanel = new(new Color(35, 40, 83), new Color(35, 40, 83), round: 10, CalculateBorder: false)
            {
                Top = detailButton.Top,
                OverflowHidden = true,
                PaddingLeft = 6f,
                PaddingRight = 6f,
                PaddingBottom = 0f,
                PaddingTop = 0f
            };
            PathPanel.SetSize(new(Width.Pixels + loadConfigButton.Left.Pixels - 44f, 23f));
            Append(PathPanel);
            PathText = new($"{GetText("Path")}{FilePath}", 0.7f)
            {
                Left = StyleDimension.FromPixels(2f),
                HAlign = 0f,
                VAlign = 0.5f,
                TextColor = Color.Gray
            };
            PathPanel.Append(PathText);
            SetSizedText();

            Height = StyleDimension.FromPixels(PathPanel.Top.Pixels + PathPanel.Height.Pixels + 22f);
        }

        private void DetailButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var ui = ConfigSLSystem.Instance.configSLUI;
            if (ui is not null)
            {
                ui.CacheSetupConfigInfos = true;
                ui.CacheConfigInfoPath = FilePath;
            }
        }

        private void RenameButtonClick()
        {
            _inputName = Name;
            _renaming = true;
            Main.blockInput = true;
            Main.clrInput();
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        private void EndRename()
        {
            _renaming = false;
            Main.blockInput = false;
            string newPath = FilePath.Replace(Name, _inputName);
            if (File.Exists(newPath) && Name != _inputName)
            {
                Main.NewText(GetText("RenameTip.Exists"));
                NameText.SetText(Name);
                return;
            }
            if (File.Exists(FilePath) && configSLUI is not null)
            {
                File.Move(FilePath, newPath);
                configSLUI.CacheSetupConfig = true;
            }
        }
        private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (File.Exists(FilePath))
                FileUtilities.Delete(FilePath, false);
            if (configSLUI is not null)
                configSLUI.CacheSetupConfig = true;
        }

        public override void Update(GameTime gameTime)
        {
            _cursorTimer++;
            _cursorTimer %= 60;
            if (Main.mouseLeft && !_oldMouseLeft)
            {
                var renameDimensions = RenameButton.GetOuterDimensions();
                if (renameDimensions.ToRectangle().Contains(Main.MouseScreen.ToPoint()) && !_renaming)
                {
                    RenameButtonClick();
                }
                else if (_renaming)
                {
                    EndRename();
                }
            }

            base.Update(gameTime);

            if (IsMouseHovering)
            {
                borderColor = BorderSelectedColor;
                backgroundColor = SelectedColor;
                NameText.TextColor = Color.White;
            }
            else
            {
                borderColor = BorderUnselectedColor;
                backgroundColor = UnselectedColor;
                NameText.TextColor = Color.LightGray;
            }
            //TODO: aaa
            //if (WandSystem.ConstructFilePath == FilePath)
            //{
            //    NameText.TextColor = new(255, 231, 69);
            //}
            SetSizedText();
            _oldMouseLeft = Main.mouseLeft;
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);

            if (Children.Any(i => i is UIImageButton && i.IsMouseHovering))
                return;

            SoundEngine.PlaySound(SoundID.MenuTick);
            if (ConfigSLSystem.ConstructFilePath == FilePath)
            {
                ConfigSLSystem.ConstructFilePath = string.Empty;
                return;
            }

            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
                return;

            //var tag = FileOperator.GetTagFromFile(FilePath);

            //if (tag is null)
            //{
            //    return;
            //}

            //ConfigSLSystem.ConstructFilePath = FilePath;
            //PreviewRenderer.ResetPreviewTarget = PreviewRenderer.ResetState.WaitReset;
            //int width = tag.GetShort("Width");
            //int height = tag.GetShort("Height");
            //PreviewRenderer.PreviewTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width * 16 + 20, height * 16 + 20, false, default, default, default, RenderTargetUsage.PreserveContents);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // 放Update，不行。放Draw，行！ReLogic我囸你_
            if (_renaming)
            {
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                var inputText = Main.GetInputText(_inputName);
                if (inputText.Length > 40)
                {
                    inputText = inputText[..40];
                    Main.NewText(GetText("RenameTip.TooLong"));
                }
                if (inputText.Contains('\\') || inputText.Contains('/') || inputText.Contains(':') || inputText.Contains('*') || inputText.Contains('?') || inputText.Contains('\"') || inputText.Contains('\'') || inputText.Contains('<') || inputText.Contains('>') || inputText.Contains('|'))
                {
                    Main.NewText(GetText("RenameTip.Illegal"));
                    return;
                }
                else
                {
                    _inputName = inputText;
                    NameText.SetText(_inputName + (_cursorTimer >= 30 ? "|" : ""));
                    NameText.Recalculate();
                }

                // Enter 或者 Esc
                if (KeyTyped(Keys.Enter) || KeyTyped(Keys.Tab) || KeyTyped(Keys.Escape))
                {
                    EndRename();
                }
            }

            base.Draw(spriteBatch);
        }

        public void SetSizedText()
        {
            string pathString = GetText("Path");
            var innerDimensions = PathPanel.GetInnerDimensions();
            var font = FontAssets.MouseText.Value;
            float scale = 0.7f;
            float dotWidth = font.MeasureString("...").X * scale;
            float pathWidth = font.MeasureString(pathString).X * scale;
            if (font.MeasureString(FilePath).X * scale >= innerDimensions.Width - 6f - pathWidth - dotWidth)
            {
                float width = 0f;
                int i;
                for (i = FilePath.Length - 1; i > 0; i--)
                {
                    width += font.MeasureString(FilePath[i].ToString()).X * scale;
                    if (width >= innerDimensions.Width - 6f - pathWidth - dotWidth)
                    {
                        break;
                    }
                }
                PathText.SetText($"{pathString}...{FilePath[i..]}");
            }
        }

        public static bool KeyTyped(Keys key) => Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
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
                Main.NewText(GetText("Successed") + path, Color.Yellow);
                //Main.NewText((config as ConfigurationSwoosh).hueOffsetValue, Color.Yellow);
                //Main.NewText(ConfigurationSwoosh.ConfigSwooshInstance.hueOffsetValue, Color.Yellow);
                //ConfigManager.Save(ConfigurationSwoosh.ConfigSwooshInstance);
                SoundEngine.PlaySound(SoundID.Unlock);
            }
            catch (Exception e) when (jsonFileExists && (e is JsonReaderException || e is JsonSerializationException))
            {
                Logging.tML.Warn($"Then config file {config.Name} from the mod {config.Mod.Name} located at {path} failed to load. The file was likely corrupted somehow, so the defaults will be loaded and the file deleted.");
                File.Delete(path);
                JsonConvert.PopulateObject("{}", config, ConfigManager.serializerSettings);
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
        public SUIScrollbar Scrollbar; // 拖动条
        public UIList UIList; // 明细列表
        public ModImageButton RefreshButton; // 刷新/回退按钮
                                             // 当前结构的信息
        /// <summary>
        /// 初始化
        /// </summary>
        public override void OnInitialize()
        {
            #region 贴图加载
            var saveTexture = GetTexture("UI/Construct/Save");
            var loadTexture = GetTexture("UI/Construct/Load");
            var explodeAndPlaceTexture = GetTexture("UI/Construct/ExplodeAndPlace");
            var placeOnlyTexture = GetTexture("UI/Construct/PlaceOnly");
            RefreshTexture = GetTexture("UI/Construct/Refresh");
            BackTexture = GetTexture("UI/Construct/Back");
            ButtonBackgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
            #endregion
            #region 面板初始化
            BasePanel = new(new(29, 34, 70), new(44, 57, 105, 160))
            {
                Top = StyleDimension.FromPixels(256f),
                HAlign = 0.2f
            };
            //BasePanel.SetPos(new Vector2(256, 256));
            BasePanel.SetSize(600f, 0f, precentHeight: 0.6f).SetPadding(12f);
            BasePanel.Draggable = true;
            Append(BasePanel);
            #endregion
            #region 列表初始化
            UIList = new UIList
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPixelsAndPercent(-50, 1f),
                PaddingBottom = 4f,
                PaddingTop = 4f,
                ListPadding = 4f,
                Top = StyleDimension.FromPixels(50)
            };
            UIList.SetPadding(2f);
            UIList.ListPadding = 4f;
            UIList.ManualSortMethod = _ => { }; // 阻止他自动排序
            BasePanel.Append(UIList);
            #endregion
            #region 滑动条初始化
            Scrollbar = new();
            Scrollbar.Left.Set(310f, 0.5f);
            Scrollbar.Top.Set(154f, 0f);
            Scrollbar.Height.Set(-8f, 0.6f);
            Scrollbar.SetView(100f, 1000f);
            //UIList.SetScrollbar(Scrollbar); // 用自己的代码
            SetupScrollBar();
            Append(Scrollbar);
            #endregion
            #region 刷新按钮初始化
            //TODO: hjson
            RefreshButton = QuickButton(RefreshTexture, "{$Mods.ImproveGame.Common.Refresh}");
            RefreshButton.SetPos(new(-296f, 0), 0.5f, 0f);
            RefreshButton.OnMouseDown += (_, _) =>
            {
                CachedConfigDatas.Clear();
                SetupConfigList();
            };
            BasePanel.Append(RefreshButton);
            #endregion
            #region 文件夹开启按钮初始化
            var folderButton = QuickButton(GetTexture("UI/Construct/Folder"), "{$LegacyInterface.110}");
            folderButton.SetPos(new(-246f, 0), 0.5f, 0f);
            folderButton.OnMouseDown += (_, _) => Utils.OpenFolder(SavePath);
            BasePanel.Append(folderButton);
            #endregion
            #region 关闭按钮初始化
            var closeButton = QuickButton(GetTexture("UI/Construct/Close"), "{$LegacyInterface.71}");
            closeButton.SetPos(new(246f, 0), 0.5f, 0f);
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
        private ModImageButton QuickButton(Asset<Texture2D> texture, string hoverText)
        {
            var button = new ModImageButton(texture, Color.White, Color.White);
            button.SetBackgroundImage(ButtonBackgroundTexture);
            button.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder"));
            button.SetSize(44, 44);
            button.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
            button.HoverText = hoverText;
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

            BasePanel.backgroundColor = new(44, 57, 105, 160);
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
                    Save(ConfigurationSwoosh.ConfigSwooshInstance);
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
