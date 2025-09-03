// using CoolerItemVisualEffect.Config.ConfigSLer;
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
// using static CoolerItemVisualEffect.WeaponSelectorSystem;

namespace CoolerItemVisualEffect;

public class WeaponSelector
{
    //public int index;
    //public class WeaponGroup
    //{
    //    public List<string> targetWeapons = [];
    //}
    //private class WeaponGroupElement : GenericConfigElement<List<string>>
    //{
    //    private bool expanded = false;
    //    protected UIElement DataListElement { get; set; }
    //    protected NestedUIList DataList { get; set; }
    //    private UIModConfigHoverImage expandButton;
    //    private bool pendingChanges = false;

    //    public override void OnBind()
    //    {
    //        base.OnBind();
    //        MaxHeight.Set(300, 0f);
    //        DataListElement = new UIElement();
    //        DataListElement.Width.Set(-10f, 1f);
    //        DataListElement.Left.Set(10f, 0f);
    //        DataListElement.Height.Set(-30, 1f);
    //        DataListElement.Top.Set(30f, 0f);

    //        Append(DataListElement);

    //        DataListElement.OverflowHidden = true;

    //        DataList = [];
    //        DataList.Width.Set(-20, 1f);
    //        DataList.Left.Set(0, 0f);
    //        DataList.Height.Set(0, 1f);
    //        DataList.ListPadding = 30f;
    //        DataListElement.Append(DataList);

    //        SetupList();

    //        expandButton = new UIModConfigHoverImage(expanded ? ExpandedTexture : CollapsedTexture, expanded ? Language.GetTextValue("tModLoader.ModConfigCollapse") : Language.GetTextValue("tModLoader.ModConfigExpand"));
    //        expandButton.Top.Set(4, 0f); // 10, -25: 4, -52
    //        expandButton.Left.Set(-79, 1f);
    //        expandButton.OnLeftClick += (a, b) =>
    //        {
    //            expanded = !expanded;
    //            pendingChanges = true;
    //        };

    //        pendingChanges = true;
    //        Recalculate(); // Needed?
    //    }

    //    public override void Update(GameTime gameTime)
    //    {
    //        base.Update(gameTime);

    //        if (!pendingChanges)
    //            return;
    //        SetupList();
    //        pendingChanges = false;
    //        RemoveChild(expandButton);
    //        RemoveChild(DataListElement);

    //        Append(expandButton);
    //        if (expanded)
    //        {
    //            Append(DataListElement);
    //            expandButton.HoverText = Language.GetTextValue("tModLoader.ModConfigCollapse");
    //            expandButton.SetImage(ExpandedTexture);
    //        }
    //        else
    //        {
    //            expandButton.HoverText = Language.GetTextValue("tModLoader.ModConfigExpand");
    //            expandButton.SetImage(CollapsedTexture);
    //        }
    //    }

    //    private void SetupList()
    //    {
    //        DataList.Clear();//清空
    //        var list = Value;
    //        int countRow = list.Count / 8 + 1;
    //        for (int n = 0; n < countRow; n++)
    //        {
    //            int countRom = (n == countRow - 1 ? list.Count % 8 : 8) + 1;
    //            SUIPanel panel = new();
    //            panel.Draggable = false;
    //            panel.Width.Set(0, 1);
    //            panel.Height.Set(60, 0);
    //            DataList.Add(panel);
    //            for (int k = 0; k < countRom; k++)
    //            {
    //                int type = 0;
    //                int index = 8 * n + k;
    //                if (index < list.Count)
    //                {
    //                    var str = list[index];
    //                    type = int.TryParse(str, out var t) ? t : ModContent.TryFind<ModItem>(str, out var result) ? result.Type : 0;
    //                }
    //                var btn = QuickButton(TextureAssets.Item[type], "");
    //                btn.Left.Set(66 * k, 0);

    //                btn.OnLeftClick += (_, elem) =>
    //                {
    //                    if (Main.mouseItem.type != ItemID.None)
    //                    {
    //                        if (Main.mouseItem != null && Main.mouseItem.damage > 0 && Main.mouseItem.useTime > 0)
    //                        {
    //                            var content = Main.mouseItem.ModItem?.FullName ?? Main.mouseItem.type.ToString();
    //                            if (!list.Contains(content))
    //                            {
    //                                if (index < list.Count)
    //                                {
    //                                    list[index] = content;
    //                                    (elem as CoolerImageButton).SetImage(TextureAssets.Item[Main.mouseItem.type]);
    //                                    (elem as CoolerImageButton).Recalculate();
    //                                }
    //                                else
    //                                {
    //                                    list.Add(content);
    //                                    SetupList();
    //                                }
    //                                InternalOnSetObject();
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (index < list.Count)
    //                        {
    //                            list.RemoveAt(index);
    //                            SetupList();
    //                            InternalOnSetObject();
    //                        }
    //                    }
    //                };
    //                panel.Append(btn);
    //            }
    //        }
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

