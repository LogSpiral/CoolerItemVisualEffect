using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;

namespace CoolerItemVisualEffect
{
    public class TestDrawModifyPlayer : ModPlayer
    {
        public bool autoSquat = true;
        public bool squat;
        public int squatCounter;
        public int jumpCounter;
        public bool jump;
        public override void ResetEffects()
        {
            if (!ConfigurationNormal.instance.CelesteMoveAnimation) return;
            if (Player.controlJump && Player.velocity.Y == 0)
            {
                jump = true;
            }
            else if (Player.velocity.Y == 0)
            {
                if (autoSquat)
                    squat = Player.controlDown;
                jump = false;
                jumpCounter = 0;
            }
            squatCounter += squat ? 1 : -2;
            squatCounter = (int)MathHelper.Clamp(squatCounter, 0, 10);
            if (jumpCounter == 10) { jump = false; jumpCounter = 0; }
            if (jump && jumpCounter < 10) jumpCounter++;
            //Main.NewText(Player.legFrame);
            if (Player.controlDown)
            {
                if (Player.velocity.Y == 0)
                {
                    Player.velocity *= new Vector2(0f, 1f);
                }
                else
                {
                    Player.maxFallSpeed = 24;
                    Player.velocity.Y = MathHelper.Clamp(Player.velocity.Y + .5f, Player.velocity.Y, Player.maxFallSpeed);
                }
            }
            //autoSquat = true;
        }
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            //PlayerDrawLayers.Skin.Hide();
            //PlayerDrawLayers.Leggings.Hide();
            base.HideDrawLayers(drawInfo);
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //drawInfo.Position += new Vector2(64);
            if (!ConfigurationNormal.instance.CelesteMoveAnimation) return;

