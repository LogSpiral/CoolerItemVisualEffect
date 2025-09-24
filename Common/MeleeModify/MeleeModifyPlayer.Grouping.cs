using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.ConfigSaveLoader;
using CoolerItemVisualEffect.UI.ConfigSaveLoader;
using CoolerItemVisualEffect.UI.WeaponGroup;
using System.Collections.Generic;
using System.IO;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;
namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public List<Weapon_Group> WeaponGroups { get; } = [];
    public Dictionary<string, MeleeConfig> MeleeConfigs { get; } = [];

    public Dictionary<int, Weapon_Group> CachedGrouping { get; } = [];

    private MeleeConfig configurationSwoosh;

    public MeleeConfig ConfigurationSwoosh
    {
        get
        {
            if (CachedGrouping.TryGetValue(Player.HeldItem.type, out var group))
            {
                if (group != null && !string.IsNullOrEmpty(group.BindConfigName) && MeleeConfigs.TryGetValue(group.BindConfigName, out var config))
                    return config;
                else goto label;
            }

            foreach (var weaponGroup in WeaponGroups)
            {
                if (weaponGroup.CheckAvailabe(Player.HeldItem))
                {
                    if (weaponGroup.BindConfigName == "" || weaponGroup.BindConfigName == null) goto label;
                    if (MeleeConfigs != null && MeleeConfigs.TryGetValue(weaponGroup.BindConfigName, out var config))
                        return config;
                    else if (Main.myPlayer == Player.whoAmI)
                    {
                        CachedGrouping[Player.HeldItem.type] = weaponGroup;
                        var configPath = Path.Combine(LoadHelper.ConfigSavePath, weaponGroup.BindConfigName + LoadHelper.Extension);
                        if (File.Exists(configPath))
                        {
                            var meleeConfig = new MeleeConfig();
                            ConfigSaveLoaderHelper.Load(meleeConfig, configPath, false, false);
                            MeleeConfigs.Add(weaponGroup.BindConfigName, meleeConfig);
                            return meleeConfig;
                        }
                        else goto label;
                    }
                    else goto label;
                }
            }
        label:
            configurationSwoosh ??= Main.myPlayer == Player.whoAmI ? MeleeConfig.Instance : new MeleeConfig();

            return configurationSwoosh;
        }
        set => configurationSwoosh = value;
    }

    public override void OnEnterWorld()
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
            SetUpWeaponGroupAndConfig();

        RegisterCurrentCanvas();

        if (ConfigSaveLoaderUI.Active)
            ConfigSaveLoaderUI.Close();
        if (WeaponGroupManagerUI.Active)
            WeaponGroupManagerUI.Close();

        base.OnEnterWorld();
    }

    private static void MigrateOldGroupPath()
    {
        var path = LoadHelper.GroupSavePathOld;
        if (Directory.Exists(path))
            Directory.Move(path, LoadHelper.GroupSavePath);
    }

    private void SetUpWeaponGroupAndConfig(bool forced = false)
    {
        if (Player.whoAmI != Main.myPlayer) return;
        if (forced)
        {
            WeaponGroups.Clear();
            MeleeConfigs.Clear();
        }


        if (!Directory.Exists(LoadHelper.GroupSavePath))
            Directory.CreateDirectory(LoadHelper.GroupSavePath);
        var tablePath = Path.Combine(LoadHelper.GroupSavePath, "indexTable.txt");
        if (File.Exists(tablePath))
        {
            var indexTable = File.ReadAllLines(tablePath);
            foreach (string path in indexTable)
            {
                var selectorPath = Path.Combine(LoadHelper.GroupSavePath, path + LoadHelper.Extension);
                if (!File.Exists(selectorPath))
                    continue;
                var selector = Weapon_Group.Load(selectorPath);
                WeaponGroups.Add(selector);
                if (selector.BindConfigName == null || selector.BindConfigName.Length == 0 || MeleeConfigs.ContainsKey(selector.BindConfigName)) continue;
                var configPath = Path.Combine(LoadHelper.ConfigSavePath, selector.BindConfigName + LoadHelper.Extension);
                if (File.Exists(configPath))
                {
                    var meleeConfig = new MeleeConfig();
                    ConfigSaveLoaderHelper.Load(meleeConfig, configPath, false, false);
                    MeleeConfigs.Add(selector.BindConfigName, meleeConfig);
                }
            }
        }
        WeaponGroupSyncing();
    }
}
file static class LoadHelper
{
    public static string GroupSavePathOld { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "WeaponSelector");
    public static string GroupSavePath { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "WeaponGroup");
    public static string ConfigSavePath { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "MeleeConfig");
    public const string Extension = ".json";
}
