using CoolerItemVisualEffect.Config;
using System;
using System.Linq;

namespace CoolerItemVisualEffect.MeleeModify;

public class BanVanillaSwooshEffectProj : GlobalProjectile
{
    private static int[] VanillaSlashProjectiles { get; } =
        [
            ProjectileID.NightsEdge,
            ProjectileID.Excalibur,
            ProjectileID.TrueExcalibur,
            ProjectileID.TheHorsemansBlade,
            ProjectileID.TerraBlade2
        ];

    public override void AI(Projectile projectile)
    {
        Player player = Main.player[projectile.owner];
        var mplr = player.GetModPlayer<MeleeModifyPlayer>();
        if (mplr.BeAbleToOverhaul && VanillaSlashProjectiles.Contains(projectile.type))
            projectile.Kill();
        base.AI(projectile);
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        if (SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.VisualOnly 
            && VanillaSlashProjectiles.Contains(projectile.type) 
            && Main.player[projectile.owner]
                .GetModPlayer<MeleeModifyPlayer>()
                .ConfigurationSwoosh
                .SwordModifyActive)
            return false;
        return base.PreDraw(projectile, ref lightColor);
    }
}
