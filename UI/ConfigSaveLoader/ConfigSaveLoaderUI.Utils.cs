using Newtonsoft.Json;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.UI.ConfigSaveLoader;

public partial class ConfigSaveLoaderUI
{
    private static class ManagerHelper
    {
        public static string SavePath { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "MeleeConfig");
        public const string Extension = ".json";
        public static string GetLocalizationKey(string suffix) => $"Mods.{nameof(CoolerItemVisualEffect)}.ConfigSaveLoader.{suffix}";
        public static LocalizedText GetLocalization(string suffix) => Language.GetText(GetLocalizationKey(suffix));
        public static string GetLocalizationValue(string suffix) => GetLocalization(suffix).Value;

        public static void SaveConfig(ModConfig config, string assignedPath = null, bool announce = true)
        {
            var modConfigPath = SavePath;
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(modConfigPath);
            var thisPath = assignedPath;
            if (assignedPath == null)
            {
                string filename = GetLocalizationValue("DefaultName");
                var resultName = filename + Extension;
                thisPath = Path.Combine(SavePath, resultName);
                const int maxCount = 30;
                var sameDefault = false;
                if (File.Exists(thisPath))
                {
                    for (var i = 2; i <= maxCount; i++)
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
                    Main.NewText(GetLocalizationValue("SameDefault"), Color.Red);

            }

            var json = JsonConvert.SerializeObject(config, ConfigManager.serializerSettings);
            File.WriteAllText(thisPath, json);

            if (announce)
                Main.NewText(GetLocalizationValue("SavedAs") + thisPath, Color.Yellow);
        }
    }
}
