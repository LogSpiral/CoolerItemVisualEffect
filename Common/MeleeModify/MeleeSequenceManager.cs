using CoolerItemVisualEffect.Common.Config;
using log4net.Core;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee.ExtendedMelee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee.StandardMelee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.BuiltInGroups.Arguments;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.Definition;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.System;
using NetSimplified;
using PropertyPanelLibrary.EntityDefinition;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public static class MeleeSequenceManager
{
    private readonly static Dictionary<string, Sequence> _presetSequences = [];

    public static IReadOnlyDictionary<string, Sequence> PresetSequences => _presetSequences;

    internal readonly static Dictionary<string, Sequence> _serverSequences = [];

    public static IReadOnlyDictionary<string, Sequence> ServerSequences => _serverSequences;

    private readonly static Dictionary<string, Sequence> _standardSequences = [];

    public static IReadOnlyDictionary<string, Sequence> StandardSequences => _standardSequences;


    private readonly static Dictionary<string, Sequence> _localSequences = [];

    public static IReadOnlyDictionary<string, Sequence> LocalSequences => _localSequences;


    public static void ReloadPresets()
    {
        _presetSequences.Clear();

        var mod = CoolerItemVisualEffectMod.Instance;
        foreach (var name in mod.GetFileNames())
        {
            if (!name.StartsWith($"PresetSequences/MeleeAction") || !name.EndsWith(".xml"))
                continue;
            var fileName = Path.GetFileNameWithoutExtension(name);

            var fullName = $"CoolerItemVisualEffect/{fileName}_Presets";
            using MemoryStream stream = new(mod.GetFileBytes(name));
            var sequence = SequenceManager<MeleeAction>.RegisterSingleSequence(fullName, stream);
            sequence.Data.Hidden = true;

            _presetSequences.Add(fileName, sequence);
        }
    }

    public static void RefillServerSequences()
    {
        _serverSequences.Clear();
        foreach (var sequence in SequenceManager<MeleeAction>.Instance.Sequences.Values)
        {
            if (sequence.Data.ModDefinition.Name is not nameof(CoolerItemVisualEffect)) continue;

            _serverSequences[sequence.Data.FileName] = sequence;
        }
    }


    public static IReadOnlyDictionary<string, Sequence> GetAvailableSequences()
    {
        var level = ServerConfig.Instance.meleeModifyLevel;
        return level switch
        {
            ServerConfig.MeleeModifyLevel.PresetOnly => PresetSequences,
            ServerConfig.MeleeModifyLevel.ServerProvide => ServerSequences,
            ServerConfig.MeleeModifyLevel.StandardOnly => StandardSequences,
            ServerConfig.MeleeModifyLevel.Overhaul => LocalSequences,
            _ => null
        };
    }

    public static void RefreshLocalSequencesForAll()
    {
        foreach (var sequence in SequenceManager<MeleeAction>.Instance.Sequences.Values)
            RefreshLocalSequences(sequence, nameof(MeleeAction));
    }

    public static void RefreshLocalSequences(Sequence sequence, string elementName)
    {
        if (elementName is not nameof(MeleeAction)) 
            return;

        var keyName = $"{sequence.Data.ModDefinition.Name}/{sequence.Data.FileName}";

        if (StandardCheck(sequence))
            _standardSequences[keyName] = sequence;
        else
            _standardSequences.Remove(keyName);

        if (LocalCheck(sequence))
            _localSequences[keyName] = sequence;
        else
            _localSequences.Remove(keyName);

         
    }

    private static bool StandardCheck(Sequence sequence)
    {
        if (!ModCheck(sequence)) 
            return false;

        foreach (var group in sequence.Groups)
        {
            foreach (var pair in group.Contents)
            {

                if (!pair.Wrapper.Available) continue;

                if (pair.Wrapper.Sequence is Sequence subSequence && StandardCheck(subSequence))
                    return true;

                if (pair.Wrapper.Element is MeleeAction  action && action.Category != "LsLibrary" && action.Category != "Extended")
                    return false;

            }
        }
        return true;
    }

    private static bool LocalCheck(Sequence sequence)
    {
        if (!ModCheck(sequence)) return false;

        foreach (var group in sequence.Groups)
        {
            foreach (var pair in group.Contents)
            {

                if (!pair.Wrapper.Available) continue;

                if (pair.Wrapper.Sequence is Sequence subSequence && LocalCheck(subSequence))
                    return true;

            }
        }
        return true;
    }

    private static bool ModCheck(Sequence sequence) => sequence.Data is null || sequence.Data.ModDefinition.Name is nameof(LogSpiralLibrary) or nameof(CoolerItemVisualEffect);
}

