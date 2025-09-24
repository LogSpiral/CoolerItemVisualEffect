using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.MeleeModify;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace CoolerItemVisualEffect.Common.WeaponDisplay;

public partial class WeaponDisplayLayer : PlayerDrawLayer
{
    public static bool ShouldWeaponDisplay(Player player)
    {
        foreach (var condition in DisplayBlackListConditionDictionary.Values)
            if (condition?.Invoke(player) is true)
                return false;
        return MiscConfig.Instance.useWeaponDisplay;
    }
    private static Dictionary<string, Func<Player, bool>> DisplayBlackListConditionDictionary { get; } = [];
    internal static bool RegisterNoWeaponDisplayCondition(Func<Player, bool> condition, string name) => DisplayBlackListConditionDictionary.TryAdd(name, condition);
    public override void Draw(ref PlayerDrawSet drawInfo)
    {
        var player = drawInfo.drawPlayer;
        if (drawInfo.headOnlyRender) return;
        if (!ShouldWeaponDisplay(player)) return;

        if (Main.gameMenu)
        {
            if (!MiscConfig.Instance.firstWeaponDisplay) return;
            //Item firstweapon =
            //    player.inventory.FirstOrDefault(
            //        weapon => weapon != null
            //        && WeaponDisplayUtils.CheckDisplay(player, weapon),
            //    null);
            Item firstWeapon = null;
            foreach (var item in player.inventory)
            {
                if (item != null && WeaponDisplayUtils.CheckDisplay(player, item))
                {
                    firstWeapon = item;
                    break;
                }
            }
            if (firstWeapon != null)
            {
                Main.instance.LoadItem(firstWeapon.type);
                DrawWeapon(player, firstWeapon, drawInfo);
            }
        }
        else
        {
            var holditem = player.inventory[player.selectedItem];
            if (WeaponDisplayUtils.CheckDisplay(player, holditem))
                DrawWeapon(player, holditem, drawInfo);


            /*
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();

            for (int n = 0; n < mplr.WeaponGroups.Count; n++)
            {
                var group = mplr.WeaponGroups[n];
                Main.spriteBatch.DrawString(
                    FontAssets.MouseText.Value,
                    (group.Name, group.BindConfigName, group.WeaponList.Count).ToString(),
                    player.Center - Main.screenPosition + new Vector2(-200, -100 + 40 * n),
                    Color.CornflowerBlue);
            }

            int height = -140;
            foreach (var pair in mplr.MeleeConfigs)
            {
                Main.spriteBatch.DrawString(
                    FontAssets.MouseText.Value,
                    pair.Key,
                    player.Center - Main.screenPosition + new Vector2(200, height += 40),
                    Color.MediumPurple);
            }
            */
        }
    }

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Backpacks);
}
