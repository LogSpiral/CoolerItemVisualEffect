using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.MeleeModify;
using MonoMod.Utils;
using NetSimplified;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;
namespace CoolerItemVisualEffect.Common.WeaponGroup;

public class SyncWeaponGroup : NetModule
{
    public byte plrIndex;
    public List<Weapon_Group> list;
    public Dictionary<string, MeleeConfig> dict;

    public static SyncWeaponGroup Get(int plrIndex, List<Weapon_Group> list, Dictionary<string, MeleeConfig> dict)
    {
        var result = NetModuleLoader.Get<SyncWeaponGroup>();
        result.plrIndex = (byte)plrIndex;
        result.list = list;
        result.dict = dict;
        return result;
    }

    public static SyncWeaponGroup Get()
    {
        var result = NetModuleLoader.Get<SyncWeaponGroup>();
        result.plrIndex = (byte)Main.myPlayer;
        var mplr = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
        result.list = mplr.WeaponGroup;
        result.dict = mplr.MeleeConfigs;
        return result;
    }

    public override void Send(ModPacket p)
    {
        p.Write(plrIndex);
        p.Write(dict != null);
        if (dict != null)
        {
            p.Write(dict.Count);
            foreach (var pair in dict)
            {
                p.Write(pair.Key);
                p.Write(JsonConvert.SerializeObject(pair.Value));
            }
        }
        p.Write(list != null);
        if (list != null)
        {
            var content2 = JsonConvert.SerializeObject(list);
            p.Write(content2);
        }

        base.Send(p);
    }

    public override void Read(BinaryReader r)
    {
        plrIndex = r.ReadByte();
        if (r.ReadBoolean())
        {
            //dict = []; JsonConvert.PopulateObject(r.ReadString(), dict);
            //dict = JsonConvert.DeserializeObject<Dictionary<string, MeleeConfig>>(r.ReadString());
            var count = r.ReadInt32();
            dict = [];
            for (var n = 0; n < count; n++)
            {
                var config = new MeleeConfig();
                config.designateData?.Colors.Clear();
                var key = r.ReadString();
                JsonConvert.PopulateObject(r.ReadString(), config);
                dict.Add(key, config);
            }
        }
        if (r.ReadBoolean())
        {
            list = []; JsonConvert.PopulateObject(r.ReadString(), list);
        }
        base.Read(r);
    }

    public override void Receive()
    {
        var plr = Main.player[plrIndex];
        var MMPlr = plr.GetModPlayer<MeleeModifyPlayer>();
        if (list != null)
        {
            MMPlr.WeaponGroup.Clear();
            MMPlr.WeaponGroup.AddRange(list);
        }
        if (dict != null)
        {
            MMPlr.MeleeConfigs.Clear();
            MMPlr.MeleeConfigs.AddRange(dict);
        }
        //if (MMPlr.heatMap != null && MMPlr.hsl != default)
        //    MeleeModifyPlayer.UpdateHeatMap(ref MMPlr.heatMap, MMPlr.hsl, MMPlr.ConfigurationSwoosh, MeleeModifyPlayer.GetWeaponTextureFromItem(plr.HeldItem));
        if (Main.dedServ)
        {
            Get(plrIndex, list, dict).Send(-1, plrIndex);
        }
    }
}
