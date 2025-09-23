using CoolerItemVisualEffect.Common.MeleeModify;
using Newtonsoft.Json;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;

namespace CoolerItemVisualEffect.UI.WeaponGroup;

public partial class WeaponGroupManagerUI
{
    public static void SaveWeaponGroup(Weapon_Group group, bool overWrite = false, bool resetIndexNeeded = true) => ManagerHelper.SaveWeaponGroup(group, overWrite, resetIndexNeeded);
    private static class ManagerHelper
    {
        public static string SavePath { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "WeaponGroup");
        public const string Extension = ".json";
        public static string GetLocalizationKey(string suffix) => $"Mods.{nameof(CoolerItemVisualEffect)}.WeaponGroup.{suffix}";
        public static LocalizedText GetLocalization(string suffix) => Language.GetText(GetLocalizationKey(suffix));
        public static string GetLocalizationValue(string suffix) => GetLocalization(suffix).Value;

        public static void SaveWeaponGroup(Weapon_Group group, bool overWrite = false, bool resetIndexNeeded = true)
        {
            if (group.Name == "")
            {
                Main.NewText(GetLocalizationValue("EmptyName"), Color.Red);
                return;
            }
            var ModConfigPath = SavePath;
            Directory.CreateDirectory(ModConfigPath);
            string filename = group.Name;
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
                        Main.NewText(GetLocalizationValue("TooManySameName"), Color.Red);
                        return;
                    }
                }
            }
            if (sameDefault)
            {
                Main.NewText(GetLocalizationValue("SameDefault"), Color.Red);
            }
        label:
            string json = JsonConvert.SerializeObject(group, ConfigManager.serializerSettings);
            group.Name = Path.GetFileNameWithoutExtension(thisPath);
            File.WriteAllText(thisPath, json);

            if (resetIndexNeeded)
            {
                string indexTable = "";
                foreach (var pair in Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroups)
                    indexTable += $"{pair.Name}\n";

                File.WriteAllText(Path.Combine(SavePath, "indexTable.txt"), indexTable);
            }

            if (!overWrite)
                Main.NewText(GetLocalizationValue("SavedAs") + thisPath, Color.Yellow);
        }
    }
}
