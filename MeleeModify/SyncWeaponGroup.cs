using CoolerItemVisualEffect.Config;
using MonoMod.Utils;
using NetSimplified;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolerItemVisualEffect.MeleeModify;

public class SyncWeaponGroup : NetModule
{
    public byte plrIndex;
    public List<WeaponSelector> list;
    public Dictionary<string, MeleeConfig> dict;

    public static SyncWeaponGroup Get(int plrIndex, List<WeaponSelector> list, Dictionary<string, MeleeConfig> dict)
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
        result.list = mplr.weaponGroup;
        result.dict = mplr.meleeConfigs;
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
            int count = r.ReadInt32();
            dict = [];
            for (int n = 0; n < count; n++)
            {
                var config = new MeleeConfig();
                config.designateData?.colors.Clear();
                string key = r.ReadString();
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
            if (MMPlr.weaponGroup == null) MMPlr.weaponGroup = [];
            else
                MMPlr.weaponGroup.Clear();
            MMPlr.weaponGroup.AddRange(list);
        }
        if (dict != null)
        {
            if (MMPlr.meleeConfigs == null) MMPlr.meleeConfigs = [];
            else
                MMPlr.meleeConfigs.Clear();
            MMPlr.meleeConfigs.AddRange(dict);
        }
        //if (MMPlr.heatMap != null && MMPlr.hsl != default)
        //    MeleeModifyPlayer.UpdateHeatMap(ref MMPlr.heatMap, MMPlr.hsl, MMPlr.ConfigurationSwoosh, MeleeModifyPlayer.GetWeaponTextureFromItem(plr.HeldItem));
        if (Main.dedServ)
        {
            Get(plrIndex, list, dict).Send(-1, plrIndex);
        }
    }
}
