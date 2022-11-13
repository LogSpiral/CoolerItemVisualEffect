using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CoolerItemVisualEffect;
using static Terraria.ModLoader.ModContent;
using CoolerItemVisualEffect.Weapons;
using Terraria.GameContent;
using System.IO;
using Terraria.Localization;

namespace CoolerItemVisualEffect.FinalFractal
{
    public class FinalFractalPlayer : ModPlayer
    {
        public Player player => Player;
        public bool holdingFinalFractal = false;
        public int usingFinalFractal = 0;
        public bool usedFinalFractal = false;
        public int waitingFinalFractal = 0;
        public int finalFractalTier = 0;
        public int firstTierCounter;
        public override void ResetEffects()
        {

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
                packet.Write((byte)HandleNetwork.MessageType.FinalFractalPlayer);
                packet.Write(holdingFinalFractal);
                packet.Write(usingFinalFractal);
                packet.Write(usedFinalFractal);
                packet.Write(waitingFinalFractal);
                packet.Write(finalFractalTier);
                packet.Send(-1, -1);
            }
        }
    }
    public class FinalFractal_Old : ModItem
    {
        //public override void SetStaticDefaults()
        //{
        //    DisplayName.SetDefault("最终分形");
        //    Tooltip.SetDefault("它的一部分包含着它，这就是分形。无数的刀刃回旋着，它们是它的一部分。");
        //}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int n = 1; n < 4; n++)
            {
                tooltips.Add(new TooltipLine(Mod, "PureSuggestion", Language.GetTextValue("Mods.CoolerItemVisualEffect.FinalFractalTip." + n)) { OverrideColor = Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * (CoolerItemVisualEffect.ModTime + 40 * n)) / 2 + 0.5f) });

            }

            if (Main.LocalPlayer.name != "")
            {
                tooltips.Add(new TooltipLine(Mod, "UShallNotPass!!!", Language.GetTextValue("Mods.CoolerItemVisualEffect.ItemName.UShallNotPass")) { OverrideColor = Color.Red });
            }
            //tooltips.Add(new TooltipLine(Mod, "PureSuggestion", "「最初与最后的究极分形」") { OverrideColor = Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * CoolerItemVisualEffect.ModTime) / 2 + 0.5f) });
            //tooltips.Add(new TooltipLine(Mod, "PureSuggestion", "「分形次元斩」") { OverrideColor = Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * (CoolerItemVisualEffect.ModTime + 40)) / 2 + 0.5f) });
            //tooltips.Add(new TooltipLine(Mod, "PureSuggestion", "天顶「FinalFractal」") { OverrideColor = Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * (CoolerItemVisualEffect.ModTime + 80)) / 2 + 0.5f) });

        }
        Item item => Item;
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<PureFractal_Old>().QuickAddIngredient(4144, 3368).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe().AddIngredient<FirstZenith_Old>().QuickAddIngredient(4144, 3368).AddTile(TileID.LunarCraftingStation).Register();
            //var recipe = CreateRecipe();
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
            //ItemID.CopperShortsword, 
            //3258, 
            //3823, 
            //676, 
            //3106, 
            //671, 
            //1928, 
            //3827, 
            //4923,
            //ItemID.WoodenSword,
            //ItemID.BorealWoodSword,
            //ItemID.PalmWoodSword,
            //ItemID.RichMahoganySword,
            //ItemID.ShadewoodSword,
            //ItemID.PearlwoodSword,
            //ItemID.CactusSword);
            //for (int n = 0; n < 6; n++)
            //    recipe.AddIngredient(3764 + n);//六种晶光刃
            //recipe.AddIngredient(ItemID.OrangePhasesaber);
            //recipe.AddIngredient(ItemID.BoneSword);
            //recipe.AddIngredient(ItemID.AntlionClaw);
            //recipe.AddIngredient(ItemID.BeamSword);
            //recipe.AddIngredient(ItemID.PurpleClubberfish);
            //recipe.AddIngredient(ItemID.Bladetongue);
            //recipe.AddIngredient(ItemID.StoneBlock, 500);
            //recipe.AddIngredient(ItemID.EbonstoneBlock, 500);
            //recipe.AddIngredient(ItemID.CrimstoneBlock, 500);
            //recipe.AddIngredient(ItemID.PearlstoneBlock, 500);
            //recipe.AddIngredient(ItemID.Sandstone, 500);
            //recipe.AddIngredient(ItemID.CorruptSandstone, 500);
            //recipe.AddIngredient(ItemID.CrimsonSandstone, 500);
            //recipe.AddIngredient(ItemID.HallowSandstone, 500);
            //recipe.AddIngredient(ItemID.Marble, 500);
            //recipe.AddIngredient(ItemID.Granite, 500);
            //recipe.AddIngredient(ItemID.Obsidian, 50);
            //recipe.AddTile(TileID.HeavyWorkBench);
            //recipe.QuickAddIngredient(
            //ItemID.CopperBroadsword,
            //ItemID.TinBroadsword,
            //ItemID.IronBroadsword,
            //ItemID.LeadBroadsword,
            //ItemID.SilverBroadsword,
            //ItemID.TungstenBroadsword,
            //ItemID.GoldBroadsword,
            //ItemID.PlatinumBroadsword,
            //ItemID.Gladius,
            //ItemID.Katana,
            //ItemID.DyeTradersScimitar,
            //ItemID.FalconBlade,
            //ItemID.CobaltSword,
            //ItemID.PalladiumSword,
            //ItemID.MythrilSword,
            //ItemID.OrichalcumSword,
            //ItemID.BreakerBlade,
            //ItemID.Cutlass,
            //ItemID.AdamantiteSword,
            //ItemID.TitaniumSword,
            //ItemID.ChlorophyteSaber,
            //ItemID.ChlorophyteClaymore);
            //recipe.AddIngredient(ItemID.BrokenHeroSword, 5);
            //recipe.AddIngredient(ItemID.Mushroom, 50);
            //recipe.AddIngredient(ItemID.GlowingMushroom, 50);
            //recipe.AddIngredient(ItemID.Acorn, 50);
            //recipe.AddIngredient(ItemID.BambooBlock, 15);
            //recipe.AddTile(TileID.LivingLoom);
            //recipe.AddTile(TileID.MythrilAnvil);
            //recipe.AddIngredient<FirstFractal_CIVE>();
            //recipe.AddTile(TileID.LunarCraftingStation);
            //recipe.Register();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            item.ShaderItemEffectInventory(spriteBatch, position, origin, CoolerItemVisualEffectMethods.GetTexture("IMBellTex"), Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * CoolerItemVisualEffect.ModTime) / 2 + 0.5f), scale);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.ShaderItemEffectInWorld(spriteBatch, CoolerItemVisualEffectMethods.GetTexture("IMBellTex"), Color.Lerp(new Color(99, 74, 187), new Color(20, 120, 118), (float)Math.Sin(MathHelper.Pi / 60 * CoolerItemVisualEffect.ModTime) / 2 + 0.5f), rotation);
        }
        public override void SetDefaults()
        {
            item.damage = 350;
            item.DamageType = DamageClass.Melee;
            item.width = 64;
            item.height = 74;
            item.rare = 11;
            item.useTime = 12;
            item.useAnimation = 12;
            item.knockBack = 8;
            item.useStyle = ItemUseStyleID.Swing;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.shoot = ProjectileType<FinalFractalItem>();
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1/* && player.name == ""*/;
        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.GetModPlayer<FinalFractalPlayer>().holdingFinalFractal = true;
                if (CanUseItem(player))
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Center.X, player.Center.Y, 0f, -1f, item.shoot, player.GetWeaponDamage(item), 8, player.whoAmI, 0f, 0f);
                }
            }
        }
    }

    public class FinalFractalItem : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终分形");
        }
        private Player Player => Main.player[Projectile.owner];
        public bool right;
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            right = reader.ReadBoolean();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(right);
        }
        private bool Right
        {
            get => right;
            set { if (Player.GetModPlayer<FinalFractalPlayer>().usingFinalFractal <= 0) { right = value; } }//else Main.NewText(Player.direction);
        }
        private int useFirstTier
        {
            get => (int)Projectile.ai[1];
            set { projectile.ai[1] = value; }
        }
        private int zenithProjCount
        {
            get => (int)Projectile.ai[0];
            set { projectile.ai[0] = value; }
        }
        Projectile projectile => Projectile;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            FinalFractalPlayer illusionBoundPlayer = Player.GetModPlayer<FinalFractalPlayer>();
            SpriteEffects spriteEffects;
            const float Coefficient = 2f;
            float useDegree;
            Right = Player.direction == 1;
            //Main.NewText(Right);
            if (Right)
            {
                spriteEffects = SpriteEffects.None;
                if (illusionBoundPlayer.usedFinalFractal)
                {
                    if (illusionBoundPlayer.finalFractalTier == 1)
                    {
                        useDegree = (float)(Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal) / Player.itemAnimationMax;
                        if (illusionBoundPlayer.usingFinalFractal != 0)
                        {
                            if (useDegree > 0.95f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_1"), Player.Center + new Vector2(0, -16) - Main.screenPosition, new Rectangle(324, 0, 162, 98), Color.White, 0, new Vector2(81, 49), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.8f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_1"), Player.Center + new Vector2(0, -16) - Main.screenPosition, new Rectangle(162, 0, 162, 98), Color.White, 0, new Vector2(81, 49), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.6f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_1"), Player.Center + new Vector2(0, -16) - Main.screenPosition, new Rectangle(0, 0, 162, 98), Color.White, 0, new Vector2(81, 49), 1f, spriteEffects, 0);
                            }
                        }
                        Player.bodyFrame.Y = 56 * (4 - (int)((MathHelper.Pi / 8 - (MathHelper.Pi / 6 * 5 + MathHelper.Pi / 8) * (1 - (float)Math.Pow(Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal, 2) / (Player.itemAnimationMax * Player.itemAnimationMax))) / (MathHelper.Pi / 8 - (MathHelper.Pi / 6 * 5 + MathHelper.Pi / 8)) * 4));
                        spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value, Player.Center + new Vector2(-4, 4) - Main.screenPosition, null, Color.White, MathHelper.Pi / 8 - (MathHelper.Pi / 6 * 5 + MathHelper.Pi / 8) * (1 - (float)Math.Pow(Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal, 2) / (Player.itemAnimationMax * Player.itemAnimationMax)), new Vector2(12, 16), 1f, spriteEffects, 0);
                    }
                    if (illusionBoundPlayer.finalFractalTier == 2)
                    {
                        useDegree = (float)((int)(Player.itemAnimationMax / 3f * 2) - illusionBoundPlayer.usingFinalFractal) / (int)(Player.itemAnimationMax / 3f * 2);
                        Player.bodyFrame.Y = 1064;
                        spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value, Player.Center + new Vector2(-4, 4) + new Vector2(-1.414213562373095f * 1.732050807568877f, 1.414213562373095f) * Coefficient * (Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal * 2) / Player.itemAnimationMax - Main.screenPosition, null, Color.White, 0, new Vector2(12, 16), 1f, spriteEffects, 0);
                        if (illusionBoundPlayer.usingFinalFractal != 0)
                        {
                            if (useDegree > 0.66f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_2"), Player.Center + new Vector2(48, 0) - Main.screenPosition, new Rectangle(160, 0, 80, 38), Color.White, 0, new Vector2(40, 19), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.33f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_2"), Player.Center + new Vector2(48, 0) - Main.screenPosition, new Rectangle(80, 0, 80, 38), Color.White, 0, new Vector2(40, 19), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.1f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_2"), Player.Center + new Vector2(48, 0) - Main.screenPosition, new Rectangle(0, 0, 80, 38), Color.White, 0, new Vector2(40, 19), 1f, spriteEffects, 0);
                            }
                        }
                    }
                    if (illusionBoundPlayer.finalFractalTier == 0)
                    {
                        useDegree = (float)((int)(Player.itemAnimationMax / 3f * 2) - illusionBoundPlayer.usingFinalFractal) / (int)(Player.itemAnimationMax / 3f * 2);
                        Player.bodyFrame.Y = 168;
                        Vector2 vec1 = new Vector2(-1.414213562373095f * 1.732050807568877f, 1.414213562373095f);
                        Vector2 vec2 = new Vector2(1.414213562373095f * 2f, 0);
                        Vector2 adder = vec2 - vec1;
                        spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value, Player.Center + new Vector2(-4, 4) + new Vector2(-1.414213562373095f * 1.732050807568877f, 1.414213562373095f) * Coefficient + adder * Coefficient * (Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal * 2) / Player.itemAnimationMax - Main.screenPosition, null, Color.White, 0, new Vector2(12, 16), 1f, spriteEffects, 0);
                        if (illusionBoundPlayer.usingFinalFractal != 0)
                        {
                            if (useDegree > 0.66f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_3"), Player.Center + new Vector2(64, 0) - Main.screenPosition, new Rectangle(180, 0, 90, 22), Color.White, 0, new Vector2(45, 11), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.33f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_3"), Player.Center + new Vector2(64, 0) - Main.screenPosition, new Rectangle(90, 0, 90, 22), Color.White, 0, new Vector2(45, 11), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.1f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_3"), Player.Center + new Vector2(64, 0) - Main.screenPosition, new Rectangle(0, 0, 90, 22), Color.White, 0, new Vector2(45, 11), 1f, spriteEffects, 0);
                            }
                        }
                    }
                }
                else
                {
                    Player.bodyFrame.Y = 0;
                    spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value, Player.Center + new Vector2(-6, 6) - Main.screenPosition, null, Color.White, MathHelper.Pi / 12, new Vector2(12, 16), 1f, spriteEffects, 0);
                }
            }
            else
            {
                spriteEffects = SpriteEffects.FlipVertically;
                if (illusionBoundPlayer.usedFinalFractal)
                {
                    if (illusionBoundPlayer.finalFractalTier == 1)
                    {
                        useDegree = (float)(Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal) / Player.itemAnimationMax;
                        if (illusionBoundPlayer.usingFinalFractal != 0)
                        {
                            if (useDegree > 0.95f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_1"), Player.Center + new Vector2(0, -16) - Main.screenPosition, new Rectangle(324, 0, 162, 98), Color.White, -MathHelper.Pi, new Vector2(81, 49), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.8f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_1"), Player.Center + new Vector2(0, -16) - Main.screenPosition, new Rectangle(162, 0, 162, 98), Color.White, -MathHelper.Pi, new Vector2(81, 49), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.6f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_1"), Player.Center + new Vector2(0, -16) - Main.screenPosition, new Rectangle(0, 0, 162, 98), Color.White, -MathHelper.Pi, new Vector2(81, 49), 1f, spriteEffects, 0);
                            }
                        }
                        Player.bodyFrame.Y = 56 * (4 - (int)((MathHelper.Pi / 8 - (MathHelper.Pi / 6 * 5 + MathHelper.Pi / 8) * (1 - (float)Math.Pow(Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal, 2) / (Player.itemAnimationMax * Player.itemAnimationMax))) / (MathHelper.Pi / 8 - (MathHelper.Pi / 6 * 5 + MathHelper.Pi / 8)) * 4));
                        spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value, Player.Center + new Vector2(4, 4) - Main.screenPosition, null, Color.White, -MathHelper.Pi - (MathHelper.Pi / 8 - (MathHelper.Pi / 6 * 5 + MathHelper.Pi / 8) * (1 - (float)Math.Pow(Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal, 2) / (Player.itemAnimationMax * Player.itemAnimationMax))), new Vector2(12, 11), 1f, spriteEffects, 0);

                    }
                    if (illusionBoundPlayer.finalFractalTier == 2)
                    {
                        useDegree = (float)((int)(Player.itemAnimationMax / 3f * 2) - illusionBoundPlayer.usingFinalFractal) / (int)(Player.itemAnimationMax / 3f * 2);
                        Player.bodyFrame.Y = 1064;
                        spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value, Player.Center + new Vector2(4, 4) + new Vector2(1.414213562373095f * 1.732050807568877f, 1.414213562373095f) * Coefficient * (Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal * 2) / Player.itemAnimationMax - Main.screenPosition, null, Color.White, -MathHelper.Pi, new Vector2(12, 11), 1f, spriteEffects, 0);
                        if (illusionBoundPlayer.usingFinalFractal != 0)
                        {
                            if (useDegree > 0.66f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_2"), Player.Center + new Vector2(-64, 0) - Main.screenPosition, new Rectangle(160, 0, 80, 38), Color.White, -MathHelper.Pi, new Vector2(40, 19), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.33f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_2"), Player.Center + new Vector2(-64, 0) - Main.screenPosition, new Rectangle(80, 0, 80, 38), Color.White, -MathHelper.Pi, new Vector2(40, 19), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.1f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_2"), Player.Center + new Vector2(-64, 0) - Main.screenPosition, new Rectangle(0, 0, 80, 38), Color.White, -MathHelper.Pi, new Vector2(40, 19), 1f, spriteEffects, 0);
                            }
                        }
                    }
                    if (illusionBoundPlayer.finalFractalTier == 0)
                    {
                        useDegree = (float)((int)(Player.itemAnimationMax / 3f * 2) - illusionBoundPlayer.usingFinalFractal) / (int)(Player.itemAnimationMax / 3f * 2);
                        Player.bodyFrame.Y = 168;
                        Vector2 vec1 = new Vector2(1.414213562373095f * 1.732050807568877f, 1.414213562373095f);
                        Vector2 vec2 = new Vector2(-1.414213562373095f * 2f, 0);
                        Vector2 adder = vec2 - vec1;
                        spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value, Player.Center + new Vector2(4, 4) + new Vector2(1.414213562373095f * 1.732050807568877f, 1.414213562373095f) * Coefficient + adder * Coefficient * (Player.itemAnimationMax - illusionBoundPlayer.usingFinalFractal * 2) / Player.itemAnimationMax - Main.screenPosition, null, Color.White, -MathHelper.Pi, new Vector2(12, 11), 1f, spriteEffects, 0);
                        if (illusionBoundPlayer.usingFinalFractal != 0)
                        {
                            if (useDegree > 0.66f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_3"), Player.Center + new Vector2(-48, 0) - Main.screenPosition, new Rectangle(180, 0, 90, 22), Color.White, -MathHelper.Pi, new Vector2(45, 11), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.33f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_3"), Player.Center + new Vector2(-48, 0) - Main.screenPosition, new Rectangle(90, 0, 90, 22), Color.White, -MathHelper.Pi, new Vector2(45, 11), 1f, spriteEffects, 0);
                            }
                            else if (useDegree > 0.1f)
                            {
                                spriteBatch.Draw(CoolerItemVisualEffectMethods.GetTexture("FinalFractalSwoosh_3"), Player.Center + new Vector2(-48, 0) - Main.screenPosition, new Rectangle(0, 0, 90, 22), Color.White, -MathHelper.Pi, new Vector2(45, 11), 1f, spriteEffects, 0);
                            }
                        }
                    }
                }
                else
                {
                    /*if (!(((p.velocity.Y == 0f || p.sliding) && p.releaseJump) || (p.autoJump && p.justJumped)))
                    {
                        p.bodyFrame.Y = 0;
                    }*/
                    Player.bodyFrame.Y = 0;
                    spriteBatch.Draw(TextureAssets.Projectile[projectile.type].Value, Player.Center + new Vector2(6, 6) - Main.screenPosition, null, Color.White, -MathHelper.Pi - MathHelper.Pi / 12, new Vector2(12, 16), 1f, spriteEffects, 0);
                }
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Right)
            {
                return targetHitbox.Intersects(new Rectangle(projectile.Hitbox.X + 96, projectile.Hitbox.Y, projectile.Hitbox.Width / 2, projectile.Hitbox.Height));
            }
            return targetHitbox.Intersects(new Rectangle(projectile.Hitbox.X, projectile.Hitbox.Y, projectile.Hitbox.Width / 2, projectile.Hitbox.Height));
        }
        private bool GetTarget(out List<NPC> validTargets)
        {
            validTargets = new List<NPC>();
            Rectangle value = Utils.CenteredRectangle(Player.Center, new Vector2(1000f, 800f));
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
        private bool GetZenithTarget(Vector2 searchCenter, float maxDistance, out int npcTargetIndex)
        {
            npcTargetIndex = 0;
            int? num = null;
            float num2 = maxDistance;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false))
                {
                    float num3 = Vector2.Distance(searchCenter, npc.Center);
                    if (num2 > num3)
                    {
                        num = new int?(i);
                        num2 = num3;
                    }
                }
            }
            if (num == null)
            {
                return false;
            }
            npcTargetIndex = num.Value;
            return true;
        }
        public override void AI()
        {
            projectile.damage = Player.GetWeaponDamage(Player.HeldItem);
            if (Player.HeldItem.type != ItemType<FinalFractal_Old>()) projectile.Kill();
            FinalFractalPlayer illusionBoundPlayer = Player.GetModPlayer<FinalFractalPlayer>();
            int utime = (int)CoolerItemVisualEffect.ModTime;
            int num = Player.name == "FFT" ? 1 : 3;
            //Player.ApplyItemAnimation(Player.HeldItem);
            Player.itemAnimationMax = (int)(Player.HeldItem.useAnimation / Player.GetWeaponAttackSpeed(Player.HeldItem));
            if (utime % num == 0 && zenithProjCount > 0)
            {
                {
                    var player = Main.player[projectile.owner];
                    zenithProjCount--;
                    Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true, true);
                    float num6 = Main.mouseX + Main.screenPosition.X - vector.X;
                    float num7 = Main.mouseY + Main.screenPosition.Y - vector.Y;
                    Vector2 velocity = new Vector2(num6, num7);
                    int num167 = Main.rand.Next(26);
                    if (zenithProjCount == 4)
                        num167 = 25;
                    Vector2 value7 = Main.MouseWorld - player.MountedCenter;
                    if (zenithProjCount < 5)
                    {
                        int num168;
                        bool zenithTarget = GetZenithTarget(Main.MouseWorld, 400f, out num168);
                        if (zenithTarget)
                        {
                            value7 = Main.npc[num168].Center - player.MountedCenter;
                        }
                        bool flag8 = zenithProjCount % 3 == 0;
                        if (zenithProjCount % 2 == 0 && !zenithTarget)
                        {
                            flag8 = true;
                        }
                        if (flag8)
                        {
                            value7 += Main.rand.NextVector2Circular(150f, 150f);
                        }
                    }
                    velocity = value7 / 2f;
                    //Vector2 velocity = new Vector2(num6, num7);
                    float ai5 = Main.rand.Next(-100, 101);
                    Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), player.Center, velocity, ProjectileType<PureFractalProj>(), projectile.damage, projectile.knockBack, player.whoAmI, ai5).frame = num167;
                }
                {
                    Vector2 value5 = Main.MouseWorld;
                    List<NPC> list2;
                    bool sparkleGuitarTarget2 = GetTarget(out list2);
                    if (sparkleGuitarTarget2)
                    {
                        NPC npc2 = list2[Main.rand.Next(list2.Count)];
                        value5 = npc2.Center + npc2.velocity * 20f;
                    }
                    Vector2 vector32 = value5 - Player.Center;
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
                                value5 = Player.Center + vector32;
                            }
                            float num81 = Utils.GetLerpValue(0f, 6f, Player.velocity.Length(), true) * 0.8f;
                            vector33 *= 1f - num81;
                            vector33 += Player.velocity * num81;
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
                        Vector2 position = value5 + value6;
                        float lerpValue2 = Main.rand.Next(0, 25);
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), position, vector34 * 2, ProjectileType<FinalFractalDimensionalSwoosh>(), projectile.damage, projectile.knockBack, projectile.owner, num83, lerpValue2);
                        //Projectile.NewProjectile(position + new Vector2(128, 112), vector34, ProjectileType<FinalFractalDimensionalSwoosh>(), projectile.damage, projectile.knockBack, projectile.owner, num83, lerpValue2);
                    }
                }
            }
            if (!Player.active || Player.dead)
            {
                projectile.Kill();
            }
            else if (illusionBoundPlayer.usingFinalFractal > 0)
            {
                projectile.friendly = true;
            }
            if (illusionBoundPlayer.usingFinalFractal > 0)
            {
                illusionBoundPlayer.usingFinalFractal--;
            }
            //else
            //{
            //    Right = Player.direction == 1;
            //    Main.NewText((Right,Player.direction));
            //}
            if (illusionBoundPlayer.usingFinalFractal == 0)
            {
                illusionBoundPlayer.waitingFinalFractal++;
            }
            if (illusionBoundPlayer.waitingFinalFractal >= 30 && illusionBoundPlayer.usedFinalFractal)
            {
                projectile.friendly = false;
                illusionBoundPlayer.usedFinalFractal = false;
                illusionBoundPlayer.finalFractalTier = 0;
                illusionBoundPlayer.waitingFinalFractal = 0;
            }
            if (Player.controlUseItem && illusionBoundPlayer.usingFinalFractal == 0)
            {
                illusionBoundPlayer.usedFinalFractal = true;
                illusionBoundPlayer.waitingFinalFractal = 0;
                if (illusionBoundPlayer.finalFractalTier == 0)
                {
                    illusionBoundPlayer.finalFractalTier = 1;
                    illusionBoundPlayer.usingFinalFractal = Player.itemAnimationMax;
                    Vector2 vec = Main.MouseWorld - Player.Center;
                    vec.Normalize();
                    //Projectile.NewProjectile(p.Center, vec * 48, ProjectileType<FinalFractalProjectile>(), projectile.damage, projectile.knockBack, p.whoAmI);
                    if (useFirstTier % 5 == 0)
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), Player.Center, vec * 48, ProjectileType<FinalFractalProjectile>(), projectile.damage, projectile.knockBack, Player.whoAmI);
                    }
                    useFirstTier++;
                }
                else if (illusionBoundPlayer.finalFractalTier == 1)
                {
                    zenithProjCount = Player.name == "FFT" ? 15 : 5;
                    illusionBoundPlayer.finalFractalTier = 2;
                    //for(int n = 0;n < 5; n++)
                    {
                        //Projectile.NewProjectile(Main.MouseWorld, new Vector2(32, 0).RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi)), ProjectileType<FinalFractalDimensionalSwoosh>(), projectile.damage, projectile.knockBack, p.whoAmI, 0, 1);
                    }
                    /*for (int n = 0; n < 5; n++)
                    {
                        float r = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        Projectile.NewProjectile(Main.MouseWorld + new Vector2(128, 0).RotatedBy(r), new Vector2(-16, 0).RotatedBy(r), ProjectileType<FinalFractalDimensionalSwoosh2>(), projectile.damage, 0, projectile.owner);
                    }*/
                    //{
                    //    var player = Main.player[projectile.owner];
                    //    Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true, true);
                    //    float num6 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
                    //    float num7 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
                    //    //int num166 = (player.itemAnimationMax - player.itemAnimation) / player.itemTime;
                    //    Vector2 velocity = new Vector2(num6, num7);
                    //    //int num167 = FinalFractalHelper.GetRandomProfileIndex();
                    //    //if (num166 == 0)
                    //    //{
                    //    //    num167 = 3065;
                    //    //}
                    //    //Vector2 value7 = Main.MouseWorld - player.MountedCenter;
                    //    //if (num166 == 1 || num166 == 2)
                    //    //{
                    //    //    int num168;
                    //    //    bool zenithTarget = this.GetZenithTarget(Main.MouseWorld, 400f, out num168);
                    //    //    if (zenithTarget)
                    //    //    {
                    //    //        value7 = Main.npc[num168].Center - player.MountedCenter;
                    //    //    }
                    //    //    bool flag8 = num166 == 2;
                    //    //    if (num166 == 1 && !zenithTarget)
                    //    //    {
                    //    //        flag8 = true;
                    //    //    }
                    //    //    if (flag8)
                    //    //    {
                    //    //        value7 += Main.rand.NextVector2Circular(150f, 150f);
                    //    //    }
                    //    //}
                    //    //velocity = value7 / 2f;
                    //    float ai5 = (float)Main.rand.Next(-100, 101);
                    //    Projectile.NewProjectile(player.Center, velocity, ProjectileType<PureFractalProj>(), projectile.damage, projectile.knockBack, player.whoAmI, ai5);
                    //}
                    //for (int n = 0; n < 5; n++)
                    //{
                    //    var player = Main.player[projectile.owner];
                    //    Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true, true);
                    //    float num6 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
                    //    float num7 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
                    //    Vector2 velocity = new Vector2(num6, num7);
                    //    float ai5 = (float)Main.rand.Next(-100, 101);
                    //    Projectile.NewProjectile(player.Center, (Main.MouseWorld - player.Center) / 2, ProjectileType<PureFractalProj>(), projectile.damage, projectile.knockBack, player.whoAmI, ai5);
                    //    //Projectile.NewProjectile(Main.MouseWorld, new Vector2(0, 0), ProjectileType<FinalFractalDimensionalSwoosh3>(), projectile.damage, projectile.knockBack, projectile.owner, Main.rand.NextFloat(-30f, 30f), Main.rand.NextFloat(0.8f, 1.2f));
                    //}
                    //for (int n = 0; n < 5; n++)
                    //{
                    //    Vector2 value5 = Main.MouseWorld;
                    //    List<NPC> list2;
                    //    bool sparkleGuitarTarget2 = GetTarget(out list2);
                    //    if (sparkleGuitarTarget2)
                    //    {
                    //        NPC npc2 = list2[Main.rand.Next(list2.Count)];
                    //        value5 = npc2.Center + npc2.velocity * 20f;
                    //    }
                    //    Vector2 vector32 = value5 - p.Center;
                    //    Vector2 vector33 = Main.rand.NextVector2CircularEdge(1f, 1f);
                    //    float num78 = 1f;
                    //    int num79 = 1;
                    //    for (int num80 = 0; num80 < num79; num80++)
                    //    {
                    //        if (!sparkleGuitarTarget2)
                    //        {
                    //            value5 += Main.rand.NextVector2Circular(24f, 24f);
                    //            if (vector32.Length() > 700f)
                    //            {
                    //                vector32 *= 700f / vector32.Length();
                    //                value5 = p.Center + vector32;
                    //            }
                    //            float num81 = Tools.GetLerpValue(0f, 6f, p.velocity.Length(), true) * 0.8f;
                    //            vector33 *= 1f - num81;
                    //            vector33 += p.velocity * num81;
                    //            vector33 = vector33.SafeNormalize(Vector2.UnitX);
                    //        }
                    //        float num82 = 60f;
                    //        float num83 = Main.rand.NextFloatDirection() * 3.14159274f * (1f / num82) * 0.5f * num78;
                    //        float num84 = num82 / 2f;
                    //        float scaleFactor3 = 12f + Main.rand.NextFloat() * 2f;
                    //        Vector2 vector34 = vector33 * scaleFactor3;
                    //        Vector2 vector35 = new Vector2(0f, 0f);
                    //        Vector2 vector36 = vector34;
                    //        int num85 = 0;
                    //        while ((float)num85 < num84)
                    //        {
                    //            vector35 += vector36;
                    //            vector36 = vector36.RotatedBy((double)num83, default(Vector2));
                    //            num85++;
                    //        }
                    //        Vector2 value6 = -vector35;
                    //        Vector2 position = value5 + value6;
                    //        float lerpValue2 = Main.rand.Next(0, 25);
                    //        Projectile.NewProjectile(position, vector34, ProjectileType<FinalFractalDimensionalSwoosh>(), projectile.damage, projectile.knockBack, projectile.owner, num83, lerpValue2);
                    //        //Projectile.NewProjectile(position + new Vector2(128, 112), vector34, ProjectileType<FinalFractalDimensionalSwoosh>(), projectile.damage, projectile.knockBack, projectile.owner, num83, lerpValue2);
                    //    }
                    //}
                    if ((int)(Player.itemAnimationMax / 3f * 2) >= 1)
                    {
                        illusionBoundPlayer.usingFinalFractal = (int)(Player.itemAnimationMax / 3f * 2);
                    }
                    else
                    {
                        illusionBoundPlayer.usingFinalFractal = 1;
                    }
                }
                else if (illusionBoundPlayer.finalFractalTier == 2)
                {
                    Vector2 vec = Main.MouseWorld - Player.Center;
                    vec.Normalize();
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), Player.Center, vec * 32, ProjectileType<Zenith_FirstFractal>(), projectile.damage, projectile.knockBack, Player.whoAmI);
                    illusionBoundPlayer.finalFractalTier = 0;
                    if ((int)(Player.itemAnimationMax / 3f * 2) >= 1)
                    {
                        illusionBoundPlayer.usingFinalFractal = (int)(Player.itemAnimationMax / 3f * 2);
                    }
                    else
                    {
                        illusionBoundPlayer.usingFinalFractal = 1;
                    }
                }
            }
            if (!illusionBoundPlayer.holdingFinalFractal)
            {
                projectile.Kill();
            }
            projectile.spriteDirection = Player.direction;
            projectile.direction = projectile.spriteDirection;
            projectile.Center = Player.Center;
            projectile.timeLeft = 2;
            Player.heldProj = projectile.whoAmI;
        }

        public override void SetDefaults()
        {
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.width = 192;
            projectile.height = 120;
            projectile.alpha = 255;
            projectile.aiStyle = -1;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.DamageType = DamageClass.Melee;
            projectile.penetrate = -1;
            projectile.light = 1f;

        }
        //public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        //{
        //    hitCoolDown = 5;
        //    target.immune[projectile.owner] = 0;
        //}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}
