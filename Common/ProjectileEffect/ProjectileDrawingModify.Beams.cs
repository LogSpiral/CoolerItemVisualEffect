using LogSpiralLibrary;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using CoolerItemVisualEffect.Common.Config.Preview;

namespace CoolerItemVisualEffect.ProjectileEffect;

public partial class ProjectileDrawingModify
{
    private static void DrawBeam(Projectile projectile)
    {
        var spriteBatch = Main.spriteBatch;
        var projectileTexture = LogSpiralLibraryMod.Misc[13].Value;//GetTexture("TerraShard");
        var length = projectile.velocity.Length();
        var scaleVec = new Vector2(projectile.scale * 3 + 1f + MathHelper.Clamp(length / 4f, 0, 2), projectile.scale * 2);
        scaleVec = new Vector2(scaleVec.Y, scaleVec.X / 3f);
        scaleVec *= new Vector2(.5f, .75f);
        if (!Main.gamePaused)
        {
            for (var k = projectile.oldPos.Length - 1; k > 0; k--)
            {
                projectile.oldPos[k] = projectile.oldPos[k - 1];
                projectile.oldRot[k] = projectile.oldRot[k - 1];
            }
            projectile.oldPos[0] = projectile.Center;
            projectile.oldRot[0] = projectile.rotation;
        }
        var mainColor = projectile.type switch
        {
            ProjectileID.TerraBeam => Color.LimeGreen,
            ProjectileID.EnchantedBeam => Color.Cyan,
            ProjectileID.LightBeam => Color.HotPink,
            ProjectileID.SwordBeam => Color.Yellow,
            ProjectileID.InfluxWaver => Color.Lerp(Color.LightCyan, Color.Cyan, .5f),
            ProjectileID.SkyFracture => Color.Lerp(Color.LightCyan, Color.Cyan, .5f),
            _ => default
        };

        #region offsetAlpha

        {
            if (!ProjectileModificationPreview.PVDrawing)
            {
                var vCenter = projectile.Center;
                var t = 0;
                var tile = Framing.GetTileSafely((int)vCenter.X / 16, (int)vCenter.Y / 16);

                while (t < 30 && !(tile.HasTile && Main.tileSolid[tile.TileType]))
                {
                    vCenter += projectile.velocity;
                    t++;
                    Point coord = new((int)vCenter.X / 16, (int)vCenter.Y / 16);
                    if (coord.X > 0 && coord.X < Main.tile.Width && coord.Y > 0 && coord.Y < Main.tile.Height)
                        tile = Framing.GetTileSafely(coord);
                }
                mainColor *= MathHelper.Clamp((t - 1) / 30f, 0, 1);
            }
        }

        #endregion offsetAlpha

        var unit = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
        var center = projectile.Center - Main.screenPosition;
        spriteBatch.Draw(projectileTexture, center - unit * 24, null, mainColor with { A = 0 }, projectile.rotation - MathHelper.PiOver4 * 3, new Vector2(36), scaleVec * new Vector2(.75f, 1.5f), SpriteEffects.None, 0f);
        spriteBatch.Draw(projectileTexture, center - unit * 24, null, Color.White with { A = 0 }, projectile.rotation - MathHelper.PiOver4 * 3, new Vector2(36), scaleVec * new Vector2(.5f, 1), SpriteEffects.None, 0f);
        spriteBatch.DrawEffectLine(projectile.Center - unit * 24, projectile.velocity.SafeNormalize(default), mainColor, LogSpiralLibraryMod.BaseTex[12].Value, 1, 0, 96, 15);
        var asset = TextureAssets.Projectile[projectile.type];
        if (!asset.IsLoaded)
            Main.instance.LoadProjectile(projectile.type);
        var projTex = asset.Value;
        Rectangle? rect = projectile.type == ProjectileID.SkyFracture ? projTex.Frame(14, 1, projectile.frame) : null;
        for (var n = 0; n < 4; n++)
        {
            var offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(Main.rand.NextFloat(12f)) - projectile.velocity * 3;
            spriteBatch.Draw(projTex, center + offset, rect, Color.White with { A = 0 } * .5f * (1 - projectile.alpha / 255f), projectile.rotation, (rect != null ? rect.Value.Size() : projTex.Size()) * .5f, projectile.scale, 0, 0);
        }
        spriteBatch.Draw(projTex, center - projectile.velocity * 3, rect, Color.White with { A = 0 } * (1 - projectile.alpha / 255f), projectile.rotation, (rect != null ? rect.Value.Size() : projTex.Size()) * .5f, projectile.scale, 0, 0);

    }

}
