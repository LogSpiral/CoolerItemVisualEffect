using CoolerItemVisualEffect.UI.ConfigSaveLoader;

namespace CoolerItemVisualEffect.Common.ConfigSaveLoader;


public class ConfigSaveLoader : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 20;
        Item.useTime = Item.useAnimation = 15;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.ResearchUnlockCount = 0;
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (player.itemAnimation == player.itemAnimationMax)
        {
            //if (ConfigSLUI.Visible)
            //    ConfigSLSystem.Instance.configSLUI.Close();
            //else
            //    ConfigSLSystem.Instance.configSLUI.Open();

            if (ConfigSaveLoaderUI.Active)
                ConfigSaveLoaderUI.Close();
            else
                ConfigSaveLoaderUI.Open();
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
            .DisableDecraft()
            .Register();
    }
}