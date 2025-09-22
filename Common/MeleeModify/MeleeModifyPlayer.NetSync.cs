using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.Config.NetSync;
using CoolerItemVisualEffect.Common.WeaponGroup;
using NetSimplified;
using NetSimplified.Syncing;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public void WeaponGroupSyncing()
    {
        SyncWeaponGroup.Get(Player.whoAmI, WeaponGroups, MeleeConfigs).Send();
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        configurationSwoosh ??= Main.myPlayer == Player.whoAmI ? MeleeConfig.Instance : new MeleeConfig();

        SetUpWeaponGroupAndConfig();
        SyncMeleeConfig.Get(Player.whoAmI, configurationSwoosh).Send(toWho, fromWho);
        SyncWeaponGroup.Get(Player.whoAmI, WeaponGroups, MeleeConfigs).Send(toWho, fromWho);
        SyncRegisterCanvas.Get((byte)Player.whoAmI).Send(toWho, fromWho);
        base.SyncPlayer(toWho, fromWho, newPlayer);
    }
}

[AutoSync]
public class SyncRegisterCanvas : NetModule
{
    private byte playerIndex;
    public static SyncRegisterCanvas Get(byte whoAmI)
    {
        var packet = NetModuleLoader.Get<SyncRegisterCanvas>();
        packet.playerIndex = whoAmI;
        return packet;
    }

    public override void Receive()
    {
        if (!Main.dedServ)
            Main.player[playerIndex]
                .GetModPlayer<MeleeModifyPlayer>()
                .RegisterCurrentCanvas();
    }
}