// using CoolerItemVisualEffect.Config.ConfigSLer;
using CoolerItemVisualEffect.MeleeModify;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.Utilities;
// using static CoolerItemVisualEffect.WeaponGroupSystem;

namespace CoolerItemVisualEffect.Common.WeaponGroup;






//public class WeaponGroupSystem : ModSystem
//{
//    public static string SavePath = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "WeaponGroup");
//    public static string Extension = ".json";

//    public static string GetText(string str, params object[] arg)
//    {
//        return Language.GetTextValue($"Mods.CoolerItemVisualEffect.ConfigSLer.{str}", arg);
//    }

//    public static Asset<Texture2D> GetTexture(string path)
//    {
//        return ModContent.Request<Texture2D>($"CoolerItemVisualEffect/Config/ConfigSLer/{path}", AssetRequestMode.ImmediateLoad);
//    }

//    public WeaponGroupUI WeaponGroupUI;
//    public UserInterface WeaponGroupInterface;
//    public static WeaponGroupSystem Instance;
//    public static string ConstructFilePath;

//    public override void Load()
//    {
//        WeaponGroupUI = new WeaponGroupUI();
//        WeaponGroupInterface = new UserInterface();
//        WeaponGroupUI.Activate();
//        WeaponGroupInterface.SetState(WeaponGroupUI);
//        Instance = this;
//        base.Load();
//    }

//    public override void Unload()
//    {
//        WeaponGroupUI = null;
//        WeaponGroupInterface = null;
//        Instance = null;
//        base.Unload();
//    }

//    public override void UpdateUI(GameTime gameTime)
//    {
//        if (WeaponGroupUI.Visible)
//            WeaponGroupInterface?.Update(gameTime);
//    }

//    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
//    {
//        int inventoryIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
//        if (inventoryIndex != -1)
//        {
//            layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("CoolerItemVisualEffect: WeaponGroup UI", () =>
//            {
//                if (WeaponGroupUI.Visible)
//                    WeaponGroupUI.Draw(Main.spriteBatch);
//                return true;
//            }, InterfaceScaleType.UI));
//        }
//    }
//}

//public class WeaponGroupUI : UIState
//{
//    public static bool Visible { get; private set; }
//    public bool CacheSetupConfig; // 缓存，在下一帧Setup
//    public bool CacheSetupConfigInfos; // 缓存，在下一帧Setup
//    public string CacheConfigInfoPath;

//    private int oldScreenHeight;

//    public Asset<Texture2D> RefreshTexture;
//    public Asset<Texture2D> BackTexture;
//    public Asset<Texture2D> ButtonBackgroundTexture;

//    private SUIPanel BasePanel; // 背景板
//    public CoolerUIScrollbar Scrollbar; // 拖动条
//    public UIList UIList; // 明细列表
//    public CoolerImageButton CreateButton;
//    public CoolerImageButton SaveButton;
//    public CoolerImageButton RevertButton;
//    public CoolerImageButton DefaultButton;
//    public bool SetConfigPending;
//    private bool ButtonAdded;
//    public CoolerImageButton RefreshButton; // 刷新/回退按钮

//    // 当前结构的信息
//    public WeaponGroup pendingSelector;

//    /// <summary>
//    /// 初始化
//    /// </summary>
//    public override void OnInitialize()
//    {
//        #region 贴图加载

//        //var saveTexture = GetTexture("UI/Construct/Save");
//        //var loadTexture = GetTexture("UI/Construct/Load");
//        //var explodeAndPlaceTexture = GetTexture("UI/Construct/ExplodeAndPlace");
//        //var placeOnlyTexture = GetTexture("UI/Construct/PlaceOnly");
//        RefreshTexture = GetTexture("UI/Construct/Refresh");
//        BackTexture = GetTexture("UI/Construct/Back");
//        ButtonBackgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");

//        #endregion 贴图加载

//        #region 面板初始化

//        BasePanel = new()
//        {
//            Top = StyleDimension.FromPixels(256f),
//            HAlign = 0.2f
//        };
//        //BasePanel.SetPos(new Vector2(256, 256));
//        ConfigSLHelper.SetSize(BasePanel, 600f, 0f, precentHeight: 0.75f).SetPadding(12f);
//        BasePanel.Draggable = true;
//        Append(BasePanel);

//        #endregion 面板初始化

//        #region 列表初始化

