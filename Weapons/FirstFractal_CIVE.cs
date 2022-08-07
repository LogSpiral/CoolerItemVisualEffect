using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ID;
using static Terraria.Utils;
using Terraria.Enums;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;

namespace CoolerItemVisualEffect.Weapons
{
    public class FirstFractal_CIVE : ModItem
    {
        //public override void SetStaticDefaults()
        //{
        //    DisplayName.SetDefault("$Mods.CoolerItemVisualEffect.ItemName.第一分形");
        //    //DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "第一分形");
        //}
        public override void SetDefaults()
        {
            Item.useStyle = 5;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.useAnimation = 35;
            Item.useTime = Item.useAnimation / 5;
            Item.shootSpeed = 16f;
            Item.damage = 190;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.crit = 10;
            Item.rare = 10;
            Item.shoot = ProjectileID.FirstFractal;
            Item.glowMask = 271;
            //Item.CloneDefaults(ItemID.FirstFractal);

            //Item.holdStyle = 1;火把一类
            //Item.holdStyle = 2;雨伞一类
            //Item.holdStyle = 3;和1一样但是没有物品未湿润条件
            //Item.holdStyle = 4;貌似是高尔夫球杆
            //Item.holdStyle = 5;没有纵向速度限制和重力方向限制的4
            //Item.holdStyle = 6;提灯，玩家另一面的手臂会举起来
        }
        private bool GetSparkleGuitarTarget(out List<NPC> validTargets)
        {
            Player player = Main.player[Main.myPlayer];
            validTargets = new List<NPC>();
            Rectangle value = Utils.CenteredRectangle(player.Center, new Vector2(1000f, 800f));
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false) && npc.Hitbox.Intersects(value))
                {
                    validTargets.Add(npc);
                }
            }
            return validTargets.Count != 0;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 value5 = Main.MouseWorld;
            List<NPC> list2;
            bool sparkleGuitarTarget2 = GetSparkleGuitarTarget(out list2);
            if (sparkleGuitarTarget2)
            {
                NPC NPC2 = list2[Main.rand.Next(list2.Count)];
                value5 = NPC2.Center + NPC2.velocity * 20f;
            }
            Vector2 vector32 = value5 - player.Center;
            Vector2 vector33 = Main.rand.NextVector2CircularEdge(1f, 1f);
            float num78 = 1f;
            int num79 = 1;
            for (int num80 = 0; num80 < num79; num80++)
            {
                if (!sparkleGuitarTarget2)
                {
                    value5 += Main.rand.NextVector2Circular(24f, 24f);
                    if (vector32.Length() > 700f)
                    {
                        vector32 *= 700f / vector32.Length();
                        value5 = player.Center + vector32;
                    }
                    float num81 = Utils.GetLerpValue(0f, 6f, player.velocity.Length(), true) * 0.8f;
                    vector33 *= 1f - num81;
                    vector33 += player.velocity * num81;
                    vector33 = vector33.SafeNormalize(Vector2.UnitX);
                }
                float num82 = 60f;
                float num83 = Main.rand.NextFloatDirection() * 3.14159274f * (1f / num82) * 0.5f * num78;
                float num84 = num82 / 2f;
                float scaleFactor3 = 12f + Main.rand.NextFloat() * 2f;
                Vector2 vector34 = vector33 * scaleFactor3;
                Vector2 vector35 = new Vector2(0f, 0f);
                Vector2 vector36 = vector34;
                int num85 = 0;
                while (num85 < num84)
                {
                    vector35 += vector36;
                    vector36 = vector36.RotatedBy(num83, default);
                    num85++;
                }
                Vector2 value6 = -vector35;
                Vector2 position1 = value5 + value6;
                float lerpValue2 = Utils.GetLerpValue(player.itemAnimationMax, 0f, player.itemAnimation, true);
                Projectile.NewProjectile(source, position1, vector34, type, damage, knockback, player.whoAmI, num83, lerpValue2);
            }
            return false;
        }
        public override void AddRecipes()
        {
            //Recipe recipe = CreateRecipe();
            //recipe.QuickAddIngredient(
            //ItemID.TerraBlade,
            //ItemID.Meowmere,
            //ItemID.StarWrath,
            //ItemID.InfluxWaver,
            //ItemID.TheHorsemansBlade,
            //ItemID.Seedler,
            //ItemID.EnchantedSword,
            //ItemID.BeeKeeper,
            //ItemID.Starfury,
            //ItemID.CopperShortsword);
            //recipe.AddTile(TileID.MythrilAnvil);
            //recipe.ReplaceResult(this);
            //recipe.Register();

            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<LivingWoodSword>();
            recipe.AddIngredient<MossStoneSword>();
            recipe.AddIngredient<RefinedSteelBlade>();
            recipe.QuickAddIngredient(3258, 3823, 676, 3106, 671, 1928, 3827, 4923);

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.ReplaceResult(this);
            recipe.Register();

            //recipe = CreateRecipe();
            //recipe.AddIngredient(ItemID.Zenith);
            //recipe.ReplaceResult(this);
            //recipe.Register();

            //recipe = CreateRecipe();
            //recipe.AddIngredient(this);
            //recipe.ReplaceResult(ItemID.Zenith);
            //recipe.Register();
        }
    }

    public class WitheredWoodSword : ModItem
    {
        public Item item => Item;

        public override void SetDefaults()
        {
            item.damage = 30;
            item.crit = 21;
            item.DamageType = DamageClass.Melee;
            item.width = 50;
            item.height = 66;
            item.rare = 4;
            item.useTime = 30;
            item.useAnimation = 30;
            item.knockBack = 6;
            item.useStyle = 1;
            item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ModContent.ProjectileType<WitheredWoodSwordProj>();
                item.shootSpeed = 1f;
                item.noUseGraphic = true;
                item.noMelee = true;
                item.mana = 50;
            }
            else
            {
                item.shoot = 0;
                item.shootSpeed = 0;
                item.noUseGraphic = false;
                item.noMelee = false;
                item.mana = 0;

            }
            return player.ownedProjectileCounts[item.shoot] < 1; ;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
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
    }
    public class LivingWoodSword : WitheredWoodSword
    {
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ModContent.ProjectileType<WitheredWoodSwordProj>();
                item.shootSpeed = 1f;
                item.noUseGraphic = true;
                item.noMelee = true;
                item.mana = 30;
            }
            else
            {
                item.shoot = 0;
                item.shootSpeed = 0;
                item.noUseGraphic = false;
                item.noMelee = false;
                item.mana = 0;

            }
            return player.ownedProjectileCounts[item.shoot] < 1; ;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 58;
            Item.height = 72;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.damage = 60;
            Item.rare = 8;
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
    public class WitheredWoodSwordProj : VertexHammerProj
    {
        public override string HammerName => base.HammerName;
        public override float MaxTime => (controlState == 2 ? 2f : 1f) * UpgradeValue(12, 9);
        public override float factor => base.factor;
        public override Vector2 CollidingSize => base.CollidingSize;
        //public override Vector2 projCenter => base.projCenter + new Vector2(Player.direction * 16, -16);
        public override Vector2 CollidingCenter => base.CollidingCenter;//new Vector2(projTex.Size().X / 3 - 16, 16)
        public override Vector2 DrawOrigin => base.DrawOrigin + new Vector2(-12, 12);
        public override Color color => base.color;
        public override Color VertexColor(float time) => Color.Lerp(Color.DarkGreen, UpgradeValue(Color.Brown, Color.Green), time);//Color.Lerp(UpgradeValue(Color.Brown, Color.Green), Color.DarkGreen, time)
        public override float MaxTimeLeft => (controlState == 2 ? 0.75f : 1f) * UpgradeValue(8, 7);
        public override float Rotation => base.Rotation;
        public override bool UseRight => true;
        public override bool UseLeft => false;
        public override (int X, int Y) FrameMax => (2, 1);
        public override void Kill(int timeLeft)
        {

            //Lighting.add

            int max = (int)(30 * factor);
            var vec = (CollidingCenter - DrawOrigin).RotatedBy(Rotation) + projCenter;
            if (factor > 0.75f)
            {
                for (int n = 0; n < max; n++)
                {
                    Dust.NewDustPerfect(vec, UpgradeValue(MyDustId.Wood, Main.rand.Next(new int[] { MyDustId.Wood, MyDustId.GreenGrass })), (MathHelper.TwoPi / max * n).ToRotationVector2() * Main.rand.NextFloat(2, 8)).noGravity = true;
                }
            }
            //if (factor == 1)
            //{
            //    Projectile.NewProjectile(projectile.GetSource_FromThis(), vec, default, ModContent.ProjectileType<HolyExp>(), player.GetWeaponDamage(player.HeldItem) * 3, projectile.knockBack, projectile.owner);
            //}
            base.Kill(timeLeft);
        }
        public virtual void OnChargedShoot()
        {
            SoundEngine.PlaySound(SoundID.Item60, projectile.position);

            int count = 0;
            int max = UpgradeValue(5, 15);
            foreach (var npc in Main.npc)
            {
                if (!npc.friendly && npc.active && npc.CanBeChasedBy() && (npc.Center - Player.Center).Length() < UpgradeValue(512, 768))
                {
                    npc.velocity *= UpgradeValue(0.25f, 0.125f);
                    if (count < max)
                    {
                        count++;
                        var unit = Main.rand.NextVector2Unit();
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), npc.Center - unit * 64, unit * UpgradeValue(16, 32), ProjectileID.NettleBurstRight, Projectile.damage / 3, projectile.knockBack, projectile.owner);
                    }
                }
            }
            if (projectile.owner == Main.myPlayer) Player.velocity += (Main.MouseWorld - Player.Center).SafeNormalize(default) * UpgradeValue(16, 24) * new Vector2(1, 0.25f);
        }
        public override void OnRelease(bool charged, bool left)
        {
            if (Charged)
            {
                if (System.Math.Abs(Player.velocity.Y) < 1f)
                {
                    Player.velocity.X *= 0.95f;
                }
                if ((int)projectile.ai[1] == 1)
                {
                    OnChargedShoot();
                }
            }
            //Main.NewText(new NPCs.Baron.Baron().CanTownNPCSpawn(10, 10));
            base.OnRelease(charged, left);
        }
        public override Rectangle? frame => projTex.Frame(2, 1, UpgradeValue(0, 1));
        public Item sourceItem;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemSource)
            {
                sourceItem = itemSource.Item;
            }
        }
        public virtual T UpgradeValue<T>(T normal, T extra, T defaultValue = default)
        {
            //var type = Player.HeldItem.type;
            var type = sourceItem.type;

            if (type == ModContent.ItemType<WitheredWoodSword>())
            {
                return normal;
            }

            if (type == ModContent.ItemType<LivingWoodSword>())
            {
                return extra;
            }

            return defaultValue;
        }
    }
    public class SereStoneSword : ModItem
    {
        public Item item => Item;
        public override void SetDefaults()
        {
            item.damage = 40;
            item.crit = 26;
            item.DamageType = DamageClass.Melee;
            item.width = 48;
            item.height = 48;
            item.rare = 5;
            item.useTime = 25;
            item.useAnimation = 25;
            item.knockBack = 8;
            item.useStyle = 1;
            item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ModContent.ProjectileType<SereStoneSwordProj>();
                item.shootSpeed = 1f;
                item.noUseGraphic = true;
                item.noMelee = true;
                item.mana = 50;
            }
            else
            {
                item.shoot = 0;
                item.shootSpeed = 0;
                item.noUseGraphic = false;
                item.noMelee = false;
                item.mana = 0;

            }
            return player.ownedProjectileCounts[item.shoot] < 1;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
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
    public class MossStoneSword : SereStoneSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.damage = 70;
            item.width = 50;
            item.rare = 8;
            item.useTime = 18;
            item.useAnimation = 18;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ModContent.ProjectileType<SereStoneSwordProj>();
                item.shootSpeed = 1f;
                item.noUseGraphic = true;
                item.noMelee = true;
                item.mana = 40;
            }
            else
            {
                item.shoot = 0;
                item.shootSpeed = 0;
                item.noUseGraphic = false;
                item.noMelee = false;
                item.mana = 0;

            }
            return player.ownedProjectileCounts[item.shoot] < 1;
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
    public class SereStoneSwordProj : WitheredWoodSwordProj
    {
        public static void ShootSharpTears(Vector2 targetPos, Player player, Projectile projectile)
        {
            player.LimitPointToPlayerReachableArea(ref targetPos);
            Vector2 vector22 = targetPos + Main.rand.NextVector2Circular(8f, 8f);
            Vector2 value7 = player.FindSharpTearsSpot(vector22).ToWorldCoordinates(Main.rand.Next(17), Main.rand.Next(17));
            if ((player.Center - value7).Length() < 48) value7 = targetPos;
            Vector2 vector23 = (vector22 - value7).SafeNormalize(-Vector2.UnitY) * 16f;
            Projectile.NewProjectile(projectile.GetSource_FromThis(), value7.X, value7.Y, vector23.X, vector23.Y, ModContent.ProjectileType<SharpStoneTears>(), projectile.damage / 4, projectile.knockBack, player.whoAmI, 0f, Main.rand.NextFloat() * 0.5f + 0.5f);
        }
        public override void OnRelease(bool charged, bool left)
        {
            if (Charged)
            {
                int max = UpgradeValue(1, 3);
                for (int n = 0; n < max; n++)
                {
                    Vector2 pointPoisition2 = Player.Center + new Vector2(128 * Player.direction, 0) * ((projectile.ai[1] + (float)n / max) / MaxTimeLeft) * max;
                    ShootSharpTears(pointPoisition2, Player, projectile);
                }
            }
            if ((int)projectile.ai[1] == 0)
            {
                projectile.damage = 0;
                if (Charged)
                {
                    projectile.damage = (int)(Player.GetWeaponDamage(Player.HeldItem) * (3 * factor * factor));
                    SoundEngine.PlaySound(Terraria.ID.SoundID.Item71);
                }
            }
            projectile.ai[1]++;
            if (projectile.ai[1] > (Charged ? (MaxTimeLeft * factor) : timeCount))
            {
                projectile.Kill();
            }
        }
        public override Color VertexColor(float time) => Color.Lerp(Color.DarkGray, UpgradeValue(Color.Gray, Color.Green), time);
        public override T UpgradeValue<T>(T normal, T extra, T defaultValue = default)
        {
            var type = sourceItem.type;

            if (type == ModContent.ItemType<SereStoneSword>())
            {
                return normal;
            }

            if (type == ModContent.ItemType<MossStoneSword>())
            {
                return extra;
            }
            return defaultValue;
        }
    }
    public class SharpStoneTears : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }
        public override void Kill(int timeLeft)
        {
            for (float num6 = 0f; num6 < 1f; num6 += 0.025f)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f) * Projectile.scale + Projectile.velocity.SafeNormalize(Vector2.UnitY) * num6 * 200f * Projectile.scale, MyDustId.GreyStone, Main.rand.NextVector2Circular(3f, 3f));
                dust2.velocity.Y += -0.3f;
                Dust dust = dust2;
                dust.velocity += Projectile.velocity * 0.2f;
                dust2.scale = 1f;
                dust2.alpha = 100;
            }
        }
        Projectile projectile => Projectile;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value33 = TextureAssets.Projectile[projectile.type].Value;
            Rectangle value34 = value33.Frame(1, 6, 0, projectile.frame);
            Vector2 origin11 = new Vector2(16f, value34.Height / 2);
            Color alpha4 = projectile.GetAlpha(lightColor);
            Vector2 scale8 = new Vector2(projectile.scale);
            float lerpValue4 = Utils.GetLerpValue(30f, 25f, projectile.ai[0], clamped: true);
            scale8.Y *= lerpValue4;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY) - projectile.velocity * projectile.scale * 0.5f, null, projectile.GetAlpha(lightColor * 0.262745f) * 1f, projectile.rotation + (float)Math.PI / 2f, TextureAssets.Extra[98].Value.Size() / 2f, projectile.scale * 0.9f, spriteEffects, 0);
            Main.EntitySpriteDraw(value33, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), value34, alpha4, projectile.rotation, origin11, scale8, spriteEffects, 0);
            return false;
        }
        public override void AI()
        {
            int num = MyDustId.GreyStone;
            float scaleFactor = 1f;
            int num2 = 30;
            int num3 = 30;
            int num4 = 2;
            int num5 = 2;
            int maxValue = 6;

            bool flag = Projectile.ai[0] < 20;
            bool flag2 = Projectile.ai[0] >= 20;
            bool flag3 = Projectile.ai[0] >= 30;
            Projectile.ai[0] += 1f;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.frame = Main.rand.Next(maxValue);
                for (int i = 0; i < num2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), num, Projectile.velocity * scaleFactor * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat()));
                    dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                }

                for (int j = 0; j < num3; j++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), num, Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * scaleFactor * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust2.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust2.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                    dust2.fadeIn = 1f;
                }

                SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            }

            if (flag)
            {
                Projectile.Opacity += 0.1f;
                Projectile.scale = Projectile.Opacity * Projectile.ai[1];
                for (int k = 0; k < num4; k++)
                {
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f), num, Projectile.velocity * scaleFactor * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust3.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust3.velocity *= 0.5f;
                    dust3.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                }
            }

            if (flag2)
            {
                Projectile.Opacity -= 0.2f;
                for (int l = 0; l < num5; l++)
                {
                    Dust dust4 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f), num, Projectile.velocity * scaleFactor * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust4.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust4.velocity *= 0.5f;
                    dust4.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                }
            }

            if (flag3)
                Projectile.Kill();
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 200f * Projectile.scale, 22f * Projectile.scale, DelegateMethods.CutTiles);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint15 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 200f * Projectile.scale, 22f * Projectile.scale, ref collisionPoint15))
                return true;
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
    }
    public class RustySteelBlade : ModItem
    {
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
    public class RefinedSteelBlade : ModItem
    {
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
}
