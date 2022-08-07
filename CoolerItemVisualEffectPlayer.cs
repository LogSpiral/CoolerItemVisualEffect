using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ModLoader;
using static CoolerItemVisualEffect.ConfigurationSwoosh;

namespace CoolerItemVisualEffect
{
    public class CoolerItemVisualEffectPlayer : ModPlayer
    {
        public List<Vector2> itemOldPositions = new();
        public float kValue;
        public bool negativeDir;
        public float rotationForShadow;

        //public override void PreUpdate() {
        //    if (Player.itemAnimation == 1) {
        //        negativeDir ^= true;
        //        rotationForShadow = (Main.MouseWorld - Player.Center).ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
        //        kValue = Main.rand.NextFloat(1, 2);
        //    }
        //}
        public override void OnEnterWorld(Player player)
        {
            //if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            //    player.GetModPlayer<WeaponDisplayPlayer>().ConfigurationSwoosh.SendData();
            //testState = player.whoAmI;
            //testState++;
            //foreach (var p in Main.player) 
            //{
            //    if (p != null && p.active) p.GetModPlayer<WeaponDisplayPlayer>().testState++;
            //}
            //var flag = player.GetModPlayer<WeaponDisplayPlayer>().configurationSwoosh == null;
            //if (flag) testState = 1;
            //player.GetModPlayer<WeaponDisplayPlayer>().testState = 1;
            //if (Main.netMode == NetmodeID.MultiplayerClient) 
            //{
            //    player.GetModPlayer<WeaponDisplayPlayer>().ConfigurationSwoosh.SendData();
            //    ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
            //    packet.Write((byte)HandleNetwork.MessageType.EnterWorld);
            //    packet.Send(-1, -1);
            //}

        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            player.GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh.SendData(Player.whoAmI, fromWho, toWho, true);//(byte)

            //testState++;
            //Main.NewText("同步辣!!!!");
            //base.SyncPlayer(toWho, fromWho, newPlayer);
        }
        public float kValueNext;
        public float rotationForShadowNext;
        //public int testState;
        public int swingCount;
        public (Texture2D tex, int type) colorBar;

