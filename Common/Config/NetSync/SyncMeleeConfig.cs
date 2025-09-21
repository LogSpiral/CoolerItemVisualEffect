using CoolerItemVisualEffect.Common.MeleeModify;
using NetSimplified;
using Newtonsoft.Json;
using System.IO;
// ReSharper disable InconsistentNaming

namespace CoolerItemVisualEffect.Common.Config.NetSync;

public class SyncMeleeConfig : NetModule
{
    private int playerIndex;
    private MeleeConfig configuration;

    public static SyncMeleeConfig Get(int plrIndex, MeleeConfig configurationCIVE)
    {
        var result = NetModuleLoader.Get<SyncMeleeConfig>();
        result.playerIndex = plrIndex;
        result.configuration = configurationCIVE;
        return result;
    }

    public override void Send(ModPacket p)
    {
        p.Write((byte)playerIndex);
        var content = JsonConvert.SerializeObject(configuration);
        p.Write(content);
        base.Send(p);
    }

    public override void Read(BinaryReader r)
    {
        playerIndex = r.ReadByte();
        var content = r.ReadString();
        configuration = new MeleeConfig();
        configuration.designateData?.Colors.Clear();
        JsonConvert.PopulateObject(content, configuration);
        //configuration = (ConfigurationCIVE)JsonConvert.DeserializeObject(content);
        base.Read(r);
    }

    public override void Receive()
    {
        var plr = Main.player[playerIndex];
        var mplr = plr.GetModPlayer<MeleeModifyPlayer>();
        mplr.ConfigurationSwoosh = configuration;
        if (mplr.HeatMap != null && mplr.WeaponHSL != default)
            MeleeModifyPlayerUtils.UpdateHeatMap(mplr);
        if (Main.dedServ)
        {
            Get(playerIndex, configuration).Send(-1, playerIndex);
        }
    }
}