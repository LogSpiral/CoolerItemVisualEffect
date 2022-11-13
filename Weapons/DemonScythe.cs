using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolerItemVisualEffect.Weapons
{
    public class DemonScythe : ModItem
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.crit = 21;
            Item.width = 50;
            Item.height = 66;
            Item.rare = 4;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 6;
            Item.useStyle = 1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.shoot = ModContent.ProjectileType<DemonScythe_Proj>();
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
    public class DemonScythe_Proj : VertexHammerProj
    {
        public override bool Charged => base.Charged;//controlState == 1 ? timeCount % MaxTime >= MaxTime * .75f : 
        public override bool Charging => base.Charging;
        public override bool WhenVertexDraw => base.WhenVertexDraw;
        public override float timeCount
        {
            get => controlState == 2 ? base.timeCount : projectile.ai[0];
            set
            {
                if (controlState == 2)
                    base.timeCount = value;
                else
                {
                    projectile.ai[0] = value;
                }
            }
        }
        public override string Texture => base.Texture.Replace("_Proj", "");
        public override string HammerName => "恶魔之镰";
        public override float MaxTime => controlState == 1 ? 30 : 48;
        public override float factor => ((projectile.ai[0] % MaxTime == 0 && projectile.ai[0] > 0 ? MaxTime - 1 : projectile.ai[0] % MaxTime)) / MaxTime;
        public override Vector2 CollidingSize => base.CollidingSize;
        //public override Vector2 projCenter => base.projCenter + new Vector2(Player.direction * 16, -16);
        public override Vector2 CollidingCenter => base.CollidingCenter;//new Vector2(projTex.Size().X / 3 - 16, 16)
        public override Vector2 DrawOrigin => base.DrawOrigin + new Vector2(16,-16);
        public override Color color => base.color;
        //public override Color VertexColor(float time) => Color.Lerp(Color.DarkGreen, UpgradeValue(Color.Brown, Color.Green), time);//Color.Lerp(UpgradeValue(Color.Brown, Color.Green), Color.DarkGreen, time)
        public override float MaxTimeLeft => 6;
        public override float Rotation
        {
            get
            {
                //Main.NewText(timeCount);
                float theta = ((float)Math.Pow(factor, 2)).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2 - MathHelper.Pi / 8);
                if (Player.controlUseItem)
                {
                    theta = -MathHelper.Pi / 8 * (3 + 2 * (1 - factor).HillFactor2());
                }
                if (projectile.ai[1] > 0)
                {
                    if (Charged)
                    {
                        //Main.NewText(projectile.ai[1] / MaxTimeLeft / factor);
                        theta = (projectile.ai[1] / MaxTimeLeft / factor).Lerp(theta, MathHelper.Pi / 8 * 3);
                        //return player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;
                    }
                    else
                    {
                        theta = ((timeCount - projectile.ai[1]) / MaxTime).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2);
                        //return player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;
                    }
                }
                else
                {
                    if (projectile.ai[0] > MaxTime)
                    {
                        theta = ((float)Math.Pow(factor, 2)).Lerp(MathHelper.Pi / 8 * 3, MathHelper.Pi / 8 * 11);
                    }
                    //else if (Player.controlUseItem)
                    //{
                    //    theta = -MathHelper.Pi / 8 * (3 + 2 * (1 - factor).HillFactor2());
                    //}
                }
                return Player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;

            }
        }
        public override bool UseRight => true;
        public override bool UseLeft => true;
        public override (int X, int Y) FrameMax => (1, 1);
        public override void Kill(int timeLeft)
        {
            //if (factor == 1)
            //{
            //    Projectile.NewProjectile(projectile.GetSource_FromThis(), vec, default, ModContent.ProjectileType<HolyExp>(), player.GetWeaponDamage(player.HeldItem) * 3, projectile.knockBack, projectile.owner);
            //}

            if (Charged || (controlState == 1 && (timeCount - 3) % MaxTime >= MaxTime * .75f))
            {
                bool large = (controlState == 2 || (int)(timeCount / MaxTime) % 4 == 3);
                var modplr = Player.GetModPlayer<CoolerItemVisualEffectPlayer>();
                modplr.NewUltraSwoosh(Color.DarkRed, Player.HeldItem.type, 1, 1
                    , large ? (byte)30 : (byte)15, ((projTex.Size() / new Vector2(FrameMax.X, FrameMax.Y)).Length() * Player.GetAdjustedItemScale(Player.HeldItem) - (new Vector2(0, projTex.Size().Y / FrameMax.Y) - DrawOrigin).Length()) * (large ? 0.625f : .5f), _negativeDir: false
                    , _rotation: 0, xscaler: large ? 1.25f : 1, heat: HeatMap, angleRange: (Player.direction == 1 ? -1.125f : 2.125f, Player.direction == 1 ? 3f / 8 : 0.625f));//MathHelper.Pi / 8 * 3, -MathHelper.PiOver2 - MathHelper.Pi / 8
                modplr.UpdateVertex();
            }

            //Main.NewText(((timeCount - 3) % MaxTime , MaxTime * .75f));
            //base.Kill(timeLeft);
        }
        public override void OnCharging(bool left, bool right)
        {
            if (left)
            {
                if (timeCount % MaxTime == MaxTime - 1)
                {
                    projectile.ai[1]++;
                }
            }
        }
        public override void OnRelease(bool charged, bool left)
        {
            if (Charged)
            {
                if ((int)projectile.ai[1] == 1)
                {
                    OnChargedShoot();
                }
            }
            if ((int)projectile.ai[1] == 1)
            {
                projectile.damage = 0;
                if (Charged || (controlState == 1 && timeCount % MaxTime >= MaxTime * .75f))
                {
                    projectile.damage = (int)(Player.GetWeaponDamage(Player.HeldItem) * ((controlState == 2 || (int)(timeCount / MaxTime) % 4 == 3) ? 4f : 2f));
                    SoundEngine.PlaySound(SoundID.Item71);
                }
            }
            projectile.ai[1]++;
            if (projectile.ai[1] > (Charged ? (MaxTimeLeft * factor) : timeCount % MaxTime))
            {
                if (left && Player.controlUseItem)
                {
                    projectile.ai[1] = 0;
                    projectile.ai[0]++;
                    Kill(projectile.timeLeft);

                }
                else
                {
                    projectile.Kill();
                }
            }
        }
        public override Texture2D HeatMap => ModContent.Request<Texture2D>(base.Texture.Replace("DemonScythe_Proj", "HeatMap_DemonScythe")).Value;
        public override void RenderInfomation(ref (float M, float Intensity, float Range) useBloom, ref (float M, float Range, Vector2 director) useDistort, ref (Texture2D fillTex, Vector2 texSize, Color glowColor, Color boundColor, float tier1, float tier2, Vector2 offset, bool lightAsAlpha) useMask)
        {
            var config = Player.GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh;
            useBloom = (0, .25f, 6);
            useDistort = (0, config.distortSize, Rotation.ToRotationVector2() * -0.15f * config.distortFactor);
        }
        public override void VertexInfomation(ref bool additive, ref int indexOfGreyTex, ref float endAngle, ref bool useHeatMap)
        {
            additive = false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int n = 0; n < 3; n++)
            {
                if (Main.rand.NextBool(n + 1))
                {
                    if ((target.CanBeChasedBy() && Main.rand.NextBool(3)) || controlState == 2 || (int)(timeCount / MaxTime) % 4 == 3)
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), (CollidingCenter - DrawOrigin).RotatedBy(Rotation) + projCenter, default, ModContent.ProjectileType<DemonScythe_1>(), projectile.damage / 2, projectile.knockBack * 1.5f, projectile.owner, target.whoAmI, Main.rand.NextFloat(0, MathHelper.TwoPi));//
                    }
                }

            }

        }
    }
    public class DemonScythe_1 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 42;
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 7;
        }
        public override void AI()
        {
            if (target.active && Projectile.timeLeft > 21)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center + (Projectile.ai[1]).ToRotationVector2() * 16, 0.1f);
            }
            if (Projectile.timeLeft == 22)
            {
                SoundEngine.PlaySound(SoundID.Item71);
                var plr = Main.player[Projectile.owner];
                var modplr = plr.GetModPlayer<CoolerItemVisualEffectPlayer>();
                var dir = -1;//Math.Sign(Projectile.Center.X - target.Center.X)
                modplr.NewUltraSwoosh(Color.DarkRed, plr.HeldItem.type, 1, 1, 30, 24, Projectile.Center, ModContent.Request<Texture2D>(base.Texture.Replace("DemonScythe_1", "HeatMap_DemonScythe")).Value, dir == 1, null, Projectile.ai[1], 1.5f, (dir == 1 ? -1.125f : -2.125f, dir == 1 ? 3f / 8 : -0.625f));
                modplr.UpdateVertex();
            }
        }
        public override void PostAI()
        {

        }
        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }
        NPC target => Main.npc[(int)Projectile.ai[0]];
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.timeLeft < 24 && Projectile.timeLeft > 18 && Projectile.penetrate > 0 && target.immune[Projectile.owner] < 1;// 
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            var alpha = ((float)Projectile.timeLeft).SymmetricalFactor(21, 12);
            const float cValue = 3;
            var fac = Projectile.timeLeft < 18 ? 0 : (Projectile.timeLeft - 18f) / 24f;
            fac = 1 - (cValue - 1) * (1 - fac) * (1 - fac) - (2 - cValue) * (1 - fac);
            var rotation = Projectile.ai[1] + MathHelper.PiOver4 * 3 - fac.Lerp(-MathHelper.PiOver2, MathHelper.Pi / 3f);
            bool dir = Projectile.Center.X - target.Center.X < 0;
            if (!dir) rotation = MathHelper.Pi - rotation;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, rotation, new Vector2(dir ? 0 : 50, 38), 1f, dir ? 0 : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

    }
}