            bool flag = Player.itemAnimation > 0;
            flag &= Player.HeldItem.DamageType == DamageClass.Ranged && Player.HeldItem.useStyle == 5;
            if (squat)
            {
                var fac = squatCounter == 0 ? 1f : (squat ? 0.6f + squatCounter / 50f : 1 - squatCounter / 50f);
                fac = 1 - fac;
                drawInfo.Position += new Vector2(0, 64 * fac);

                Player.bodyFrame.Y = 616 + (squatCounter - 7) * 56;
                if (squatCounter < 7) Player.bodyFrame.Y = 280;
                if (flag) Player.bodyFrame.Y = 896;
                if (Player.velocity.Y == 0)
                {
                    if (flag)
                    {
                        Player.legFrame.Y = 616 + (int)MathHelper.Clamp(squatCounter * 3 / 4 * 56, 0, 280);
                        //Player.legFrame.Y = squatCounter < 2 ? 0 : (squatCounter < 4 ? 1064 : 392 + 56 * ((squatCounter - 3) / 2));
                    }
                    else
                    {
                        drawInfo.legsOffset += new Vector2(0, -2f);
                        Player.legFrame.Y = 560;
                    }

                }
                else
                {
                    drawInfo.legsOffset += new Vector2(0, -2f);
                }
            }
            else if (squatCounter > 0 && flag)
            {
                if (Player.velocity.Y == 0)
                {
                    Player.legFrame.Y = 616 + (int)MathHelper.Clamp(squatCounter * 3 / 4 * 56, 0, 280);
                }
            }
            base.ModifyDrawInfo(ref drawInfo);
        }
    }
    public class SkinModifyLayer : PlayerDrawLayer
    {
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            return;
            Player player = drawInfo.drawPlayer;
            var modplr = player.GetModPlayer<TestDrawModifyPlayer>();
            if (modplr.squat)
            {
                if (drawInfo.usesCompositeTorso)
                {
                    PlayerDrawLayers.DrawPlayer_12_Skin_Composite(ref drawInfo);
                    return;
                }
                if (drawInfo.isSitting)
                    drawInfo.hidesBottomSkin = true;

                if (!drawInfo.hidesTopSkin)
                {
                    drawInfo.Position.Y += drawInfo.torsoOffset;
                    DrawData drawData = new DrawData(TextureAssets.Players[drawInfo.skinVar, 3].Value, new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2), drawInfo.drawPlayer.bodyFrame, drawInfo.colorBodySkin, drawInfo.drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
                    drawData.shader = drawInfo.skinDyePacked;
                    DrawData item = drawData;
                    drawInfo.DrawDataCache.Add(item);
                    drawInfo.Position.Y -= drawInfo.torsoOffset;
                }

                if (!drawInfo.hidesBottomSkin && !drawInfo.isBottomOverriden)
                {
                    LeggingModifyLayer.DrawSittingLegs(ref drawInfo, TextureAssets.Players[drawInfo.skinVar, 10].Value, drawInfo.colorLegs, 0, false, -.5f);
                }
            }
            else
            {
                PlayerDrawLayers.DrawPlayer_12_Skin(ref drawInfo);
            }
            //for (int n = 0; n < 6; n++)
            //{
            //    DrawData data = new DrawData(TextureAssets.MagicPixel.Value, drawInfo.drawPlayer.Center - Main.screenPosition + new Vector2(0, -48),
            //        new Rectangle(0, 0, 1, 1), Main.DiscoColor * (.2f * (1 + n)), ((float)Main.time + n * 5) / 30f, default, 20f, 0, 0);
            //    //data.shader = drawInfo.cHead;
            //    drawInfo.DrawDataCache.Add(data);
            //}
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return true;
        }
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Skin);
        }
    }
    public class LeggingModifyLayer : PlayerDrawLayer
    {
        public static void DrawSittingLegs(ref PlayerDrawSet drawinfo, Texture2D textureToDraw, Color matchingColor, int shaderIndex = 0, bool glowmask = false, float offseter = 1f, float factor = 1f)
        {
            Vector2 legsOffset = drawinfo.legsOffset;
            Vector2 value = new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.legFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.legFrame.Height + 4f)) + drawinfo.drawPlayer.legPosition + drawinfo.legVect;
            Rectangle legFrame = drawinfo.drawPlayer.legFrame;
            value.Y -= 2f;
            value.Y += drawinfo.seatYOffset;
            value += legsOffset;
            int num = 2;
            int offsetY = 42;
            int times = 2;
            int num4 = 2;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            float step = 2 * factor;
            bool flag = drawinfo.drawPlayer.legs == 101 || drawinfo.drawPlayer.legs == 102 || drawinfo.drawPlayer.legs == 118 || drawinfo.drawPlayer.legs == 99;
            if (drawinfo.drawPlayer.wearsRobe && !flag)
            {
                num = 0;
                num4 = 0;
                offsetY = 6;
                value.Y += 4f;
                legFrame.Y = legFrame.Height * 5;
            }

            switch (drawinfo.drawPlayer.legs)
            {
                case 214:
                case 215:
                case 216:
                    num = -6;
                    num4 = 2;
                    num5 = 2;
                    times = 4;
                    offsetY = 6;
                    legFrame = drawinfo.drawPlayer.legFrame;
                    value.Y += 2f;
                    break;
                case 106:
                case 143:
                case 226:
                    num = 0;
                    num4 = 0;
                    offsetY = 6;
                    value.Y += 4f;
                    legFrame.Y = legFrame.Height * 5;
                    break;
                case 132:
                    num = -2;
                    num7 = 2;
                    break;
                case 193:
                case 194:
                    if (drawinfo.drawPlayer.body == 218)
                    {
                        num = -2;
                        num7 = 2;
                        value.Y += 2f;
                    }
                    break;
                case 210:
                    if (glowmask)
                    {
                        Vector2 vector = new Vector2((float)Main.rand.Next(-10, 10) * 0.125f, (float)Main.rand.Next(-10, 10) * 0.125f);
                        value += vector;
                    }
                    break;
            }

            for (int i = times; i >= 0; i--)
            {
                Vector2 position = value + new Vector2(num, 2f) * new Vector2(drawinfo.drawPlayer.direction, 1f);
                Rectangle realFrame = legFrame;
                realFrame.Y += (int)(i * step);
                realFrame.Y += offsetY;
                realFrame.Height -= offsetY;
                realFrame.Height -= (int)(i * step);
                if (i != times)
                    realFrame.Height = (int)step;

                position.X += (drawinfo.drawPlayer.direction * num4 * i + num6 * drawinfo.drawPlayer.direction) * offseter;
                if (i != 0)
                    position.X += num7 * drawinfo.drawPlayer.direction;

                position.Y += offsetY;
                position.Y += num5;
                DrawData item = new DrawData(textureToDraw, position, realFrame, matchingColor, drawinfo.drawPlayer.legRotation, drawinfo.legVect, 1f, drawinfo.playerEffect, 0);
                item.shader = shaderIndex;
                drawinfo.DrawDataCache.Add(item);
            }
        }
        public override void Load()
        {

            Main.PlayerRenderer = new CoolerPlayerRender();
            base.Load();
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            return;
            Player player = drawInfo.drawPlayer;
            var modplr = player.GetModPlayer<TestDrawModifyPlayer>();
            if (modplr.squat)
            {
                if (!player.invis)
                {
                    DrawSittingLegs(ref drawInfo, TextureAssets.ArmorLeg[drawInfo.drawPlayer.legs].Value, drawInfo.colorArmorLegs, drawInfo.cLegs, false, -.5f, modplr.squatCounter / 10f);
                    if (drawInfo.legsGlowMask != -1)
                    {
                        DrawSittingLegs(ref drawInfo, TextureAssets.GlowMask[drawInfo.legsGlowMask].Value, drawInfo.legsGlowColor, drawInfo.cLegs, false, -.5f, modplr.squatCounter / 10f);
                    }
                }
            }
            else
            {
                PlayerDrawLayers.DrawPlayer_13_Leggings(ref drawInfo);
            }
            for (int n = 0; n < 6; n++)
            {
                DrawData data = new DrawData(TextureAssets.MagicPixel.Value, drawInfo.drawPlayer.Center - Main.screenPosition + new Vector2(0, -48),
                    new Rectangle(0, 0, 1, 1), Main.DiscoColor * (.2f * (1 + n)), ((float)Main.time + n * 5) / 30f, default, 20f, 0, 0);
                //data.shader = drawInfo.cHead;
                drawInfo.DrawDataCache.Add(data);
            }
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return true;
        }
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Leggings);
        }
    }
    public class CoolerPlayerRender : IPlayerRenderer
    {
        private readonly List<DrawData> _drawData = new List<DrawData>();
        private readonly List<int> _dust = new List<int>();
        private readonly List<int> _gore = new List<int>();
        public static SamplerState MountedSamplerState
        {
            get
            {
                if (!Main.drawToScreen)
                    return SamplerState.AnisotropicClamp;

                return SamplerState.LinearClamp;
            }
        }

        public void DrawPlayers(Camera camera, IEnumerable<Player> players)
        {
            foreach (Player player in players)
            {
                DrawPlayerFull(camera, player);
            }
        }

        public void DrawPlayerHead(Camera camera, Player drawPlayer, Vector2 position, float alpha = 1f, float scale = 1f, Color borderColor = default(Color))
        {
            /*
			if (!drawPlayer.ShouldNotDraw) {
				_drawData.Clear();
				_dust.Clear();
				_gore.Clear();
				PlayerDrawHeadSet drawinfo = default(PlayerDrawHeadSet);
				drawinfo.BoringSetup(drawPlayer, _drawData, _dust, _gore, position.X, position.Y, alpha, scale);
				PlayerDrawHeadLayers.DrawPlayer_00_BackHelmet(ref drawinfo);
				PlayerDrawHeadLayers.DrawPlayer_01_FaceSkin(ref drawinfo);
				PlayerDrawHeadLayers.DrawPlayer_02_DrawArmorWithFullHair(ref drawinfo);
				PlayerDrawHeadLayers.DrawPlayer_03_HelmetHair(ref drawinfo);
				PlayerDrawHeadLayers.DrawPlayer_04_HatsWithFullHair(ref drawinfo);
				PlayerDrawHeadLayers.DrawPlayer_05_TallHats(ref drawinfo);
				PlayerDrawHeadLayers.DrawPlayer_06_NormalHats(ref drawinfo);
				PlayerDrawHeadLayers.DrawPlayer_07_JustHair(ref drawinfo);
				PlayerDrawHeadLayers.DrawPlayer_08_FaceAcc(ref drawinfo);
				CreateOutlines(alpha, scale, borderColor);
				PlayerDrawHeadLayers.DrawPlayer_RenderAllLayers(ref drawinfo);
			}
			*/

            DrawPlayerInternal(camera, drawPlayer, position + Main.screenPosition, 0f, Vector2.Zero, alpha: alpha, scale: scale, headOnly: true);
        }

        private void CreateOutlines(float alpha, float scale, Color borderColor)
        {
            if (!(borderColor != Color.Transparent))
                return;

            List<DrawData> collection = new List<DrawData>(_drawData);
            List<DrawData> list = new List<DrawData>(_drawData);
            float num = 2f * scale;
            Color color = borderColor;
            color *= alpha * alpha;
            Color black = Color.Black;
            black *= alpha * alpha;
            int colorOnlyShaderIndex = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            for (int i = 0; i < list.Count; i++)
            {
                DrawData value = list[i];
                value.shader = colorOnlyShaderIndex;
                value.color = black;
                list[i] = value;
            }

            int num2 = 2;
            Vector2 vector;
            for (int j = -num2; j <= num2; j++)
            {
                for (int k = -num2; k <= num2; k++)
                {
                    if (Math.Abs(j) + Math.Abs(k) == num2)
                    {
                        vector = new Vector2((float)j * num, (float)k * num);
                        for (int l = 0; l < list.Count; l++)
                        {
                            DrawData item = list[l];
                            item.position += vector;
                            _drawData.Add(item);
                        }
                    }
                }
            }

            for (int m = 0; m < list.Count; m++)
            {
                DrawData value2 = list[m];
                value2.shader = colorOnlyShaderIndex;
                value2.color = color;
                list[m] = value2;
            }

            vector = Vector2.Zero;
            num2 = 1;
            for (int n = -num2; n <= num2; n++)
            {
                for (int num3 = -num2; num3 <= num2; num3++)
                {
                    if (Math.Abs(n) + Math.Abs(num3) == num2)
                    {
                        vector = new Vector2((float)n * num, (float)num3 * num);
                        for (int num4 = 0; num4 < list.Count; num4++)
                        {
                            DrawData item2 = list[num4];
                            item2.position += vector;
                            _drawData.Add(item2);
                        }
                    }
                }
            }

            _drawData.AddRange(collection);
        }

        public void DrawPlayer(Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow = 0f, float scale = 1f)
        {
            DrawPlayerInternal(camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale);
        }

        private void DrawPlayerInternal(Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow = 0f, float alpha = 1f, float scale = 1f, bool headOnly = false)
        {
            if (drawPlayer.ShouldNotDraw)
                return;

            PlayerDrawSet drawinfo = default(PlayerDrawSet);
            _drawData.Clear();
            _dust.Clear();
            _gore.Clear();

            if (headOnly)
            {
                drawinfo.HeadOnlySetup(drawPlayer, _drawData, _dust, _gore, position.X, position.Y, alpha, scale);
            }
            else
            {
                drawinfo.BoringSetup(drawPlayer, _drawData, _dust, _gore, position, shadow, rotation, rotationOrigin);
            }
            PlayerLoader.ModifyDrawInfo(ref drawinfo);

            foreach (var layer in PlayerDrawLayerLoader.GetDrawLayers(drawinfo))
            {
                if (!headOnly || layer.IsHeadLayer)
                {
                    layer.DrawWithTransformationAndChildren(ref drawinfo);
                }
            }

            PlayerDrawLayers.DrawPlayer_MakeIntoFirstFractalAfterImage(ref drawinfo);
            PlayerDrawLayers.DrawPlayer_TransformDrawData(ref drawinfo);
            #region OtherTransform

            #region 跳跃压缩

            var modplr = drawPlayer.GetModPlayer<TestDrawModifyPlayer>();

            if (!headOnly && ConfigurationNormal.instance.CelesteMoveAnimation)
            {
                Vector2 scaler = new Vector2(1f - MathHelper.Clamp((Math.Abs(drawPlayer.velocity.Y) / 3f + (modplr.jumpCounter != 0 ? 9 - modplr.jumpCounter : 0)) / 45f, 0, .2f) * 1.5f
                    , modplr.squatCounter == 0 ? 1f : (modplr.squat ? 0.6f + modplr.squatCounter / 50f : 1 - modplr.squatCounter / 50f));
                Vector2 drawCenter = position - Main.screenPosition;//drawPlayer.Center
                for (int k = 0; k < drawinfo.DrawDataCache.Count; k++)
                {
                    DrawData value3 = drawinfo.DrawDataCache[k];
                    value3.position = (value3.position - drawCenter) * scaler + drawCenter;
                    value3.scale *= scaler;
                    drawinfo.DrawDataCache[k] = value3;
                }
            }

            #endregion

            #endregion
            if (scale != 1f)
                PlayerDrawLayers.DrawPlayer_ScaleDrawData(ref drawinfo, scale);

            PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawinfo);



            if (!drawinfo.drawPlayer.mount.Active || drawinfo.drawPlayer.mount.Type != 11)
                goto mylabel;

            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == drawinfo.drawPlayer.whoAmI && Main.projectile[i].type == 591)
                    Main.instance.DrawProj(i);
            }
        mylabel:
            if (!drawPlayer.isFirstFractalAfterImage && shadow == 0f && !headOnly)
                CoolerItemVisualEffect.DrawSwooshWithPlayer(drawPlayer);
        }

        private void DrawPlayerFull(Camera camera, Player drawPlayer)
        {
            SpriteBatch spriteBatch = camera.SpriteBatch;
            SamplerState samplerState = camera.Sampler;
            if (drawPlayer.mount.Active && drawPlayer.fullRotation != 0f)
                samplerState = MountedSamplerState;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, samplerState, DepthStencilState.None, camera.Rasterizer, null, camera.GameViewMatrix.TransformationMatrix);
            if (Main.gamePaused)
                drawPlayer.PlayerFrame();

            if (drawPlayer.ghost)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vector = drawPlayer.shadowPos[i];
                    vector = drawPlayer.position - drawPlayer.velocity * (2 + i * 2);
                    DrawGhost(camera, drawPlayer, vector, 0.5f + 0.2f * (float)i);
                }

                DrawGhost(camera, drawPlayer, drawPlayer.position);
            }
            else
            {
                if (drawPlayer.inventory[drawPlayer.selectedItem].flame || drawPlayer.head == 137 || drawPlayer.wings == 22)
                {
                    drawPlayer.itemFlameCount--;
                    if (drawPlayer.itemFlameCount <= 0)
                    {
                        drawPlayer.itemFlameCount = 5;
                        for (int j = 0; j < 7; j++)
                        {
                            drawPlayer.itemFlamePos[j].X = (float)Main.rand.Next(-10, 11) * 0.15f;
                            drawPlayer.itemFlamePos[j].Y = (float)Main.rand.Next(-10, 1) * 0.35f;
                        }
                    }
                }

                if (drawPlayer.armorEffectDrawShadowEOCShield)
                {
                    int num = drawPlayer.eocDash / 4;
                    if (num > 3)
                        num = 3;

                    for (int k = 0; k < num; k++)
                    {
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[k], drawPlayer.shadowRotation[k], drawPlayer.shadowOrigin[k], 0.5f + 0.2f * (float)k);
                    }
                }

                Vector2 position = default(Vector2);
                if (drawPlayer.invis)
                {
                    drawPlayer.armorEffectDrawOutlines = false;
                    drawPlayer.armorEffectDrawShadow = false;
                    drawPlayer.armorEffectDrawShadowSubtle = false;
                    position = drawPlayer.position;
                    if (drawPlayer.aggro <= -750)
                    {
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 1f);
                    }
                    else
                    {
                        drawPlayer.invis = false;
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin);
                        drawPlayer.invis = true;
                    }
                }

                if (drawPlayer.armorEffectDrawOutlines)
                {
                    _ = drawPlayer.position;
                    if (!Main.gamePaused)
                        drawPlayer.ghostFade += drawPlayer.ghostDir * 0.075f;

                    if ((double)drawPlayer.ghostFade < 0.1)
                    {
                        drawPlayer.ghostDir = 1f;
                        drawPlayer.ghostFade = 0.1f;
                    }
                    else if ((double)drawPlayer.ghostFade > 0.9)
                    {
                        drawPlayer.ghostDir = -1f;
                        drawPlayer.ghostFade = 0.9f;
                    }

                    float num2 = drawPlayer.ghostFade * 5f;
                    for (int l = 0; l < 4; l++)
                    {
                        float num3;
                        float num4;
                        switch (l)
                        {
                            default:
                                num3 = num2;
                                num4 = 0f;
                                break;
                            case 1:
                                num3 = 0f - num2;
                                num4 = 0f;
                                break;
                            case 2:
                                num3 = 0f;
                                num4 = num2;
                                break;
                            case 3:
                                num3 = 0f;
                                num4 = 0f - num2;
                                break;
                        }

                        position = new Vector2(drawPlayer.position.X + num3, drawPlayer.position.Y + drawPlayer.gfxOffY + num4);
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, drawPlayer.ghostFade);
                    }
                }

                if (drawPlayer.armorEffectDrawOutlinesForbidden)
                {
                    _ = drawPlayer.position;
                    if (!Main.gamePaused)
                        drawPlayer.ghostFade += drawPlayer.ghostDir * 0.025f;

                    if ((double)drawPlayer.ghostFade < 0.1)
                    {
                        drawPlayer.ghostDir = 1f;
                        drawPlayer.ghostFade = 0.1f;
                    }
                    else if ((double)drawPlayer.ghostFade > 0.9)
                    {
                        drawPlayer.ghostDir = -1f;
                        drawPlayer.ghostFade = 0.9f;
                    }

                    float num5 = drawPlayer.ghostFade * 5f;
                    for (int m = 0; m < 4; m++)
                    {
                        float num6;
                        float num7;
                        switch (m)
                        {
                            default:
                                num6 = num5;
                                num7 = 0f;
                                break;
                            case 1:
                                num6 = 0f - num5;
                                num7 = 0f;
                                break;
                            case 2:
                                num6 = 0f;
                                num7 = num5;
                                break;
                            case 3:
                                num6 = 0f;
                                num7 = 0f - num5;
                                break;
                        }

                        position = new Vector2(drawPlayer.position.X + num6, drawPlayer.position.Y + drawPlayer.gfxOffY + num7);
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, drawPlayer.ghostFade);
                    }
                }

                if (drawPlayer.armorEffectDrawShadowBasilisk)
                {
                    int num8 = (int)(drawPlayer.basiliskCharge * 3f);
                    for (int n = 0; n < num8; n++)
                    {
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[n], drawPlayer.shadowRotation[n], drawPlayer.shadowOrigin[n], 0.5f + 0.2f * (float)n);
                    }
                }
                else if (drawPlayer.armorEffectDrawShadow)
                {
                    for (int num9 = 0; num9 < 3; num9++)
                    {
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[num9], drawPlayer.shadowRotation[num9], drawPlayer.shadowOrigin[num9], 0.5f + 0.2f * (float)num9);
                    }
                }

                if (drawPlayer.armorEffectDrawShadowLokis)
                {
                    for (int num10 = 0; num10 < 3; num10++)
                    {
                        DrawPlayer(camera, drawPlayer, Vector2.Lerp(drawPlayer.shadowPos[num10], drawPlayer.position + new Vector2(0f, drawPlayer.gfxOffY), 0.5f), drawPlayer.shadowRotation[num10], drawPlayer.shadowOrigin[num10], MathHelper.Lerp(1f, 0.5f + 0.2f * (float)num10, 0.5f));
                    }
                }

                if (drawPlayer.armorEffectDrawShadowSubtle)
                {
                    for (int num11 = 0; num11 < 4; num11++)
                    {
                        position.X = drawPlayer.position.X + (float)Main.rand.Next(-20, 21) * 0.1f;
                        position.Y = drawPlayer.position.Y + (float)Main.rand.Next(-20, 21) * 0.1f + drawPlayer.gfxOffY;
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.9f);
                    }
                }

                if (drawPlayer.shadowDodge)
                {
                    drawPlayer.shadowDodgeCount += 1f;
                    if (drawPlayer.shadowDodgeCount > 30f)
                        drawPlayer.shadowDodgeCount = 30f;
                }
                else
                {
                    drawPlayer.shadowDodgeCount -= 1f;
                    if (drawPlayer.shadowDodgeCount < 0f)
                        drawPlayer.shadowDodgeCount = 0f;
                }

                if (drawPlayer.shadowDodgeCount > 0f)
                {
                    _ = drawPlayer.position;
                    position.X = drawPlayer.position.X + drawPlayer.shadowDodgeCount;
                    position.Y = drawPlayer.position.Y + drawPlayer.gfxOffY;
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
                    position.X = drawPlayer.position.X - drawPlayer.shadowDodgeCount;
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
                }

                if (drawPlayer.brainOfConfusionDodgeAnimationCounter > 0)
                {
                    Vector2 value = drawPlayer.position + new Vector2(0f, drawPlayer.gfxOffY);
                    float lerpValue = Utils.GetLerpValue(300f, 270f, drawPlayer.brainOfConfusionDodgeAnimationCounter);
                    float y = MathHelper.Lerp(2f, 120f, lerpValue);
                    if (lerpValue >= 0f && lerpValue <= 1f)
                    {
                        for (float num12 = 0f; num12 < (float)Math.PI * 2f; num12 += (float)Math.PI / 3f)
                        {
                            position = value + new Vector2(0f, y).RotatedBy((float)Math.PI * 2f * lerpValue * 0.5f + num12);
                            DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, lerpValue);
                        }
                    }
                }

                position = drawPlayer.position;
                position.Y += drawPlayer.gfxOffY;
                if (drawPlayer.stoned)
                    DrawPlayerStoned(camera, drawPlayer, position);
                else if (!drawPlayer.invis)
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin);
            }

            spriteBatch.End();
        }

        private void DrawPlayerStoned(Camera camera, Player drawPlayer, Vector2 position)
        {
            if (!drawPlayer.dead)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                spriteEffects = ((drawPlayer.direction != 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                camera.SpriteBatch.Draw(TextureAssets.Extra[37].Value, new Vector2((int)(position.X - camera.UnscaledPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (int)(position.Y - camera.UnscaledPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 8f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2), null, Lighting.GetColor((int)((double)position.X + (double)drawPlayer.width * 0.5) / 16, (int)((double)position.Y + (double)drawPlayer.height * 0.5) / 16, Color.White), 0f, new Vector2(TextureAssets.Extra[37].Width() / 2, TextureAssets.Extra[37].Height() / 2), 1f, spriteEffects, 0f);
            }
        }

        private void DrawGhost(Camera camera, Player drawPlayer, Vector2 position, float shadow = 0f)
        {
            byte mouseTextColor = Main.mouseTextColor;
            SpriteEffects effects = (drawPlayer.direction != 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color immuneAlpha = drawPlayer.GetImmuneAlpha(Lighting.GetColor((int)((double)drawPlayer.position.X + (double)drawPlayer.width * 0.5) / 16, (int)((double)drawPlayer.position.Y + (double)drawPlayer.height * 0.5) / 16, new Color((int)mouseTextColor / 2 + 100, (int)mouseTextColor / 2 + 100, (int)mouseTextColor / 2 + 100, (int)mouseTextColor / 2 + 100)), shadow);
            immuneAlpha.A = (byte)((float)(int)immuneAlpha.A * (1f - Math.Max(0.5f, shadow - 0.5f)));
            Rectangle value = new Rectangle(0, TextureAssets.Ghost.Height() / 4 * drawPlayer.ghostFrame, TextureAssets.Ghost.Width(), TextureAssets.Ghost.Height() / 4);
            Vector2 origin = new Vector2((float)value.Width * 0.5f, (float)value.Height * 0.5f);
            camera.SpriteBatch.Draw(TextureAssets.Ghost.Value, new Vector2((int)(position.X - camera.UnscaledPosition.X + (float)(value.Width / 2)), (int)(position.Y - camera.UnscaledPosition.Y + (float)(value.Height / 2))), value, immuneAlpha, 0f, origin, 1f, effects, 0f);
        }
    }
}