public class CIVESequenceDefinition(string name) : EntityDefinition(nameof(CoolerItemVisualEffect), name)
{
    private static ServerConfig ServerConfig => ServerConfig.Instance;
    private static ServerConfig.MeleeModifyLevel ModifyLevel => ServerConfig.meleeModifyLevel;
    public override int Type
    {
        get
        {
            if (MeleeSequenceManager.GetAvailableSequences() is not { } dictionary) return -1;
            int n = 0;
            foreach (var pair in dictionary.Keys)
            {
                if (Name == pair)
                    return n;
                n++;
            }
            return -1;

        }
    }
    public override bool IsUnloaded => Type < 0;

    public static CIVESequenceDefinition Load(TagCompound tag) => new(tag.GetString("name"));

    public static readonly Func<TagCompound, CIVESequenceDefinition> DESERIALIZER = Load;
    public override string DisplayName => ModifyLevel < ServerConfig.MeleeModifyLevel.PresetOnly ? "N/A" :
                                          GetSequence()?.Data.DisplayName ?? Language.GetTextValue("LegacyInterface.23");

    public Sequence GetSequence()
    {
        if (IsUnloaded
            || MeleeSequenceManager.GetAvailableSequences() is not { } dictionary
            || !dictionary.TryGetValue(Name, out var sequence)) 
            return null;
        return sequence;
    }

    public static CIVESequenceDefinition FromLSLSequenceDefinition(SequenceDefinition<MeleeAction> definition)
    {
        if (definition.IsUnloaded) return new("");
        var level = ServerConfig.Instance.meleeModifyLevel;
        return level switch
        {
            ServerConfig.MeleeModifyLevel.PresetOnly or ServerConfig.MeleeModifyLevel.ServerProvide => new CIVESequenceDefinition(definition.Name),
            ServerConfig.MeleeModifyLevel.StandardOnly or ServerConfig.MeleeModifyLevel.Overhaul => new CIVESequenceDefinition($"{definition.Mod}/{definition.Name}"),
            _ => new(""),
        };
    }

    public override string ToString() => Name;
}
public class CIVESequenceDefinitionHandler : EntityDefinitionCommonHandler
{
    public override UIView CreateChoiceView(PropertyOption.IMetaDataHandler metaData)
    {
        return OptionChoice = new SUIDEfinitionTextOption() { Definition = metaData.GetValue() as EntityDefinition };
    }

    protected override void FillingOptionList(List<SUIEntityDefinitionOption> options)
    {

        if (MeleeSequenceManager.GetAvailableSequences() is not { } dictionary) return;

        foreach (var pair in dictionary)
            options.Add(new SUIDEfinitionTextOption() { Definition = new CIVESequenceDefinition(pair.Key) });
    }
}

public class SyncServerSequence : NetModule
{
    private byte _playerIndex;
    private Dictionary<string, byte[]> _sequenceData;
    public static SyncServerSequence Get(int playerIndex)
    {
        var packet = NetModuleLoader.Get<SyncServerSequence>();
        packet._playerIndex = (byte)playerIndex;
        return packet;
    }

    public override void Send(ModPacket p)
    {
        p.Write(_playerIndex);
        if (Main.dedServ)
        {
            p.Write((ushort)_sequenceData.Count);
            foreach (var data in _sequenceData)
            {
                p.Write(data.Key);
                p.Write(data.Value.Length);
                p.Write(data.Value);
            }
        }
        base.Send(p);
    }
    public override void Read(BinaryReader r)
    {
        _playerIndex = r.ReadByte();

        if (Main.dedServ) return;

        _sequenceData = [];
        int count = r.ReadUInt16();
        for (int k = 0; k < count; k++)
        {
            string key = r.ReadString();
            int length = r.ReadInt32();
            byte[] data = r.ReadBytes(length);
            _sequenceData.Add(key, data);
        }
    }
    public override void Receive()
    {
        if (Main.dedServ)
        {
            var packet = Get(_playerIndex);

            var dictionary = packet._sequenceData = [];
            foreach (var pair in MeleeSequenceManager.ServerSequences)
            {
                using MemoryStream stream = new();
                using XmlWriter writer = XmlWriter.Create(stream);
                SequenceGlobalManager.Serializer.Serialize(writer, pair.Value);
                dictionary.Add(pair.Key, stream.ToArray());
            }

            packet.Send(_playerIndex);
        }
        else
        {
            MeleeSequenceManager._serverSequences.Clear();
            foreach (var pair in _sequenceData)
            {
                using MemoryStream stream = new(pair.Value);
                var result = SequenceManager<MeleeAction>.RegisterSingleSequence($"{nameof(CoolerItemVisualEffect)}/{pair.Key}_Server", stream);
                result.Data.Hidden = true;
                MeleeSequenceManager._serverSequences[pair.Key] = result;
            }
        }
    }
}