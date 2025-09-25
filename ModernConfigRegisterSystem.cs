using Terraria.Localization;
using CoolerItemVisualEffect.Common.Config;
using static LogSpiralLibrary.ImproveGame_ModernConfigCrossModHelper;
namespace CoolerItemVisualEffect;

public class ModernConfigRegisterSystem : ModSystem
{
    public override void PostSetupContent() => RegisterModernConfig();
    public override void Unload() => UnloadModernConfig();

    private void RegisterModernConfig()
    {
        if (Main.dedServ || !ModLoader.TryGetMod("ImproveGame", out var qot)) return;

        AddModernConfigTitle(qot, Mod, Language.GetOrRegister("Mods.CoolerItemVisualEffect.ModernConfig.ModernConfigTitle"));
        static string GetLocalization(string suffix) => Language.GetOrRegister($"Mods.CoolerItemVisualEffect.ModernConfig.{suffix}").Value;
        SetAboutPage(
            qot, 
            Mod,
            () => GetLocalization("AboutPage.Content"), 
            ItemID.IronShortsword, 
            null, 
            () => GetLocalization("AboutPage.Label"), 
            () => GetLocalization("AboutPage.Tooltip"));

        RegisterCategory(qot, Mod, [
            (ServerConfig.Instance,[nameof(ServerConfig.meleeModifyLevel)]),
        (MeleeConfig.Instance,
        [
             nameof(MeleeConfig.UsePreview),
             nameof(MeleeConfig.useRenderEffectPVInOtherConfig),
             nameof(MeleeConfig.SwordModifyActive),
             nameof(MeleeConfig.swooshActionStyle),
             nameof(MeleeConfig.swooshActionStyle),
             nameof(MeleeConfig.baseIndexSwoosh),
             nameof(MeleeConfig.animateIndexSwoosh),
             nameof(MeleeConfig.baseIndexStab),
             nameof(MeleeConfig.animateIndexStab),
             nameof(MeleeConfig.swooshTimeLeft),
             nameof(MeleeConfig.shake),
             nameof(MeleeConfig.dustQuantity)
        ])],
        ItemID.TitaniumSword, 
        null, 
        () => GetLocalization("MeleeConfig.Label"),
        () => GetLocalization("MeleeConfig.Tooltip"));

        RegisterCategory(qot, Mod, MeleeConfig.Instance,
        [
             nameof(MeleeConfig.UsePreview),
             nameof(MeleeConfig.useRenderEffectPVInOtherConfig),
             nameof(MeleeConfig.weaponExtraLight),
             nameof(MeleeConfig.colorVector),
             nameof(MeleeConfig.alphaFactor),
             nameof(MeleeConfig.heatMapCreateStyle),
             nameof(MeleeConfig.heatMapFactorStyle),
             nameof(MeleeConfig.byFuncData),
             nameof(MeleeConfig.rgbData),
             nameof(MeleeConfig.hslData),
             nameof(MeleeConfig.designateData),
             nameof(MeleeConfig.directOfHeatMap)
        ],
        ItemID.RainbowWallpaper, 
        null,
        () => GetLocalization("RenderingConfig.Label"), 
        () => GetLocalization("RenderingConfig.Tooltip"));

        RegisterCategory(qot, Mod, MeleeConfig.Instance,
        [
             nameof(MeleeConfig.UsePreview),
             nameof(MeleeConfig.distortConfigs),
             nameof(MeleeConfig.maskConfigs),
             nameof(MeleeConfig.dyeConfigs),
             nameof(MeleeConfig.bloomConfigs)
        ],
        ItemID.LastPrism,
        null,
        () => GetLocalization("EffectConfig.Label"),
        () => GetLocalization("EffectConfig.Tooltip"));

        RegisterCategory(qot, Mod, MiscConfig.Instance,
        [
             nameof(MiscConfig.UsePreview),
             nameof(MiscConfig.useWeaponDisplay),
             nameof(MiscConfig.firstWeaponDisplay),
             nameof(MiscConfig.weaponScale),
             nameof(MiscConfig.ItemDropEffectActive),
             nameof(MiscConfig.ItemInventoryEffectActive),
             nameof(MiscConfig.VanillaProjectileDrawModifyActive),
             nameof(MiscConfig.TeleportEffectActive)
        ],
        ItemID.Cog,
        null, 
        () => GetLocalization("MiscConfig.Label"), 
        () => GetLocalization("MiscConfig.Tooltip"));
    }

    private void UnloadModernConfig()
    {
        if (Main.dedServ || !ModLoader.TryGetMod("ImproveGame", out var qot)) return;
        qot.Call("RemoveCategory", this);
        qot.Call("RemoveAboutPage", this);
        //这个我不是很确定是否需要
    }
}
