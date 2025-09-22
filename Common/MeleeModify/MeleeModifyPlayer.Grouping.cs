using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.ConfigSaveLoader;
using System.Collections.Generic;
using System.IO;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;
namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public List<Weapon_Group> WeaponGroups { get; } = [];
    public Dictionary<string, MeleeConfig> MeleeConfigs { get; } = [];

    private MeleeConfig configurationSwoosh;

    public MeleeConfig ConfigurationSwoosh
    {
        get
        {
            if (WeaponGroups != null)
                foreach (var pair in WeaponGroups)
                {
                    if (pair.CheckAvailabe(Player.HeldItem))
                    {
                        if (pair.BindSequenceName == "" || pair.BindSequenceName == null) goto label;
                        if (MeleeConfigs != null && MeleeConfigs.TryGetValue(pair.BindSequenceName, out var config))
                            return config;
                        else if (Main.myPlayer == Player.whoAmI)
                        {
                            var configPath = Path.Combine(LoadHelper.ConfigSavePath, pair.BindSequenceName + LoadHelper.Extension);
                            if (File.Exists(configPath))
                            {
                                var meleeConfig = new MeleeConfig();
                                ConfigSaveLoaderHelper.Load(meleeConfig, configPath, false, false);
                                MeleeConfigs.Add(pair.BindSequenceName, meleeConfig);
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
        SetUpWeaponGroupAndConfig();

        RegisterCurrentCanvas();
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
        else
            return;




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
                if (selector.BindSequenceName == null || selector.BindSequenceName.Length == 0 || MeleeConfigs.ContainsKey(selector.BindSequenceName)) continue;
                var configPath = Path.Combine(LoadHelper.ConfigSavePath, selector.BindSequenceName + LoadHelper.Extension);
                if (File.Exists(configPath))
                {
                    var meleeConfig = new MeleeConfig();
                    ConfigSaveLoaderHelper.Load(meleeConfig, configPath, false, false);
                    MeleeConfigs.Add(selector.BindSequenceName, meleeConfig);
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