//        UIList = new UIList
//        {
//            Width = StyleDimension.FromPixelsAndPercent(0, 1f),
//            Height = StyleDimension.FromPixelsAndPercent(-60, 1f),
//            PaddingBottom = 4f,
//            PaddingTop = 4f,
//            ListPadding = 24f,
//            Top = StyleDimension.FromPixels(60)
//        };
//        UIList.SetPadding(2f);
//        UIList.ManualSortMethod = _ => { }; // 阻止他自动排序
//        BasePanel.Append(UIList);

//        #endregion 列表初始化

//        #region 滑动条初始化

//        Scrollbar = new();
//        Scrollbar.Left.Set(600f, 0f);
//        Scrollbar.Top.Set(8f, 0f);
//        Scrollbar.Height.Set(0, 0.975f);
//        Scrollbar.Width.Set(32, 0);
//        //UIList.SetScrollbar(Scrollbar); // 用自己的代码
//        SetupScrollBar();
//        BasePanel.Append(Scrollbar);

//        #endregion 滑动条初始化

//        #region 刷新按钮初始化

//        RefreshButton = QuickButton(RefreshTexture, "{$Mods.CoolerItemVisualEffect.ConfigSLer.Refresh}");
//        RefreshButton.SetPos(new(-286f, 8), 0.5f, 0f);
//        RefreshButton.OnLeftMouseDown += (_, _) =>
//        {
//            //CachedConfigDatas.Clear();
//            SetupConfigList();
//        };
//        BasePanel.Append(RefreshButton);

//        #endregion 刷新按钮初始化

//        #region 文件夹开启按钮初始化

//        var folderButton = QuickButton(GetTexture("UI/Construct/Folder"), "{$LegacyInterface.110}");
//        folderButton.SetPos(new(-220f, 8), 0.5f, 0f);
//        folderButton.OnLeftMouseDown += (_, _) => Utils.OpenFolder(SavePath);
//        BasePanel.Append(folderButton);

//        #endregion 文件夹开启按钮初始化

//        #region 新建筛选器按钮初始化

//        CreateButton = QuickButton(GetTexture("UI/Construct/Create"), "{$Mods.CoolerItemVisualEffect.WeaponGroup.Create}");
//        CreateButton.SetPos(new(-154f, 8), 0.5f, 0f);
//        CreateButton.OnLeftMouseDown += (_, _) =>
//        {
//            CacheSetupConfig = true;
//            var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup;
//            //int i = dict.Count;
//            var selector = new WeaponGroup();
//            //selector.index = i;
//            list.Insert(0, selector);
//            selector.Save();
//            SyncWeaponGroup.Get(Main.myPlayer, list, null).Send();
//        };
//        BasePanel.Append(CreateButton);

//        #endregion 新建筛选器按钮初始化

//        #region 保存按钮初始化

//        SaveButton = QuickButton(GetTexture("UI/Construct/Save"), "{$Mods.CoolerItemVisualEffect.ConfigSLer.Save}");
//        SaveButton.SetPos(new(-88f, 8), 0.5f, 0f);
//        SaveButton.OnLeftMouseDown += (_, _) =>
//        {
//            if (SetConfigPending && pendingSelector != null)
//            {
//                pendingSelector.Save(true);
//                int counter = 0;
//                string name = Path.GetFileNameWithoutExtension(CacheConfigInfoPath);
//                var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup;
//                foreach (var s in list)
//                {
//                    if (s.Name == name)
//                    {
//                        break;
//                    }
//                    counter++;
//                }
//                if (counter < list.Count)
//                    WeaponGroup.Load(list[counter], CacheConfigInfoPath);
//                SyncWeaponGroup.Get(Main.myPlayer, list, null).Send();
//                BasePanel.RemoveChild(SaveButton);
//                BasePanel.RemoveChild(RevertButton);
//                BasePanel.RemoveChild(DefaultButton);
//                SetConfigPending = false;
//                ButtonAdded = false;
//                SoundEngine.PlaySound(SoundID.Unlock);
//            }
//        };
//        //BasePanel.Append(createButton);

//        #endregion 保存按钮初始化

//        #region 撤销按钮初始化

