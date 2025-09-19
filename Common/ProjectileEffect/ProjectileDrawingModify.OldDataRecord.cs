namespace CoolerItemVisualEffect.ProjectileEffect;
public partial class ProjectileDrawingModify
{
    public override void PostAI(Projectile projectile)
    {
        switch (projectile.type)
        {
            case ProjectileID.NightBeam:
            case ProjectileID.DeathSickle:

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
                {
                    if (ProjectileID.Sets.TrailingMode[projectile.type] == -1)
                    {
                        for (int n = projectile.oldPos.Length - 1; n > 0; n--)
                        {
                            projectile.oldPos[n] = projectile.oldPos[n - 1];
                            projectile.oldRot[n] = projectile.oldRot[n - 1];
                        }
                        projectile.oldPos[0] = projectile.Center;
                        projectile.oldRot[0] = projectile.rotation;
                    }
                    break;
                }
        }
        base.PostAI(projectile);
    }
}
