using LogSpiralLibrary;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoolerItemVisualEffect.ConfigurationSwoosh;
using static LogSpiralLibrary.LogSpiralLibraryMod;
namespace CoolerItemVisualEffect
{
    public class CoolerItemVisualEffectRenderDrawing : RenderBasedDrawing
    {
        public override void CommonDrawingMethods(SpriteBatch spriteBatch)
        {
            List<Projectile> pureFractals = new List<Projectile>();
            List<Projectile> firstZeniths = new List<Projectile>();
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;

            foreach (var proj in Main.projectile)
            {
                if (proj.active)
                {
                    if (proj.type == ModContent.ProjectileType<Weapons.PureFractalProj>()) pureFractals.Add(proj);
                    if (proj.type == ModContent.ProjectileType<Weapons.FirstZenithProj>()) firstZeniths.Add(proj);
                }
            }
            if (pureFractals.Count > 0 || firstZeniths.Count > 0)
            {
                var bars = new List<CustomVertexInfo>();
                #region 绘制FZ
                if (firstZeniths.Count > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                    foreach (var projectile in firstZeniths)
                    {
                        var firstZenith = projectile.ModProjectile as Weapons.FirstZenithProj;
                        if (firstZenith == null) { Main.NewText("nmdwsm"); continue; }
                        firstZenith.DrawOthers();
                        var max = projectile.oldPos.Length - 1;
                        for (int n = 0; n < projectile.oldPos.Length; n++)
                        {
                            if (projectile.oldPos[n] == default) { max = n; break; }
                        }
                        if (max < 2) { Main.NewText("太短了太短了！！  " + max + "   " + projectile.localAI[0] + "   " + projectile.oldPos[0]); continue; }

                        float _scaler = 98f;
                        var realColor = new Color(151, 145, 186);
                        var hsl = new Vector3(0.691667f, 0.229166f, 0.65f);
                        var multiValue = 1 - projectile.localAI[0] / 90f;
                        bars.Add(new CustomVertexInfo(projectile.oldPos[0] + projectile.oldRot[0].ToRotationVector2() * _scaler * ConfigSwooshInstance.swooshSize, default(Color), new Vector3(1, 1, 0.6f)));
                        bars.Add(new CustomVertexInfo(projectile.oldPos[0], default(Color), new Vector3(0, 0, 0.6f)));
                        for (int i = 0; i < max; i++)
                        {
                            var f = i / (max - 1f);
                            f = 1 - f;
                            var alphaLight = 0.6f;
                            //TODO 初源绘制检查
                            if (false)//ConfigSwooshInstance.swooshColorType == SwooshColorType.单向渐变 || ConfigSwooshInstance.swooshColorType == SwooshColorType.单向渐变与对角线混合
                            {
                                float h = (hsl.X + ConfigSwooshInstance.hueOffsetValue + ConfigSwooshInstance.hueOffsetRange * (2 * f - 1)) % 1;
                                float s = MathHelper.Clamp(hsl.Y * ConfigSwooshInstance.saturationScalar, 0, 1);
                                float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + ConfigSwooshInstance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - ConfigSwooshInstance.luminosityRange), 0, 1);
                                realColor = Main.hslToRgb(h, s, l);
                            }
                            var _f = 6 * f / (3 * f + 1);//6 * f / (3 * f + 1) /(float)Math.Pow(f,instance.maxCount)
                            _f = MathHelper.Clamp(_f, 0, 1);
                            realColor.A = (byte)(_f * 255);
                            bars.Add(new CustomVertexInfo(projectile.oldPos[i] + projectile.oldRot[i].ToRotationVector2() * _scaler * ConfigSwooshInstance.swooshSize, realColor * multiValue, new Vector3(1 - f, 1, alphaLight)));
                            realColor.A = 0;
                            bars.Add(new CustomVertexInfo(projectile.oldPos[i], realColor * multiValue, new Vector3(0, 0, alphaLight)));
                        }
                    }
                    spriteBatch.End();
                }
                #endregion
                if (RenderEffect == null || ShaderSwooshUL == null) return;
                List<CustomVertexInfo> _triangleList = new List<CustomVertexInfo>();
                SamplerState sampler;
                switch (ConfigSwooshInstance.swooshSampler)
                {
                    default:
                    case SwooshSamplerState.各向异性: sampler = SamplerState.AnisotropicWrap; break;
                    case SwooshSamplerState.线性: sampler = SamplerState.LinearWrap; break;
                    case SwooshSamplerState.点: sampler = SamplerState.PointWrap; break;
                }
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                var sb = Main.spriteBatch;

