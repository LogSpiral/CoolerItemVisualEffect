using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.UI.WeaponGroup;

namespace CoolerItemVisualEffect.Common.WeaponGroup;

public class WeaponGroupManager : ModItem
{
    public override bool IsLoadingEnabled(Mod mod) => ServerConfig.Instance.UseItemManager;
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
        if (player.whoAmI != Main.myPlayer) return;
        if (player.itemAnimation == player.itemAnimationMax)
        {
            if (WeaponGroupManagerUI.Active)
                WeaponGroupManagerUI.Close();
            else
                WeaponGroupManagerUI.Open();
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