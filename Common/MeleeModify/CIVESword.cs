using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.MeleeModify;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee.Core;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee.StandardMelee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.System;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;

namespace CoolerItemVisualEffect.MeleeModify;

public partial class CIVESword : MeleeSequenceProj
{
    #region 标准参数设置

    //public override void UpdateStandardInfo(StandardInfo standardInfo, VertexDrawStandardInfo vertexStandard)
    public override void InitializeStandardInfo(StandardInfo standardInfo, VertexDrawStandardInfo vertexStandard)
    {
        var plr = Player;
        var item = plr.HeldItem;

        #region 设置标准参数

        standardInfo.standardScaler = 80;

        standardInfo.standardColor = plr.GetModPlayer<MeleeModifyPlayer>().MainColor;

        standardInfo.standardTimer = Player.itemAnimationMax;

        standardInfo.standardShotCooldown = CombinedHooks.TotalUseTime(plr.HeldItem.useTime, plr, plr.HeldItem);


        standardInfo.itemType = item.type;

        standardInfo.soundStyle = item.UseSound;

        standardInfo.dustAmount = Player.GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh.dustQuantity;


        standardInfo.extraLight = ConfigurationSwoosh.weaponExtraLight;

        #endregion 设置标准参数

        #region 设置顶点绘制标准参数以及其它视觉相关

        if (Main.dedServ) return;

        #region 加载原版贴图

        if (item.flame && !TextureAssets.ItemFlame[item.type].IsLoaded)
            Main.instance.LoadItemFlames(item.type);

        if (item.glowMask != -1 && !TextureAssets.GlowMask[item.glowMask].IsLoaded)
            Main.Assets.Request<Texture2D>(TextureAssets.GlowMask[item.glowMask].Name);

        #endregion 加载原版贴图

        var rectangle = Main.itemAnimations[item.type]?.GetFrame(TextureAssets.Item[item.type].Value);

        standardInfo.standardGlowTexture = item.glowMask != -1 ? TextureAssets.GlowMask[item.glowMask].Value : (item.flame ? TextureAssets.ItemFlame[item.type].Value : null);

        standardInfo.frame = rectangle;


        vertexStandard.canvasName = MeleeModifyPlayer.GetCanvasNameViaID(Player.whoAmI);

        vertexStandard.heatMap = plr.GetModPlayer<MeleeModifyPlayer>().HeatMap ?? LogSpiralLibraryMod.HeatMap[0].Value;

        vertexStandard.scaler = (item.type is ItemID.NightsEdge or ItemID.TrueExcalibur ? 1.5f : 1) * (rectangle == null ? MeleeModifyPlayerUtils.GetWeaponTextureFromItem(item).Size() : rectangle.Value.Size()).Length() * 1.25f * plr.GetAdjustedItemScale(item);

        vertexStandard.timeLeft = ConfigurationSwoosh.swooshTimeLeft;

        vertexStandard.colorVec = ConfigurationSwoosh.colorVector.AlphaVector;

        vertexStandard.swooshTexIndex = (ConfigurationSwoosh.animateIndexSwoosh, ConfigurationSwoosh.baseIndexSwoosh);

        vertexStandard.stabTexIndex = (ConfigurationSwoosh.animateIndexStab, ConfigurationSwoosh.baseIndexStab);

        vertexStandard.alphaFactor = ConfigurationSwoosh.alphaFactor;

        vertexStandard.heatRotation = ConfigurationSwoosh.directOfHeatMap;

        if (ConfigurationSwoosh.dyeConfigs is { Available: true } dyeConfig)
            vertexStandard.SetDyeShaderID(dyeConfig.Dye.Type);

        #endregion 设置顶点绘制标准参数
    }

    #endregion 标准参数设置

    public override bool? CanHitNPC(NPC target)
    {
        if (target.isLikeATownNPC && Player.HeldItem.type == ItemID.Flymeal) return true;
        return base.CanHitNPC(target);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (UseSwordModify)
            return base.Colliding(projHitbox, targetHitbox);
        return false;
    }

