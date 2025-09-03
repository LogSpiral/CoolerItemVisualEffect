#if false


using CoolerItemVisualEffect.Config.ConfigSLer;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.ComplexPanel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Utilities;
using static CoolerItemVisualEffect.Config.ConfigSLer.ConfigSLHelper;
using static CoolerItemVisualEffect.Config.ConfigSLer.CoolerPanelInfo;

namespace CoolerItemVisualEffect.Config.ConfigSLer
{
    public enum ConfigTexStyle : byte
    {
        /*原始*/
        Origin,
        /*暗紫*/
        DarkPurple,
        /*深色金属*/
        DarkMetal,
        /*暗黑*/
        Dark,
        Purple,
        Silver,
        Holy
    }

    public class CoolerPanelInfo : ComplexPanelInfo
    {
        public static bool KeepOrigin => currentStyle == ConfigTexStyle.Origin;

        public static Texture2D currentStyleTex => currentStyle != 0 ? GetConfigStyleTex(currentStyle) : null;

        public static ConfigTexStyle currentStyle => ConfigTexStyle.DarkPurple;

        public static Texture2D GetConfigStyleTex(ConfigTexStyle configTexStyle) => ModContent.Request<Texture2D>($"CoolerItemVisualEffect/Config/ConfigTex/Template_{configTexStyle}").Value;

        public override Texture2D StyleTexture
        { get => GetConfigStyleTex(configTexStyle); set { base.StyleTexture = value; Main.NewText("对这货赋值无效"); } }

        public ConfigTexStyle configTexStyle = ConfigTexStyle.DarkPurple;

        public override Rectangle DrawComplexPanel(SpriteBatch spriteBatch)
        {
            if (configTexStyle == 0)
            {
                ConfigElement.DrawPanel2(spriteBatch, destination.TopLeft(), TextureAssets.SettingsPanel.Value, destination.Width, destination.Height, mainColor);
                //spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(960, 560), new Rectangle(0, 0, 1, 1), Color.Red, 0, new Vector2(.5f), 4f, 0, 0);
                return destination;
            }
            else
            {
                return base.DrawComplexPanel(spriteBatch);
            }
        }
    }

    //下面几个都抄写或魔改自qol，感谢Cyril和局长
    public class SUIPanel : UIElement
    {
        public bool Draggable;
        public bool Dragging;
        public Vector2 Offset;
        public float border;
        public bool CalculateBorder;
        public CoolerPanelInfo panelInfo;

        public SUIPanel(float border = 3, bool CalculateBorder = true)
        {
            SetPadding(10f);
            this.border = border;
            this.CalculateBorder = CalculateBorder;
            OnLeftMouseDown += DragStart;
            OnLeftMouseUp += DragEnd;
            panelInfo = new CoolerPanelInfo
            {
                origin = GetDimensions().Size() * .5f,
                glowEffectColor = default,
                mainColor = Color.White
            };
            panelInfo.backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/HotbarRadial_1").Value;
            panelInfo.backgroundFrame = new Rectangle(4, 4, 28, 28);
            panelInfo.backgroundUnitSize = new Vector2(28, 28) * 2f;
        }

