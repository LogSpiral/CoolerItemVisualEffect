using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Localization;
using static CoolerItemVisualEffect.ConfigurationCIVE;
using Terraria.GameContent.Drawing;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures;
using Terraria.ModLoader;
using System.Linq;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Melee;
using NetSimplified;
using NetSimplified.Syncing;
using Terraria.GameInput;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using System.Reflection;

namespace CoolerItemVisualEffect
{
    public class MeleeModifierProj : GlobalProjectile
    {
        public static int[] vanillaSlashProjectiles = [ProjectileID.NightsEdge, ProjectileID.Excalibur, ProjectileID.TrueExcalibur, ProjectileID.TheHorsemansBlade, ProjectileID.TerraBlade2];
        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (mplr.ConfigurationSwoosh.SwordModifyActive && mplr.IsMeleeBroadSword && vanillaSlashProjectiles.Contains(projectile.type))
                projectile.Kill();
            base.AI(projectile);
        }
    }
    public class MeleeModifierItem : GlobalItem
    {
        public static int[] vanillaSlashItems = [ItemID.NightsEdge, ItemID.TrueNightsEdge, ItemID.TheHorsemansBlade, ItemID.Excalibur, ItemID.TrueExcalibur, ItemID.TerraBlade];
        public override bool AltFunctionUse(Item item, Player player)
        {
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (mplr.ConfigurationSwoosh.SwordModifyActive && mplr.IsMeleeBroadSword)
                return true;
            return base.AltFunctionUse(item, player);
        }
        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (player.itemAnimation == player.itemAnimationMax && player.ownedProjectileCounts[ModContent.ProjectileType<CIVESword>()] == 0 && mplr.ConfigurationSwoosh.SwordModifyActive && mplr.IsMeleeBroadSword)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, default, ModContent.ProjectileType<CIVESword>(), player.HeldItem.damage, player.HeldItem.knockBack, player.whoAmI);
            }
            base.UseStyle(item, player, heldItemFrame);
        }
        //public override bool CanShoot(Item item, Player player)
        //{
        //    var mplr = player.GetModPlayer<MeleeModifyPlayer>();
        //    if (ConfigCIVEInstance.SwordModifyActive && mplr.IsMeleeBroadSword && vanillaSlashItems.Contains(item.type))
        //        return false;
        //    return base.CanShoot(item, player);
        //}
        public override bool? CanHitNPC(Item item, Player player, NPC target)
        {
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (mplr.ConfigurationSwoosh.SwordModifyActive && mplr.IsMeleeBroadSword)
                return false;
            return base.CanHitNPC(item, player, target);
        }
        public override bool CanHitPvp(Item item, Player player, Player target)
        {
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (mplr.ConfigurationSwoosh.SwordModifyActive && mplr.IsMeleeBroadSword)
                return false;
            return base.CanHitPvp(item, player, target);
        }
    }
    public class MeleeModifyPlayer : ModPlayer
    {


        #region 基本量声明
        ConfigurationCIVE configurationSwoosh;
        public ConfigurationCIVE ConfigurationSwoosh
        {
            get
            {
                if (configurationSwoosh == null)
                {
                    configurationSwoosh = Main.myPlayer == player.whoAmI ? ConfigCIVEInstance : new ConfigurationCIVE();
                }
                return configurationSwoosh;
            }
            set => configurationSwoosh = value;
        }
        Player player => Player;
        public static bool MeleeBroadSwordCheck(Item item)
        {
            bool flag = MeleeCheck(item.DamageType);
            flag &= item.damage > 0;
            flag &= !item.noUseGraphic;
            flag &= (!item.noMelee) || MeleeModifierItem.vanillaSlashItems.Contains(item.type);
            flag &= item.useStyle == ItemUseStyleID.Swing;
            flag &= item.pick == 0;
            flag &= item.axe == 0;
            flag &= item.hammer == 0;
            flag &= item.type != ItemID.GravediggerShovel;
            return flag;
        }
        public bool IsMeleeBroadSword => MeleeBroadSwordCheck(player.HeldItem);
        public bool UseSwordModify => ConfigurationSwoosh.SwordModifyActive && IsMeleeBroadSword && player.itemAnimation > 0;
        #endregion

        #region 视觉效果相关
        public Texture2D heatMap;
        public Color mainColor;
        public float hollowCheckScaler;
        public int lastWeaponHash;
        public Vector3 hsl;
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (UseSwordModify)
                drawInfo.heldItem = new Item();
            //drawInfo.weaponDrawOrder = WeaponDrawOrder.BehindBackArm;
            base.HideDrawLayers(drawInfo);
        }

        #endregion

        public static ModKeybind ModifyActiveKeybind { get; private set; }

        public override void Load()
        {
            On_Player.ItemCheck_EmitUseVisuals += On_Player_ItemCheck_EmitUseVisuals_CIVEMelee;
            IL_Player.ItemCheck_OwnerOnlyCode += ProjectileShootBan;
            ModifyActiveKeybind = KeybindLoader.RegisterKeybind(Mod, "ModifyActive", "I");



            base.Load();
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ModifyActiveKeybind.JustReleased)
            {
                bool active = configurationSwoosh.SwordModifyActive ^= true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    SyncMeleeModifyActive.Get(player.whoAmI, active).Send(-1, player.whoAmI);
                for (int n = 0; n < 32; n++)
                    Dust.NewDustPerfect(player.Center, Main.rand.Next([DustID.FireworkFountain_Blue]), Main.rand.NextVector2Unit() * 32).noGravity = true;
                //, DustID.FireworkFountain_Green, DustID.FireworkFountain_Pink, DustID.FireworkFountain_Red, DustID.FireworkFountain_Yellow
                Main.NewText(Language.GetOrRegister($"Mods.CoolerItemVisualEffect.Misc.MeleeModify{(active ? "Active" : "Deactive")}"));
                if (Main.myPlayer == player.whoAmI)
                    ConfigManager.Save(configurationSwoosh);
            }
            base.ProcessTriggers(triggersSet);
        }

        private void ProjectileShootBan(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchCall<Player>("ItemCheck_Shoot")))//找到ItemCheck_Shoot的位置
                return;
            if (!c.TryGotoPrev(i => i.MatchAnd()))//找到它前面最近的一次取与
                return;
            c.Index++;

            c.EmitLdarg0();
            c.EmitDelegate<Func<Player, bool>>
                (
                player =>
                {
                    var mplr = player.GetModPlayer<MeleeModifyPlayer>();
                    return !(mplr.ConfigurationSwoosh.SwordModifyActive && mplr.IsMeleeBroadSword);
                })
                ;
            c.EmitAnd();


        }

        private Rectangle On_Player_ItemCheck_EmitUseVisuals_CIVEMelee(On_Player.orig_ItemCheck_EmitUseVisuals orig, Player self, Item sItem, Rectangle itemRectangle)
        {
            if (self.ownedProjectileCounts[ModContent.ProjectileType<CIVESword>()] < 1)
                orig(self, sItem, itemRectangle);
            return itemRectangle;
        }

        public override void Unload()
        {
            On_Player.ItemCheck_EmitUseVisuals -= On_Player_ItemCheck_EmitUseVisuals_CIVEMelee;
            IL_Player.ItemCheck_OwnerOnlyCode -= ProjectileShootBan;
            base.Unload();
        }
        #region 更新状态

        public override void PostUpdate()
        {
            if (IsMeleeBroadSword)
                ItemID.Sets.SkipsInitialUseSound[player.HeldItem.type] = ConfigurationSwoosh.SwordModifyActive;
            CheckItemChange(player);

            //Main.NewText((player.itemAnimation, player.itemAnimationMax, player.itemTime, player.itemTimeMax),Color.Red);


            base.PostUpdate();
        }
        /// <summary>
        /// 支持粘武器用的函数
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Texture2D GetWeaponTextureFromItem_Nullable(Item item)
        {
            var moditem = item.ModItem;
            if (moditem != null && ModContent.TryFind<ModItem>("StickyWeapons", "StickyItem", out var sticky) && sticky.GetType().Equals(moditem.GetType()))
            {
                try
                {
                    dynamic dynamicItem = moditem;
                    return dynamicItem.complexTexture;
                }
                catch (Exception e)
                {
                    Main.NewText(e.Message);
                }
            }
            else
            {
            }
            return null;
        }
        public static List<(Func<Item, Texture2D> func, float priority)> weaponGetFunctions = new();
        public static bool RefreshWeaponTexFunc;
        /// <summary>
        /// 如果你的武器贴图和"看上去"的不一样，用这个换成看上去的
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Texture2D GetWeaponTextureFromItem(Item item)
        {
            if (RefreshWeaponTexFunc)
            {
                RefreshWeaponTexFunc = false;
                weaponGetFunctions.Sort((v1, v2) => v1.priority > v2.priority ? 1 : -1);
            }
            Texture2D texture = null;
            foreach (var func in weaponGetFunctions)
            {
                if (func.func == null) continue;
                if (texture != null) break;
                texture = func.func.Invoke(item);
            }
            return texture ?? TextureAssets.Item[item.type].Value;
        }
        #endregion
        #region 辅助函数
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            SyncConfigCIVE.Get(player.whoAmI, ConfigurationSwoosh).Send(toWho, fromWho);
            base.SyncPlayer(toWho, fromWho, newPlayer);
        }
        public static bool SwordCheck(Item item)
        {
            return item.useStyle == ItemUseStyleID.Swing && item.damage > 0 && !item.noUseGraphic && MeleeCheck(item.DamageType);
        }
        public static bool MeleeCheck(DamageClass damageClass) => damageClass == DamageClass.Melee || damageClass.GetEffectInheritance(DamageClass.Melee) || !damageClass.GetModifierInheritance(DamageClass.Melee).Equals(StatInheritanceData.None);
        private static float GetHeatMapFactor(float t, int colorCount, HeatMapFactorStyle style) => style switch
        {
            HeatMapFactorStyle.Linear => t,
            HeatMapFactorStyle.Floor => (int)(t * (colorCount + 1)) / (float)colorCount,
            HeatMapFactorStyle.Quadratic => t * t,
            HeatMapFactorStyle.SquareRoot => MathF.Sqrt(t),
            HeatMapFactorStyle.SmoothFloor => (t * colorCount).SmoothFloor() / colorCount,
            _ => t
        };
        public static void UpdateHeatMap(ref Texture2D texture, Vector3 hsl, ConfigurationCIVE config, Texture2D itemTexture)
        {
            var colors = new Color[300];
            ref Vector3 _color = ref hsl;
            switch (config.heatMapCreateStyle)
            {
                case HeatMapCreateStyle.FromTexturePixel:
                    {
                        var w = itemTexture.Width;
                        var h = itemTexture.Height;
                        var cs = new Color[w * h];
                        itemTexture.GetData(cs);
                        var currentColor = new Color[5];
                        var infos = new (float? distance, int? index)[w * h];
                        for (int n = 0; n < w * h; n++)
                        {
                            var color = cs[n];
                            if (color != default)
                            {
                                infos[n] = (hsl.DistanceColor(Main.rgbToHsl(color)), n);//Main.hslToRgb(hsl).DistanceColor(color,0)
                            }
                        }
                        var (distanceMin, distanceMax, indexMin, indexMax) = (114514f, 0f, 0, 0);
                        foreach (var info in infos)
                        {
                            if (info.distance != null)
                            {
                                if (info.distance < distanceMin)
                                {
                                    distanceMin = info.distance.Value;
                                    indexMin = info.index.Value;
                                }
                                if (info.distance > distanceMax)
                                {
                                    distanceMax = info.distance.Value;
                                    indexMax = info.index.Value;
                                }
                            }
                        }
                        currentColor[4] = cs[indexMin];
                        currentColor[0] = cs[indexMax];

                        var _dis = new float[] { 114514, 114514, 114514 };
                        var _target = new float[] { distanceMax * .75f + distanceMin * .25f, distanceMax * .5f + distanceMin * .5f, distanceMax * .25f + distanceMin * .75f };
                        var _index = new int[] { -1, -1, -1 };
                        foreach (var info in infos)
                        {
                            if (info.distance != null)
                            {
                                for (int n = 0; n < 3; n++)
                                {
                                    var d = Math.Abs(info.distance.Value - _target[n]);
                                    if (d < _dis[n])
                                    {
                                        _dis[n] = d;
                                        _index[n] = info.index.Value;
                                    }
                                }
                            }
                        }
                        for (int n = 0; n < 3; n++)
                        {
                            currentColor[n + 1] = cs[_index[n]];
                        }
                        switch (config.heatMapFactorStyle)
                        {
                            case HeatMapFactorStyle.Linear:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        colors[n] = (n / 299f).ArrayLerp(currentColor);
                                    }
                                    break;
                                }
                            case HeatMapFactorStyle.Floor:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        colors[n] = currentColor[n / 60];
                                    }
                                    break;
                                }
                            case HeatMapFactorStyle.Quadratic:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        var fac = n / 299f;
                                        fac *= fac;
                                        colors[n] = fac.ArrayLerp(currentColor);
                                    }
                                    break;
                                }
                            case HeatMapFactorStyle.SquareRoot:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        colors[n] = MathF.Sqrt(n / 299f).ArrayLerp(currentColor);
                                    }
                                    break;
                                }
                            case HeatMapFactorStyle.SmoothFloor:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        colors[n] = ((n / 299f * 5).SmoothFloor() / 5f).ArrayLerp(currentColor);
                                    }
                                    break;
                                }
                        }


                        break;
                    }
                case HeatMapCreateStyle.Designate:
                    {
                        var list = config.heatMapColors;
                        int count = list.Count;
                        switch (count)
                        {
                            case 0:
                                {
                                    Main.NewText("更新热度图失败！请确保至少有一个颜色。", Color.DarkRed);
                                    Main.NewText("Failed To Update The Heat Map! Please Ensure There's At Least One Color.", Color.DarkRed);
                                    return;
                                }
                            case 1:
                                {
                                    var color = list[0];
                                    for (int i = 0; i < 300; i++)
                                    {
                                        colors[i] = color;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    var color1 = list[0];
                                    var color2 = list[1];
                                    for (int i = 0; i < 300; i++)
                                    {
                                        colors[i] = Color.Lerp(color1, color2, GetHeatMapFactor(i / 299f, 2, config.heatMapFactorStyle));
                                    }
                                    break;
                                }
                            default:
                                {
                                    var array = list.ToArray();
                                    for (int i = 0; i < 300; i++)
                                    {
                                        colors[i] = GetHeatMapFactor(i / 299f, list.Count, config.heatMapFactorStyle).ArrayLerp(array);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case HeatMapCreateStyle.ByFunction:
                default:
                    {
                        for (int i = 0; i < 300; i++)
                        {
                            var f = GetHeatMapFactor(i / 299f, 6, config.heatMapFactorStyle);//分割成25次惹，f从1/25f到1//1 - 
                                                                                             //f = f * f;// *f
                            float h = (hsl.X + config.hueOffsetValue + config.hueOffsetRange * (2 * f - 1)) % 1;
                            float s = MathHelper.Clamp(hsl.Y * config.saturationScalar, 0, 1);
                            float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + config.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - config.luminosityRange), 0, 1);
                            while (h < 0) h++;
                            var currentColor = Main.hslToRgb(h, s, l);
                            colors[i] = currentColor;
                        }
                        break;
                    }
            }
            if (texture == null)
            {
                try
                {
                    texture = new Texture2D(Main.graphics.GraphicsDevice, 300, 1);
                }
                catch
                {
                    Texture2D texdummy = null;
                    Main.RunOnMainThread(() => { texdummy = new Texture2D(Main.graphics.GraphicsDevice, 300, 1); });
                    texture = texdummy;
                }
            }
            texture.SetData(colors);
        }
        //public static void WhenConfigSwooshChange()
        //{
        //    if (Main.player != null)
        //        foreach (var player in Main.player)
        //        {
        //            if (player.active)
        //            {
        //                var modPlr = player.GetModPlayer<MeleeModifyPlayer>();
        //                UpdateHeatMap(ref modPlr.heatMap, modPlr.hsl, modPlr.ConfigurationSwoosh, TextureAssets.Item[player.HeldItem.type].Value);
        //            }
        //        }
        //}
        public static void CheckItemChange(Player player, bool airCheck = false)
        {
            if (Main.netMode == NetmodeID.Server) return;
            MeleeModifyPlayer modPlayer = player.GetModPlayer<MeleeModifyPlayer>();
            if (!TextureAssets.Item[player.HeldItem.type].IsLoaded) Main.instance.LoadItem(player.HeldItem.type);//TextureAssets.Item[player.HeldItem.type] = Main.Assets.Request<Texture2D>("Images/Item_" + player.HeldItem.type, ReLogic.Content.AssetRequestMode.AsyncLoad);
            var texture = GetWeaponTextureFromItem(player.HeldItem);
            if (modPlayer.lastWeaponHash != player.HeldItem.GetHashCode())
            {
                if (!modPlayer.IsMeleeBroadSword)
                {
                    foreach (var proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<CIVESword>() && proj.owner == player.whoAmI)
                        {
                            proj.Kill();
                            return;
                        }
                    }
                }
                modPlayer.lastWeaponHash = player.HeldItem.GetHashCode();
                var width = texture.Width;
                var height = texture.Height;
                var pixelColor = new Color[width * height];

                texture.GetData(pixelColor);
                Vector4 vcolor = default;
                float count = 0;
                modPlayer.hollowCheckScaler = 1;
                Color target = default;
                for (int n = 0; n < pixelColor.Length; n++)
                {
                    if (pixelColor[n] != default && (n - width < 0 || pixelColor[n - width] != default) && (n - 1 < 0 || pixelColor[n - 1] != default) &&
                        (n + width >= pixelColor.Length || pixelColor[n + width] != default) && (n + 1 >= pixelColor.Length || pixelColor[n + 1] != default))
                    {
                        var weight = (float)((n + 1) % width * (height - n / width)) / width / height;
                        vcolor += pixelColor[n].ToVector4() * weight;
                        count += weight;
                    }
                    Vector2 coord = new Vector2(n % width, n / width);
                    coord /= new Vector2(width, height);
                    if (airCheck)
                        if (Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && pixelColor[n] != default && target == default)
                        {
                            target = pixelColor[n];
                            modPlayer.hollowCheckScaler = coord.X;
                        }
                }
                vcolor /= count;
                var newColor = modPlayer.mainColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
                modPlayer.hsl = Main.rgbToHsl(newColor);
                UpdateHeatMap(ref modPlayer.heatMap, modPlayer.hsl, modPlayer.ConfigurationSwoosh, texture);
            }
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (UseSwordModify)
                drawInfo.heldItem = new Item();
            if (ConfigurationSwoosh.TeleprotEffectActive && player.HeldItem.type == ItemID.MagicMirror && player.ItemAnimationActive)
            {
                var fac = player.itemAnimation / (float)player.itemAnimationMax;
                var _fac = (fac * 2 % 1).HillFactor2() * (fac < .5f ? .5f : 1f);
                var yscaler = (.25f + fac * (fac - 1)) * 1.5f;
                Main.spriteBatch.Draw(LogSpiralLibraryMod.MagicZone[2].Value, player.Center + new Vector2(0, -128 + 256 * fac) - Main.screenPosition, null, Color.Cyan with { A = 0 } * _fac, 0, new Vector2(150), new Vector2(1, yscaler) * _fac, 0, 0);

                Main.spriteBatch.Draw(LogSpiralLibraryMod.MagicZone[2].Value, player.Center + new Vector2(0, 128 - 256 * fac) - Main.screenPosition, null, Color.Cyan with { A = 0 } * _fac, 0, new Vector2(150), new Vector2(1, yscaler) * _fac, 0, 0);
            }
            //if (ConfigurationSwoosh.showHeatMap && heatMap != null && !Main.gameMenu && !drawInfo.headOnlyRender)
            //{
            //    //Vector2 drawPos = player.whoAmI == Main.myPlayer ? new Vector2(600,400) : (player.Center - Main.screenPosition - Vector2.UnitY * 64);
            //    Vector2 drawPos = player.Center - Main.screenPosition - Vector2.UnitY * 64;
            //    Main.spriteBatch.Draw(heatMap, drawPos, null, Color.White, 0, new Vector2(150, .5f), new Vector2(1, 50f), SpriteEffects.None, 0);
            //}
            if (ConfigurationSwoosh.useWeaponDisplay && !drawInfo.headOnlyRender)
            {
                if (Main.gameMenu && ConfigurationSwoosh.firstWeaponDisplay)//
                {
                    Item firstweapon = null;
                    for (int num2 = 0; num2 <= 58; num2++)
                    {
                        Item weapon = Player.inventory[num2];
                        if (weapon == null) continue;
                        if (weapon.stack > 0 && (weapon.damage > 0 || weapon.type == ItemID.CoinGun) && weapon.useAnimation > 0 && weapon.useTime > 0 && !weapon.consumable && weapon.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(weapon, Player) && weapon.holdStyle == 0 && weapon.type != ItemID.FlareGun && weapon.type != ItemID.MagicalHarp && weapon.type != ItemID.NebulaBlaze && weapon.type != ItemID.NebulaArcanum && weapon.type != ItemID.TragicUmbrella && weapon.type != ItemID.CombatWrench && weapon.type != ItemID.FairyQueenMagicItem && weapon.type != ItemID.BouncingShield && weapon.type != ItemID.SparkleGuitar)
                        {
                            firstweapon = weapon;
                            break;
                        }
                    }
                    if (firstweapon != null)
                    {
                        Main.instance.LoadItem(firstweapon.type);
                        DrawWeapon(Player, firstweapon, drawInfo);
                    }
                }
                Item holditem = Player.inventory[Player.selectedItem];
                if (Player.active && !Player.dead && holditem.stack > 0 && (holditem.damage > 0 || holditem.type == ItemID.CoinGun) && holditem.useAnimation > 0 && holditem.useTime > 0 && !holditem.consumable && holditem.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(holditem, Player))
                {
                    if (holditem.holdStyle == 0 && holditem.type != ItemID.FlareGun && holditem.type != ItemID.MagicalHarp && holditem.type != ItemID.NebulaBlaze && holditem.type != ItemID.NebulaArcanum && holditem.type != ItemID.TragicUmbrella && holditem.type != ItemID.CombatWrench && holditem.type != ItemID.FairyQueenMagicItem && holditem.type != ItemID.BouncingShield && holditem.type != ItemID.SparkleGuitar)
                    {
                        DrawWeapon(Player, holditem, drawInfo);
                    }
                }
            }

            base.ModifyDrawInfo(ref drawInfo);
        }
        public void DrawWeapon(Player Player, Item holditem, PlayerDrawSet drawInfo)
        {
            Texture2D texture = GetWeaponTextureFromItem(holditem);
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

            var animation = Main.itemAnimations[holditem.type];
            if (animation != null)
            {//动态武器
                rectangle = animation.GetFrame(texture, -1);
                origin = animation.GetFrame(texture).Size() * .5f;
            }
            DrawData item = new DrawData(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, ConfigurationSwoosh.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
            drawInfo.DrawDataCache.Add(item);
            if (holditem.glowMask >= 0)
            {
                Texture2D glow = TextureAssets.GlowMask[holditem.glowMask].Value;
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationSwoosh.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
                drawInfo.DrawDataCache.Add(itemglow);
            }
            if (holditem.ModItem != null && ModContent.HasAsset(holditem.ModItem.Texture + "_Glow"))
            {
                Texture2D glow = ModContent.Request<Texture2D>(holditem.ModItem.Texture + "_Glow").Value;
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationSwoosh.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
                drawInfo.DrawDataCache.Add(itemglow);
            }
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
        #endregion
    }
    public class CIVESword : MeleeSequenceProj
    {
        public override bool? CanHitNPC(NPC target)
        {
            if (target.isLikeATownNPC && player.HeldItem.type == ItemID.Flymeal) return true;
            return base.CanHitNPC(target);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (UseSwordModify)
                return base.Colliding(projHitbox, targetHitbox);
            return false;
        }
        public bool UseSwordModify => player.GetModPlayer<MeleeModifyPlayer>().UseSwordModify || (meleeSequence.currentData != null && ((meleeSequence.currentData.counter < meleeSequence.currentData.Cycle || (meleeSequence.currentData.counter == meleeSequence.currentData.Cycle && meleeSequence.currentData.timer > 0)) && !meleeSequence.currentWrapper.finished));
        public override string Texture => $"Terraria/Images/Item_{ItemID.TerraBlade}";
        public ConfigurationCIVE ConfigurationSwoosh => player.GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh;
        public override StandardInfo StandardInfo
        {
            get
            {
                var plr = player;
                var item = plr.HeldItem;
                if (item.flame && !TextureAssets.ItemFlame[item.type].IsLoaded)
                {
                    Main.instance.LoadItemFlames(item.type);
                }
                if (item.glowMask != -1 && !TextureAssets.GlowMask[item.glowMask].IsLoaded)
                {
                    Main.Assets.Request<Texture2D>(TextureAssets.GlowMask[item.glowMask].Name);
                }
                var rectangle = Main.itemAnimations[item.type]?.GetFrame(TextureAssets.Item[item.type].Value);
                var result = base.StandardInfo with
                {
                    standardColor = plr.GetModPlayer<MeleeModifyPlayer>().mainColor,// * .25f
                                                                                    //standardGlowTexture = ModContent.Request<Texture2D>(GlowTexture).Value,
                    standardTimer = plr.itemAnimationMax,
                    vertexStandard = Main.netMode == NetmodeID.Server ? default : new VertexDrawInfoStandardInfo() with
                    {
                        active = true,
                        heatMap = plr.GetModPlayer<MeleeModifyPlayer>().heatMap ?? LogSpiralLibraryMod.HeatMap[0].Value,
                        renderInfos = [[ConfigurationSwoosh.distortConfigs.effectInfo], [ConfigurationSwoosh.maskConfigs.maskEffectInfo, ConfigurationSwoosh.bloomConfigs.effectInfo]],

                        scaler = (item.type == ItemID.TrueExcalibur ? 1.5f : 1) * (rectangle == null ? MeleeModifyPlayer.GetWeaponTextureFromItem(item).Size() : rectangle.Value.Size()).Length() * 1.25f * plr.GetAdjustedItemScale(item),
                        timeLeft = ConfigurationSwoosh.swooshTimeLeft,
                        colorVec = ConfigurationSwoosh.colorVector.AlphaVector,
                        swooshTexIndex = (ConfigurationSwoosh.animateIndex, ConfigurationSwoosh.imageIndex)
                    },
                    standardGlowTexture = item.glowMask != -1 ? TextureAssets.GlowMask[item.glowMask].Value : (item.flame ? TextureAssets.ItemFlame[item.type].Value : null),
                    itemType = item.type,
                    soundStyle = item.UseSound,
                    dustAmount = plr.GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh.dustQuantity,
                    frame = rectangle
                };
                return result;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (currentData != null && UseSwordModify)
            {
                meleeSequence.active = true;
                meleeSequence.currentData.Draw(Main.spriteBatch, MeleeModifyPlayer.GetWeaponTextureFromItem(player.HeldItem));
            }
            return false;
        }
        public override void AI()
        {
            base.AI();
        }
        public override void InitializeSequence(string modName, string fileName)
        {
            var definition = ConfigurationSwoosh.swooshActionStyle;

            if (definition != null)
                base.InitializeSequence(definition.Mod, definition.Name);
            else
                base.InitializeSequence(modName, fileName);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            #region *复杂的伤害计算*
            var plr = player;
            var vec = ((MeleeAction)currentData).targetedVector;

            var itemRectangle = Utils.CenteredRectangle(plr.Center + vec * .5f, vec);
            var sItem = plr.HeldItem;
            float num = 1000f; // to reduce patches, set to 1000, and then turn it into a multiplier later
            if (!sItem.DamageType.UseStandardCritCalcs)
                goto skipStandardCritCalcs;

            int weaponCrit = plr.GetWeaponCrit(sItem);

        skipStandardCritCalcs:
            plr.ApplyBannerOffenseBuff(target, ref modifiers);

            if (plr.parryDamageBuff && sItem.melee)
            {
                modifiers.ScalingBonusDamage += 4f; //num *= 5;
                plr.parryDamageBuff = false;
                plr.ClearBuff(198);
            }
            if (sItem.type == ItemID.BreakerBlade && target.life >= target.lifeMax * 9 / 10)
                num = (int)((float)num * 2.5f);

            if (sItem.type == ItemID.HamBat)
            {
                int num3 = 0;
                if (plr.FindBuffIndex(26) != -1)
                    num3 = 1;

                if (plr.FindBuffIndex(206) != -1)
                    num3 = 2;

                if (plr.FindBuffIndex(207) != -1)
                    num3 = 3;

                float num4 = 1f + 0.05f * (float)num3;
                num = (int)((float)num * num4);
            }

            if (sItem.type == ItemID.Keybrand)
            {
                float t = (float)target.life / (float)target.lifeMax;
                float lerpValue = Utils.GetLerpValue(1f, 0.1f, t, clamped: true);
                float num5 = 1f * lerpValue;
                num = (int)((float)num * (1f + num5));
                Vector2 point = itemRectangle.Center.ToVector2();
                Vector2 positionInWorld = target.Hitbox.ClosestPointInRect(point);
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings
                {
                    PositionInWorld = positionInWorld
                }, plr.whoAmI);
            }

            /*
			int num6 = Main.DamageVar(num, luck);
			*/
            modifiers.SourceDamage *= num / 1000f;
            float armorPenetrationPercent = 0f;
            if (sItem.type == ItemID.Flymeal && target.isLikeATownNPC)
            {
                armorPenetrationPercent = 1f;
                if (target.type == NPCID.Nurse)
                    modifiers.TargetDamageMultiplier *= 2; //num6 *= 2;
            }
            ParticleOrchestraType? particle = sItem.type switch
            {
                ItemID.SlapHand => ParticleOrchestraType.SlapHand,
                ItemID.WaffleIron => ParticleOrchestraType.WaffleIron,
                ItemID.Flymeal => ParticleOrchestraType.FlyMeal,
                ItemID.NightsEdge => ParticleOrchestraType.NightsEdge,
                ItemID.TrueNightsEdge => ParticleOrchestraType.TrueNightsEdge,
                ItemID.Excalibur => ParticleOrchestraType.Excalibur,
                ItemID.TrueExcalibur => ParticleOrchestraType.TrueExcalibur,
                ItemID.TerraBlade => ParticleOrchestraType.TerraBlade,
                _ => null
            };
            if (particle != null)
            {
                ParticleOrchestraSettings particleOrchestraSettings = default;
                particleOrchestraSettings.PositionInWorld = target.Center;
                ParticleOrchestraSettings settings = particleOrchestraSettings;
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, particle.Value, settings, plr.whoAmI);
            }


            plr.StatusToNPC(sItem.type, target.whoAmI);
            if (target.life > 5)
                plr.OnHit(target.Center.X, target.Center.Y, target);

            /*
			num6 += nPC.checkArmorPenetration(armorPenetration, armorPenetrationPercent);
			*/
            modifiers.ArmorPenetration += plr.GetWeaponArmorPenetration(sItem);
            modifiers.ScalingArmorPenetration += armorPenetrationPercent;
            CombinedHooks.ModifyPlayerHitNPCWithItem(plr, sItem, target, ref modifiers);
            #endregion
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //if (!target.CanBeChasedBy()) return;
            try
            {
                var sItem = player.HeldItem;
                var vec = ((MeleeAction)currentData).targetedVector;
                var itemRectangle = Utils.CenteredRectangle(player.Center + vec * .5f, vec);
                CombinedHooks.OnPlayerHitNPCWithItem(player, sItem, target, hit, damageDone);
                player.ApplyNPCOnHitEffects(player.HeldItem, itemRectangle, damageDone, hit.Knockback, target.whoAmI, hit.Damage, damageDone);


                if (sItem.type == ItemID.TheHorsemansBlade && target.CanBeChasedBy())
                {
                    player.HorsemansBlade_SpawnPumpkin(target.whoAmI, damageDone, hit.Knockback);
                }
                //(ApplyNPCOnHitEffects ??= typeof(Player).GetMethod(nameof(ApplyNPCOnHitEffects), BindingFlags.Instance | BindingFlags.NonPublic))
                //    ?.Invoke(player, new object[] { player.HeldItem, itemRectangle, num, knockback, target.whoAmI, damage, damage });

            }
            catch
            {
                Main.NewText("炸了！");
            }

            player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake = ConfigurationSwoosh.shake * Main.rand.NextFloat(0.85f, 1.15f) * (damageDone / MathHelper.Clamp(player.HeldItem.damage, 1, int.MaxValue));//
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake = ConfigurationSwoosh.shake * Main.rand.NextFloat(0.85f, 1.15f);

            base.OnHitPlayer(target, info);
        }
        //public override void AI()
        //{
        //    if (UseSwordModify)
        //    {

        //        base.AI();

        //        //if (currentData != null)
        //        //    player.itemAnimation = currentData.timer;
        //    }
        //    else
        //    {
        //        Projectile.timeLeft = 10;
        //        //meleeSequence.ResetCounter();
        //    }
        //    Projectile.Center = player.Center;
        //    Main.NewText(player.itemAnimation);

        //}
    }

}