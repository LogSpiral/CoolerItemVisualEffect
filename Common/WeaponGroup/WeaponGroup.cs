using CoolerItemVisualEffect.Common.MeleeModify;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.WeaponGroup;

public class WeaponGroup
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
    //        CoolerUITextPanel<LocalizedText> curConfigPanel = new(Language.GetOrRegister("Mods.CoolerItemVisualEffect.WeaponGroup.CurrentConfig"));
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

    private const string key = "$Mods.CoolerItemVisualEffect.WeaponGroup.";

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

    public WeaponGroup()
    {
        Name = Language.GetOrRegister("Mods.CoolerItemVisualEffect.WeaponGroup.DefaultName").Value;
    }

    [JsonIgnore]
    public IEnumerable<int> targetTypes => from str in weaponGroup select int.TryParse(str, out var index) ? index : ModContent.TryFind<ModItem>(str, out var result) ? result.Type : 0;

    public bool CheckAvailabe(Item item)
    {
        if (item.type == ItemID.None) return false;
        var defaultCondition = !whiteList;
        if (basedOnDefaultCondition)
            defaultCondition = MeleeModifyPlayerUtils.MeleeBroadSwordCheck(item);
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

    public static WeaponGroup Load(string path)
    {
        var result = new WeaponGroup();
        Load(result, path);
        result.Name = Path.GetFileNameWithoutExtension(path);
        return result;
    }

    public static void Load(WeaponGroup WeaponGroup, string path) => JsonConvert.PopulateObject(File.ReadAllText(path), WeaponGroup, ConfigManager.serializerSettings);

    public static void RestoreToDefault(WeaponGroup WeaponGroup) => JsonConvert.PopulateObject("{}", WeaponGroup, ConfigManager.serializerSettings);
}