//        RevertButton = QuickButton(GetTexture("UI/Construct/Revert"), "{$Mods.CoolerItemVisualEffect.ConfigSLer.Revert}");
//        RevertButton.SetPos(new(-22f, 8), 0.5f, 0f);
//        RevertButton.OnLeftMouseDown += (_, _) =>
//        {
//            if (SetConfigPending && pendingSelector != null)
//            {
//                WeaponGroup.Load(pendingSelector, CacheConfigInfoPath);
//                BasePanel.RemoveChild(SaveButton);
//                BasePanel.RemoveChild(RevertButton);
//                BasePanel.RemoveChild(DefaultButton);
//                SetConfigPending = false;
//                ButtonAdded = false;
//                SoundEngine.PlaySound(SoundID.MenuClose);
//            }
//        };
//        //BasePanel.Append(createButton);

//        #endregion 撤销按钮初始化

//        #region 默认按钮初始化

//        DefaultButton = QuickButton(GetTexture("UI/Construct/Default"), "{$Mods.CoolerItemVisualEffect.ConfigSLer.Default}");
//        DefaultButton.SetPos(new(44f, 8), 0.5f, 0f);
//        DefaultButton.OnLeftMouseDown += (_, _) =>
//        {
//            if (SetConfigPending && pendingSelector != null)
//            {
//                WeaponGroup.RestoreToDefault(pendingSelector);
//                SoundEngine.PlaySound(SoundID.MenuClose);
//            }
//        };
//        //BasePanel.Append(createButton);

//        #endregion 默认按钮初始化

//        #region 关闭按钮初始化

//        var closeButton = QuickButton(GetTexture("UI/Construct/Close"), "{$LegacyInterface.71}");
//        closeButton.SetPos(new(243, 8), 0.5f, 0f);
//        closeButton.OnLeftMouseDown += (_, _) => Close();
//        BasePanel.Append(closeButton);

//        #endregion 关闭按钮初始化
//    }

//    /// <summary>
//    /// 快速实例化一个按钮
//    /// </summary>
//    /// <param name="texture"></param>
//    /// <param name="hoverText"></param>
//    /// <returns></returns>
//    private CoolerImageButton QuickButton(Asset<Texture2D> texture, string hoverText)
//    {
//        var button = new CoolerImageButton(texture, Color.Cyan with { A = 0 } * .75f, default);
//        ConfigSLHelper.SetSize(button, 44, 44);
//        button.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
//        button.HoverText = hoverText;
//        button.configTexStyle = CoolerPanelInfo.currentStyle;
//        return button;
//    }

//    public override void ScrollWheel(UIScrollWheelEvent evt)
//    {
//        base.ScrollWheel(evt);
//        if (BasePanel.GetOuterDimensions().Contains(evt.MousePosition.ToPoint()) || Scrollbar.GetOuterDimensions().Contains(evt.MousePosition.ToPoint()))
//            Scrollbar.BufferViewPosition += evt.ScrollWheelValue;
//    }

//    /// <summary>
//    /// 绘制这边更新滑动条的位置(?
//    /// </summary>
//    /// <param name="spriteBatch"></param>
//    public override void DrawSelf(SpriteBatch spriteBatch)
//    {
//        //SDFGraphics.Gallery(new Vector2(40, 400), default, new(40, 60), Color.Cyan, 4, Color.Blue, ModAsset.HeatMap_0.Value, Main.GlobalTimeWrappedHourly * .05f, .05f,SDFGraphics.GetMatrix(true));

//        //SDFGraphics.FumoFumoKoishi(new Vector2(200, 400));
//        var innerList = UIList.GetType().GetField("_innerList", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(UIList) as UIElement;
//        if (Scrollbar is not null && innerList is not null)
//        {
//            innerList.Top.Set(-Scrollbar.ViewPosition, 0);
//        }
//        UIList.Recalculate();
//        base.DrawSelf(spriteBatch);
//    }

//    public override void Update(GameTime gameTime)
//    {
//        if (CacheSetupConfig)
//        {
//            SetupConfigList();
//            CacheSetupConfig = false;
//        }
//        if (CacheSetupConfigInfos)
//        {
//            SetupCurrentConfigList();
//            CacheSetupConfigInfos = false;
//        }

//        Recalculate();
//        base.Update(gameTime);

//        if (BasePanel.IsMouseHovering || Scrollbar.IsMouseHovering && Scrollbar.Visible)
//        {
//            if (Scrollbar.Visible)
//            {
//                PlayerInput.LockVanillaMouseScroll("CoolerItemVisualEffect: Config GUI");
//            }
//            Main.LocalPlayer.mouseInterface = true;
//        }

//        if (oldScreenHeight != Main.screenHeight)
//        {
//            SetupScrollBar(false);
//        }

//        oldScreenHeight = Main.screenHeight;
//    }