                if (bars.Count > 2)
                {
                    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                    var passCount = 0;
                    for (int i = 0; i < bars.Count - 2; i += 2)
                    {
                        _triangleList.Add(bars[i]);
                        _triangleList.Add(bars[i + 2]);
                        _triangleList.Add(bars[i + 1]);
                        _triangleList.Add(bars[i + 1]);
                        _triangleList.Add(bars[i + 2]);
                        _triangleList.Add(bars[i + 3]);
                    }
                    ShaderSwooshUL.Parameters["uTransform"].SetValue(model * trans * projection);
                    //ShaderSwooshUL.Parameters["uLighter"].SetValue(ConfigSwooshInstance.luminosityFactor);
                    ShaderSwooshUL.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
                    ShaderSwooshUL.Parameters["checkAir"].SetValue(ConfigSwooshInstance.checkAir);
                    ShaderSwooshUL.Parameters["airFactor"].SetValue(1);
                    ShaderSwooshUL.Parameters["gather"].SetValue(ConfigSwooshInstance.gather);
                    ShaderSwooshUL.Parameters["lightShift"].SetValue(0);
                    ShaderSwooshUL.Parameters["distortScaler"].SetValue(0);
                    var _v = ConfigSwooshInstance.directOfHeatMap.ToRotationVector2();
                    ShaderSwooshUL.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });
                    ShaderSwooshUL.Parameters["alphaFactor"].SetValue(ConfigSwooshInstance.alphaFactor);
                    ShaderSwooshUL.Parameters["heatMapAlpha"].SetValue(ConfigSwooshInstance.alphaFactor == 0);
                    ShaderSwooshUL.Parameters["AlphaVector"].SetValue(ConfigSwooshInstance.colorVector.AlphaVector);
                    Main.graphics.GraphicsDevice.Textures[0] = BaseTex[ConfigSwooshInstance.ImageIndex].Value;
                    Main.graphics.GraphicsDevice.Textures[1] = AniTex[ConfigSwooshInstance.AnimateIndex + 11].Value;
                    Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Weapons/FirstZenithProj_5").Value;
                    Main.graphics.GraphicsDevice.Textures[3] = Main.LocalPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>().colorInfo.tex;
                    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;

