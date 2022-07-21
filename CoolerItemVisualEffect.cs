global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.Audio;
global using Terraria.DataStructures;
global using Terraria.GameInput;
global using Terraria.ID;
global using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria.GameContent;
using Terraria.GameContent.Skies.CreditsRoll;
using Terraria.UI;
using static Terraria.GameContent.Skies.CreditsRoll.Segments.PlayerSegment;
using static CoolerItemVisualEffect.ConfigurationSwoosh;
using Terraria.Localization;

namespace CoolerItemVisualEffect
{
    public class CoolerItemVisualEffect : Mod
    {
        //public static Vector2 pos = Vector2.Zero;//旧刀光武器绘制位置定位坐标

        //internal static bool ItemAdditive;
        //internal static bool ToolsNoUse = false;
        //internal static float IsLighterDecider = 0.6f;
        //internal static bool UseItemTexForSwoosh = false;

        internal static CoolerItemVisualEffect Instance;

        public override void Load()
        {
            Instance = this;
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_27_HeldItem += PlayerDrawLayers_DrawPlayer_27_HeldItem_WeaponDisplay;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            //On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayerInternal += LegacyPlayerRenderer_DrawPlayerInternal_WD;
            //On.Terraria.GameContent.Skies.CreditsRoll.Segments.PlayerSegment.Draw += PlayerSegment_Draw_WD;
            //CreateRender();
        }
        public Item _FirstInventoryItem;
        //private void PlayerSegment_Draw_WD(On.Terraria.GameContent.Skies.CreditsRoll.Segments.PlayerSegment.orig_Draw orig, Segments.PlayerSegment self, ref CreditsRollInfo info)
        //{
        //    var _targetTime = (int)self.GetType().GetField("_targetTime", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
        //    if ((float)info.TimeInAnimation > (float)_targetTime + self.DedicatedTimeNeeded || info.TimeInAnimation < _targetTime)
        //    {
        //        return;
        //    }
        //    var _player = (Player)self.GetType().GetField("_player", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
        //    _FirstInventoryItem = _player.inventory[0];
        //    //Main.NewText("re我囸你先人");
        //    //orig(self, ref info);
        //    var _anchorOffset = (Vector2)self.GetType().GetField("_anchorOffset", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);

        //    _player.CopyVisuals(Main.LocalPlayer);
        //    _player.position = info.AnchorPositionOnScreen + _anchorOffset;
        //    _player.opacityForCreditsRoll = 1f;
        //    float localTimeForObject = info.TimeInAnimation - _targetTime;
        //    var _actions = (List<ICreditsRollSegmentAction<Player>>)self.GetType().GetField("_actions", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
        //    for (int i = 0; i < _actions.Count; i++)
        //    {
        //        _actions[i].ApplyTo(_player, localTimeForObject);
        //    }
        //    if (info.DisplayOpacity != 0f)
        //    {
        //        _player.ResetEffects();
        //        _player.ResetVisibleAccessories();
        //        _player.UpdateMiscCounter();
        //        _player.UpdateDyes();
        //        _player.PlayerFrame();
        //        _player.socialIgnoreLight = true;
        //        Player player = _player;
        //        player.position += Main.screenPosition;
        //        Player player2 = _player;
        //        player2.position -= new Vector2((float)(_player.width / 2), (float)_player.height);
        //        _player.opacityForCreditsRoll *= info.DisplayOpacity;
        //        Item item = _player.inventory[_player.selectedItem];
        //        //_player.inventory[_player.selectedItem] = new Item();
        //        float num = 1f - _player.opacityForCreditsRoll;
        //        num = 0f;
        //        var _shaderEffect = (IShaderEffect)self.GetType().GetField("_shaderEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
        //        if (_shaderEffect != null)
        //        {
        //            _shaderEffect.BeforeDrawing(ref info);
        //        }
        //        Main.PlayerRenderer.DrawPlayer(Main.Camera, _player, _player.position, 0f, _player.fullRotationOrigin, num);
        //        if (_shaderEffect != null)
        //        {
        //            _shaderEffect.AfterDrawing(ref info);
        //        }
        //        _player.inventory[_player.selectedItem] = item;
        //    }
        //}

        //private void LegacyPlayerRenderer_DrawPlayerInternal_WD(On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.orig_DrawPlayerInternal orig, Terraria.Graphics.Renderers.LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float alpha, float scale, bool headOnly)
        //{
        //    Texture2D tex = TextureAssets.Item[drawPlayer.inventory[0].type].Value;
        //    Vector2 vec = new Vector2(120, 120);
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin();
        //    Main.spriteBatch.Draw(tex, vec,
        //        null, Color.White, 0,
        //        tex.Size() * .5f, 1f, 0, 0);
        //    Main.spriteBatch.DrawString(FontAssets.MouseText.Value, (0, drawPlayer.inventory[0].type).ToString(), vec + new Vector2(0, 16), Color.White);
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin();
        //    orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, alpha, scale, headOnly);
        //}

