using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;

namespace CoolerItemVisualEffect.ProjectileEffect;

public partial class ProjectileDrawingModify
{
    private static void DrawArrows(Projectile projectile) 
    {
        var spriteBatch = Main.spriteBatch;
        var width = 30f;
        var offset = -projectile.velocity * 2;
        var alpha = .25f;
        var mainColor = Color.White;
        switch (projectile.type)
        {
            //case ProjectileID.WoodenArrowHostile:
            //case ProjectileID.WoodenArrowFriendly:
            case ProjectileID.UnholyArrow:
            case ProjectileID.ShadowFlameArrow:
            case ProjectileID.VenomArrow:
                {
                    mainColor = Color.Purple;
                    alpha = 1;
                    break;
                }
            case ProjectileID.PhantasmArrow:
            case ProjectileID.MoonlordArrow:
                {
                    alpha = .75f;
                    mainColor = Color.Lerp(Color.Cyan, Color.Green, .33f);
                    break;
                }
            case ProjectileID.FlamingArrow:
            case ProjectileID.FireArrow:
            case ProjectileID.HellfireArrow:
            case ProjectileID.DD2BetsyArrow:
                {
                    alpha = 1;
                    mainColor = Color.Lerp(Color.Orange, Color.OrangeRed, .66f);
                    break;
                }
            case ProjectileID.FrostburnArrow:
            case ProjectileID.FrostArrow:
                {
                    alpha = .75f;
                    mainColor = Color.Lerp(Color.Cyan, Color.Blue, .33f);
                    break;
                }
            case ProjectileID.CursedArrow:
            case ProjectileID.ChlorophyteArrow:
                {
                    alpha = .75f;
                    mainColor = Color.LimeGreen;
                    break;
                }
            case ProjectileID.JestersArrow:
            case ProjectileID.IchorArrow:
                {
                    alpha = .75f;
                    mainColor = Color.Lerp(Color.Yellow, Color.Orange, .33f);
                    break;
                }
            case ProjectileID.HolyArrow:
                {
                    alpha = .75f;
                    mainColor = Color.Lerp(Color.Pink, Color.HotPink, .5f);
                    break;
                }
            case ProjectileID.BoneArrowFromMerchant:
            case ProjectileID.BoneArrow:
                {
                    alpha = .5f;
                    mainColor = Color.Lerp(Color.White, Color.BurlyWood, .5f);
                    break;
                }
            case ProjectileID.BloodArrow:
                {
                    alpha = .5f;
                    mainColor = Color.Red;
                    break;
                }
            case ProjectileID.BeeArrow:
                {
                    break;
                }
            case ProjectileID.ShimmerArrow:
                {
                    alpha = 1f;
                    mainColor = Main.DiscoColor;
                    break;
                }
        }
        spriteBatch.DrawShaderTail(projectile, LogSpiralLibraryMod.BaseTex[8].Value, LogSpiralLibraryMod.AniTex[2].Value, LogSpiralLibraryMod.BaseTex[12].Value, width, offset, alpha, true, false, mainColor);
        switch (projectile.type)
        {
            case ProjectileID.PhantasmArrow:
            case ProjectileID.MoonlordArrow:
            case ProjectileID.JestersArrow:
            case ProjectileID.HolyArrow:
            case ProjectileID.DD2BetsyArrow:
            case ProjectileID.ShimmerArrow:
            case ProjectileID.CursedArrow:
            case ProjectileID.IchorArrow:
            case ProjectileID.ChlorophyteArrow:
                var u = -projectile.velocity.SafeNormalize(default);
                spriteBatch.DrawQuadraticLaser_PassNormal(projectile.Center - 24 * u, u, mainColor with { A = 255 }, LogSpiralLibraryMod.AniTex[8].Value, 48, 24, 0, 1, 0.25f, true);
                spriteBatch.DrawQuadraticLaser_PassNormal(projectile.Center - 4 * u, u, mainColor with { A = 255 } * 1.5f, LogSpiralLibraryMod.AniTex[8].Value, 64, 16, 0, 1f, 0.25f, true);
                spriteBatch.DrawQuadraticLaser_PassNormal(projectile.Center - 4 * u, u, mainColor with { A = 255 } * .5f, LogSpiralLibraryMod.AniTex[8].Value, 32, 64, 0, 1, 0.25f, true);
                break;
        }
    }
}