        public override void Recalculate()
        {
            base.Recalculate();
            //panelInfo.destination = GetDimensions().ToRectangle();
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimenstions = GetDimensions();
            var rect = dimenstions.ToRectangle();
            if (CalculateBorder)
            {
                rect = Utils.CenteredRectangle(rect.Center.ToVector2(), rect.Size() + new Vector2(border * 2));
            }
            panelInfo.configTexStyle = currentStyle;
            panelInfo.destination = rect;
            panelInfo.DrawComplexPanel(spriteBatch);
            //ConfigurationSwoosh.DrawCoolerPanel(spriteBatch, rect, borderColor, 0, ConfigurationSwoosh.currentStyle);
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

        public override void OnInitialize()
        {
            //panelInfo.backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/HotbarRadial_1").Value;
            //panelInfo.backgroundFrame = new Rectangle(4, 4, 28, 28);
            //panelInfo.backgroundUnitSize = new Vector2(28, 28) * 2f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                panelInfo.mainColor = KeepOrigin ? CoolerUIPanel.BackgroundDefaultUnselectedColor : Color.White;
            }

            if (Dragging)
            {
                Left.Set(Main.mouseX - Offset.X, 0f);
                Top.Set(Main.mouseY - Offset.Y, 0f);
                Recalculate();
                OnDrag?.Invoke(this);
            }
            panelInfo.backgroundColor = Color.Lerp(Color.Purple, Color.Pink, MathF.Sin(Main.GlobalTimeWrappedHourly) * .5f + .5f) * .5f;
        }

        public event ElementEvent OnDrag;
    }

    public class CoolerUIPanel : SUIPanel
    {
        #region 默认配色们

        public static Color BackgroundDefaultSelectedColor => new(89, 116, 213);
        public static Color BackgroundDefaultUnselectedColor => new(39, 46, 100);
        public static Color GlowDefaultSelectedColor => Color.Lerp(Color.Cyan, Color.Blue, .25f);
        public static Color GlowDefaultUnselectedColor => new(62, 80, 146);
        public static Color MainDefaultSelectedColor => Color.White;
        public static Color MainDefaultUnselectedColor => (Color.White * .8f) with { A = 255 };

        #endregion 默认配色们

        public int[] timers = new int[1];

        public int HoverCounter
        {
            get => timers[0];
            set => timers[0] = value;
        }

        public float HoverFactor => MathHelper.SmoothStep(0, 1, HoverCounter / 15f);
        public virtual float OverrideScaler => MathHelper.SmoothStep(1, 1.2f, HoverCounter / 15f);

        public virtual Color ModifyGlowColor(Color color)
        {
            return color;
        }

        public virtual float GlowShakingStrength => 0f;

        public virtual Vector2 OverrideOffsetPosition
        {
            get
            {
                if (!IsMouseHovering) return default;
                Rectangle rect = GetDimensions().ToRectangle();
                Vector2 target = (new Vector2(Main.mouseX, Main.mouseY) - rect.Center()) / new Vector2(rect.Width, rect.Height) * 2;
                Vector2 result = new(MathHelper.SmoothStep(0, 1, Math.Abs(target.X)) * Math.Sign(target.X), MathHelper.SmoothStep(0, 1, Math.Abs(target.Y)) * Math.Sign(target.Y));
                result *= rect.Size();
                result *= new Vector2(0.0625f, 0.25f);
                //float right = (464f - rect.Width) / 2;
                //if (result.X > right) result.X = right;
                return result;
                //Vector2 result = rect.Center();
                //return (new Vector2(Main.mouseX, Main.mouseY) - result) * .0625f;
            }
        }

        public CoolerUIPanel(float border = 3, bool CalculateBorder = true, Vector2 shakeRange = default, Color? glowEffectColorS = null, Color? glowEffectColorU = null, Color? backGroundColorS = null, Color? backGrounColorU = null, Color? mainColorS = null, Color? mainColorU = null) : base(border, CalculateBorder)
        {
            GlowEffectColorS = glowEffectColorS ?? GlowDefaultSelectedColor;
            GlowEffectColorU = glowEffectColorU ?? GlowDefaultUnselectedColor;
            BackGroundColorS = backGroundColorS ?? BackgroundDefaultSelectedColor;
            BackGrounColorU = backGrounColorU ?? BackgroundDefaultUnselectedColor;
            MainColorS = mainColorS ?? MainDefaultSelectedColor;
            MainColorU = mainColorU ?? MainDefaultUnselectedColor;
            ShakeRange = shakeRange;
            //panelInfo.scaler = 1f;
        }

