using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures;

namespace CoolerItemVisualEffect
{
    /// <summary>
    /// 部分弹幕绘制的修改
    /// </summary>
    public class ProjectileDrawingModify : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            //if (projectile.ModProjectile is VertexHammerProj vertexHammer)
            //{
            //    var player = vertexHammer.Player;
            //    var modplr = player.GetModPlayer<CoolerItemVisualEffectPlayer>();
            //    if (modplr.colorInfo.tex == null)
            //    {
            //        Main.RunOnMainThread(() => modplr.colorInfo.tex = new Texture2D(Main.graphics.GraphicsDevice, 300, 1));
            //    }
            //    CoolerItemVisualEffectPlayer.ChangeItemTex(player);
            //    CoolerItemVisualEffectMod.UpdateHeatMap(ref modplr.colorInfo.tex, modplr.hsl, modplr.ConfigurationSwoosh, TextureAssets.Item[player.HeldItem.type].Value);
            //    vertexHammer.heatMap = modplr.colorInfo.tex;
            //}
            base.OnSpawn(projectile, source);
        }
        public override void PostAI(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.NightBeam:
                case ProjectileID.DeathSickle:

                case ProjectileID.Starfury:
                case ProjectileID.StarCannonStar:
                case ProjectileID.SuperStar:
                case ProjectileID.StarWrath:
                case ProjectileID.FallingStar:
                case ProjectileID.HallowStar:
                case ProjectileID.ManaCloakStar:
                case ProjectileID.BeeCloakStar:
                case ProjectileID.StarVeilStar:
                case ProjectileID.StarCloakStar:

                case ProjectileID.EnchantedBoomerang:
                case ProjectileID.IceBoomerang:
                case ProjectileID.WoodenBoomerang:
                case ProjectileID.Flamarang:
                case ProjectileID.Bananarang:
                case ProjectileID.Shroomerang:

                case ProjectileID.WoodenArrowHostile:
                case ProjectileID.WoodenArrowFriendly:
                case ProjectileID.VenomArrow:
                case ProjectileID.UnholyArrow:
                case ProjectileID.ShadowFlameArrow:
                case ProjectileID.PhantasmArrow:
                case ProjectileID.MoonlordArrow:
                case ProjectileID.JestersArrow:
                case ProjectileID.IchorArrow:
                case ProjectileID.HolyArrow:
                case ProjectileID.HellfireArrow:
                case ProjectileID.FrostburnArrow:
                case ProjectileID.FrostArrow:
                case ProjectileID.FlamingArrow:
                case ProjectileID.FireArrow:
                case ProjectileID.DD2BetsyArrow:
                case ProjectileID.CursedArrow:
                case ProjectileID.ChlorophyteArrow:
                case ProjectileID.BoneArrowFromMerchant:
                case ProjectileID.BoneArrow:
                case ProjectileID.BloodArrow:
                    {
                        if (ProjectileID.Sets.TrailingMode[projectile.type] == -1)// !Main.gamePaused && 
                        {
                            for (int n = projectile.oldPos.Length - 1; n > 0; n--)
                            {
                                projectile.oldPos[n] = projectile.oldPos[n - 1];
                                projectile.oldRot[n] = projectile.oldRot[n - 1];
                            }
                            projectile.oldPos[0] = projectile.Center;
                            projectile.oldRot[0] = projectile.rotation;
                        }
                        break;
                    }
            }
            base.PostAI(projectile);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!ConfigurationCIVE.ConfigCIVEInstance.VanillaProjectileDrawModifyActive) goto mylabel;
            SpriteBatch spriteBatch = Main.spriteBatch;
            //spriteBatch.Draw4C(TextureAssets.Extra[98].Value, projectile.Center - Main.screenPosition, null, Color.Cyan with { A = 0 }, Color.Cyan with { A = 0}, Color.Blue with { A = 0 }, Color.Blue with { A = 0 }, projectile.rotation, new Vector2(36), new Vector2(1,3), 0, 0);
            //spriteBatch.Draw(TextureAssets.Extra[98].Value, projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, projectile.rotation, new Vector2(36), new Vector2(1, 3) * .75f, 0, 0);

            switch (projectile.type)
            {
                case ProjectileID.NightBeam:
                //case ProjectileID.DeathSickle:

                case ProjectileID.Starfury:
                case ProjectileID.StarCannonStar:
                case ProjectileID.SuperStar:
                case ProjectileID.StarWrath:
                case ProjectileID.FallingStar:
                case ProjectileID.HallowStar:
                case ProjectileID.ManaCloakStar:
                case ProjectileID.BeeCloakStar:
                case ProjectileID.StarVeilStar:
                case ProjectileID.StarCloakStar:

                case ProjectileID.EnchantedBoomerang:
                case ProjectileID.IceBoomerang:
                case ProjectileID.WoodenBoomerang:
                case ProjectileID.Flamarang:
                case ProjectileID.Bananarang:
                case ProjectileID.Shroomerang:
                    {

                        //if (!Main.gamePaused && ProjectileID.Sets.TrailingMode[projectile.type] == -1)// 
                        //{
                        //    for (int n = projectile.oldPos.Length - 1; n > 0; n--)
                        //    {
                        //        projectile.oldPos[n] = projectile.oldPos[n - 1];
                        //        projectile.oldRot[n] = projectile.oldRot[n - 1];
                        //    }
                        //    projectile.oldPos[0] = projectile.Center;
                        //    projectile.oldRot[0] = projectile.rotation;
                        //}
                        if (CoolerItemVisualEffectMod.RenderEffect == null || CoolerItemVisualEffectMod.ShaderSwooshEX == null) goto mylabel;
                        //goto mylabel;
                        var bars = new List<CustomVertexInfo>();
                        var max = projectile.oldPos.Length;
                        for (int n = 0; n < projectile.oldPos.Length; n++)
                        {
                            if (projectile.oldPos[n] == default) { max = n; break; }
                        }
                        if (max < 2) { goto mylabel; }//Main.NewText("太短了太短了！！  " + max + "   " + projectile.localAI[0] + "   " + projectile.oldPos[0]);

                        float _scaler = 98f;
                        var mainColor = new Color(151, 145, 186);
                        float angleOffset = 0f;
                        Vector2 centerOffset = default;
                        var facVel = 1 - 1 / (projectile.velocity.Length() / 4 + 1);
                        facVel *= 3;
                        bool isBoomerang = false;
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
                                        ProjectileID.Sets.TrailCacheLength[ProjectileID.StarWrath] = 20;
                                    }
                                    break;
                                }
                            case ProjectileID.StarCannonStar:
                            case ProjectileID.FallingStar:
                            case ProjectileID.ManaCloakStar:
                                {
                                    mainColor = Color.Blue;
                                    _scaler = 16;
                                    break;
                                }
                            case ProjectileID.SuperStar:
                                {
                                    mainColor = Color.Yellow;
                                    _scaler = 16;
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
                        if (projectile.tileCollide)
                        {
                            var vCenter = projectile.Center;
                            int t = 0;
                            var tile = Main.tile[(int)vCenter.X / 16, (int)vCenter.Y / 16];

                            while (t < 30 && !(tile.HasTile && (Main.tileSolid[tile.TileType])))// || tile.TileType == TileID.TargetDummy
                            {
                                vCenter += projectile.velocity;
                                t++;
                                tile = Main.tile[(int)vCenter.X / 16, (int)vCenter.Y / 16];
                            }
                            mainColor *= MathHelper.Clamp((t - 1) / 30f, 0, 1);
                        }
                        #endregion
                        var multiValue = 1 - projectile.localAI[0] / 90f;

                        if (projectile.type == ProjectileID.DeathSickle) mainColor *= facVel;

                        var positionArray1 = new Vector2[max];
                        var positionArray2 = new Vector2[max];
                        for (int n = 0; n < max; n++)
                        {
                            var f = n / (max - 1f);
                            var __scaler = (1 + MathF.Sqrt(f) * facVel);
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
                        for (int i = 0; i < positionArray1.Length; i++)
                        {
                            var f = i / (positionArray1.Length - 1f);
                            f = 1 - f;
                            var _f = 6 * f / (3 * f + 1);//6 * f / (3 * f + 1) /(float)Math.Pow(f,instance.maxCount)
                            _f = MathHelper.Clamp(_f, 0, 1);
                            mainColor.A = (byte)(_f * 255);
                            bars.Add(new CustomVertexInfo(positionArray1[i], mainColor * multiValue, new Vector3(1 - f, 1, alphaLight)));
                            mainColor.A = 0;
                            bars.Add(new CustomVertexInfo(positionArray2[i], mainColor * multiValue, new Vector3(0, 0, alphaLight)));
                        }
                        List<CustomVertexInfo> _triangleList = new List<CustomVertexInfo>();
                        SamplerState sampler = SamplerState.AnisotropicWrap;
                        //RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                        var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                        var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                        var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
                        var sb = Main.spriteBatch;

                        if (bars.Count > 2)
                        {
                            sb.End();

                            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                            //projectile.DrawProjWithStarryTrail(spriteBatch, Main.rgbToHsl(mainColor).X, Color.White, 0);
                            int counter = 0;
                        theLabel:
                            for (int i = 0; i < bars.Count - 2; i += 2)
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
                                            for (int i = 0; i < max; i++)
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
                                        break;
                                    }
                            }
                            if (isBoomerang && counter < 2)
                            {
                                bars.Clear();

                                angleOffset += MathHelper.PiOver2;
                                bars.Add(new CustomVertexInfo(projectile.oldPos[0] + (projectile.oldRot[0] + angleOffset).ToRotationVector2() * _scaler + centerOffset, default(Color), new Vector3(1, 1, alphaLight)));
                                bars.Add(new CustomVertexInfo(projectile.oldPos[0] + centerOffset, default(Color), new Vector3(0, 0, alphaLight)));
                                for (int i = 0; i < max; i++)
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
                            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["alphaFactor"].SetValue(ConfigurationCIVE.ConfigCIVEInstance.alphaFactor);
                            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(ConfigurationCIVE.ConfigCIVEInstance.alphaFactor == 0);
                            var _v = ConfigurationCIVE.ConfigCIVEInstance.directOfHeatMap.ToRotationVector2();
                            CoolerItemVisualEffectMod.ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });
                            //var par = CoolerItemVisualEffect.ShaderSwooshEX.Parameters["heatRotation"];
                            //var wht = (par.Annotations, par.ColumnCount, par.RowCount, par.ParameterType, par.Elements, par.Name, par.ParameterClass, par.Semantic, par.StructureMembers);            Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.BaseTex[instance.ImageIndex].Value;
                            Main.graphics.GraphicsDevice.Textures[0] = LogSpiralLibraryMod.BaseTex_Swoosh[ConfigurationCIVE.ConfigCIVEInstance.baseIndexSwoosh].Value;
                            Main.graphics.GraphicsDevice.Textures[1] = LogSpiralLibraryMod.AniTex_Swoosh[ConfigurationCIVE.ConfigCIVEInstance.animateIndexSwoosh].Value;
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
                        goto mylabel;
                    }
                case ProjectileID.TerraBeam:
                case ProjectileID.EnchantedBeam:
                case ProjectileID.LightBeam:
                case ProjectileID.SwordBeam:
                case ProjectileID.InfluxWaver:
                case ProjectileID.SkyFracture:
                    {
                        Texture2D projectileTexture = LogSpiralLibraryMod.Misc[13].Value;//GetTexture("TerraShard");
                        var length = projectile.velocity.Length();
                        var scaleVec = new Vector2(projectile.scale * 3 + 1f + MathHelper.Clamp(length / 4f, 0, 2), projectile.scale * 2);
                        scaleVec = new Vector2(scaleVec.Y, scaleVec.X / 3f);
                        scaleVec *= new Vector2(.5f, .75f);
                        if (!Main.gamePaused)
                        {
                            for (int k = projectile.oldPos.Length - 1; k > 0; k--)
                            {
                                projectile.oldPos[k] = projectile.oldPos[k - 1];
                                projectile.oldRot[k] = projectile.oldRot[k - 1];
                            }
                            projectile.oldPos[0] = projectile.Center;
                            projectile.oldRot[0] = projectile.rotation;
                        }
                        //for (int k = projectile.oldPos.Length - 1; k > 0; k--)
                        //{
                        //    Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition;
                        //    var factor = 1 - k / (float)projectile.oldPos.Length;
                        //    spriteBatch.Draw(projectileTexture, drawPos, null, Color.LimeGreen * factor, projectile.rotation - MathHelper.PiOver4, new Vector2(7.5f, 3.5f), (1 - 0.1f * k) * scaleVec, SpriteEffects.None, 0f);
                        //}
                        //var unit = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
                        //spriteBatch.Draw(projectileTexture, projectile.Center - Main.screenPosition - unit * 24, null, Color.White, projectile.rotation - MathHelper.PiOver4, new Vector2(7.5f, 3.5f), scaleVec, SpriteEffects.None, 0f);
                        //var mainColor = Color.White;
                        //switch (projectile.type) 
                        //{

                        //}
                        var mainColor = projectile.type switch
                        {
                            ProjectileID.TerraBeam => Color.LimeGreen,
                            ProjectileID.EnchantedBeam => Color.Cyan,
                            ProjectileID.LightBeam => Color.HotPink,
                            ProjectileID.SwordBeam => Color.Yellow,
                            ProjectileID.InfluxWaver => Color.Lerp(Color.LightCyan, Color.Cyan, .5f),
                            ProjectileID.SkyFracture => Color.Lerp(Color.LightCyan, Color.Cyan, .5f),
                            _ => default
                        };
                        #region offsetAlpha
                        {
                            var vCenter = projectile.Center;
                            int t = 0;
                            var tile = Main.tile[(int)vCenter.X / 16, (int)vCenter.Y / 16];

                            while (t < 30 && !(tile.HasTile && Main.tileSolid[tile.TileType]))
                            {
                                vCenter += projectile.velocity;
                                t++;
                                tile = Main.tile[(int)vCenter.X / 16, (int)vCenter.Y / 16];
                            }
                            mainColor *= MathHelper.Clamp((t - 1) / 30f, 0, 1);
                        }
                        #endregion
                        //for (int k = projectile.oldPos.Length - 1; k > 0; k--)
                        //{
                        //    Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition;
                        //    var factor = 1 - k / (float)projectile.oldPos.Length;
                        //    spriteBatch.Draw(projectileTexture, drawPos, null, mainColor with { A = 0 } * factor, projectile.rotation - MathHelper.PiOver4 * 3, new Vector2(36), (1 - 0.1f * k) * scaleVec, SpriteEffects.None, 0f);
                        //}
                        //lightColor = lightColor with { A = 0 };
                        //if (projectile.type != ProjectileID.InfluxWaver)
                        //{
                        //    projectile.alpha = 0;
                        //    projectile.scale = 1;
                        //}
                        //else 
                        //{
                        //    Main.NewText((projectile.alpha, projectile.scale));
                        //}
                        var unit = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
                        var center = projectile.Center - Main.screenPosition;
                        spriteBatch.Draw(projectileTexture, center - unit * 24, null, mainColor with { A = 0 }, projectile.rotation - MathHelper.PiOver4 * 3, new Vector2(36), scaleVec * new Vector2(.75f, 1.5f), SpriteEffects.None, 0f);
                        spriteBatch.Draw(projectileTexture, center - unit * 24, null, Color.White with { A = 0 }, projectile.rotation - MathHelper.PiOver4 * 3, new Vector2(36), scaleVec * new Vector2(.5f, 1), SpriteEffects.None, 0f);
                        spriteBatch.DrawQuadraticLaser_PassNormal(projectile.Center - unit * 16, -unit, mainColor, LogSpiralLibraryMod.AniTex[10].Value, MathHelper.Clamp(length, 0, 16) * 4 + 36, 16);
                        //spriteBatch.DrawQuadraticLaser_PassHeatMap(projectile.Center, -unit, GetTexture("HeatMap_11"), GetTexture("Style_10"), MathHelper.Clamp(length, 0, 16) * 4 + 28, 24);
                        spriteBatch.DrawEffectLine(projectile.Center - unit * 24, projectile.velocity.SafeNormalize(default), mainColor, LogSpiralLibraryMod.BaseTex[12].Value, 1, 0, 96, 15);
                        //lightColor = lightColor with { A = 0};
                        var projTex = TextureAssets.Projectile[projectile.type].Value;
                        Rectangle? rect = projectile.type == ProjectileID.SkyFracture ? projTex.Frame(14, 1, projectile.frame, 0) : null;
                        for (int n = 0; n < 4; n++)
                        {
                            var offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(Main.rand.NextFloat(12f)) - projectile.velocity * 3;
                            spriteBatch.Draw(projTex, center + offset, rect, Color.White with { A = 0 } * .5f * (1 - projectile.alpha / 255f), projectile.rotation, (rect != null ? rect.Value.Size() : projTex.Size()) * .5f, projectile.scale, 0, 0);
                            //spriteBatch.Draw(projectileTexture, center - unit * 24 + offset, null, mainColor with { A = 0 } * .25f, projectile.rotation - MathHelper.PiOver4 * 3, new Vector2(36), scaleVec * new Vector2(.75f, 1.5f), SpriteEffects.None, 0f);
                            //spriteBatch.Draw(projectileTexture, center - unit * 24 + offset, null, Color.White with { A = 0 } * .25f, projectile.rotation - MathHelper.PiOver4 * 3, new Vector2(36), scaleVec * new Vector2(.5f, 1), SpriteEffects.None, 0f);
                        }
                        spriteBatch.Draw(projTex, center - projectile.velocity * 3, rect, Color.White with { A = 0 } * (1 - projectile.alpha / 255f), projectile.rotation, (rect != null ? rect.Value.Size() : projTex.Size()) * .5f, projectile.scale, 0, 0);

                        return false;//base.PreDraw(projectile,ref lightColor)
                    }
                case ProjectileID.Meowmere:
                    {
                        goto mylabel;
                    }
                case ProjectileID.WoodenArrowHostile:
                case ProjectileID.WoodenArrowFriendly:
                case ProjectileID.VenomArrow:
                case ProjectileID.UnholyArrow:
                case ProjectileID.ShadowFlameArrow:
                case ProjectileID.PhantasmArrow:
                case ProjectileID.MoonlordArrow:
                case ProjectileID.JestersArrow:
                case ProjectileID.IchorArrow:
                case ProjectileID.HolyArrow:
                case ProjectileID.HellfireArrow:
                case ProjectileID.FrostburnArrow:
                case ProjectileID.FrostArrow:
                case ProjectileID.FlamingArrow:
                case ProjectileID.FireArrow:
                case ProjectileID.DD2BetsyArrow:
                case ProjectileID.CursedArrow:
                case ProjectileID.ChlorophyteArrow:
                case ProjectileID.BoneArrowFromMerchant:
                case ProjectileID.BoneArrow:
                case ProjectileID.BloodArrow:
                    //case ProjectileID.BeeArrow: 
                    {
                        //if (!Main.gamePaused && ProjectileID.Sets.TrailingMode[projectile.type] == -1)// 
                        //{
                        //    for (int n = projectile.oldPos.Length - 1; n > 0; n--)
                        //    {
                        //        projectile.oldPos[n] = projectile.oldPos[n - 1];
                        //        projectile.oldRot[n] = projectile.oldRot[n - 1];
                        //    }
                        //    projectile.oldPos[0] = projectile.Center;
                        //    projectile.oldRot[0] = projectile.rotation;
                        //}
                        var width = 30f;
                        var offset = -projectile.velocity * 2;
                        var alpha = .25f;
                        var mainColor = Color.White;
                        switch (projectile.type)
                        {
                            //case ProjectileID.WoodenArrowHostile:
                            //case ProjectileID.WoodenArrowFriendly:
                            case ProjectileID.UnholyArrow:
                            case ProjectileID.ShadowFlameArrow:
                            case ProjectileID.VenomArrow:
                                {
                                    mainColor = Color.Purple;
                                    alpha = 1;
                                    break;
                                }
                            case ProjectileID.PhantasmArrow:
                            case ProjectileID.MoonlordArrow:
                                {
                                    alpha = .75f;
                                    mainColor = Color.Lerp(Color.Cyan, Color.Green, .33f);
                                    break;
                                }
                            case ProjectileID.FlamingArrow:
                            case ProjectileID.FireArrow:
                            case ProjectileID.HellfireArrow:
                            case ProjectileID.DD2BetsyArrow:
                                {
                                    alpha = 1;
                                    mainColor = Color.Lerp(Color.Orange, Color.OrangeRed, .66f);
                                    break;
                                }
                            case ProjectileID.FrostburnArrow:
                            case ProjectileID.FrostArrow:
                                {
                                    alpha = .75f;
                                    mainColor = Color.Lerp(Color.Cyan, Color.Blue, .33f);
                                    break;
                                }
                            case ProjectileID.CursedArrow:
                            case ProjectileID.ChlorophyteArrow:
                                {
                                    alpha = .75f;
                                    mainColor = Color.LimeGreen;
                                    break;
                                }
                            case ProjectileID.JestersArrow:
                            case ProjectileID.IchorArrow:
                                {
                                    alpha = .75f;
                                    mainColor = Color.Lerp(Color.Yellow, Color.Orange, .33f);
                                    break;
                                }
                            case ProjectileID.HolyArrow:
                                {
                                    alpha = .75f;
                                    mainColor = Color.Lerp(Color.Pink, Color.HotPink, .5f);
                                    break;
                                }
                            case ProjectileID.BoneArrowFromMerchant:
                            case ProjectileID.BoneArrow:
                                {
                                    alpha = .5f;
                                    mainColor = Color.Lerp(Color.White, Color.BurlyWood, .5f);
                                    break;
                                }
                            case ProjectileID.BloodArrow:
                                {
                                    alpha = .5f;
                                    mainColor = Color.Red;
                                    break;
                                }
                            case ProjectileID.BeeArrow:
                                {
                                    break;
                                }
                        }
                        spriteBatch.DrawShaderTail(projectile, LogSpiralLibraryMod.BaseTex[8].Value, LogSpiralLibraryMod.AniTex[2].Value, LogSpiralLibraryMod.BaseTex[12].Value, width, offset, alpha, true, false, mainColor);
                        goto mylabel;
                    }
            }
        mylabel:
            return base.PreDraw(projectile, ref lightColor);
        }
        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            base.PostDraw(projectile, lightColor);
        }
    }
}