    //    public override void Recalculate()
    //    {
    //        base.Recalculate();

    //        float defaultHeight = 30;
    //        float h = DataListElement.Parent != null ? DataList.GetTotalHeight() + defaultHeight : defaultHeight; // 24 for UIElement

    //        h = Utils.Clamp(h, 30, 300);

    //        MaxHeight.Set(300, 0f);
    //        Height.Set(h, 0f);

    //        if (Parent != null && Parent is UISortableElement)
    //        {
    //            Parent.Height.Set(h, 0f);
    //        }
    //    }
    //}

    //private class BindSequenceElement : GenericConfigElement<string>
    //{
    //    private bool expanded = false;
    //    protected UIElement DataListElement { get; set; }
    //    protected NestedUIList DataList { get; set; }
    //    private UIModConfigHoverImage expandButton;
    //    private bool pendingChanges = false;

    //    public override void OnBind()
    //    {
    //        base.OnBind();
    //        TextDisplayFunction = () => Label + (Value == null ? "" : ": " + Value.ToString());
    //        MaxHeight.Set(300, 0f);
    //        DataListElement = new UIElement();
    //        DataListElement.Width.Set(-10f, 1f);
    //        DataListElement.Left.Set(10f, 0f);
    //        DataListElement.Height.Set(-30, 1f);
    //        DataListElement.Top.Set(30f, 0f);

    //        Append(DataListElement);

    //        DataListElement.OverflowHidden = true;

    //        DataList = [];
    //        DataList.Width.Set(-20, 1f);
    //        DataList.Left.Set(0, 0f);
    //        DataList.Height.Set(0, 1f);
    //        DataList.ListPadding = 30f;
    //        DataListElement.Append(DataList);

    //        SetupList();

    //        expandButton = new UIModConfigHoverImage(expanded ? ExpandedTexture : CollapsedTexture, expanded ? Language.GetTextValue("tModLoader.ModConfigCollapse") : Language.GetTextValue("tModLoader.ModConfigExpand"));
    //        expandButton.Top.Set(4, 0f); // 10, -25: 4, -52
    //        expandButton.Left.Set(-79, 1f);
    //        expandButton.OnLeftClick += (a, b) =>
    //        {
    //            expanded = !expanded;
    //            pendingChanges = true;
    //        };

    //        pendingChanges = true;
    //        Recalculate(); // Needed?
    //    }

    //    public override void Update(GameTime gameTime)
    //    {
    //        base.Update(gameTime);

    //        if (!pendingChanges)
    //            return;
    //        SetupList();
    //        pendingChanges = false;
    //        RemoveChild(expandButton);
    //        RemoveChild(DataListElement);

    //        Append(expandButton);
    //        if (expanded)
    //        {
    //            Append(DataListElement);
    //            expandButton.HoverText = Language.GetTextValue("tModLoader.ModConfigCollapse");
    //            expandButton.SetImage(ExpandedTexture);
    //        }
    //        else
    //        {
    //            expandButton.HoverText = Language.GetTextValue("tModLoader.ModConfigExpand");
    //            expandButton.SetImage(CollapsedTexture);
    //        }
    //    }

    //    private void SetupList()
    //    {
    //        DataList.Clear();//清空
    //        CoolerUITextPanel<LocalizedText> curConfigPanel = new(Language.GetOrRegister("Mods.CoolerItemVisualEffect.WeaponSelector.CurrentConfig"));
    //        curConfigPanel.OnLeftClick += (_, _) =>
    //        {
    //            Value = "";
    //            SoundEngine.PlaySound(SoundID.Unlock);
    //        };
    //        DataList.Add(curConfigPanel);//如果有就添加目标

    //        if (!Directory.Exists(ConfigSLHelper.SavePath))
    //            Directory.CreateDirectory(ConfigSLHelper.SavePath);
    //        var filePaths = Directory.GetFiles(ConfigSLHelper.SavePath);
    //        foreach (string path in filePaths)
    //        {
    //            if (Path.GetExtension(path) == Extension)
    //            {
    //                CoolerUITextPanel<string> configPanel = new(Path.GetFileNameWithoutExtension(path));
    //                configPanel.OnLeftClick += (_, elem) =>
    //                {
    //                    Value = Path.GetFileNameWithoutExtension((elem as CoolerUITextPanel<string>).Text);
    //                    SoundEngine.PlaySound(SoundID.Unlock);
    //                };
    //                DataList.Add(configPanel);//如果有就添加目标
    //            }
    //        }
    //    }

    //    public override void Recalculate()
    //    {
    //        base.Recalculate();

    //        float defaultHeight = 30;
    //        float h = DataListElement.Parent != null ? DataList.GetTotalHeight() + defaultHeight : defaultHeight; // 24 for UIElement

