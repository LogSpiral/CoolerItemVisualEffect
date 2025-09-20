using System.Collections.Generic;
namespace CoolerItemVisualEffect.Common.WeaponDisplay;

public static partial class WeaponDisplayUtils
{
    private static HashSet<int> DisplayItemBlackList { get; } =
        [
            ItemID.FlareGun,
            ItemID.MagicalHarp,
            ItemID.NebulaBlaze,
            ItemID.NebulaArcanum,
            ItemID.TragicUmbrella,
            ItemID.CombatWrench,
            ItemID.FairyQueenMagicItem,
            ItemID.BouncingShield,
            ItemID.SparkleGuitar
        ];

    private static bool CheckDisplayItem(Item item)
    {
        return
            item.holdStyle == 0
            && item.stack > 0
            && (item.damage > 0 || item.type == ItemID.CoinGun)
            && item.useAnimation > 0
            && item.useTime > 0
            && !item.consumable
            && item.ammo == 0
            && !DisplayItemBlackList.Contains(item.type);
    }

    private static bool CheckDisplayPlayer(Player player)
    {
        return
            (player.active || Main.gameMenu)
            && !player.dead
            && player.itemAnimation == 0
            && player.ItemTimeIsZero;
    }
}
