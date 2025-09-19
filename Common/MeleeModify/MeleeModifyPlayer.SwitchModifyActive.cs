using CoolerItemVisualEffect.Common.Config.NetSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public static ModKeybind ModifyActiveKeybind { get; private set; }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (ModifyActiveKeybind.JustReleased)
        {
            bool active = configurationSwoosh.SwordModifyActive ^= true;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                SyncMeleeModifyActive.Get(Player.whoAmI, active).Send(-1, Player.whoAmI);
            for (int n = 0; n < 32; n++)
                Dust.NewDustPerfect(Player.Center, Main.rand.Next([DustID.FireworkFountain_Blue]), Main.rand.NextVector2Unit() * 32).noGravity = true;
            //, DustID.FireworkFountain_Green, DustID.FireworkFountain_Pink, DustID.FireworkFountain_Red, DustID.FireworkFountain_Yellow
            Main.NewText(Language.GetOrRegister($"Mods.CoolerItemVisualEffect.Misc.MeleeModify{(active ? "Active" : "Deactive")}"));
            if (Main.myPlayer == Player.whoAmI)
                ConfigManager.Save(configurationSwoosh);
        }
        base.ProcessTriggers(triggersSet);
    }
}
