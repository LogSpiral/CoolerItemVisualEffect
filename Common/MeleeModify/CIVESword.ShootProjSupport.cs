using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee.StandardMelee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.Interfaces;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.System;
using System;

namespace CoolerItemVisualEffect.MeleeModify;

public partial class CIVESword
{
    protected override void InitializeElement(ISequenceElement element)
    {
        base.InitializeElement(element);
        if (element is MeleeAction melee)
            melee.OnShootExtraProjectile += GeneralShootSupport;
        if (element is CutInfo cut)
            cut.OnStartAttackEvent += CutShootSupport;
        if (element is SwooshInfo swoosh)
            swoosh.OnStartAttackEvent += SwooshShootSupport;
        if (element is StabInfo stab)
            stab.OnStartAttackEvent += StabShootSupport;
    }

    private static void GeneralShootSupport(MeleeAction info) 
    {
        if (!info.IsLocalProjectile) return;
        if (info.Owner is not Player plr) return;
        var dmg = info.CurrentDamage;
        var type = plr.HeldItem.shoot;
        var origCount = plr.ownedProjectileCounts[type];
        plr.ownedProjectileCounts[type]++;
        if (origCount != 0 || ItemLoader.CanShoot(plr.HeldItem, plr))
            plr.ItemCheck_Shoot(plr.whoAmI, plr.HeldItem, dmg);
        plr.ownedProjectileCounts[type] = origCount;
    }
    private static void CutShootSupport(MeleeAction info) 
    {
        if (info.Owner is Player plr)
        {
            var seqPlayer = plr.GetModPlayer<SequencePlayer>();

            var dmg = info.CurrentDamage;
            if (info.StandardInfo.standardShotCooldown > 0)
            {
                var delta = info.StandardInfo.standardTimer * info.ModifyData.TimeScaler / info.CounterMax;
                var canShoot = plr.HeldItem.shoot > ProjectileID.None;

                var m = Math.Max(info.StandardInfo.standardShotCooldown, delta);
                if (canShoot || seqPlayer.cachedTime < m)
                    seqPlayer.cachedTime += delta + 1;
                if (seqPlayer.cachedTime > m)
                    seqPlayer.cachedTime = m;
                var count = (int)(seqPlayer.cachedTime / info.StandardInfo.standardShotCooldown);
                if (canShoot)
                {
                    seqPlayer.cachedTime -= info.StandardInfo.standardShotCooldown * count;
                    if (count > 0)
                    {
                        count--;
                        info.ShootExtraProjectile();
                    }
                }

                var orig = Main.MouseWorld;
                var unit = orig - plr.Center;//.SafeNormalize(default) * 32f;
                var angleMax = MathHelper.Pi / 6;
                if (count % 2 == 1)
                {
                    count--;
                    var target = plr.Center + unit.RotatedBy(angleMax * Main.rand.NextFloat(-.5f, .5f));
                    Main.mouseX = (int)(target.X - Main.screenPosition.X);
                    Main.mouseY = (int)(target.Y - Main.screenPosition.Y);
                    info.ShootExtraProjectile();
                }
                count /= 2;
                for (var i = 0; i < count; i++)
                {
                    var angle = angleMax * MathF.Pow((i + 1f) / count, 2);

                    var target = plr.Center + unit.RotatedBy(angle);
                    Main.mouseX = (int)(target.X - Main.screenPosition.X);
                    Main.mouseY = (int)(target.Y - Main.screenPosition.Y);
                    info.ShootExtraProjectile();

                    target = plr.Center + unit.RotatedBy(-angle);
                    Main.mouseX = (int)(target.X - Main.screenPosition.X);
                    Main.mouseY = (int)(target.Y - Main.screenPosition.Y);
                    info.ShootExtraProjectile();
                }
                Main.mouseX = (int)(orig.X - Main.screenPosition.X);
                Main.mouseY = (int)(orig.Y - Main.screenPosition.Y);
            }
            else
                info.ShootExtraProjectile();
        }
    }

