using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using static CoolerItemVisualEffect.ConfigurationSwoosh_Advanced;
using Terraria.ID;

namespace CoolerItemVisualEffect
{
    public class UltraSwoosh
    {
        public float rotation;
        public byte timeLeft;
        public float xScaler;
        public Vector2 center;
        public bool negativeDir;
        public Texture2D heatMap;
        public Color color;
        public Vector3 hsl;
        public int type;
        public float checkAirFactor;
        public float rotationVelocity;
        public readonly CustomVertexInfo[] vertexInfos = new CustomVertexInfo[60];
        public bool Active => timeLeft > 0;
        public float scaler;
        public byte timeLeftMax;
    }
    public class CoolerItemVisualEffectPlayer : ModPlayer
    {
        public bool SwooshActive
        {
            get
            {
                bool flag = false;
                foreach (var ultra in ultraSwooshes)
                {
                    if (ultra != null && ultra.Active) { flag = true; break; }
                }
                return flag && ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra;
            }

        }
        public int lastItemAnimation;
        public float kValue;
        public bool negativeDir;
        public float rotationForShadow;
        public float kValueNext;
        public float rotationForShadowNext;
        //public int testState;
        public int swingCount;
        public (Texture2D tex, Color color, float checkAirFactor, int type) colorInfo;
        public Vector3 hsl;
        public float direct;
        ConfigurationSwoosh_Advanced configurationSwoosh;
        public float scaler;
        public readonly CustomVertexInfo[] vertexInfos = new CustomVertexInfo[90];
        public (Vector2 u, Vector2 v) vectors;
        public readonly UltraSwoosh[] ultraSwooshes = new UltraSwoosh[60];
        public UltraSwoosh currentSwoosh;
        //public override void PreUpdate() {
        //    if (Player.itemAnimation == 1) {
        //        negativeDir ^= true;
        //        rotationForShadow = (Main.MouseWorld - Player.Center).ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
        //        kValue = Main.rand.NextFloat(1, 2);
        //    }
        //}
        public int swooshTimeCounter;
        public float actionOffsetSize;
        public float actionOffsetSpeed;
        public float actionOffsetKnockBack;
        public float actionOffsetDamage;
        public int actionOffsetCritAdder;
        public float actionOffsetCritMultiplyer;

