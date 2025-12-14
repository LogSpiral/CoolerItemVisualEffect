using CoolerItemVisualEffect.Common.Config.NetSync;
using CoolerItemVisualEffect.Common.MeleeModify;
using CoolerItemVisualEffect.Common.WeaponGroup;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.Definition;
using PropertyPanelLibrary.EntityDefinition;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces;
using SilkyUIFramework.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.UIBase.WeaponGroup;

public partial class DefaultWeaponGroup : UIElementGroup
{
    public DefaultWeaponGroup()
    {
        InitializeComponent();
        NameBox.Text = Language.GetTextValue("Mods.CoolerItemVisualEffect.WeaponGroup.DefaultGroup"); // 本地化
        DataPanel.Filler = new ObjectMetaDataFiller(new DefaultGroupSetHelper());
        DataPanel.OnUpdate += delegate
        {
            DataPanel.SetHeight(DataPanel.OptionList.Container.Bounds.Height + 24);
        };
    }
}
public class DefaultGroupSetHelper : IMemberLocalized
{
    private MeleeModifyPlayer ModifyPlayer { get; } = Main.gameMenu ? null : Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
    public bool IsModifyActive
    {
        get => ModifyPlayer.IsModifyActiveDefaultGroup;
        set
        {
            ModifyPlayer.IsModifyActiveDefaultGroup = value;
            SaveData();
            if (Main.netMode == NetmodeID.MultiplayerClient)
                SyncMeleeModifyActive.Get(Main.myPlayer, va).Send(-1, Main.myPlayer);
        }
    }

    [TypeConverter(typeof(ToFromStringConverter<SequenceDefinition<MeleeAction>>))]
    [CustomEntityDefinitionHandler<SequenceDefinitionHandler<MeleeAction>>]
    public SequenceDefinition<MeleeAction> SwooshActionStyle
    {
        get => ModifyPlayer.SwooshActionStyleDefaultGroup;
        set
        {
            ModifyPlayer.SwooshActionStyleDefaultGroup = value;
            SaveData();
        }
    }

    string IMemberLocalized.LocalizationRootPath { get; } = $"Mods.{nameof(CoolerItemVisualEffect)}.WeaponGroup";
    IReadOnlyList<string> IMemberLocalized.LocalizationSuffixes { get; } = ["Label"];

    private void SaveData()
    {
        var defaultGroupFilePath = Path.Combine(LoadHelper.GroupSavePath, "DefaultGroup.txt");
        StringBuilder builder = new();
        builder.AppendLine(IsModifyActive.ToString());
        builder.AppendLine(SwooshActionStyle.ToString());
        File.WriteAllText(defaultGroupFilePath, builder.ToString());
    }
}
