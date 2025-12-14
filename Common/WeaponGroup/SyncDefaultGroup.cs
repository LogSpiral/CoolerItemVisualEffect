using CoolerItemVisualEffect.Common.MeleeModify;
using NetSimplified;
using System.IO;

namespace CoolerItemVisualEffect.Common.WeaponGroup;

public class SyncDefaultGroup : NetModule
{
    public byte plrIndex;

    public bool IsActive;

    public static SyncDefaultGroup Get(int plrIndex, bool isActive)
    {
        var result = NetModuleLoader.Get<SyncDefaultGroup>();
        result.plrIndex = (byte)plrIndex;
        result.IsActive = isActive;
        return result;
    }

    public override void Send(ModPacket p)
    {
        p.Write(plrIndex);
        p.Write(IsActive);

        base.Send(p);
    }

    public override void Read(BinaryReader r)
    {
        plrIndex = r.ReadByte();
        IsActive = r.ReadBoolean();
        base.Read(r);
    }

    public override void Receive()
    {
        var plr = Main.player[plrIndex];
        var MMPlr = plr.GetModPlayer<MeleeModifyPlayer>();
        MMPlr.IsModifyActiveDefaultGroup = IsActive;
        if (Main.dedServ)
            Get(plrIndex, IsActive).Send(-1, plrIndex);

    }
}