//    public void OnSetConfigElementValue(GenericConfigElement e, bool f)
//    {
//        SetConfigPending = f;
//        if (f && !ButtonAdded)
//        {
//            BasePanel.Append(SaveButton);
//            BasePanel.Append(RevertButton);
//            BasePanel.Append(DefaultButton);
//            ButtonAdded = true;
//        }
//        if (!f && ButtonAdded)
//        {
//            SaveButton.Remove();
//            RevertButton.Remove();
//            DefaultButton.Remove();
//            ButtonAdded = false;
//        }
//    }

//    /// <summary>
//    /// <br>加载当前设置的详细信息</br>
//    /// <br>未完工，乐</br>
//    /// </summary>
//    public void SetupCurrentConfigList()
//    {
//        if (string.IsNullOrEmpty(CacheConfigInfoPath) || !File.Exists(CacheConfigInfoPath))
//        {
//            SetupConfigList();
//            return;
//        }

//        //WeaponGroup selector = null;
//        //string name = Path.GetFileNameWithoutExtension(CacheConfigInfoPath);
//        //foreach (var s in Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup)
//        //{
//        //    if (s.Name == name)
//        //    {
//        //        selector = s;
//        //        break;
//        //    }
//        //}
//        WeaponGroup selector = WeaponGroup.Load(CacheConfigInfoPath);
//        if (selector == null)
//        {
//            SetupConfigList();
//            return;
//        }

//        #region 放弃乐

//        /*SUIPanel bpanel = new SUIPanel();
//        bpanel.Draggable = false;
//        ConfigSLHelper.SetSize(bpanel, new Vector2(0, 40), 1, 0);
//        UIList.Add(bpanel);

//        UIToggleImage basedOnDefToggle = new UIToggleImage(ModContent.Request<Texture2D>("CoolerItemVisualEffect/Config/WeaponGroupTex/FalseTrue", AssetRequestMode.ImmediateLoad), 48, 32, new Point(48, 0), new Point(0, 0));
//        basedOnDefToggle.SetState(selector.basedOnDefaultCondition);
//        basedOnDefToggle.OnUpdate += _ =>
//        {
//            selector.basedOnDefaultCondition = basedOnDefToggle.IsOn;
//        };

//        UIText blabel = new UIText("你好");
//        blabel.HAlign = 1f;
//        blabel.OnUpdate += _ =>
//        {
//            blabel.SetText("基于默认设置: " + (selector.basedOnDefaultCondition ? "开" : "关"));
//        };

//        bpanel.Append(basedOnDefToggle);
//        bpanel.Append(blabel);

//        SUIPanel spanel = new SUIPanel();
//        spanel.Draggable = false;
//        ConfigSLHelper.SetSize(spanel, new Vector2(0, 40), 1, 0);
//        UIList.Add(spanel);

//        UIToggleImage whiteListToggle = new UIToggleImage(ModContent.Request<Texture2D>("CoolerItemVisualEffect/Config/WeaponGroupTex/BlackWhite", AssetRequestMode.ImmediateLoad), 20, 20, new Point(20, 0), new Point(0, 0));
//        whiteListToggle.HAlign = 0f;
//        whiteListToggle.VAlign = 0f;
//        whiteListToggle.SetState(selector.whiteList);
//        whiteListToggle.OnUpdate += _ =>
//        {
//            selector.whiteList = whiteListToggle.IsOn;
//        };
//        UIText slabel = new UIText("");
//        slabel.HAlign = 1f;
//        slabel.OnUpdate += _ =>
//        {
//            slabel.SetText("模式: " + (selector.whiteList ? "白名单" : "黑名单"));
//        };
//        spanel.Append(whiteListToggle);
//        spanel.Append(slabel);*/
//        //UIList.Add(QuickTitleText(GetText("NotFinishYet"), 0.5f));

//        #endregion 放弃乐

//        Action<GenericConfigElement, bool> onSetFunc = (e, f) =>
//        {
//            SetConfigPending = f;
//            if (f && !ButtonAdded)
//            {
//                BasePanel.Append(SaveButton);
//                BasePanel.Append(RevertButton);
//                BasePanel.Append(DefaultButton);
//                ButtonAdded = true;
//            }
//            if (!f && ButtonAdded)
//            {
//                SaveButton.Remove();
//                RevertButton.Remove();
//                DefaultButton.Remove();
//                ButtonAdded = false;
//            }
//        };
//        UIList.Clear();
//        pendingSelector = selector;
//        int top = 0;
//        int order = 0;
//        foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(selector))
//        {
//            if (variable.Name == "passWord" || Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)))
//                continue;
//            GenericConfigElement.WrapIt(UIList, ref top, variable, selector, order++, onSetObj: OnSetConfigElementValue, owner: WeaponGroupSystem.Instance.WeaponGroupUI);
//        }

