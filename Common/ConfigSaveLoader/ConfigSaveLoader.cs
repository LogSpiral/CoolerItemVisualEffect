namespace CoolerItemVisualEffect.Common.ConfigSaveLoader;


public class ConfigSaveLoader : ModItem
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
        if (player.itemAnimation == player.itemAnimationMax)
        {
            //if (ConfigSLUI.Visible)
            //    ConfigSLSystem.Instance.configSLUI.Close();
            //else
            //    ConfigSLSystem.Instance.configSLUI.Open();
        }
    }
    private static Condition EmptyHandCondition { get; } = 
        new Condition(
            "Mods.CoolerItemVisualEffect.EmptyHand", 
            () => Main.LocalPlayer.HeldItem.type == ItemID.None);
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddCondition(EmptyHandCondition)
            .Register();
    }
}