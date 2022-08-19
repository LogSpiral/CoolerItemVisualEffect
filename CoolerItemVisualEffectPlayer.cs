using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using static CoolerItemVisualEffect.ConfigurationSwoosh_Advanced;

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
            float rotVel = instance.swooshActionStyle == SwooshAction.两次普通斩击一次高速旋转 && modPlayer.swingCount % 3 == 2 ? instance.rotationVelocity : 1;
            var theta = (1.2375f * fac * rotVel - 1.125f) * MathHelper.Pi;
            var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
            float xScaler = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, fac) : modPlayer.kValue;
            //float scaler = ;
            modPlayer.scaler = itemTex.Size().Length() * drawPlayer.GetAdjustedItemScale(drawPlayer.HeldItem) / xScaler * 0.5f * instance.swooshSize * modPlayer.colorInfo.checkAirFactor;

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
                vectors.u /= ConfigurationSwoosh.swooshSize;
                vectors.v /= ConfigurationSwoosh.swooshSize;
            }
            var theta3 = (1.2375f * swooshAniFac * rotVel - 1.125f) * MathHelper.Pi;
            float xScaler3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, swooshAniFac) : modPlayer.kValue;
            var rotator3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, swooshAniFac) : modPlayer.rotationForShadow;
            var alphaLight = hsl.Z > instance.isLighterDecider ? Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).R / 255f * .6f : 0.6f;

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
                vertexInfos[2 * i] = new CustomVertexInfo(newVec, colorInfo.color with { A = 255 }, new Vector3(1 - f, 1, alphaLight));//(byte)(_f * 255)//drawCen + 
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
                default:
                case SwooshAction.正常挥砍: fac = ((float)Math.Sqrt(_factor) + _factor) * .5f; break;
                case SwooshAction.两次普通斩击一次高速旋转:
                case SwooshAction.向后倾一定角度后重击:
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
                case SwooshAction.向后倾一定角度后重击_旧: fac = 1 - (cValue - 1) * (1 - _factor) * (1 - _factor) - (2 - cValue) * (1 - _factor); break;
                case SwooshAction.向后倾一定角度后重击_失败:
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
        public override void ModifyScreenPosition()
        {
            //player.HeldItem.damage > 0 && player.HeldItem.useStyle == ItemUseStyleID.Swing && player.itemAnimation > 0 && player.HeldItem.DamageType == DamageClass.Melee && !player.HeldItem.noUseGraphic && ConfigSwooshInstance.CoolerSwooshActive && !Main.gamePaused && (!ConfigSwooshInstance.ToolsNoUseNewSwooshEffect || player.HeldItem.axe == 0 && player.HeldItem.hammer == 0 && player.HeldItem.pick == 0) || player.HeldItem.type == ItemID.Zenith && player.itemAnimation > 0 && ConfigSwooshInstance.allowZenith
            if (UseSlash)
            {
                var fac = FactorGeter;
                fac *= 4 * (1 - fac);
                fac = MathHelper.Clamp(2 * fac - 1, 0, 1);
                Main.screenPosition += Main.rand.NextVector2Unit() * fac * 24 * ConfigurationSwoosh.shake * (swingCount % 3 == 2 ? 3 : 1);
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
            var flag = player.HeldItem.damage > 0 && player.HeldItem.useStyle == ItemUseStyleID.Swing && player.HeldItem.DamageType == DamageClass.Melee && !player.HeldItem.noUseGraphic;
            flag |= player.HeldItem.type.SpecialCheck() && ConfigurationSwoosh.allowZenith;
            if ((player.itemAnimation == player.itemAnimationMax || player.itemAnimation == 0) && lastItemAnimation == 1 && flag && ConfigurationSwoosh.coolerSwooshQuality == QualityType.极限ultra && HitboxPosition != default)
            {
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
                            ultra.rotationVelocity = ConfigurationSwoosh.swooshActionStyle == SwooshAction.两次普通斩击一次高速旋转 && swingCount % 3 == 2 ? ConfigurationSwoosh.rotationVelocity : 1f;//
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
        public void ChangeShooshStyle()
        {
            var vec = Main.MouseWorld - player.Center;
            vec.Y *= player.gravDir;
            player.direction = Math.Sign(vec.X);


            negativeDir ^= true;


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
                kValueNext = Main.rand.NextFloat(1, 2);
                if (kValue == 0) kValue = kValueNext;
            }
            else
            {
                kValue = Main.rand.NextFloat(1, 2);
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
                packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); // 同步direction
            }
        }
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
            DrawData item = new DrawData(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, ConfigurationNormal.instance.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
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
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationNormal.instance.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
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
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, ConfigurationNormal.instance.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
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