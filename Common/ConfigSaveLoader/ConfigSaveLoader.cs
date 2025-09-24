using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.UI.ConfigSaveLoader;
using LogSpiralLibrary.CodeLibrary.ConfigModification;

namespace CoolerItemVisualEffect.Common.ConfigSaveLoader;

public class ConfigSaveLoader : ModItem
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
    public override void Load()
    {
        base.Load();
        ConfigPreviewSystem.RegisterRenderOnCondition(() => ConfigSaveLoaderUI.Active);
    }
    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (player.whoAmI != Main.myPlayer) return;
        if (player.itemAnimation == player.itemAnimationMax)
        {
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