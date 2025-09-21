using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ReplaceAutoPropertyWithComputedProperty

namespace CoolerItemVisualEffect.Common.Config;

public class SeverConfig : ModConfig
{
    public static SeverConfig Instance { get; private set; }

    public override void OnLoaded()
    {
        Instance = this;
        base.OnLoaded();
    }

    public override ConfigScope Mode => ConfigScope.ServerSide;

    public enum MeleeModifyLevel
    {
        Vanilla,
        VisualOnly,
        Overhaul
    }

    [DefaultValue(MeleeModifyLevel.Overhaul)]
    [DrawTicks]
    public MeleeModifyLevel meleeModifyLevel = MeleeModifyLevel.VisualOnly;

    [DefaultValue(true)] 
    public bool AutoBalanceData { get; } = true;

    public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
    {
        if (NetMessage.DoesPlayerSlotCountAsAHost(whoAmI)) return true;
        message = NetworkText.FromKey("tModLoader.ModConfigRejectChangesNotHost");
        return false;
    }
}