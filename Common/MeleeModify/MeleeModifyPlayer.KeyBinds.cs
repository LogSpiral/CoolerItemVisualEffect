using CoolerItemVisualEffect.Common.Config.NetSync;
using CoolerItemVisualEffect.UI.ConfigSaveLoader;
using CoolerItemVisualEffect.UI.WeaponGroup;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    private static ModKeybind ModifyActiveKeybind { get; set; }
    private static ModKeybind ConfigManagerKeybind { get; set; }
    private static ModKeybind GroupManagerKeybind { get; set; }
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (ModifyActiveKeybind.JustReleased)
        {
            var active = configurationSwoosh.SwordModifyActive ^= true;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                SyncMeleeModifyActive.Get(Player.whoAmI, active).Send(-1, Player.whoAmI);
            for (var n = 0; n < 32; n++)
                Dust.NewDustPerfect(Player.Center, Main.rand.Next([DustID.FireworkFountain_Blue]), Main.rand.NextVector2Unit() * 32).noGravity = true;
            //, DustID.FireworkFountain_Green, DustID.FireworkFountain_Pink, DustID.FireworkFountain_Red, DustID.FireworkFountain_Yellow
            Main.NewText(Language.GetOrRegister($"Mods.CoolerItemVisualEffect.Misc.MeleeModify{(active ? "Active" : "Deactive")}"));
            if (Main.myPlayer == Player.whoAmI)
                ConfigManager.Save(configurationSwoosh);
        }
        if (ConfigManagerKeybind.JustPressed) 
        {
            if (ConfigSaveLoaderUI.Active)
                ConfigSaveLoaderUI.Close();
            else
                ConfigSaveLoaderUI.Open();
        }
        if (GroupManagerKeybind.JustPressed) 
        {
            if (WeaponGroupManagerUI.Active)
                WeaponGroupManagerUI.Close();
            else
                WeaponGroupManagerUI.Open();
        }
        base.ProcessTriggers(triggersSet);
    }
}
