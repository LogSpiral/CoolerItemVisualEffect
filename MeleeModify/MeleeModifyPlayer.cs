using CoolerItemVisualEffect.Config;
// using CoolerItemVisualEffect.Config.ConfigSLer;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingContents;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingEffects;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using static CoolerItemVisualEffect.Config.MeleeConfig;

namespace CoolerItemVisualEffect.MeleeModify;
public class MeleeModifyPlayer : ModPlayer
{
    #region 基本量声明

    private MeleeConfig configurationSwoosh;

    public MeleeConfig ConfigurationSwoosh
    {
        get
        {
#if false
            if (weaponGroup != null)
                foreach (var pair in weaponGroup)
                {
                    if (pair.CheckAvailabe(Player.HeldItem))
                    {
                        if (pair.BindSequenceName == "" || pair.BindSequenceName == null) goto label;
                        if (meleeConfigs != null && meleeConfigs.TryGetValue(pair.BindSequenceName, out var config))
                            return config;
                        else if (Main.myPlayer == Player.whoAmI)
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
#endif
        label:
            configurationSwoosh ??= Main.myPlayer == Player.whoAmI ? Instance : new MeleeConfig();

            return configurationSwoosh;
        }
        set => configurationSwoosh = value;
    }

    public List<WeaponSelector> weaponGroup;
    public Dictionary<string, MeleeConfig> meleeConfigs;

    private static int[] VanillaSlashItems { get; }
        = [
            ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.TheHorsemansBlade,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            ItemID.TerraBlade
            ];

    public static bool MeleeBroadSwordCheck(Item item)
    {
        bool flag = MeleeCheck(item.DamageType);
        flag &= item.damage > 0;
        flag &= !item.noUseGraphic;
        flag &= !item.noMelee || VanillaSlashItems.Contains(item.type);
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
                    if (selector.CheckAvailabe(Player.HeldItem))
                        return true;
                }
            return MeleeBroadSwordCheck(Player.HeldItem);
        }
    }

    public bool BeAbleToOverhaul => SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.Overhaul && ConfigurationSwoosh.SwordModifyActive && IsMeleeBroadSword;

    public bool UseSwordModify => BeAbleToOverhaul && Player.itemAnimation > 0;

    public void WeaponGroupSyncing()
    {
        SyncWeaponGroup.Get(Player.whoAmI, weaponGroup, meleeConfigs).Send();
    }

    #endregion 基本量声明

    #region 视觉效果相关

