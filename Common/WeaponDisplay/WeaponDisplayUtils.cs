namespace CoolerItemVisualEffect.Common.WeaponDisplay;

public static partial class WeaponDisplayUtils
{
    public static bool CheckDisplay(Player player, Item item)
    {
        return
            CheckDisplayPlayer(player)
            && CheckDisplayItem(item)
            && ItemLoader.CanUseItem(item, player);

    }
}
