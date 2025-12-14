namespace CoolerItemVisualEffect.Common.WeaponDisplay;

public static partial class WeaponDisplayUtils
{
    public static bool CheckDisplay(Player player, Item item)
    {
        return
            CheckDisplayPlayer(player)
            && CheckDisplayItem(item);
            // 为什么会有在CanUseItem里面发射弹幕的情况啊草
            // && ItemLoader.CanUseItem(item, player);

    }
}
