using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ID;

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
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(757, 1);
            recipe.AddIngredient(3063, 1);
            recipe.AddIngredient(3065, 1);
            recipe.AddIngredient(2880, 1);
            recipe.AddIngredient(1826, 1);
            recipe.AddIngredient(3018, 1);
            recipe.AddIngredient(989, 1);
            recipe.AddIngredient(1123, 1);
            recipe.AddIngredient(65, 1);
            recipe.AddIngredient(3507, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.ReplaceResult(this);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Zenith);
            recipe.ReplaceResult(this);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddIngredient(this);
            recipe.ReplaceResult(ItemID.Zenith);
            recipe.Register();
        }
    }
}