        public Color GlowEffectColorS;
        public Color GlowEffectColorU;
        public Color BackGroundColorS;
        public Color BackGrounColorU;
        public Color MainColorS;
        public Color MainColorU;
        public Vector2 ShakeRange;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverCounter += IsMouseHovering ? 1 : -1;
            HoverCounter = (int)MathHelper.Clamp(HoverCounter, 0, 15);
            panelInfo.configTexStyle = currentStyle;
            //panelInfo.offset = Vector2.Lerp(panelInfo.offset, OverrideOffsetPosition, 0.05f);
            //panelInfo.scaler = MathHelper.Lerp(panelInfo.scaler, OverrideScaler, 0.025f);
            //panelInfo.glowEffectColor = Color.Lerp(panelInfo.glowEffectColor, IsMouseHovering ? GlowEffectColorS : GlowEffectColorU, 0.05f);
            float factor = HoverFactor;
            panelInfo.glowEffectColor = Color.Lerp(GlowEffectColorU, GlowEffectColorS, factor);
            panelInfo.backgroundColor = Color.Lerp(BackGrounColorU, BackGroundColorS, factor);
            panelInfo.mainColor = KeepOrigin ? IsMouseHovering ? BackGroundColorS : BackGrounColorU : Color.Lerp(MainColorU, MainColorS, factor);
            panelInfo.glowShakingStrength = MathHelper.Lerp(ShakeRange.X, ShakeRange.Y, factor);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
        }
    }

    public class ConfigItemPanel : CoolerUIPanel
    {
        #region 文件相关

        public string FilePath { get; private set; }
        public string Name { get; private set; }
        private string _inputName;  // 用于先装输入缓存的
        private bool _renaming;
        private int _cursorTimer;
        private bool _oldMouseLeft;
        private string _selectedButtonName = "";

        #endregion 文件相关

        #region 控件

        public UIText NameText;
        public UIText PathText;
        public UIImageButton RenameButton;
        public CoolerUIPanel PathPanel;

        #endregion 控件

        public ConfigItemPanel(string filePath) : base()
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
            detailButton.OnLeftClick += DetailButtonClick;
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
            deleteButton.OnLeftClick += DeleteButtonClick;
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
            loadConfigButton.OnLeftClick += (ev, e) =>
            {
                Load(MeleeConfig.Instance, FilePath);
            };
            loadConfigButton.OnUpdate += (_) =>
            {
                if (loadConfigButton.IsMouseHovering)
                {
                    _selectedButtonName = "Mods.CoolerItemVisualEffect.ConfigSLer.LoadThisConfig";
                }
            };
            Append(loadConfigButton);

            PathPanel = new(3, false)
            {
                Top = detailButton.Top,
                OverflowHidden = true,
                PaddingLeft = 6f,
                PaddingRight = 6f,
                PaddingBottom = 0f,
                PaddingTop = 0f,
                BackGroundColorS = Color.Transparent,
                BackGrounColorU = Color.Transparent
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
            BackGroundColorS = Color.Cyan with { A = 0 } * .25f;
            BackGrounColorU = Color.Blue with { A = 127 } * .5f;
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
            var mplr = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            foreach (var w in mplr.weaponGroup)
            {
                if (w.BindSequenceName == Name)
                {
                    w.BindSequenceName = _inputName;
                    w.Save(true, false);
                }
            }
            bool flag = false;
            foreach (var pair in mplr.meleeConfigs)
            {
                if (pair.Key == Name)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                var value = mplr.meleeConfigs[Name];
                mplr.meleeConfigs.Remove(Name);
                mplr.meleeConfigs.Add(_inputName, value);
            }
            WeaponSelectorSystem.Instance.WeaponSelectorUI.CacheSetupConfig = true;
            mplr.WeaponGroupSyncing();
            if (File.Exists(FilePath) && configSLUI is not null)
            {
                File.Move(FilePath, newPath);
                configSLUI.CacheSetupConfig = true;
            }
        }

        private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            var mplr = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            foreach (var w in mplr.weaponGroup)
            {
                if (w.BindSequenceName == Name)
                {
                    w.BindSequenceName = "";
                    w.Save(true, false);
                }
            }
            bool flag = false;
            foreach (var pair in mplr.meleeConfigs)
            {
                if (pair.Key == Name)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                var value = mplr.meleeConfigs[Name];
                mplr.meleeConfigs.Remove(Name);
            }
            WeaponSelectorSystem.Instance.WeaponSelectorUI.CacheSetupConfig = true;
            mplr.WeaponGroupSyncing();
            if (File.Exists(FilePath))
                FileUtilities.Delete(FilePath, false);
            if (configSLUI is not null)
                configSLUI.CacheSetupConfig = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
            //if (IsMouseHovering)
            //{
            //    NameText.TextColor = Color.White;
            //}
            //else
            //{
            //    NameText.TextColor = Color.LightGray;
            //}
            NameText.TextColor = Color.Lerp(NameText.TextColor, IsMouseHovering ? Color.White : Color.LightGray, .25f);
            if (ConfigSLSystem.ConstructFilePath == FilePath)
            {
                NameText.TextColor = new(255, 231, 69);
            }
            SetSizedText();
            _oldMouseLeft = Main.mouseLeft;

            //panelInfo.glowShakingStrength = HoverFactor;
            //panelInfo.glowHueOffsetRange = .1f;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

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
            panelInfo.glowEffectColor = panelInfo.glowEffectColor with { A = 0 };
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

        public override void OnInitialize()
        {
            base.OnInitialize();
            //panelInfo.backgroundTexture = null;
        }
    }

    public class CoolerImageButton : UIElement
    {
        public string HoverText = "";
        public Func<Color> DrawColor;
        public Asset<Texture2D> Texture { get; private set; }
        public Color ColorActive;
        public Color ColorInactive;
        public Color currentColor;
        public SoundStyle? PlaySound { get; private set; }
        public ConfigTexStyle configTexStyle;

        public CoolerImageButton(Asset<Texture2D> texture, Color activeColor = default, Color inactiveColor = default)
        {
            if (texture is not null)
            {
                Texture = texture;
                Width.Set(Texture.Width(), 0f);
                Height.Set(Texture.Height(), 0f);
            }
            ColorActive = activeColor;
            ColorInactive = inactiveColor;
            PlaySound = null;
        }

        #region 各种设置方法

        public void SetCenter(int x, int y)
        {
            CalculatedStyle dimensions = GetDimensions();
            Left.Set(x - dimensions.Width / 2, 0f);
            Top.Set(y - dimensions.Height / 2, 0f);
        }

        public void SetSound(SoundStyle sound)
        {
            PlaySound = sound;
        }

        public void SetColor(Color? activeColor = null, Color? inactiveColor = null)
        {
            ColorActive = activeColor ?? ColorActive;
            ColorInactive = inactiveColor ?? ColorActive;
        }

        public void SetImage(Asset<Texture2D> texture, bool changeSize = false)
        {
            Texture = texture;
            if (changeSize)
            {
                Width.Set(Texture.Width(), 0f);
                Height.Set(Texture.Height(), 0f);
            }
        }

        #endregion 各种设置方法

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (!string.IsNullOrEmpty(HoverText))
                {
                    string text = HoverText;
                    if (HoverText.StartsWith("{$") && HoverText.EndsWith("}"))
                    {
                        text = Language.GetTextValue(HoverText.Substring("{$".Length, HoverText.Length - "{$}".Length));
                    }
                    Main.instance.MouseText(text);
                }
            }
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            var mainColor = IsMouseHovering ? ColorActive : ColorInactive;
            if (DrawColor is not null)
            {
                mainColor = DrawColor.Invoke();
            }
            currentColor = Color.Lerp(currentColor, mainColor, 0.05f);
            //drawcoolertex
            if (currentStyleTex != null)
            {
                //var rect = dimensions.ToRectangle();
                //rect = rect.OffsetSize((int)dimensions.Width / 2, (int)(-dimensions.Height / 4));
                //rect.Offset(0, (int)(dimensions.Height / 8));
                //DrawCoolerTextBox_Combine(spriteBatch, currentStyleTex, rect, currentColor);
                var rect = dimensions.ToRectangle().OffsetSize(8, 8);
                rect.Offset(-4, -4);
                var info = new CoolerPanelInfo();
                info.configTexStyle = currentStyle;
                info.destination = rect;
                info.scaler = 1f;
                info.origin = default;
                //info.glowShakingStrength = IsMouseHovering ? .25f : 0f;
                //info.glowHueOffsetRange = .1f;
                //info.glowEffectColor = currentColor;
                info.backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/HotbarRadial_1").Value;
                info.backgroundFrame = new Rectangle(4, 4, 28, 28);
                info.backgroundUnitSize = new Vector2(28, 28) * 2f;
                info.backgroundColor = currentColor;
                info.DrawComplexPanel(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel").Value, dimensions.Center(), null, currentColor, 0f, new Vector2(22), 1f, SpriteEffects.None, 0f);
                if (IsMouseHovering)
                    spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder").Value, dimensions.Center(), null, currentColor, 0f, new Vector2(22), 1f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(Texture.Value, dimensions.Center(), null, Color.White, 0f, Texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (PlaySound.HasValue)
            {
                SoundEngine.PlaySound(PlaySound.Value);
            }
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }
    }

    public class CoolerImageButtonSplit : CoolerImageButton
    {
        public string HoverTextUp;
        public string HoverTextDown;

        public CoolerImageButtonSplit(Asset<Texture2D> texture, Color activeColor = default, Color inactiveColor = default) : base(texture, activeColor, inactiveColor)
        {
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = GetDimensions().ToRectangle();
            if (IsMouseHovering)
            {
                if (Main.mouseY < rectangle.Y + rectangle.Height / 2)
                {
                    UIModConfig.Tooltip = HoverTextUp;
                }
                else
                {
                    UIModConfig.Tooltip = HoverTextDown;
                }
            }
            base.DrawSelf(spriteBatch);
        }
    }

    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class CoolerUIScrollbar : UIElement
    {
        private float viewPosition; // 滚动条当前位置
        private float MaxViewPoisition => maxViewSize - viewSize;

        private float viewSize = 1f; // 显示出来的高度
        private float maxViewSize = 20f; // 控制元素的高度
        private float ViewScale => viewSize / maxViewSize;
        private bool innerHovered;

        // 用于拖动内滚动条
        private float offsetY;

        public bool dragging;

        public bool Visible;
        public int timer;
        public float factor => MathHelper.SmoothStep(0, 1, timer / 15f);

        public float ViewPosition
        {
            get => viewPosition;
            set => viewPosition = MathHelper.Clamp(value, 0f, MaxViewPoisition);
        }

        private float _bufferViewPosition;

        /// <summary>
        /// 缓冲距离, 不想使用动画就直接设置 <see cref="ViewPosition"/>
        /// </summary>
        public float BufferViewPosition
        {
            get => _bufferViewPosition;
            set => _bufferViewPosition = value;
        }

        public CoolerUIScrollbar()
        {
            Visible = true;
            Width.Pixels = 20;
            MaxWidth.Pixels = 20;
            SetPadding(5);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            CalculatedStyle InnerDimensions = GetInnerDimensions();
            CalculatedStyle InnerRectangle = InnerDimensions;
            InnerRectangle.Y += ViewPosition / MaxViewPoisition * (InnerDimensions.Height * (1 - ViewScale));
            InnerRectangle.Height = InnerDimensions.Height * ViewScale;
            if (InnerRectangle.Contains(Main.MouseScreen))
            {
                if (!innerHovered)
                {
                    innerHovered = true;
                    InnerMouseOver();
                }
            }
            else
            {
                if (innerHovered)
                {
                    InnerMouseOut();
                }
                innerHovered = false;
            }
            timer = (int)MathHelper.Clamp(timer, 0, 15);
            base.Update(gameTime);

            if (dragging)
            {
                if (ViewScale != 1)
                    ViewPosition = (Main.MouseScreen.Y - InnerDimensions.Y - offsetY) / (InnerDimensions.Height * (1 - ViewScale)) * MaxViewPoisition;
            }

            if (BufferViewPosition != 0)
            {
                ViewPosition -= BufferViewPosition * 0.2f;
                BufferViewPosition *= 0.8f;
                if (MathF.Abs(BufferViewPosition) < 0.1f)
                {
                    ViewPosition = MathF.Round(ViewPosition, 1);
                    BufferViewPosition = 0;
                }
            }
        }

        public virtual void InnerMouseOver()
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            timer++;
        }

        public virtual void InnerMouseOut()
        {
            timer--;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            if (!Visible)
                return;

            if (evt.Target == this)
            {
                CalculatedStyle InnerDimensions = GetInnerDimensions();

                if (InnerDimensions.Contains(Main.MouseScreen))
                {
                    dragging = true;
                    offsetY = evt.MousePosition.Y - InnerDimensions.Y - InnerDimensions.Height * (1 - ViewScale) * (viewPosition / MaxViewPoisition);
                }
                BufferViewPosition = 0;
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            if (!Visible)
                return;
            dragging = false;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (!Visible)
                return;
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIScrollbar");
        }

        public readonly Color hoveredColor = new(220, 220, 220);

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            CalculatedStyle dimension = GetDimensions();
            var info = new CoolerPanelInfo();
            info.configTexStyle = currentStyle;
            info.destination = dimension.ToRectangle();
            info.scaler = 1f;
            info.origin = default;
            info.backgroundColor = Color.White;
            info.glowShakingStrength = .5f;
            info.glowEffectColor = (Color.Cyan * .25f) with { A = 51 };
            info.mainColor = currentStyle == 0 ? CoolerUIPanel.GlowDefaultUnselectedColor : Color.White;
            info.DrawComplexPanel(spriteBatch);

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 innerPosition = innerDimensions.Position();
            Vector2 innerSize = innerDimensions.Size();
            if (MaxViewPoisition != 0)
                innerPosition.Y += innerDimensions.Height * (1 - ViewScale) * (ViewPosition / MaxViewPoisition);
            innerSize.Y *= ViewScale;
            if (currentStyle != 0)
                ComplexPanelInfo.DrawComplexPanel_Bound(spriteBatch, currentStyleTex, innerPosition + innerSize * .5f, innerSize.Y, innerSize.X / 24f, MathHelper.PiOver2, (Color.Cyan * .5f) with { A = 0 }, 1f, 1, 0);
            else
            {
                var rectangle = Utils.CenteredRectangle(innerPosition + innerSize * .5f, innerSize);
                //ComplexPanelInfo.(spriteBatch, ref rectangle, Color.White, texStyle: 0);
                ConfigElement.DrawPanel2(spriteBatch, rectangle.TopLeft(), TextureAssets.SettingsPanel.Value, rectangle.Width, rectangle.Height, Color.White);
            }
        }

        public void SetView(float viewSize, float maxViewSize)
        {
            viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
            viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);

            this.viewSize = viewSize;
            this.maxViewSize = maxViewSize;
        }
    }

    public class CoolerUITextPanel<T> : UIPanel
    {
        public T _text;

        public float _textScale = 1f;

        public Vector2 _textSize = Vector2.Zero;

        public bool _isLarge;

        public Color _color = Color.White;

        public bool _drawPanel = true;

        public float TextHAlign = 0.5f;

        public bool HideContents;

        public string _asterisks;

        public bool IsLarge => _isLarge;

        public new bool DrawPanel
        {
            get
            {
                return _drawPanel;
            }
            set
            {
                _drawPanel = value;
            }
        }

        public float TextScale
        {
            get
            {
                return _textScale;
            }
            set
            {
                _textScale = value;
            }
        }

        public Vector2 TextSize => _textSize;

        public string Text
        {
            get
            {
                if (_text != null)
                {
                    return _text.ToString();
                }

                return "";
            }
        }

        public Color TextColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public Color ColorActive;
        public Color ColorInactive;
        public Color currentColor;

        public CoolerUITextPanel(T text, float textScale = 1f, bool large = false, Color activeColor = default, Color inactiveColor = default)
        {
            SetText(text, textScale, large);
        }

        public override void Recalculate()
        {
            SetText(_text, _textScale, _isLarge);
            base.Recalculate();
        }

        public void SetText(T text)
        {
            SetText(text, _textScale, _isLarge);
        }

        public virtual void SetText(T text, float textScale, bool large)
        {
            Vector2 stringSize = ChatManager.GetStringSize(large ? FontAssets.DeathText.Value : FontAssets.MouseText.Value, text.ToString(), new Vector2(textScale));
            stringSize.Y = (large ? 32f : 16f) * textScale;
            _text = text;
            _textScale = textScale;
            _textSize = stringSize;
            _isLarge = large;
            MinWidth.Set(stringSize.X + PaddingLeft + PaddingRight, 0f);
            MinHeight.Set(stringSize.Y + PaddingTop + PaddingBottom, 0f);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_drawPanel)
            {
                if (currentStyleTex != null)
                {
                    //var rect = dimensions.ToRectangle();
                    //rect = rect.OffsetSize((int)dimensions.Width / 2, (int)(-dimensions.Height / 4));
                    //rect.Offset(0, (int)(dimensions.Height / 8));
                    //DrawCoolerTextBox_Combine(spriteBatch, currentStyleTex, rect, currentColor);
                    var mainColor = IsMouseHovering ? ColorActive : ColorInactive;
                    currentColor = Color.Lerp(currentColor, mainColor, 0.05f);
                    var rect = GetDimensions().ToRectangle().OffsetSize(8, 8);
                    rect.Offset(-4, -4);
                    var info = new CoolerPanelInfo();
                    info.configTexStyle = currentStyle;
                    info.destination = rect;
                    info.scaler = 1f;
                    info.origin = default;
                    info.backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/HotbarRadial_1").Value;
                    info.backgroundFrame = new Rectangle(4, 4, 28, 28);
                    info.backgroundUnitSize = new Vector2(28, 28) * 2f;
                    info.backgroundColor = currentColor;
                    info.backgroundColor = Color.White;
                    info.DrawComplexPanel(spriteBatch);
                }
                else
                    base.DrawSelf(spriteBatch);
            }

            DrawText(spriteBatch);
        }

        public void DrawText(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 pos = innerDimensions.Position();
            if (_isLarge)
            {
                pos.Y -= 10f * _textScale * _textScale;
            }
            else
            {
                pos.Y -= 2f * _textScale;
            }

            pos.X += (innerDimensions.Width - _textSize.X) * TextHAlign;
            string text = Text;
            if (HideContents)
            {
                if (_asterisks == null || _asterisks.Length != text.Length)
                {
                    _asterisks = new string('*', text.Length);
                }

                text = _asterisks;
            }

            if (_isLarge)
            {
                Utils.DrawBorderStringBig(spriteBatch, text, pos, _color, _textScale);
            }
            else
            {
                Utils.DrawBorderString(spriteBatch, text, pos, _color, _textScale);
            }
        }
    }
}
#endif