using CoolerItemVisualEffect.Common.Config.Preview;
using LogSpiralLibrary.CodeLibrary.ConfigModification;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.Config;

[HorizonOverflowEnable]
public class MiscConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    //[Header("MiscPart")]
    public static MiscConfig Instance => ModContent.GetInstance<MiscConfig>();

    [DefaultValue(true)]
    [CustomPreview<UsePVPreview>]
    public bool usePreview = true;

    [DefaultValue(true)]
    [CustomPreview<WeaponDisplayPreview>]
    public bool useWeaponDisplay = true;

    [DefaultValue(true)]
    [CustomPreview<WeaponDisplayPreview>]
    public bool firstWeaponDisplay = true;

    [Increment(0.05f)]
    [Range(0.5f, 2f)]
    [DefaultValue(1f)]
    [Slider]
    [CustomPreview<WeaponScalePreview>]
    public float weaponScale = 1f;

    [DefaultValue(false)]
    [CustomPreview<ItemEffectPreview>]
    public bool ItemDropEffectActive = false;

    [DefaultValue(false)]
    [CustomPreview<ItemEffectPreview>]
    public bool ItemInventoryEffectActive = false;

    [DefaultValue(true)]
    [CustomPreview<ProjectileModificationPreview>]
    public bool VanillaProjectileDrawModifyActive = true;

    [DefaultValue(false)]
    [CustomPreview<TeleportModificationPreview>]
    public bool TeleprotEffectActive = false;
}