//        RefreshButton.SetImage(BackTexture);
//        RefreshButton.HoverText = "{$UI.Back}";
//        BasePanel.RemoveChild(CreateButton);

//        Recalculate();
//        SetupScrollBar();
//    }

//    private void SetupScrollBar(bool resetViewPosition = true)
//    {
//        float height = UIList.GetInnerDimensions().Height;
//        float totalHeight = UIList.GetTotalHeight();
//        Scrollbar.SetView(height, totalHeight);
//        if (resetViewPosition)
//            Scrollbar.ViewPosition = 0f;

//        Scrollbar.Visible = true;
//        if (height >= totalHeight)
//        {
//            Scrollbar.Visible = false;
//        }
//    }

//    /// <summary>
//    /// 加载设置项并且设置滑动条
//    /// </summary>
//    public void SetupConfigList()
//    {
//        UIList.Clear();//清空
//        if (!Directory.Exists(SavePath))
//            Directory.CreateDirectory(SavePath);

//        var tablePath = Path.Combine(SavePath, "indexTable.txt");
//        if (File.Exists(tablePath))
//        {
//            var indexTable = File.ReadAllLines(tablePath);
//            foreach (string path in indexTable)
//            {
//                var pth = Path.Combine(SavePath, path + Extension);
//                if (File.Exists(pth))
//                    UIList.Add(new WeaponGroupInfoPanel(pth));//如果有就添加目标
//            }
//        }
//        pendingSelector = null;
//        if (CreateButton.Parent == null)
//            BasePanel.Append(CreateButton);
//        ButtonAdded = false;
//        SaveButton.Remove();
//        RevertButton.Remove();
//        DefaultButton.Remove();

//        RefreshButton.SetImage(RefreshTexture);
//        RefreshButton.HoverText = "{$Mods.CoolerItemVisualEffect.ConfigSLer.Refresh}";

//        Recalculate();
//        SetupScrollBar();
//    }

//    /// <summary>
//    /// 开启ui,加载设置表
//    /// </summary>
//    public void Open()
//    {
//        Visible = true;
//        SoundEngine.PlaySound(SoundID.MenuOpen);

//        var vec = Main.MouseScreen;
//        vec /= Main.UIScale;
//        float zoom = Main.GameZoomTarget * Main.ForcedMinimumZoom;
//        vec = (vec - Main.ScreenSize.ToVector2() * .5f) * zoom + Main.ScreenSize.ToVector2() * .5f;

//        BasePanel.SetPos(vec - Vector2.UnitX * 360);//vec
//        SetupConfigList();
//    }

//    public void Close()
//    {
//        pendingSelector = null;
//        Visible = false;
//        Main.blockInput = false;
//        SoundEngine.PlaySound(SoundID.MenuClose);
//    }

//    private static UIText QuickTitleText(string text, float originY, float originX = 0.5f) => new(text, 0.6f, true)
//    {
//        Height = StyleDimension.FromPixels(50f),
//        Width = StyleDimension.FromPercent(1f),
//        TextOriginX = originX,
//        TextOriginY = originY
//    };
//}

//public class WeaponGroupInfoPanel : CoolerUIPanel
//{
//    #region 文件相关

//    public string FilePath { get; private set; }
//    public string Name { get; private set; }
//    private string _inputName;  // 用于先装输入缓存的
//    private bool _renaming;
//    private int _cursorTimer;
//    private bool _oldMouseLeft;
//    private string _selectedButtonName = "";

//    #endregion 文件相关

//    #region 控件

//    public UIText NameText;
//    public UIText PathText;
//    public UIImageButton RenameButton;
//    public CoolerUIPanel PathPanel;

//    #endregion 控件

//    public WeaponGroupInfoPanel(string filePath) : base()
//    {
//        FilePath = filePath;

//        _oldMouseLeft = true;

//        Width = StyleDimension.FromPixels(580f);