    //        h = Utils.Clamp(h, 30, 300);

    //        MaxHeight.Set(300, 0f);
    //        Height.Set(h, 0f);

    //        if (Parent != null && Parent is UISortableElement)
    //        {
    //            Parent.Height.Set(h, 0f);
    //        }
    //    }
    //}

    [JsonIgnore]
    public string Name;

    private const string key = "$Mods.CoolerItemVisualEffect.WeaponSelector.";

    [LabelKey($"{key}basedOnDefaultCondition.Label")]
    public bool basedOnDefaultCondition;

    [LabelKey($"{key}whiteList.Label")]
    [DefaultValue(true)]
    public bool whiteList = true;

    [LabelKey($"{key}BindConfigName.Label")]
    // [CustomGenericConfigItem<BindSequenceElement>]
    public string BindSequenceName;

    [LabelKey($"{key}weaponGroup.Label")]
    // [CustomGenericConfigItem<WeaponGroupElement>]
    public List<string> weaponGroup = [];

    public WeaponSelector()
    {
        Name = Language.GetOrRegister("Mods.CoolerItemVisualEffect.WeaponSelector.DefaultName").Value;
    }

    [JsonIgnore]
    public IEnumerable<int> targetTypes => from str in weaponGroup select int.TryParse(str, out var index) ? index : ModContent.TryFind<ModItem>(str, out var result) ? result.Type : 0;

    public bool CheckAvailabe(Item item)
    {
        if (item.type == ItemID.None) return false;
        bool defaultCondition = !whiteList;
        if (basedOnDefaultCondition)
            defaultCondition = MeleeModifyPlayer.MeleeBroadSwordCheck(item);
        if (whiteList)
            return (defaultCondition || targetTypes.Contains(item.type)) && item.damage > 0;
        else
            return defaultCondition && !targetTypes.Contains(item.type) && item.damage > 0;
    }

    public void Save(bool overWrite = false, bool resetIndexNeeded = true)
    {
#if false
        if (Name == "")
        {
            Main.NewText("禁止空名  Plz give it a name.", Color.Red);
            return;
        }
        var ModConfigPath = SavePath;
        Directory.CreateDirectory(ModConfigPath);
        string filename = Name;
        string resultName = filename + Extension;
        string thisPath = Path.Combine(SavePath, resultName);
        int maxCount = 30;
        bool sameDefault = false;
        if (overWrite) goto label;
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
    label:
        string json = JsonConvert.SerializeObject(this, ConfigManager.serializerSettings);
        Name = Path.GetFileNameWithoutExtension(thisPath);
        File.WriteAllText(thisPath, json);

        if (resetIndexNeeded)
        {
            string indexTable = "";
            //int counter = 0;
            foreach (var pair in Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup)
            {
                indexTable += $"{pair.Name}\n";// >> {counter}
                                               //counter++;
            }
            File.WriteAllText(Path.Combine(SavePath, "indexTable.txt"), indexTable);
        }

        if (!overWrite)
            Main.NewText(GetText("SavedAs") + thisPath, Color.Yellow);

        //if (ConfigSLUI.Visible && configSLUI is not null)
        //{
        //    configSLUI.CacheSetupConfig = true;
        //    if (ConfigSLSystem.ConstructFilePath == thisPath)
        //        ConfigSLSystem.ConstructFilePath = string.Empty;
        //}
#endif
    }

    public static WeaponSelector Load(string path)
    {
        var result = new WeaponSelector();
        Load(result, path);
        result.Name = Path.GetFileNameWithoutExtension(path);
        return result;
    }

    public static void Load(WeaponSelector weaponSelector, string path) => JsonConvert.PopulateObject(File.ReadAllText(path), weaponSelector, ConfigManager.serializerSettings);

    public static void RestoreToDefault(WeaponSelector weaponSelector) => JsonConvert.PopulateObject("{}", weaponSelector, ConfigManager.serializerSettings);
}

public class WeaponSelectorItem : ModItem
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
        //if (player.itemAnimation == player.itemAnimationMax)
        //{
        //    if (WeaponSelectorUI.Visible)
        //        Instance.WeaponSelectorUI.Close();
        //    else
        //        Instance.WeaponSelectorUI.Open();
        //}
    }

    public override bool AltFunctionUse(Player player) => true;

    public override void AddRecipes()
    {
        // CreateRecipe().AddCondition(ConfigSLer.EmptyHandCondition).Register();
    }
}


//public class WeaponSelectorSystem : ModSystem
//{
//    public static string SavePath = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "WeaponSelector");
//    public static string Extension = ".json";

//    public static string GetText(string str, params object[] arg)
//    {
//        return Language.GetTextValue($"Mods.CoolerItemVisualEffect.ConfigSLer.{str}", arg);
//    }

