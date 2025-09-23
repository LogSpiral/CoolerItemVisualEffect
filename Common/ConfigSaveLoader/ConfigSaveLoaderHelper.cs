using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.ConfigSaveLoader;

public static class ConfigSaveLoaderHelper
{
    // public static ConfigSLUI configSLUI => ConfigSLSystem.Instance.configSLUI;
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

    public static string SavePath { get; } = Path.Combine(ModLoader.ModPath, "CoolerItemVisualEffect", "MeleeConfig");
    public static string Extension { get; } = ".json";

    public static void Save(ModConfig config, string assignedPath = null, bool announce = true)
    {


    }

    public static void Load(ModConfig config, string filename, bool autoPath = false, bool announce = true)
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
            if (announce)
            {
                Main.NewText(GetText("Succeed"), Color.Yellow);
                SoundEngine.PlaySound(SoundID.Unlock);
            }
            config.OnChanged();
        }
        catch (Exception e) when (jsonFileExists && e is JsonReaderException or JsonSerializationException)
        {
            Logging.tML.Warn($"Then config file {config.Name} from the mod {config.Mod.Name} located at {path} failed to load. The file was likely corrupted somehow, so the defaults will be loaded and the file deleted.");
            File.Delete(path);
            Main.NewText(GetText("Failed"), Color.Red);
            SoundEngine.PlaySound(SoundID.Thunder);
        }
    }

}
