using CoolerItemVisualEffect.Common.MeleeModify;
using NetSimplified;
using NetSimplified.Syncing;

namespace CoolerItemVisualEffect.Common.Config.NetSync;

[AutoSync]
public class SyncMeleeModifyActive : NetModule
{
    private int playerIndex;
    private bool active;

    public static SyncMeleeModifyActive Get(int plrIndex, bool active)
    {
        var result = NetModuleLoader.Get<SyncMeleeModifyActive>();
        result.playerIndex = plrIndex;
        result.active = active;
        return result;
    }

    public override void Receive()
    {
        var plr = Main.player[playerIndex];
        var mplr = plr.GetModPlayer<MeleeModifyPlayer>();
        mplr.ConfigurationSwoosh.SwordModifyActive = active;
        if (mplr.HeatMap != null && mplr.WeaponHSL != default)
            MeleeModifyPlayerUtils.UpdateHeatMap(mplr);
        if (Main.dedServ)
        {
            Get(playerIndex, active).Send(-1, playerIndex);
        }
    }
}
