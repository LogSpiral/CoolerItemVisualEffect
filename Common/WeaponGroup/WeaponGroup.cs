using CoolerItemVisualEffect.Common.MeleeModify;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CoolerItemVisualEffect.UIBase.WeaponGroup;
using PropertyPanelLibrary.PropertyPanelComponents.Attributes;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.WeaponGroup;

public class WeaponGroup : IMemberLocalized
{
    [JsonIgnore]
    [PropertyPanelIgnore]
    public string Name;
    public bool BasedOnDefaultCondition { get; set; }

    [DefaultValue(true)]
    public bool WhiteList { get; set; } = true;

    [CustomOptionElement<OptionBindConfig>]
    public string BindConfigName { get; set; } = "";

    [CustomOptionElement<OptionWeaponList>]
    public List<string> WeaponList { get; set; } = [];

    public WeaponGroup()
    {
        Name = Language.GetOrRegister("Mods.CoolerItemVisualEffect.WeaponGroup.DefaultName").Value;
    }

    [JsonIgnore]
    [PropertyPanelIgnore]
    private IEnumerable<int> TargetTypes => 
        from str in WeaponList select int.TryParse(str, out var index) 
        ? index : ModContent.TryFind<ModItem>(str, out var result) 
            ? result.Type : 0;

    public bool CheckAvailabe(Item item)
    {
        if (item.type == ItemID.None) return false;
        var defaultCondition = !WhiteList;
        if (BasedOnDefaultCondition)
            defaultCondition = MeleeModifyPlayerUtils.MeleeBroadSwordCheck(item);
        if (WhiteList)
            return (defaultCondition || TargetTypes.Contains(item.type)) && item.damage > 0;
        else
            return defaultCondition && !TargetTypes.Contains(item.type) && item.damage > 0;
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

    string IMemberLocalized.LocalizationRootPath { get; } = $"Mods.{nameof(CoolerItemVisualEffect)}.WeaponGroup";
    IReadOnlyList<string> IMemberLocalized.LocalizationSuffixes { get; } = ["Label"];
}
