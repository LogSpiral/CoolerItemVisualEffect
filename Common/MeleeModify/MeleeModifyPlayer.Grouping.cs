using CoolerItemVisualEffect.Common.Config;
using System.Collections.Generic;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;
namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public List<Weapon_Group> WeaponGroup { get; } = [];
    public Dictionary<string, MeleeConfig> MeleeConfigs { get; } = [];

    private MeleeConfig configurationSwoosh;

    public MeleeConfig ConfigurationSwoosh
    {
        get
        {
#if false
        if (weaponGroup != null)
            foreach (var pair in weaponGroup)
            {
                if (pair.CheckAvailabe(Player.HeldItem))
                {
                    if (pair.BindSequenceName == "" || pair.BindSequenceName == null) goto label;
                    if (meleeConfigs != null && meleeConfigs.TryGetValue(pair.BindSequenceName, out var config))
                        return config;
                    else if (Main.myPlayer == Player.whoAmI)
                    {
                        var configPath = Path.Combine(ConfigSLHelper.SavePath, pair.BindSequenceName + ConfigSLHelper.Extension);
                        if (File.Exists(configPath))
                        {
                            var meleeConfig = new MeleeConfig();
                            ConfigSLHelper.Load(meleeConfig, configPath, false, false);
                            meleeConfigs.Add(pair.BindSequenceName, meleeConfig);
                            return meleeConfig;
                        }
                        else goto label;
                    }
                    else goto label;
                }
            }
#endif
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

    private void SetUpWeaponGroupAndConfig(bool forced = false)
    {
        if (Player.whoAmI != Main.myPlayer) return;
        if (forced)
        {
            WeaponGroup.Clear();
            MeleeConfigs.Clear();
        }
        else
            return;

#if false
        if (!Directory.Exists(WeaponGroupSystem.SavePath))
            Directory.CreateDirectory(WeaponGroupSystem.SavePath);
        var tablePath = Path.Combine(WeaponGroupSystem.SavePath, "indexTable.txt");
        if (File.Exists(tablePath))
        {
            var indexTable = File.ReadAllLines(tablePath);
            foreach (string path in indexTable)
            {
                var selectorPath = Path.Combine(WeaponGroupSystem.SavePath, path + WeaponGroupSystem.Extension);
                if (!File.Exists(selectorPath))
                    continue;
                var selector = WeaponGroup.Load(selectorPath);
                weaponGroup.Add(selector);
                if (selector.BindSequenceName == null || selector.BindSequenceName.Length == 0 || meleeConfigs.ContainsKey(selector.BindSequenceName)) continue;
                var configPath = Path.Combine(ConfigSLHelper.SavePath, selector.BindSequenceName + ConfigSLHelper.Extension);
                if (File.Exists(configPath))
                {
                    var meleeConfig = new MeleeConfig();
                    ConfigSLHelper.Load(meleeConfig, configPath, false, false);
                    meleeConfigs.Add(selector.BindSequenceName, meleeConfig);
                }
            }
        }
        WeaponGroupSyncing();
#endif
    }
}
