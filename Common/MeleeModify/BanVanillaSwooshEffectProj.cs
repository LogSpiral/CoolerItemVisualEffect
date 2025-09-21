using CoolerItemVisualEffect.Common.Config;
using System;

namespace CoolerItemVisualEffect.Common.MeleeModify;

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
        var player = Main.player[projectile.owner];
        var mplr = player.GetModPlayer<MeleeModifyPlayer>();
        if (mplr.BeAbleToOverhaul && VanillaSlashProjectiles.Contains(projectile.type))
            projectile.Kill();
        base.AI(projectile);
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        if (ServerConfig.Instance.meleeModifyLevel == ServerConfig.MeleeModifyLevel.VisualOnly 
            && VanillaSlashProjectiles.Contains(projectile.type) 
            && Main.player[projectile.owner]
                .GetModPlayer<MeleeModifyPlayer>()
                .ConfigurationSwoosh
                .SwordModifyActive)
            return false;
        return base.PreDraw(projectile, ref lightColor);
    }
}
