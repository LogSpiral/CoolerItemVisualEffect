using CoolerItemVisualEffect.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Item holditem = player.inventory[player.selectedItem];
            if (WeaponDisplayUtils.CheckDisplay(player, holditem))
                DrawWeapon(player, holditem, drawInfo);
        }
    }

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Backpacks);
}
