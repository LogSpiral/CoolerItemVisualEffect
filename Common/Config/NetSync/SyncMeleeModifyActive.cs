using CoolerItemVisualEffect.Common.MeleeModify;
using NetSimplified;
using NetSimplified.Syncing;

namespace CoolerItemVisualEffect.Common.Config.NetSync;

[AutoSync]
public class SyncMeleeModifyActive : NetModule
{
    public int plrIndex;
    public bool active;

    public static SyncMeleeModifyActive Get(int plrIndex, bool active)
    {
        var result = NetModuleLoader.Get<SyncMeleeModifyActive>();
        result.plrIndex = plrIndex;
        result.active = active;
        return result;
    }

    public override void Receive()
    {
        var plr = Main.player[plrIndex];
        var mplr = plr.GetModPlayer<MeleeModifyPlayer>();
        mplr.ConfigurationSwoosh.SwordModifyActive = active;
        if (mplr.HeatMap != null && mplr.WeaponHSL != default)
            MeleeModifyPlayerUtils.UpdateHeatMap(mplr);
        if (Main.dedServ)
        {
            Get(plrIndex, active).Send(-1, plrIndex);
        }
    }
}