//    public static Asset<Texture2D> GetTexture(string path)
//    {
//        return ModContent.Request<Texture2D>($"CoolerItemVisualEffect/Config/ConfigSLer/{path}", AssetRequestMode.ImmediateLoad);
//    }

//    public WeaponSelectorUI WeaponSelectorUI;
//    public UserInterface WeaponSelectorInterface;
//    public static WeaponSelectorSystem Instance;
//    public static string ConstructFilePath;

//    public override void Load()
//    {
//        WeaponSelectorUI = new WeaponSelectorUI();
//        WeaponSelectorInterface = new UserInterface();
//        WeaponSelectorUI.Activate();
//        WeaponSelectorInterface.SetState(WeaponSelectorUI);
//        Instance = this;
//        base.Load();
//    }

//    public override void Unload()
//    {
//        WeaponSelectorUI = null;
//        WeaponSelectorInterface = null;
//        Instance = null;
//        base.Unload();
//    }

//    public override void UpdateUI(GameTime gameTime)
//    {
//        if (WeaponSelectorUI.Visible)
//            WeaponSelectorInterface?.Update(gameTime);
//    }

//    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
//    {
//        int inventoryIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
//        if (inventoryIndex != -1)
//        {
//            layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("CoolerItemVisualEffect: WeaponSelector UI", () =>
//            {
//                if (WeaponSelectorUI.Visible)
//                    WeaponSelectorUI.Draw(Main.spriteBatch);
//                return true;
//            }, InterfaceScaleType.UI));
//        }
//    }
//}

//public class WeaponSelectorUI : UIState
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
//    public WeaponSelector pendingSelector;

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

//        CreateButton = QuickButton(GetTexture("UI/Construct/Create"), "{$Mods.CoolerItemVisualEffect.WeaponSelector.Create}");
//        CreateButton.SetPos(new(-154f, 8), 0.5f, 0f);
//        CreateButton.OnLeftMouseDown += (_, _) =>
//        {
//            CacheSetupConfig = true;
//            var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup;
//            //int i = dict.Count;
//            var selector = new WeaponSelector();
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
//                    WeaponSelector.Load(list[counter], CacheConfigInfoPath);
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
//                WeaponSelector.Load(pendingSelector, CacheConfigInfoPath);
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
//                WeaponSelector.RestoreToDefault(pendingSelector);
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

//        //WeaponSelector selector = null;
//        //string name = Path.GetFileNameWithoutExtension(CacheConfigInfoPath);
//        //foreach (var s in Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().weaponGroup)
//        //{
//        //    if (s.Name == name)
//        //    {
//        //        selector = s;
//        //        break;
//        //    }
//        //}
//        WeaponSelector selector = WeaponSelector.Load(CacheConfigInfoPath);
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

//        UIToggleImage basedOnDefToggle = new UIToggleImage(ModContent.Request<Texture2D>("CoolerItemVisualEffect/Config/WeaponSelectorTex/FalseTrue", AssetRequestMode.ImmediateLoad), 48, 32, new Point(48, 0), new Point(0, 0));
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

//        UIToggleImage whiteListToggle = new UIToggleImage(ModContent.Request<Texture2D>("CoolerItemVisualEffect/Config/WeaponSelectorTex/BlackWhite", AssetRequestMode.ImmediateLoad), 20, 20, new Point(20, 0), new Point(0, 0));
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
//            GenericConfigElement.WrapIt(UIList, ref top, variable, selector, order++, onSetObj: OnSetConfigElementValue, owner: WeaponSelectorSystem.Instance.WeaponSelectorUI);
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
//                    UIList.Add(new WeaponSelectorInfoPanel(pth));//如果有就添加目标
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

//public class WeaponSelectorInfoPanel : CoolerUIPanel
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

//    public WeaponSelectorInfoPanel(string filePath) : base()
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
//            if (WeaponSelectorSystem.Instance.WeaponSelectorUI is not null)
//                WeaponSelectorSystem.Instance.WeaponSelectorUI.CacheSetupConfig = true;
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
//        var ui = WeaponSelectorSystem.Instance.WeaponSelectorUI;
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
//        if (File.Exists(FilePath) && WeaponSelectorSystem.Instance.WeaponSelectorUI is not null)
//        {
//            File.Move(FilePath, newPath);
//            WeaponSelectorSystem.Instance.WeaponSelectorUI.CacheSetupConfig = true;
//        }
//    }

//    private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
//    {
//        SoundEngine.PlaySound(SoundID.MenuTick);
//        if (File.Exists(FilePath))
//            FileUtilities.Delete(FilePath, false);
//        if (WeaponSelectorSystem.Instance.WeaponSelectorUI is not null)
//            WeaponSelectorSystem.Instance.WeaponSelectorUI.CacheSetupConfig = true;
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


