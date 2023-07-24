using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using static CoolerItemVisualEffect.ConfigurationSwoosh;
using Terraria.ID;
using System.Reflection;
using Terraria.GameContent.Drawing;
using LogSpiralLibrary;
using Terraria;
using System.IO;
using LogSpiralLibrary.CodeLibrary;

namespace CoolerItemVisualEffect
{
    public class CoolerSwoosh : UltraSwoosh
    {
        public int type;
        public float checkAirFactor;
        public float rotationVelocity;
        public Vector3 hsl;
        public CoolerItemVisualEffectPlayer modPlr;
        public override IRenderDrawInfo[] RenderDrawInfos => new IRenderDrawInfo[] { };//new EmptyEffectInfo()
        public override void Uptate()
        {
            var drawPlayer = modPlr.Player;
            var instance = modPlr.ConfigurationSwoosh;
            var alphaLight = hsl.Z < instance.isLighterDecider ? Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).R / 255f * .5f : 0.5f;
            for (int i = 0; i < 30; i++)
            {
                var f = i / 29f;
                var num = 1 - factor;
                var lerp = f.Lerp(instance.IsCloseAngleFade ? num : 0, 1);//num
                                                                          //float theta2 = (1.8375f * lerp - 1.125f) * MathHelper.Pi + MathHelper.Pi;
                float theta2 = ((angleRange.to - angleRange.from) * lerp * rotationVelocity + angleRange.from) * MathHelper.Pi + MathHelper.Pi;
                if (negativeDir) theta2 = MathHelper.TwoPi - theta2;
                Vector2 offsetVec = -2 * (theta2.ToRotationVector2() * new Vector2(xScaler * (instance.IsHorizontallyGrow ? (1 + num) : 1), 1)).RotatedBy(rotation) * scaler * (instance.IsExpandGrow ? (1 + num * (instance.growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest ? 0.125f : 0.25f)) : 1);
                Vector2 adder = (offsetVec * 0.25f + rotation.ToRotationVector2() * scaler * 2f) * (instance.IsOffestGrow ? num : 0);
                if (instance.growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest) adder *= 0.25f;
                var realColor = color.Invoke(f);
                realColor.A = (byte)((1 - f).HillFactor2(1) * (instance.IsTransparentFade ? MathF.Sqrt(1 - num) : 1) * 255);
                VertexInfos[2 * i] = new CustomVertexInfo(center + offsetVec + adder, realColor, new Vector3(1 - f, 1, alphaLight));
                //realColor.A = 0;
                VertexInfos[2 * i + 1] = new CustomVertexInfo(center + adder, realColor, new Vector3(0, 0, alphaLight));
            }
        }
        public override void PreDraw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, RenderTarget2D render, RenderTarget2D renderAirDistort)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="contextArgument">难得用到的上下文参数，这里是缩放大小</param>
        public override void Draw(SpriteBatch spriteBatch, IRenderDrawInfo renderDrawInfo, params object[] contextArgument)
        {
            LogSpiralLibraryMod.ShaderSwooshUL.Parameters["airFactor"].SetValue(checkAirFactor);
            Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.Item[type].Value;
            Main.graphics.GraphicsDevice.Textures[3] = heatMap;
            LogSpiralLibraryMod.ShaderSwooshUL.Parameters["lightShift"].SetValue(modPlr.ConfigurationSwoosh.IsDarkFade ? factor - 1f : 0);
            LogSpiralLibraryMod.ShaderSwooshUL.CurrentTechnique.Passes[7].Apply();
            DrawPrimitives((float)contextArgument[0]);
        }
        public override void PostDraw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, RenderTarget2D render, RenderTarget2D renderAirDistort)
        {

        }
    }
    public class CoolerItemVisualEffectPlayer : ModPlayer
    {
        #region 基本量声明
        ConfigurationSwoosh configurationSwoosh;
        public ConfigurationSwoosh ConfigurationSwoosh
        {
            get
            {
                if (configurationSwoosh == null)
                {
                    configurationSwoosh = Main.myPlayer == player.whoAmI ? ConfigSwooshInstance : new ConfigurationSwoosh();
                }
                return configurationSwoosh;
            }
            set => configurationSwoosh = value;
        }
        Player player => Player;
        /// <summary>
        /// 刀光碰撞箱相对玩家的坐标，为了适配联机把原来写的改了一下
        /// </summary>
        public Vector2 HitboxPosition = Vector2.Zero;
        /// <summary>
        /// 该玩家是否使用斩击特效，为了联机同步写的
        /// </summary>
        public bool UseSlash;
        public bool IsMeleeBroadSword => CoolerItemVisualEffectMod.MeleeCheck(player.HeldItem.DamageType) || ConfigurationSwoosh.ignoreDamageType;
        public float TimeToCutThem => ConfigurationSwoosh.swingAttackTime * 2;//8f
        /// <summary>
        /// 剑气是否可用
        /// </summary>
        public bool SwooshActive
        {
            get
            {
                bool flag = false;
                foreach (var ultra in coolerSwooshes)
                {
                    if (ultra != null && ultra.Active) { flag = true; break; }
                }
                return flag && ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra;
            }

        }
        #endregion

        #region 视觉效果修改部分
        public float kValue;
        public bool negativeDir;
        public bool oldNegativeDir;
        public float rotationForShadow;
        public float kValueNext;
        public float rotationForShadowNext;
        //public int testState;
        public int swingCount;
        public (Texture2D tex, Color color, float checkAirFactor, int type) colorInfo;
        public Vector3 hsl;
        public float direct;
        public (Vector2 u, Vector2 v) vectors;
        public readonly CustomVertexInfo[] vertexInfos = new CustomVertexInfo[90];
        public readonly CoolerSwoosh[] coolerSwooshes = new CoolerSwoosh[60];
        //public CoolerSwoosh[] coolerSwooshes => (from swoosh in ultraSwooshes where true select swoosh as CoolerSwoosh).ToArray();
        public CoolerSwoosh currentSwoosh;
        #endregion

        #region 实际效果修改部分
        public float actionOffsetSize;
        public float actionOffsetSpeed;
        public float actionOffsetKnockBack;
        public float actionOffsetDamage;
        public int actionOffsetCritAdder;
        public float actionOffsetCritMultiplyer;
        public float RealSize => ConfigurationSwoosh.swooshSize * currentSize;
        public float currentSize;
        public float currentRotation;

        /// <summary>
        /// 对NPC攻击判定的修改
        /// <br>这个不是很好用</br>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool? CanHitNPCWithItem(Item item, NPC target)
        {
            //bool? modCanHit = CombinedHooks.CanPlayerHitNPCWithItem(this, sItem, Main.npc[i]);
            var style = ConfigurationSwoosh.hitBoxStyle;
            if (style == 0 || style == HitBoxStyle.矩形Rectangle || ConfigurationSwoosh.coolerSwooshQuality == QualityType.关off || !UseSlash) return null;
            if (style == HitBoxStyle.弹幕Projectile && ConfigurationSwoosh.coolerSwooshQuality != QualityType.关off) return false;

            if (ConfigurationSwoosh.actionModifyEffect)
            {
                if (player.itemAnimation > TimeToCutThem / 2f && (ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra ^ !SwooshActive))// && 
                    return false;
                else
                    player.attackCD = 0;
            }
            var canHit = false;
            if (style == HitBoxStyle.剑气UltraSwoosh)
            {
                if (ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra)
                {
                    foreach (var swoosh in coolerSwooshes)
                    {
                        if (swoosh != null && swoosh.Active)
                        {
                            float _point = 0f;
                            Vector2 unit = swoosh.rotation.ToRotationVector2() * swoosh.scaler * swoosh.xScaler * 2;
                            var num = 1 - swoosh.timeLeft / (float)swoosh.timeLeftMax;
                            Vector2 adder = (unit * 0.25f + swoosh.rotation.ToRotationVector2() * scaler * 4f) * (ConfigurationSwoosh.IsOffestGrow ? num : 0);
                            if (ConfigurationSwoosh.growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest) adder *= 0.25f;
                            Vector2 center = swoosh.center + adder;
                            if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), center - .5f * unit, center + unit, scaler * 2, ref _point))
                            {
                                canHit = true;
                                //Main.NewText("事剑气诶！");
                                break;
                            }
                        }
                    }
                }
                goto mylabel;
            }
            if (style == HitBoxStyle.线状AABBLine) goto mylabel;
            return false;

        mylabel:
            float point = 0f;
            canHit |= Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), player.Center, HitboxPosition + player.Center, 32, ref point);
            return canHit && !target.dontTakeDamage && player.CanNPCBeHitByPlayerOrPlayerProjectile(target) && !target.friendly;
        }

        /// <summary>
        /// 修改使用速度
        /// </summary>
        public override float UseSpeedMultiplier(Item item) => ConfigurationSwoosh.actionOffsetSpeed && UseSlash ? actionOffsetSpeed : 1f;
        /// <summary>
        /// 魔改打击效果
        /// </summary>
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ConfigurationSwoosh.actionModifyEffect)
            {
                modifiers.SourceDamage *= actionOffsetDamage;
                strengthOfShake = actionOffsetDamage * Main.rand.NextFloat(0.85f, 1.15f);
                modifiers.Knockback *= actionOffsetKnockBack;
                var _crit = player.GetWeaponCrit(item);
                _crit += actionOffsetCritAdder;
                _crit = (int)(_crit * actionOffsetCritMultiplyer);
                if (Main.rand.Next(100) < _crit)
                {
                    modifiers.SetCrit();
                }
                else
                {
                    modifiers.DisableCrit();
                }
            }
        }

        private void SetActionValue(float size = 1f /*,float speed = 1f*/, float knockBack = 1f, float damage = 1f, int critAdder = 0, float critMultiplyer = 1f)
        {
            actionOffsetSize = size;
            //actionOffsetSpeed = speed;
            actionOffsetKnockBack = knockBack;
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
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = CoolerItemVisualEffectMod.Instance.GetPacket();
                packet.Write((byte)HandleNetwork.MessageType.ActionOffsetSpeed);
                packet.Write(actionOffsetSpeed);
                packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.myPlayer); // 同步direction
            }
        }
        private void CloudSet(int counter, out float _newKValue)
        {
            var flag = counter == 2;
            oldNegativeDir = negativeDir;
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
            oldNegativeDir = negativeDir;
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
                oldNegativeDir = negativeDir;
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
                oldNegativeDir = negativeDir;
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
            oldNegativeDir = negativeDir;
            negativeDir ^= true;
            _newKValue = counter == 3 ? Main.rand.NextFloat(4f, 7f) : Main.rand.NextFloat(3f, 4.5f);
            SetActionValue
                (
                    counter % 4 == 3 ? 2f : 1.25f,
                    //1.5f,
                    .75f,
                    0.6f,
                    2,
                    .9f
                );
        }
        private void CloudSpeed(int counter) => actionOffsetSpeed = counter == 2 ? 0.33f : 0.5f;
        private void WindSpeed(int counter) => actionOffsetSpeed = counter == 2 ? 0.5f : 1f;
        private void RainSpeed(int counter) => actionOffsetSpeed = counter < 3 ? .5f : (counter == 7 ? 0.25f : 1f);
        private void ThunderSpeed(int counter) => actionOffsetSpeed = 1.5f;

        #endregion

        #region 更新状态
        public int swooshTimeCounter;
        public int lastItemAnimation;
        public float scaler;
        float factor;

        /// <summary>
        /// 不是吧螺线，这点计算量都要省?
        /// </summary>
        public float FactorGeter
        {
            get
            {
                if (!Main.gamePaused) UpdateFactor();
                return factor;
            }
        }
        //↓↓↓寒假的螺线看不懂自己暑假的时候写的什么寄吧
        //AnimationMax决定着函数的形状♂
        //但是显然，我们需要把这些复杂的东西在变化的时候存着。
        //public int lastAnimationMax;
        ////给三次函数f用的几个系数
        //public float a;
        //public float b;
        //public float c;

        /// <summary>
        /// 最核心的，改变使用动作参数
        /// </summary>
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
                    oldNegativeDir = negativeDir;
                    negativeDir ^= true;
                    _newKValue = Main.rand.NextFloat(1, 2);
                    SetActionValue();
                    break;
                case SwooshAction.重斩:
                    oldNegativeDir = negativeDir;
                    negativeDir = player.direction == -1;
                    _newKValue = Main.rand.NextFloat(1, 1.2f);
                    SetActionValue(Main.rand.NextFloat(1f, 1.5f)/*, .5f*/, 1.5f, 2, 4, 1.05f);
                    break;
                case SwooshAction.上挑:
                    oldNegativeDir = negativeDir;
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
            if (!ConfigurationSwoosh.DontChangeMyTitle && player.whoAmI == Main.myPlayer)
                Main.instance.Window.Title = Language.GetTextValue("Mods.CoolerItemVisualEffect.StrangeTitle." + Main.rand.Next(11));//"幻世边境：完了泰拉成替身了";//"{$Mods.CoolerItemVisualEffect.StrangeTitle." + Main.rand.Next(15)+"}"//15

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = CoolerItemVisualEffectMod.Instance.GetPacket();
                packet.Write((byte)HandleNetwork.MessageType.BasicStats);
                packet.Write(negativeDir);
                packet.Write(rotationForShadow);
                packet.Write(rotationForShadowNext);
                packet.Write(swingCount);
                packet.Write(kValue);
                packet.Write(kValueNext);
                packet.Write(UseSlash);
                packet.Write(actionOffsetSize);
                packet.Write(actionOffsetDamage);
                packet.Write(actionOffsetKnockBack);
                packet.Write(actionOffsetCritAdder);
                packet.Write(actionOffsetCritMultiplyer);
                packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.myPlayer); // 同步direction
            }
        }

        /// <summary>
        /// 更新顶点
        /// </summary>
        public void UpdateVertex()
        {
            var drawPlayer = player;
            var instance = ConfigurationSwoosh;
            var alphaLight = hsl.Z < instance.isLighterDecider ? Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).R / 255f * .5f : 0.5f;
            if (UseSlash)
            {
                var modPlayer = this;
                var fac = modPlayer.FactorGeter;
                fac = modPlayer.negativeDir ? 1 - fac : fac;
                float rotVel = instance.swooshActionStyle == SwooshAction.旋风劈 && modPlayer.swingCount % 3 == 0 ? 3 : 1;
                var theta = (1.2375f * fac * rotVel - 1.125f) * MathHelper.Pi;
                var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
                float xScaler = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, fac) : modPlayer.kValue;
                currentSize = MathHelper.Lerp(currentSize, (ConfigurationSwoosh.actionOffsetSize ? actionOffsetSize : 1), 0.2f);
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
                var _dustAllow = player.itemAnimation == 1 && ConfigurationSwoosh.dustQuantity != 0;
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
                    //var progress = _flag ? Utils.GetLerpValue(MathHelper.Clamp(player.itemAnimationMax, TimeToCutThem, 114514), TimeToCutThem, player.itemAnimation, true) : 1f;
                    var progress = _flag ? Utils.GetLerpValue(TimeToCutThem, TimeToCutThem * .5f, player.itemAnimation, true) : 1f;//MathHelper.Clamp(player.itemAnimationMax, TimeToCutThem, 114514)
                    var alphaValue = MathHelper.Clamp((1 - f).HillFactor2() * 2f, 0, 1);
                    if (_dustAllow)
                    {
                        var scaler = MathHelper.Lerp(player.itemAnimationMax / (float)player.HeldItem.useAnimation, 1, .5f);//1 / (ConfigurationSwoosh.actionOffsetSpeed && UseSlash ? actionOffsetSpeed : 1f)
                        if (Main.rand.Next(100) < f.HillFactor2() * 50 * scaler * ConfigurationSwoosh.dustQuantity)
                        {
                            int _num = Main.rand.Next(2, 6);
                            for (int k = 0; k < _num; k++)
                            {
                                var unit = new Vector2(-newVec.Y, newVec.X).SafeNormalize(default) * (negativeDir ? -1 : 1);
                                var dustColor = Color.Lerp(Main.hslToRgb(Vector3.Clamp(hsl * new Vector3(1, ConfigurationSwoosh.saturationScalar, Main.rand.NextFloat(0.85f, 1.15f)), default, Vector3.One)), Color.White, Main.rand.NextFloat(0, 0.3f));
                                Dust dust = Dust.NewDustPerfect(player.Center + newVec * Main.rand.NextFloat(1f, 1.25f), 278, unit, 100, dustColor, 1f);
                                dust.scale = 0.4f + Main.rand.NextFloat(-1, 1) * 0.1f;
                                dust.scale *= scaler;
                                dust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.3f;
                                dust.fadeIn *= .5f * scaler;
                                dust.noGravity = true;
                                dust.velocity += unit * (3f + Main.rand.NextFloat() * 4f) * 2 * scaler;
                            }
                        }
                    }
                    alphaValue *= 255 * progress;
                    vertexInfos[2 * i] = new CustomVertexInfo(newVec, colorInfo.color with { A = (byte)alphaValue }, new Vector3(1 - f, 1, alphaLight));//(byte)(_f * 255)//drawCen + 
                    vertexInfos[2 * i + 1] = new CustomVertexInfo(default, colorInfo.color with { A = (byte)alphaValue }, new Vector3(0, 0, alphaLight));//drawCen
                }
            }
            coolerSwooshes.UpdateVertexInfo();
        }
        /// <summary>
        /// 更新剑气的采样图
        /// </summary>
        public void UpdateSwooshHM()
        {
            foreach (var us in coolerSwooshes)
            {
                if (us != null && us.Active)
                {
                    CoolerItemVisualEffectMod.UpdateHeatMap(ref us.heatMap, us.hsl, ConfigurationSwoosh, TextureAssets.Item[us.type].Value);
                }
            }
        }
        /// <summary>
        /// 更新插值
        /// </summary>
        public void UpdateFactor()
        {
            //Main.NewText((player.itemAnimation, player.itemAnimationMax));
            var _factor = (float)player.itemAnimation / player.itemAnimationMax;//物品挥动程度的插值，这里应该是从1到0
            const float cValue = 3f;
            float fac;
            switch (ConfigurationSwoosh.swooshActionStyle)
            {
                case SwooshAction.左右横劈: fac = ((float)Math.Sqrt(_factor) + _factor) * .5f; break;
                case SwooshAction.旋风劈:
                case SwooshAction.左右横劈_后倾:
                default:
                    {
                        if (player.itemAnimationMax > TimeToCutThem)
                        {
                            if ((negativeDir == oldNegativeDir && swingCount > 0) && player.itemAnimation > TimeToCutThem / 2f)//
                            {
                                var tangent1 = 1f / TimeToCutThem / (player.itemAnimationMax - TimeToCutThem * .5f);
                                fac = MathHelper.Hermite(1.125f, tangent1, 160 / 99f, 0f, Utils.GetLerpValue(TimeToCutThem / 2f, player.itemAnimationMax, player.itemAnimation, true));
                            }
                            else
                            {
                                //_factor = player.itemAnimation <= TimeToCutThem / 2f ? (player.itemAnimation / TimeToCutThem) : (((player.itemAnimation - TimeToCutThem / 2) / (player.itemAnimationMax - TimeToCutThem / 2) + 1f) / 2f);
                                //fac = 1 - (cValue - 1) * (1 - _factor) * (1 - _factor) - (2 - cValue) * (1 - _factor);
                                //↑旧版代码，有点小难受那种
                                float k = TimeToCutThem / 2f;
                                float max = player.itemAnimationMax;
                                float t = player.itemAnimation;
                                if (t >= k)
                                {
                                    fac = MathHelper.SmoothStep(1, 1.125f, Utils.GetLerpValue(max, k, t));
                                }
                                else
                                {
                                    fac = MathHelper.SmoothStep(0, 1.125f, player.itemAnimation / k);
                                }
                            }
                        }
                        else
                        {
                            //float n = player.itemAnimationMax / TimeToCutThem * 3f;
                            //if (n < 1) n = 1;
                            //fac = 1 - (n - 1) * (1 - _factor) * (1 - _factor) - (2 - n) * (1 - _factor);
                            fac = MathHelper.SmoothStep(0, 1.125f, _factor);

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
            }
            factor = fac;
        }

        /// <summary>
        /// 魔改攻击CD
        /// </summary>
        public override void PreUpdate()
        {
            if (ConfigurationSwoosh.actionModifyEffect && player.itemAnimation < TimeToCutThem)
            {
                player.attackCD = 0;
            }
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        public override void PostUpdate()
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<HitAssistProj>()] == 0 &&
                ConfigurationSwoosh.hitBoxStyle == HitBoxStyle.弹幕Projectile &&
                Main.myPlayer == player.whoAmI && UseSlash)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, 0), player.Center, default, ModContent.ProjectileType<HitAssistProj>(), 1, 1, player.whoAmI);
            }
            //base.PostUpdate();
            if (ConfigurationSwoosh.actionModifyEffect && player.itemAnimation < TimeToCutThem)
            {
                player.attackCD = 0;
            }
            //Main.NewText($"这事颜色{colorInfo.color}  {hsl}", colorInfo.color);
            //TODO 整出新的热度图生成模式 修复无法出现黑色的bug 增加剑气像素精度设置(1*1 2*2 3*3...)
            if (Player.HeldItem.type.SpecialCheck())
            {
                if (ConfigurationSwoosh.allowZenith && ConfigurationSwoosh.CoolerSwooshActive)
                {
                    Player.HeldItem.noUseGraphic = false;
                    Player.HeldItem.useStyle = ItemUseStyleID.Swing;
                    Player.HeldItem.channel = false;
                    Player.HeldItem.noMelee = false;
                }
                else
                {
                    Player.HeldItem.noUseGraphic = true;
                    Player.HeldItem.useStyle = ItemUseStyleID.Shoot;
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
            //Main.NewText((swingCount,negativeDir,oldNegativeDir));
            if ((player.itemAnimation == player.itemAnimationMax || player.itemAnimation == 0) && lastItemAnimation == 1 && UseSlash && ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra && HitboxPosition != default)
            {

                SetActionSpeed();

                NewCoolerSwoosh();
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
                        if (player.itemAnimationMax > TimeToCutThem / 2)
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

            if (player.itemAnimation == (int)(TimeToCutThem / 2) && ItemID.Sets.SkipsInitialUseSound[player.HeldItem.type])
            {
                SoundEngine.PlaySound(player.HeldItem.UseSound, player.Center);
            }
            if (player.itemAnimation > 0 && UseSlash)
            {
                player.itemRotation = direct - MathHelper.ToRadians(90f); // 别问为啥-90°，问re去
                //Main.NewText("!!!!!");
                player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
                //player.direction = Math.Sign(Main.MouseWorld.X - player.Center.X);
                player.direction = Math.Sign(MathF.Cos(rotationForShadow));
                if (player.itemAnimation > TimeToCutThem && ConfigurationSwoosh.dustQuantity != 0)
                {
                    var _flag = (byte)ConfigurationSwoosh.swooshActionStyle > 0 && (byte)ConfigurationSwoosh.swooshActionStyle < 9;
                    var progress = _flag ? Utils.GetLerpValue(MathHelper.Clamp(player.itemAnimationMax, TimeToCutThem, 114514), TimeToCutThem, player.itemAnimation, true) : 1f;
                    var scaler = player.itemAnimationMax / (float)player.HeldItem.useAnimation;
                    if (Main.rand.Next(100) < progress * 100 * ConfigurationSwoosh.dustQuantity)
                    {
                        int _num = Main.rand.Next(2, 6);
                        for (int k = 0; k < _num; k++)
                        {
                            var unit = Main.rand.NextVector2Unit();
                            var dustColor = Color.Lerp(Main.hslToRgb(Vector3.Clamp(hsl * new Vector3(1, ConfigurationSwoosh.saturationScalar, Main.rand.NextFloat(0.85f, 1.15f)), default, Vector3.One)), Color.White, Main.rand.NextFloat(0, 0.3f));

                            Dust dust = Dust.NewDustPerfect(player.Center + 128 * progress * unit, 278, -unit, 100, dustColor, 1f);
                            dust.scale = (0.4f + Main.rand.NextFloat(-0.1f, 0.1f)) * scaler;
                            dust.scale *= .5f;
                            dust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.3f;
                            dust.fadeIn *= scaler * .5f;
                            dust.noGravity = true;
                            dust.velocity -= unit * (3f + Main.rand.NextFloat() * 4f) * .5f;
                            dust.velocity *= .5f;
                        }
                    }
                }
                else
                {

                }
            }
            if (player.itemAnimation == 0)
            {
                currentRotation = (player.direction - 1) * MathHelper.PiOver2;
                currentSize = 0;
            }
            for (int n = 0; n < 60; n++)
            {
                if (ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra)
                {
                    var ultra = coolerSwooshes[n];
                    if (ultra != null && ultra.Active)
                    {
                        ultra.timeLeft--;
                    }
                }
                else
                {
                    coolerSwooshes[n] = null;
                }

            }
            if (UseSlash || SwooshActive)
                UpdateVertex();
        }
        #endregion

        #region 震动效果
        public float strengthOfShake;
        public override void ModifyScreenPosition()
        {
            //player.HeldItem.damage > 0 && player.HeldItem.useStyle == ItemUseStyleID.Swing && player.itemAnimation > 0 && player.HeldItem.DamageType == DamageClass.Melee && !player.HeldItem.noUseGraphic && ConfigSwooshInstance.CoolerSwooshActive && !Main.gamePaused && (!ConfigSwooshInstance.ToolsNoUseNewSwooshEffect || player.HeldItem.axe == 0 && player.HeldItem.hammer == 0 && player.HeldItem.pick == 0) || player.HeldItem.type == ItemID.Zenith && player.itemAnimation > 0 && ConfigSwooshInstance.allowZenith
            if (UseSlash || SwooshActive)
            {
                //var fac = FactorGeter;
                //fac *= 4 * (1 - fac);
                //fac = MathHelper.Clamp(2 * fac - 1, 0, 1);
                //Main.screenPosition += Main.rand.NextVector2Unit() * fac * 24 * ConfigurationSwoosh.shake * (swingCount % 3 == 0 ? 3 : 1);
                strengthOfShake *= 0.6f;
                if (strengthOfShake < 0.025f) strengthOfShake = 0;
                Main.screenPosition += Main.rand.NextVector2Unit() * strengthOfShake * 48 * ConfigurationSwoosh.shake;
            }
        }
        #endregion

        #region 辅助函数
        public static void ChangeItemTex(Player player, bool airCheck = false)
        {
            CoolerItemVisualEffectPlayer modPlayer = player.GetModPlayer<CoolerItemVisualEffectPlayer>();
            if (!TextureAssets.Item[player.HeldItem.type].IsLoaded) TextureAssets.Item[player.HeldItem.type] = Main.Assets.Request<Texture2D>("Images/Item_" + player.HeldItem.type, ReLogic.Content.AssetRequestMode.AsyncLoad);
            var texture = TextureAssets.Item[player.HeldItem.type].Value;
            if (modPlayer.colorInfo.type != player.HeldItem.type)
            {
                var w = texture.Width;
                var h = texture.Height;
                var cs = new Color[w * h];

                texture.GetData(cs);
                Vector4 vcolor = default;
                float count = 0;
                modPlayer.colorInfo.checkAirFactor = 1;
                Color target = default;

                for (int n = 0; n < cs.Length; n++)
                {
                    if (cs[n] != default && (n - w < 0 || cs[n - w] != default) && (n - 1 < 0 || cs[n - 1] != default) && (n + w >= cs.Length || cs[n + w] != default) && (n + 1 >= cs.Length || cs[n + 1] != default))
                    {
                        var weight = (float)((n + 1) % w * (h - n / w)) / w / h;
                        vcolor += cs[n].ToVector4() * weight;
                        count += weight;
                    }
                    Vector2 coord = new Vector2(n % w, n / w);
                    coord /= new Vector2(w, h);
                    if (airCheck)
                        if (modPlayer.ConfigurationSwoosh.checkAir && Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && cs[n] != default && target == default)
                        {
                            target = cs[n];
                            modPlayer.colorInfo.checkAirFactor = coord.X;
                        }
                }
                vcolor /= count;
                var newColor = modPlayer.colorInfo.color = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
                modPlayer.hsl = Main.rgbToHsl(newColor);
            }


        }

        /// <summary>
        /// 生成新的剑气
        /// </summary>
        public void NewCoolerSwoosh(
            Color? color = null, int? type = null, float? airFac = null,
            float? rotVel = null, byte? timeLeft = null, float? _scaler = null,
            Vector2? center = null, Texture2D heat = null, bool? _negativeDir = null,
            Vector3? _hsl = null, float? _rotation = null, float? xscaler = null,
            (float, float)? angleRange = null)
        {
            var ultra = UltraSwoosh.NewUltraSwoosh<CoolerSwoosh>(
                color ?? colorInfo.color,
                coolerSwooshes,
                timeLeft ?? (byte)ConfigurationSwoosh.swooshTimeLeft,
                _scaler ?? scaler,
                center ?? player.Center,
                heat ?? colorInfo.tex,
                _negativeDir ?? negativeDir,
                _rotation ?? rotationForShadow,
                xscaler ?? kValue,
                angleRange ?? (-1.125f, 0.7125f));

            ultra.type = type ?? colorInfo.type;
            ultra.checkAirFactor = airFac ?? colorInfo.checkAirFactor;
            ultra.rotationVelocity = rotVel ?? (ConfigurationSwoosh.swooshActionStyle == SwooshAction.旋风劈 && swingCount % 3 == 0 ? 2 : 1f);
            ultra.hsl = _hsl ?? hsl;
            ultra.modPlr = this;
            currentSwoosh = ultra;
        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            player.GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh.SendData(Player.whoAmI, fromWho, toWho, true);
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //var results = from text in LanguageManager.Instance._localizedTexts.Keys where text.Contains("CoolerItemVisualEffect.Configs.ConfigurationSwoosh.coolerSwooshQuality") select text;
            //Main.NewText(results.Count());
            //Main.spriteBatch.DrawString(FontAssets.DeathText.Value, Language.GetTextValue("Mods.CoolerItemVisualEffect.Configs.MeleeSwooshConfigs.glowLight.Label"), new Vector2(200), Color.White);
            //int height = 0;
            //foreach (var item in LogSpiralLibraryMod.ShaderSwooshUL.CurrentTechnique.Passes) 
            //{
            //    Main.spriteBatch.DrawString(FontAssets.MouseText.Value, $"{item.Name},{height / 40}", new Vector2(200, 200 + height), Color.White);
            //    height += 40;
            //}
            if (ConfigurationSwoosh.TeleprotEffectActive && player.HeldItem.type == ItemID.MagicMirror && player.ItemAnimationActive)
            {
                var fac = player.itemAnimation / (float)player.itemAnimationMax;
                var _fac = (fac * 2 % 1).HillFactor2() * (fac < .5f ? .5f : 1f);
                var yscaler = (.25f + fac * (fac - 1)) * 1.5f;
                Main.spriteBatch.Draw(LogSpiralLibraryMod.MagicZone[2].Value, player.Center + new Vector2(0, -128 + 256 * fac) - Main.screenPosition, null, Color.Cyan with { A = 0 } * _fac, 0, new Vector2(150), new Vector2(1, yscaler) * _fac, 0, 0);

                Main.spriteBatch.Draw(LogSpiralLibraryMod.MagicZone[2].Value, player.Center + new Vector2(0, 128 - 256 * fac) - Main.screenPosition, null, Color.Cyan with { A = 0 } * _fac, 0, new Vector2(150), new Vector2(1, yscaler) * _fac, 0, 0);
            }
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
            if (ConfigurationSwoosh.showHeatMap && colorInfo.tex != null && !Main.gameMenu && !drawInfo.headOnlyRender)
            {
                Main.spriteBatch.Draw(colorInfo.tex, player.Center - Main.screenPosition + (player.whoAmI == Main.myPlayer ? new Vector2(-760, -340) : new Vector2(0, -64)), null, Color.White, 0, new Vector2(150, .5f), new Vector2(1, 50f), SpriteEffects.None, 0);
                //Main.spriteBatch.DrawString(FontAssets.MouseText.Value, (actionOffsetSize, actionOffsetSpeed, actionOffsetDamage, actionOffsetKnockBack, actionOffsetCritAdder, actionOffsetCritMultiplyer).ToString(), player.Center - Main.screenPosition + new Vector2(0, -64), Color.White);
            }

            //Main.spriteBatch.DrawLine(player.Center, HitboxPosition, Color.Red, 32, true, -Main.screenPosition);
            //Main.spriteBatch.Draw(colorInfo.tex, new Rectangle(200, 200, 300, 50), Color.White);
            //HitboxPosition = Vector2.Zero;//重置
            //Main.spriteBatch.DrawString(FontAssets.MouseText.Value, player.isFirstFractalAfterImage.ToString(), Player.Center - new Vector2(0, 64) - Main.screenPosition, Color.Red);
            //这个写法可以让绘制的东西在人物旋转后保持原来与人物的相对位置(试做的武器显示)
            if (ConfigurationSwoosh.useWeaponDisplay && !drawInfo.headOnlyRender)
            {
                if (Main.gameMenu && ConfigurationSwoosh.firstWeaponDisplay)//
                {
                    Item firstweapon = null;
                    //Main.NewText(WeaponDisplay.Instance._FirstInventoryItem == null);
                    for (int num2 = 0; num2 <= 58; num2++)
                    {
                        Item weapon = Player.inventory[num2];//num2 == 0 ? WeaponDisplay.Instance._FirstInventoryItem : 
                        if (weapon == null) continue;
                        if (weapon.stack > 0 && (weapon.damage > 0 || weapon.type == ItemID.CoinGun) && weapon.useAnimation > 0 && weapon.useTime > 0 && !weapon.consumable && weapon.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(weapon, Player) && weapon.holdStyle == 0 && weapon.type != ItemID.FlareGun && weapon.type != ItemID.MagicalHarp && weapon.type != ItemID.NebulaBlaze && weapon.type != ItemID.NebulaArcanum && weapon.type != ItemID.TragicUmbrella && weapon.type != ItemID.CombatWrench && weapon.type != ItemID.FairyQueenMagicItem && weapon.type != ItemID.BouncingShield && weapon.type != ItemID.SparkleGuitar)
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
                if (Player.active && !Player.dead && holditem.stack > 0 && (holditem.damage > 0 || holditem.type == ItemID.CoinGun) && holditem.useAnimation > 0 && holditem.useTime > 0 && !holditem.consumable && holditem.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(holditem, Player))
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
        public void DrawWeapon(Player Player, Item holditem, PlayerDrawSet drawInfo)
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

            var animation = Main.itemAnimations[holditem.type];
            if (animation != null)
            {//动态武器
                rectangle = animation.GetFrame(texture, -1);
                origin = animation.GetFrame(texture).Size() * .5f;
            }
            DrawData item = new DrawData(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, ConfigurationSwoosh.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
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
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationSwoosh.weaponScale * holditem.scale, drawInfo.playerEffect, 0);
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

        public override void OnEnterWorld()
        {
            //string content = "";
            //for (int n = 0; n < 5; n++) 
            //{
            //    Type type = n switch
            //    {
            //        0 => typeof(ConfigurationSwoosh.MeleeSwooshConfigs),
            //        1 => typeof(ConfigurationSwoosh.DrawConfigs),
            //        2 => typeof(ConfigurationSwoosh.HeatMapConfigs),
            //        3 => typeof(ConfigurationSwoosh.RenderConfigs),
            //        4 => typeof(ConfigurationSwoosh.OtherConfigs)
            //    };
            //    var fields = type.GetFields();
            //    var props = type.GetProperties();
            //    foreach (var field in fields)
            //    {
            //        content += field.Name + "\n";
            //    }
            //    foreach (var prop in props)
            //    {
            //        content += prop.Name + "\n";
            //    }
            //    content += "\n";
            //}

            //File.WriteAllText("E:/阿吧阿吧.txt", content);
            //base.OnEnterWorld();
        }

    }
    public class HitAssistProj : ModProjectile
    {
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!effectPlayer.SwooshActive)
            {
                if (player.itemAnimation > effectPlayer.TimeToCutThem / 2f)
                    return false;
                if (player.itemAnimation == 0) return false;
            }

            foreach (var swoosh in effectPlayer.coolerSwooshes)
            {
                if (swoosh != null && swoosh.Active)
                {
                    float _point = 0f;
                    Vector2 unit = swoosh.rotation.ToRotationVector2() * swoosh.scaler * swoosh.xScaler * 2;
                    var num = 1 - swoosh.timeLeft / (float)swoosh.timeLeftMax;
                    Vector2 adder = (unit * 0.25f + swoosh.rotation.ToRotationVector2() * effectPlayer.scaler * 4f) * (effectPlayer.ConfigurationSwoosh.IsOffestGrow ? num : 0);
                    if (effectPlayer.ConfigurationSwoosh.growStyle == SwooshGrowStyle.横向扩大与平移BothExpandHorizontallyAndOffest) adder *= 0.25f;
                    Vector2 center = swoosh.center + adder;
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center - .5f * unit, center + unit, effectPlayer.scaler * 2, ref _point))
                    {
                        return true;
                    }
                }
            }
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, effectPlayer.HitboxPosition + player.Center, 32, ref point))
            {
                //Main.NewText(effectPlayer.HitboxPosition);
                return true;

            }
            return false;
        }
        Player player => Main.player[Projectile.owner];
        CoolerItemVisualEffectPlayer effectPlayer => player.GetModPlayer<CoolerItemVisualEffectPlayer>();
        public override void AI()
        {
            if (effectPlayer.ConfigurationSwoosh.hitBoxStyle == HitBoxStyle.弹幕Projectile && (effectPlayer.UseSlash || effectPlayer.SwooshActive))
                Projectile.timeLeft = 4;
            Projectile.damage = player.GetWeaponDamage(player.HeldItem);
            Projectile.knockBack = player.GetWeaponKnockback(player.HeldItem) * effectPlayer.actionOffsetKnockBack;
            Projectile.direction = player.direction;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.friendly = player.itemAnimation == effectPlayer.TimeToCutThem / 2f;
            Projectile.Center = player.Center;
            Projectile.velocity = (Main.MouseWorld - player.Center).SafeNormalize(default);
            if (effectPlayer.SwooshActive)
            {
                foreach (var swoosh in effectPlayer.coolerSwooshes)
                {
                    if (swoosh != null && swoosh.Active && swoosh.timeLeft == swoosh.timeLeftMax - 1 && !Projectile.friendly)
                    {
                        Projectile.friendly = true;
                        break;
                    }
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (effectPlayer.ConfigurationSwoosh.actionModifyEffect)
            {
                modifiers.SourceDamage *= effectPlayer.actionOffsetDamage;
                effectPlayer.strengthOfShake = effectPlayer.actionOffsetDamage * Main.rand.NextFloat(0.85f, 1.15f);
                modifiers.Knockback *= effectPlayer.actionOffsetKnockBack;
                var _crit = player.GetWeaponCrit(player.HeldItem);
                _crit += effectPlayer.actionOffsetCritAdder;
                _crit = (int)(_crit * effectPlayer.actionOffsetCritMultiplyer);
                if (Main.rand.Next(100) < _crit)
                {
                    modifiers.SetCrit();
                }
                else
                {
                    modifiers.DisableCrit();
                }
            }
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 20;
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        //public static MethodBase ApplyNPCOnHitEffects;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //ItemLoader.OnHitNPC(player.HeldItem, player, target, hit, damageDone);
            try
            {
                #region *复杂的伤害计算*
                int num = Projectile.damage;
                var sItem = player.HeldItem;
                var itemRectangle = Utils.CenteredRectangle(player.Center + effectPlayer.HitboxPosition * .5f, new Vector2(Math.Abs(effectPlayer.HitboxPosition.X), Math.Abs(effectPlayer.HitboxPosition.Y)));
                int num2 = Item.NPCtoBanner(target.BannerID());
                if (num2 > 0 && player.HasNPCBannerBuff(num2))
                    num = ((!Main.expertMode) ? ((int)((float)num * ItemID.Sets.BannerStrength[Item.BannerToItem(num2)].NormalDamageDealt)) : ((int)((float)num * ItemID.Sets.BannerStrength[Item.BannerToItem(num2)].ExpertDamageDealt)));

                if (player.parryDamageBuff && sItem.DamageType.CountsAsClass(DamageClass.Melee))
                {
                    num *= 5;
                    player.parryDamageBuff = false;
                    player.ClearBuff(198);
                }

                if (sItem.type == ItemID.BreakerBlade && (float)target.life >= (float)target.lifeMax * 0.9f)
                    num = (int)((float)num * 2f);

                if (sItem.type == ItemID.HamBat)
                {
                    int num3 = 0;
                    if (player.FindBuffIndex(26) != -1)
                        num3 = 1;

                    if (player.FindBuffIndex(206) != -1)
                        num3 = 2;

                    if (player.FindBuffIndex(207) != -1)
                        num3 = 3;

                    float num4 = 1f + 0.05f * (float)num3;
                    num = (int)((float)num * num4);
                }

                if (sItem.type == ItemID.Keybrand)
                {
                    float t = (float)target.life / (float)target.lifeMax;
                    float lerpValue = Utils.GetLerpValue(1f, 0.1f, t, clamped: true);
                    float num5 = 1.5f * lerpValue;
                    num = (int)((float)num * (1f + num5));
                    Vector2 point = itemRectangle.Center.ToVector2();
                    Vector2 positionInWorld = target.Hitbox.ClosestPointInRect(point);
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings
                    {
                        PositionInWorld = positionInWorld
                    }, player.whoAmI);
                }
                #endregion
                player.ApplyNPCOnHitEffects(player.HeldItem, itemRectangle, num, hit.Knockback, target.whoAmI, hit.Damage, damageDone);
                //(ApplyNPCOnHitEffects ??= typeof(Player).GetMethod(nameof(ApplyNPCOnHitEffects), BindingFlags.Instance | BindingFlags.NonPublic))
                //    ?.Invoke(player, new object[] { player.HeldItem, itemRectangle, num, knockback, target.whoAmI, damage, damage });

            }
            catch
            {
                Main.NewText("炸了！");
            }
            if (target.type == NPCID.WallofFlesh || target.type == NPCID.WallofFleshEye || !target.CanBeChasedBy()) return;
            var vec = effectPlayer.HitboxPosition;
            vec = new Vector2(-vec.Y, vec.X) * (effectPlayer.negativeDir ? -1 : 1);
            vec = (vec.SafeNormalize(default) + Projectile.velocity) * MathF.Sqrt(Projectile.knockBack * target.knockBackResist) * .5f;
            if (!target.boss) vec *= 2;
            if (hit.Crit) vec *= 1.5f;
            target.velocity += vec * (hit.Crit ? 1.5f : 1f);
            //Projectile.ai[0] = 1;
            //NetMessage.SendData(MessageID.DamageNPC,)
        }
        public override string Texture => "Terraria/Images/Item_1";
    }
}