//        string name = FilePath.Split('\\').Last();
//        name = name[..^Extension.Length]; // name.Substring(0, name.Length - FileOperator.Extension.Length)
//        Name = name;
//        NameText = new(name, 1.05f)
//        {
//            Left = StyleDimension.FromPixels(2f),
//            Height = StyleDimension.FromPixels(24f)
//        };
//        Append(NameText);

//        var buttonNameText = new UIText("")
//        {
//            Left = StyleDimension.FromPercent(1f),
//            Top = StyleDimension.FromPixels(4f),
//            Height = StyleDimension.FromPixels(20f)
//        };
//        buttonNameText.OnUpdate += (_) =>
//        {
//            string text = Language.GetTextValue(_selectedButtonName);
//            var font = FontAssets.MouseText.Value;
//            buttonNameText.SetText(text);
//            buttonNameText.Left = new StyleDimension(-font.MeasureString(text).X, 1f);
//            _selectedButtonName = "";
//        };
//        Append(buttonNameText);

//        UIHorizontalSeparator separator = new()
//        {
//            Top = StyleDimension.FromPixels(NameText.Height.Pixels - 2f),
//            Width = StyleDimension.FromPercent(1f),
//            Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
//        };
//        Append(separator);

//        var detailButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"))
//        {
//            Top = new StyleDimension(separator.Height.Pixels + separator.Top.Pixels + 3f, 0f),
//            Left = new StyleDimension(-20f, 1f)
//        };
//        ConfigSLHelper.SetSize(detailButton, 24f, 24f);
//        detailButton.OnLeftClick += DetailButtonClick;
//        detailButton.OnUpdate += (_) =>
//        {
//            if (detailButton.IsMouseHovering)
//            {
//                _selectedButtonName = "tModLoader.ModsMoreInfo";
//            }
//        };
//        Append(detailButton);

//        var deleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"))
//        {
//            Top = detailButton.Top,
//            Left = new StyleDimension(detailButton.Left.Pixels - 24f, 1f)
//        };
//        ConfigSLHelper.SetSize(deleteButton, 24f, 24f);
//        deleteButton.OnLeftClick += DeleteButtonClick;
//        deleteButton.OnUpdate += (_) =>
//        {
//            if (deleteButton.IsMouseHovering)
//            {
//                _selectedButtonName = "UI.Delete";
//            }
//        };
//        Append(deleteButton);

//        RenameButton = new(Main.Assets.Request<Texture2D>("Images/UI/ButtonRename"))
//        {
//            Top = detailButton.Top,
//            Left = new StyleDimension(deleteButton.Left.Pixels - 24f, 1f)
//        };
//        ConfigSLHelper.SetSize(RenameButton, 24f, 24f);
//        RenameButton.OnUpdate += (_) =>
//        {
//            if (RenameButton.IsMouseHovering)
//            {
//                _selectedButtonName = "UI.Rename";
//            }
//        };
//        Append(RenameButton);

//        var upDownButton = new UIImageButton(UICommon.ButtonUpDownTexture)
//        {
//            Top = detailButton.Top,
//            Left = new StyleDimension(RenameButton.Left.Pixels - 24f, 1f)
//        };
//        ConfigSLHelper.SetSize(upDownButton, 24f, 24f);
//        upDownButton.OnLeftClick += (ev, e) =>
//        {
//            Rectangle r = e.GetDimensions().ToRectangle();
//            var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup;
//            int index = 0;
//            foreach (var s in list)
//            {
//                if (s.Name == Name)
//                    break;
//                index++;
//            }
//            if (index >= list.Count) return;
//            if (ev.MousePosition.Y < r.Y + r.Height / 2)
//            {
//                if (index > 0)
//                {
//                    var dummy = list[index];
//                    list.RemoveAt(index);
//                    list.Insert(index - 1, dummy);
//                    goto label;
//                }
//            }
//            else
//            {
//                if (index < list.Count - 1)
//                {
//                    var dummy = list[index];
//                    list.RemoveAt(index);
//                    list.Insert(index + 1, dummy);
//                    goto label;
//                }
//            }
//            return;
//        label:
//            //Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroupSyncing();
//            SyncWeaponGroup.Get(Main.myPlayer, list, null);
//            string indexTable = "";
//            //int counter = 0;
//            foreach (var pair in list)
//            {
//                indexTable += $"{pair.Name}\n";// >> {counter}
//                                               //counter++;
//            }
//            File.WriteAllText(Path.Combine(SavePath, "indexTable.txt"), indexTable);
//            if (WeaponGroupSystem.Instance.WeaponGroupUI is not null)
//                WeaponGroupSystem.Instance.WeaponGroupUI.CacheSetupConfig = true;
//        };
//        Append(upDownButton);

