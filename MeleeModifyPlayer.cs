using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Localization;
using static CoolerItemVisualEffect.Config.MeleeConfig;
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
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using NetSimplified;
using NetSimplified.Syncing;
using Terraria.GameInput;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using System.Reflection;
using System.Runtime.CompilerServices;
using CoolerItemVisualEffect.Config;
using CoolerItemVisualEffect.Config.ConfigSLer;
using Newtonsoft.Json;
using tModPorter;
using System.Configuration;
using Terraria.GameContent.UI.Elements;
using MonoMod.Utils;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing;
using static Terraria.ModLoader.PlayerDrawLayer;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.System;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee.StandardMelee;

namespace CoolerItemVisualEffect
{
    public class MeleeModifierProj : GlobalProjectile
    {
        public static int[] vanillaSlashProjectiles = [ProjectileID.NightsEdge, ProjectileID.Excalibur, ProjectileID.TrueExcalibur, ProjectileID.TheHorsemansBlade, ProjectileID.TerraBlade2];
        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (mplr.BeAbleToOverhaul && vanillaSlashProjectiles.Contains(projectile.type))
                projectile.Kill();
            base.AI(projectile);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.VisualOnly && vanillaSlashProjectiles.Contains(projectile.type) && Main.player[projectile.owner].GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh.SwordModifyActive)
                return false;
            return base.PreDraw(projectile, ref lightColor);
        }
    }
    public class MeleeModifierItem : GlobalItem
    {
        static bool CheckRightUse(MeleeSequence sequence)
        {
            foreach (var group in sequence.Groups)
            {
                foreach (var wraper in group.wrapers)
                {
                    if (wraper.conditionDefinition.Name == "MouseRight")
                        return true;
                    else if (wraper.IsSequence && CheckRightUse(wraper.sequenceInfo))
                        return true;
                }
            }
            return false;
        }
        public static int[] vanillaSlashItems = [ItemID.NightsEdge, ItemID.TrueNightsEdge, ItemID.TheHorsemansBlade, ItemID.Excalibur, ItemID.TrueExcalibur, ItemID.TerraBlade];
        public override bool AltFunctionUse(Item item, Player player)
        {
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            string key = $"{Mod.Name}/{typeof(CIVESword).Name}";
            if (player.GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh.swooshActionStyle is SequenceDefinition<MeleeAction> definition)
                key = $"{definition.Mod}/{definition.Name}";
            if (mplr.BeAbleToOverhaul && SequenceManager<MeleeAction>.sequences.TryGetValue(key, out var value))
            {
                return CheckRightUse(value);
            }
            return base.AltFunctionUse(item, player);
        }
        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (player.itemAnimation == player.itemAnimationMax && player.ownedProjectileCounts[ModContent.ProjectileType<CIVESword>()] == 0 && mplr.BeAbleToOverhaul)
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
            if (mplr.BeAbleToOverhaul)
                return false;
            return base.CanHitNPC(item, player, target);
        }
        public override bool CanHitPvp(Item item, Player player, Player target)
        {
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (mplr.BeAbleToOverhaul)
                return false;
            return base.CanHitPvp(item, player, target);
        }
    }
    public class SyncWeaponGroup : NetModule
    {
        public byte plrIndex;
        public List<WeaponSelector> list;
        public Dictionary<string, MeleeConfig> dict;
        public static SyncWeaponGroup Get(int plrIndex, List<WeaponSelector> list, Dictionary<string, MeleeConfig> dict)
        {
            var result = NetModuleLoader.Get<SyncWeaponGroup>();
            result.plrIndex = (byte)plrIndex;
            result.list = list;
            result.dict = dict;
            return result;
        }
        public static SyncWeaponGroup Get()
        {
            var result = NetModuleLoader.Get<SyncWeaponGroup>();
            result.plrIndex = (byte)Main.myPlayer;
            var mplr = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
            result.list = mplr.weaponGroup;
            result.dict = mplr.meleeConfigs;
            return result;
        }
        public override void Send(ModPacket p)
        {
            p.Write(plrIndex);
            p.Write(dict != null);
            if (dict != null)
            {
                p.Write(dict.Count);
                foreach (var pair in dict)
                {
                    p.Write(pair.Key);
                    p.Write(JsonConvert.SerializeObject(pair.Value));
                }
            }
            p.Write(list != null);
            if (list != null)
            {
                var content2 = JsonConvert.SerializeObject(list);
                p.Write(content2);
            }

            base.Send(p);
        }
        public override void Read(BinaryReader r)
        {
            plrIndex = r.ReadByte();
            if (r.ReadBoolean())
            {
                //dict = []; JsonConvert.PopulateObject(r.ReadString(), dict);
                //dict = JsonConvert.DeserializeObject<Dictionary<string, MeleeConfig>>(r.ReadString());
                int count = r.ReadInt32();
                dict = [];
                for (int n = 0; n < count; n++)
                {
                    var config = new MeleeConfig();
                    config.designateData?.colors.Clear();
                    string key = r.ReadString();
                    JsonConvert.PopulateObject(r.ReadString(), config);
                    dict.Add(key, config);
                }
            }
            if (r.ReadBoolean())
            {
                list = []; JsonConvert.PopulateObject(r.ReadString(), list);
            }
            base.Read(r);
        }
        public override void Receive()
        {
            var plr = Main.player[plrIndex];
            var MMPlr = plr.GetModPlayer<MeleeModifyPlayer>();
            if (list != null)
            {
                if (MMPlr.weaponGroup == null) MMPlr.weaponGroup = [];
                else
                    MMPlr.weaponGroup.Clear();
                MMPlr.weaponGroup.AddRange(list);
            }
            if (dict != null)
            {
                if (MMPlr.meleeConfigs == null) MMPlr.meleeConfigs = [];
                else
                    MMPlr.meleeConfigs.Clear();
                MMPlr.meleeConfigs.AddRange(dict);
            }
            //if (MMPlr.heatMap != null && MMPlr.hsl != default)
            //    MeleeModifyPlayer.UpdateHeatMap(ref MMPlr.heatMap, MMPlr.hsl, MMPlr.ConfigurationSwoosh, MeleeModifyPlayer.GetWeaponTextureFromItem(plr.HeldItem));
            if (Main.dedServ)
            {
                Get(plrIndex, list, dict).Send(-1, plrIndex);
            }
        }
    }
    public class MeleeModifyPlayer : ModPlayer
    {
        #region 基本量声明
        MeleeConfig configurationSwoosh;
        public MeleeConfig ConfigurationSwoosh
        {
            get
            {
                if (weaponGroup != null)
                    foreach (var pair in weaponGroup)
                    {
                        if (pair.CheckAvailabe(player.HeldItem))
                        {
                            if (pair.BindSequenceName == "" || pair.BindSequenceName == null) goto label;
                            if (meleeConfigs != null && meleeConfigs.TryGetValue(pair.BindSequenceName, out var config))
                                return config;
                            else if (Main.myPlayer == player.whoAmI)
                            {
                                var configPath = Path.Combine(ConfigSLHelper.SavePath, pair.BindSequenceName + ConfigSLHelper.Extension);
                                if (File.Exists(configPath))
                                {
                                    var meleeConfig = new MeleeConfig();
                                    ConfigSLHelper.Load(meleeConfig, configPath, false, false);
                                    meleeConfigs.Add(pair.BindSequenceName, meleeConfig);
                                    return meleeConfig;
                                }
                                else goto label;
                            }
                            else goto label;
                        }
                    }
                label:
                configurationSwoosh ??= Main.myPlayer == player.whoAmI ? Instance : new MeleeConfig();

                return configurationSwoosh;
            }
            set => configurationSwoosh = value;
        }
        Player player => Player;
        public List<WeaponSelector> weaponGroup;
        public Dictionary<string, MeleeConfig> meleeConfigs;
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
            flag &= !new int[] { ItemID.GravediggerShovel, ItemID.Sickle, ItemID.BreathingReed, ItemID.StaffofRegrowth }.Contains(item.type);
            return flag;
        }
        public bool IsMeleeBroadSword
        {
            get
            {
                if (weaponGroup != null)
                    foreach (var selector in weaponGroup)
                    {
                        if (selector.CheckAvailabe(player.HeldItem))
                            return true;
                    }
                return MeleeBroadSwordCheck(player.HeldItem);
            }
        }
        public bool BeAbleToOverhaul => SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.Overhaul && ConfigurationSwoosh.SwordModifyActive && IsMeleeBroadSword;

        public bool UseSwordModify => BeAbleToOverhaul && player.itemAnimation > 0;

        public void WeaponGroupSyncing()
        {
            SyncWeaponGroup.Get(Player.whoAmI, weaponGroup, meleeConfigs).Send();
        }
        #endregion

        #region 视觉效果相关
        public Texture2D heatMap;
        public Color mainColor;
        public float hollowCheckScaler;
        public int lastWeaponHash;
        static int lastWeaponType;
        public static int LastWeaponType => lastWeaponType == ItemID.None ? ItemID.TerraBlade : lastWeaponType;
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

        public override void OnEnterWorld()
        {
            SetUpWeaponGroupAndConfig();
            //foreach(var pair in dict.OrderBy(pair => -pair.Key.index))
            //    weaponGroup.Add(pair.Key, pair.Value);
            base.OnEnterWorld();
        }
        void SetUpWeaponGroupAndConfig(bool forced = false)
        {
            if (player.whoAmI != Main.myPlayer) return;
            if (weaponGroup == null && meleeConfigs == null)
            {
                weaponGroup = [];
                meleeConfigs = [];
            }
            else
            {
                if (forced)
                {
                    weaponGroup.Clear();
                    meleeConfigs.Clear();
                }
                else
                    return;
            }

            if (!Directory.Exists(WeaponSelectorSystem.SavePath))
                Directory.CreateDirectory(WeaponSelectorSystem.SavePath);
            var tablePath = Path.Combine(WeaponSelectorSystem.SavePath, "indexTable.txt");
            if (File.Exists(tablePath))
            {
                var indexTable = File.ReadAllLines(tablePath);
                foreach (string path in indexTable)
                {
                    var selectorPath = Path.Combine(WeaponSelectorSystem.SavePath, path + WeaponSelectorSystem.Extension);
                    if (!File.Exists(selectorPath))
                        continue;
                    var selector = WeaponSelector.Load(selectorPath);
                    weaponGroup.Add(selector);
                    if (selector.BindSequenceName == null || selector.BindSequenceName.Length == 0 || meleeConfigs.ContainsKey(selector.BindSequenceName)) continue;
                    var configPath = Path.Combine(ConfigSLHelper.SavePath, selector.BindSequenceName + ConfigSLHelper.Extension);
                    if (File.Exists(configPath))
                    {
                        var meleeConfig = new MeleeConfig();
                        ConfigSLHelper.Load(meleeConfig, configPath, false, false);
                        meleeConfigs.Add(selector.BindSequenceName, meleeConfig);
                    }
                }
            }
            WeaponGroupSyncing();
        }
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
            c.EmitDelegate<Func<Player, bool>>(player => !player.GetModPlayer<MeleeModifyPlayer>().BeAbleToOverhaul);// && !(SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.VisualOnly && player.GetModPlayer<MeleeModifyPlayer>().configurationSwoosh.SwordModifyActive && MeleeModifyPlayer.MeleeBroadSwordCheck(player.HeldItem))
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
                ItemID.Sets.SkipsInitialUseSound[player.HeldItem.type] = SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.Overhaul && ConfigurationSwoosh.SwordModifyActive;
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
        public static List<(Func<Item, Texture2D> func, float priority)> weaponGetFunctions = [];
        public static bool RefreshWeaponTexFunc;
        /// <summary>
        /// 如果你的武器贴图和"看上去"的不一样，用这个换成看上去的
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Texture2D GetWeaponTextureFromItem(Item item)
        {
            if (item == null || item.type == ItemID.None) return TextureAssets.Item[LastWeaponType].Value;
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
            if (configurationSwoosh == null)
            {
                configurationSwoosh = Main.myPlayer == player.whoAmI ? Instance : new MeleeConfig();
            }
            SetUpWeaponGroupAndConfig();
            SyncMeleeConfig.Get(player.whoAmI, configurationSwoosh).Send(toWho, fromWho);
            SyncWeaponGroup.Get(player.whoAmI, weaponGroup, meleeConfigs).Send(toWho, fromWho);
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
        public static void UpdateHeatMap(ref Texture2D texture, Vector3 hsl, MeleeConfig config, Texture2D itemTexture)
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
                        for (int n = 0; n < 300; n++)
                            colors[n] = GetHeatMapFactor(n / 299f, 6, config.heatMapFactorStyle).ArrayLerp(currentColor);
                        break;
                    }
                case HeatMapCreateStyle.Designate:
                    {
                        config.designateData.PreGetValue();
                        for (int i = 0; i < 300; i++)
                            colors[i] = config.designateData.GetValue(GetHeatMapFactor(i / 299f, 6, config.heatMapFactorStyle));
                        break;
                    }
                case HeatMapCreateStyle.CosineGenerate_RGB:
                    {
                        for (int i = 0; i < 300; i++)
                            colors[i] = config.rgbData.GetValue(GetHeatMapFactor(i / 299f, 6, config.heatMapFactorStyle));
                        break;
                    }
                case HeatMapCreateStyle.CosineGenerate_HSL:
                    {
                        for (int i = 0; i < 300; i++)
                            colors[i] = config.hslData.GetValue(GetHeatMapFactor(i / 299f, 6, config.heatMapFactorStyle));
                        break;
                    }
                case HeatMapCreateStyle.ByFunction:
                default:
                    {
                        for (int i = 0; i < 300; i++)
                            colors[i] = config.byFuncData.GetColor(GetHeatMapFactor(i / 299f, 6, config.heatMapFactorStyle), hsl);
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
                lastWeaponType = player.HeldItem.type;
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
                    Vector2 coord = new(n % width, n / width);
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

        public bool ShouldWeaponDisplay
        {
            get
            {
                bool flag = MiscConfig.Instance.useWeaponDisplay;
                flag &= !UseSwordModify;
                foreach (var condition in noWeaponDisplayDelegate)
                    if (condition?.Invoke() is bool f)
                        flag &= !f;
                return flag;
            }
        }
        static List<Func<bool>> noWeaponDisplayDelegate = [];
        static HashSet<string> registeredDelegateName = [];
        public static void RegisterNoWeaponDisplayCondition(Func<bool> condition, string name)
        {
            if (registeredDelegateName.Contains(name))
                throw new Exception("a condition with a same name has been added");
            noWeaponDisplayDelegate.Add(condition);
            registeredDelegateName.Add(name);
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (UseSwordModify)
                drawInfo.heldItem = new Item();
            if (ShouldWeaponDisplay && !drawInfo.headOnlyRender)
            {
                if (Main.gameMenu && MiscConfig.Instance.firstWeaponDisplay)//
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
            Rectangle rectangle = new(0, 0, texture.Width, texture.Height);
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
            DrawData item = new(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, MiscConfig.Instance.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
            drawInfo.DrawDataCache.Add(item);
            if (holditem.glowMask >= 0)
            {
                Texture2D glow = TextureAssets.GlowMask[holditem.glowMask].Value;
                DrawData itemglow = new(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, MiscConfig.Instance.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
                drawInfo.DrawDataCache.Add(itemglow);
            }
            if (holditem.ModItem != null && ModContent.HasAsset(holditem.ModItem.Texture + "_Glow"))
            {
                Texture2D glow = ModContent.Request<Texture2D>(holditem.ModItem.Texture + "_Glow").Value;
                DrawData itemglow = new(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, MiscConfig.Instance.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
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

        UltraSwoosh currentSwoosh;
        UltraSwoosh extraSwoosh;
        public override void PreUpdate()
        {
            configurationSwoosh ??= Main.myPlayer == player.whoAmI ? Instance : new MeleeConfig();
            if (SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.VisualOnly && configurationSwoosh.SwordModifyActive)
            {
                if (MeleeBroadSwordCheck(player.HeldItem))
                {
                    if (player.itemAnimationMax != 0 && player.itemAnimation == player.itemAnimationMax)
                    {
                        var timeLeft = configurationSwoosh.swooshTimeLeft;
                        var length = TextureAssets.Item[player.HeldItem.type].Value.Size().Length() * 1.2f;
                        var aniIdx = configurationSwoosh.animateIndexSwoosh;
                        var baseIdx = configurationSwoosh.baseIndexSwoosh;
                        var alphaVec = configurationSwoosh.colorVector.AlphaVector;
                        var eVec = alphaVec with { Y = 0 };
                        if (eVec.X == 0 && eVec.Z == 0)
                            eVec = new(.5f, 0, .5f);

                        currentSwoosh = UltraSwoosh.NewUltraSwoosh(Color.White, timeLeft, length, player.Center, heatMap, player.direction != 1, 0, 1f, (-1.5f, .25f), aniIdx, baseIdx, eVec);
                        if (player.HeldItem.type == ItemID.TrueExcalibur)
                            extraSwoosh = UltraSwoosh.NewUltraSwoosh(Color.White, timeLeft, length * 1.5f, player.Center, LogSpiralLibraryMod.HeatMap[5].Value, player.direction != 1, 0, 1, (-1.5f, .25f), aniIdx, baseIdx, alphaVec);
                        currentSwoosh.ModityAllRenderInfo([[configurationSwoosh.distortConfigs.effectInfo], [configurationSwoosh.maskConfigs.maskEffectInfo, configurationSwoosh.dyeConfigs.dyeInfo, configurationSwoosh.bloomConfigs.effectInfo]]);
                    }
                    if (player.itemAnimation > 0 && currentSwoosh != null)
                    {
                        currentSwoosh.timeLeft++;
                        float k = 1 - (float)player.itemAnimation / player.itemAnimationMax;

                        if (player.direction == 1)
                            currentSwoosh.angleRange = (-1.5f + k * .5f, MathHelper.Lerp(-1f, .25f, k));
                        else
                            currentSwoosh.angleRange = (-.5f, MathHelper.Lerp(-1f, .25f, k) + 1);
                        currentSwoosh.center = player.Center;
                        currentSwoosh.ColorVector = configurationSwoosh.colorVector.AlphaVector * k;

                        if (extraSwoosh != null)
                        {
                            extraSwoosh.timeLeft++;
                            extraSwoosh.angleRange = currentSwoosh.angleRange;
                            extraSwoosh.center = currentSwoosh.center;
                            var eVec = currentSwoosh.ColorVector with { Y = 0 };
                            if (eVec.X == 0 && eVec.Z == 0)
                                eVec = new Vector3(.5f, 0, .5f) * k;
                            extraSwoosh.ColorVector = eVec;
                        }
                    }
                }
            }
            base.PreUpdate();
        }
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
        public MeleeConfig ConfigurationSwoosh => player.GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh;
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
                    standardShotCooldown = CombinedHooks.TotalUseTime(plr.HeldItem.useTime, plr, plr.HeldItem),//plr.itemTimeMax,
                    vertexStandard = Main.netMode == NetmodeID.Server ? default : new VertexDrawInfoStandardInfo() with
                    {
                        active = true,
                        heatMap = plr.GetModPlayer<MeleeModifyPlayer>().heatMap ?? LogSpiralLibraryMod.HeatMap[0].Value,
                        renderInfos = [[ConfigurationSwoosh.distortConfigs.effectInfo], [ConfigurationSwoosh.maskConfigs.maskEffectInfo, ConfigurationSwoosh.dyeConfigs.dyeInfo, ConfigurationSwoosh.bloomConfigs.effectInfo]],

                        scaler = (item.type == ItemID.TrueExcalibur ? 1.5f : 1) * (rectangle == null ? MeleeModifyPlayer.GetWeaponTextureFromItem(item).Size() : rectangle.Value.Size()).Length() * 1.25f * plr.GetAdjustedItemScale(item),
                        timeLeft = ConfigurationSwoosh.swooshTimeLeft,
                        colorVec = ConfigurationSwoosh.colorVector.AlphaVector,
                        swooshTexIndex = (ConfigurationSwoosh.animateIndexSwoosh, ConfigurationSwoosh.baseIndexSwoosh),
                        stabTexIndex = (ConfigurationSwoosh.animateIndexStab, ConfigurationSwoosh.baseIndexStab),
                        alphaFactor = ConfigurationSwoosh.alphaFactor,
                        heatRotation = ConfigurationSwoosh.directOfHeatMap
                    },
                    standardGlowTexture = item.glowMask != -1 ? TextureAssets.GlowMask[item.glowMask].Value : (item.flame ? TextureAssets.ItemFlame[item.type].Value : null),
                    itemType = item.type,
                    soundStyle = item.UseSound,
                    dustAmount = plr.GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh.dustQuantity,
                    frame = rectangle,
                    extraLight = ConfigurationSwoosh.weaponExtraLight
                };
                return result;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var dyeInfo = ConfigurationSwoosh.dyeConfigs.dyeInfo;
            if (dyeInfo.Active && LogSpiralLibraryMod.CanUseRender)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                dyeInfo.PreDraw(Main.spriteBatch, Main.instance.GraphicsDevice, LogSpiralLibraryMod.Instance.Render, LogSpiralLibraryMod.Instance.Render_Swap);
            }

            if (currentData != null && UseSwordModify)
            {
                meleeSequence.active = true;
                meleeSequence.currentData.Draw(Main.spriteBatch, MeleeModifyPlayer.GetWeaponTextureFromItem(player.HeldItem));
            }
            if (dyeInfo.Active && LogSpiralLibraryMod.CanUseRender)
            {
                Main.spriteBatch.End();
                dyeInfo.PostDraw(Main.spriteBatch, Main.instance.GraphicsDevice, LogSpiralLibraryMod.Instance.Render, LogSpiralLibraryMod.Instance.Render_Swap);
                dyeInfo.DrawToScreen(Main.spriteBatch, Main.instance.GraphicsDevice, LogSpiralLibraryMod.Instance.Render, LogSpiralLibraryMod.Instance.Render_Swap);
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            //base.PreDraw(ref lightColor);
            return false;
        }
        static float BalancingData(ActionModifyData orig, int cycle)
        {
            float k = 1f;
            k /= MathF.Sqrt(orig.actionOffsetSize);
            k *= orig.actionOffsetTimeScaler;
            k /= MathF.Max(MathF.Pow(orig.actionOffsetKnockBack, .25f), 1f);
            k /= 1 + orig.actionOffsetCritMultiplyer * .2f + orig.actionOffsetCritAdder * .01f;

            return k / cycle / orig.actionOffsetDamage;// orig.actionOffsetDamage *
        }
        bool UseBalance => SeverConfig.Instance.AutoBalanceData && meleeSequence?.currentData is LSLMelee;// meleeSequence?.currentData != null;
        public override void AI()
        {
            var mplr = player.GetModPlayer<MeleeModifyPlayer>();
            if (!mplr.ConfigurationSwoosh.SwordModifyActive)
                Projectile.Kill();
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
            var vec = currentData.targetedVector;

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

            if (UseBalance)
                modifiers.SourceDamage *= BalancingData(currentData.ModifyData, currentData.Cycle);
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //if (!target.CanBeChasedBy()) return;

            SequencePlayer seqPlr = player.GetModPlayer<SequencePlayer>();
            if (seqPlr.cachedTime >= StandardInfo.standardShotCooldown && player.HeldItem.shoot == ProjectileID.None)
            {
                seqPlr.cachedTime -= StandardInfo.standardShotCooldown;
                player._spawnBloodButcherer = true;
                player._spawnMuramasaCut = true;
                player._spawnTentacleSpikes = true;
                player._spawnVolcanoExplosion = true;
            }

            try
            {
                var sItem = player.HeldItem;
                var vec = currentData.targetedVector;
                var itemRectangle = Utils.CenteredRectangle(player.Center + vec * .5f, new Vector2(MathF.Abs(vec.X), MathF.Abs(vec.Y)));
                CombinedHooks.OnPlayerHitNPCWithItem(player, sItem, target, hit, damageDone);
                player.ApplyNPCOnHitEffects(player.HeldItem, itemRectangle, damageDone, hit.Knockback, target.whoAmI, hit.Damage, damageDone);


                if (sItem.type == ItemID.TheHorsemansBlade && target.CanBeChasedBy())
                {
                    player.HorsemansBlade_SpawnPumpkin(target.whoAmI, damageDone, hit.Knockback);
                }
                //(ApplyNPCOnHitEffects ??= typeof(Player).GetMethod(nameof(ApplyNPCOnHitEffects), BindingFlags.Instance | BindingFlags.NonPublic))
                //    ?.Invoke(player, new object[] { player.HeldItem, itemRectangle, num, knockback, target.whoAmI, damage, damage });
            }
            catch (Exception e)
            {
                Main.NewText(e.Message);
            }
            float delta = player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake;
            base.OnHitNPC(target, hit, damageDone);
            delta -= player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake;
            player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake += (1 - ConfigurationSwoosh.shake) * delta;
            //player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake = ConfigurationSwoosh.shake * Main.rand.NextFloat(0.85f, 1.15f) * (damageDone / MathHelper.Clamp(player.HeldItem.damage, 1, int.MaxValue));//
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