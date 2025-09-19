using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.MeleeModify;
using NetSimplified;
using Newtonsoft.Json;
using System.IO;

namespace CoolerItemVisualEffect.Common.Config.NetSync;

public class SyncMeleeConfig : NetModule
{
    public int plrIndex;
    public MeleeConfig configuration;

    public static SyncMeleeConfig Get(int plrIndex, MeleeConfig configurationCIVE)
    {
        var result = NetModuleLoader.Get<SyncMeleeConfig>();
        result.plrIndex = plrIndex;
        result.configuration = configurationCIVE;
        return result;
    }

    public override void Send(ModPacket p)
    {
        p.Write((byte)plrIndex);
        string content = JsonConvert.SerializeObject(configuration);
        p.Write(content);
        base.Send(p);
    }

    public override void Read(BinaryReader r)
    {
        plrIndex = r.ReadByte();
        string content = r.ReadString();
        configuration = new MeleeConfig();
        configuration.designateData?.colors.Clear();
        JsonConvert.PopulateObject(content, configuration);
        //configuration = (ConfigurationCIVE)JsonConvert.DeserializeObject(content);
        base.Read(r);
    }

    public override void Receive()
    {
        var plr = Main.player[plrIndex];
        var mplr = plr.GetModPlayer<MeleeModifyPlayer>();
        mplr.ConfigurationSwoosh = configuration;
        if (mplr.HeatMap != null && mplr.WeaponHSL != default)
            MeleeModifyPlayerUtils.UpdateHeatMap(mplr);
        if (Main.dedServ)
        {
            Get(plrIndex, configuration).Send(-1, plrIndex);
        }
    }
}