//        PathPanel = new(3, false)
//        {
//            Top = detailButton.Top,
//            OverflowHidden = true,
//            PaddingLeft = 6f,
//            PaddingRight = 6f,
//            PaddingBottom = 0f,
//            PaddingTop = 0f,
//            BackGroundColorS = Color.Transparent,
//            BackGrounColorU = Color.Transparent
//        };
//        ConfigSLHelper.SetSize(PathPanel, new(Width.Pixels + upDownButton.Left.Pixels - 44f, 23f));
//        Append(PathPanel);
//        PathText = new($"{GetText("Path")}{FilePath}", 0.7f)
//        {
//            Left = StyleDimension.FromPixels(2f),
//            HAlign = 0f,
//            VAlign = 0.5f,
//            TextColor = Color.Gray
//        };
//        PathPanel.Append(PathText);
//        SetSizedText();
//        BackGroundColorS = Color.Cyan with { A = 0 } * .25f;
//        BackGrounColorU = Color.Blue with { A = 127 } * .5f;
//        Height = StyleDimension.FromPixels(PathPanel.Top.Pixels + PathPanel.Height.Pixels + 22f);
//    }

//    private void DetailButtonClick(UIMouseEvent evt, UIElement listeningElement)
//    {
//        var ui = WeaponGroupSystem.Instance.WeaponGroupUI;
//        if (ui is not null)
//        {
//            ui.CacheSetupConfigInfos = true;
//            ui.CacheConfigInfoPath = FilePath;
//        }
//    }

//    private void RenameButtonClick()
//    {
//        _inputName = Name;
//        _renaming = true;
//        Main.blockInput = true;
//        Main.clrInput();
//        SoundEngine.PlaySound(SoundID.MenuTick);
//    }

//    private void EndRename()
//    {
//        if (_inputName == "") return;
//        _renaming = false;
//        Main.blockInput = false;
//        string newPath = FilePath.Replace(Name, _inputName);
//        var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup;

//        if (File.Exists(newPath) && Name != _inputName)
//        {
//            Main.NewText(GetText("RenameTip.Exists"));
//            NameText.SetText(Name);
//            return;
//        }
//        foreach (var s in list)
//        {
//            if (s.Name == Name)
//                s.Name = _inputName;
//        }
//        string indexTable = "";
//        //int counter = 0;
//        foreach (var pair in list)
//        {
//            indexTable += $"{pair.Name}\n";// >> {counter}
//            //counter++;
//        }
//        Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroupSyncing();
//        File.WriteAllText(Path.Combine(SavePath, "indexTable.txt"), indexTable);
//        if (File.Exists(FilePath) && WeaponGroupSystem.Instance.WeaponGroupUI is not null)
//        {
//            File.Move(FilePath, newPath);
//            WeaponGroupSystem.Instance.WeaponGroupUI.CacheSetupConfig = true;
//        }
//    }

//    private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
//    {
//        SoundEngine.PlaySound(SoundID.MenuTick);
//        if (File.Exists(FilePath))
//            FileUtilities.Delete(FilePath, false);
//        if (WeaponGroupSystem.Instance.WeaponGroupUI is not null)
//            WeaponGroupSystem.Instance.WeaponGroupUI.CacheSetupConfig = true;
//        var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup;
//        //int i = -1;
//        foreach (var s in list)
//        {
//            if (s.Name == Name)
//            {
//                list.Remove(s);
//                //i = s.index;
//                break;
//            }
//        }
//        Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroupSyncing();
//        string indexTable = "";
//        //int counter = 0;
//        foreach (var pair in list)
//        {
//            indexTable += $"{pair.Name}\n";// >> {counter}
//            //counter++;
//        }
//        File.WriteAllText(Path.Combine(SavePath, "indexTable.txt"), indexTable);

//        //foreach (var s in dict.Keys)
//        //    if (s.index > i) s.index--;
//    }