    public bool UseSwordModify => Player.GetModPlayer<MeleeModifyPlayer>().UseSwordModify;//!SequenceModel.IsCompleted;
    public override string Texture => $"Terraria/Images/Item_{ItemID.TerraBlade}";
    public MeleeConfig ConfigurationSwoosh => Player.GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh;

    public override bool PreDraw(ref Color lightColor)
    {
        if (!UseSwordModify) return false;

        var dyeInfo = ConfigurationSwoosh.dyeConfigs.EffectInstance;
        var spriteBatch = Main.spriteBatch;
        var graphicsDevice = Main.graphics.GraphicsDevice;

        if (dyeInfo.Active && LogSpiralLibraryMod.CanUseRender)
        {
            spriteBatch.End();
            spriteBatch.Begin();
            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            graphicsDevice.SetRenderTarget(LogSpiralLibraryMod.Instance.RenderOrig);
            graphicsDevice.Clear(Color.Transparent);
        }

        DrawBlade(MeleeModifyPlayerUtils.GetWeaponTextureFromItem(Player.HeldItem));

        if (dyeInfo.Active && LogSpiralLibraryMod.CanUseRender)
        {
            var contentRender = LogSpiralLibraryMod.Instance.RenderOrig;
            var assistRender = LogSpiralLibraryMod.Instance.Render;

            spriteBatch.End();
            dyeInfo.ProcessRender(Main.spriteBatch, Main.instance.GraphicsDevice, ref contentRender, ref assistRender);



            spriteBatch.Begin();
            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            spriteBatch.Draw(contentRender, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        return false;
    }

    private static float BalancingData(ActionModifyData orig, int cycle)
    {
        var k = 1f;
        k /= MathF.Sqrt(orig.Size);
        k *= orig.TimeScaler;
        k /= MathF.Max(MathF.Pow(orig.KnockBack, .25f), 1f);
        k /= 1 + orig.CritMultiplyer * .2f + orig.CritAdder * .01f;

        return k / cycle / orig.Damage;// orig.actionOffsetDamage *
    }

    private bool UseBalance => ServerConfig.Instance.AutoBalanceData && CurrentElement is LSLMelee;// meleeSequence?.currentData != null;

    public override void AI()
    {
        var mplr = Player.GetModPlayer<MeleeModifyPlayer>();
        if (!mplr.IsModifyActive)
            Projectile.Kill();
        base.AI();
    }

    public override void InitializeSequence(string modName, string fileName)
    {
        var definition = Player.GetModPlayer<MeleeModifyPlayer>().SwooshActionStyle;
        if (definition == null || definition.GetSequence() is not { } result) 
        {
            Projectile.Kill();
            Main.NewText(this.GetLocalizedValue("Failed"), Color.Red);
            return;
        }
        meleeSequence = result;
        SequenceModel = new SequenceModel(meleeSequence);
        SequenceModel.OnInitializeElement += element =>
        {
            if (element is not MeleeAction action) return;
            action.StandardInfo = StandardInfo;
            action.Owner = Player;
            action.Projectile = Projectile;
            Projectile.netUpdate = true;
            InitializeElement(action);
        };
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        #region *复杂的伤害计算*

        var plr = Player;
        var vec = CurrentElement.targetedVector;

        var itemRectangle = Utils.CenteredRectangle(plr.Center + vec * .5f, vec);
        var sItem = plr.HeldItem;
        var num = 1000f; // to reduce patches, set to 1000, and then turn it into a multiplier later
        if (!sItem.DamageType.UseStandardCritCalcs)
            goto skipStandardCritCalcs;

        var weaponCrit = plr.GetWeaponCrit(sItem);

    skipStandardCritCalcs:
        plr.ApplyBannerOffenseBuff(target, ref modifiers);

        if (plr.parryDamageBuff && sItem.melee)
        {
            modifiers.ScalingBonusDamage += 4f; //num *= 5;
            plr.parryDamageBuff = false;
            plr.ClearBuff(BuffID.ParryDamageBuff);
        }
        if (sItem.type == ItemID.BreakerBlade && target.life >= target.lifeMax * 9 / 10)
            num = (int)((float)num * 2.5f);

        if (sItem.type == ItemID.HamBat)
        {
            var num3 = 0;
            if (plr.FindBuffIndex(BuffID.WellFed) != -1)
                num3 = 1;

            if (plr.FindBuffIndex(BuffID.WellFed2) != -1)
                num3 = 2;

            if (plr.FindBuffIndex(BuffID.WellFed3) != -1)
                num3 = 3;

            var num4 = 1f + 0.05f * (float)num3;
            num = (int)((float)num * num4);
        }

        if (sItem.type == ItemID.Keybrand)
        {
            var t = (float)target.life / (float)target.lifeMax;
            var lerpValue = Utils.GetLerpValue(1f, 0.1f, t, clamped: true);
            var num5 = 1f * lerpValue;
            num = (int)((float)num * (1f + num5));
            var point = itemRectangle.Center.ToVector2();
            var positionInWorld = target.Hitbox.ClosestPointInRect(point);
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings
            {
                PositionInWorld = positionInWorld
            }, plr.whoAmI);
        }

        /*
			int num6 = Main.DamageVar(num, luck);
			*/
        modifiers.SourceDamage *= num / 1000f;
        var armorPenetrationPercent = 0f;
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
            var settings = particleOrchestraSettings;
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

        #endregion *复杂的伤害计算*

        if (UseBalance)
            modifiers.SourceDamage *= BalancingData(CurrentElement.ModifyData, CurrentElement.CounterMax);
        base.ModifyHitNPC(target, ref modifiers);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //if (!target.CanBeChasedBy()) return;

        var seqPlr = Player.GetModPlayer<SequencePlayer>();
        if (seqPlr.cachedTime >= StandardInfo.standardShotCooldown && Player.HeldItem.shoot == ProjectileID.None)
        {
            seqPlr.cachedTime -= StandardInfo.standardShotCooldown;
            Player._spawnBloodButcherer = true;
            Player._spawnMuramasaCut = true;
            Player._spawnTentacleSpikes = true;
            Player._spawnVolcanoExplosion = true;
        }

        try
        {
            var sItem = Player.HeldItem;
            var vec = CurrentElement.targetedVector;
            var itemRectangle = Utils.CenteredRectangle(Player.Center + vec * .5f, new Vector2(MathF.Abs(vec.X), MathF.Abs(vec.Y)));
            CombinedHooks.OnPlayerHitNPCWithItem(Player, sItem, target, hit, damageDone);
            Player.ApplyNPCOnHitEffects(Player.HeldItem, itemRectangle, damageDone, hit.Knockback, target.whoAmI, hit.Damage, damageDone);

            if (sItem.type == ItemID.TheHorsemansBlade && target.CanBeChasedBy())
            {
                Player.HorsemansBlade_SpawnPumpkin(target.whoAmI, damageDone, hit.Knockback);
            }
            //(ApplyNPCOnHitEffects ??= typeof(Player).GetMethod(nameof(ApplyNPCOnHitEffects), BindingFlags.Instance | BindingFlags.NonPublic))
            //    ?.Invoke(Player, new object[] { Player.HeldItem, itemRectangle, num, knockback, target.whoAmI, damage, damage });
        }
        catch (Exception e)
        {
            Main.NewText(e.Message);
        }
        var delta = Player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake;
        base.OnHitNPC(target, hit, damageDone);
        delta -= Player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake;
        Player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake += (1 - ConfigurationSwoosh.shake) * delta;
        //Player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake = ConfigurationSwoosh.shake * Main.rand.NextFloat(0.85f, 1.15f) * (damageDone / MathHelper.Clamp(Player.HeldItem.damage, 1, int.MaxValue));//
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        Player.GetModPlayer<LogSpiralLibraryPlayer>().strengthOfShake = ConfigurationSwoosh.shake * Main.rand.NextFloat(0.85f, 1.15f);

        base.OnHitPlayer(target, info);
    }


}