    private static void SwooshShootSupport(MeleeAction info) 
    {
        if (info.Owner is Player plr)
        {
            var seqPlayer = plr.GetModPlayer<SequencePlayer>();

            var dmg = info.CurrentDamage;
            if (info.StandardInfo.standardShotCooldown > 0)
            {
                var delta = info.StandardInfo.standardTimer * info.ModifyData.TimeScaler / info.CounterMax;
                var canShoot = plr.HeldItem.shoot > ProjectileID.None;

                var m = Math.Max(info.StandardInfo.standardShotCooldown, delta);
                if (canShoot || seqPlayer.cachedTime < m)
                    seqPlayer.cachedTime += delta + 1;
                if (seqPlayer.cachedTime > m)
                    seqPlayer.cachedTime = m;
                var count = (int)(seqPlayer.cachedTime / info.StandardInfo.standardShotCooldown);
                if (canShoot)
                {
                    seqPlayer.cachedTime -= info.StandardInfo.standardShotCooldown * count;
                    if (count > 0)
                    {
                        count--;
                        info.ShootExtraProjectile();
                    }
                }

                var orig = Main.MouseWorld;
                var unit = orig - plr.Center;//.SafeNormalize(default) * 32f;
                var angleMax = MathHelper.Pi / 6;
                if (count % 2 == 1)
                {
                    count--;
                    var target = plr.Center + unit.RotatedBy(angleMax * Main.rand.NextFloat(-.5f, .5f));
                    Main.mouseX = (int)(target.X - Main.screenPosition.X);
                    Main.mouseY = (int)(target.Y - Main.screenPosition.Y);
                    info.ShootExtraProjectile();
                }
                count /= 2;
                for (var i = 0; i < count; i++)
                {
                    var angle = angleMax * MathF.Pow((i + 1f) / count, 2);

                    var target = plr.Center + unit.RotatedBy(angle);
                    Main.mouseX = (int)(target.X - Main.screenPosition.X);
                    Main.mouseY = (int)(target.Y - Main.screenPosition.Y);
                    info.ShootExtraProjectile();

                    target = plr.Center + unit.RotatedBy(-angle);
                    Main.mouseX = (int)(target.X - Main.screenPosition.X);
                    Main.mouseY = (int)(target.Y - Main.screenPosition.Y);
                    info.ShootExtraProjectile();
                }
                Main.mouseX = (int)(orig.X - Main.screenPosition.X);
                Main.mouseY = (int)(orig.Y - Main.screenPosition.Y);
            }
            else
                info.ShootExtraProjectile();
        }
    }

    private static void StabShootSupport(MeleeAction info) 
    {
        if (info.Owner is Player plr)
        {
            var seqPlr = plr.GetModPlayer<SequencePlayer>();
            var dmg = info.CurrentDamage;
            if (info.StandardInfo.standardShotCooldown > 0)
            {
                var delta = info.StandardInfo.standardTimer * info.ModifyData.TimeScaler / info.CounterMax;
                seqPlr.cachedTime += delta + 1;
                var count = (int)(seqPlr.cachedTime / info.StandardInfo.standardShotCooldown);
                seqPlr.cachedTime -= count * info.StandardInfo.standardShotCooldown;
                if (count > 0)
                {
                    count--;
                    info.ShootExtraProjectile();
                }
                var orig = plr.Center;
                var unit = (Main.MouseWorld - orig).SafeNormalize(default) * 64;
                for (var i = 0; i < count; i++)
                {
                    plr.Center += unit.RotatedBy(MathHelper.Pi / count * (i - (count - 1) * .5f));
                    info.ShootExtraProjectile();

                    plr.Center = orig;
                }
                if (count > 0)
                    if (Main.myPlayer == plr.whoAmI && Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        SyncPlayerPosition.Get(plr.whoAmI, plr.position).Send(-1, plr.whoAmI);
                    }
            }
            else
                info.ShootExtraProjectile();
        }
    }
}