        public override bool? CanHitNPC(Item item, NPC target)
        {
            //bool? modCanHit = CombinedHooks.CanPlayerHitNPCWithItem(this, sItem, Main.npc[i]);
            var style = ConfigurationNormal.instance.hitBoxStyle;
            if (style == 0 || style == ConfigurationNormal.HitBoxStyle.矩形Rectangle || ConfigurationSwoosh.coolerSwooshQuality == QualityType.关off) return null;
            var canHit = false;

            if (style == ConfigurationNormal.HitBoxStyle.剑气UltraSwoosh)
            {
                if (ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra)
                {
                    foreach (var swoosh in ultraSwooshes)
                    {
                        if (swoosh != null && swoosh.Active)
                        {
                            float _point = 0f;
                            Vector2 center = swoosh.center;
                            Vector2 unit = swoosh.rotation.ToRotationVector2() * swoosh.scaler * swoosh.xScaler;
                            if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), center - .5f * unit, center + unit, scaler, ref _point))
                            {
                                canHit = true;
                                break;
                            }
                        }
                    }
                }
                goto mylabel;
            }
            if (style == ConfigurationNormal.HitBoxStyle.线状AABBLine) goto mylabel;
            return false;
        mylabel:
            float point = 0f;
            canHit |= Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), player.Center, HitboxPosition + player.Center, 32, ref point);
            return canHit && !target.dontTakeDamage && player.CanNPCBeHitByPlayerOrPlayerProjectile(target);
        }
        public override void OnEnterWorld(Player player)
        {

            //Main.instance.Window.Title = "泰拉瑞亚：东方太阳，正在升起";
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
        public void UpdateVertex()
        {
            //Main.NewText((ConfigSwooshInstance.IsExpandGrow, ConfigSwooshInstance.IsHorizontallyGrow, ConfigSwooshInstance.IsOffestGrow, ConfigSwooshInstance.growStyle));
            var modPlayer = this;
            var instance = ConfigurationSwoosh;
            var drawPlayer = player;
            var fac = modPlayer.FactorGeter;
            fac = modPlayer.negativeDir ? 1 - fac : fac;
            //var drawCen = drawPlayer.Center;
            float rotVel = instance.swooshActionStyle == SwooshAction.旋风劈 && modPlayer.swingCount % 3 == 0 ? instance.rotationVelocity : 1;
            var theta = (1.2375f * fac * rotVel - 1.125f) * MathHelper.Pi;
            var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
            float xScaler = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, fac) : modPlayer.kValue;
            //float scaler = ;
            modPlayer.scaler = itemTex.Size().Length() * drawPlayer.GetAdjustedItemScale(drawPlayer.HeldItem) / xScaler * 0.5f * modPlayer.RealSize * modPlayer.colorInfo.checkAirFactor;

            var rotator = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, fac) : modPlayer.rotationForShadow;
            float swooshAniFac;
            if (instance.coolerSwooshQuality == QualityType.极限ultra)
            {
                swooshAniFac = modPlayer.negativeDir ? 0 : 1;
            }
            else
            {
                swooshAniFac = modPlayer.negativeDir ? 4 * fac - 3 : 4 * fac;
                swooshAniFac = MathHelper.Clamp(swooshAniFac, 0, 1);
            }
            var vec = theta.ToRotationVector2() * scaler;
            vectors.u = new Vector2(xScaler * (vec.X - vec.Y), -vec.X - vec.Y).RotatedBy(rotator) / colorInfo.checkAirFactor;
            vectors.v = new Vector2(-xScaler * (vec.X + vec.Y), vec.Y - vec.X).RotatedBy(rotator) / colorInfo.checkAirFactor;
            if (ConfigurationSwoosh.onlyChangeSizeOfSwoosh)
            {
                vectors.u /= RealSize;
                vectors.v /= RealSize;
            }
            var theta3 = (1.2375f * swooshAniFac * rotVel - 1.125f) * MathHelper.Pi;
            float xScaler3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, swooshAniFac) : modPlayer.kValue;
            var rotator3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, swooshAniFac) : modPlayer.rotationForShadow;
            var alphaLight = hsl.Z < instance.isLighterDecider ? Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).R / 255f * .6f : 0.6f;

            for (int i = 0; i < 45; i++)
            {
                var f = i / 44f;
                var theta2 = f.Lerp(theta3, theta, true);
                var xScaler2 = (instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? f : 1).Lerp(xScaler3, xScaler, true);
                var rotator2 = (instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? f : 1).Lerp(rotator3, rotator, true);
                var cos2 = (float)Math.Cos(theta2) * scaler;
                var sin2 = (float)Math.Sin(theta2) * scaler;
                var u2 = new Vector2(xScaler2 * (cos2 - sin2), -cos2 - sin2).RotatedBy(rotator2);
                var v2 = new Vector2(-xScaler2 * (cos2 + sin2), sin2 - cos2).RotatedBy(rotator2);
                var newVec = u2 + v2;
                //var _f = f * f;
                //_f = MathHelper.Clamp(_f, 0, 1);
                var _flag = (byte)ConfigurationSwoosh.swooshActionStyle > 0 && (byte)ConfigurationSwoosh.swooshActionStyle < 9;
                var progress = _flag ? Utils.GetLerpValue(player.itemAnimationMax, 18, player.itemAnimation, true) : 1f;
                vertexInfos[2 * i] = new CustomVertexInfo(newVec, colorInfo.color with { A = (byte)((1 - f).HillFactor2(1) * 255 * progress) }, new Vector3(1 - f, 1, alphaLight));//(byte)(_f * 255)//drawCen + 
                vertexInfos[2 * i + 1] = new CustomVertexInfo(default, colorInfo.color with { A = 0 }, new Vector3(0, 0, alphaLight));//drawCen
            }

            foreach (var swoosh in ultraSwooshes)
            {
                if (swoosh != null && swoosh.Active)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        var f = i / 29f;
                        var num = 1 - swoosh.timeLeft / (float)swoosh.timeLeftMax;
                        var lerp = f.Lerp(instance.IsCloseAngleFade ? num : 0, 1);//num
                        //float theta2 = (1.8375f * lerp - 1.125f) * MathHelper.Pi + MathHelper.Pi;
                        float theta2 = (1.8375f * lerp * swoosh.rotationVelocity - 1.125f) * MathHelper.Pi + MathHelper.Pi;
                        if (swoosh.negativeDir) theta2 = MathHelper.TwoPi - theta2;
                        Vector2 offsetVec = -2 * (theta2.ToRotationVector2() * new Vector2(swoosh.xScaler * (instance.IsHorizontallyGrow ? (1 + num) : 1), 1)).RotatedBy(swoosh.rotation) * swoosh.scaler * (instance.IsExpandGrow ? (1 + num * (instance.growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest ? 0.125f : 0.25f)) : 1);
                        Vector2 adder = (offsetVec * 0.25f + swoosh.rotation.ToRotationVector2() * scaler * 2f) * (instance.IsOffestGrow ? num : 0);
                        if (instance.growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest) adder *= 0.25f;
                        var realColor = swoosh.color;
                        realColor.A = (byte)((1 - f).HillFactor2(1) * (instance.IsTransparentFade ? (1 - num) : 1) * 255);
                        swoosh.vertexInfos[2 * i] = new CustomVertexInfo(swoosh.center + offsetVec + adder, realColor, new Vector3(1 - f, 1, alphaLight));
                        realColor.A = 0;
                        swoosh.vertexInfos[2 * i + 1] = new CustomVertexInfo(swoosh.center + adder, realColor, new Vector3(0, 0, alphaLight));
                    }
                }
            }

        }
        public void UpdateSwooshHM()
        {
            foreach (var us in ultraSwooshes)
            {
                if (us != null && us.Active)
                {
                    CoolerItemVisualEffect.UpdateHeatMap(ref us.heatMap, us.hsl, ConfigurationSwoosh);
                }
            }
        }
        ////public ConfigurationSwoosh ConfigurationSwoosh
        ////{
        ////    get => configurationSwoosh ??= (Main.myPlayer == player.whoAmI ? instance : new ConfigurationSwoosh());//ConfigurationSwoosh.instance
        ////    set => configurationSwoosh = value;
        ////}
        public ConfigurationSwoosh_Advanced ConfigurationSwoosh
        {
            get
            {
                if (configurationSwoosh == null)
                {
                    configurationSwoosh = Main.myPlayer == player.whoAmI ? ConfigSwooshInstance : new ConfigurationSwoosh_Advanced();
                }
                return configurationSwoosh;
            }
            set => configurationSwoosh = value;
        }
        Player player => Player;
        float factor;
        public void UpdateFactor()
        {
            var _factor = (float)player.itemAnimation / (player.itemAnimationMax - 1);//物品挥动程度的插值，这里应该是从1到0
            const float cValue = 3f;
            float fac;
            //int speed = 0;
            //int type = 0;
            //for (int n = 1; n < Main.maxItemTypes; n++) 
            //{
            //    var item = new Item();
            //    item.SetDefaults(n);
            //    if (item.useStyle == 1 && item.useAnimation > speed && item.DamageType == DamageClass.Melee && item.noUseGraphic == false) { speed = item.useAnimation; type = n; }
            //}
            //var _item = new Item();
            //_item.SetDefaults(type);
            //Main.NewText((speed, _item.Name, _item.type));
            //Main.NewText(player.itemAnimationMax);
            switch (ConfigurationSwoosh.swooshActionStyle)
            {
                case SwooshAction.左右横劈: fac = ((float)Math.Sqrt(_factor) + _factor) * .5f; break;
                case SwooshAction.旋风劈:
                case SwooshAction.左右横劈_后倾:
                default:
                    {
                        if (player.itemAnimationMax > 18)
                        {
                            _factor = player.itemAnimation <= 9 ? (player.itemAnimation / 18f) : (((player.itemAnimation - 9f) / (player.itemAnimationMax - 9f) + 1f) / 2f);
                            fac = 1 - (cValue - 1) * (1 - _factor) * (1 - _factor) - (2 - cValue) * (1 - _factor);
                        }
                        else
                        {
                            float n = player.itemAnimationMax / 6f;
                            if (n < 1) n = 1;
                            fac = 1 - (n - 1) * (1 - _factor) * (1 - _factor) - (2 - n) * (1 - _factor);
                        }
                    }
                    break;
                case SwooshAction.左右横劈_后倾_旧: fac = 1 - (cValue - 1) * (1 - _factor) * (1 - _factor) - (2 - cValue) * (1 - _factor); break;
                case SwooshAction.左右横劈_失败:
                    {
                        float n = player.itemAnimationMax / 5f;
                        if (n < 1) n = 1;
                        fac = 1 - (n - 1) * (1 - _factor) * (1 - _factor) - (2 - n) * (1 - _factor);
                        if (n > 3) fac = MathHelper.Clamp(fac, 0, 1);
                    }
                    break;
                    //case SwooshAction.摇摆重击: //失败品
                    //    if (player.itemAnimationMax != lastAnimationMax)
                    //    {
                    //        lastAnimationMax = player.itemAnimationMax;
                    //        const float k = 15f;
                    //        var m = lastAnimationMax * 3;
                    //        var n = (m + k) * .5f;
                    //        try
                    //        {
                    //            a = 1 / (m * n * k);
                    //            b = -(m + n + k) * a;
                    //            c = (3 - (float)(Math.Pow(k, 3) + Math.Pow(m, 3) + Math.Pow(n, 3)) * a - (float)(Math.Pow(k, 2) + Math.Pow(m, 2) + Math.Pow(n, 2)) * b) / (k + m + n);
                    //        }
                    //        catch { }
                    //    }
                    //    var t = (lastAnimationMax * 3 - 30) / 60f;
                    //    t = MathHelper.Clamp(t, 0, 1);
                    //    factor *= lastAnimationMax * 3;//m*x
                    //    factor = (float)(a * Math.Pow(factor, 3) + b * Math.Pow(factor, 2) + c * factor);//q(x) = f(m*x)
                    //    fac = 4 * factor / (3 * factor + 1);
                    //    fac = MathHelper.Lerp(factor, fac, t);
                    //    break;
            }
            factor = fac;
        }
        public float FactorGeter
        {
            get
            {
                if (!Main.gamePaused) UpdateFactor();
                return factor;
            }
        }
        //AnimationMax决定着函数的形状♂
        //但是显然，我们需要把这些复杂的东西在变化的时候存着。
        //public int lastAnimationMax;
        ////给三次函数f用的几个系数
        //public float a;
        //public float b;
        //public float c;
        public bool IsMeleeBroadSword => CoolerItemVisualEffect.MeleeCheck(player.HeldItem.DamageType) || ConfigurationSwoosh.ignoreDamageType;
        public float strengthOfShake;
        public override void ModifyScreenPosition()
        {
            //player.HeldItem.damage > 0 && player.HeldItem.useStyle == ItemUseStyleID.Swing && player.itemAnimation > 0 && player.HeldItem.DamageType == DamageClass.Melee && !player.HeldItem.noUseGraphic && ConfigSwooshInstance.CoolerSwooshActive && !Main.gamePaused && (!ConfigSwooshInstance.ToolsNoUseNewSwooshEffect || player.HeldItem.axe == 0 && player.HeldItem.hammer == 0 && player.HeldItem.pick == 0) || player.HeldItem.type == ItemID.Zenith && player.itemAnimation > 0 && ConfigSwooshInstance.allowZenith
            if (UseSlash)
            {
                //var fac = FactorGeter;
                //fac *= 4 * (1 - fac);
                //fac = MathHelper.Clamp(2 * fac - 1, 0, 1);
                //Main.screenPosition += Main.rand.NextVector2Unit() * fac * 24 * ConfigurationSwoosh.shake * (swingCount % 3 == 0 ? 3 : 1);
                strengthOfShake *= 0.8f;
                if (strengthOfShake < 0.25f) strengthOfShake = 0;
                Main.screenPosition += Main.rand.NextVector2Unit() * strengthOfShake * 24 * ConfigurationSwoosh.shake;
            }
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

            //Main.NewText($"这事颜色{colorInfo.color}  {hsl}", colorInfo.color);
            //TODO 整出新的热度图生成模式 修复alphablend判定bug 修复无法出现黑色的bug
            if (Player.HeldItem.type.SpecialCheck())
            {
                if (ConfigurationSwoosh.allowZenith && ConfigurationSwoosh.CoolerSwooshActive)
                {
                    Player.HeldItem.noUseGraphic = false;
                    Player.HeldItem.useStyle = 1;
                    Player.HeldItem.channel = false;
                    Player.HeldItem.noMelee = false;
                }
                else
                {
                    Player.HeldItem.noUseGraphic = true;
                    Player.HeldItem.useStyle = 5;
                    Player.HeldItem.channel = true;
                    Player.HeldItem.noMelee = true;

                }
                //Main.NewText(player.HeldItem.noUseGraphic);
            }
            var flag = player.HeldItem.damage > 0 && player.HeldItem.useStyle == ItemUseStyleID.Swing && IsMeleeBroadSword && !player.HeldItem.noUseGraphic;
            flag |= player.HeldItem.type.SpecialCheck() && ConfigurationSwoosh.allowZenith;
            if (flag && player.itemAnimation > 0) swooshTimeCounter = 0; else swooshTimeCounter++;
            if (swooshTimeCounter >= 15)
            {
                swooshTimeCounter = 0;
                swingCount = 0;
                SetActionValue();
                SetActionSpeed();
            }
            if ((player.itemAnimation == player.itemAnimationMax || player.itemAnimation == 0) && lastItemAnimation == 1 && flag && ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra && HitboxPosition != default)
            {

                SetActionSpeed();

                for (int n = 0; n < 60; n++)
                {
                    var ultra = ultraSwooshes[n];
                    if (ultra == null || !ultra.Active)
                    {
                        if (ultra == null) ultra = ultraSwooshes[n] = new UltraSwoosh();
                        if (!ultra.Active)
                        {
                            ultra.color = colorInfo.color;
                            ultra.type = colorInfo.type;
                            //Main.NewText(new Item(ultra.type).Name);
                            ultra.checkAirFactor = colorInfo.checkAirFactor;
                            ultra.rotationVelocity = ConfigurationSwoosh.swooshActionStyle == SwooshAction.旋风劈 && swingCount % 3 == 0 ? ConfigurationSwoosh.rotationVelocity : 1f;//
                            ultra.timeLeftMax = ultra.timeLeft = (byte)ConfigurationSwoosh.swooshTimeLeft;
                            ultra.scaler = scaler;
                            ultra.center = player.Center;
                            ultra.heatMap = colorInfo.tex;
                            ultra.negativeDir = negativeDir;
                            ultra.hsl = hsl;
                            ultra.rotation = rotationForShadow;
                            ultra.xScaler = kValue;
                            currentSwoosh = ultra;
                        }
                        break;
                    }
                }
            }
            lastItemAnimation = player.itemAnimation;
            //Main.NewText(player.HeldItem.noUseGraphic);
            if (player.itemAnimation == player.itemAnimationMax && player.itemAnimation > 0)
            {
                //var flag = player.HeldItem.damage > 0 && player.HeldItem.useStyle == ItemUseStyleID.Swing && player.HeldItem.DamageType == DamageClass.Melee && !player.HeldItem.noUseGraphic;
                //flag |= player.HeldItem.type.SpecialCheck() && ConfigurationSwoosh.allowZenith;
                if (flag)
                {
                    if (ConfigurationSwoosh.CoolerSwooshActive) // 
                    {
                        //Main.NewText(swingCount);
                        if (player.itemAnimationMax > 9)
                            ItemID.Sets.SkipsInitialUseSound[player.HeldItem.type] = true;
                        if (Main.myPlayer == player.whoAmI)
                            ChangeShooshStyle();
                    }
                    else
                    {
                        ItemID.Sets.SkipsInitialUseSound[player.HeldItem.type] = false;
                    }
                }
            }

            if (player.itemAnimation == 9 && ItemID.Sets.SkipsInitialUseSound[player.HeldItem.type])
            {
                SoundEngine.PlaySound(player.HeldItem.UseSound, player.Center);
            }
            if (player.itemAnimation > 0 && UseSlash)
            {
                player.itemRotation = direct - MathHelper.ToRadians(90f); // 别问为啥-90°，问re去
                //Main.NewText("!!!!!");
                player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            }
            for (int n = 0; n < 60; n++)
            {
                if (ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra)
                {
                    var ultra = ultraSwooshes[n];
                    if (ultra != null && ultra.Active)
                    {
                        ultra.timeLeft--;
                    }
                }
                else
                {
                    ultraSwooshes[n] = null;
                }

            }
        }
        public override float UseSpeedMultiplier(Item item) => ConfigurationSwoosh.actionOffsetSpeed ? actionOffsetSpeed : 1f;
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (ConfigurationSwoosh.actionModifyEffect)
            {
                damage = (int)(damage * actionOffsetDamage);
                strengthOfShake = actionOffsetDamage * Main.rand.NextFloat(0.85f, 1.15f);
                knockback = (int)(damage * actionOffsetKnockBack);
                var _crit = player.GetWeaponCrit(item);
                _crit += actionOffsetCritAdder;
                _crit = (int)(_crit * actionOffsetCritMultiplyer);
                crit = Main.rand.Next(100) < _crit;
            }
        }
        private void SetActionValue(float size = 1f /*,float speed = 1f*/, float knockBack = 1f, float damage = 1f, int critAdder = 0, float critMultiplyer = 1f)
        {
            actionOffsetSize = size;
            //actionOffsetSpeed = speed;
            actionOffsetDamage = damage;
            actionOffsetCritAdder = critAdder;
            actionOffsetCritMultiplyer = critMultiplyer;
        }
        private void SetActionSpeed()
        {
            var action = ConfigurationSwoosh.swooshActionStyle;
            switch (action)
            {
                case SwooshAction.左右横劈:
                case SwooshAction.左右横劈_后倾:
                case SwooshAction.左右横劈_后倾_旧:
                case SwooshAction.左右横劈_失败:
                default:
                    actionOffsetSpeed = 1f;
                    break;
                case SwooshAction.重斩:
                    actionOffsetSpeed = 0.5f;
                    break;
                case SwooshAction.上挑:
                    actionOffsetSpeed = 0.5f;
                    break;
                case SwooshAction.腾云斩:
                    CloudSpeed((swingCount + 1) % 3);
                    break;
                case SwooshAction.旋风劈:
                    WindSpeed((swingCount + 1) % 3);
                    break;
                case SwooshAction.流雨断:
                    RainSpeed((swingCount + 1) % 8);
                    break;
                case SwooshAction.鸣雷刺:
                    ThunderSpeed((swingCount + 1) % 4);
                    break;
                case SwooshAction.风暴灭却剑:
                    //雷雨风云 雨雷云风
                    var sc = (swingCount + 1) % 36;
                    if (sc < 4) ThunderSpeed(sc);
                    else if (sc < 12) RainSpeed(sc - 4);
                    else if (sc < 15) WindSpeed(sc - 12);
                    else if (sc < 18) CloudSpeed(sc - 15);
                    else if (sc < 26) RainSpeed(sc - 18);
                    else if (sc < 30) ThunderSpeed(sc - 26);
                    else if (sc < 33) CloudSpeed(sc - 30);
                    else WindSpeed(sc - 33);
                    break;
            }
        }
        private void CloudSet(int counter, out float _newKValue)
        {
            var flag = counter == 2;
            negativeDir = flag ^ player.direction == -1;
            _newKValue = flag ? Main.rand.NextFloat(0.5f, .8f) : Main.rand.NextFloat(1, 1.2f);
            SetActionValue
                (
                    flag ? Main.rand.NextFloat(1f, 1.25f) : Main.rand.NextFloat(1f, 1.5f),
                    /*flag ? 0.33f : 0.5f,*/ flag ? 2f : 1.5f, flag ? 2.5f : 2, 4, 1.05f
                );
        }
        private void WindSet(int counter, out float _newKValue)
        {
            negativeDir ^= true;
            _newKValue = Main.rand.NextFloat(1, 2);
            var flag = counter == 2;
            if (flag)
                SetActionValue
                (
                    1.5f,
                    /*0.33f,*/ 1.5f, 2, 6, 1.1f
                );
            else SetActionValue();
        }
        private void RainSet(int counter, out float _newKValue)
        {
            if (counter < 3)
            {
                negativeDir = player.direction == -1;
                _newKValue = Main.rand.NextFloat(1, 1.2f);
                SetActionValue
                    (
                        Main.rand.NextFloat(1f, 1.5f),
                        /*0.5f,*/ 1.5f, 2, 4, 1.05f
                    );
            }
            else
            {
                negativeDir = counter % 2 == 1 ^ player.direction == -1;
                _newKValue = counter == 7 ? Main.rand.NextFloat(2, 3f) : Main.rand.NextFloat(1.5f, 2f);
                if (counter == 7)
                    SetActionValue
                        (
                            Main.rand.NextFloat(2, 3),
                            //0.25f,
                            2f,
                            3,
                            6,
                            1.1f
                        );
                else SetActionValue();
            }
        }
        private void ThunderSet(int counter, out float _newKValue)
        {
            negativeDir ^= true;
            _newKValue = counter == 3 ? Main.rand.NextFloat(4f, 7f) : Main.rand.NextFloat(3f, 4.5f);
            SetActionValue
                (
                    counter % 4 == 3 ? 2f : 1.25f,
                    //1.5f,
                    .75f,
                    0.8f,
                    2,
                    .9f
                );
        }
        private void CloudSpeed(int counter) => actionOffsetSpeed = counter == 2 ? 0.33f : 0.5f;
        private void WindSpeed(int counter) => actionOffsetSpeed = counter == 2 ? 0.33f : 1f;
        private void RainSpeed(int counter) => actionOffsetSpeed = counter < 3 ? .5f : (counter == 7 ? 0.25f : 1f);
        private void ThunderSpeed(int counter) => actionOffsetSpeed = 1.5f;
        public void ChangeShooshStyle()
        {
            var vec = Main.MouseWorld - player.Center;
            vec.Y *= player.gravDir;
            player.direction = Math.Sign(vec.X);

            var action = ConfigurationSwoosh.swooshActionStyle;
            float _newKValue;
            switch (action)
            {
                case SwooshAction.左右横劈:
                case SwooshAction.左右横劈_后倾:
                case SwooshAction.左右横劈_后倾_旧:
                case SwooshAction.左右横劈_失败:
                default:
                    negativeDir ^= true;
                    _newKValue = Main.rand.NextFloat(1, 2);
                    SetActionValue();
                    break;
                case SwooshAction.重斩:
                    negativeDir = player.direction == -1;
                    _newKValue = Main.rand.NextFloat(1, 1.2f);
                    SetActionValue(Main.rand.NextFloat(1f, 1.5f)/*, .5f*/, 1.5f, 2, 4, 1.05f);
                    break;
                case SwooshAction.上挑:
                    negativeDir = player.direction == 1;
                    _newKValue = Main.rand.NextFloat(0.5f, .8f);
                    SetActionValue(Main.rand.NextFloat(1f, 1.25f)/*, .5f*/, 1.5f, 2, 4, 1.05f);
                    break;
                case SwooshAction.腾云斩:
                    CloudSet(swingCount % 3, out _newKValue);
                    break;
                case SwooshAction.旋风劈:
                    WindSet(swingCount % 3, out _newKValue);
                    break;
                case SwooshAction.流雨断:
                    RainSet(swingCount % 8, out _newKValue);
                    break;
                case SwooshAction.鸣雷刺:
                    ThunderSet(swingCount % 4, out _newKValue);
                    break;
                case SwooshAction.风暴灭却剑:
                    //雷雨风云 雨雷云风
                    var sc = swingCount % 36;
                    if (sc < 4) ThunderSet(sc, out _newKValue);
                    else if (sc < 12) RainSet(sc - 4, out _newKValue);
                    else if (sc < 15) WindSet(sc - 12, out _newKValue);
                    else if (sc < 18) CloudSet(sc - 15, out _newKValue);
                    else if (sc < 26) RainSet(sc - 18, out _newKValue);
                    else if (sc < 30) ThunderSet(sc - 26, out _newKValue);
                    else if (sc < 33) CloudSet(sc - 30, out _newKValue);
                    else WindSet(sc - 33, out _newKValue);
                    break;
            }


            if (ConfigSwooshInstance.swooshFactorStyle == SwooshFactorStyle.系数中间插值)
            {
                rotationForShadow = rotationForShadowNext;
                rotationForShadowNext = vec.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
                if (rotationForShadow == 0) rotationForShadow = vec.ToRotation();
            }
            else
            {
                rotationForShadow = vec.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
            }

            swingCount++;

            if (ConfigSwooshInstance.swooshFactorStyle == SwooshFactorStyle.系数中间插值)
            {
                kValue = kValueNext;
                kValueNext = _newKValue;
                if (kValue == 0) kValue = kValueNext;
            }
            else
            {
                kValue = _newKValue;
            }



            //string str = "";
            //switch (Main.rand.Next(15))
            //{
            //    case 0: str = "泰拉瑞亚:每挥动一次换一次标题"; break;
            //    case 1: str = "泰拉瑞亚:试试阅读《毛泽东选集》"; break;
            //    case 2: str = "泰拉瑞亚:全世界无产者联合起来"; break;
            //    case 3: str = "泰拉瑞亚:无间之钟无尽之夜"; break;
            //    case 4: str = "泰拉瑞亚:听风雪喧嚷，看流星在飞翔"; break;
            //    case 5: str = "幻世边境:完了泰拉成替身了"; break;
            //    case 6: str = "泰拉瑞亚:风暴之城建设中"; break;
            //    case 7: str = "泰拉瑞亚:东方太阳，正在升起"; break;
            //    case 8: str = "泰拉瑞亚:无垠星空与时间洪流"; break;
            //    case 9: str = "泰拉瑞亚:你打算在这幻界逗留多久呢?"; break;
            //    case 10: str = "泰拉瑞亚:阿汪居然塞私货，太可恶了！！"; break;
            //    case 11: str = "幻世边境:第一回 风暴城辉光重现，赤康邦暗流涌动。";break;
            //    case 12: str = "幻世边境:第二回 星之河孤帆渺渺，时之风独旅茫茫。"; break;
            //    case 13: str = "幻世边境:第三回 虚幻界醉生梦死，现实境山重水复。"; break;
            //    case 14: str = "幻世边境:第四回 无间狱暮雾蒙蒙，人间世长夜漫漫。"; break;

            //}
            if (!ConfigurationNormal.instance.DontChangeMyTitle)
                Main.instance.Window.Title = Language.GetTextValue("Mods.CoolerItemVisualEffect.StrangeTitle." + Main.rand.Next(11));//"幻世边境：完了泰拉成替身了";//"{$Mods.CoolerItemVisualEffect.StrangeTitle." + Main.rand.Next(15)+"}"//15

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
                packet.Write((byte)HandleNetwork.MessageType.BasicStats);
                packet.Write(negativeDir);
                packet.Write(rotationForShadow);
                packet.Write(rotationForShadowNext);
                packet.Write(swingCount);
                packet.Write(kValue);
                packet.Write(kValueNext);
                packet.Write(UseSlash);
                packet.Write(actionOffsetSize);
                packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); // 同步direction
            }
        }
        public float RealSize => ConfigurationSwoosh.swooshSize * (ConfigurationSwoosh.actionOffsetSize ? actionOffsetSize : 1);
        //public int ScalerOfSword
        //{
        //    get
        //    {
        //        if (scaler > 15) 
        //        {
        //            return scaler - 16;
        //        }
        //        return scaler;
        //    }
        //}
        public override void ResetEffects()
        {


            //if (player.controlUseItem || (player.controlUseTile && player.altFunctionUse == 2) || (scaler > 0 && scaler < 15))
            //{
            //    scaler++;
            //}
            //else if (scaler > 15)
            //{
            //    scaler--;
            //}
            //if (scaler == 15)
            //{
            //    scaler = 31;
            //}
            //if (scaler == 16)
            //{
            //    scaler = 0;
            //}


            //scaler = (int)MathHelper.Clamp(scaler, 0, 31);
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
            if (ConfigurationSwoosh.showHeatMap && colorInfo.tex != null && !Main.gameMenu)
                Main.spriteBatch.Draw(colorInfo.tex, player.Center - Main.screenPosition + new Vector2(-760, -340), null, Color.White, 0, new Vector2(150, .5f), new Vector2(1, 50f), SpriteEffects.None, 0);

            //Main.spriteBatch.Draw(colorInfo.tex, new Rectangle(200, 200, 300, 50), Color.White);
            //HitboxPosition = Vector2.Zero;//重置
            //Main.spriteBatch.DrawString(FontAssets.MouseText.Value, player.isFirstFractalAfterImage.ToString(), Player.Center - new Vector2(0, 64) - Main.screenPosition, Color.Red);
            //这个写法可以让绘制的东西在人物旋转后保持原来与人物的相对位置(试做的武器显示)
            if (ConfigurationNormal.instance.useWeaponDisplay)
            {
                if (Main.gameMenu && ConfigurationNormal.instance.firstWeaponDisplay)//
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
            DrawData item = new DrawData(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, ConfigurationNormal.instance.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
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
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationNormal.instance.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
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
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationNormal.instance.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
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