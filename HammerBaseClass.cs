using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using static CoolerItemVisualEffect.CoolerItemVisualEffectMethods;

namespace CoolerItemVisualEffect
{
    public interface IHammerProj
    {
        //string HammerName { get; }
        Vector2 CollidingSize { get; }
        Vector2 CollidingCenter { get; }
        Vector2 DrawOrigin { get; }
        Texture2D projTex { get; }
        Vector2 projCenter { get; }
        Rectangle? frame { get; }
        Color color { get; }
        float Rotation { get; }
        Vector2 scale { get; }
        SpriteEffects flip { get; }
        (int X, int Y) FrameMax { get; }
        Player Player { get; }
    }
    public abstract class HammerProj : ModProjectile, IHammerProj
    {
        public virtual Vector2 scale => new Vector2(1);
        public virtual Rectangle? frame => null;
        public virtual Vector2 projCenter => Player.Center + new Vector2(0, Player.gfxOffY);
        public Projectile projectile => Projectile;
        public virtual bool Charged => factor > 0.75f;
        public virtual SpriteEffects flip => Player.direction == -1 ? SpriteEffects.FlipHorizontally : 0;
        public virtual (int X, int Y) FrameMax => (1, 1);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(HammerName);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.scale = 1f;
            //projectile.alpha = 255;
            projectile.hide = false;
            projectile.ownerHitCheck = true;
            projectile.DamageType = DamageClass.Melee;
            projectile.tileCollide = false;
            projectile.friendly = true;
        }
        public virtual void OnCharging(bool left, bool right)
        {

        }
        public virtual void OnRelease(bool charged, bool left)
        {
            if ((int)projectile.ai[1] == 0)
            {
                projectile.damage = 0;
                if (Charged)
                {
                    projectile.damage = (int)(Player.GetWeaponDamage(Player.HeldItem) * (3 * factor * factor));
                    SoundEngine.PlaySound(SoundID.Item71);
                }
            }
            projectile.ai[1]++;
            if (projectile.ai[1] > (Charged ? (MaxTimeLeft * factor) : timeCount))
            {
                projectile.Kill();
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((int)projectile.ai[1] == 0)
            {
                return false;
            }
            float point = 0f;
            return targetHitbox.Intersects(Utils.CenteredRectangle((CollidingCenter - DrawOrigin).RotatedBy(Rotation) + projCenter, CollidingSize)) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projCenter, (CollidingCenter - DrawOrigin).RotatedBy(Rotation) + projCenter, 8, ref point);
            //float point = 0f;
            //Vector2 vec = Pos - player.Center;
            //vec.Normalize();
            //bool flag2 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center - vec * (30 - Distance - NegativeDistance), player.Center + vec * (66 + Distance + NegativeDistance), 18, ref point);
            //return flag2;
        }
        //public float Rotation => projectile.ai[1] > 0 ? ((int)factor == 1 ? (projectile.ai[1] / 5).Lerp(-MathHelper.PiOver2, MathHelper.Pi / 8 * 3) : ((timeCount - projectile.ai[1]) / MaxTime).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2)) : ((float)Math.Pow(factor,2)).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2 - MathHelper.Pi / 8);//MathHelper.Pi / 8 * 3 - factor * (MathHelper.Pi / 8 * 7)
        public virtual float Rotation
        {
            get
            {
                //Main.NewText(timeCount);
                var theta = ((float)Math.Pow(factor, 2)).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2 - MathHelper.Pi / 8);
                if (projectile.ai[1] > 0)
                {
                    if (Charged)
                    {
                        //Main.NewText(projectile.ai[1] / MaxTimeLeft / factor);
                        theta = (projectile.ai[1] / MaxTimeLeft / factor).Lerp(theta, MathHelper.Pi / 8 * 3);
                        //return player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;
                    }
                    else
                    {
                        theta = ((timeCount - projectile.ai[1]) / MaxTime).Lerp(MathHelper.Pi / 8 * 3, -MathHelper.PiOver2);
                        //return player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;
                    }
                }
                return Player.direction == -1 ? MathHelper.Pi * 1.5f - theta : theta;

            }
        }
        public Player Player => Main.player[projectile.owner];

        public virtual float timeCount
        {
            get => projectile.ai[0];
            set
            {
                projectile.ai[0] = MathHelper.Clamp(value, 0, MaxTime);
            }
        }
        public Texture2D projTex => TextureAssets.Projectile[projectile.type].Value;
        public virtual string HammerName => "做个锤子";
        public virtual float MaxTime => 15;
        public virtual float factor => timeCount / MaxTime;
        public virtual Vector2 CollidingSize => new Vector2(32);
        public virtual Vector2 CollidingCenter => new Vector2(projTex.Size().X / FrameMax.X - 16, 16);
        public virtual Vector2 DrawOrigin => new Vector2(16, projTex.Size().Y / FrameMax.Y - 16);

        public virtual Color color => /*projectile.GetAlpha(Color.White);*/Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, Color.White);
        public virtual float MaxTimeLeft => 5;
        public virtual bool UseLeft => true;
        public virtual bool UseRight => false;
        public virtual bool Charging => (UseLeft && Player.controlUseItem) || (UseRight && Player.controlUseTile);
        public override void AI()
        {
            //Projectiles.KluexEnergyCrystal.KluexEnergyZone
            if (Player.dead) projectile.Kill();
            if (Charging && projectile.ai[1] == 0)
            {
                OnCharging(Player.controlUseItem, Player.controlUseTile);
                timeCount++;
                if (Player.controlUseItem)
                {
                    controlState = 1;
                }
                if (Player.controlUseTile)
                {
                    controlState = 2;
                }
            }
            else
            {
                OnRelease(Charged, controlState == 1);
            }
            projectile.timeLeft = 2;
            Player.heldProj = projectile.whoAmI;
            Player.RotatedRelativePoint(Player.MountedCenter, true);
            Player.itemTime = 2;
            Player.itemAnimation = 2;
            Player.ChangeDir(Math.Sign((Main.MouseWorld - projCenter).X));
            Player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, Rotation - (Player.direction == -1 ? MathHelper.Pi : MathHelper.PiOver2));// -MathHelper.PiOver2

            projectile.Center = Player.Center + new Vector2(0, Player.gfxOffY);

        }
        public byte controlState;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.DrawHammer(this);
            return false;
        }
    }
    public abstract class VertexHammerProj : HammerProj
    {
        public override float Rotation => base.Rotation;
        public virtual CustomVertexInfo[] CreateVertexs(Vector2 drawCen, float scaler, float startAngle, float endAngle, float alphaLight, ref int[] whenSkip)
        {
            var bars = new CustomVertexInfo[90];
            for (int i = 0; i < 45; i++)
            {
                var f = i / 44f;
                //var newVec = (endAngle.AngleLerp(startAngle, f) - MathHelper.PiOver4).ToRotationVector2() * scaler;
                var newVec = (f.Lerp(endAngle + (Player.direction == -1 ? MathHelper.TwoPi : 0), startAngle + (Player.direction == -1 && Player.gravDir == -1 ? MathHelper.TwoPi * 2 : 0)) - MathHelper.PiOver4).ToRotationVector2() * scaler;// + (Player.direction == -1 ? MathHelper.TwoPi : 0)
                //Main.spriteBatch.DrawLine(drawCen, drawCen + newVec, Color.Red, 1, drawOffset: -Main.screenPosition);
                var _f = 6 * f / (3 * f + 1);
                _f = MathHelper.Clamp(_f, 0, 1);
                var realColor = VertexColor(f);
                realColor.A = (byte)(_f * 255);
                bars[2 * i] = new CustomVertexInfo(drawCen + newVec, realColor, new Vector3(1 - f, 1, alphaLight));
                realColor.A = 0;
                bars[2 * i + 1] = new CustomVertexInfo(drawCen, realColor, new Vector3(0, 0, alphaLight));
            }
            return bars;
        }
        public virtual Color VertexColor(float time) => Color.White;
        public virtual void VertexInfomation(ref bool additive, ref int indexOfGreyTex, ref float endAngle, ref bool useHeatMap) { }
        public virtual void RenderInfomation(ref (float M, float Intensity, float Range) useBloom, ref (float M, float Range, Vector2 director) useDistort, ref (Texture2D fillTex, Vector2 texSize, Color glowColor, Color boundColor, float tier1, float tier2, Vector2 offset, bool lightAsAlpha) useMask) { }
        public virtual bool RedrawSelf => false;
        public virtual bool WhenVertexDraw => !Charging && Charged;
        protected Texture2D heatMap;
        public virtual Texture2D HeatMap
        {
            get
            {
                return heatMap;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!WhenVertexDraw || CoolerItemVisualEffect.ShaderSwooshEX == null || Main.GameViewMatrix == null) goto mylable; //
            var itemTex = TextureAssets.Item[Player.HeldItem.type].Value;

            //Main.NewText("Passssssssssss");
            //var airFactor = 1f;
            //var itemTex = TextureAssets.Item[Player.HeldItem.type].Value;
            //var w = itemTex.Width;
            //var h = itemTex.Height;
            //var cs = new Color[w * h];
            //itemTex.GetData(cs);
            //var target = default(Color);
            //for (int n = 0; n < cs.Length; n++)
            //{
            //    Vector2 coord = new Vector2(n % w, n / w);
            //    coord /= new Vector2(w, h);
            //    if (Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && cs[n] != default && target == default)
            //    {
            //        target = cs[n];
            //        airFactor = coord.X;
            //    }
            //}
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            var _center = projCenter;// - (new Vector2(0, projTex.Size().Y / FrameMax.Y) - DrawOrigin).RotatedBy(Rotation)

            //var drawCen = Player.gravDir == -1 ? new Vector2(_center.X, (2 * (Main.screenPosition + new Vector2(960, 560)) - _center - new Vector2(0, 96)).Y) : _center;
            var drawCen = _center;
            float xScaler = 1f;
            float scaler = (projTex.Size() / new Vector2(FrameMax.X, FrameMax.Y)).Length() * Player.GetAdjustedItemScale(Player.HeldItem) / xScaler - (new Vector2(0, projTex.Size().Y / FrameMax.Y) - DrawOrigin).Length();//(CollidingCenter - DrawOrigin).Length() * 1.414f
            //Main.NewText(-(new Vector2(0, projTex.Size().Y / FrameMax.Y) - DrawOrigin).Length());
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            //originalState.FillMode = FillMode.WireFrame;
            //RasterizerState rasterizerState = new();
            //rasterizerState.CullMode = CullMode.None;
            //rasterizerState.FillMode = FillMode.WireFrame;
            //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;
            bool additive = false;
            int indexOfGreyTex = 7;
            float endAngle = Player.direction == -1 ? MathHelper.Pi / 8 : (-MathHelper.PiOver2 - MathHelper.Pi / 8);
            bool useHeatMap = false;
            (float M, float Intensity, float Range) useBloom = default;
            (float M, float Range, Vector2 director) useDistort = default;
            (Texture2D fillTex, Vector2 texSize, Color glowColor, Color boundColor, float tier1, float tier2, Vector2 offset, bool lightAsAlpha) useMask = default;
            VertexInfomation(ref additive, ref indexOfGreyTex, ref endAngle, ref useHeatMap);
            RenderInfomation(ref useBloom, ref useDistort, ref useMask);
            int[] whenSkip = new int[0];
            endAngle = Player.gravDir == -1 ? MathHelper.PiOver2 - endAngle : endAngle;
            CustomVertexInfo[] bars = CreateVertexs(drawCen, scaler,Player.gravDir == -1 ? MathHelper.PiOver2 - Rotation: Rotation, endAngle, additive ? 0.6f : Lighting.GetColor((projCenter / 16).ToPoint().X, (projCenter / 16).ToPoint().Y).R / 255f * .6f, ref whenSkip);
            if (bars.Length < 2) goto mylable;
            SamplerState sampler = SamplerState.LinearClamp;
            CustomVertexInfo[] triangleList = new CustomVertexInfo[(bars.Length - 2) * 3];//
            for (int i = 0; i < bars.Length - 2; i += 2)
            {
                if (whenSkip.Contains(i)) continue;
                var k = i / 2;
                if (6 * k < triangleList.Length)
                {
                    triangleList[6 * k] = bars[i];
                    triangleList[6 * k + 1] = bars[i + 2];
                    triangleList[6 * k + 2] = bars[i + 1];
                }
                if (6 * k + 3 < triangleList.Length)
                {
                    triangleList[6 * k + 3] = bars[i + 1];
                    triangleList[6 * k + 4] = bars[i + 2];
                    triangleList[6 * k + 5] = bars[i + 3];
                }
            }

            //var colors = new Color[300];
            //for (int i = 0; i < 300; i++)
            //{
            //    var f = i / 299f;//分割成25次惹，f从1/25f到1//1 - 
            //    f = f * f;// *f
            //    //float h = (hsl.X + instance.hueOffsetValue + instance.hueOffsetRange * (2 * f - 1)) % 1;
            //    //float s = MathHelper.Clamp(hsl.Y * instance.saturationScalar, 0, 1);
            //    //float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + instance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - instance.luminosityRange), 0, 1);
            //    //colors[i] = Main.hslToRgb(1 / 12f, 1, f * .5f + .5f);
            //    colors[i] = f.GetLerpValue(Color.Red, Color.Orange, Color.White);
            //}
            //Texture2D tex = new Texture2D(Main.instance.GraphicsDevice, 300, 1);
            //tex.SetData(colors);
            var sb = Main.spriteBatch;
            #region Fail         
            //if (useDistort)
            //{
            //    GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            //    sb.End();
            //    graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            //    graphicsDevice.Clear(Color.Transparent);
            //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //    Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            //    Main.spriteBatch.End();

            //    //取样
            //    graphicsDevice.SetRenderTarget(CoolerItemVisualEffect.Instance.render);
            //    graphicsDevice.Clear(Color.Transparent);
            //    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //    sb.Begin(SpriteSortMode.Immediate, additive ? BlendState.Additive : BlendState.NonPremultiplied, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
            //    //CoolerItemVisualEffect.Bloom.CurrentTechnique.Passes[0].Apply();//取亮度超过m值的部分

            //    //CoolerItemVisualEffect.Bloom.Parameters["m"].SetValue(0.9f);
            //    //Main.spriteBatch.End();

            //}
            //else 
            //{
            //    sb.End();
            //    sb.Begin(SpriteSortMode.Immediate, additive ? BlendState.Additive : BlendState.NonPremultiplied, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
            //}
            #endregion
            //CoolerItemVisualEffect.bloomValue += useBloom;
            if ((useBloom.Range != 0 || useDistort.director != default || useMask.fillTex != null) && (Lighting.Mode == Terraria.Graphics.Light.LightMode.White || Lighting.Mode == Terraria.Graphics.Light.LightMode.Color) && Main.WaveQuality != 0)
            {
                GraphicsDevice gd = Main.instance.GraphicsDevice;
                RenderTarget2D render = CoolerItemVisualEffect.Instance.render;
                sb.End();
                gd.SetRenderTarget(render);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Immediate, additive ? BlendState.Additive : BlendState.NonPremultiplied, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["uLighter"].SetValue(0);
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["checkAir"].SetValue(true);
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["gather"].SetValue(true);
                Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_" + indexOfGreyTex);
                Main.graphics.GraphicsDevice.Textures[1] = GetTexture("AniTex");
                Main.graphics.GraphicsDevice.Textures[2] = itemTex;
                if (HeatMap != null && useHeatMap)
                    Main.graphics.GraphicsDevice.Textures[3] = HeatMap;

                Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;

                CoolerItemVisualEffect.ShaderSwooshEX.CurrentTechnique.Passes[HeatMap != null && useHeatMap ? 2 : 3].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList, 0, bars.Length - 2);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                sb.End();
                //然后在随便一个render里绘制屏幕，并把上面那个带弹幕的render传进shader里对屏幕进行处理
                //原版自带的screenTargetSwap就是一个可以使用的render，（原版用来连续上滤镜）
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                //Main.NewText(CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes.Count);

                //CoolerItemVisualEffect.DistortEffect.Parameters["offset"].SetValue(Rotation.ToRotationVector2() * -0.002f * useDistort);//* (1 - 2 * Math.Abs(0.5f - useDistort))
                //CoolerItemVisualEffect.DistortEffect.Parameters["invAlpha"].SetValue(0);


                CoolerItemVisualEffect.DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                CoolerItemVisualEffect.DistortEffect.Parameters["tex0"].SetValue(render);
                if (useBloom.Range != 0)
                {
                    CoolerItemVisualEffect.DistortEffect.Parameters["position"].SetValue(new Vector2(useBloom.M, useBloom.Range));
                    CoolerItemVisualEffect.DistortEffect.Parameters["tier2"].SetValue(useBloom.Intensity);
                    for (int n = 0; n < 3; n++)
                    {
                        gd.SetRenderTarget(Main.screenTargetSwap);
                        gd.Clear(Color.Transparent);
                        CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes[7].Apply();
                        sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);



                        gd.SetRenderTarget(Main.screenTarget);
                        gd.Clear(Color.Transparent);
                        CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes[6].Apply();
                        sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    }
                }
                if (useDistort.director != default)
                {
                    CoolerItemVisualEffect.DistortEffect.Parameters["position"].SetValue(new Vector2(useDistort.M, useDistort.Range));
                    CoolerItemVisualEffect.DistortEffect.Parameters["ImageSize"].SetValue(useDistort.director);
                    for (int n = 0; n < 2; n++)
                    {
                        gd.SetRenderTarget(Main.screenTargetSwap);
                        gd.Clear(Color.Transparent);
                        CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes[5].Apply();
                        sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

                        gd.SetRenderTarget(Main.screenTarget);
                        gd.Clear(Color.Transparent);
                        CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes[4].Apply();
                        sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    }
                    //Main.NewText(CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes.Count);
                }
                if (useMask.fillTex != null)
                {
                    #region MyRegion
                    //gd.SetRenderTarget(Main.screenTargetSwap);
                    //gd.Clear(Color.Transparent);
                    //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    //Main.graphics.GraphicsDevice.Textures[1] = useMask.fillTex;
                    //CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes[1].Apply();
                    //CoolerItemVisualEffect.DistortEffect.Parameters["tex0"].SetValue(render);
                    //CoolerItemVisualEffect.DistortEffect.Parameters["invAlpha"].SetValue(useMask.tier1);
                    //CoolerItemVisualEffect.DistortEffect.Parameters["lightAsAlpha"].SetValue(useMask.lightAsAlpha);
                    //CoolerItemVisualEffect.DistortEffect.Parameters["tier2"].SetValue(useMask.tier2);
                    //CoolerItemVisualEffect.DistortEffect.Parameters["position"].SetValue(useMask.offset);
                    //CoolerItemVisualEffect.DistortEffect.Parameters["maskGlowColor"].SetValue(useMask.glowColor.ToVector4());
                    //CoolerItemVisualEffect.DistortEffect.Parameters["maskBoundColor"].SetValue(useMask.boundColor.ToVector4());
                    //CoolerItemVisualEffect.DistortEffect.Parameters["ImageSize"].SetValue(useMask.texSize);
                    //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                    //sb.End();
                    //gd.SetRenderTarget(Main.screenTarget);
                    //gd.Clear(Color.Transparent);
                    //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    //sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    //sb.End();
                    #endregion

                    gd.SetRenderTarget(Main.screenTargetSwap);
                    gd.Clear(Color.Transparent);
                    Main.graphics.GraphicsDevice.Textures[1] = useMask.fillTex;
                    CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes[1].Apply();
                    CoolerItemVisualEffect.DistortEffect.Parameters["tex0"].SetValue(render);
                    CoolerItemVisualEffect.DistortEffect.Parameters["invAlpha"].SetValue(useMask.tier1);
                    CoolerItemVisualEffect.DistortEffect.Parameters["lightAsAlpha"].SetValue(useMask.lightAsAlpha);
                    CoolerItemVisualEffect.DistortEffect.Parameters["tier2"].SetValue(useMask.tier2);
                    CoolerItemVisualEffect.DistortEffect.Parameters["position"].SetValue(useMask.offset);
                    CoolerItemVisualEffect.DistortEffect.Parameters["maskGlowColor"].SetValue(useMask.glowColor.ToVector4());
                    CoolerItemVisualEffect.DistortEffect.Parameters["maskBoundColor"].SetValue(useMask.boundColor.ToVector4());
                    CoolerItemVisualEffect.DistortEffect.Parameters["ImageSize"].SetValue(useMask.texSize);
                    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                    gd.SetRenderTarget(Main.screenTarget);
                    gd.Clear(Color.Transparent);
                    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                }



                //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                //sb.End();

                ////最后在screenTarget上把刚刚的结果画上
                //gd.SetRenderTarget(Main.screenTarget);
                //gd.Clear(Color.Transparent);
                //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                //sb.Draw(render, Vector2.Zero, Color.White);

            }
            else
            {
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, additive ? BlendState.Additive : BlendState.NonPremultiplied, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["uLighter"].SetValue(0);
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["checkAir"].SetValue(true);
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["airFactor"].SetValue(1f);
                CoolerItemVisualEffect.ShaderSwooshEX.Parameters["gather"].SetValue(true);
                Main.graphics.GraphicsDevice.Textures[0] = GetTexture("BaseTex_" + indexOfGreyTex);
                Main.graphics.GraphicsDevice.Textures[1] = GetTexture("AniTex");
                Main.graphics.GraphicsDevice.Textures[2] = itemTex;
                if (HeatMap != null && useHeatMap)
                    Main.graphics.GraphicsDevice.Textures[3] = HeatMap;

                Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;

                CoolerItemVisualEffect.ShaderSwooshEX.CurrentTechnique.Passes[HeatMap != null && useHeatMap ? 2 : 3].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList, 0, bars.Length - 2);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
            }
        #region Fail
        //if (!useBloom)
        //{
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        //}
        //else

        ////Main.NewText("??");


        ////Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);//绘制内容
        ////if (useBloom) 
        //{
        //    GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
        //    graphicsDevice.SetRenderTarget(Main.screenTarget);
        //    graphicsDevice.Clear(Color.Transparent);
        //    sb.End();

        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        //    CoolerItemVisualEffect.Bloom.CurrentTechnique.Passes[0].Apply();//取亮度超过m值的部分
        //    CoolerItemVisualEffect.Bloom.Parameters["m"].SetValue(0.9f);
        //    //CoolerItemVisualEffect.Bloom.CurrentTechnique.Passes["GlurV"].Apply();//横向

        //    Main.spriteBatch.Draw(CoolerItemVisualEffect.Instance.render, Vector2.Zero, Color.White);

        //    Main.spriteBatch.End();

        //    //处理
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        //    CoolerItemVisualEffect.Bloom.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        //    CoolerItemVisualEffect.Bloom.Parameters["uRange"].SetValue(2.5f);
        //    CoolerItemVisualEffect.Bloom.Parameters["uIntensity"].SetValue(0.3f);
        //    for (int i = 0; i < 3; i++)//交替使用两个RenderTarget2D，进行多次模糊
        //    {
        //        CoolerItemVisualEffect.Bloom.CurrentTechnique.Passes["GlurH"].Apply();//纵向
        //        graphicsDevice.SetRenderTarget(CoolerItemVisualEffect.Instance.render);
        //        graphicsDevice.Clear(Color.Transparent);
        //        Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);


        //        CoolerItemVisualEffect.Bloom.CurrentTechnique.Passes["GlurV"].Apply();//横向
        //        graphicsDevice.SetRenderTarget(Main.screenTarget);
        //        graphicsDevice.Clear(Color.Transparent);
        //        Main.spriteBatch.Draw(CoolerItemVisualEffect.Instance.render, Vector2.Zero, Color.White);
        //    }
        //    Main.spriteBatch.End();

        //    //叠加到原图上
        //    graphicsDevice.SetRenderTarget(Main.screenTarget);
        //    graphicsDevice.Clear(Color.Transparent);
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);//Additive把模糊后的部分加到Main.screenTarget里
        //    Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //    Main.spriteBatch.Draw(CoolerItemVisualEffect.Instance.render, Vector2.Zero, Color.White);
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);

        //}
        #endregion
        mylable:
            if (!RedrawSelf)
                return base.PreDraw(ref lightColor);
            return false;
        }
    }
}