                    ShaderSwooshUL.CurrentTechnique.Passes[7].Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleList.ToArray(), 0, _triangleList.Count / 3);
                    sb.End();
                }
                if (pureFractals.Count > 0)
                {
                    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                    foreach (var projectile in pureFractals)
                    {
                        (projectile.ModProjectile as Weapons.PureFractalProj)?.DrawSwoosh();
                    }
                    sb.End();
                }
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                if (pureFractals.Count > 0 || firstZeniths.Count > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                    if (pureFractals.Count > 0)
                    {
                        foreach (var projectile in pureFractals)
                        {
                            (projectile.ModProjectile as Weapons.PureFractalProj)?.DrawSword();
                        }
                    }
                    if (firstZeniths.Count > 0)
                    {
                        foreach (var projectile in firstZeniths)
                        {
                            (projectile.ModProjectile as Weapons.FirstZenithProj)?.DrawSword();
                        }
                    }
                    sb.End();
                }
            }
        }

        public override void RenderDrawingMethods(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, RenderTarget2D render, RenderTarget2D renderAirDistort)
        {
            //goto mylabel;
            List<Projectile> pureFractals = new List<Projectile>();
            List<Projectile> firstZeniths = new List<Projectile>();
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            var disFlag = ConfigSwooshInstance.distortFactor != 0 && ConfigSwooshInstance.distortSize != 1;
            foreach (var proj in Main.projectile)
            {
                if (proj.active)
                {
                    if (proj.type == ModContent.ProjectileType<Weapons.PureFractalProj>()) pureFractals.Add(proj);
                    if (proj.type == ModContent.ProjectileType<Weapons.FirstZenithProj>()) firstZeniths.Add(proj);
                }
            }
            if (pureFractals.Count > 0 || firstZeniths.Count > 0)
            {
                var bars = new List<CustomVertexInfo>();
                var bars_2 = new List<CustomVertexInfo>();

                #region 绘制FZ
                if (firstZeniths.Count > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                    foreach (var projectile in firstZeniths)
                    {
                        //var instance = Main.netMode == NetmodeID.MultiplayerClient ? Main.player[projectile.owner].GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh : ConfigurationSwoosh.instance;

                        var firstZenith = projectile.ModProjectile as Weapons.FirstZenithProj;
                        if (firstZenith == null) { Main.NewText("nmdwsm"); continue; }
                        firstZenith.DrawOthers();
                        var max = projectile.oldPos.Length - 1;
                        for (int n = 0; n < projectile.oldPos.Length; n++)
                        {
                            if (projectile.oldPos[n] == default) { max = n; break; }
                        }
                        if (max < 2) { Main.NewText("太短了太短了！！  " + max + "   " + projectile.localAI[0] + "   " + projectile.oldPos[0]); continue; }

                        float _scaler = 98f;
                        var realColor = new Color(151, 145, 186);
                        var hsl = new Vector3(0.691667f, 0.229166f, 0.65f);
                        var multiValue = 1 - projectile.localAI[0] / 90f;
                        bars.Add(new CustomVertexInfo(projectile.oldPos[0] + projectile.oldRot[0].ToRotationVector2() * _scaler * ConfigSwooshInstance.swooshSize, default(Color), new Vector3(1, 1, 0.6f)));
                        bars_2.Add(bars[^1] with { Position = (bars[^1].Position - projectile.oldPos[0]) * ConfigSwooshInstance.distortSize + projectile.oldPos[0] });
                        bars.Add(new CustomVertexInfo(projectile.oldPos[0], default(Color), new Vector3(0, 0, 0.6f)));
                        bars_2.Add(bars[^1] with { Position = (bars[^1].Position - projectile.oldPos[0]) * ConfigSwooshInstance.distortSize + projectile.oldPos[0] });

                        for (int i = 0; i < max; i++)
                        {
                            var f = i / (max - 1f);
                            f = 1 - f;
                            var alphaLight = 0.6f;
                            if (false)
                            {
                                float h = (hsl.X + ConfigSwooshInstance.hueOffsetValue + ConfigSwooshInstance.hueOffsetRange * (2 * f - 1)) % 1;
                                float s = MathHelper.Clamp(hsl.Y * ConfigSwooshInstance.saturationScalar, 0, 1);
                                float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + ConfigSwooshInstance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - ConfigSwooshInstance.luminosityRange), 0, 1);
                                realColor = Main.hslToRgb(h, s, l);
                            }
                            var _f = 6 * f / (3 * f + 1);//6 * f / (3 * f + 1) /(float)Math.Pow(f,instance.maxCount)
                            _f = MathHelper.Clamp(_f, 0, 1);
                            realColor.A = (byte)(_f * 255);
                            bars.Add(new CustomVertexInfo(projectile.oldPos[i] + projectile.oldRot[i].ToRotationVector2() * _scaler * ConfigSwooshInstance.swooshSize, realColor * multiValue, new Vector3(1 - f, 1, alphaLight)));
                            bars_2.Add(bars[^1] with { Position = (bars[^1].Position - projectile.oldPos[i]) * ConfigSwooshInstance.distortSize + projectile.oldPos[0] });

                            realColor.A = 0;
                            bars.Add(new CustomVertexInfo(projectile.oldPos[i], realColor * multiValue, new Vector3(0, 0, alphaLight)));
                            bars_2.Add(bars[^1] with { Position = (bars[^1].Position - projectile.oldPos[i]) * ConfigSwooshInstance.distortSize + projectile.oldPos[0] });

                        }
                    }
                    spriteBatch.End();
                }

                //Main.PlayerRenderer.DrawPlayer(Main.Camera, Main.player[projectile.owner], projectile.Center, 0f, new Vector2(20, 28));
                #endregion
                //SpriteEffects spriteEffects = SpriteEffects.None;
                //if (projectile.spriteDirection == -1)
                //{
                //    spriteEffects = SpriteEffects.FlipHorizontally;
                //}
                if (RenderEffect == null || ShaderSwooshUL == null) return;

                List<CustomVertexInfo> _triangleList = new List<CustomVertexInfo>();
                SamplerState sampler;
                switch (ConfigSwooshInstance.swooshSampler)
                {
                    default:
                    case SwooshSamplerState.各向异性: sampler = SamplerState.AnisotropicWrap; break;
                    case SwooshSamplerState.线性: sampler = SamplerState.LinearWrap; break;
                    case SwooshSamplerState.点: sampler = SamplerState.PointWrap; break;
                }
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                var gd = Main.graphics.GraphicsDevice;
                var sb = Main.spriteBatch;


                //sb.End();
                gd.SetRenderTarget(Instance.Render);
                gd.Clear(Color.Transparent);
                if (bars.Count > 2)
                {
                    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                    var passCount = 0;
                    for (int i = 0; i < bars.Count - 2; i += 2)
                    {
                        _triangleList.Add(bars[i]);
                        _triangleList.Add(bars[i + 2]);
                        _triangleList.Add(bars[i + 1]);
                        _triangleList.Add(bars[i + 1]);
                        _triangleList.Add(bars[i + 2]);
                        _triangleList.Add(bars[i + 3]);
                    }
                    ShaderSwooshUL.Parameters["uTransform"].SetValue(model * trans * projection);
                    //ShaderSwooshUL.Parameters["uLighter"].SetValue(ConfigSwooshInstance.luminosityFactor);
                    ShaderSwooshUL.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
                    ShaderSwooshUL.Parameters["checkAir"].SetValue(ConfigSwooshInstance.checkAir);
                    ShaderSwooshUL.Parameters["airFactor"].SetValue(1);
                    ShaderSwooshUL.Parameters["gather"].SetValue(ConfigSwooshInstance.gather);
                    ShaderSwooshUL.Parameters["alphaFactor"].SetValue(ConfigSwooshInstance.alphaFactor);
                    ShaderSwooshUL.Parameters["heatMapAlpha"].SetValue(ConfigSwooshInstance.alphaFactor == 0);
                    var _v = ConfigSwooshInstance.directOfHeatMap.ToRotationVector2();
                    ShaderSwooshUL.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });

                    ShaderSwooshUL.Parameters["lightShift"].SetValue(0);
                    ShaderSwooshUL.Parameters["distortScaler"].SetValue(0);
                    ShaderSwooshUL.Parameters["AlphaVector"].SetValue(ConfigSwooshInstance.colorVector.AlphaVector);

                    Main.graphics.GraphicsDevice.Textures[0] = BaseTex[ConfigSwooshInstance.ImageIndex].Value;
                    Main.graphics.GraphicsDevice.Textures[1] = AniTex[ConfigSwooshInstance.AnimateIndex + 11].Value;
                    Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Weapons/FirstZenithProj_5").Value;
                    Main.graphics.GraphicsDevice.Textures[3] = Main.LocalPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>().colorInfo.tex;
                    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;;
                    ShaderSwooshUL.CurrentTechnique.Passes[7].Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleList.ToArray(), 0, _triangleList.Count / 3);
                    sb.End();

                    if (disFlag)
                    {
                        gd.SetRenderTarget(Instance.Render_AirDistort);
                        gd.Clear(Color.Transparent);
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                        ShaderSwooshUL.Parameters["distortScaler"].SetValue(ConfigSwooshInstance.distortSize);
                        ShaderSwooshUL.CurrentTechnique.Passes[7].Apply();
                        _triangleList.Clear();
                        for (int i = 0; i < bars_2.Count - 2; i += 2)
                        {
                            _triangleList.Add(bars_2[i]);
                            _triangleList.Add(bars_2[i + 2]);
                            _triangleList.Add(bars_2[i + 1]);
                            _triangleList.Add(bars_2[i + 1]);
                            _triangleList.Add(bars_2[i + 2]);
                            _triangleList.Add(bars_2[i + 3]);
                        }
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleList.ToArray(), 0, _triangleList.Count / 3);
                        sb.End();
                    }
                }

                if (pureFractals.Count > 0)
                {
                    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                    foreach (var projectile in pureFractals)
                    {
                        (projectile.ModProjectile as Weapons.PureFractalProj)?.DrawSwoosh();
                    }
                    sb.End();

                    if (disFlag)
                    {
                        gd.SetRenderTarget(Instance.Render_AirDistort);
                        gd.Clear(Color.Transparent);
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                        foreach (var projectile in pureFractals)
                        {
                            var dummy = ConfigSwooshInstance.swooshSize;
                            ConfigSwooshInstance.swooshSize *= ConfigSwooshInstance.distortSize;
                            (projectile.ModProjectile as Weapons.PureFractalProj)?.DrawSwoosh();
                            ConfigSwooshInstance.swooshSize = dummy;
                        }
                        sb.End();
                    }
                }
                Main.graphics.GraphicsDevice.RasterizerState = originalState;




                switch (ConfigSwooshInstance.coolerSwooshQuality)
                {
                    case QualityType.中medium:
                        {
                            for (int n = 0; n < ConfigSwooshInstance.maxCount; n++)
                            {
                                //sb.End();
                                gd.SetRenderTarget(Main.screenTargetSwap);
                                gd.Clear(Color.Transparent);
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                                RenderEffect.Parameters["tex0"].SetValue(Instance.Render);
                                RenderEffect.Parameters["offset"].SetValue(new Vector2(0.707f) * -0.09f * ConfigSwooshInstance.distortFactor);
                                RenderEffect.Parameters["invAlpha"].SetValue(0);
                                RenderEffect.CurrentTechnique.Passes[0].Apply();
                                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                                sb.End();
                                gd.SetRenderTarget(Main.screenTarget);
                                gd.Clear(Color.Transparent);
                                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                                sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, 0));
                                sb.End();
                            }
                            break;
                        }
                    case QualityType.高high:
                    case QualityType.极限ultra:
                        {
                            #region MyRegion
                            //for (int n = 0; n < ConfigSwooshInstance.maxCount; n++)
                            //{
                            //    //Mark1

                            //    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                            //    //gd.SetRenderTarget(Main.screenTargetSwap);
                            //    //DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                            //    //DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
                            //    //DistortEffect.Parameters["position"].SetValue(new Vector2(0, 4));
                            //    //DistortEffect.Parameters["tier2"].SetValue(ConfigSwooshInstance.luminosityFactor);
                            //    //gd.Clear(Color.Transparent);
                            //    //DistortEffect.CurrentTechnique.Passes[7].Apply();
                            //    //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

                            //    //gd.SetRenderTarget(Main.screenTarget);
                            //    //gd.Clear(Color.Transparent);
                            //    //DistortEffect.CurrentTechnique.Passes[6].Apply();
                            //    //sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);

                            //    //DistortEffect.Parameters["position"].SetValue(new Vector2(0, 9));
                            //    //DistortEffect.Parameters["ImageSize"].SetValue(new Vector2(0.707f) * -0.008f * ConfigSwooshInstance.distortFactor);
                            //    //gd.SetRenderTarget(Main.screenTargetSwap);
                            //    //gd.Clear(Color.Transparent);
                            //    //DistortEffect.CurrentTechnique.Passes[5].Apply();
                            //    //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

                            //    //gd.SetRenderTarget(Main.screenTarget);
                            //    //gd.Clear(Color.Transparent);
                            //    //DistortEffect.CurrentTechnique.Passes[4].Apply();
                            //    //sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                            //    //sb.End();
                            //    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                            //    //sb.Draw(Instance.Render, Vector2.Zero, Color.White);
                            //    //sb.End();

                            //    gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
                            //    gd.Clear(Color.Transparent);//清空
                            //    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                            //    //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix



                            //    //Vector2 direct = (instance.swooshFactorStyle == SwooshFactorStyle.每次开始时决定系数 ? modPlayer.kValue : ((modPlayer.kValue + modPlayer.kValueNext) * .5f)).ToRotationVector2() * -0.1f * fac.SymmetricalFactor2(0.5f, 0.2f) * instance.distortFactor;//(u + v)
                            //    DistortEffect.Parameters["offset"].SetValue(new Vector2(0.707f) * -0.03f * ConfigSwooshInstance.distortFactor);//设置参数时间
                            //    DistortEffect.Parameters["invAlpha"].SetValue(0);
                            //    DistortEffect.Parameters["tex0"].SetValue(Instance.Render_AirDistort);
                            //    DistortEffect.CurrentTechnique.Passes[0].Apply();//ApplyPass
                            //    spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);//绘制原先屏幕内容
                            //    gd.SetRenderTarget(Main.screenTarget);
                            //    gd.Clear(Color.Transparent);
                            //    spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);

                            //    DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                            //    DistortEffect.Parameters["tex0"].SetValue(Render);

                            //    DistortEffect.Parameters["position"].SetValue(new Vector2(0, 3f));
                            //    DistortEffect.Parameters["tier2"].SetValue(0.2f);
                            //    //for (int n = 0; n < 1; n++)
                            //    //{
                            //    gd.SetRenderTarget(Main.screenTargetSwap);
                            //    gd.Clear(Color.Transparent);
                            //    DistortEffect.CurrentTechnique.Passes[7].Apply();
                            //    spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);



                            //    gd.SetRenderTarget(Main.screenTarget);
                            //    gd.Clear(Color.Transparent);
                            //    DistortEffect.CurrentTechnique.Passes[6].Apply();
                            //    spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                            //    //}
                            //    //Distort.Parameters["position"].SetValue(new Vector2(0, 5f));
                            //    //Distort.Parameters["ImageSize"].SetValue(new Vector2(0.707f) * -0.006f);//projectile.rotation.ToRotationVector2() * -0.006f


                            //    //for (int n = 0; n < 1; n++)
                            //    //{
                            //    //gd.SetRenderTarget(Main.screenTargetSwap);
                            //    //gd.Clear(Color.Transparent);
                            //    //Distort.CurrentTechnique.Passes[5].Apply();
                            //    //spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);

                            //    //gd.SetRenderTarget(Main.screenTarget);
                            //    //gd.Clear(Color.Transparent);
                            //    //Distort.CurrentTechnique.Passes[4].Apply();
                            //    //spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                            //    //}
                            //    spriteBatch.End();
                            //    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                            //    spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                            //    spriteBatch.Draw(Render, Vector2.Zero, Color.White);
                            //    spriteBatch.End();
                            //}
                            #endregion


                            for (int n = 0; n < ConfigSwooshInstance.maxCount; n++)
                            {
                                if (n == 0)
                                {
                                    //sb.End();
                                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                                }
                                if (ConfigSwooshInstance.luminosityFactor != 0)
                                {
                                    gd.SetRenderTarget(Main.screenTargetSwap);
                                    RenderEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                                    RenderEffect.Parameters["tex0"].SetValue(Instance.Render);
                                    RenderEffect.Parameters["position"].SetValue(new Vector2(0, 6));
                                    RenderEffect.Parameters["tier2"].SetValue(ConfigSwooshInstance.luminosityFactor);
                                    gd.Clear(Color.Transparent);
                                    RenderEffect.CurrentTechnique.Passes[7].Apply();
                                    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                                    gd.SetRenderTarget(Main.screenTarget);
                                    gd.Clear(Color.Transparent);
                                    RenderEffect.CurrentTechnique.Passes[6].Apply();
                                    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                                    sb.End();
                                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                                }


                                #region MyRegion
                                //DistortEffect.Parameters["position"].SetValue(new Vector2(0, 9));
                                //DistortEffect.Parameters["ImageSize"].SetValue((u + v) * -0.0004f * (1 - 2 * Math.Abs(0.5f - fac)) * instance.distortFactor);
                                //gd.SetRenderTarget(Main.screenTargetSwap);
                                //gd.Clear(Color.Transparent);
                                //DistortEffect.CurrentTechnique.Passes[5].Apply();
                                //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

                                //gd.SetRenderTarget(Main.screenTarget);
                                //gd.Clear(Color.Transparent);
                                //DistortEffect.CurrentTechnique.Passes[4].Apply();
                                //sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                                //sb.End();
                                //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                                //sb.Draw(Instance.Render, Vector2.Zero, Color.White);
                                #endregion

                                if (ConfigSwooshInstance.distortFactor != 0)
                                {
                                    gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
                                    gd.Clear(Color.Transparent);//清空
                                                                //Vector2 direct = (instance.swooshFactorStyle == SwooshFactorStyle.每次开始时决定系数 ? modPlayer.kValue : ((modPlayer.kValue + modPlayer.kValueNext) * .5f)).ToRotationVector2() * -0.1f * fac.SymmetricalFactor2(0.5f, 0.2f) * instance.distortFactor;//(u + v)
                                    RenderEffect.Parameters["offset"].SetValue(new Vector2(0.707f) * -0.09f * ConfigSwooshInstance.distortFactor);//设置参数时间
                                    RenderEffect.Parameters["invAlpha"].SetValue(0);
                                    RenderEffect.Parameters["tex0"].SetValue(ConfigSwooshInstance.distortSize != 1 ? Instance.Render_AirDistort : Instance.Render);
                                    RenderEffect.CurrentTechnique.Passes[0].Apply();//ApplyPass
                                    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//绘制原先屏幕内容
                                    gd.SetRenderTarget(Main.screenTarget);
                                    gd.Clear(Color.Transparent);
                                    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                                    //sb.End();

                                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                                    //Main.instance.GraphicsDevice.BlendState = BlendState.Additive;
                                    sb.End();
                                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                                }

                                if (ConfigSwooshInstance.distortFactor == 0 && ConfigSwooshInstance.luminosityFactor == 0 && n == 0)
                                {
                                    gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
                                    gd.Clear(Color.Transparent);
                                    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                                    gd.SetRenderTarget(Main.screenTarget);//将画布设置为这个
                                    gd.Clear(Color.Transparent);
                                    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                                }
                                sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, (Main.LocalPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>().hsl.Z < ConfigSwooshInstance.isLighterDecider) ? 1 : 0));//
                                                                                                                                                                                                                     //Main.instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                            }
                            sb.End();
                            break;
                        }
                }
                if (pureFractals.Count > 0 || firstZeniths.Count > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                    if (pureFractals.Count > 0)
                    {
                        foreach (var projectile in pureFractals)
                        {
                            (projectile.ModProjectile as Weapons.PureFractalProj)?.DrawSword();
                        }
                    }
                    if (firstZeniths.Count > 0)
                    {
                        foreach (var projectile in firstZeniths)
                        {
                            (projectile.ModProjectile as Weapons.FirstZenithProj)?.DrawSword();
                        }
                    }
                    sb.End();
                }
            }
        }
    }
}
