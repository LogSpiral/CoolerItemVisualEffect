using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.Config.NetSync;
using CoolerItemVisualEffect.Common.WeaponGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public void WeaponGroupSyncing()
    {
        SyncWeaponGroup.Get(Player.whoAmI, WeaponGroup, MeleeConfigs).Send();
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        configurationSwoosh ??= Main.myPlayer == Player.whoAmI ? MeleeConfig.Instance : new MeleeConfig();

        SetUpWeaponGroupAndConfig();
        SyncMeleeConfig.Get(Player.whoAmI, configurationSwoosh).Send(toWho, fromWho);
        SyncWeaponGroup.Get(Player.whoAmI, WeaponGroup, MeleeConfigs).Send(toWho, fromWho);
        base.SyncPlayer(toWho, fromWho, newPlayer);
    }
}
