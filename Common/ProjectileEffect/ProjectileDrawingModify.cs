using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.Config.Preview;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace CoolerItemVisualEffect.ProjectileEffect;

/// <summary>
/// 部分弹幕绘制的修改
/// </summary>
public partial class ProjectileDrawingModify : GlobalProjectile
{
    public override Color? GetAlpha(Projectile projectile, Color lightColor)
    {
        if (ProjectileModificationPreview.PVDrawing)
        {
            return lightColor;
        }
        return base.GetAlpha(projectile, lightColor);
    }



    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        if (!MiscConfig.Instance.VanillaProjectileDrawModifyActive) goto mylabel;
        switch (projectile.type)
        {
            case ProjectileID.NightBeam:
            //case ProjectileID.DeathSickle:

            case ProjectileID.Starfury:
            case ProjectileID.StarCannonStar:
            case ProjectileID.SuperStar:
            case ProjectileID.StarWrath:
            case ProjectileID.FallingStar:
            case ProjectileID.HallowStar:
            case ProjectileID.ManaCloakStar:
            case ProjectileID.BeeCloakStar:
            case ProjectileID.StarVeilStar:
            case ProjectileID.StarCloakStar:

            case ProjectileID.EnchantedBoomerang:
            case ProjectileID.IceBoomerang:
            case ProjectileID.WoodenBoomerang:
            case ProjectileID.Flamarang:
            case ProjectileID.Bananarang:
            case ProjectileID.Shroomerang:
                {
                    DrawTails(projectile);
                    goto mylabel;
                }
            case ProjectileID.TerraBeam:
            case ProjectileID.EnchantedBeam:
            case ProjectileID.LightBeam:
            case ProjectileID.SwordBeam:
            case ProjectileID.InfluxWaver:
            case ProjectileID.SkyFracture:
                {
                    DrawBeam(projectile);
                    return false;
                }
            case ProjectileID.Meowmere:
                {
                    goto mylabel;
                }
            case ProjectileID.WoodenArrowHostile:
            case ProjectileID.WoodenArrowFriendly:
            case ProjectileID.VenomArrow:
            case ProjectileID.UnholyArrow:
            case ProjectileID.ShadowFlameArrow:
            case ProjectileID.PhantasmArrow:
            case ProjectileID.MoonlordArrow:
            case ProjectileID.JestersArrow:
            case ProjectileID.IchorArrow:
            case ProjectileID.HolyArrow:
            case ProjectileID.HellfireArrow:
            case ProjectileID.FrostburnArrow:
            case ProjectileID.FrostArrow:
            case ProjectileID.FlamingArrow:
            case ProjectileID.FireArrow:
            case ProjectileID.DD2BetsyArrow:
            case ProjectileID.CursedArrow:
            case ProjectileID.ChlorophyteArrow:
            case ProjectileID.BoneArrowFromMerchant:
            case ProjectileID.BoneArrow:
            case ProjectileID.BloodArrow:
            case ProjectileID.ShimmerArrow:
                //case ProjectileID.BeeArrow:
                {
                    DrawArrows(projectile);
                    goto mylabel;
                }
        }
    mylabel:
        return base.PreDraw(projectile, ref lightColor);
    }
}