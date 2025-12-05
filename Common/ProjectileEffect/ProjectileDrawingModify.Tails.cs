using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.Config.Preview;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace CoolerItemVisualEffect.ProjectileEffect;

public partial class ProjectileDrawingModify
{
    private static void DrawTails(Projectile projectile)
    {
        if (CoolerItemVisualEffectMod.RenderEffect == null || CoolerItemVisualEffectMod.ShaderSwooshEX == null) return;
        var spriteBatch = Main.spriteBatch;
        var bars = new List<CustomVertexInfo>();
        var max = projectile.oldPos.Length;
        for (var n = 0; n < projectile.oldPos.Length; n++)
        {
            if (projectile.oldPos[n] == default) { max = n; break; }
        }
        if (max < 2) return;

        var _scaler = 98f;
        var mainColor = new Color(151, 145, 186);
        var angleOffset = 0f;
        Vector2 centerOffset = default;
        var facVel = 1 - 1 / (projectile.velocity.Length() / 4 + 1);
        facVel *= 3;
        var isBoomerang = false;
        switch (projectile.type)
        {
            case ProjectileID.NightBeam:
                {
                    mainColor = Color.LimeGreen;
                    angleOffset = MathHelper.PiOver4 * (6 + projectile.direction);
                    _scaler = 56.6f;
                    centerOffset = (projectile.oldRot[0] + angleOffset).ToRotationVector2() * -_scaler * .5f;
                    facVel = 1;
                    break;
                }
            case ProjectileID.DeathSickle:
                {
                    mainColor = Color.Purple;
                    angleOffset = MathHelper.PiOver4 * (6 + projectile.direction);
                    _scaler = 64f;
                    centerOffset = (projectile.oldRot[0] + angleOffset).ToRotationVector2() * -_scaler * .5f;
                    ProjectileID.Sets.TrailCacheLength[ProjectileID.DeathSickle] = 20;
                    facVel /= 3;
                    break;
                }
            case ProjectileID.StarWrath:
            case ProjectileID.Starfury:
                {
                    mainColor = Color.HotPink;
                    _scaler = 16;
                    if (projectile.type == ProjectileID.StarWrath)
                    {
                        angleOffset = (float)LogSpiralLibraryMod.ModTime * MathHelper.Pi / 30f;
                        centerOffset = new Vector2(projectile.width, projectile.height) * .5f;
                        ProjectileID.Sets.TrailCacheLength[ProjectileID.StarWrath] = 40;
                        mainColor *= .75f;
                    }
                    break;
                }
            case ProjectileID.StarCannonStar:
            case ProjectileID.FallingStar:
            case ProjectileID.ManaCloakStar:
                {
                    mainColor = Color.Blue;
                    _scaler = 16;
                    ProjectileID.Sets.TrailCacheLength[ProjectileID.StarCannonStar] = 20;
                    ProjectileID.Sets.TrailCacheLength[ProjectileID.FallingStar] = 20;
                    ProjectileID.Sets.TrailCacheLength[ProjectileID.ManaCloakStar] = 20;

                    break;
                }
            case ProjectileID.SuperStar:
                {
                    mainColor = Color.Yellow;
                    _scaler = 16;
                    ProjectileID.Sets.TrailCacheLength[ProjectileID.SuperStar] = 15;
                    break;
                }
            case ProjectileID.HallowStar:
                {
                    mainColor = Color.Lerp(Color.Purple, Color.White, .5f);
                    _scaler = 16;
                    break;
                }
            case ProjectileID.BeeCloakStar:
                {
                    mainColor = Color.Yellow;
                    _scaler = 16;
                    break;
                }
            case ProjectileID.StarCloakStar:
                {
                    mainColor = Color.Purple;
                    _scaler = 16;
                    break;
                }
            case ProjectileID.StarVeilStar:
                {
                    mainColor = Color.Lerp(Color.Pink, Color.HotPink, .5f);
                    _scaler = 16;
                    break;
                }
            case ProjectileID.IceBoomerang:
                {
                    mainColor = Color.Cyan;
                    isBoomerang = true;
                    break;
                }
            case ProjectileID.WoodenBoomerang:
                {
                    mainColor = Color.BurlyWood * .5f;
                    isBoomerang = true;
                    break;
                }
            case ProjectileID.Shroomerang:
                {
                    mainColor = Color.Lerp(Color.Blue, Color.Cyan, .5f);
                    isBoomerang = true;
                    break;
                }
            case ProjectileID.Bananarang:
                {
                    mainColor = Color.Yellow;
                    isBoomerang = true;
                    break;
                }
            case ProjectileID.EnchantedBoomerang:
                {
                    mainColor = Color.Blue;
                    isBoomerang = true;
                    break;
                }
            case ProjectileID.Flamarang:
                {
                    mainColor = Color.Orange;
                    isBoomerang = true;
                    break;
                }
        }
        if (isBoomerang) { angleOffset = -MathHelper.PiOver4; _scaler = 8; facVel = 0; }//23

        #region offsetAlpha

        if (projectile.tileCollide && !ProjectileModificationPreview.PVDrawing)
        {
            var vCenter = projectile.Center;
            var t = 0;
            var tile = Framing.GetTileSafely((int)vCenter.X / 16, (int)vCenter.Y / 16);

            while (t < 30 && !(tile.HasTile && Main.tileSolid[tile.TileType]))// || tile.TileType == TileID.TargetDummy
            {
                vCenter += projectile.velocity;
                t++;
                Point coord = new((int)vCenter.X / 16, (int)vCenter.Y / 16);
                if (coord.X > 0 && coord.X < Main.tile.Width && coord.Y > 0 && coord.Y < Main.tile.Height)
                    tile = Framing.GetTileSafely(coord);
            }
            mainColor *= MathHelper.Clamp((t - 1) / 30f, 0, 1);
        }

        #endregion offsetAlpha

        var multiValue = 1 - projectile.localAI[0] / 90f;

        if (projectile.type == ProjectileID.DeathSickle) mainColor *= facVel;

        var positionArray1 = new Vector2[max];
        var positionArray2 = new Vector2[max];
        for (var n = 0; n < max; n++)
        {
            var f = n / (max - 1f);
            var __scaler = 1 + MathF.Sqrt(f) * facVel;
            if (projectile.type == ProjectileID.NightBeam || projectile.type == ProjectileID.DeathSickle)
            {
                centerOffset = (projectile.oldRot[n] + angleOffset).ToRotationVector2() * -_scaler * .5f;
                //__scaler = projectile.type == ProjectileID.NightBeam ? (1 + MathF.Sqrt(f)) : 1;
                //__scaler = 1 + MathF.Sqrt(f) * facVel;
            }
            if (isBoomerang)
            {
                centerOffset = projectile.oldRot[n].ToRotationVector2() * -9;
            }
            positionArray1[n] = projectile.oldPos[n] + (projectile.oldRot[n] + angleOffset + (projectile.type == ProjectileID.StarWrath ? -MathHelper.TwoPi * f / 4f : 0)).ToRotationVector2() * _scaler * __scaler + centerOffset;
            positionArray2[n] = projectile.oldPos[n] + centerOffset;
        }
        positionArray1 = max > 4 ? positionArray1.CatMullRomCurve(max * 3) : positionArray1;
        positionArray2 = max > 4 ? positionArray2.CatMullRomCurve(max * 3) : positionArray2;

        var alphaLight = 0.5f;

        bars.Add(new CustomVertexInfo(positionArray1[0], default(Color), new Vector3(1, 1, alphaLight)));
        bars.Add(new CustomVertexInfo(positionArray2[0], default(Color), new Vector3(0, 0, alphaLight)));
        for (var i = 0; i < positionArray1.Length; i++)
        {
            var f = i / (positionArray1.Length - 1f);
            f = 1 - f;
            var _f = 6 * f / (3 * f + 1);//6 * f / (3 * f + 1) /(float)Math.Pow(f,instance.maxCount)
            _f = MathHelper.Clamp(_f, 0, 1);
            _f = f * f;
            mainColor.A = (byte)(_f * 255);
            bars.Add(new CustomVertexInfo(positionArray1[i], mainColor * multiValue, new Vector3(1 - f, 1, alphaLight)));
            mainColor.A = 0;
            bars.Add(new CustomVertexInfo(positionArray2[i], mainColor * multiValue, new Vector3(0, 0, alphaLight)));
        }
        List<CustomVertexInfo> _triangleList = [];
        var sampler = SamplerState.AnisotropicWrap;
        //RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
        var bounds = Main.instance.Window.ClientBounds;
        var projection = Matrix.CreateOrthographicOffCenter(0, bounds.Width, bounds.Height, 0, 0, 1);
        var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
        var trans = spriteBatch.transformMatrix;// Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
        var sb = Main.spriteBatch;

        if (bars.Count > 2)
        {
            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
            //projectile.DrawProjWithStarryTrail(spriteBatch, Main.rgbToHsl(mainColor).X, Color.White, 0);
            var counter = 0;
        theLabel:
            for (var i = 0; i < bars.Count - 2; i += 2)
            {
                _triangleList.Add(bars[i]);
                _triangleList.Add(bars[i + 2]);
                _triangleList.Add(bars[i + 1]);
                _triangleList.Add(bars[i + 1]);
                _triangleList.Add(bars[i + 2]);
                _triangleList.Add(bars[i + 3]);
            }
            counter++;
            switch (projectile.type)
            {
                case ProjectileID.StarCannonStar:
                case ProjectileID.SuperStar:
                case ProjectileID.Starfury:
                case ProjectileID.StarWrath:
                case ProjectileID.FallingStar:
                case ProjectileID.HallowStar:
                case ProjectileID.ManaCloakStar:
                case ProjectileID.BeeCloakStar:
                case ProjectileID.StarCloakStar:
                case ProjectileID.StarVeilStar:
                    {
                        if (counter < 5)
                        {
                            bars.Clear();

                            angleOffset += MathHelper.TwoPi / 5;
                            bars.Add(new CustomVertexInfo(projectile.oldPos[0] + (projectile.oldRot[0] + angleOffset).ToRotationVector2() * _scaler + centerOffset, default(Color), new Vector3(1, 1, alphaLight)));
                            bars.Add(new CustomVertexInfo(projectile.oldPos[0] + centerOffset, default(Color), new Vector3(0, 0, alphaLight)));
                            var k = 1 - counter / 5f;
                            for (var i = 0; i < max; i++)
                            {
                                var f = i / (max - 1f);
                                f = 1 - f;
                                var _f = 6 * f / (3 * f + 1);//6 * f / (3 * f + 1) /(float)Math.Pow(f,instance.maxCount)
                                _f = MathHelper.Clamp(_f, 0, 1);
                                _f = f * f;
                                mainColor.A = (byte)(_f * 255);
                                bars.Add(new CustomVertexInfo(projectile.oldPos[i] + (projectile.oldRot[i] + angleOffset + (projectile.type == ProjectileID.StarWrath ? -MathHelper.TwoPi * (1 - f) / 4f : 0)).ToRotationVector2() * _scaler * (1 + MathF.Sqrt(1 - f) * 3) + centerOffset, mainColor * multiValue * k, new Vector3(1 - f, 1, alphaLight)));
                                mainColor.A = 0;
                                bars.Add(new CustomVertexInfo(projectile.oldPos[i] + centerOffset, mainColor * multiValue * k, new Vector3(0, 0, alphaLight)));
                            }
                            if (counter == 4)
                            {
                                var u = -projectile.velocity.SafeNormalize(default);
                                spriteBatch.DrawQuadraticLaser_PassNormal(projectile.Center - 24 * u, u, mainColor with { A = 255 }, LogSpiralLibraryMod.AniTex[8].Value, 128, 64, 0, 1, 0.25f, false);
                                spriteBatch.DrawQuadraticLaser_PassNormal(projectile.Center - 24 * u, u, mainColor with { A = 255 } * 1.5f, LogSpiralLibraryMod.AniTex[8].Value, 256, 32, 0, 1f, 0.25f, false);
                                spriteBatch.DrawQuadraticLaser_PassNormal(projectile.Center - 24 * u, u, mainColor with { A = 255 } * .5f, LogSpiralLibraryMod.AniTex[8].Value, 64, 128, 0, 1, 0.25f, false);
                            }
                            goto theLabel;
                        }
                        break;
                    }
            }
            if (isBoomerang && counter < 2)
            {
                bars.Clear();

                angleOffset += MathHelper.PiOver2;
                bars.Add(new CustomVertexInfo(projectile.oldPos[0] + (projectile.oldRot[0] + angleOffset).ToRotationVector2() * _scaler + centerOffset, default(Color), new Vector3(1, 1, alphaLight)));
                bars.Add(new CustomVertexInfo(projectile.oldPos[0] + centerOffset, default(Color), new Vector3(0, 0, alphaLight)));
                for (var i = 0; i < max; i++)
                {
                    var f = i / (max - 1f);
                    f = 1 - f;
                    var _f = 6 * f / (3 * f + 1);//6 * f / (3 * f + 1) /(float)Math.Pow(f,instance.maxCount)
                    _f = MathHelper.Clamp(_f, 0, 1);
                    mainColor.A = (byte)(_f * 255);
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + (projectile.oldRot[i] + angleOffset + (projectile.type == ProjectileID.StarWrath ? -MathHelper.TwoPi * (1 - f) / 4f : 0)).ToRotationVector2() * _scaler * (1 + MathF.Sqrt(1 - f) * 3) + centerOffset, mainColor * multiValue, new Vector3(1 - f, 1, alphaLight)));
                    mainColor.A = 0;
                    bars.Add(new CustomVertexInfo(projectile.oldPos[i] + centerOffset, mainColor * multiValue, new Vector3(0, 0, alphaLight)));
                }
                goto theLabel;
            }
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
            //ShaderSwooshEX.Parameters["uLighter"].SetValue(ConfigSwooshInstance.luminosityFactor);
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["uTime"].SetValue(-(float)LogSpiralLibraryMod.ModTime * 0.03f);
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["checkAir"].SetValue(false);
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["gather"].SetValue(true);
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["distortScaler"].SetValue(0);
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["alphaFactor"].SetValue(MeleeConfig.Instance.alphaFactor);
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(MeleeConfig.Instance.alphaFactor == 0);
            var _v = MeleeConfig.Instance.directOfHeatMap.ToRotationVector2();
            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });
            //var par = CoolerItemVisualEffect.ShaderSwooshEX.Parameters["heatRotation"];
            //var wht = (par.Annotations, par.ColumnCount, par.RowCount, par.ParameterType, par.Elements, par.Name, par.ParameterClass, par.Semantic, par.StructureMembers);            Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.BaseTex[instance.ImageIndex].Value;
            Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.BaseTex_Swoosh[MeleeConfig.Instance.baseIndexSwoosh].Value;
            Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.AniTex_Swoosh[MeleeConfig.Instance.animateIndexSwoosh].Value;
            Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.Item[ItemID.InfluxWaver].Value;//ModContent.Request<Texture2D>("CoolerItemVisualEffect/Weapons/FirstZenithProj_5").Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
            //Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

            CoolerItemVisualEffectMod.ShaderSwooshEX.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleList.ToArray(), 0, _triangleList.Count / 3);

            //Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //sb.End();
            sb.End();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        }
        //lightColor.A = 0;
    }
}
