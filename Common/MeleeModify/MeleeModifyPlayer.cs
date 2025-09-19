using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.Config.NetSync;
using CoolerItemVisualEffect.Common.WeaponGroup;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using static CoolerItemVisualEffect.Common.Config.MeleeConfig;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;

namespace CoolerItemVisualEffect.Common.MeleeModify;
public partial class MeleeModifyPlayer : ModPlayer
{
    #region 基本量声明

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
            configurationSwoosh ??= Main.myPlayer == Player.whoAmI ? Instance : new MeleeConfig();

            return configurationSwoosh;
        }
        set => configurationSwoosh = value;
    }

    public List<Weapon_Group> WeaponGroup { get; } = [];
    public Dictionary<string, MeleeConfig> MeleeConfigs { get; } = [];

    public bool IsMeleeBroadSword
    {
        get
        {
            if (WeaponGroup != null)
                foreach (var selector in WeaponGroup)
                    if (selector.CheckAvailabe(Player.HeldItem))
                        return true;

            return MeleeModifyPlayerUtils.MeleeBroadSwordCheck(Player.HeldItem);
        }
    }

    public bool BeAbleToOverhaul =>
        SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.Overhaul
        && ConfigurationSwoosh.SwordModifyActive
        && IsMeleeBroadSword;

    public bool UseSwordModify =>
        BeAbleToOverhaul
        && Player.itemAnimation > 0;

    public void WeaponGroupSyncing()
    {
        SyncWeaponGroup.Get(Player.whoAmI, WeaponGroup, MeleeConfigs).Send();
    }

    #endregion 基本量声明

    #region 视觉效果相关

    public Texture2D HeatMap 
    { 
        get
        {
            CoolerItemVisualEffectHelper.CreateHeatMapIfNull(ref field);
            return field;
        } 
    }
    public Color MainColor { get; set; }
    public int LastWeaponHash { get; set; }
    public static int LastWeaponType
    {
        get => field == ItemID.None ? ItemID.TerraBlade : field;
        set;
    }
    public Vector3 WeaponHSL { get; set; }

    #endregion 视觉效果相关



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



    #region 更新状态

    public override void PostUpdate()
    {
        if (IsMeleeBroadSword)
            ItemID.Sets.SkipsInitialUseSound[Player.HeldItem.type] =
                SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.Overhaul
                && ConfigurationSwoosh.SwordModifyActive;

        MeleeModifyPlayerUtils.CheckItemChange(Player);
        base.PostUpdate();
    }



    #endregion 更新状态

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        configurationSwoosh ??= Main.myPlayer == Player.whoAmI ? Instance : new MeleeConfig();

        SetUpWeaponGroupAndConfig();
        SyncMeleeConfig.Get(Player.whoAmI, configurationSwoosh).Send(toWho, fromWho);
        SyncWeaponGroup.Get(Player.whoAmI, WeaponGroup, MeleeConfigs).Send(toWho, fromWho);
        base.SyncPlayer(toWho, fromWho, newPlayer);
    }

}

