using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.MeleeModify;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public static partial class MeleeModifyPlayerUtils
{
    public static List<(Func<Item, Texture2D> func, float priority)> WeaponGetFunctions { get; } = [];
    private static bool _pendingRefreshWeaponFunctionOrder;

    public static void RegisterModifyWeaponTex(Func<Item, Texture2D> func, float priority)
    {
        WeaponGetFunctions.Add((func, priority));
        _pendingRefreshWeaponFunctionOrder = true;
    }


    /// <summary>
    /// 如果你的武器贴图和"看上去"的不一样，用这个换成看上去的
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static Texture2D GetWeaponTextureFromItem(Item item)
    {
        if (item == null || item.type == ItemID.None) return TextureAssets.Item[MeleeModifyPlayer.LastWeaponType].Value;
        if (_pendingRefreshWeaponFunctionOrder)
        {
            _pendingRefreshWeaponFunctionOrder = false;
            WeaponGetFunctions.Sort((v1, v2) => v1.priority > v2.priority ? 1 : -1);
        }
        Texture2D texture = null;
        foreach (var func in WeaponGetFunctions)
        {
            if (func.func == null) continue;
            if (texture != null) break;
            texture = func.func.Invoke(item);
        }
        return texture ?? TextureAssets.Item[item.type].Value;
    }

    public static void UpdateHeatMap(MeleeModifyPlayer mplr)
    {
        UpdateHeatMap(
            mplr.HeatMap, 
            mplr.WeaponHSL, 
            mplr.ConfigurationSwoosh,
            GetWeaponTextureFromItem(mplr.Player.HeldItem)
            );
    }

    public static void UpdateHeatMap(Texture2D heatMap, Vector3 hsl, MeleeConfig config, Texture2D itemTexture)
    {
        if (heatMap == null) return;
        var colors = new Color[heatMap.Width * heatMap.Height];
        FillHeatMap(colors, hsl, itemTexture, config);
        heatMap.SetData(colors);
    }

    public static void CheckItemChange(Player player)
    {
        MeleeModifyPlayer modPlayer = player.GetModPlayer<MeleeModifyPlayer>();
        var hashCode = player.HeldItem.GetHashCode();
        if (Main.dedServ) return;
        if (modPlayer.LastWeaponHash == hashCode) return;
        Main.instance.LoadItem(player.HeldItem.type);
        var texture = GetWeaponTextureFromItem(player.HeldItem);

        if (!modPlayer.IsMeleeBroadSword)
            foreach (var proj in Main.projectile)
                if (proj.type == ModContent.ProjectileType<CIVESword>() && proj.owner == player.whoAmI)
                {
                    proj.Kill();
                    return;
                }

        modPlayer.LastWeaponHash = hashCode;
        MeleeModifyPlayer.LastWeaponType = player.HeldItem.type;
        var newColor = modPlayer.MainColor = CoolerItemVisualEffectHelper.CalculateWeightedMean(texture);
        modPlayer.WeaponHSL = Main.rgbToHsl(newColor);
        UpdateHeatMap(modPlayer.HeatMap, modPlayer.WeaponHSL, modPlayer.ConfigurationSwoosh, texture);
        modPlayer.RefreshConfigEffects();
    }
    public static bool MeleeBroadSwordCheck(Item item)
    {
        bool flag = MeleeClassCheck(item.DamageType);
        flag &= item.damage > 0;
        flag &= !item.noUseGraphic;
        flag &= !item.noMelee || VanillaSlashItems.Contains(item.type);
        flag &= item.useStyle == ItemUseStyleID.Swing;
        flag &= item.pick == 0;
        flag &= item.axe == 0;
        flag &= item.hammer == 0;
        flag &= !VanillaToolItems.Contains(item.type);
        return flag;
    }
}
