/*
using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.ConfigSaveLoader;
using LogSpiralLibrary.CodeLibrary.ConfigModification;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Reflection;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

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
    public CoolerImageButton CreateButton;
    public CoolerImageButton SaveButton;
    public CoolerImageButton RevertButton;
    public CoolerImageButton DefaultButton;
    public bool SetConfigPending;
    private bool ButtonAdded;
    public CoolerImageButton RefreshButton; // 刷新/回退按钮

    // 当前结构的信息
    public MeleeConfig pendingConfig;

    public static void GetConfig(UIElement element, out ModConfig modConfig)
    {
        modConfig = null;
        UIElement pare = element;
        while (pare != null && pare is not ConfigSLUI)
            pare = pare.Parent;
        if (pare is ConfigSLUI configslUI)
            modConfig = configslUI.pendingConfig;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public override void OnInitialize()
    {
        ConfigPreviewSystem.ConfigSettingRegister(GetConfig, () => pendingConfig != null, "CIVE:ConfigSLUI");

        #region 贴图加载

        //var saveTexture = GetTexture("UI/Construct/Save");
        //var loadTexture = GetTexture("UI/Construct/Load");
        //var explodeAndPlaceTexture = GetTexture("UI/Construct/ExplodeAndPlace");
        //var placeOnlyTexture = GetTexture("UI/Construct/PlaceOnly");
        RefreshTexture = GetTexture("UI/Construct/Refresh");
        BackTexture = GetTexture("UI/Construct/Back");
        ButtonBackgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");

        #endregion 贴图加载

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

        #endregion 面板初始化

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

        #endregion 列表初始化

        #region 滑动条初始化

        Scrollbar = new();
        Scrollbar.Left.Set(600f, 0f);
        Scrollbar.Top.Set(8f, 0f);
        Scrollbar.Height.Set(0, 0.975f);
        Scrollbar.Width.Set(32, 0);
        //UIList.SetScrollbar(Scrollbar); // 用自己的代码
        SetupScrollBar();
        BasePanel.Append(Scrollbar);

        #endregion 滑动条初始化

        #region 刷新按钮初始化

        RefreshButton = QuickButton(RefreshTexture, "{$Mods.CoolerItemVisualEffect.ConfigSLer.Refresh}");
        RefreshButton.SetPos(new(-286f, 8), 0.5f, 0f);
        RefreshButton.OnLeftMouseDown += (_, _) =>
        {
            //CachedConfigDatas.Clear();
            SetupConfigList();
            pendingConfig = null;
        };
        BasePanel.Append(RefreshButton);

        #endregion 刷新按钮初始化

        #region 文件夹开启按钮初始化

        var folderButton = QuickButton(GetTexture("UI/Construct/Folder"), "{$LegacyInterface.110}");
        folderButton.SetPos(new(-220f, 8), 0.5f, 0f);
        folderButton.OnLeftMouseDown += (_, _) => Utils.OpenFolder(SavePath);
        BasePanel.Append(folderButton);

        #endregion 文件夹开启按钮初始化

        #region 新建近战设置按钮初始化

        CreateButton = QuickButton(GetTexture("UI/Construct/Create"), "{$Mods.CoolerItemVisualEffect.ConfigSLer.Create}");
        CreateButton.SetPos(new(-154f, 8), 0.5f, 0f);
        CreateButton.OnLeftMouseDown += (_, _) =>
        {
            CacheSetupConfig = true;
            Save(new MeleeConfig());
        };
        BasePanel.Append(CreateButton);

        #endregion 新建近战设置按钮初始化

        #region 保存按钮初始化

        SaveButton = QuickButton(GetTexture("UI/Construct/Save"), "{$Mods.CoolerItemVisualEffect.ConfigSLer.Save}");
        SaveButton.SetPos(new(-88f, 8), 0.5f, 0f);
        SaveButton.OnLeftMouseDown += (_, _) =>
        {
            if (SetConfigPending && pendingConfig != null)
            {
                Save(pendingConfig, CacheConfigInfoPath, false);
                Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().meleeConfigs[Path.GetFileNameWithoutExtension(CacheConfigInfoPath)] = (MeleeConfig)pendingConfig.Clone();
                SyncWeaponGroup.Get().Send();
                BasePanel.RemoveChild(SaveButton);
                BasePanel.RemoveChild(RevertButton);
                BasePanel.RemoveChild(DefaultButton);
                SetConfigPending = false;
                ButtonAdded = false;
                SoundEngine.PlaySound(SoundID.Unlock);
            }
        };
        //BasePanel.Append(createButton);

        #endregion 保存按钮初始化

        #region 撤销按钮初始化

        RevertButton = QuickButton(GetTexture("UI/Construct/Revert"), "{$Mods.CoolerItemVisualEffect.ConfigSLer.Revert}");
        RevertButton.SetPos(new(-22f, 8), 0.5f, 0f);
        RevertButton.OnLeftMouseDown += (_, _) =>
        {
            if (SetConfigPending && pendingConfig != null)
            {
                Load(pendingConfig, CacheConfigInfoPath, false, false);
                BasePanel.RemoveChild(SaveButton);
                BasePanel.RemoveChild(RevertButton);
                BasePanel.RemoveChild(DefaultButton);
                SetConfigPending = false;
                ButtonAdded = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        };
        //BasePanel.Append(createButton);

        #endregion 撤销按钮初始化

        #region 默认按钮初始化

        DefaultButton = QuickButton(GetTexture("UI/Construct/Default"), "{$Mods.CoolerItemVisualEffect.ConfigSLer.Default}");
        DefaultButton.SetPos(new(44f, 8), 0.5f, 0f);
        DefaultButton.OnLeftMouseDown += (_, _) =>
        {
            if (SetConfigPending && pendingConfig != null)
            {
                ConfigManager.Reset(pendingConfig);
                SoundEngine.PlaySound(SoundID.MenuClose);
                //pendingSelector = new WeaponGroup() { Name = pendingSelector.Name };
            }
        };
        //BasePanel.Append(createButton);

        #endregion 默认按钮初始化

        #region 关闭按钮初始化

        var closeButton = QuickButton(GetTexture("UI/Construct/Close"), "{$LegacyInterface.71}");
        closeButton.SetPos(new(243, 8), 0.5f, 0f);
        closeButton.OnLeftMouseDown += (_, _) => Close();
        BasePanel.Append(closeButton);

        #endregion 关闭按钮初始化
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
        button.configTexStyle = CoolerPanelInfo.currentStyle;
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

        if (BasePanel.IsMouseHovering || Scrollbar.IsMouseHovering && Scrollbar.Visible)
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

    public void OnSetConfigElementValue(GenericConfigElement e, bool f)
    {
        SetConfigPending = f;
        if (f && !ButtonAdded)
        {
            BasePanel.Append(SaveButton);
            BasePanel.Append(RevertButton);
            BasePanel.Append(DefaultButton);
            ButtonAdded = true;
        }
        if (!f && ButtonAdded)
        {
            SaveButton.Remove();
            RevertButton.Remove();
            DefaultButton.Remove();
            ButtonAdded = false;
        }
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

        var mConfig = new MeleeConfig();
        Load(mConfig, CacheConfigInfoPath, false, false);
        pendingConfig = mConfig;
        Action<GenericConfigElement, bool> onSetFunc = (e, f) =>
        {
            SetConfigPending = f;
            if (f && !ButtonAdded)
            {
                BasePanel.Append(SaveButton);
                BasePanel.Append(RevertButton);
                BasePanel.Append(DefaultButton);
                ButtonAdded = true;
            }
            if (!f && ButtonAdded)
            {
                SaveButton.Remove();
                RevertButton.Remove();
                DefaultButton.Remove();
                ButtonAdded = false;
            }
        };
        int top = 0;
        int order = 0;
        foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(mConfig))
        {
            if (variable.Name == "UsePreview" || Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)))
                continue;
            UIModConfig.HandleHeader(UIList, ref top, ref order, variable);

            GenericConfigElement.WrapIt(UIList, ref top, variable, mConfig, order++, onSetObj: OnSetConfigElementValue, owner: configSLUI);//var (container, elem) =
        }
        //UIList.Add(QuickTitleText(GetText("NotFinishYet"), 0.5f));

        RefreshButton.SetImage(BackTexture);
        RefreshButton.HoverText = "{$UI.Back}";
        BasePanel.RemoveChild(CreateButton);

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
        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);
        var filePaths = Directory.GetFiles(SavePath);
        foreach (string path in filePaths)
        {
            if (Path.GetExtension(path) == Extension)
            {
                UIList.Add(new ConfigItemPanel(path));//如果有就添加目标
            }
        }
        pendingConfig = null;
        if (CreateButton.Parent == null)
            BasePanel.Append(CreateButton);
        ButtonAdded = false;
        SaveButton.Remove();
        RevertButton.Remove();
        DefaultButton.Remove();

        RefreshButton.SetImage(RefreshTexture);
        RefreshButton.HoverText = "{$Mods.CoolerItemVisualEffect.ConfigSLer.Refresh}";

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
        var vec = Main.MouseScreen;
        vec /= Main.UIScale;
        float zoom = Main.GameZoomTarget * Main.ForcedMinimumZoom;
        vec = (vec - Main.ScreenSize.ToVector2() * .5f) * zoom + Main.ScreenSize.ToVector2() * .5f;
        pendingConfig = null;
        BasePanel.SetPos(vec - Vector2.UnitX * 360);//vec
        SetupConfigList();
    }

    public void Close()
    {
        pendingConfig = null;
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
*/