    public Texture2D heatMap;
    public Color mainColor;
    public float hollowCheckScaler;
    public int lastWeaponHash;
    private static int lastWeaponType;
    public static int LastWeaponType => lastWeaponType == ItemID.None ? ItemID.TerraBlade : lastWeaponType;
    public Vector3 hsl;

    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        if (UseSwordModify)
            drawInfo.heldItem = new Item();
        //drawInfo.weaponDrawOrder = WeaponDrawOrder.BehindBackArm;
        base.HideDrawLayers(drawInfo);
    }

    #endregion 视觉效果相关

    public static ModKeybind ModifyActiveKeybind { get; private set; }

    #region Canvas

    public const string CANVASNAMEPREFIX = "CoolerItemVisualEffect:MeleeModify_";

    public static string GetCanvasNameViaID(int whoami) => CANVASNAMEPREFIX + whoami;

    private readonly AirDistortEffect airDistortEffect = new();

    private readonly MaskEffect maskEffect = new();

    private readonly DyeEffect dyeEfect = new();

    private readonly BloomEffect bloomEffect = new();

    private IRenderEffect[][] RenderEffects => field ??= [[airDistortEffect], [maskEffect, dyeEfect, bloomEffect]];

    public void RefreshConfigEffects()
    {
        var config = ConfigurationSwoosh;
        config?.distortConfigs?.CopyToInstance(airDistortEffect);
        config?.maskConfigs?.CopyToInstance(maskEffect);
        config?.dyeConfigs?.CopyToInstance(dyeEfect);
        config?.bloomConfigs?.CopyToInstance(bloomEffect);
    }

    public void RegisterCurrentCanvas()
    {
        RenderCanvasSystem.RegisterCanvasFactory(GetCanvasNameViaID(Player.whoAmI), () => new(RenderEffects));
    }

    #endregion Canvas

    public override void OnEnterWorld()
    {
        SetUpWeaponGroupAndConfig();

        RegisterCurrentCanvas();

        base.OnEnterWorld();
    }

    private void SetUpWeaponGroupAndConfig(bool forced = false)
    {
        if (Player.whoAmI != Main.myPlayer) return;
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

#if false
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
#endif
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
                SyncMeleeModifyActive.Get(Player.whoAmI, active).Send(-1, Player.whoAmI);
            for (int n = 0; n < 32; n++)
                Dust.NewDustPerfect(Player.Center, Main.rand.Next([DustID.FireworkFountain_Blue]), Main.rand.NextVector2Unit() * 32).noGravity = true;
            //, DustID.FireworkFountain_Green, DustID.FireworkFountain_Pink, DustID.FireworkFountain_Red, DustID.FireworkFountain_Yellow
            Main.NewText(Language.GetOrRegister($"Mods.CoolerItemVisualEffect.Misc.MeleeModify{(active ? "Active" : "Deactive")}"));
            if (Main.myPlayer == Player.whoAmI)
                ConfigManager.Save(configurationSwoosh);
        }
        base.ProcessTriggers(triggersSet);
    }

    private void ProjectileShootBan(ILContext il)
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
            ItemID.Sets.SkipsInitialUseSound[Player.HeldItem.type] = SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.Overhaul && ConfigurationSwoosh.SwordModifyActive;
        CheckItemChange(Player);

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

    #endregion 更新状态

    #region 辅助函数

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        if (configurationSwoosh == null)
        {
            configurationSwoosh = Main.myPlayer == Player.whoAmI ? Instance : new MeleeConfig();
        }
        SetUpWeaponGroupAndConfig();
        SyncMeleeConfig.Get(Player.whoAmI, configurationSwoosh).Send(toWho, fromWho);
        SyncWeaponGroup.Get(Player.whoAmI, weaponGroup, meleeConfigs).Send(toWho, fromWho);
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
        if (Main.dedServ) return;
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
            modPlayer.RefreshConfigEffects();
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

    private static List<Func<bool>> noWeaponDisplayDelegate = [];
    private static HashSet<string> registeredDelegateName = [];

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

    #endregion 辅助函数

    private UltraSwoosh currentSwoosh;
    private UltraSwoosh extraSwoosh;

    public override void PreUpdate()
    {
        configurationSwoosh ??= Main.myPlayer == Player.whoAmI ? Instance : new MeleeConfig();
        if (SeverConfig.Instance.meleeModifyLevel == SeverConfig.MeleeModifyLevel.VisualOnly && configurationSwoosh.SwordModifyActive)
        {
            if (MeleeBroadSwordCheck(Player.HeldItem))
            {
                if (Player.itemAnimationMax != 0 && Player.itemAnimation == Player.itemAnimationMax)
                {
                    var timeLeft = configurationSwoosh.swooshTimeLeft;
                    var length = TextureAssets.Item[Player.HeldItem.type].Value.Size().Length() * 1.2f;
                    var aniIdx = configurationSwoosh.animateIndexSwoosh;
                    var baseIdx = configurationSwoosh.baseIndexSwoosh;
                    var alphaVec = configurationSwoosh.colorVector.AlphaVector;
                    var eVec = alphaVec with { Y = 0 };
                    if (eVec.X == 0 && eVec.Z == 0)
                        eVec = new(.5f, 0, .5f);

                    string canvasName = GetCanvasNameViaID(Player.whoAmI);

                    var swoosh = currentSwoosh = UltraSwoosh.NewUltraSwoosh(canvasName, timeLeft, length, Player.Center, (-1.5f, .25f));
                    swoosh.heatMap = heatMap;
                    swoosh.negativeDir = Player.direction != -1;
                    swoosh.rotation = 0f;
                    swoosh.xScaler = 1f;
                    swoosh.aniTexIndex = aniIdx;
                    swoosh.baseTexIndex = baseIdx;
                    swoosh.ColorVector = eVec;

                    if (Player.HeldItem.type == ItemID.TrueExcalibur)
                    {
                        swoosh = extraSwoosh = UltraSwoosh.NewUltraSwoosh(canvasName, timeLeft, length * 1.5f, Player.Center, (-1.5f, .25f));
                        swoosh.heatMap = LogSpiralLibraryMod.HeatMap[5].Value;
                        swoosh.negativeDir = Player.direction != -1;
                        swoosh.rotation = 0f;
                        swoosh.xScaler = 1f;
                        swoosh.aniTexIndex = aniIdx;
                        swoosh.baseTexIndex = baseIdx;
                        swoosh.ColorVector = alphaVec;
                    }
                }
                if (Player.itemAnimation > 0 && currentSwoosh != null)
                {
                    currentSwoosh.timeLeft++;
                    float k = 1 - (float)Player.itemAnimation / Player.itemAnimationMax;

                    if (Player.direction == 1)
                        currentSwoosh.angleRange = (-1.5f + k * .5f, MathHelper.Lerp(-1f, .25f, k));
                    else
                        currentSwoosh.angleRange = (-.5f, MathHelper.Lerp(-1f, .25f, k) + 1);
                    currentSwoosh.center = Player.Center;
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

