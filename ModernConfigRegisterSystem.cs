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

        AddModernConfigTitle(qot, Mod, Language.GetOrRegister("Mods.CoolerItemVisualEffect.Configs.ModernConfigTitle"));

        SetAboutPage(qot, Mod, () => "非常酷大剑转转转的配置中心！！！", ItemID.IronShortsword, null, () => "关于大剑", () => "酷酷酷酷酷");

        RegisterCategory(qot, Mod, [
            (SeverConfig.Instance,[nameof(SeverConfig.meleeModifyLevel)]),
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
        ItemID.TitaniumSword, null, () => "近战设置", () => "拜托这早就不只是视觉上的修改了");

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
        ItemID.RainbowWallpaper, null, () => "渲染设置", () => "说实在的没有预览功能再怎么好看的配置面板也没用(");

        RegisterCategory(qot, Mod, MeleeConfig.Instance,
        [
             nameof(MeleeConfig.UsePreview),
             nameof(MeleeConfig.distortConfigs),
             nameof(MeleeConfig.maskConfigs),
             nameof(MeleeConfig.dyeConfigs),
             nameof(MeleeConfig.bloomConfigs)
        ],
        ItemID.LastPrism, null, () => "特效设置", () => "亮瞎眼了啊喂，显卡要冒烟了啊喂");

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
        ItemID.Cog, null, () => "杂项设置", () => "非常水");
    }

    private void UnloadModernConfig()
    {
        if (Main.dedServ || !ModLoader.TryGetMod("ImproveGame", out var qot)) return;
        qot.Call("RemoveCategory", this);
        qot.Call("RemoveAboutPage", this);
        //这个我不是很确定是否需要
    }
}
