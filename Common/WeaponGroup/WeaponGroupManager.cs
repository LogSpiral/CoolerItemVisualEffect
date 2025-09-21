namespace CoolerItemVisualEffect.Common.WeaponGroup;

public class WeaponGroupManager : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 20;
        Item.useTime = Item.useAnimation = 15;
        Item.useStyle = ItemUseStyleID.HoldUp;
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        //if (player.itemAnimation == player.itemAnimationMax)
        //{
        //    if (WeaponGroupUI.Visible)
        //        Instance.WeaponGroupUI.Close();
        //    else
        //        Instance.WeaponGroupUI.Open();
        //}
    }

    public override bool AltFunctionUse(Player player) => true;

    public override void AddRecipes()
    {
        // CreateRecipe().AddCondition(ConfigSLer.EmptyHandCondition).Register();
    }
}