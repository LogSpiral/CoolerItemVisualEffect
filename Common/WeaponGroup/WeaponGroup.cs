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
    private IEnumerable<int> TargetTypes => from str in weaponGroup select int.TryParse(str, out var index) ? index : ModContent.TryFind<ModItem>(str, out var result) ? result.Type : 0;

    public bool CheckAvailabe(Item item)
    {
        if (item.type == ItemID.None) return false;
        var defaultCondition = !whiteList;
        if (basedOnDefaultCondition)
            defaultCondition = MeleeModifyPlayerUtils.MeleeBroadSwordCheck(item);
        if (whiteList)
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
}