        public float direct;
        ConfigurationSwoosh configurationSwoosh;
        ////public ConfigurationSwoosh ConfigurationSwoosh
        ////{
        ////    get => configurationSwoosh ??= (Main.myPlayer == player.whoAmI ? instance : new ConfigurationSwoosh());//ConfigurationSwoosh.instance
        ////    set => configurationSwoosh = value;
        ////}
        public ConfigurationSwoosh ConfigurationSwoosh
        {
            get
            {
                if (configurationSwoosh == null)
                {
                    configurationSwoosh = Main.myPlayer == player.whoAmI ? instance : new ConfigurationSwoosh();
                }
                return configurationSwoosh;
            }
            set => configurationSwoosh = value;
        }
        Player player => Player;
        public float factorGeter
        {
            get
            {
                var factor = (float)player.itemAnimation / (player.itemAnimationMax - 1);//物品挥动程度的插值，这里应该是从1到0
                const float cValue = 3f;
                float fac;
                switch (instance.swooshActionStyle)
                {
                    default:
                    case SwooshAction.正常挥砍: fac = ((float)Math.Sqrt(factor) + factor) * .5f; break;
                    case SwooshAction.两次普通斩击一次高速旋转:
                    case SwooshAction.向后倾一定角度后重击: fac = 1 - (cValue - 1) * (1 - factor) * (1 - factor) - (2 - cValue) * (1 - factor); break;
                }
                return fac;
            }
        }
        public override void ModifyScreenPosition()
        {
            if (player.HeldItem.damage > 0 && player.HeldItem.useStyle == ItemUseStyleID.Swing && player.itemAnimation > 0 && player.HeldItem.DamageType == DamageClass.Melee && !player.HeldItem.noUseGraphic && instance.CoolerSwooshActive && !Main.gamePaused && (!instance.ToolsNoUseNewSwooshEffect || player.HeldItem.axe == 0 && player.HeldItem.hammer == 0 && player.HeldItem.pick == 0) || player.HeldItem.type == ItemID.Zenith && player.itemAnimation > 0 && instance.allowZenith)
            {
                Main.screenPosition += Main.rand.NextVector2Unit() * (float)Math.Pow(factorGeter, 2) * 16 * instance.Shake * (swingCount % 3 == 2 ? 3 : 1);
            }
            base.ModifyScreenPosition();
        }
        public override void PreUpdate()
        {
            //bool hasItem = false;
            //foreach (var item in player.inventory) 
            //{
            //    if (item.type == 2333) 
            //    {
            //        hasItem = true;
            //        break;
            //    }
            //}
        }
        public override void PostUpdate()
        {
            //base.PostUpdate();
            if (Player.HeldItem.type == ItemID.Zenith || Player.HeldItem.type == ModContent.ItemType<Weapons.FirstFractal_CIVE>() || Player.HeldItem.type == ModContent.ItemType<Weapons.PureFractal>())
            {
                if (ConfigurationSwoosh.instance.allowZenith && ConfigurationSwoosh.instance.CoolerSwooshActive)
                {
                    Player.HeldItem.noUseGraphic = false;
                    Player.HeldItem.useStyle = 1;
                    Player.HeldItem.channel = false;
                }
                else
                {
                    Player.HeldItem.noUseGraphic = true;
                    Player.HeldItem.useStyle = 5;
                    Player.HeldItem.channel = true;
                }

                //Main.NewText(player.HeldItem.noUseGraphic);
            }
            //Main.NewText(player.HeldItem.noUseGraphic);
            //Main.NewText((Player.itemAnimation, Player.itemAnimationMax), Color.Red);
            if (player.itemAnimation == player.itemAnimationMax && player.itemAnimation > 0)
            {
                var flag = player.HeldItem.damage > 0 && player.HeldItem.useStyle == ItemUseStyleID.Swing && player.HeldItem.DamageType == DamageClass.Melee && !player.HeldItem.noUseGraphic && ConfigurationSwoosh.instance.CoolerSwooshActive;
                flag |= (player.HeldItem.type == ItemID.Zenith || player.HeldItem.type == ModContent.ItemType<Weapons.FirstFractal_CIVE>() || player.HeldItem.type == ModContent.ItemType<Weapons.PureFractal>()) && ConfigurationSwoosh.instance.allowZenith && ConfigurationSwoosh.instance.CoolerSwooshActive;
                if (Main.myPlayer == player.whoAmI && flag) // 
                {
                    CoolerItemVisualEffect.ChangeShooshStyle(player);
                }
            }
            if (player.itemAnimation > 0 && UseSlash)
            {
                player.itemRotation = direct - MathHelper.ToRadians(90f); // 别问为啥-90°，问re去
                //Main.NewText("!!!!!");
                player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            }
        }
        public override void ResetEffects()
        {



            //Main.NewText((testState, Player.whoAmI, Player.name));
            //if (testState == 1)
            //{
            //    foreach (var p in Main.player)
            //    {
            //        if (p != null && p.active) p.GetModPlayer<WeaponDisplayPlayer>().testState = 2;
            //    }
            //}
            //if (testState == 2)
            //{
            //    //Main.NewText(testState);
            //    testState = 0;
            //    ConfigurationSwoosh.SendData();
            //}
            //if (Main.GameUpdateCount % 4 == 0 && player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            //    configurationSwoosh.SendData();
        }
        private static Vector2 DrawPlayer_Head_GetSpecialDrawPosition(ref PlayerDrawSet drawinfo, Vector2 helmetOffset, Vector2 hatOffset)
        {
            Vector2 value = Main.OffsetsPlayerHeadgear[drawinfo.drawPlayer.bodyFrame.Y / drawinfo.drawPlayer.bodyFrame.Height] * drawinfo.drawPlayer.Directions;
            Vector2 vector = drawinfo.Position - Main.screenPosition + helmetOffset + new Vector2((float)(-(float)drawinfo.drawPlayer.bodyFrame.Width / 2 + drawinfo.drawPlayer.width / 2), drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4) + hatOffset * drawinfo.drawPlayer.Directions + value;
            vector = vector.Floor();
            vector += drawinfo.drawPlayer.headPosition + drawinfo.headVect;
            if (drawinfo.drawPlayer.gravDir == -1f)
            {
                vector.Y += 12f;
            }
            vector = vector.Floor();
            return vector;
        }
        /*public override void UpdateLifeRegen()
        {
            Main.NewText($"{Player.direction}");
            base.UpdateLifeRegen();
        }*/
        //public bool Display = false,FirstWeaponDisplay = false;
        //public override void ModifyDrawLayerOrdering(IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions)
        //{
        //    positions.
        //}

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {

            //foreach (var projectile in Main.projectile) 
            //{
            //    if (projectile.type == ProjectileID.FinalFractal && projectile.active) 
            //    {
            //        for (int n = 0; n < projectile.oldPos.Length; n++)
            //        {
            //            float factor = 1 - n / (projectile.oldPos.Length - 1f);
            //            factor *= .5f;
            //            Main.spriteBatch.DrawLine(projectile.oldPos[n], (projectile.oldRot[n] - MathHelper.PiOver2).ToRotationVector2() * 64 , Color.Cyan * factor, 4, true, -Main.screenPosition);
            //            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, projectile.oldPos[n] - Main.screenPosition, new Rectangle(0, 0, 1, 1), Color.Red * factor, 0, new Vector2(.5f), 8f, 0, 0);
            //        }
            //    }
            //}

            HitboxPosition = Vector2.Zero;//重置
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, player.isFirstFractalAfterImage.ToString(), Player.Center - new Vector2(0, 64) - Main.screenPosition, Color.Red);
            //这个写法可以让绘制的东西在人物旋转后保持原来与人物的相对位置(试做的武器显示)
            if (ConfigurationPreInstall.instance.useWeaponDisplay)
            {
                if (Main.gameMenu && ConfigurationPreInstall.instance.firstWeaponDisplay)//
                {
                    Item firstweapon = null;
                    //Main.NewText(WeaponDisplay.Instance._FirstInventoryItem == null);
                    for (int num2 = 0; num2 <= 58; num2++)
                    {
                        Item weapon = Player.inventory[num2];//num2 == 0 ? WeaponDisplay.Instance._FirstInventoryItem : 
                        if (weapon == null) continue;
                        if (weapon.stack > 0 && (weapon.damage > 0 || weapon.type == 905) && weapon.useAnimation > 0 && weapon.useTime > 0 && !weapon.consumable && weapon.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(weapon, Player) && weapon.holdStyle == 0 && weapon.type != ItemID.FlareGun && weapon.type != ItemID.MagicalHarp && weapon.type != ItemID.NebulaBlaze && weapon.type != ItemID.NebulaArcanum && weapon.type != ItemID.TragicUmbrella && weapon.type != ItemID.CombatWrench && weapon.type != ItemID.FairyQueenMagicItem && weapon.type != ItemID.BouncingShield && weapon.type != ItemID.SparkleGuitar)
                        {
                            firstweapon = weapon;
                            break;
                        }
                    }
                    if (firstweapon != null)
                    {
                        DrawWeapon(Player, firstweapon, drawInfo);
                    }
                    //for (int n = 0; n < player.inventory.Length; n++)
                    //{
                    //    Item item = player.inventory[n];//n == 0 ? WeaponDisplay.Instance._FirstInventoryItem : 
                    //    if (item != null)
                    //    {
                    //        Main.spriteBatch.End();
                    //        Main.spriteBatch.Begin();
                    //        Vector2 vec = new Vector2(360 + 128 * (n % 10), 460 + 64 * (n / 10));
                    //        Main.spriteBatch.Draw(TextureAssets.Item[item.type].Value, vec,
                    //            null, Color.White, 0,
                    //            TextureAssets.Item[item.type].Size() * .5f, 1f, 0, 0);//n * MathHelper.TwoPi / player.inventory.Length
                    //        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, (n, item.type).ToString(), vec + new Vector2(0, 16), Color.White);
                    //        Main.spriteBatch.End();
                    //        Main.spriteBatch.Begin();
                    //    }
                    //}
                    //int n = (int)(Main.GlobalTimeWrappedHourly / 15 % player.inventory.Length);
                    //if (player.inventory[n] != null)
                    //{
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin();
                    //    Main.spriteBatch.Draw(TextureAssets.Item[player.inventory[n].type].Value, new Vector2(960, 560),
                    //        null, Color.White, n * MathHelper.TwoPi / player.inventory.Length,
                    //        TextureAssets.Item[player.inventory[n].type].Size() * .5f, 1f, 0, 0);
                    //    //Main.spriteBatch.DrawString(FontAssets.MouseText.Value, Main.GlobalTimeWrappedHourly.ToString(), new Vector2(960, 560), Color.White);
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin();
                    //    drawInfo.DrawDataCache.Add(new DrawData(TextureAssets.Item[player.inventory[n].type].Value, drawInfo.Center, null, Color.White, n * MathHelper.TwoPi / player.inventory.Length, TextureAssets.Item[player.inventory[n].type].Size() * .5f, 1f, 0, 0));
                    //}
                }
                Item holditem = Player.inventory[Player.selectedItem];
                if (Player.active && !Player.dead && holditem.stack > 0 && (holditem.damage > 0 || holditem.type == 905) && holditem.useAnimation > 0 && holditem.useTime > 0 && !holditem.consumable && holditem.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(holditem, Player))
                {
                    if (holditem.holdStyle == 0 && holditem.type != ItemID.FlareGun && holditem.type != ItemID.MagicalHarp && holditem.type != ItemID.NebulaBlaze && holditem.type != ItemID.NebulaArcanum && holditem.type != ItemID.TragicUmbrella && holditem.type != ItemID.CombatWrench && holditem.type != ItemID.FairyQueenMagicItem && holditem.type != ItemID.BouncingShield && holditem.type != ItemID.SparkleGuitar)
                    {
                        DrawWeapon(Player, holditem, drawInfo);
                    }
                }
            }