        private void Main_OnResolutionChanged(Vector2 obj)//在分辨率更改时，重建render防止某些bug
        {
            CreateRender();
        }
        public void CreateRender()
        {
            Render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth == 0 ? 1920 : Main.screenWidth, Main.screenHeight == 0 ? 1120 : Main.screenHeight);
        }
        public RenderTarget2D Render
        {
            get => render ??= new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth == 0 ? 1920 : Main.screenWidth, Main.screenHeight == 0 ? 1120 : Main.screenHeight);
            set
            {
                render = value;
            }
        }

        public RenderTarget2D render;

        public override void Unload()
        {
            Instance = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            HandleNetwork.HandlePacket(reader, whoAmI);
            base.HandlePacket(reader, whoAmI);
        }

        internal static Effect ItemEffect => itemEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ItemGlowEffect").Value;
        internal static Effect ColorfulEffect => colorfulEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ShaderSwooshEffect").Value;
        internal static Effect ShaderSwooshEX => shaderSwooshEX ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ShaderSwooshEffectEX").Value;
        internal static Effect DistortEffect => distortEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/DistortEffect").Value;

        internal static Effect shaderSwooshEX;
        internal static Effect itemEffect;
        internal static Effect colorfulEffect;
        internal static Effect distortEffect;

        public static void ChangeShooshStyle(Player player)
        {
            var vec = Main.MouseWorld - player.Center;
            vec.Y *= player.gravDir;
            player.direction = Math.Sign(vec.X);
            var modPlayer = player.GetModPlayer<WeaponDisplayPlayer>();
            modPlayer.negativeDir ^= true;


            if (ConfigurationSwoosh.instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值)
            {
                modPlayer.rotationForShadow = modPlayer.rotationForShadowNext;
                modPlayer.rotationForShadowNext = vec.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
                if (modPlayer.rotationForShadow == 0) modPlayer.rotationForShadow = vec.ToRotation();
            }
            else 
            {
                modPlayer.rotationForShadow = vec.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
            }

            modPlayer.swingCount++;

            if (ConfigurationSwoosh.instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值)
            {
                modPlayer.kValue = modPlayer.kValueNext;
                modPlayer.kValueNext = Main.rand.NextFloat(1, 2);
                if (modPlayer.kValue == 0) modPlayer.kValue = modPlayer.kValueNext;
            }
            else 
            {
                modPlayer.kValue = Main.rand.NextFloat(1, 2);
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
            if(!ConfigurationPreInstall.instance.DontChangeMyTitle)
            Main.instance.Window.Title = Language.GetTextValue("Mods.CoolerItemVisualEffect.StrangeTitle." + Main.rand.Next(15));//"幻世边境：完了泰拉成替身了";//"{$Mods.CoolerItemVisualEffect.StrangeTitle." + Main.rand.Next(15)+"}"

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Instance.GetPacket();
                packet.Write((byte)HandleNetwork.MessageType.BasicStats);
                packet.Write(modPlayer.negativeDir);
                packet.Write(modPlayer.rotationForShadow);
                packet.Write(modPlayer.rotationForShadowNext);
                packet.Write(modPlayer.kValue);
                packet.Write(modPlayer.kValueNext);
                packet.Write(modPlayer.UseSlash);

                packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); // 同步direction
            }
        }
        private void PlayerDrawLayers_DrawPlayer_27_HeldItem_WeaponDisplay(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_27_HeldItem orig, ref PlayerDrawSet drawinfo)
        {
            var drawPlayer = drawinfo.drawPlayer;
            var modPlayer = drawPlayer.GetModPlayer<WeaponDisplayPlayer>();
            Item heldItem = drawinfo.heldItem;
            bool flag = drawPlayer.itemAnimation > 0 && heldItem.useStyle != ItemUseStyleID.None;
            bool flag2 = heldItem.holdStyle != 0 && !drawPlayer.pulley;
            if (!drawPlayer.CanVisuallyHoldItem(heldItem))
            {
                flag2 = false;
            }
            bool flagMelee = true;
            if ((heldItem.type == ItemID.Zenith || heldItem.type == ModContent.ItemType<Weapons.FirstFractal_CIVE>()) && drawPlayer.itemAnimation > 0 && instance.allowZenith && instance.CoolerSwooshActive) goto mylabel;// || heldItem.type == ItemID.Terragrim || heldItem.type == ItemID.Arkhalis
            flagMelee = drawPlayer.HeldItem.damage > 0 && drawPlayer.HeldItem.useStyle == ItemUseStyleID.Swing && drawPlayer.itemAnimation > 0 && drawPlayer.HeldItem.DamageType == DamageClass.Melee && !drawPlayer.HeldItem.noUseGraphic && instance.CoolerSwooshActive;
            if (instance.ToolsNoUseNewSwooshEffect)
            {
                flagMelee = flagMelee && drawPlayer.HeldItem.axe == 0 && drawPlayer.HeldItem.hammer == 0 && drawPlayer.HeldItem.pick == 0;
            }
            bool shouldNotDrawItem = drawinfo.shadow != 0f || drawPlayer.frozen || !(flag || flag2) || heldItem.type <= ItemID.None || drawPlayer.dead || heldItem.noUseGraphic || drawPlayer.JustDroppedAnItem ||
                drawPlayer.wet && heldItem.noWet || drawPlayer.happyFunTorchTime && drawPlayer.inventory[drawPlayer.selectedItem].createTile == TileID.Torches && drawPlayer.itemAnimation == 0 ||
                !flagMelee;
            modPlayer.UseSlash = flagMelee;
            if (shouldNotDrawItem || !modPlayer.UseSlash)
            {
                if (!flagMelee)
                    orig.Invoke(ref drawinfo);
                //// 如果只开启碰撞箱，显示原版挥剑效果，但是仍然要运行斩击代码来获取碰撞箱。所以不return
                //if (!ConfigGameplay.UseHitbox || shouldNotDrawItem)
                //    return;
                if (shouldNotDrawItem) return;
            }
        mylabel:
            if (flagMelee)
            {
                modPlayer.UseSlash = true;
                var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
                var w = itemTex.Width;
                var h = itemTex.Height;
                var cs = new Color[w * h];
                itemTex.GetData(cs);
                Vector4 vcolor = default;
                float count = 0;
                float airFactor = 1;
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
                    if (instance.checkAir && Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && cs[n] != default && target == default)
                    {
                        target = cs[n];
                        airFactor = coord.X;
                    }
                }
                vcolor /= count;
                var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
                var hsl = Main.rgbToHsl(newColor);
                try
                {
                    DrawSwoosh(drawPlayer, newColor, hsl.Z < instance.IsLighterDecider, airFactor, out var rectangle);
                    //Main.NewText("!!!");
                }
                catch
                {

                }
            }
            //var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
            //var w = itemTex.Width;
            //var h = itemTex.Height;
            //var cs = new Color[w * h];
            //itemTex.GetData(cs);
            //Vector4 vcolor = default;
            //float count = 0;
            //for (int n = 0; n < cs.Length; n++)
            //{
            //    if (cs[n] != default)
            //    {
            //        var weight = (float)((n + 1) % w * (h - n / w)) / w / h;
            //        vcolor += cs[n].ToVector4() * weight;
            //        count += weight;
            //    }
            //}
            //vcolor /= count;
            //var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
            //var hsl = Main.rgbToHsl(newColor);
            ////Main.NewText(newColor);
            //if (hsl.Z < IsLighterDecider)
            //{
            //    DrawSwoosh_AlphaBlend(drawPlayer, newColor);
            //}
            //else
            //{
            //    DrawSwoosh(drawPlayer, newColor);
            //}
        }
        //{
        //    if (drawinfo.drawPlayer.JustDroppedAnItem)
        //    {
        //        return;
        //    }

        //    if (drawinfo.drawPlayer.heldProj >= 0 && drawinfo.shadow == 0f && !drawinfo.heldProjOverHand)
        //    {
        //        drawinfo.projectileDrawPosition = drawinfo.DrawDataCache.Count;
        //    }

        //    Item heldItem = drawinfo.heldItem;
        //    int num = heldItem.type;
        //    if (num == 8 && drawinfo.drawPlayer.UsingBiomeTorches)
        //    {
        //        num = drawinfo.drawPlayer.BiomeTorchHoldStyle(num);
        //    }

        //    float adjustedItemScale = drawinfo.drawPlayer.GetAdjustedItemScale(heldItem);
        //    Main.instance.LoadItem(num);
        //    Texture2D value = TextureAssets.Item[num].Value;
        //    Vector2 position = new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y));
        //    Rectangle? sourceRect = new Rectangle(0, 0, value.Width, value.Height);
        //    if (num == 75)
        //    {
        //        sourceRect = value.Frame(1, 8);
        //    }

        //    if (ItemID.Sets.IsFood[num])
        //    {
        //        sourceRect = value.Frame(1, 3, 0, 1);
        //    }

        //    drawinfo.itemColor = Lighting.GetColor((int)((double)drawinfo.Position.X + (double)drawinfo.drawPlayer.width * 0.5) / 16, (int)(((double)drawinfo.Position.Y + (double)drawinfo.drawPlayer.height * 0.5) / 16.0));
        //    if (num == 678)
        //    {
        //        drawinfo.itemColor = Color.White;
        //    }

        //    if (drawinfo.drawPlayer.shroomiteStealth && heldItem.DamageType == DamageClass.Ranged)
        //    {
        //        float num2 = drawinfo.drawPlayer.stealth;
        //        if ((double)num2 < 0.03)
        //        {
        //            num2 = 0.03f;
        //        }

        //        float num3 = (1f + num2 * 10f) / 11f;
        //        drawinfo.itemColor = new Color((byte)((float)(int)drawinfo.itemColor.R * num2), (byte)((float)(int)drawinfo.itemColor.G * num2), (byte)((float)(int)drawinfo.itemColor.B * num3), (byte)((float)(int)drawinfo.itemColor.A * num2));
        //    }

        //    if (drawinfo.drawPlayer.setVortex && heldItem.DamageType == DamageClass.Ranged)
        //    {
        //        float num4 = drawinfo.drawPlayer.stealth;
        //        if ((double)num4 < 0.03)
        //        {
        //            num4 = 0.03f;
        //        }

        //        _ = (1f + num4 * 10f) / 11f;
        //        drawinfo.itemColor = drawinfo.itemColor.MultiplyRGBA(new Color(Vector4.Lerp(Vector4.One, new Vector4(0f, 0.12f, 0.16f, 0f), 1f - num4)));
        //    }

        //    bool flag = drawinfo.drawPlayer.itemAnimation > 0 && heldItem.useStyle != 0;
        //    bool flag2 = heldItem.holdStyle != 0 && !drawinfo.drawPlayer.pulley;
        //    if (!drawinfo.drawPlayer.CanVisuallyHoldItem(heldItem))
        //    {
        //        flag2 = false;
        //    }

        //    if (drawinfo.shadow != 0f || drawinfo.drawPlayer.frozen || !(flag || flag2) || num <= 0 || drawinfo.drawPlayer.dead || heldItem.noUseGraphic || (drawinfo.drawPlayer.wet && heldItem.noWet) || (drawinfo.drawPlayer.happyFunTorchTime && drawinfo.drawPlayer.inventory[drawinfo.drawPlayer.selectedItem].createTile == 4 && drawinfo.drawPlayer.itemAnimation == 0))
        //    {
        //        return;
        //    }

        //    _ = drawinfo.drawPlayer.name;
        //    Color color = new Color(250, 250, 250, heldItem.alpha);
        //    Vector2 vector = Vector2.Zero;
        //    switch (num)
        //    {
        //        case 5094:
        //        case 5095:
        //            vector = new Vector2(4f, -4f) * drawinfo.drawPlayer.Directions;
        //            break;
        //        case 5096:
        //        case 5097:
        //            vector = new Vector2(6f, -6f) * drawinfo.drawPlayer.Directions;
        //            break;
        //    }

        //    if (num == 3823)
        //    {
        //        vector = new Vector2(7 * drawinfo.drawPlayer.direction, -7f * drawinfo.drawPlayer.gravDir);
        //    }

        //    if (num == 3827)
        //    {
        //        vector = new Vector2(13 * drawinfo.drawPlayer.direction, -13f * drawinfo.drawPlayer.gravDir);
        //        color = heldItem.GetAlpha(drawinfo.itemColor);
        //        color = Color.Lerp(color, Color.White, 0.6f);
        //        color.A = 66;
        //    }

        //    Vector2 origin = new Vector2((float)sourceRect.Value.Width * 0.5f - (float)sourceRect.Value.Width * 0.5f * (float)drawinfo.drawPlayer.direction, sourceRect.Value.Height);
        //    if (heldItem.useStyle == 9 && drawinfo.drawPlayer.itemAnimation > 0)
        //    {
        //        Vector2 value2 = new Vector2(0.5f, 0.4f);
        //        if (heldItem.type == 5009 || heldItem.type == 5042)
        //        {
        //            value2 = new Vector2(0.26f, 0.5f);
        //            if (drawinfo.drawPlayer.direction == -1)
        //            {
        //                value2.X = 1f - value2.X;
        //            }
        //        }

        //        origin = sourceRect.Value.Size() * value2;
        //    }

        //    if (drawinfo.drawPlayer.gravDir == -1f)
        //    {
        //        origin.Y = (float)sourceRect.Value.Height - origin.Y;
        //    }

        //    origin += vector;
        //    float num5 = drawinfo.drawPlayer.itemRotation;
        //    if (heldItem.useStyle == 8)
        //    {
        //        ref float x = ref position.X;
        //        float num6 = x;
        //        _ = drawinfo.drawPlayer.direction;
        //        x = num6 - 0f;
        //        num5 -= MathF.PI / 2f * (float)drawinfo.drawPlayer.direction;
        //        origin.Y = 2f;
        //        origin.X += 2 * drawinfo.drawPlayer.direction;
        //    }

        //    if (num == 425 || num == 507)
        //    {
        //        if (drawinfo.drawPlayer.gravDir == 1f)
        //        {
        //            if (drawinfo.drawPlayer.direction == 1)
        //            {
        //                drawinfo.itemEffect = SpriteEffects.FlipVertically;
        //            }
        //            else
        //            {
        //                drawinfo.itemEffect = (SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
        //            }
        //        }
        //        else if (drawinfo.drawPlayer.direction == 1)
        //        {
        //            drawinfo.itemEffect = SpriteEffects.None;
        //        }
        //        else
        //        {
        //            drawinfo.itemEffect = SpriteEffects.FlipHorizontally;
        //        }
        //    }

        //    if ((num == 946 || num == 4707) && num5 != 0f)
        //    {
        //        position.Y -= 22f * drawinfo.drawPlayer.gravDir;
        //        num5 = -1.57f * (float)(-drawinfo.drawPlayer.direction) * drawinfo.drawPlayer.gravDir;
        //    }

        //    ItemSlot.GetItemLight(ref drawinfo.itemColor, heldItem);
        //    DrawData item;
        //    switch (num)
        //    {
        //        case 3476:
        //            {
        //                Texture2D value5 = TextureAssets.Extra[64].Value;
        //                Rectangle rectangle2 = value5.Frame(1, 9, 0, drawinfo.drawPlayer.miscCounter % 54 / 6);
        //                Vector2 value6 = new Vector2(rectangle2.Width / 2 * drawinfo.drawPlayer.direction, 0f);
        //                Vector2 origin3 = rectangle2.Size() / 2f;
        //                item = new DrawData(value5, (drawinfo.ItemLocation - Main.screenPosition + value6).Floor(), rectangle2, heldItem.GetAlpha(drawinfo.itemColor).MultiplyRGBA(new Color(new Vector4(0.5f, 0.5f, 0.5f, 0.8f))), drawinfo.drawPlayer.itemRotation, origin3, adjustedItemScale, drawinfo.itemEffect, 0);
        //                drawinfo.DrawDataCache.Add(item);
        //                value5 = TextureAssets.GlowMask[195].Value;
        //                item = new DrawData(value5, (drawinfo.ItemLocation - Main.screenPosition + value6).Floor(), rectangle2, new Color(250, 250, 250, heldItem.alpha) * 0.5f, drawinfo.drawPlayer.itemRotation, origin3, adjustedItemScale, drawinfo.itemEffect, 0);
        //                drawinfo.DrawDataCache.Add(item);
        //                return;
        //            }
        //        case 4049:
        //            {
        //                Texture2D value3 = TextureAssets.Extra[92].Value;
        //                Rectangle rectangle = value3.Frame(1, 4, 0, drawinfo.drawPlayer.miscCounter % 20 / 5);
        //                Vector2 value4 = new Vector2(rectangle.Width / 2 * drawinfo.drawPlayer.direction, 0f);
        //                value4 += new Vector2(-10 * drawinfo.drawPlayer.direction, 8f * drawinfo.drawPlayer.gravDir);
        //                Vector2 origin2 = rectangle.Size() / 2f;
        //                item = new DrawData(value3, (drawinfo.ItemLocation - Main.screenPosition + value4).Floor(), rectangle, heldItem.GetAlpha(drawinfo.itemColor), drawinfo.drawPlayer.itemRotation, origin2, adjustedItemScale, drawinfo.itemEffect, 0);
        //                drawinfo.DrawDataCache.Add(item);
        //                return;
        //            }
        //        case 3779:
        //            {
        //                Texture2D texture2D = value;
        //                Rectangle rectangle3 = texture2D.Frame();
        //                Vector2 value7 = new Vector2(rectangle3.Width / 2 * drawinfo.drawPlayer.direction, 0f);
        //                Vector2 origin4 = rectangle3.Size() / 2f;
        //                float num7 = ((float)drawinfo.drawPlayer.miscCounter / 75f * (MathF.PI * 2f)).ToRotationVector2().X * 1f + 0f;
        //                Color color2 = new Color(120, 40, 222, 0) * (num7 / 2f * 0.3f + 0.85f) * 0.5f;
        //                num7 = 2f;
        //                for (float num8 = 0f; num8 < 4f; num8 += 1f)
        //                {
        //                    item = new DrawData(TextureAssets.GlowMask[218].Value, (drawinfo.ItemLocation - Main.screenPosition + value7).Floor() + (num8 * (MathF.PI / 2f)).ToRotationVector2() * num7, rectangle3, color2, drawinfo.drawPlayer.itemRotation, origin4, adjustedItemScale, drawinfo.itemEffect, 0);
        //                    drawinfo.DrawDataCache.Add(item);
        //                }

        //                item = new DrawData(texture2D, (drawinfo.ItemLocation - Main.screenPosition + value7).Floor(), rectangle3, heldItem.GetAlpha(drawinfo.itemColor).MultiplyRGBA(new Color(new Vector4(0.5f, 0.5f, 0.5f, 0.8f))), drawinfo.drawPlayer.itemRotation, origin4, adjustedItemScale, drawinfo.itemEffect, 0);
        //                drawinfo.DrawDataCache.Add(item);
        //                return;
        //            }
        //    }

        //    if (heldItem.useStyle == 5)
        //    {
        //        if (Item.staff[num])
        //        {
        //            float num9 = drawinfo.drawPlayer.itemRotation + 0.785f * (float)drawinfo.drawPlayer.direction;
        //            float num10 = 0f;
        //            float num11 = 0f;
        //            Vector2 origin5 = new Vector2(0f, value.Height);
        //            if (num == 3210)
        //            {
        //                num10 = 8 * -drawinfo.drawPlayer.direction;
        //                num11 = 2 * (int)drawinfo.drawPlayer.gravDir;
        //            }

        //            if (num == 3870)
        //            {
        //                Vector2 vector2 = (drawinfo.drawPlayer.itemRotation + MathF.PI / 4f * (float)drawinfo.drawPlayer.direction).ToRotationVector2() * new Vector2((float)(-drawinfo.drawPlayer.direction) * 1.5f, drawinfo.drawPlayer.gravDir) * 3f;
        //                num10 = (int)vector2.X;
        //                num11 = (int)vector2.Y;
        //            }

        //            if (num == 3787)
        //            {
        //                num11 = (int)((float)(8 * (int)drawinfo.drawPlayer.gravDir) * (float)Math.Cos(num9));
        //            }

        //            if (num == 3209)
        //            {
        //                Vector2 vector3 = (new Vector2(-8f, 0f) * drawinfo.drawPlayer.Directions).RotatedBy(drawinfo.drawPlayer.itemRotation);
        //                num10 = vector3.X;
        //                num11 = vector3.Y;
        //            }

        //            if (drawinfo.drawPlayer.gravDir == -1f)
        //            {
        //                if (drawinfo.drawPlayer.direction == -1)
        //                {
        //                    num9 += 1.57f;
        //                    origin5 = new Vector2(value.Width, 0f);
        //                    num10 -= (float)value.Width;
        //                }
        //                else
        //                {
        //                    num9 -= 1.57f;
        //                    origin5 = Vector2.Zero;
        //                }
        //            }
        //            else if (drawinfo.drawPlayer.direction == -1)
        //            {
        //                origin5 = new Vector2(value.Width, value.Height);
        //                num10 -= (float)value.Width;
        //            }

        //            ItemLoader.HoldoutOrigin(drawinfo.drawPlayer, ref origin5);
        //            item = new DrawData(value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + origin5.X + num10), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + num11)), sourceRect, heldItem.GetAlpha(drawinfo.itemColor), num9, origin5, adjustedItemScale, drawinfo.itemEffect, 0);
        //            drawinfo.DrawDataCache.Add(item);
        //            if (num == 3870)
        //            {
        //                item = new DrawData(TextureAssets.GlowMask[238].Value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + origin5.X + num10), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + num11)), sourceRect, new Color(255, 255, 255, 127), num9, origin5, adjustedItemScale, drawinfo.itemEffect, 0);
        //                drawinfo.DrawDataCache.Add(item);
        //            }

        //            return;
        //        }

        //        if (num == 5118)
        //        {
        //            float rotation = drawinfo.drawPlayer.itemRotation + 1.57f * (float)drawinfo.drawPlayer.direction;
        //            Vector2 vector4 = new Vector2((float)value.Width * 0.5f, (float)value.Height * 0.5f);
        //            Vector2 origin6 = new Vector2((float)value.Width * 0.5f, value.Height);
        //            Vector2 spinningpoint = new Vector2(10f, 4f) * drawinfo.drawPlayer.Directions;
        //            spinningpoint = spinningpoint.RotatedBy(drawinfo.drawPlayer.itemRotation);
        //            item = new DrawData(value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector4.X + spinningpoint.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector4.Y + spinningpoint.Y)), sourceRect, heldItem.GetAlpha(drawinfo.itemColor), rotation, origin6, adjustedItemScale, drawinfo.itemEffect, 0);
        //            drawinfo.DrawDataCache.Add(item);
        //            return;
        //        }

        //        int num12 = 10;
        //        Vector2 vector5 = new Vector2(value.Width / 2, value.Height / 2);
        //        Vector2 vector6 = Main.DrawPlayerItemPos(drawinfo.drawPlayer.gravDir, num);
        //        num12 = (int)vector6.X;
        //        vector5.Y = vector6.Y;
        //        Vector2 origin7 = new Vector2(-num12, value.Height / 2);
        //        if (drawinfo.drawPlayer.direction == -1)
        //        {
        //            origin7 = new Vector2(value.Width + num12, value.Height / 2);
        //        }

        //        item = new DrawData(value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector5.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector5.Y)), sourceRect, heldItem.GetAlpha(drawinfo.itemColor), drawinfo.drawPlayer.itemRotation, origin7, adjustedItemScale, drawinfo.itemEffect, 0);
        //        drawinfo.DrawDataCache.Add(item);
        //        if (heldItem.color != default(Color))
        //        {
        //            item = new DrawData(value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector5.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector5.Y)), sourceRect, heldItem.GetColor(drawinfo.itemColor), drawinfo.drawPlayer.itemRotation, origin7, adjustedItemScale, drawinfo.itemEffect, 0);
        //            drawinfo.DrawDataCache.Add(item);
        //        }

        //        if (heldItem.glowMask != -1)
        //        {
        //            item = new DrawData(TextureAssets.GlowMask[heldItem.glowMask].Value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector5.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector5.Y)), sourceRect, new Color(250, 250, 250, heldItem.alpha), drawinfo.drawPlayer.itemRotation, origin7, adjustedItemScale, drawinfo.itemEffect, 0);
        //            drawinfo.DrawDataCache.Add(item);
        //        }

        //        if (num == 3788)
        //        {
        //            float num13 = ((float)drawinfo.drawPlayer.miscCounter / 75f * (MathF.PI * 2f)).ToRotationVector2().X * 1f + 0f;
        //            Color color3 = new Color(80, 40, 252, 0) * (num13 / 2f * 0.3f + 0.85f) * 0.5f;
        //            for (float num14 = 0f; num14 < 4f; num14 += 1f)
        //            {
        //                item = new DrawData(TextureAssets.GlowMask[220].Value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector5.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector5.Y)) + (num14 * (MathF.PI / 2f) + drawinfo.drawPlayer.itemRotation).ToRotationVector2() * num13, null, color3, drawinfo.drawPlayer.itemRotation, origin7, adjustedItemScale, drawinfo.itemEffect, 0);
        //                drawinfo.DrawDataCache.Add(item);
        //            }
        //        }

        //        return;
        //    }
        //    var drawPlayer = drawinfo.drawPlayer;
        //    var flagMelee = drawinfo.drawPlayer.HeldItem.damage > 0 && drawinfo.drawPlayer.HeldItem.useStyle == ItemUseStyleID.Swing && drawinfo.drawPlayer.itemAnimation > 0 && drawinfo.drawPlayer.HeldItem.DamageType == DamageClass.Melee && !drawinfo.drawPlayer.HeldItem.noUseGraphic;// && counter == 0

        //    if (drawinfo.drawPlayer.gravDir == -1f)
        //    {
        //        if (!flagMelee)
        //        {
        //            drawPlayer.HeldItem.useTurn = drawPlayer.HeldItem.shoot == 0;
        //            item = new DrawData(value, position, sourceRect, heldItem.GetAlpha(drawinfo.itemColor), num5, origin, adjustedItemScale, drawinfo.itemEffect, 0);
        //            drawinfo.DrawDataCache.Add(item);
        //            if (heldItem.color != default(Color))
        //            {
        //                item = new DrawData(value, position, sourceRect, heldItem.GetColor(drawinfo.itemColor), num5, origin, adjustedItemScale, drawinfo.itemEffect, 0);
        //                drawinfo.DrawDataCache.Add(item);
        //            }

        //            if (heldItem.glowMask != -1)
        //            {
        //                item = new DrawData(TextureAssets.GlowMask[heldItem.glowMask].Value, position, sourceRect, new Color(250, 250, 250, heldItem.alpha), num5, origin, adjustedItemScale, drawinfo.itemEffect, 0);
        //                drawinfo.DrawDataCache.Add(item);
        //            }
        //        }
        //        return;
        //    }
        //    if (!flagMelee)
        //    {
        //        drawPlayer.HeldItem.useTurn = drawPlayer.HeldItem.shoot == 0;
        //        item = new DrawData(value, position, sourceRect, heldItem.GetAlpha(drawinfo.itemColor), num5, origin, adjustedItemScale, drawinfo.itemEffect, 0);
        //        drawinfo.DrawDataCache.Add(item);
        //        if (heldItem.color != default(Color))
        //        {
        //            item = new DrawData(value, position, sourceRect, heldItem.GetColor(drawinfo.itemColor), num5, origin, adjustedItemScale, drawinfo.itemEffect, 0);
        //            drawinfo.DrawDataCache.Add(item);
        //        }

        //        if (heldItem.glowMask != -1)
        //        {
        //            item = new DrawData(TextureAssets.GlowMask[heldItem.glowMask].Value, position, sourceRect, color, num5, origin, adjustedItemScale, drawinfo.itemEffect, 0);
        //            drawinfo.DrawDataCache.Add(item);
        //        }


        //        if (!heldItem.flame || drawinfo.shadow != 0f)
        //        {
        //            return;
        //        }

        //        try
        //        {
        //            Main.instance.LoadItemFlames(num);
        //            if (TextureAssets.ItemFlame[num].IsLoaded)
        //            {
        //                if (num == 3823)
        //                {
        //                    return;
        //                }
        //                Color color4 = new Color(100, 100, 100, 0);
        //                int num15 = 7;
        //                float num16 = 1f;
        //                switch (num)
        //                {
        //                    case 3045:
        //                        color4 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
        //                        break;
        //                    case 4952:
        //                        num15 = 3;
        //                        num16 = 0.6f;
        //                        color4 = new Color(50, 50, 50, 0);
        //                        break;
        //                }

        //                for (int i = 0; i < num15; i++)
        //                {
        //                    float num17 = drawinfo.drawPlayer.itemFlamePos[i].X * adjustedItemScale * num16;
        //                    float num18 = drawinfo.drawPlayer.itemFlamePos[i].Y * adjustedItemScale * num16;
        //                    item = new DrawData(TextureAssets.ItemFlame[num].Value, new Vector2((int)(position.X + num17), (int)(position.Y + num18)), sourceRect, color4, num5, origin, adjustedItemScale, drawinfo.itemEffect, 0);
        //                    drawinfo.DrawDataCache.Add(item);
        //                }
        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    else 
        //    {
        //        var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
        //        var w = itemTex.Width;
        //        var h = itemTex.Height;
        //        var cs = new Color[w * h];
        //        itemTex.GetData(cs);
        //        Vector4 vcolor = default;
        //        float count = 0;
        //        float airFactor = 1;
        //        Color target = default;
        //        for (int n = 0; n < cs.Length; n++)
        //        {
        //            if (cs[n] != default && (n - w < 0 || cs[n - w] != default) && (n - 1 < 0 || cs[n - 1] != default) && (n + w >= cs.Length || cs[n + w] != default) && (n + 1 >= cs.Length || cs[n + 1] != default))
        //            {
        //                var weight = (float)((n + 1) % w * (h - n / w)) / w / h;
        //                vcolor += cs[n].ToVector4() * weight;
        //                count += weight;
        //            }
        //            Vector2 coord = new Vector2(n % w, n / w);
        //            coord /= new Vector2(w, h);
        //            if (ConfigurationSwoosh.instance.checkAir && Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && cs[n] != default && target == default)
        //            {
        //                target = cs[n];
        //                airFactor = coord.X;
        //            }
        //        }
        //        vcolor /= count;
        //        var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
        //        var hsl = Main.rgbToHsl(newColor);
        //        try
        //        {
        //            DrawSwoosh(drawPlayer, newColor, hsl.Z < ConfigurationSwoosh.instance.IsLighterDecider, airFactor, out var rectangle);
        //            Main.NewText("!!!");
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //}
        public static void DrawSwoosh(Player drawPlayer, Color newColor, bool alphaBlend, float checkAirFactor, out Rectangle bodyRec)
        {
            //Main.NewText(checkAirFactor);
            bodyRec = default;
            if (ShaderSwooshEX == null) return;
            if (ItemEffect == null) return;
            if (DistortEffect == null) return;
            if (Main.GameViewMatrix == null) return;
            //Main.NewText("!!!!");
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            var modPlayer = drawPlayer.GetModPlayer<WeaponDisplayPlayer>();//modplayer类
                                                                           //if (drawPlayer.itemAnimation == drawPlayer.itemAnimationMax - 1 && !Main.gamePaused)
                                                                           //{
                                                                           //    var vec = Main.MouseWorld - drawPlayer.Center;
                                                                           //    vec.Y *= drawPlayer.gravDir;
                                                                           //    drawPlayer.direction = Math.Sign(vec.X);
                                                                           //    modPlayer.negativeDir ^= true;
                                                                           //    modPlayer.rotationForShadow = vec.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
                                                                           //    modPlayer.kValue = Main.rand.NextFloat(1, 2);
                                                                           //}
            var fac = modPlayer.factorGeter;
            fac = modPlayer.negativeDir ? 1 - fac : fac;//每次挥动都会改变方向，所以插值函数方向也会一起变（原本是从1到0，反过来就是0到1(虽然说一般都是0到1




            //var fac = 1 - (cValue - 1) * (1 - factor) * (1 - factor) - (2 - cValue) * (1 - factor);//丢到另一个插值函数里了，可以自己画一下图像，这个插值效果比上面那个线性插值好//((float)Math.Sqrt(factor) + factor) * .5f;//(cValue - 1) * factor * factor + (2 - cValue) * factor
            //fac *= 3;
            //fac %= 1;
            //Main.NewText(new Vector2(fac,factor));

            var drawCen = drawPlayer.gravDir == -1 ? new Vector2(drawPlayer.Center.X, (2 * (Main.screenPosition + new Vector2(960, 560)) - drawPlayer.Center - new Vector2(0, 96)).Y) : drawPlayer.Center;//2 * (Main.screenPosition + new Vector2(960, 560)) - drawPlayer.Center - new Vector2(0, 96)
                                                                                                                                                                                                          //var fac = (float)Math.Sqrt(factor);
                                                                                                                                                                                                          //var theta = (fac * -1.125f + (1 - fac) * 0.1125f) * Pi;

            float rotVel = instance.swooshActionStyle == SwooshAction.两次普通斩击一次高速旋转 && modPlayer.swingCount % 3 == 2 ? instance.rotationVelocity : 1;
            var theta = (1.2375f * fac * rotVel - 1.125f) * MathHelper.Pi;//线性插值后乘上一个系数，这里的起始角度和终止角度是试出来的（
            CustomVertexInfo[] c = new CustomVertexInfo[6];//顶点数组，绘制完整的物品需要两个三角形(六个顶点，两组重合
            var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;//获取物品贴图
            float xScaler = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, fac) : modPlayer.kValue;//获取x轴方向缩放系数
            float scaler = itemTex.Size().Length() * drawPlayer.GetAdjustedItemScale(drawPlayer.HeldItem) / xScaler * 0.5f * trans.M11 * instance.swooshSize * checkAirFactor;//对椭圆进行位似变换(你直接说坐标乘上一个系数不就好了吗，屑阿汪
            var cos = (float)Math.Cos(theta) * scaler;
            var sin = (float)Math.Sin(theta) * scaler;//这里(cos,sin)对应的位置就是我们希望贴图右上角所在的位置，而(0,0)对应的位置是贴图左下角所在的位置
            var rotator = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, fac) : modPlayer.rotationForShadow;
            var u = new Vector2(xScaler * (cos - sin), -cos - sin).RotatedBy(rotator);
            var v = new Vector2(-xScaler * (cos + sin), sin - cos).RotatedBy(rotator);//这里其实应该是都要除个二，或者上面scaler那里0.7改成0.5
                                                                                      //此处u对应的是贴图左上角或者右下角(由方向决定,v同理。u+v就是贴图右上角(剑锋位置。因为我们希望画出来是椭圆，所以我们给x方向乘上个系数，然后在根据使用朝向进行旋转就好啦
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            //方法名是创建正交偏移中心 l=left  r=right t=top b=bottom n=zNearPlane f=zFarPlane
            //( 2 / (r - l),           0,           0, -(r + l) / (r - l)
            //            0, 2 / (t - b),           0, -(t + b) / (t - b)
            //            0,           0, 2 / (n - f), -(n + f) / (f - n)
            //            0,           0,           0,                  1）
            //这尼玛的是什么鬼————
            //用人话说就是
            //x取值在[l,r]
            //y取值在[b,t]
            //z取值在[n,f]
            //w取值为1时
            //将这个b矩阵作用在(x,y,z,w)上后
            //x、y、z映射到[-1,1]

            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            //假设这里丢进去是(x,y,z),出来的矩阵就是
            //( 1, 0, 0, 0
            //  0, 1, 0, 0
            //  0, 0, 1, 0
            //  x, y, z, 1)
            //然后如果你将矩阵作用在(a,b,c)上就是(a, b, c, a * x + b * y + c * z + w),说实话我不是很能理解这个的意义

            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

            //var w = itemTex.Width;
            //var h = itemTex.Height;
            //var cs = new Color[w * h];
            //itemTex.GetData(cs);
            //Vector4 vcolor = default;
            //float count = 0;
            //for (int n = 0; n < cs.Length; n++)
            //{
            //    if (cs[n] != default)
            //    {
            //        var weight = (float)((n + 1) % w * (h - n / w)) / w / h;
            //        vcolor += cs[n].ToVector4() * weight;
            //        count += weight;
            //    }
            //}
            //vcolor /= count;
            //var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
            //这一步是对贴图的颜色取加权平均数，右上角权重为1，左下角权重为0.01，左上和右下0.1
            //说人话就是尽量贴近剑锋处的颜色
            //其实我可以把武器的贴图丢到shader里然后整分多层颜色，没必要整这个加权平均
            //我下次试试那样的.fx


            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            var swooshAniFac = modPlayer.negativeDir ? 4 * fac - 3 : 4 * fac;////modPlayer.negativeDir ? 1 - (float)Math.Sqrt(1 - fac * fac) : 1 - (float)Math.Sqrt(1 - (fac - 1) * (fac - 1))
            swooshAniFac = MathHelper.Clamp(swooshAniFac, 0, 1);
            var theta3 = (1.2375f * swooshAniFac * rotVel - 1.125f) * MathHelper.Pi;//这里是又一处插值
            float xScaler3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, swooshAniFac) : modPlayer.kValue;
            var rotator3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, swooshAniFac) : modPlayer.rotationForShadow;
            //我们武器所在的角度是theta，我们拖尾的末端的角度就是上面的theta3，下面的theta2就是theta渐变到theta3

            //var cos2 = (float)Math.Cos(theta3) * scaler;
            //var sin2 = (float)Math.Sin(theta3) * scaler;
            //var u2 = new Vector2(cos2 * xScaler + sin2, -sin2 * xScaler + cos2).RotatedBy(modPlayer.rotationForShadow);
            //var v2 = new Vector2(cos2 * xScaler - sin2, -sin2 * xScaler - cos2).RotatedBy(modPlayer.rotationForShadow) * length;
            //var oldVec = u2 + v2;



            Color realColor = newColor;
            Vector3 hsl = Main.rgbToHsl(realColor);
            for (int i = 0; i < 45; i++)
            {
                var f = i / 44f;//分割成25次惹，f从1/25f到1
                var theta2 = f.Lerp(theta3, theta, true);//快乐线性插值
                var xScaler2 = (instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? f : 1).Lerp(xScaler3, xScaler, true);
                var rotator2 = (instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? f : 1).Lerp(rotator3, rotator, true);
                var cos2 = (float)Math.Cos(theta2) * scaler;
                var sin2 = (float)Math.Sin(theta2) * scaler;


                var u2 = new Vector2(xScaler2 * (cos2 - sin2), -cos2 - sin2).RotatedBy(rotator2);
                var v2 = new Vector2(-xScaler2 * (cos2 + sin2), sin2 - cos2).RotatedBy(rotator2);
                //和刚刚上面那里一样的流程，不要问我为什么不用一个数组存储已经算好的之前的u、v
                //因为那样的话如果你武器很快的话效果就很烂了（指不够平滑圆润
                //这种写法虽然对电脑不太友好但是效果好（x
                var newVec = u2 + v2;//不过这里我们只需要最后的结果(那为什么不直接(cos2 * xScaler,sin2)，阿汪你在干什么
                var alphaLight = alphaBlend ? Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).R / 255f * .6f : 0.6f;
                if (instance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 || instance.swooshColorType == SwooshColorType.色调处理与对角线混合)
                {
                    float h = (hsl.X + instance.hueOffsetValue + instance.hueOffsetRange * (2 * f - 1)) % 1;
                    float s = MathHelper.Clamp(hsl.Y * instance.saturationScalar, 0, 1);
                    float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + instance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - instance.luminosityRange), 0, 1);
                    realColor = Main.hslToRgb(h, s, l);
                    //Main.NewText((h, s, l, realColor), realColor);
                }
                var _f = 6 * f / (3 * f + 1);//f;// 
                _f = MathHelper.Clamp(_f, 0, 1);
                //_f = ConfigurationSwoosh.instance.distortFactor != 0 ? _f : _f * _f;
                realColor.A = (byte)(_f * 255);//.MultiplyRGBA(new Color(1,1,1,_f))
                bars.Add(new CustomVertexInfo(drawCen + newVec, realColor, new Vector3(1 - f, 1, alphaLight)));//(3 * f - 4) / (4 * f - 3)//快乐连顶点
                realColor.A = 0;//.MultiplyRGBA(new Color(1, 1, 1, 0))
                bars.Add(new CustomVertexInfo(drawCen, realColor, new Vector3(0, 0, alphaLight)));// + newVec * (1 - f)
                                                                                                  //bars.Add(new CustomVertexInfo(drawCen + newVec, alphaBlend ? new Color(realColor.R / 255f, realColor.G / 255f, realColor.B / 255f, _f) : realColor, new Vector3(1 - f, 1, alphaBlend ? alphaLight : _f)));//(3 * f - 4) / (4 * f - 3)//快乐连顶点
                                                                                                  //bars.Add(new CustomVertexInfo(drawCen, alphaBlend ? new Color(realColor.R / 255f, realColor.G / 255f, realColor.B / 255f, 0) : realColor, new Vector3(0, 0, alphaBlend ? alphaLight : 0)));
                                                                                                  //bars.Add(new CustomVertexInfo(drawCen + newVec, alphaBlend ? new Color(newColor.R / 255f, newColor.G / 255f, newColor.B / 255f, f) : newColor, new Vector3(1 - f, 1, alphaBlend ? alphaLight : 3 * f / (3 * f + 1))));//(3 * f - 4) / (4 * f - 3)//快乐连顶点
                                                                                                  //bars.Add(new CustomVertexInfo(drawCen, alphaBlend ? new Color(newColor.R / 255f, newColor.G / 255f, newColor.B / 255f, 0) : newColor, new Vector3(0, 0, alphaBlend ? alphaLight : 0)));
                                                                                                  //oldVec = newVec;
            }
            //Main.NewText(new Vector3(fac, MathHelper.Clamp(modPlayer.negativeDir ? (4 * fac - 3) : 4 * fac, 0, 1), modPlayer.negativeDir ? -1 : 1));
            SamplerState sampler;
            switch (instance.swooshSampler)
            {
                default:
                case SwooshSamplerState.各向异性: sampler = SamplerState.AnisotropicClamp; break;
                case SwooshSamplerState.线性: sampler = SamplerState.LinearClamp; break;
                case SwooshSamplerState.点: sampler = SamplerState.PointClamp; break;
            }
            List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }


                //RenderTarget2D render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                bool useRender = instance.distortFactor != 0;
                var gd = Main.graphics.GraphicsDevice;
                var sb = Main.spriteBatch;
                var passCount = 0;
                switch (instance.swooshColorType)
                {
                    case SwooshColorType.函数生成热度图: passCount = 2; break;
                    case SwooshColorType.武器贴图对角线: passCount = 1; break;
                    case SwooshColorType.色调处理与对角线混合: passCount = 3; break;
                }
                if (instance.swooshColorType == SwooshColorType.函数生成热度图 && (modPlayer.colorBar.tex == null || modPlayer.colorBar.type != drawPlayer.HeldItem.type))
                {
                    var colors = new Color[1500];
                    for (int i = 0; i < 300; i++)
                    {
                        var f = i / 299f;//分割成25次惹，f从1/25f到1//1 - 
                        f = f * f;// *f
                        float h = (hsl.X + instance.hueOffsetValue + instance.hueOffsetRange * (2 * f - 1)) % 1;
                        float s = MathHelper.Clamp(hsl.Y * instance.saturationScalar, 0, 1);
                        float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + instance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - instance.luminosityRange), 0, 1);
                        var currentColor = Main.hslToRgb(h, s, l);
                        for (int n = 0; n < 5; n++)
                        {
                            colors[i + 300 * n] = currentColor;
                        }
                    }
                    if (modPlayer.colorBar.tex == null) modPlayer.colorBar.tex = new Texture2D(Main.graphics.GraphicsDevice, 300, 5);
                    modPlayer.colorBar.tex.SetData(colors);
                    modPlayer.colorBar.type = drawPlayer.HeldItem.type;
                    //Main.graphics.GraphicsDevice.Textures[2] = colorBar;
                    //sb.Draw(colorBar, new Vector2(240, 240), Color.White);
                }
                if (useRender)
                {
                    #region MyRegion
                    //gd.SetRenderTarget(Main.screenTargetSwap);
                    //sp.End();

                    //sp.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    //sp.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                    //sp.End();

                    //gd.SetRenderTarget(render);
                    //gd.Clear(Color.Transparent);


                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                    //DistortEffect.Parameters["uTransform"].SetValue(model * projection);
                    //DistortEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.06f);//-(float)Main.time * 0.06f
                    //DistortEffect.Parameters["unit"].SetValue((u + v) * 0.005f);
                    //Main.graphics.GraphicsDevice.Textures[0] = Main.screenTarget;
                    //Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("BaseTex_7");
                    //Main.graphics.GraphicsDevice.Textures[2] = GetWeaponDisplayImage("AniTex");
                    //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
                    //Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
                    //Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicClamp;
                    //ShaderSwooshEX.CurrentTechnique.Passes[0].Apply();
                    //var tris = new CustomVertexInfo[triangleList.Count];
                    //for (int n = 0; n < tris.Length; n++)
                    //{
                    //    tris[n] = triangleList[n];
                    //    tris[n].Color = new Vector4((tris[n].Position - Main.screenPosition) / new Vector2(1920, 1120), 0, 0).ToColor();
                    //}
                    //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, tris, 0, tris.Length / 3);
                    //Main.graphics.GraphicsDevice.RasterizerState = originalState;
                    ////Main.spriteBatch.Draw(Main.screenTarget, new Vector2(64, 64), null, Color.White * .5f, 0, default, 1, 0, 0);
                    //Main.spriteBatch.End();

                    //gd.SetRenderTarget(Main.screenTarget);
                    //sp.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    //sp.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    //sp.Draw(render, Vector2.Zero, Color.White);
                    #endregion
                    //先在自己的render上画这个弹幕
                    sb.End();
                    gd.SetRenderTarget(Instance.Render);
                    gd.Clear(Color.Transparent);
                    sb.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
                    ShaderSwooshEX.Parameters["uTransform"].SetValue(model * projection);
                    ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
                    ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
                    ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir);//-(float)Main.time * 0.06f
                    ShaderSwooshEX.Parameters["airFactor"].SetValue(checkAirFactor);
                    ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather);

                    Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + instance.ImageIndex);//字面意义，base那个是不会随时间动的，ani那个会动//BaseTex//_7
                    Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
                    Main.graphics.GraphicsDevice.Textures[2] = itemTex;
                    if (instance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = modPlayer.colorBar.tex;


                    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;


                    //if (alphaBlend) passCount += 2;
                    ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                    sb.End();
                    //然后在随便一个render里绘制屏幕，并把上面那个带弹幕的render传进shader里对屏幕进行处理
                    //原版自带的screenTargetSwap就是一个可以使用的render，（原版用来连续上滤镜）

                    gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
                    gd.Clear(Color.Transparent);//清空
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    DistortEffect.CurrentTechnique.Passes[0].Apply();//ApplyPass
                    DistortEffect.Parameters["tex0"].SetValue(Instance.Render);//render可以当成贴图使用或者绘制。（前提是当前gd.SetRenderTarget的不是这个render，否则会报错）
                    DistortEffect.Parameters["offset"].SetValue((u + v) * -0.002f * (1 - 2 * Math.Abs(0.5f - fac)) * instance.distortFactor);//设置参数时间
                    DistortEffect.Parameters["invAlpha"].SetValue(0);
                    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//绘制原先屏幕内容
                    //pixelshader里处理
                    sb.End();

                    //最后在screenTarget上把刚刚的结果画上
                    gd.SetRenderTarget(Main.screenTarget);
                    gd.Clear(Color.Transparent);
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    //sb.End();

                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                    //sb.Draw(render, Vector2.Zero, Color.White);
                }
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
                ShaderSwooshEX.Parameters["uTransform"].SetValue(model * projection);
                ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
                ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
                ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir);
                ShaderSwooshEX.Parameters["airFactor"].SetValue(checkAirFactor);
                ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather);


                Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + instance.ImageIndex);//字面意义，base那个是不会随时间动的，ani那个会动//BaseTex//_7
                Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
                Main.graphics.GraphicsDevice.Textures[2] = itemTex;
                if (instance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = modPlayer.colorBar.tex;
                //if (ConfigurationSwoosh.instance.swooshColorType == SwooshColorType.函数生成热度图) 
                //{
                //    var colorBar = new Texture2D(Main.graphics.GraphicsDevice,300,60);
                //    colorBar.SetData<Color>();
                //    Main.graphics.GraphicsDevice.Textures[3] = colorBar;
                //}


                Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;

                //var passCount = 0;
                //if (ConfigurationSwoosh.instance.swooshColorType == SwooshColorType.武器贴图对角线) passCount++;
                //if (alphaBlend) passCount += 2;
                ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                //ShaderSwooshEX.Parameters["uTransform"].SetValue(model * projection);
                //ShaderSwooshEX.Parameters["uLighter"].SetValue(ConfigurationSwoosh.instance.luminosityFactor);
                //ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
                //Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_7");//字面意义，base那个是不会随时间动的，ani那个会动//BaseTex
                //Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
                //Main.graphics.GraphicsDevice.Textures[2] = itemTex;
                ////if (ConfigurationSwoosh.instance.swooshColorType == SwooshColorType.函数生成热度图) 
                ////{
                ////    var colorBar = new Texture2D(Main.graphics.GraphicsDevice,300,60);
                ////    colorBar.SetData<Color>();
                ////    Main.graphics.GraphicsDevice.Textures[3] = colorBar;
                ////}


                //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
                //Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
                //Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicClamp;
                //Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;

                //var passCount = 0;
                //if (ConfigurationSwoosh.instance.swooshColorType == SwooshColorType.武器贴图对角线) passCount++;
                ////if (alphaBlend) passCount += 2;
                //ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
                //Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }

            //var color = Main.hslToRgb(0.33f, 0.75f, 0.75f);
            //.5f
            var num0 = modPlayer.negativeDir ? 1 : 0;
            u /= checkAirFactor;
            v /= checkAirFactor;
            float light = instance.glowLight;
            c[0] = new CustomVertexInfo(drawCen, newColor, new Vector3(0, 1, light));//因为零向量固定是左下角所以纹理固定(0,1)
            c[1] = new CustomVertexInfo(u + drawCen, newColor, new Vector3(num0 ^ 1, num0 ^ 1, light));//这一处也许有更优美的写法
            c[2] = new CustomVertexInfo(v + drawCen, newColor, new Vector3(num0, num0, light));
            c[3] = c[1];
            c[4] = new CustomVertexInfo(u + v + drawCen, newColor, new Vector3(1, 0, light));//因为u+v固定是右上角所以纹理固定(1,0)
            c[5] = c[2];
            //Main.spriteBatch.DrawLine(u + v + drawPlayer.Center, drawPlayer.Center, Color.Red);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, instance.ItemAdditive ? BlendState.Additive : BlendState.AlphaBlend, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
            itemEffect.Parameters["uTransform"].SetValue(model * projection);
            //将变换矩阵作用在正交投影矩阵上，具体结果以及意义我下次再想想
            //半年前就问过零群各位大佬，他们都说没必要搞懂，tr图像变换矩阵而已。
            itemEffect.Parameters["uTime"].SetValue((float)Main.time / 60 % 1);//传入时间偏移量
            itemEffect.Parameters["uItemColor"].SetValue(instance.ItemHighLight ? Vector4.One : Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).ToVector4());
            //传入顶点绘制出的物品的颜色，这里采用环境光，和sb.Draw的那个color参数差不多(吧
            itemEffect.Parameters["uItemGlowColor"].SetValue(new Color(250, 250, 250, drawPlayer.HeldItem.alpha).ToVector4());

            Main.graphics.GraphicsDevice.Textures[0] = itemTex;//传入物品贴图
            Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("Style_12");//传入因时间而x纹理坐标发生偏移的灰度图，这里其实并不明显，你可以参考我mod里的无间之钟在黑暗环境下的效果
            Main.graphics.GraphicsDevice.Textures[2] = GetWeaponDisplayImage("Style_18");//传入固定叠加的灰度图
            var tex = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
            tex.SetData(new Color[] { Color.Transparent });
            Main.graphics.GraphicsDevice.Textures[3] = tex;
            var g = drawPlayer.HeldItem.glowMask;
            if (g != -1)
            {
                //Main.graphics.GraphicsDevice.Textures[3] = TextureAssets.GlowMask[g].Value;
                Main.graphics.GraphicsDevice.Textures[3] = TextureAssets.GlowMask[g].Value;
            }
            if (drawPlayer.HeldItem.type == 3823)
            {
                //Main.graphics.GraphicsDevice.Textures[1] = TextureAssets.ItemFlame[3823].Value;
                Main.graphics.GraphicsDevice.Textures[3] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/ItemFlame_3823").Value;

                //ItemEffect.Parameters["uItemGlowColor"].SetValue(new Color(100, 100, 100, 0).ToVector4());

            }
            //上面这两个灰度图叠加后作为插值的t，大概是这样的映射:t=0时最终物品上的颜色是0(黑色，additive模式下是透明的)，t=0.5时是color（顶点传入的color参数，不是上面uItemColor,t=1时是1(白色)
            Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;

            itemEffect.CurrentTechnique.Passes[2].Apply();//这里是第三个pass，可以直接写下标不必写pass名(
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, c, 0, 2);
            Main.graphics.GraphicsDevice.RasterizerState = originalState;

            //float num20 = (u+v).ToRotation() * drawPlayer.direction;
            //drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 3;
            //if ((double)num20 < -0.75)
            //{
            //    drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 2;
            //    if (drawPlayer.gravDir == -1f)
            //    {
            //        drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 4;
            //    }
            //}
            //if ((double)num20 > 0.6)
            //{
            //    drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 4;
            //    if (drawPlayer.gravDir == -1f)
            //    {
            //        drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 2;
            //        return;
            //    }
            //}
            //var vel = u + v;
            //bodyRec = drawPlayer.bodyFrame;
            //bodyRec.Y = 112 + 56 * (int)(Math.Abs(new Vector2(-vel.Y, vel.X).ToRotation()) / MathHelper.Pi * 3);
            //drawPlayer.bodyFrame.Y = 112 + 56 * (int)(Math.Abs(new Vector2(-vel.Y, vel.X).ToRotation()) / MathHelper.Pi * 3);
            modPlayer.direct = (u + v).ToRotation();
            modPlayer.HitboxPosition = (u + v) / Main.GameViewMatrix.TransformationMatrix.M11;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Instance.GetPacket();
                packet.Write((byte)HandleNetwork.MessageType.rotationDirect);
                packet.Write(modPlayer.direct);
                packet.WritePackedVector2(modPlayer.HitboxPosition);
                packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, drawPlayer.whoAmI); // 同步direction
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        }
        public const string ImagePath = "CoolerItemVisualEffect/Shader/";
        public static Texture2D GetWeaponDisplayImage(string name) => ModContent.Request<Texture2D>(ImagePath + name).Value;
        #region 阿汪废弃的代码，为什么是注释而不是删掉呢？？反正下辈子都用不到的东西
        //private void DrawSwoosh(Player drawPlayer, Color newColor)
        //{
        //    //counter++;
        //    //if(!Main.gamePaused)
        //    //Main.NewText((drawPlayer.itemAnimation, drawPlayer.itemAnimationMax));

        //    if (ColorfulEffect == null) return;
        //    if (ItemEffect == null) return;
        //    var modPlayer = drawPlayer.GetModPlayer<WeaponDisplayPlayer>();//modplayer类
        //    var factor = (float)drawPlayer.itemAnimation / (drawPlayer.itemAnimationMax - 1);//物品挥动程度的插值，这里应该是从1到0
        //    const float cValue = 3f;
        //    var fac = 1 - (cValue - 1) * (1 - factor) * (1 - factor) - (2 - cValue) * (1 - factor);//丢到另一个插值函数里了，可以自己画一下图像，这个插值效果比上面那个线性插值好//((float)Math.Sqrt(factor) + factor) * .5f;//(cValue - 1) * factor * factor + (2 - cValue) * factor
        //    //Main.NewText(new Vector2(fac,factor));

        //    var drawCen = drawPlayer.gravDir == -1 ? new Vector2(drawPlayer.Center.X, (2 * (Main.screenPosition + Main.ScreenSize.ToVector2() / 2f) - drawPlayer.Center - new Vector2(0, 96)).Y) : drawPlayer.Center;
        //    //2 * (Main.screenPosition + new Vector2(960, 560)) - drawPlayer.Center - new Vector2(0, 96)
        //    //var fac = (float)Math.Sqrt(factor);
        //    //var theta = (fac * -1.125f + (1 - fac) * 0.1125f) * Pi;
        //    fac = modPlayer.negativeDir ? 1 - fac : fac;//每次挥动都会改变方向，所以插值函数方向也会一起变（原本是从1到0，反过来就是0到1(虽然说一般都是0到1
        //    var theta = (fac * -1.125f + (1 - fac) * 0.1125f) * MathHelper.Pi;//线性插值后乘上一个系数，这里的起始角度和终止角度是试出来的（
        //    CustomVertexInfo[] c = new CustomVertexInfo[6];//顶点数组，绘制完整的物品需要两个三角形(六个顶点，两组重合
        //    var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;//获取物品贴图
        //    float xScaler = modPlayer.kValue;//获取x轴方向缩放系数
        //    float scaler = itemTex.Size().Length() * drawPlayer.GetAdjustedItemScale(drawPlayer.HeldItem) / xScaler * 0.7f * Main.GameViewMatrix.TransformationMatrix.M11;//对椭圆进行位似变换(你直接说坐标乘上一个系数不就好了吗，屑阿汪
        //    var cos = (float)Math.Cos(theta) * scaler;
        //    var sin = (float)Math.Sin(theta) * scaler;//这里(cos,sin)对应的位置就是我们希望贴图右上角所在的位置，而(0,0)对应的位置是贴图左下角所在的位置
        //    var u = new Vector2(xScaler * (cos - sin), -cos - sin).RotatedBy(modPlayer.rotationForShadow);
        //    var v = new Vector2(-xScaler * (cos + sin), sin - cos).RotatedBy(modPlayer.rotationForShadow);//这里其实应该是都要除个二，或者上面scaler那里0.7改成0.5
        //    //此处u对应的是贴图左上角或者右下角(由方向决定,v同理。u+v就是贴图右上角(剑锋位置。因为我们希望画出来是椭圆，所以我们给x方向乘上个系数，然后在根据使用朝向进行旋转就好啦
        //    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
        //    //方法名是创建正交偏移中心 l=left  r=right t=top b=bottom n=zNearPlane f=zFarPlane
        //    //( 2 / (r - l),           0,           0, -(r + l) / (r - l)
        //    //            0, 2 / (t - b),           0, -(t + b) / (t - b)
        //    //            0,           0, 2 / (n - f), -(n + f) / (f - n)
        //    //            0,           0,           0,                  1）
        //    //这尼玛的是什么鬼————
        //    //用人话说就是
        //    //x取值在[l,r]
        //    //y取值在[b,t]
        //    //z取值在[n,f]
        //    //w取值为1时
        //    //将这个b矩阵作用在(x,y,z,w)上后
        //    //x、y、z映射到[-1,1]

        //    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
        //    //假设这里丢进去是(x,y,z),出来的矩阵就是 ( 1, 0, 0, 0
        //    //  0, 1, 0, 0
        //    //  0, 0, 1, 0
        //    //  x, y, z, 1)
        //    //然后如果你将矩阵作用在(a,b,c)上就是(a, b, c, a * x + b * y + c * z + w),说实话我不是很能理解这个的意义

        //    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

        //    //var w = itemTex.Width;
        //    //var h = itemTex.Height;
        //    //var cs = new Color[w * h];
        //    //itemTex.GetData(cs);
        //    //Vector4 vcolor = default;
        //    //float count = 0;
        //    //for (int n = 0; n < cs.Length; n++)
        //    //{
        //    //    if (cs[n] != default)
        //    //    {
        //    //        var weight = (float)((n + 1) % w * (h - n / w)) / w / h;
        //    //        vcolor += cs[n].ToVector4() * weight;
        //    //        count += weight;
        //    //    }
        //    //}
        //    //vcolor /= count;
        //    //var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
        //    //这一步是对贴图的颜色取加权平均数，右上角权重为1，左下角权重为0.01，左上和右下0.1
        //    //说人话就是尽量贴近剑锋处的颜色
        //    //其实我可以把武器的贴图丢到shader里然后整分多层颜色，没必要整这个加权平均
        //    //我下次试试那样的.fx


        //    List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

        //    var theta3 = (modPlayer.negativeDir ? (4 * fac - 3) : 4 * fac).Lerp(0.1125f, -1.125f, true) * MathHelper.Pi;//这里是又一处插值
        //    //我们武器所在的角度是theta，我们拖尾的末端的角度就是上面的theta3，下面的theta2就是theta渐变到theta3

        //    //var cos2 = (float)Math.Cos(theta3) * scaler;
        //    //var sin2 = (float)Math.Sin(theta3) * scaler;
        //    //var u2 = new Vector2(cos2 * xScaler + sin2, -sin2 * xScaler + cos2).RotatedBy(modPlayer.rotationForShadow);
        //    //var v2 = new Vector2(cos2 * xScaler - sin2, -sin2 * xScaler - cos2).RotatedBy(modPlayer.rotationForShadow) * length;
        //    //var oldVec = u2 + v2;

        //    // 为了联机下缩放看到别的玩家挥舞武器，武器显示在正常的地方
        //    var screenCenterPos = Main.screenPosition + Main.ScreenSize.ToVector2() / 2f;
        //    var centerToPlayerVec = drawCen - screenCenterPos; // 玩家坐标减去屏幕中心坐标得到向量
        //    float centerToPlayerLength = centerToPlayerVec.Length() * Main.GameViewMatrix.TransformationMatrix.M11; // 原距离乘屏幕缩放得到视觉距离
        //    var playerVisualPos = screenCenterPos + Vector2.Normalize(centerToPlayerVec) * centerToPlayerLength;

        //    var hitboxPosition = modPlayer.HitboxPosition;
        //    for (int i = 0; i < 25; i++)
        //    {
        //        var f = (i + 1) / 25f;//分割成25次惹，f从1/25f到1
        //        var theta2 = f.Lerp(theta3, theta, true);//快乐线性插值
        //        var cos2 = (float)Math.Cos(theta2) * scaler;
        //        var sin2 = (float)Math.Sin(theta2) * scaler;
        //        var u2 = new Vector2(xScaler * (cos2 - sin2), -cos2 - sin2).RotatedBy(modPlayer.rotationForShadow);
        //        var v2 = new Vector2(-xScaler * (cos2 + sin2), sin2 - cos2).RotatedBy(modPlayer.rotationForShadow);
        //        //和刚刚上面那里一样的流程，不要问我为什么不用一个数组存储已经算好的之前的u、v
        //        //因为那样的话如果你武器很快的话效果就很烂了（指不够平滑圆润
        //        //这种写法虽然对电脑不太友好但是效果好（x

        //        var newVec = u2 + v2;//不过这里我们只需要最后的结果(那为什么不直接(cos2 * xScaler,sin2)，阿汪你在干什么
        //        var slashPos = drawCen + newVec;
        //        if (Main.myPlayer == drawPlayer.whoAmI)
        //        {
        //            hitboxPosition = (slashPos - drawPlayer.Center) / Main.GameViewMatrix.TransformationMatrix.M11;
        //        }

        //        bars.Add(new CustomVertexInfo(playerVisualPos + newVec, newColor, new Vector3(1 - f, 1, 3 * f / (3 * f + 1))));//(3 * f - 4) / (4 * f - 3)//快乐连顶点
        //        bars.Add(new CustomVertexInfo(playerVisualPos, newColor, new Vector3(0, 0, 0)));
        //        //oldVec = newVec;
        //    }

        //    modPlayer.HitboxPosition = hitboxPosition;
        //    if (Main.netMode == NetmodeID.MultiplayerClient)
        //    {
        //        ModPacket packet = Instance.GetPacket();
        //        packet.Write((byte)HandleNetwork.MessageType.Hitbox);
        //        packet.WritePackedVector2(modPlayer.HitboxPosition);
        //        packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
        //    }

        //    // 不开启特效就到此为止了
        //    if (!modPlayer.UseSlash)
        //    {
        //        return;
        //    }

        //    //spriteBatch.End();
        //    //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        //    //Main.NewText(new Vector3(fac, MathHelper.Clamp(modPlayer.negativeDir ? (4 * fac - 3) : 4 * fac, 0, 1), modPlayer.negativeDir ? -1 : 1));
        //    List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
        //    if (bars.Count > 2)
        //    {
        //        for (int i = 0; i < bars.Count - 2; i += 2)
        //        {
        //            triangleList.Add(bars[i]);
        //            triangleList.Add(bars[i + 2]);
        //            triangleList.Add(bars[i + 1]);

        //            triangleList.Add(bars[i + 1]);
        //            triangleList.Add(bars[i + 2]);
        //            triangleList.Add(bars[i + 3]);
        //        }
        //        ColorfulEffect.Parameters["uTransform"].SetValue(model * projection);
        //        ColorfulEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.06f);
        //        Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/BaseTex").Value;
        //        Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/AniTex").Value;
        //        Main.graphics.GraphicsDevice.Textures[2] = itemTex;

        //        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

        //        ColorfulEffect.CurrentTechnique.Passes[1 + UseItemTexForSwoosh.ToInt()].Apply();
        //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
        //        Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //    }

        //    if (fac != 0)
        //    {
        //        //var color = Main.hslToRgb(0.33f, 0.75f, 0.75f);
        //        var num0 = modPlayer.negativeDir ? 0 : 1;
        //        c[0] = new CustomVertexInfo(playerVisualPos, newColor, new Vector3(0, 1, .5f));//因为零向量固定是左下角所以纹理固定(0,1)
        //        c[1] = new CustomVertexInfo(u + playerVisualPos, newColor, new Vector3(num0 ^ 1, num0 ^ 1, .5f));//这一处也许有更优美的写法
        //        c[2] = new CustomVertexInfo(v + playerVisualPos, newColor, new Vector3(num0, num0, .5f));
        //        c[3] = c[1];
        //        c[4] = new CustomVertexInfo(u + v + playerVisualPos, newColor, new Vector3(1, 0, .5f));//因为u+v固定是右上角所以纹理固定(1,0)
        //        c[5] = c[2];
        //        //Main.spriteBatch.DrawLine(u + v + drawPlayer.Center, drawPlayer.Center, Color.Red);
        //        Main.spriteBatch.End();
        //        Main.spriteBatch.Begin(SpriteSortMode.Immediate, ItemAdditive ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        //        ItemEffect.Parameters["uTransform"].SetValue(model * projection);
        //        //将变换矩阵作用在正交投影矩阵上，具体结果以及意义我下次再想想
        //        //半年前就问过零群各位大佬，他们都说没必要搞懂，tr图像变换矩阵而已。
        //        ItemEffect.Parameters["uTime"].SetValue((float)Main.time / 60 % 1);//传入时间偏移量
        //        ItemEffect.Parameters["uItemColor"].SetValue((Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y)).ToVector4());
        //        //传入顶点绘制出的物品的颜色，这里采用环境光，和sb.Draw的那个color参数差不多(吧
        //        ItemEffect.Parameters["uItemGlowColor"].SetValue(new Color(250, 250, 250, drawPlayer.HeldItem.alpha).ToVector4());

        //        Main.graphics.GraphicsDevice.Textures[0] = itemTex;//传入物品贴图
        //        Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/Style_12").Value;
        //        Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/Style_18").Value;
        //        var g = drawPlayer.HeldItem.glowMask;
        //        if (g != -1)
        //        {
        //            //Main.graphics.GraphicsDevice.Textures[3] = TextureAssets.GlowMask[g].Value;
        //            Main.graphics.GraphicsDevice.Textures[3] = TextureAssets.GlowMask[g].Value;
        //        }
        //        if (drawPlayer.HeldItem.type == 3823)
        //        {
        //            //Main.graphics.GraphicsDevice.Textures[1] = TextureAssets.ItemFlame[3823].Value;
        //            Main.graphics.GraphicsDevice.Textures[3] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/ItemFlame_3823").Value;

        //            //ItemEffect.Parameters["uItemGlowColor"].SetValue(new Color(100, 100, 100, 0).ToVector4());

        //        }
        //        //上面这两个灰度图叠加后作为插值的t，大概是这样的映射:t=0时最终物品上的颜色是0(黑色，additive模式下是透明的)，t=0.5时是color（顶点传入的color参数，不是上面uItemColor,t=1时是1(白色)
        //        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
        //        ItemEffect.CurrentTechnique.Passes[2].Apply();//这里是第三个pass，可以直接写下标不必写pass名(
        //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, c, 0, 2);
        //        Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //    }
        //    //float num20 = (u+v).ToRotation() * drawPlayer.direction;
        //    //drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 3;
        //    //if ((double)num20 < -0.75)
        //    //{
        //    //    drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 2;
        //    //    if (drawPlayer.gravDir == -1f)
        //    //    {
        //    //        drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 4;
        //    //    }
        //    //}
        //    //if ((double)num20 > 0.6)
        //    //{
        //    //    drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 4;
        //    //    if (drawPlayer.gravDir == -1f)
        //    //    {
        //    //        drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 2;
        //    //        return;
        //    //    }
        //    //}
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
        //}

        //private void DrawSwoosh_AlphaBlend(Player drawPlayer, Color newColor)
        //{
        //    if (ColorfulEffect == null) return;
        //    if (ItemEffect == null) return;
        //    var modPlayer = drawPlayer.GetModPlayer<WeaponDisplayPlayer>();//modplayer类

        //    //drawPlayer.itemAnimation
        //    //Main.NewText(new Vector2(drawPlayer.itemAnimation , drawPlayer.itemAnimationMax - 1));
        //    var factor = (float)drawPlayer.itemAnimation / (drawPlayer.itemAnimationMax - 1);//物品挥动程度的插值，这里应该是从1到0
        //    const float cValue = 3f;
        //    var fac = 1 - (cValue - 1) * (1 - factor) * (1 - factor) - (2 - cValue) * (1 - factor);//丢到另一个插值函数里了，可以自己画一下图像，这个插值效果比上面那个线性插值好//((float)Math.Sqrt(factor) + factor) * .5f;//(cValue - 1) * factor * factor + (2 - cValue) * factor
        //    //Main.NewText(new Vector2(fac,factor));

        //    var drawCen = drawPlayer.gravDir == -1 ? new Vector2(drawPlayer.Center.X, (2 * (Main.screenPosition + Main.ScreenSize.ToVector2() / 2f) - drawPlayer.Center - new Vector2(0, 96)).Y) : drawPlayer.Center;
        //    //2 * (Main.screenPosition + new Vector2(960, 560)) - drawPlayer.Center - new Vector2(0, 96)
        //    //var fac = (float)Math.Sqrt(factor);
        //    //var theta = (fac * -1.125f + (1 - fac) * 0.1125f) * Pi;

        //    fac = modPlayer.negativeDir ? 1 - fac : fac;//每次挥动都会改变方向，所以插值函数方向也会一起变（原本是从1到0，反过来就是0到1(虽然说一般都是0到1
        //    var theta = (fac * -1.125f + (1 - fac) * 0.1125f) * MathHelper.Pi;//线性插值后乘上一个系数，这里的起始角度和终止角度是试出来的（
        //    CustomVertexInfo[] c = new CustomVertexInfo[6];//顶点数组，绘制完整的物品需要两个三角形(六个顶点，两组重合
        //    var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
        //    float xScaler = modPlayer.kValue;//获取x轴方向缩放系数
        //    float scaler = itemTex.Size().Length() * drawPlayer.GetAdjustedItemScale(drawPlayer.HeldItem) / xScaler * 0.7f * Main.GameViewMatrix.TransformationMatrix.M11;//对椭圆进行位似变换(你直接说坐标乘上一个系数不就好了吗，屑阿汪
        //    var cos = (float)Math.Cos(theta) * scaler;
        //    var sin = (float)Math.Sin(theta) * scaler;//这里(cos,sin)对应的位置就是我们希望贴图右上角所在的位置，而(0,0)对应的位置是贴图左下角所在的位置
        //    var u = new Vector2(xScaler * (cos - sin), -cos - sin).RotatedBy(modPlayer.rotationForShadow);
        //    var v = new Vector2(-xScaler * (cos + sin), sin - cos).RotatedBy(modPlayer.rotationForShadow);//这里其实应该是都要除个二，或者上面scaler那里0.7改成0.5
        //    //此处u对应的是贴图左上角或者右下角(由方向决定,v同理。u+v就是贴图右上角(剑锋位置。因为我们希望画出来是椭圆，所以我们给x方向乘上个系数，然后在根据使用朝向进行旋转就好啦
        //    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
        //    //方法名是创建正交偏移中心 l=left  r=right t=top b=bottom n=zNearPlane f=zFarPlane
        //    //( 2 / (r - l),           0,           0, -(r + l) / (r - l)
        //    //            0, 2 / (t - b),           0, -(t + b) / (t - b)
        //    //            0,           0, 2 / (n - f), -(n + f) / (f - n)
        //    //            0,           0,           0,                  1）
        //    //这尼玛的是什么鬼————
        //    //用人话说就是
        //    //x取值在[l,r]
        //    //y取值在[b,t]
        //    //z取值在[n,f]
        //    //w取值为1时
        //    //将这个b矩阵作用在(x,y,z,w)上后
        //    //x、y、z映射到[-1,1]

        //    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
        //    //假设这里丢进去是(x,y,z),出来的矩阵就是 ( 1, 0, 0, 0
        //    //  0, 1, 0, 0
        //    //  0, 0, 1, 0
        //    //  x, y, z, 1)
        //    //然后如果你将矩阵作用在(a,b,c)上就是(a, b, c, a * x + b * y + c * z + w),说实话我不是很能理解这个的意义

        //    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

        //    //Main.NewText(newColor);

        //    newColor = Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y, newColor);
        //    //newColor = Color.Red;
        //    //Main.NewText(newColor);
        //    //这一步是对贴图的颜色取加权平均数，右上角权重为1，左下角权重为0.01，左上和右下0.1
        //    //说人话就是尽量贴近剑锋处的颜色
        //    //其实我可以把武器的贴图丢到shader里然后整分多层颜色，没必要整这个加权平均
        //    //我下次试试那样的.fx

        //    List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

        //    var theta3 = (modPlayer.negativeDir ? (4 * fac - 3) : 4 * fac).Lerp(0.1125f, -1.125f, true) * MathHelper.Pi;//这里是又一处插值
        //    //我们武器所在的角度是theta，我们拖尾的末端的角度就是上面的theta3，下面的theta2就是theta渐变到theta3

        //    //var cos2 = (float)Math.Cos(theta3) * scaler;
        //    //var sin2 = (float)Math.Sin(theta3) * scaler;
        //    //var u2 = new Vector2(cos2 * xScaler + sin2, -sin2 * xScaler + cos2).RotatedBy(modPlayer.rotationForShadow);
        //    //var v2 = new Vector2(cos2 * xScaler - sin2, -sin2 * xScaler - cos2).RotatedBy(modPlayer.rotationForShadow) * length;
        //    //var oldVec = u2 + v2;

        //    // 为了联机下缩放看到别的玩家挥舞武器，武器显示在正常的地方
        //    var screenCenterPos = Main.screenPosition + Main.ScreenSize.ToVector2() / 2f;
        //    var centerToPlayerVec = drawCen - screenCenterPos; // 玩家坐标减去屏幕中心坐标得到向量
        //    float centerToPlayerLength = centerToPlayerVec.Length() * Main.GameViewMatrix.TransformationMatrix.M11; // 原距离乘屏幕缩放得到视觉距离
        //    var playerVisualPos = screenCenterPos + Vector2.Normalize(centerToPlayerVec) * centerToPlayerLength;

        //    var lightVec = Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).ToVector4();
        //    var hitboxPosition = modPlayer.HitboxPosition;
        //    for (int i = 0; i < 25; i++)
        //    {
        //        var f = i / 24f;//分割成25次惹，f从1/25f到1
        //        var theta2 = f.Lerp(theta3, theta, true);//快乐线性插值
        //        var cos2 = (float)Math.Cos(theta2) * scaler;
        //        var sin2 = (float)Math.Sin(theta2) * scaler;
        //        var u2 = new Vector2(xScaler * (cos2 - sin2), -cos2 - sin2).RotatedBy(modPlayer.rotationForShadow);
        //        var v2 = new Vector2(-xScaler * (cos2 + sin2), sin2 - cos2).RotatedBy(modPlayer.rotationForShadow);
        //        //和刚刚上面那里一样的流程，不要问我为什么不用一个数组存储已经算好的之前的u、v
        //        //因为那样的话如果你武器很快的话效果就很烂了（指不够平滑圆润
        //        //这种写法虽然对电脑不太友好但是效果好（x

        //        var newVec = u2 + v2;//不过这里我们只需要最后的结果(那为什么不直接(cos2 * xScaler,sin2)，阿汪你在干什么
        //        var slashPos = drawCen + newVec;
        //        if (Main.myPlayer == drawPlayer.whoAmI)
        //        {
        //            hitboxPosition = (slashPos - drawPlayer.Center) / Main.GameViewMatrix.TransformationMatrix.M11;
        //        }

        //        bars.Add(new CustomVertexInfo(playerVisualPos + newVec, new Color(newColor.R / 255f, newColor.G / 255f, newColor.B / 255f, f), new Vector3(1 - f, 1, .5f * lightVec.X)));//(3 * f - 4) / (4 * f - 3)//快乐连顶点//Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).R / 510f)
        //        bars.Add(new CustomVertexInfo(playerVisualPos, new Color(newColor.R / 255f, newColor.G / 255f, newColor.B / 255f, 0), new Vector3(0, 0, .5f * lightVec.X)));
        //        //oldVec = newVec;
        //    }

        //    modPlayer.HitboxPosition = hitboxPosition;
        //    if (Main.netMode == NetmodeID.MultiplayerClient)
        //    {
        //        ModPacket packet = Instance.GetPacket();
        //        packet.Write((byte)HandleNetwork.MessageType.Hitbox);
        //        packet.WritePackedVector2(modPlayer.HitboxPosition);
        //        packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
        //    }

        //    // 不开启特效就到此为止了
        //    if (!modPlayer.UseSlash)
        //    {
        //        return;
        //    }

        //    //spriteBatch.End();
        //    //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        //    //Main.NewText(new Vector3(fac, MathHelper.Clamp(modPlayer.negativeDir ? (4 * fac - 3) : 4 * fac, 0, 1), modPlayer.negativeDir ? -1 : 1));
        //    List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
        //    if (bars.Count > 2)
        //    {
        //        for (int i = 0; i < bars.Count - 2; i += 2)
        //        {
        //            triangleList.Add(bars[i]);
        //            triangleList.Add(bars[i + 2]);
        //            triangleList.Add(bars[i + 1]);

        //            triangleList.Add(bars[i + 1]);
        //            triangleList.Add(bars[i + 2]);
        //            triangleList.Add(bars[i + 3]);
        //        }
        //        ColorfulEffect.Parameters["uTransform"].SetValue(model * projection);
        //        ColorfulEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.06f);
        //        Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/BaseTex").Value;
        //        Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/AniTex").Value;
        //        Main.graphics.GraphicsDevice.Textures[2] = itemTex;

        //        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

        //        ColorfulEffect.CurrentTechnique.Passes[3 + UseItemTexForSwoosh.ToInt()].Apply();
        //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
        //        Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //    }

        //    if (fac != 0)
        //    {
        //        //var color = Main.hslToRgb(0.33f, 0.75f, 0.75f);
        //        var num0 = modPlayer.negativeDir ? 0 : 1;
        //        c[0] = new CustomVertexInfo(playerVisualPos, newColor, new Vector3(0, 1, .5f));//因为零向量固定是左下角所以纹理固定(0,1)
        //        c[1] = new CustomVertexInfo(u + playerVisualPos, newColor, new Vector3(num0 ^ 1, num0 ^ 1, .5f));//这一处也许有更优美的写法
        //        c[2] = new CustomVertexInfo(v + playerVisualPos, newColor, new Vector3(num0, num0, .5f));
        //        c[3] = c[1];
        //        c[4] = new CustomVertexInfo(u + v + playerVisualPos, newColor, new Vector3(1, 0, .5f));//因为u+v固定是右上角所以纹理固定(1,0)
        //        c[5] = c[2];
        //        //Main.spriteBatch.DrawLine(u + v + drawPlayer.Center, drawPlayer.Center, Color.Red);
        //        Main.spriteBatch.End();
        //        Main.spriteBatch.Begin(SpriteSortMode.Immediate, ItemAdditive ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        //        ItemEffect.Parameters["uTransform"].SetValue(model * projection);
        //        //将变换矩阵作用在正交投影矩阵上，具体结果以及意义我下次再想想
        //        //半年前就问过零群各位大佬，他们都说没必要搞懂，tr图像变换矩阵而已。
        //        ItemEffect.Parameters["uTime"].SetValue((float)Main.time / 60 % 1);//传入时间偏移量
        //        ItemEffect.Parameters["uItemColor"].SetValue(lightVec);
        //        ItemEffect.Parameters["uItemGlowColor"].SetValue(new Color(250, 250, 250, drawPlayer.HeldItem.alpha).ToVector4());

        //        //传入顶点绘制出的物品的颜色，这里采用环境光，和sb.Draw的那个color参数差不多(吧
        //        Main.graphics.GraphicsDevice.Textures[0] = itemTex;//传入物品贴图
        //        Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/Style_12").Value;
        //        Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/Style_18").Value;

        //        var g = drawPlayer.HeldItem.glowMask;
        //        if (g != -1)
        //        {
        //            //Main.graphics.GraphicsDevice.Textures[3] = TextureAssets.GlowMask[g].Value;
        //            //ItemEffect.Parameters["uGlowTex"].SetValue(TextureAssets.GlowMask[g].Value);
        //            Main.graphics.GraphicsDevice.Textures[3] = TextureAssets.GlowMask[g].Value;

        //        }
        //        if (drawPlayer.HeldItem.type == 3823)
        //        {

        //            Main.graphics.GraphicsDevice.Textures[3] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/ItemFlame_3823").Value;
        //            //ItemEffect.Parameters["uItemGlowColor"].SetValue(new Color(100, 100, 100, 0).ToVector4());
        //        }

        //        //上面这两个灰度图叠加后作为插值的t，大概是这样的映射:t=0时最终物品上的颜色是0(黑色，additive模式下是透明的)，t=0.5时是color（顶点传入的color参数，不是上面uItemColor,t=1时是1(白色)
        //        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
        //        Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.PointWrap;

        //        ItemEffect.CurrentTechnique.Passes[2].Apply();//这里是第三个pass，可以直接写下标不必写pass名(
        //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, c, 0, 2);
        //        Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //    }
        //    //float num20 = (u+v).ToRotation() * drawPlayer.direction;
        //    //drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 3;
        //    //if ((double)num20 < -0.75)
        //    //{
        //    //    drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 2;
        //    //    if (drawPlayer.gravDir == -1f)
        //    //    {
        //    //        drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 4;
        //    //    }
        //    //}
        //    //if ((double)num20 > 0.6)
        //    //{
        //    //    drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 4;
        //    //    if (drawPlayer.gravDir == -1f)
        //    //    {
        //    //        drawPlayer.bodyFrame.Y = drawPlayer.bodyFrame.Height * 2;
        //    //        return;
        //    //    }
        //    //}
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
        //}
        #endregion

    }
}