//    public override void Update(GameTime gameTime)
//    {
//        base.Update(gameTime);
//        _cursorTimer++;
//        _cursorTimer %= 60;
//        if (Main.mouseLeft && !_oldMouseLeft)
//        {
//            var renameDimensions = RenameButton.GetOuterDimensions();
//            if (renameDimensions.ToRectangle().Contains(Main.MouseScreen.ToPoint()) && !_renaming)
//            {
//                RenameButtonClick();
//            }
//            else if (_renaming)
//            {
//                EndRename();
//            }
//        }
//        //if (IsMouseHovering)
//        //{
//        //    NameText.TextColor = Color.White;
//        //}
//        //else
//        //{
//        //    NameText.TextColor = Color.LightGray;
//        //}
//        NameText.TextColor = Color.Lerp(NameText.TextColor, IsMouseHovering ? Color.White : Color.LightGray, .25f);
//        if (ConfigSLSystem.ConstructFilePath == FilePath)
//        {
//            NameText.TextColor = new(255, 231, 69);
//        }
//        SetSizedText();
//        _oldMouseLeft = Main.mouseLeft;

//        //panelInfo.glowShakingStrength = HoverFactor;
//        //panelInfo.glowHueOffsetRange = .1f;
//    }

//    public override void LeftMouseDown(UIMouseEvent evt)
//    {
//        base.LeftMouseDown(evt);

//        if (Children.Any(i => i is UIImageButton && i.IsMouseHovering))
//            return;

//        SoundEngine.PlaySound(SoundID.MenuTick);
//        if (ConfigSLSystem.ConstructFilePath == FilePath)
//        {
//            ConfigSLSystem.ConstructFilePath = string.Empty;
//            return;
//        }

//        if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
//            return;

//        //var tag = FileOperator.GetTagFromFile(FilePath);

//        //if (tag is null)
//        //{
//        //    return;
//        //}

//        //ConfigSLSystem.ConstructFilePath = FilePath;
//        //PreviewRenderer.ResetPreviewTarget = PreviewRenderer.ResetState.WaitReset;
//        //int width = tag.GetShort("Width");
//        //int height = tag.GetShort("Height");
//        //PreviewRenderer.PreviewTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width * 16 + 20, height * 16 + 20, false, default, default, default, RenderTargetUsage.PreserveContents);
//    }

//    public override void Draw(SpriteBatch spriteBatch)
//    {
//        // 放Update，不行。放Draw，行！ReLogic我囸你_
//        if (_renaming)
//        {
//            PlayerInput.WritingText = true;
//            Main.instance.HandleIME();

//            var inputText = Main.GetInputText(_inputName);
//            if (inputText.Length > 40)
//            {
//                inputText = inputText[..40];
//                Main.NewText(GetText("RenameTip.TooLong"));
//            }
//            if (inputText.Contains('\\') || inputText.Contains('/') || inputText.Contains(':') || inputText.Contains('*') || inputText.Contains('?') || inputText.Contains('\"') || inputText.Contains('\'') || inputText.Contains('<') || inputText.Contains('>') || inputText.Contains('|'))
//            {
//                Main.NewText(GetText("RenameTip.Illegal"));
//                return;
//            }
//            else
//            {
//                _inputName = inputText;
//                NameText.SetText(_inputName + (_cursorTimer >= 30 ? "|" : ""));
//                NameText.Recalculate();
//            }

//            // Enter 或者 Esc
//            if (KeyTyped(Keys.Enter) || KeyTyped(Keys.Tab) || KeyTyped(Keys.Escape))
//            {
//                EndRename();
//            }
//        }
//        panelInfo.glowEffectColor = panelInfo.glowEffectColor with { A = 0 };
//        base.Draw(spriteBatch);
//    }

//    public void SetSizedText()
//    {
//        string pathString = GetText("Path");
//        var innerDimensions = PathPanel.GetInnerDimensions();
//        var font = FontAssets.MouseText.Value;
//        float scale = 0.7f;
//        float dotWidth = font.MeasureString("...").X * scale;
//        float pathWidth = font.MeasureString(pathString).X * scale;
//        if (font.MeasureString(FilePath).X * scale >= innerDimensions.Width - 6f - pathWidth - dotWidth)
//        {
//            float width = 0f;
//            int i;
//            for (i = FilePath.Length - 1; i > 0; i--)
//            {
//                width += font.MeasureString(FilePath[i].ToString()).X * scale;
//                if (width >= innerDimensions.Width - 6f - pathWidth - dotWidth)
//                {
//                    break;
//                }
//            }
//            PathText.SetText($"{pathString}...{FilePath[i..]}");
//        }
//    }

//    public static bool KeyTyped(Keys key) => Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);

//    public override void OnInitialize()
//    {
//        base.OnInitialize();
//        //panelInfo.backgroundTexture = null;
//    }
//}


