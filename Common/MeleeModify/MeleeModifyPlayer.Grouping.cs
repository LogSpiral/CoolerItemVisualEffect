using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.ConfigSaveLoader;
using CoolerItemVisualEffect.UI.ConfigSaveLoader;
using CoolerItemVisualEffect.UI.WeaponGroup;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.Definition;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;
namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public List<Weapon_Group> WeaponGroups { get; } = [];
    public Dictionary<string, MeleeConfig> MeleeConfigs { get; } = [];

    public Dictionary<int, Weapon_Group> CachedGrouping { get; } = [];

    public MeleeConfig DefaultGroupConfig { get; set; }

    public bool IsModifyActiveDefaultGroup { get; set; } = true;

    public CIVESequenceDefinition SwooshActionStyleDefaultGroup { get; set; } = new("");

    public MeleeConfig ConfigurationSwoosh
    {
        get
        {
            // 处在分组内时先获取当前分组绑定的配置
            if (CurrentWeaponGroup is { } weaponGroup)
            {
                if (string.IsNullOrEmpty(weaponGroup.BindConfigName))
                    goto Label;
                if (!MeleeConfigs.TryGetValue(weaponGroup.BindConfigName, out var config)) goto Label;
                else return config;
            }

        // 没有绑定本地配置或者文件找不到等就采用默认的
        Label:
            DefaultGroupConfig ??=
                Main.myPlayer == Player.whoAmI
                ? MeleeConfig.Instance : new MeleeConfig();

            return DefaultGroupConfig;
        }
    }

    public Weapon_Group CurrentWeaponGroup
    {
        get
        {
            if (CachedGrouping.TryGetValue(Player.HeldItem.type, out var group) && group != null)
                return group;
            foreach (var weaponGroup in WeaponGroups)
            {
                if (!weaponGroup.CheckAvailabe(Player.HeldItem)) continue;
                CachedGrouping[Player.HeldItem.type] = weaponGroup;
                return weaponGroup;
            }
            return null;
        }
    }

    public bool IsModifyActive => CurrentWeaponGroup?.IsModifyActive ?? IsModifyActiveDefaultGroup;

    public CIVESequenceDefinition SwooshActionStyle => CurrentWeaponGroup?.SwooshActionStyle ?? SwooshActionStyleDefaultGroup;

    public override void OnEnterWorld()
    {
        if (Player.whoAmI == Main.myPlayer)
            LoadDefaultGroupData();

        if (Main.netMode == NetmodeID.SinglePlayer) 
        {
            SetUpWeaponGroupAndConfig();
            MeleeSequenceManager.RefillServerSequences();
        }

        RegisterCurrentCanvas();

        if (ConfigSaveLoaderUI.Active)
            ConfigSaveLoaderUI.Close();
        if (WeaponGroupManagerUI.Active)
            WeaponGroupManagerUI.Close();

        base.OnEnterWorld();
    }

    private void LoadDefaultGroupData()
    {
        var defaultGroupFilePath = Path.Combine(LoadHelper.GroupSavePath, "DefaultGroup.txt");
        if (File.Exists(defaultGroupFilePath))
        {
            string[] contents = File.ReadAllLines(defaultGroupFilePath);
            if (contents is not [string, string]) return;
            if (bool.TryParse(contents[0], out var isActive)) IsModifyActiveDefaultGroup = isActive;
            SwooshActionStyleDefaultGroup = new CIVESequenceDefinition(contents[1]);
        }
        else
        {
            var configInstance = MeleeConfig.Instance;
            if (configInstance.SwordModifyActiveOld.HasValue || configInstance.swooshActionStyleOld != null)
            {
                IsModifyActiveDefaultGroup = configInstance.SwordModifyActiveOld ?? true;
                SwooshActionStyleDefaultGroup = CIVESequenceDefinition.FromLSLSequenceDefinition(configInstance.swooshActionStyleOld);

                configInstance.SwordModifyActiveOld = null;
                configInstance.swooshActionStyleOld = null;

                SaveDefaultGroupData();
            }
        }
    }

    private static void MigrateOldGroupPath()
    {
        var path = LoadHelper.GroupSavePathOld;
        if (Directory.Exists(path))
            Directory.Move(path, LoadHelper.GroupSavePath);
    }

    private void SetUpWeaponGroupAndConfig()
    {
        if (Player.whoAmI != Main.myPlayer) return;

        WeaponGroups.Clear();
        MeleeConfigs.Clear();

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
                if (string.IsNullOrEmpty(selector.BindConfigName) || MeleeConfigs.ContainsKey(selector.BindConfigName)) continue;
                var configPath = Path.Combine(LoadHelper.ConfigSavePath, selector.BindConfigName + LoadHelper.Extension);
                if (File.Exists(configPath))
                {
                    var meleeConfig = new MeleeConfig();
                    ConfigSaveLoaderHelper.Load(meleeConfig, configPath, false, false);
                    MeleeConfigs.TryAdd(selector.BindConfigName, meleeConfig);

                    if (!selector.SwooshActionStyle.IsUnloaded) continue;
                    if (meleeConfig.SwordModifyActiveOld.HasValue)
                    {
                        selector.IsModifyActive = meleeConfig.SwordModifyActiveOld.Value;
                        meleeConfig.SwordModifyActiveOld = null;
                    }
                    if (meleeConfig.swooshActionStyleOld != null)
                    {
                        selector.SwooshActionStyle = CIVESequenceDefinition.FromLSLSequenceDefinition(meleeConfig.swooshActionStyleOld);
                        meleeConfig.swooshActionStyleOld = null;
                    }
                }
            }
        }
        WeaponGroupSyncing();
    }
}
internal static class LoadHelper
{
    public static string GroupSavePathOld { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "WeaponSelector");
    public static string GroupSavePath { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "WeaponGroup");
    public static string ConfigSavePath { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "MeleeConfig");
    public const string Extension = ".json";
}