            //drawInfo.DrawDataCache.Add(new DrawData(TextureAssets.Item[ItemID.Zenith].Value, player.Center + new Vector2(-21, 65) - Main.screenPosition, new Rectangle(0,22,34,34), Color.White, MathHelper.PiOver4, TextureAssets.Item[ItemID.Zenith].Size(), 1f, 0, 0));
            //drawInfo.DrawDataCache.Insert(0, new DrawData(TextureAssets.MagicPixel.Value, player.Center + new Vector2(3, -7) - Main.screenPosition, new Rectangle(0, 0, 1, 1), Color.Red, 0, new Vector2(0.5f), new Vector2(4, 2) * 10, 0, 0));//
            base.ModifyDrawInfo(ref drawInfo);
        }

        public static void DrawWeapon(Player Player, Item holditem, PlayerDrawSet drawInfo)
        {
            Texture2D texture = TextureAssets.Item[holditem.type].Value;
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = rectangle.Size() / 2f;
            Vector2 value5 = DrawPlayer_Head_GetSpecialDrawPosition(ref drawInfo, Vector2.Zero, new Vector2(0f, 8f));
            float rot = MathHelper.Pi;
            if (holditem.DamageType == DamageClass.Ranged || (holditem.axe > 0 || holditem.pick > 0) && holditem.channel ||
                holditem.useStyle == ItemUseStyleID.Shoot && !Item.staff[holditem.type] && holditem.DamageType == DamageClass.Magic
                || holditem.type == ItemID.KOCannon || holditem.type == ItemID.GolemFist)
            {
                if (Player.gravDir == -1f)
                {
                    rot += MathHelper.PiOver4;
                    if (Player.direction < 0) rot -= MathHelper.PiOver2;
                }
                else
                {
                    rot -= MathHelper.PiOver4;
                    if (Player.direction < 0) rot += MathHelper.PiOver2;
                }
            }
            if (Main.itemAnimations[holditem.type] != null)
            {//动态武器
                rectangle = Main.itemAnimations[holditem.type].GetFrame(texture, -1);
            }
            DrawData item = new DrawData(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, ConfigurationPreInstall.instance.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
            //switch (CoolerItemVisualEffect.Config.DyeUsed)
            //{
            //    case DyeSlot.None:
            //        item.shader = 0;
            //        break;
            //    case DyeSlot.Head:
            //        item.shader = Player.dye[0].dye;
            //        break;
            //    case DyeSlot.Body:
            //        item.shader = Player.dye[1].dye;
            //        break;
            //    case DyeSlot.Leg:
            //        item.shader = Player.dye[2].dye;
            //        break;
            //    default:
            //        break;
            //}
            drawInfo.DrawDataCache.Add(item);
            if (holditem.glowMask >= 0)
            {
                Texture2D glow = TextureAssets.GlowMask[holditem.glowMask].Value;
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationPreInstall.instance.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
                //switch (CoolerItemVisualEffect.Config.DyeUsed)
                //{
                //    case DyeSlot.None:
                //        itemglow.shader = 0;
                //        break;
                //    case DyeSlot.Head:
                //        item.shader = Player.dye[0].dye;
                //        break;
                //    case DyeSlot.Body:
                //        item.shader = Player.dye[1].dye;
                //        break;
                //    case DyeSlot.Leg:
                //        item.shader = Player.dye[2].dye;
                //        break;
                //    default:
                //        break;
                //}
                drawInfo.DrawDataCache.Add(itemglow);
            }
            if (holditem.ModItem != null && ModContent.HasAsset(holditem.ModItem.Texture + "_Glow"))
            {
                Texture2D glow = ModContent.Request<Texture2D>(holditem.ModItem.Texture + "_Glow").Value;
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationPreInstall.instance.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
                drawInfo.DrawDataCache.Add(itemglow);
            }
        }
        private static bool CheckItemCanUse(Item sItem, Player player)
        {
            bool flag = true;
            if (sItem.shoot == ProjectileID.EnchantedBoomerang || sItem.shoot == ProjectileID.Flamarang || sItem.shoot == ProjectileID.ThornChakram || sItem.shoot == ProjectileID.WoodenBoomerang || sItem.shoot == ProjectileID.IceBoomerang || sItem.shoot == ProjectileID.BloodyMachete || sItem.shoot == ProjectileID.FruitcakeChakram || sItem.shoot == ProjectileID.Anchor || sItem.shoot == ProjectileID.FlyingKnife || sItem.shoot == ProjectileID.Shroomerang || sItem.shoot == ProjectileID.CombatWrench || sItem.shoot == ProjectileID.BouncingShield)
            {
                if (player.ownedProjectileCounts[sItem.shoot] > 0)
                {
                    flag = false;
                }
            }
            if (sItem.shoot == ProjectileID.LightDisc || sItem.shoot == ProjectileID.Bananarang)
            {
                if (player.ownedProjectileCounts[sItem.shoot] >= sItem.stack)
                {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// 刀光碰撞箱相对玩家的坐标，为了适配联机把原来写的改了一下
        /// </summary>
        public Vector2 HitboxPosition = Vector2.Zero;
        /// <summary>
        /// 该玩家是否使用斩击特效，为了联机同步写的
        /// </summary>
        public bool UseSlash;

        /*public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            Texture2D texture2D = TextureAssets.MagicPixel.Value;
            Color color = Color.Blue;
            Main.spriteBatch.Draw(texture2D, pos - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(10f, 10f), SpriteEffects.None, 0f);
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }*/
    }
}