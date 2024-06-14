using System.Collections.Generic;
using Terraria.Localization;
using static Terraria.Utils;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using static Terraria.ModLoader.ModContent;
using static CoolerItemVisualEffect.CoolerItemVisualEffectMethods;
using static CoolerItemVisualEffect.CoolerItemVisualEffectMod;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures;

namespace CoolerItemVisualEffect.Weapons
{
    public class WitheredWoodSword : ModItem
    {
        public Item item => Item;
        //public override string Texture => base.Texture + "_Old";
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.crit = 21;
            Item.width = 50;
            Item.height = 54;
            Item.rare = ItemRarityID.LightRed;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 6;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.shoot = ProjectileType<WitheredWoodSword_Blade>();
        }
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.QuickAddIngredient(
            ItemID.WoodenSword,
            ItemID.BorealWoodSword,
            ItemID.PalmWoodSword,
            ItemID.RichMahoganySword,
            ItemID.ShadewoodSword,
            ItemID.PearlwoodSword,
            ItemID.CactusSword);
            recipe.AddIngredient(ItemID.Mushroom, 50);
            recipe.AddIngredient(ItemID.GlowingMushroom, 50);
            recipe.AddIngredient(ItemID.Acorn, 50);
            recipe.AddIngredient(ItemID.BambooBlock, 15);
            recipe.AddTile(TileID.LivingLoom);
            recipe.ReplaceResult(this);
            recipe.Register();
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
    public class LivingWoodSword : WitheredWoodSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 52;
            Item.height = 70;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.damage = 60;
            Item.rare = ItemRarityID.Yellow;
        }
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient<WitheredWoodSword>();
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.ReplaceResult(this);
            recipe.Register();
        }
    }
    public class WitheredWoodSword_Blade : HandMeleeProj
    {
        public virtual T UpgradeValue<T>(T normal, T extra, T defaultValue = default)
        {
            //var type = Player.HeldItem.type;
            var type = sourceItem.type;

            if (type == ItemType<WitheredWoodSword>())
            {
                return normal;
            }

            if (type == ItemType<LivingWoodSword>())
            {
                return extra;
            }

            return defaultValue;
        }
        public override bool Charged => base.Charged;
        public override bool Charging => base.Charging;
        public override SpriteEffects flip => (Player.direction == -1 ^ (controlState == 1 && controlTier % 2 == 1)) ? SpriteEffects.FlipHorizontally : 0;//(int)(projectile.ai[0] / MaxTime)
        public override Rectangle? frame => projTex.Frame(2, 1, UpgradeValue(0, 1));//
        public override Vector2 projCenter => base.projCenter;
        public override Vector2 scale => base.scale;
        public override float timeCount
        {
            get => base.timeCount;
            set => base.timeCount = value;
        }
        //public override string Texture => base.Texture.Replace("_Blade", "Proj");
        public override string ProjName => "WitheredWoodSword";
        public override float MaxTime => controlState == 2 ? 48 : (controlTier % 5) switch
        {
            4 => 32,
            _ => 16
        };
        public override float factor => base.factor;
        public override Vector2 CollidingSize => base.CollidingSize;
        //public override Vector2 projCenter => base.projCenter + new Vector2(Player.direction * 16, -16);
        public override Vector2 CollidingCenter => base.CollidingCenter;//new Vector2(projTex.Size().X / 3 - 16, 16)
        public override Vector2 DrawOrigin => new Vector2(12, projTex.Size().Y / FrameMax.Y - 12);// + new Vector2(-12, 12)
        public override Color color => base.color;
        //public override Color VertexColor(float time) => Color.Lerp(Color.DarkGreen, UpgradeValue(Color.Brown, Color.Green), time);//Color.Lerp(UpgradeValue(Color.Brown, Color.Green), Color.DarkGreen, time)
        public override float MaxTimeLeft => 8;
        public override float RealRotation
        {
            get
            {
                //return MathHelper.PiOver4;
                if (controlState == 1)
                {
                    int tier = controlTier;//(int)(projectile.ai[0] / MaxTime)
                    float k = 3f;
                    float _factor = factor;
                    if (controlTier % 5 == 4)
                    {
                        float key = MaxTime - MaxTimeLeft;
                        _factor = timeCount < key ? GetLerpValue(0, key, timeCount) * .5f : GetLerpValue(key, MaxTime, timeCount) * .5f + .5f;
                        _factor = (k - 1) * _factor * _factor + (2 - k) * _factor;
                        if (tier % 2 == 1) _factor = 1 - _factor;
                        return MathHelper.Lerp(MathHelper.Pi * 6 / 8, -MathHelper.Pi * 6 / 8, _factor);
                    }
                    else
                    {
                        _factor = MathF.Pow(_factor, 2);
                    }


                    if (tier % 2 == 1) _factor = 1 - _factor;
                    return MathHelper.Hermite(MathHelper.Pi * 6 / 8, 0, -MathHelper.Pi * 6 / 8, 0, _factor);//MathHelper.SmoothStep(MathHelper.Pi * 6 / 8, -MathHelper.Pi * 6 / 8, _factor)
                }
                return base.RealRotation;
            }
        }
        public override float Rotation
        {
            get
            {
                ////Main.NewText(timeCount);
                //if (Player.controlUseTile) 
                //{
                //    var factor = MathHelper.Clamp(projectile.ai[0] / MaxTime, 0, 1);
                //    var theta = ((float)Math.Pow(factor, 2)).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2 - MathHelper.Pi / 8);
                //    if (projectile.ai[1] > 0)
                //    {
                //        if (Charged)
                //        {
                //            //Main.NewText(projectile.ai[1] / MaxTimeLeft / factor);
                //            theta = (projectile.ai[1] / MaxTimeLeft / factor).Lerp(theta, MathHelper.Pi / 8 * 3);
                //            //return player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;
                //        }
                //        else
                //        {
                //            theta = ((timeCount - projectile.ai[1]) / MaxTime).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2);
                //            //return player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;
                //        }
                //    }
                //    return theta;
                //}
                //return base.Rotation;
                return base.Rotation;

            }
        }
        public override bool UseRight => true;
        public override bool UseLeft => true;
        public override (int X, int Y) FrameMax => (2, 1);
        public override void Kill(int timeLeft)
        {
            if (Charged && Player.CheckMana(50, true))
            {
                var cen = projCenter - (CollidingCenter - DrawOrigin).RotatedBy(RealRotation) * new Vector2(Player.direction, 1) * 1.5f + new Vector2(0, -24);
                Projectile.NewProjectile(projectile.GetSource_FromThis(), cen, default, ProjectileType<WitheredTree>(), Projectile.damage * 4, Projectile.knockBack, Projectile.owner, UpgradeValue(1, 3));
                for (int n = 0; n < 30; n++)
                {
                    Dust.NewDustPerfect(cen, UpgradeValue(MyDustId.Wood, MyDustId.GreenGrass), (MathHelper.TwoPi / 30 * n).ToRotationVector2() * Main.rand.NextFloat(2, 8));
                }
            }
        }
        public override void OnEndAttack()
        {
            if (ConfigurationSwoosh.ConfigSwooshInstance.coolerSwooshQuality == ConfigurationSwoosh.QualityType.极限ultra)
            {
                bool large = (controlState == 2 || controlTier % 5 == 4);
                var modplr = Player.GetModPlayer<CoolerItemVisualEffectPlayer>();
                modplr.NewCoolerSwoosh(Color.DarkRed, Player.HeldItem.type, 1, 1
                    , large ? (byte)30 : (byte)15, ((projTex.Size() / new Vector2(FrameMax.X, FrameMax.Y)).Length() * Player.GetAdjustedItemScale(Player.HeldItem) - (new Vector2(0, projTex.Size().Y / FrameMax.Y) - DrawOrigin).Length()) * (large ? 0.625f : .5f), _negativeDir: controlTier % 2 == 1
                    , _rotation: 0, xscaler: large ? 1.25f : 1, angleRange: (Player.direction == 1 ? -1.125f : 2.125f, Player.direction == 1 ? 3f / 8 : 0.625f));//MathHelper.Pi / 8 * 3, -MathHelper.PiOver2 - MathHelper.Pi / 8
                modplr.UpdateVertex();
            }

            base.OnEndAttack();
        }
        public override void OnCharging(bool left, bool right)
        {
            SACoolDown--;
            if (left)
            {
                projectile.ai[0]++;
                if (projectile.ai[0] >= MaxTime)
                {
                    OnEndAttack();
                }
            }
            else
            {
                projectile.ai[0] += projectile.ai[0] < MaxTime ? 1 : 0;
            }

            var when = (controlTier % 5 == 4 ? (int)(MaxTime - MaxTimeLeft) : (int)MaxTime / 4);
            if ((int)projectile.ai[0] == when && left)
            {
                SoundEngine.PlaySound(SoundID.Item71);
            }
            projectile.friendly = projectile.ai[0] > when;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[projectile.owner] = 5;
            if (SACoolDown < 0 && Main.rand.NextBool(controlTier % 5 == 4 ? 2 : 5))
            {
                var cen = target.Center + new Vector2(0, 24);
                Projectile.NewProjectile(projectile.GetSource_FromThis(), cen, default, ProjectileType<WitheredTree>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, UpgradeValue(0, 2));
                SACoolDown = 15;
                for (int n = 0; n < 30; n++)
                {
                    Dust.NewDustPerfect(cen, UpgradeValue(MyDustId.Wood, MyDustId.GreenGrass), (MathHelper.TwoPi / 30 * n).ToRotationVector2() * Main.rand.NextFloat(2, 8));
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnRelease(bool charged, bool left)
        {
            SACoolDown--;
            base.OnRelease(charged, left);
        }
        public Item sourceItem;
        public int SACoolDown;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemSource)
            {
                sourceItem = itemSource.Item;
            }
            var modplr = Player.GetModPlayer<CoolerItemVisualEffectPlayer>();
            if (modplr.colorInfo.tex == null)
            {
                Main.RunOnMainThread(() => modplr.colorInfo.tex = new Texture2D(Main.graphics.GraphicsDevice, 300, 1));
            }
            CoolerItemVisualEffectPlayer.ChangeItemTex(Player);
            UpdateHeatMap(ref modplr.colorInfo.tex, modplr.hsl, modplr.ConfigurationSwoosh, TextureAssets.Item[Player.HeldItem.type].Value);
            base.OnSpawn(source);
        }
    }
    public class WitheredTree : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override string Texture => "Terraria/Images/Item_1";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void Kill(int timeLeft)
        {
            //foreach (var point in tree.keyPoints)
            //{
            //    if (tree.rand() > .85f)
            //    {
            //        var rand = (int)Projectile.ai[0] / 2 == 1 ? Main.rand.Next(4) : 0;
            //        int index = -1;
            //        if (rand != 0 && Main.rand.NextBool(3))
            //        {
            //            foreach (var npc in Main.npc)
            //            {
            //                if (npc.active && npc.CanBeChasedBy() && !npc.friendly && (npc.Center - point).Length() <= 768)
            //                {
            //                    index = npc.whoAmI;
            //                    break;
            //                }
            //            }
            //        }
            //        var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), point, new Vector2(tree.rand() * 2 - 1, 0) * 4, ModContent.ProjectileType<WitheredWood>(), Projectile.damage / 4, Projectile.knockBack * .5f, Projectile.owner, rand, index);
            //        proj.rotation = tree.rand() * MathHelper.TwoPi;
            //    }
            //}
            tree.SpawnProjectile(Projectile, Projectile.Center, new Vector2(0, -1), tree.root, -.15f);
        }
        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.timeLeft == 1)
            {
                tree.SpawnDust(Projectile.Center, new Vector2(0, -1));
            }
        }
        private LightTree tree;
        public override void OnSpawn(IEntitySource source)
        {
            tree = new LightTree(Main.rand);
            tree.Generate(Projectile.Center, new Vector2(0, -.5f), new Vector2(0, -2048) + Projectile.Center, ((int)Projectile.ai[0] % 2 == 0 ? 64 : 128) * (tree.rand() * .25f + .75f), Projectile.ai[0] > 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //tree?.Draw(Main.spriteBatch, Projectile.Center - Main.screenPosition, new Vector2(0, -1), Projectile.ai[1] / 2f);//Projectile.ai[1] * Projectile.ai[1] / 3600f * 30
            //tree?.Draw(Main.spriteBatch, Projectile.ai[0] > 1 ? TextureAssets.TreeBranch[9].Value : null, Projectile.Center - Main.screenPosition, new Vector2(0, -1), Lighting.GetColor((Projectile.Center / 16f).ToPoint()), 16f, Projectile.ai[1] / 2f);
            //if (Projectile.ai[0] > 1)
            //    tree?.Draw(Main.spriteBatch, Projectile.Center - Main.screenPosition, new Vector2(0, -1), Lighting.GetColor((Projectile.Center / 16f).ToPoint()), 16f, Projectile.ai[1] / 2f);
            //else
            //    tree?.Draw(Main.spriteBatch, null, Projectile.Center - Main.screenPosition, new Vector2(0, -1), Lighting.GetColor((Projectile.Center / 16f).ToPoint()), 16f, Projectile.ai[1] / 2f);
            tree?.Draw(Main.spriteBatch, Projectile.Center - Main.screenPosition, new Vector2(0, -1), Lighting.GetColor((Projectile.Center / 16f).ToPoint()), 16f, Projectile.ai[1] / 2f);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //if (MathF.Abs(MathF.Sin((float)Main.time / 24f)) >= .5f)
            //    return tree?.Check(targetHitbox, Projectile.Center, new Vector2(0, -1), Projectile.ai[1] / 2f) ?? false;
            tree?.Check(targetHitbox);
            return false;
        }
    }
    public class WitheredWood : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_1";

        public override void SetDefaults()
        {
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }
        public LightTree tree;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (tree?.Check(targetHitbox, Projectile.Center, Projectile.rotation.ToRotationVector2(), 10) == true)
            {
                if (Projectile.penetrate == 1)
                {
                    tree.SpawnDust(Projectile.Center, Projectile.rotation.ToRotationVector2());
                    if (tree.rand() < .5f)
                    {
                        tree.SpawnProjectile(Projectile, Projectile.Center, Projectile.rotation.ToRotationVector2(), tree.root, .05f);
                    }
                }

                return true;
            }
            return false;
        }
        public override void AI()
        {
            Projectile.velocity += new Vector2(0, .5f);
            if (Projectile.ai[0] > 0 && Projectile.ai[1] != -1 && Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.npc[(int)Projectile.ai[1]].Center - Projectile.Center).SafeNormalize(default) * 32, .025f);
            }
            Projectile.rotation += .05f;
            for (int n = 9; n > 0; n--)
            {
                Projectile.oldPos[n] = Projectile.oldPos[n - 1];
                Projectile.oldRot[n] = Projectile.oldRot[n - 1];
            }
            Projectile.oldPos[0] = Projectile.Center;
            Projectile.oldRot[0] = Projectile.rotation;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //try
            //{
            //    if (!TextureAssets.Tile[5].IsLoaded) Main.instance.LoadTiles(5);
            //    if (!TextureAssets.Gore[385].IsLoaded) Main.instance.LoadGore(385);
            //    if (!TextureAssets.Gore[384].IsLoaded) Main.instance.LoadGore(384);
            //}
            //catch { }

            //Texture2D tex = (int)Projectile.ai[0] switch
            //{
            //    0 or 1 => TextureAssets.Tile[5].Value,
            //    2 => TextureAssets.TreeBranch[9].Value,
            //    3 => TextureAssets.Gore[385].Value,
            //    _ or 4 => TextureAssets.Gore[384].Value
            //};
            //Rectangle frame = (int)Projectile.ai[0] switch
            //{
            //    0 or 1 => new Rectangle(66, 0, 22, 22),
            //    2 => new Rectangle(42, 0, 42, 42),
            //    3 => new Rectangle(0, 0, 36, 34),
            //    _ or 4 => new Rectangle(0, 0, 40, 28)
            //};
            //Color color = (int)Projectile.ai[0] switch
            //{
            //    0 or 1 => Color.Brown,
            //    2 or 3 => Color.Green,
            //    _ or 4 => Color.Pink
            //};
            //for (int n = 9; n > -1; n--)
            //{
            //    var c = Lighting.GetColor((Projectile.Center / 16).ToPoint(), n == 0 ? Color.White : color);
            //    if (n != 0) c = c with { A = 0 } * (1 - n * .1f) * .5f;
            //    Main.EntitySpriteDraw(tex, Projectile.oldPos[n] - Main.screenPosition, frame, c, Projectile.oldRot[n], frame.Size() * .5f, (1 - n * .1f) * 1.5f, 0, 0);// * 1.5f//with { A = 0 } * (1 - n * .1f) * .5f)

            //}
            for (int n = 9; n > -1; n--)
            {
                var c = Lighting.GetColor((Projectile.Center / 16).ToPoint(), Color.White);// n == 0 ? Color.White : color
                if (n != 0)
                {
                    var fac = (1 - n * .1f) * .5f;
                    c = c * fac * fac;
                    c.A = (byte)(c.A * (9 - n) / 9f);
                }
                tree?.Draw(Main.spriteBatch, Projectile.oldPos[n] - Main.screenPosition, Projectile.oldRot[n].ToRotationVector2(), c, 16f, 10);
            }
            return false;
        }
    }
    public class WitheredWoodSword_Blade_GivenUp : VertexHammerProj
    {
        public virtual T UpgradeValue<T>(T normal, T extra, T defaultValue = default)
        {
            //var type = Player.HeldItem.type;
            var type = sourceItem.type;

            if (type == ItemType<WitheredWoodSword>())
            {
                return normal;
            }

            if (type == ItemType<LivingWoodSword>())
            {
                return extra;
            }

            return defaultValue;
        }
        public override bool Charged => base.Charged;
        public override bool Charging => base.Charging;
        public override SpriteEffects flip => (int)((timeCount - 1) / MaxTime) % 2 == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        public override Rectangle? frame => projTex.Frame(2, 1, UpgradeValue(0, 1));//
        public override Vector2 projCenter => base.projCenter;
        public override bool RedrawSelf => base.RedrawSelf;
        public override Vector2 scale => base.scale;
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
        public override bool WhenVertexDraw => base.WhenVertexDraw;
        public override string Texture => base.Texture.Replace("_Blade_GivenUp", "Proj");
        public override string HammerName => "WitheredWoodSword";
        public override float MaxTime => 16;
        public override float Factor => ((projectile.ai[0] % MaxTime == 0 && projectile.ai[0] > 0 ? MaxTime - 1 : projectile.ai[0] % MaxTime)) / MaxTime;
        public override Vector2 CollidingSize => base.CollidingSize;
        //public override Vector2 projCenter => base.projCenter + new Vector2(Player.direction * 16, -16);
        public override Vector2 CollidingCenter => base.CollidingCenter;//new Vector2(projTex.Size().X / 3 - 16, 16)
        public override Vector2 DrawOrigin => base.DrawOrigin + new Vector2(-12, 12);
        public override Color color => base.color;
        //public override Color VertexColor(float time) => Color.Lerp(Color.DarkGreen, UpgradeValue(Color.Brown, Color.Green), time);//Color.Lerp(UpgradeValue(Color.Brown, Color.Green), Color.DarkGreen, time)
        public override float MaxTimeLeft => 8;
        public override float Rotation
        {
            get
            {
                var theta = ((float)Math.Pow(Factor, 2)).Lerp(MathHelper.Pi / 8 * 3 * (projectile.ai[0] > MaxTime ? -1 : 1), -MathHelper.PiOver2 - MathHelper.Pi / 8);
                if (projectile.ai[1] > 0)
                {
                    if (Charged)
                    {
                        theta = (projectile.ai[1] / MaxTimeLeft / Factor).Lerp(theta, MathHelper.Pi / 8 * 3);
                    }
                    else
                    {
                        theta = ((timeCount - projectile.ai[1]) / MaxTime).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2);
                    }
                }
                theta = -MathHelper.PiOver2;
                //if(timeCount / MaxTime % 2 == 1)theta = -theta;
                return Player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;

            }
        }
        public override bool UseRight => true;
        public override bool UseLeft => true;
        public override (int X, int Y) FrameMax => (2, 1);
        public override void OnKill(int timeLeft)
        {
            //if (factor == 1)
            //{
            //    Projectile.NewProjectile(projectile.GetSource_FromThis(), vec, default, ModContent.ProjectileType<HolyExp>(), player.GetWeaponDamage(player.HeldItem) * 3, projectile.knockBack, projectile.owner);
            //}
            if (Charged || (controlState == 1 && (timeCount - 3) % MaxTime >= MaxTime * .75f))
            {
                bool large = (controlState == 2 || (int)(timeCount / MaxTime) % 5 == 4);
                var modplr = Player.GetModPlayer<CoolerItemVisualEffectPlayer>();
                modplr.NewCoolerSwoosh(Color.DarkRed, Player.HeldItem.type, 1, large ? 2 : 1
                    , large ? (byte)30 : (byte)15, ((projTex.Size() / new Vector2(FrameMax.X, FrameMax.Y)).Length() * Player.GetAdjustedItemScale(Player.HeldItem) - (new Vector2(0, projTex.Size().Y / FrameMax.Y) - DrawOrigin).Length()) * (large ? 0.625f : .5f), _negativeDir: false
                    , _rotation: large ? MathHelper.Pi : 0, xscaler: large ? 1.25f : 1, heat: HeatMap, angleRange: (Player.direction == 1 ? -1.125f : 2.125f, Player.direction == 1 ? 3f / 8 : 0.625f));//MathHelper.Pi / 8 * 3, -MathHelper.PiOver2 - MathHelper.Pi / 8
                modplr.UpdateVertex();
            }
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
            if (projectile.ai[1] > (Charged ? (MaxTimeLeft * Factor) : timeCount % MaxTime))
            {
                if (left && Player.controlUseItem)
                {
                    projectile.ai[1] = 0;
                    projectile.ai[0]++;
                    OnKill(projectile.timeLeft);
                    

                }
                else
                {
                    projectile.Kill();
                }
            }
        }
        public Item sourceItem;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemSource)
            {
                sourceItem = itemSource.Item;
            }
            base.OnSpawn(source);
        }
        public override void RenderInfomation(ref BloomEffectInfo useBloom, ref AirDistortEffectInfo useDistort, ref MaskEffectInfo useMask)
        {
            var config = Player.GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh;
            useBloom = new BloomEffectInfo(0, config.luminosityFactor, 6, 3, true);
            useDistort = new AirDistortEffectInfo(config.distortSize * 3f);
        }
        public override void VertexInfomation(ref bool additive, ref int indexOfGreyTex, ref float endAngle, ref bool useHeatMap, ref int p)
        {
            var modplr = Player.GetModPlayer<CoolerItemVisualEffectPlayer>();
            additive = modplr.hsl.Z >= modplr.ConfigurationSwoosh.isLighterDecider;
            p = 2;
        }
    }
    public class SereStoneSword : WitheredWoodSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.damage = 40;
            item.crit = 26;
            item.DamageType = DamageClass.Melee;
            item.width = 48;
            item.height = 48;
            item.rare = ItemRarityID.Pink;
            item.useTime = 25;
            item.useAnimation = 25;
            item.knockBack = 8;
            item.useStyle = ItemUseStyleID.Swing;
            item.autoReuse = true;
            Item.shoot = ProjectileType<SereStoneSword_Blade>();

        }
        public override void AddRecipes()
        {

            var recipe = CreateRecipe();
            for (int n = 0; n < 6; n++)
                recipe.AddIngredient(3764 + n);//六种晶光刃
            recipe.AddIngredient(ItemID.OrangePhasesaber);
            recipe.AddIngredient(ItemID.BoneSword);
            recipe.AddIngredient(ItemID.AntlionClaw);
            recipe.AddIngredient(ItemID.BeamSword);
            recipe.AddIngredient(ItemID.PurpleClubberfish);
            recipe.AddIngredient(ItemID.Bladetongue);
            recipe.AddIngredient(ItemID.StoneBlock, 500);
            recipe.AddIngredient(ItemID.EbonstoneBlock, 500);
            recipe.AddIngredient(ItemID.CrimstoneBlock, 500);
            recipe.AddIngredient(ItemID.PearlstoneBlock, 500);
            recipe.AddIngredient(ItemID.Sandstone, 500);
            recipe.AddIngredient(ItemID.CorruptSandstone, 500);
            recipe.AddIngredient(ItemID.CrimsonSandstone, 500);
            recipe.AddIngredient(ItemID.HallowSandstone, 500);
            recipe.AddIngredient(ItemID.Granite, 500);
            recipe.AddIngredient(ItemID.Obsidian, 50);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.ReplaceResult(this);
            recipe.Register();
        }
    }
    public class CrystalStoneSword : SereStoneSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.damage = 70;
            item.width = 50;
            item.rare = ItemRarityID.Yellow;
            item.useTime = 18;
            item.useAnimation = 18;
        }
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient<SereStoneSword>();
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.ReplaceResult(this);
            recipe.Register();
        }
    }
    public class SereStoneSword_Blade : WitheredWoodSword_Blade
    {
        public override bool Charged => base.Charged;
        public override bool Charging => base.Charging;
        public override SpriteEffects flip => (Player.direction == -1 ^ (controlState == 1 && controlTier % 2 == 1)) ? SpriteEffects.FlipHorizontally : 0;//(int)(projectile.ai[0] / MaxTime)
        public override Rectangle? frame => projTex.Frame(2, 1, UpgradeValue(0, 1));//
        public override Vector2 projCenter => base.projCenter;
        public override Vector2 scale => base.scale;
        public override float timeCount
        {
            get => base.timeCount;
            set => base.timeCount = value;
        }
        //public override string Texture => base.Texture.Replace("_Blade", "Proj");
        public override string ProjName => "WitheredWoodSword";
        public override float MaxTime => controlState == 2 ? 48 : (controlTier % 5) switch
        {
            4 => 32,
            _ => 16
        };
        public override float factor => base.factor;
        public override Vector2 CollidingSize => base.CollidingSize;
        //public override Vector2 projCenter => base.projCenter + new Vector2(Player.direction * 16, -16);
        public override Vector2 CollidingCenter => base.CollidingCenter;//new Vector2(projTex.Size().X / 3 - 16, 16)
        public override Vector2 DrawOrigin => new Vector2(12, projTex.Size().Y / FrameMax.Y - 12);// + new Vector2(-12, 12)
        public override Color color => base.color;
        //public override Color VertexColor(float time) => Color.Lerp(Color.DarkGreen, UpgradeValue(Color.Brown, Color.Green), time);//Color.Lerp(UpgradeValue(Color.Brown, Color.Green), Color.DarkGreen, time)
        public override float MaxTimeLeft => 8;
        public override float RealRotation
        {
            get
            {
                //return MathHelper.PiOver4;
                if (controlState == 1)
                {
                    int tier = controlTier;//(int)(projectile.ai[0] / MaxTime)
                    float k = 3f;
                    float _factor = factor;
                    if (controlTier % 5 == 4)
                    {
                        float key = MaxTime - MaxTimeLeft;
                        _factor = timeCount < key ? GetLerpValue(0, key, timeCount) * .5f : GetLerpValue(key, MaxTime, timeCount) * .5f + .5f;
                        _factor = (k - 1) * _factor * _factor + (2 - k) * _factor;
                        if (tier % 2 == 1) _factor = 1 - _factor;
                        return MathHelper.Lerp(MathHelper.Pi * 6 / 8, -MathHelper.Pi * 6 / 8, _factor);
                    }
                    else
                    {
                        _factor = MathF.Pow(_factor, 2);
                    }


                    if (tier % 2 == 1) _factor = 1 - _factor;
                    return MathHelper.Hermite(MathHelper.Pi * 6 / 8, 0, -MathHelper.Pi * 6 / 8, 0, _factor);//MathHelper.SmoothStep(MathHelper.Pi * 6 / 8, -MathHelper.Pi * 6 / 8, _factor)
                }
                return base.RealRotation;
            }
        }
        public override float Rotation
        {
            get
            {
                ////Main.NewText(timeCount);
                //if (Player.controlUseTile) 
                //{
                //    var factor = MathHelper.Clamp(projectile.ai[0] / MaxTime, 0, 1);
                //    var theta = ((float)Math.Pow(factor, 2)).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2 - MathHelper.Pi / 8);
                //    if (projectile.ai[1] > 0)
                //    {
                //        if (Charged)
                //        {
                //            //Main.NewText(projectile.ai[1] / MaxTimeLeft / factor);
                //            theta = (projectile.ai[1] / MaxTimeLeft / factor).Lerp(theta, MathHelper.Pi / 8 * 3);
                //            //return player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;
                //        }
                //        else
                //        {
                //            theta = ((timeCount - projectile.ai[1]) / MaxTime).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2);
                //            //return player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;
                //        }
                //    }
                //    return theta;
                //}
                //return base.Rotation;
                return base.Rotation;

            }
        }
        public override bool UseRight => true;
        public override bool UseLeft => true;
        public override (int X, int Y) FrameMax => (2, 1);
        public override void Kill(int timeLeft)
        {
            //TODO 枯石蓄力攻击
            //if (Charged && Player.CheckMana(50, true))
            //{
            //    var cen = projCenter - (CollidingCenter - DrawOrigin).RotatedBy(RealRotation) * new Vector2(Player.direction, 1) * 1.5f + new Vector2(0, -24);
            //    Projectile.NewProjectile(projectile.GetSource_FromThis(), cen, default, ModContent.ProjectileType<WitheredTree>(), Projectile.damage * 3 / 2, Projectile.knockBack, Projectile.owner, UpgradeValue(1, 3));
            //    for (int n = 0; n < 30; n++)
            //    {
            //        Dust.NewDustPerfect(cen, UpgradeValue(MyDustId.Wood, MyDustId.GreenGrass), (MathHelper.TwoPi / 30 * n).ToRotationVector2() * Main.rand.NextFloat(2, 8));
            //    }
            //}

        }
        public override void OnEndAttack()
        {
            base.OnEndAttack();
        }
        public override void OnCharging(bool left, bool right)
        {
            SACoolDown--;
            if (left)
            {
                projectile.ai[0]++;
                if (projectile.ai[0] >= MaxTime)
                {
                    OnEndAttack();
                }
            }
            else
            {
                projectile.ai[0] += projectile.ai[0] < MaxTime ? 1 : 0;
            }

            var when = (controlTier % 5 == 4 ? (int)(MaxTime - MaxTimeLeft) : (int)MaxTime / 4);
            if ((int)projectile.ai[0] == when && left)
            {
                SoundEngine.PlaySound(SoundID.Item71);
            }
            projectile.friendly = projectile.ai[0] > when;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[projectile.owner] = 5;
            //TODO 枯石特殊攻击
            if (SACoolDown < 0 && Main.rand.NextBool(controlTier % 5 == 4 ? 2 : 5))
            {
                var max = UpgradeValue(8, 12);
                for (int n = 0; n < max; n++)
                {
                    if (Main.rand.NextBool(UpgradeValue(3, 2)))
                    {
                        projectile.damage *= 2;
                        SereStoneSwordProj.ShootSharpTears(target.Center + new Vector2((n - max / 2f) * 8, 24), Player, projectile);
                        projectile.damage /= 2;
                    }
                }
            }
        }
        public override void OnRelease(bool charged, bool left)
        {
            if (Charged && Player.CheckMana(5, true))
            {
                int max = UpgradeValue(1, 3);
                for (int n = 0; n < max; n++)
                {
                    Vector2 pointPoisition2 = Player.Center + new Vector2(128 * Player.direction, 0) * ((projectile.ai[1] + (float)n / max) / MaxTimeLeft) * max;
                    SereStoneSwordProj.ShootSharpTears(pointPoisition2, Player, projectile);
                }
            }
            base.OnRelease(charged, left);
        }
        public override T UpgradeValue<T>(T normal, T extra, T defaultValue = default)
        {
            var type = sourceItem.type;

            if (type == ItemType<SereStoneSword>())
            {
                return normal;
            }

            if (type == ItemType<CrystalStoneSword>())
            {
                return extra;
            }

            return defaultValue;
        }
    }
    public class RustySteelBlade : WitheredWoodSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.damage = 70;
            item.crit = 26;
            item.DamageType = DamageClass.Melee;
            item.width = 66;
            item.height = 74;
            item.rare = ItemRarityID.LightPurple;
            item.useTime = 21;
            item.useAnimation = 21;
            item.knockBack = 8;
            item.useStyle = ItemUseStyleID.Swing;
            item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.QuickAddIngredient(
            ItemID.CopperBroadsword,
            ItemID.TinBroadsword,
            ItemID.IronBroadsword,
            ItemID.LeadBroadsword,
            ItemID.SilverBroadsword,
            ItemID.TungstenBroadsword,
            ItemID.GoldBroadsword,
            ItemID.PlatinumBroadsword,
            ItemID.Gladius,
            ItemID.Katana,
            ItemID.DyeTradersScimitar,
            ItemID.FalconBlade,
            ItemID.CobaltSword,
            ItemID.PalladiumSword,
            ItemID.MythrilSword,
            ItemID.OrichalcumSword,
            ItemID.BreakerBlade,
            ItemID.Cutlass,
            ItemID.AdamantiteSword,
            ItemID.TitaniumSword,
            ItemID.ChlorophyteSaber,
            ItemID.ChlorophyteClaymore);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.ReplaceResult(this);
            recipe.Register();
        }
    }
    public class RefinedSteelBlade : RustySteelBlade
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.damage = 90;
            item.rare = ItemRarityID.Yellow;
            item.useTime = 15;
            item.useAnimation = 15;
        }
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient<RustySteelBlade>();
            recipe.AddIngredient(ItemID.BrokenHeroSword, 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.ReplaceResult(this);
            recipe.Register();
        }
    }
    public class PureFractal : WitheredWoodSword
    {
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            item.ShaderItemEffectInventory(spriteBatch, position, origin, LogSpiralLibraryMod.Misc[1].Value, Color.Lerp(new Color(0, 162, 232), new Color(34, 177, 76), (float)Math.Sin(MathHelper.Pi / 60 * ModTime) / 2 + 0.5f), scale);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.ShaderItemEffectInWorld(spriteBatch, LogSpiralLibraryMod.Misc[1].Value, Color.Lerp(new Color(0, 162, 232), new Color(34, 177, 76), (float)Math.Sin(MathHelper.Pi / 60 * ModTime) / 2 + 0.5f), rotation);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.useStyle = ItemUseStyleID.Swing;
            item.width = 56;
            item.height = 56;
            //item.UseSound = SoundID.Item169;
            item.autoReuse = true;
            item.DamageType = DamageClass.Melee;
            item.shoot = ProjectileType<PureFractalProj>();
            item.useAnimation = 30;
            item.useTime = item.useAnimation / 3;
            item.shootSpeed = 16f;
            item.damage = 240;
            item.knockBack = 6.5f;
            item.value = Item.sellPrice(0, 20, 0, 0);
            item.crit = 10;
            item.rare = ItemRarityID.Purple;
            item.noUseGraphic = true;
            item.noMelee = true;
        }
        //public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Zenith).AddIngredient<FirstFractal_CIVE>().AddTile(TileID.LunarCraftingStation).Register();
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.QuickAddIngredient(ItemID.Zenith);
            recipe.AddIngredient<FirstFractal_CIVE>();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
    public class FirstZenith : WitheredWoodSword
    {
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Item.ShaderItemEffectInventory(spriteBatch, position, origin, LogSpiralLibraryMod.Misc[0].Value, Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * ModTime) / 2 + 0.5f), scale);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.ShaderItemEffectInWorld(spriteBatch, LogSpiralLibraryMod.Misc[0].Value, Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * ModTime) / 2 + 0.5f), rotation);
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 58;
            Item.height = 64;
            Item.noUseGraphic = true;
            //Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.useAnimation = 15;
            Item.useTime = 15;// 
            Item.shootSpeed = 16f;
            Item.damage = 500;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.crit = 31;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ProjectileType<FirstZenith_Blade>();
        }
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Zenith);
            recipe.AddIngredient<FirstFractal_CIVE>();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddIngredient<PureFractal_Old>();
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddIngredient(this);
            recipe.ReplaceResult<PureFractal_Old>();
            recipe.Register();
        }
    }
    public class FirstZenith_Blade : VertexHammerProj
    {
        public override bool Charged => base.Charged;
        public override bool Charging => base.Charging;
        public override SpriteEffects flip => base.flip;
        public override Rectangle? frame => base.frame;
        public override Vector2 projCenter => base.projCenter;
        public override bool RedrawSelf => base.RedrawSelf;
        public override Vector2 scale => base.scale;
        public override float timeCount { get => base.timeCount; set => base.timeCount = value; }
        public override bool WhenVertexDraw => base.WhenVertexDraw;
        public override string Texture => base.Texture.Replace("_Blade", "_Old");
        public override string HammerName => "初源峰巅";
        public override float MaxTime => 12;
        public override float Factor => base.Factor;
        public override Vector2 CollidingSize => base.CollidingSize;
        //public override Vector2 projCenter => base.projCenter + new Vector2(Player.direction * 16, -16);
        public override Vector2 CollidingCenter => base.CollidingCenter;//new Vector2(projTex.Size().X / 3 - 16, 16)
        public override Vector2 DrawOrigin => base.DrawOrigin + new Vector2(-12, 12);
        public override Color color => base.color;
        //public override Color VertexColor(float time) => Color.Lerp(Color.DarkGreen, UpgradeValue(Color.Brown, Color.Green), time);//Color.Lerp(UpgradeValue(Color.Brown, Color.Green), Color.DarkGreen, time)
        public override float MaxTimeLeft => 8;
        public override float Rotation => base.Rotation;
        public override bool UseRight => true;
        public override bool UseLeft => true;
        public override (int X, int Y) FrameMax => (1, 1);
        public override void OnKill(int timeLeft)
        {
            //if (factor == 1)
            //{
            //    Projectile.NewProjectile(projectile.GetSource_FromThis(), vec, default, ModContent.ProjectileType<HolyExp>(), player.GetWeaponDamage(player.HeldItem) * 3, projectile.knockBack, projectile.owner);
            //}
            //CoolerSystem.UseInvertGlass = !CoolerSystem.UseInvertGlass;

            base.OnKill(timeLeft);
        }
        public override void OnCharging(bool left, bool right)
        {

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
            //Main.NewText(new NPCs.Baron.Baron().CanTownNPCSpawn(10, 10));
            base.OnRelease(charged, left);
        }
        public Item sourceItem;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemSource)
            {
                sourceItem = itemSource.Item;
            }
            base.OnSpawn(source);
        }
        public override void RenderInfomation(ref BloomEffectInfo useBloom, ref AirDistortEffectInfo useDistort, ref MaskEffectInfo useMask)
        {
            var config = Player.GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh;
            useBloom = new BloomEffectInfo(0, config.luminosityFactor, 6, 3, true);
            useDistort = new AirDistortEffectInfo(config.distortSize * 3f);
        }
        public override void VertexInfomation(ref bool additive, ref int indexOfGreyTex, ref float endAngle, ref bool useHeatMap, ref int p)
        {
            var modplr = Player.GetModPlayer<CoolerItemVisualEffectPlayer>();
            additive = modplr.hsl.Z >= modplr.ConfigurationSwoosh.isLighterDecider;
            p = 2;
        }
        public override void OnChargedShoot()
        {
            //CoolerSystem.UseInvertGlass = !CoolerSystem.UseInvertGlass;
            Projectile.NewProjectile(projectile.GetSource_FromThis(), Player.Center, default, ProjectileType<VectorField_Ultra>(), 0, 0, Player.whoAmI);//(Main.MouseWorld - Player.Center).SafeNormalize(default) * 6
        }
    }


    public class VectorField : ModProjectile 
    {
        public override void SetDefaults()
        {
            IHat = new Vector2(1, 1) * 4;
            JHat = new Vector2(-1, 1) * 4;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
        }
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server) return; 
            vectorTex = LogSpiralLibraryMod.Misc[4].Value;
        }
        public Texture2D vectorTex;
        public override string Texture => GetInstance<FirstZenith>().Texture;
        public Vector2 IHat
        {
            //((float)Main.time / 30f).ToRotationVector2() * 2f
            //(MathF.Sin((float)Main.time / 60f) * MathHelper.PiOver2 + MathHelper.Pi).ToRotationVector2() * 2f
            get => new Vector2(-1, 1);
            set => iHat = value;
        }
        public Vector2 iHat;
        public Vector2 JHat
        {
            get => new Vector2(-1, 0);// new Vector2(-IHat.Y, IHat.X)
            set => jHat = value;
        }
        public Vector2 jHat;

        public Vector2[,] flows = new Vector2[120, 30];
        public bool[] flowActive = new bool[120];
        public float[] flowFade = new float[120];
        public Vector2 Transform(Vector2 vec) => new Vector2(vec.X * IHat.X + vec.Y * JHat.X, vec.X * IHat.Y + vec.Y * JHat.Y);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void AI()
        {
            for (int n = 0; n < 120; n++)
            {
                if (!flowActive[n])
                {
                    if (Main.rand.NextBool(120))
                    {
                        flowActive[n] = true;
                        flows[n, 0] = Main.rand.NextVector2Unit() * Main.rand.NextFloat(512, 1024) + Projectile.Center;
                        for (int i = 1; i < 30; i++)
                        {
                            flows[n, i] = flows[n, 0];
                        }
                    }
                }
                else
                {
                    for (int i = 29; i > 0; i--)
                    {
                        flows[n, i] = flows[n, i - 1];
                    }
                    if ((flows[n, 0] - Projectile.Center).Length() < 8f || (flows[n, 0] - Projectile.Center).Length() > 1440)
                    {
                        flowFade[n] = flowFade[n] * .9f;
                        if ((flows[n, 29] - Projectile.Center).Length() < 8f || (flows[n, 29] - Projectile.Center).Length() > 1440)
                        {
                            flowActive[n] = false;
                            flowFade[n] = 0;
                        }
                    }
                    else
                    {
                        flowFade[n] = flowFade[n] * .95f + .05f;
                        var vec = Transform(flows[n, 0] - Projectile.Center);
                        if ((Projectile.velocity - vec / 60f).Length() < 2f) //Math.Abs(Projectile.velocity.Length() - vec.Length() / 60f)
                        {
                            flowActive[n] = false;
                            flowFade[n] = 0;
                        }
                        flows[n, 0] += vec / 60f;
                    }
                }
            }
            IHat += Transform(IHat) / 60f;
            JHat += Transform(JHat) / 60f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (vectorTex == null)
            {
                vectorTex = LogSpiralLibraryMod.Misc[4].Value;
                return false;
            }

            float stepLength = 64f;
            var fieldCen = Projectile.Center - Main.screenPosition - new Vector2(stepLength * 60);
            fieldCen = new Vector2((int)(fieldCen.X / stepLength) * stepLength, (int)(fieldCen.Y / stepLength) * stepLength);
            var sb = Main.spriteBatch;
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);
            for (int n = 0; n < 120; n++)
            {
                for (int i = 0; i < 120; i++)
                {
                    var drawCen = fieldCen + new Vector2(stepLength * n, stepLength * i);
                    var target = Transform(drawCen - Projectile.Center + Main.screenPosition);
                    if (vectorTex == null) vectorTex = LogSpiralLibraryMod.Misc[4].Value;
                    sb.Draw(vectorTex, drawCen, null, Color.Lerp(Color.Cyan, Color.Red, target.Length() / 1024f), target.ToRotation(), new Vector2(0, 11), .5f, 0, 0);
                }
            }


            if (ShaderSwooshEffect == null) return false;
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            Matrix result = model * trans * projection;

            List<CustomVertexInfo> vertexInfos = new List<CustomVertexInfo>();

            for (int n = 0; n < 120; n++)
            {
                if (!flowActive[n]) continue;
                var current = flows[n, 0];
                int max = 30;
                for (int i = 1; i < 30; i++)
                {
                    if (current == flows[n, i])
                    {
                        max = i;
                        break;
                    }
                    else
                    {
                        current = flows[n, i];
                    }
                }
                if (max < 2) { continue; }//Main.NewText("??");
                CustomVertexInfo[] infos = new CustomVertexInfo[max * 2];
                for (int i = 0; i < max; i++)
                {
                    float factor = i / (max - 1f);
                    var unit = i == 0 ? flows[n, 0] - flows[n, 1] : flows[n, i - 1] - flows[n, i];
                    unit = new Vector2(-unit.Y, unit.X).SafeNormalize(default) * ((1 - (float)Math.Cos(MathHelper.TwoPi * factor)) * 0.5f) * 16;
                    infos[2 * i] = new CustomVertexInfo(flows[n, i] + unit, Color.White with { A = (byte)(flowFade[n] * 255) }, new Vector3(factor, 0, flowFade[n]));
                    infos[2 * i + 1] = new CustomVertexInfo(flows[n, i] - unit, Color.White with { A = (byte)(flowFade[n] * 255) }, new Vector3(factor, 1, flowFade[n]));
                }
                for (int i = 0; i < max * 2 - 2; i += 2)
                {
                    //if (i == 28) continue;
                    vertexInfos.Add(infos[i]);
                    vertexInfos.Add(infos[i + 2]);
                    vertexInfos.Add(infos[i + 1]);
                    vertexInfos.Add(infos[i + 1]);
                    vertexInfos.Add(infos[i + 2]);
                    vertexInfos.Add(infos[i + 3]);
                }
            }
            //for (int n = 0; n < vertexInfos.Count - 1; n++)
            //{
            //    sb.DrawLine(vertexInfos[n].Position, vertexInfos[n + 1].Position, Color.Red, 4, false, -Main.screenPosition);
            //}
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, trans);
            ShaderSwooshEX.Parameters["uTransform"].SetValue(result);
            ShaderSwooshEX.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            ShaderSwooshEX.Parameters["checkAir"].SetValue(false);
            ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
            ShaderSwooshEX.Parameters["gather"].SetValue(false);
            ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity);
            ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
            ShaderSwooshEX.Parameters["distortScaler"].SetValue(1);
            ShaderSwooshEX.Parameters["alphaFactor"].SetValue(1);
            ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(false);
            Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.BaseTex[8].Value;
            Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.AniTex[10].Value;
            Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;
            Main.graphics.GraphicsDevice.Textures[3] = GetTexture("greyBar");
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;
            ShaderSwooshEX.CurrentTechnique.Passes[2].Apply();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertexInfos.ToArray(), 0, vertexInfos.Count / 3);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, trans);
            sb.Draw(vectorTex, Projectile.Center - Main.screenPosition, null, Color.Purple, IHat.ToRotation(), new Vector2(0, 11), new Vector2(IHat.Length() * 4 / 78f, MathF.Sqrt(IHat.Length()) / 11f) * 4, 0, 0);
            sb.Draw(vectorTex, Projectile.Center - Main.screenPosition, null, Color.Pink, JHat.ToRotation(), new Vector2(0, 11), new Vector2(JHat.Length() * 4 / 78f, MathF.Sqrt(JHat.Length()) / 11f) * 4, 0, 0);
            return false;
        }
    }
    public class VectorField_EX : ModProjectile
    {
        public int tier;
        public Matrix[] packedMatrix;
        //public Vector2 Transform(Vector2 target, (Vector2 i, Vector2 j) info) => new Vector2(target.X * info.i.X + target.Y + info.j.X, target.X * info.i.Y + target.Y + info.j.Y);
        public Vector2 Transform(Vector2 target, (Vector2 i, Vector2 j) info)
        {
            //var color = Main.hslToRgb(Main.rand.NextFloat(0, 1), 1, 0.5f);
            //Main.NewText(target, color);
            var _target = new Vector2(target.X * info.i.X + target.Y * info.j.X, target.X * info.i.Y + target.Y * info.j.Y);
            //Main.NewText((target, _target), color);
            return _target;
        }

        public (Vector2 i, Vector2 j) GetCurrentTransform(int n)
        {
            int i = n % 4;
            var m = packedMatrix[n / 4];
            return i switch
            {
                0 => (new Vector2(m.M11, m.M21), new Vector2(m.M12, m.M22)),
                1 => (new Vector2(m.M13, m.M23), new Vector2(m.M14, m.M24)),
                2 => (new Vector2(m.M31, m.M41), new Vector2(m.M32, m.M42)),
                3 => (new Vector2(m.M33, m.M43), new Vector2(m.M34, m.M44)),
                _ => default
            };
        }
        public class Flow
        {
            public Vector2[] vectorInfos;
            public Vector2[] oldPos;
            public int timeLeft;
            public float fade;
            public bool active;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            tier = 1;
            packedMatrix = new Matrix[(tier - 1) / 4 + 1];
            flows = new Flow[flowCount];
            for (int n = 0; n < packedMatrix.Length; n++)
            {

                packedMatrix[n] = n switch
                {
                    0 =>
                    new Matrix(
                        Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1),
                        Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1),
                        Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1),
                        Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)
                        ),
                    _ => default
                };
            }
            for (int n = 0; n < flowCount; n++)
            {
                flows[n] = new Flow();
                flows[n].vectorInfos = new Vector2[tier + 1];
                if (n < realFlow)
                    flows[n].oldPos = new Vector2[30];
                else
                {
                    float stepLength = 64f;
                    int i = n - realFlow;
                    //var fieldCen = Projectile.Center - new Vector2(stepLength * 15) + new Vector2(i % 30 * stepLength, i / 30 * stepLength);
                    //flows[n].vectorInfos[0] = new Vector2((int)(fieldCen.X / stepLength) * stepLength, (int)(fieldCen.Y / stepLength) * stepLength) - Main.screenPosition - new Vector2(960, 560);
                    flows[n].vectorInfos[0] = new Vector2(i % 30 * stepLength, i / 30 * stepLength) - new Vector2(stepLength * 15);
                    //for (int k = 1; k < tier; k++)
                    //{
                    //    flows[n].vectorInfos[k] = Transform(flows[n].vectorInfos[k - 1], (new Vector2(0, 1), new Vector2(-1, 0))) * (1 - (float)k / tier);
                    //}
                }
            }
        }
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server) return;

            vectorTex = LogSpiralLibraryMod.Misc[4].Value;
        }
        public Texture2D vectorTex;
        public int realFlow => 120;
        public int flowCount => realFlow + 900;
        public Flow[] flows;
        public override string Texture => GetInstance<FirstZenith>().Texture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void AI()
        {
            (Vector2 i, Vector2 j)[] transInfo = new (Vector2 i, Vector2 j)[tier];
            for (int n = 0; n < tier; n++)
            {
                transInfo[n] = GetCurrentTransform(n);
            }
            for (int n = 0; n < flowCount; n++)
            {
                var flow = flows[n];
                if (n >= realFlow)
                {
                    Vector2 vector = default;
                    for (int i = 0; i < tier; i++)
                    {
                        vector += Transform(flow.vectorInfos[i], transInfo[i]);
                    }
                    flow.vectorInfos[tier] = vector;
                    for (int i = tier - 1; i > 0; i--)
                    {
                        flow.vectorInfos[i] += vector / 60f;
                        vector = flow.vectorInfos[i];
                    }
                }
                else
                {
                    if (!flow.active)
                    {
                        if (Main.rand.NextBool(120))
                        {
                            flow.active = true;
                            flow.vectorInfos[0] = Main.rand.NextVector2Unit() * Main.rand.NextFloat(128, 1024);
                            for (int i = 0; i < tier; i++)
                            {
                                flow.vectorInfos[i] = Main.rand.NextVector2Unit() * Main.rand.NextFloat(128, 1024) * (1 - (float)i / tier);
                            }
                            //for (int i = 1; i < tier; i++)
                            //{
                            //    flow.vectorInfos[i] = Transform(flow.vectorInfos[i - 1], (new Vector2(0, 1), new Vector2(-1, 0))) * (1 - (float)i / tier);
                            //}
                            var vec = flow.vectorInfos[0];

                            for (int i = 0; i < 30; i++)
                            {
                                flow.oldPos[i] = vec + Projectile.Center;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 29; i > 0; i--)
                        {
                            flow.oldPos[i] = flow.oldPos[i - 1];
                        }
                        var pos = flow.oldPos[0] = flow.vectorInfos[0] + Projectile.Center;
                        if ((pos - Projectile.Center).Length() < 8f || (pos - Projectile.Center).Length() > 1440)
                        {
                            flow.fade *= .9f;
                            if ((flow.oldPos[29] - Projectile.Center).Length() < 8f || (flow.oldPos[29] - Projectile.Center).Length() > 1440)
                            {
                                flow.active = false;
                                for (int i = 0; i < tier; i++)
                                    flow.vectorInfos[i] = default;
                                flow.fade = 0;
                            }
                        }
                        else
                        {
                            flow.fade = flow.fade * .95f + .05f;
                            Vector2 vector = default;
                            for (int i = 0; i < tier; i++)
                            {
                                vector += Transform(flow.vectorInfos[i], transInfo[i]);
                            }
                            //flow.vectorInfos[tier] = vector;
                            for (int i = tier - 1; i >= 0; i--)
                            {
                                flow.vectorInfos[i] += vector / 60f;
                                vector = flow.vectorInfos[i];
                            }
                        }
                    }
                }

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (vectorTex == null)
            {
                vectorTex = LogSpiralLibraryMod.Misc[4].Value;
                return false;
            }
            var sb = Main.spriteBatch;
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, 1920, 1120), Color.Black);
            (Vector2 i, Vector2 j)[] transInfo = new (Vector2 i, Vector2 j)[tier];
            for (int n = 0; n < tier; n++)
            {
                transInfo[n] = GetCurrentTransform(n);
            }
            for (int n = 0; n < 900; n++)
            {
                var flow = flows[n + realFlow];
                var drawCen = flow.vectorInfos[0] + Projectile.Center - Main.screenPosition;// + Main.screenPosition
                var size = 0.7f;
                if (vectorTex == null) vectorTex = LogSpiralLibraryMod.Misc[4].Value;
                for (int m = 0; m < tier; m++)
                {
                    var target = flow.vectorInfos[m + 1];
                    sb.Draw(vectorTex, drawCen, null, Color.Lerp(Color.Cyan, Color.Red, target.Length() / 1024f), target.ToRotation(), new Vector2(0, 11), size, 0, 0);
                    drawCen += size * target.SafeNormalize(default) * 78;
                    size *= .9f;
                }
            }


            if (ShaderSwooshEffect == null) return false;
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            Matrix result = model * trans * projection;

            List<CustomVertexInfo> vertexInfos = new List<CustomVertexInfo>();

            for (int n = 0; n < realFlow; n++)
            {
                var flow = flows[n];
                if (!flow.active) continue;
                var current = flow.oldPos[0];
                int max = 30;
                for (int i = 1; i < 30; i++)
                {
                    if (current == flow.oldPos[i])
                    {
                        max = i;
                        break;
                    }
                    else
                    {
                        current = flow.oldPos[i];
                    }
                }
                if (max < 2) { continue; }//Main.NewText("??");
                CustomVertexInfo[] infos = new CustomVertexInfo[max * 2];
                for (int i = 0; i < max; i++)
                {
                    float factor = i / (max - 1f);
                    var unit = i == 0 ? flow.oldPos[0] - flow.oldPos[1] : flow.oldPos[i - 1] - flow.oldPos[i];
                    unit = new Vector2(-unit.Y, unit.X).SafeNormalize(default) * ((1 - (float)Math.Cos(MathHelper.TwoPi * factor)) * 0.5f) * 16;
                    infos[2 * i] = new CustomVertexInfo(flow.oldPos[i] + unit, Color.White with { A = (byte)(flow.fade * 255) }, new Vector3(factor, 0, flow.fade));
                    infos[2 * i + 1] = new CustomVertexInfo(flow.oldPos[i] - unit, Color.White with { A = (byte)(flow.fade * 255) }, new Vector3(factor, 1, flow.fade));
                }
                for (int i = 0; i < max * 2 - 2; i += 2)
                {
                    //if (i == 28) continue;
                    vertexInfos.Add(infos[i]);
                    vertexInfos.Add(infos[i + 2]);
                    vertexInfos.Add(infos[i + 1]);
                    vertexInfos.Add(infos[i + 1]);
                    vertexInfos.Add(infos[i + 2]);
                    vertexInfos.Add(infos[i + 3]);
                }
            }
            //for (int n = 0; n < vertexInfos.Count - 1; n++) 
            //{
            //    sb.DrawLine(vertexInfos[n].Position, vertexInfos[n + 1].Position, Color.Red, 4, false, -Main.screenPosition);
            //}
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, trans);
            ShaderSwooshEX.Parameters["uTransform"].SetValue(result);
            ShaderSwooshEX.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            ShaderSwooshEX.Parameters["checkAir"].SetValue(false);
            ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
            ShaderSwooshEX.Parameters["gather"].SetValue(false);
            ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity);
            ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
            ShaderSwooshEX.Parameters["distortScaler"].SetValue(1);
            ShaderSwooshEX.Parameters["alphaFactor"].SetValue(1);
            ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(false);
            Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.BaseTex[8].Value;
            Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.AniTex[10].Value;
            Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;
            Main.graphics.GraphicsDevice.Textures[3] = GetTexture("greyBar");
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;
            ShaderSwooshEX.CurrentTechnique.Passes[2].Apply();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertexInfos.ToArray(), 0, vertexInfos.Count / 3);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, trans);
            //sb.Draw(vectorTex, Projectile.Center - Main.screenPosition, null, Color.Purple, IHat.ToRotation(), new Vector2(0, 11), new Vector2(IHat.Length() * 4 / 78f, MathF.Sqrt(IHat.Length()) / 11f) * 4, 0, 0);
            //sb.Draw(vectorTex, Projectile.Center - Main.screenPosition, null, Color.Pink, JHat.ToRotation(), new Vector2(0, 11), new Vector2(JHat.Length() * 4 / 78f, MathF.Sqrt(JHat.Length()) / 11f) * 4, 0, 0);
            return false;
        }
    }
    public class VectorField_Ultra : ModProjectile
    {
        public class Flow
        {
            public bool active;
            public float fade;
            public Vector2[] position = new Vector2[30];
            public Vector2 velocity;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            flows = new Flow[flowCount];
            for (int n = 0; n < flowCount; n++)
            {
                flows[n] = new Flow();
            }
        }
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server) return;

            vectorTex = LogSpiralLibraryMod.Misc[4].Value;
        }
        public Texture2D vectorTex;
        public override string Texture => GetInstance<FirstZenith>().Texture;
        public Flow[] flows;
        public int flowCount => 120;
        public Vector2 ElectricField(Vector2 vec) => new Vector2(vec.Y, MathF.Sin(vec.X) * 512);//new Vector2(vec.X * vec.X - vec.Y * vec.Y, 2 * vec.X * vec.Y) * .2f + new Vector2(-vec.Y,vec.X) * .8f
        public float MagneticField(Vector2 vec) => -1;//(MathF.Cos(vec.X / 60f * MathHelper.TwoPi) + MathF.Cos(vec.Y / 60f * MathHelper.TwoPi)) * 2
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void AI()
        {
            for (int n = 0; n < 120; n++)
            {
                var flow = flows[n];
                if (!flows[n].active)
                {
                    if (Main.rand.NextBool(120))
                    {
                        flow.active = true;
                        flow.position[0] = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 16) + Projectile.Center - new Vector2(256, 0);//Main.rand.NextVector2Unit() * Main.rand.NextFloat(512, 1024)
                        flow.velocity = new Vector2(60, 0);
                        for (int i = 1; i < 30; i++)
                        {
                            flow.position[i] = flow.position[0];
                        }
                    }
                }
                else
                {
                    for (int i = 29; i > 0; i--)
                    {
                        flow.position[i] = flow.position[i - 1];
                    }
                    if ((flow.position[0] - Projectile.Center).Length() < 8f || (flow.position[0] - Projectile.Center).Length() > 1440)
                    {
                        flow.fade *= .9f;
                        if ((flow.position[29] - Projectile.Center).Length() < 8f || (flow.position[29] - Projectile.Center).Length() > 1440)
                        {
                            flow.active = false;
                            flow.fade = 0;
                        }
                    }
                    else
                    {
                        flow.fade = flow.fade * .95f + .05f;
                        var realPosition = flow.position[0] - Projectile.Center;
                        var acceleration_E = ElectricField(realPosition);
                        var acceleration_M = MagneticField(realPosition) * new Vector2(-flow.velocity.Y, flow.velocity.X);
                        flow.velocity += (acceleration_E + acceleration_M) / 60f;
                        flow.position[0] += flow.velocity / 60f;

                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (vectorTex == null)
            {
                vectorTex = LogSpiralLibraryMod.Misc[4].Value;
                return false;
            }

            float stepLength = 64f;
            var fieldCen = Projectile.Center - Main.screenPosition - new Vector2(stepLength * 60);
            fieldCen = new Vector2((int)(fieldCen.X / stepLength) * stepLength, (int)(fieldCen.Y / stepLength) * stepLength);
            var sb = Main.spriteBatch;
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);
            for (int n = 0; n < 120; n++)
            {
                for (int i = 0; i < 120; i++)
                {
                    var drawCen = fieldCen + new Vector2(stepLength * n, stepLength * i);
                    var target = ElectricField(drawCen - Projectile.Center + Main.screenPosition);
                    if (target.Length() != 0)
                    {
                        if (vectorTex == null) vectorTex = LogSpiralLibraryMod.Misc[4].Value;
                        sb.Draw(vectorTex, drawCen, null, Color.Lerp(Color.Cyan, Color.Red, target.Length() / 1024f), target.ToRotation(), new Vector2(0, 11), .5f, 0, 0);
                    }


                    var B = MagneticField(drawCen - Projectile.Center + Main.screenPosition);
                    if (B == 0) continue;
                    sb.Draw(LogSpiralLibraryMod.Misc[5].Value, drawCen, new Rectangle((Math.Sign(B) + 1) / 2 * 32, 0, 32, 32), Color.Lerp(Color.LimeGreen, Color.DarkGreen, Math.Abs(B) / 4f), 0, new Vector2(16), .5f, 0, 0);

                }
            }


            if (ShaderSwooshEffect == null) return false;
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            Matrix result = model * trans * projection;

            List<CustomVertexInfo> vertexInfos = new List<CustomVertexInfo>();

            for (int n = 0; n < 120; n++)
            {
                var flow = flows[n];
                if (!flow.active) continue;
                var current = flow.position[0];
                int max = 30;
                for (int i = 1; i < 30; i++)
                {
                    if (current == flow.position[i])
                    {
                        max = i;
                        break;
                    }
                    else
                    {
                        current = flow.position[i];
                    }
                }
                if (max < 2) { continue; }//Main.NewText("??");
                CustomVertexInfo[] infos = new CustomVertexInfo[max * 2];
                for (int i = 0; i < max; i++)
                {
                    float factor = i / (max - 1f);
                    var unit = i == 0 ? flow.position[0] - flow.position[1] : flow.position[i - 1] - flow.position[i];
                    unit = new Vector2(-unit.Y, unit.X).SafeNormalize(default) * ((1 - (float)Math.Cos(MathHelper.TwoPi * factor)) * 0.5f) * 16;
                    infos[2 * i] = new CustomVertexInfo(flow.position[i] + unit, Color.White with { A = (byte)(flow.fade * 255) } * .5f, new Vector3(factor, 0, flow.fade));
                    infos[2 * i + 1] = new CustomVertexInfo(flow.position[i] - unit, Color.White with { A = (byte)(flow.fade * 255) } * .5f, new Vector3(factor, 1, flow.fade));
                }
                for (int i = 0; i < max * 2 - 2; i += 2)
                {
                    //if (i == 28) continue;
                    vertexInfos.Add(infos[i]);
                    vertexInfos.Add(infos[i + 2]);
                    vertexInfos.Add(infos[i + 1]);
                    vertexInfos.Add(infos[i + 1]);
                    vertexInfos.Add(infos[i + 2]);
                    vertexInfos.Add(infos[i + 3]);
                }
            }
            //for (int n = 0; n < vertexInfos.Count - 1; n++)
            //{
            //    sb.DrawLine(vertexInfos[n].Position, vertexInfos[n + 1].Position, Color.Red, 4, false, -Main.screenPosition);
            //}
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, trans);
            ShaderSwooshEX.Parameters["uTransform"].SetValue(result);
            ShaderSwooshEX.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            ShaderSwooshEX.Parameters["checkAir"].SetValue(false);
            ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
            ShaderSwooshEX.Parameters["gather"].SetValue(false);
            ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity);
            ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
            ShaderSwooshEX.Parameters["distortScaler"].SetValue(1);
            ShaderSwooshEX.Parameters["alphaFactor"].SetValue(1);
            ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(false);
            Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.BaseTex[8].Value;
            Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.AniTex[10].Value;
            Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;
            Main.graphics.GraphicsDevice.Textures[3] = GetTexture("greyBar");
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicWrap;
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;
            ShaderSwooshEX.CurrentTechnique.Passes[2].Apply();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertexInfos.ToArray(), 0, vertexInfos.Count / 3);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, trans);
            //sb.Draw(vectorTex, Projectile.Center - Main.screenPosition, null, Color.Purple, IHat.ToRotation(), new Vector2(0, 11), new Vector2(IHat.Length() * 4 / 78f, MathF.Sqrt(IHat.Length()) / 11f) * 4, 0, 0);
            //sb.Draw(vectorTex, Projectile.Center - Main.screenPosition, null, Color.Pink, JHat.ToRotation(), new Vector2(0, 11), new Vector2(JHat.Length() * 4 / 78f, MathF.Sqrt(JHat.Length()) / 11f) * 4, 0, 0);
            return false;
        }
    }
    public class FinalFractal : WitheredWoodSword
    {
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<PureFractal_Old>().QuickAddIngredient(4144, 3368).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe().AddIngredient<FirstZenith_Old>().QuickAddIngredient(4144, 3368).AddTile(TileID.LunarCraftingStation).Register();
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            item.ShaderItemEffectInventory(spriteBatch, position, origin, LogSpiralLibraryMod.Misc[0].Value, Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * ModTime) / 2 + 0.5f), scale);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.ShaderItemEffectInWorld(spriteBatch, LogSpiralLibraryMod.Misc[0].Value, Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * ModTime) / 2 + 0.5f), rotation);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int n = 1; n < 4; n++)
            {
                tooltips.Add(new TooltipLine(Mod, "PureSuggestion", Language.GetTextValue("Mods.CoolerItemVisualEffect.FinalFractalTip." + n)) { OverrideColor = Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * (ModTime + 40 * n)) / 2 + 0.5f) });
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.damage = 350;
            item.DamageType = DamageClass.Melee;
            item.width = 64;
            item.height = 74;
            item.rare = ItemRarityID.Purple;
            item.useTime = 12;
            item.useAnimation = 12;
            item.knockBack = 8;
            item.useStyle = ItemUseStyleID.Swing;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
        }
    }
}
