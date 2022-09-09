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
using static CoolerItemVisualEffect.ConfigurationSwoosh_Advanced;
using System.Linq;
using Terraria.Localization;
using Terraria.Graphics.Renderers;
using MonoMod.Cil;

namespace CoolerItemVisualEffect
{
    public class CoolerItemVisualEffect : Mod
    {
        //public static Vector2 pos = Vector2.Zero;//旧刀光武器绘制位置定位坐标

        //internal static bool ItemAdditive;
        //internal static bool ToolsNoUse = false;
        //internal static float IsLighterDecider = 0.6f;
        //internal static bool UseItemTexForSwoosh = false;

        //private void Player_ItemCheck_MeleeHitNPCs(ILContext il)
        //{
        //    var c = new ILCursor(il);
        //    while (c.TryGotoNext(MoveType.After, i => i.MatchLdcR8(0.33)))
        //    {
        //        c.EmitDelegate<Func<double, double>>((_) =>
        //        {
        //            return 1.0 / ConfigurationNormal.instance.ItemAttackCD;
        //        });
        //    }
        //}

        internal static CoolerItemVisualEffect Instance;
        public static int ModTime => CoolerSystem.ModTime;
        public static void ChangeAllPureHeatMap()
        {
            Main.RunOnMainThread(() =>
            {
                for (int n = 0; n < 26; n++)
                {
                    //ChangePureHeatMap(n);
                    if (ConfigSwooshInstance == null) return;
                    var itemTex = GetPureFractalProjTexs(n);
                    if (itemTex == null) return;
                    if (Main.graphics.GraphicsDevice == null) return;
                    var w = itemTex.Width;
                    var he = itemTex.Height;
                    var cs = new Color[w * he];
                    itemTex.GetData(cs);
                    Vector4 vcolor = default;
                    var airFactor = 1f;
                    Color target = default;

                    float count = 0;
                    for (int i = 0; i < cs.Length; i++)
                    {
                        if (cs[i] != default && (i - w < 0 || cs[i - w] != default) && (i - 1 < 0 || cs[i - 1] != default) && (i + w >= cs.Length || cs[i + w] != default) && (i + 1 >= cs.Length || cs[i + 1] != default))
                        {
                            var weight = (float)((i + 1) % w * (he - i / w)) / w / he;
                            vcolor += cs[i].ToVector4() * weight;
                            count += weight;
                        }
                        Vector2 coord = new Vector2(i % w, i / w);
                        coord /= new Vector2(w, he);
                        if (ConfigSwooshInstance.checkAir && Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && cs[i] != default && target == default)
                        {
                            target = cs[i];
                            airFactor = coord.X;
                        }
                    }
                    vcolor /= count;
                    var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
                    PureFractalColors[n] = newColor;
                    PureFractalAirFactorss[n] = airFactor;
                    var hsl = Main.rgbToHsl(newColor);
                    var colors = new Color[300];
                    for (int i = 0; i < 300; i++)
                    {
                        var f = i / 299f;//分割成25次惹，f从1/25f到1//1 - 
                        f = f * f;// *f
                        float h = (hsl.X + ConfigSwooshInstance.hueOffsetValue + ConfigSwooshInstance.hueOffsetRange * (2 * f - 1)) % 1;
                        float s = MathHelper.Clamp(hsl.Y * ConfigSwooshInstance.saturationScalar, 0, 1);
                        float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + ConfigSwooshInstance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - ConfigSwooshInstance.luminosityRange), 0, 1);
                        colors[i] = Main.hslToRgb(h, s, l);
                    }
                    if (PureFractalHeatMaps[n] == null) PureFractalHeatMaps[n] = new Texture2D(Main.graphics.GraphicsDevice, 300, 1);
                    PureFractalHeatMaps[n].SetData(colors);
                }
            });
        }
        public static void ChangePureHeatMap(int n)
        {
            Main.RunOnMainThread(() =>
            {
                if (ConfigSwooshInstance == null) return;
                var itemTex = GetPureFractalProjTexs(n);
                if (itemTex == null) return;
                if (Main.graphics.GraphicsDevice == null) return;
                var w = itemTex.Width;
                var he = itemTex.Height;
                var cs = new Color[w * he];
                itemTex.GetData(cs);
                Vector4 vcolor = default;
                float count = 0;
                var airFactor = 1f;
                Color target = default;
                for (int i = 0; i < cs.Length; i++)
                {
                    if (cs[i] != default && (i - w < 0 || cs[i - w] != default) && (i - 1 < 0 || cs[i - 1] != default) && (i + w >= cs.Length || cs[i + w] != default) && (i + 1 >= cs.Length || cs[i + 1] != default))
                    {
                        var weight = (float)((i + 1) % w * (he - i / w)) / w / he;
                        vcolor += cs[i].ToVector4() * weight;
                        count += weight;
                    }
                    Vector2 coord = new Vector2(n % w, n / w);
                    coord /= new Vector2(w, he);
                    if (ConfigSwooshInstance.checkAir && Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && cs[n] != default && target == default)
                    {
                        target = cs[n];
                        airFactor = coord.X;
                    }
                }
                vcolor /= count;
                var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
                PureFractalColors[n] = newColor;
                PureFractalAirFactorss[n] = airFactor;

                var hsl = Main.rgbToHsl(newColor);
                var colors = new Color[300];
                for (int i = 0; i < 300; i++)
                {
                    var f = i / 299f;//分割成25次惹，f从1/25f到1//1 - 
                    f = f * f;// *f
                    float h = (hsl.X + ConfigSwooshInstance.hueOffsetValue + ConfigSwooshInstance.hueOffsetRange * (2 * f - 1)) % 1;
                    float s = MathHelper.Clamp(hsl.Y * ConfigSwooshInstance.saturationScalar, 0, 1);
                    float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + ConfigSwooshInstance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - ConfigSwooshInstance.luminosityRange), 0, 1);
                    colors[i] = Main.hslToRgb(h, s, l);
                }
                if (PureFractalHeatMaps[n] == null) PureFractalHeatMaps[n] = new Texture2D(Main.graphics.GraphicsDevice, 300, 1);
                PureFractalHeatMaps[n].SetData(colors);
            }
            );
        }
        public override void Load()
        {
            Instance = this;
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_27_HeldItem += PlayerDrawLayers_DrawPlayer_27_HeldItem_WeaponDisplay;
            //On.Terraria.DataStructures.PlayerDrawLayers.drplayer
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            //IL.Terraria.Player.ItemCheck_MeleeHitNPCs += Player_ItemCheck_MeleeHitNPCs;
            On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayerInternal += LegacyPlayerRenderer_DrawPlayerInternal;
            On.Terraria.Graphics.Effects.FilterManager.EndCapture += FilterManager_EndCapture_CoolerSwoosh;
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles_CoolerSwoosh;
            //On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayerInternal += LegacyPlayerRenderer_DrawPlayerInternal_WD;
            //On.Terraria.GameContent.Skies.CreditsRoll.Segments.PlayerSegment.Draw += PlayerSegment_Draw_WD;
            //CreateRender();

        }

        private void Main_DrawProjectiles_CoolerSwoosh(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            if (CanUseRender) goto _myLabel;
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
                SpriteBatch spriteBatch = Main.spriteBatch;
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
                            if (ConfigSwooshInstance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 || ConfigSwooshInstance.swooshColorType == SwooshColorType.色调处理与对角线混合)
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
                if (DistortEffect == null || ShaderSwooshEX == null) return;
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
                    switch (ConfigSwooshInstance.swooshColorType)
                    {
                        case SwooshColorType.函数生成热度图: passCount = 2; break;
                        case SwooshColorType.武器贴图对角线: passCount = 1; break;
                        case SwooshColorType.色调处理与对角线混合: passCount = 3; break;
                        case SwooshColorType.加权平均_饱和与色调处理: passCount = 4; break;
                    }
                    ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                    //ShaderSwooshEX.Parameters["uLighter"].SetValue(ConfigSwooshInstance.luminosityFactor);
                    ShaderSwooshEX.Parameters["uTime"].SetValue(0);
                    ShaderSwooshEX.Parameters["checkAir"].SetValue(ConfigSwooshInstance.checkAir);
                    ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
                    ShaderSwooshEX.Parameters["gather"].SetValue(ConfigSwooshInstance.gather);
                    ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
                    ShaderSwooshEX.Parameters["distortScaler"].SetValue(0);
                    var _v = ConfigSwooshInstance.directOfHeatMap.ToRotationVector2();
                    ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });
                    Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + ConfigSwooshInstance.ImageIndex);
                    Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage($"AniTex_{ConfigSwooshInstance.AnimateIndex}");
                    Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Weapons/FirstZenithProj_5").Value;
                    if (ConfigSwooshInstance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = GetPureFractalHeatMaps(25);
                    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
                    ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
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
        _myLabel:

            orig(self);
        }
        public static Texture2D emptyTex;

        private void FilterManager_EndCapture_CoolerSwoosh(On.Terraria.Graphics.Effects.FilterManager.orig_EndCapture orig, Terraria.Graphics.Effects.FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            //goto mylabel;
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
                SpriteBatch spriteBatch = Main.spriteBatch;
                var bars = new List<CustomVertexInfo>();
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
                        bars.Add(new CustomVertexInfo(projectile.oldPos[0], default(Color), new Vector3(0, 0, 0.6f)));
                        for (int i = 0; i < max; i++)
                        {
                            var f = i / (max - 1f);
                            f = 1 - f;
                            var alphaLight = 0.6f;
                            if (ConfigSwooshInstance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 || ConfigSwooshInstance.swooshColorType == SwooshColorType.色调处理与对角线混合)
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

                //Main.PlayerRenderer.DrawPlayer(Main.Camera, Main.player[projectile.owner], projectile.Center, 0f, new Vector2(20, 28));
                #endregion
                //SpriteEffects spriteEffects = SpriteEffects.None;
                //if (projectile.spriteDirection == -1)
                //{
                //    spriteEffects = SpriteEffects.FlipHorizontally;
                //}
                if (DistortEffect == null || ShaderSwooshEX == null) return;

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
                    switch (ConfigSwooshInstance.swooshColorType)
                    {
                        case SwooshColorType.函数生成热度图: passCount = 2; break;
                        case SwooshColorType.武器贴图对角线: passCount = 1; break;
                        case SwooshColorType.色调处理与对角线混合: passCount = 3; break;
                        case SwooshColorType.加权平均_饱和与色调处理: passCount = 4; break;
                    }
                    ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                    //ShaderSwooshEX.Parameters["uLighter"].SetValue(ConfigSwooshInstance.luminosityFactor);
                    ShaderSwooshEX.Parameters["uTime"].SetValue(0);
                    ShaderSwooshEX.Parameters["checkAir"].SetValue(ConfigSwooshInstance.checkAir);
                    ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
                    ShaderSwooshEX.Parameters["gather"].SetValue(ConfigSwooshInstance.gather);
                    var _v = ConfigSwooshInstance.directOfHeatMap.ToRotationVector2();
                    ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });
                    Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + ConfigSwooshInstance.ImageIndex);
                    Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage($"AniTex_{ConfigSwooshInstance.AnimateIndex}");
                    Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Weapons/FirstZenithProj_5").Value;
                    if (ConfigSwooshInstance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = GetPureFractalHeatMaps(25);
                    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
                    ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
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
                                DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
                                DistortEffect.Parameters["offset"].SetValue(new Vector2(0.707f) * -0.09f * ConfigSwooshInstance.distortFactor);
                                DistortEffect.Parameters["invAlpha"].SetValue(0);
                                DistortEffect.CurrentTechnique.Passes[0].Apply();
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
                            for (int n = 0; n < ConfigSwooshInstance.maxCount; n++)
                            {

                                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                                gd.SetRenderTarget(Main.screenTargetSwap);
                                DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                                DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
                                DistortEffect.Parameters["position"].SetValue(new Vector2(0, 4));
                                DistortEffect.Parameters["tier2"].SetValue(ConfigSwooshInstance.luminosityFactor);
                                gd.Clear(Color.Transparent);
                                DistortEffect.CurrentTechnique.Passes[7].Apply();
                                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

                                gd.SetRenderTarget(Main.screenTarget);
                                gd.Clear(Color.Transparent);
                                DistortEffect.CurrentTechnique.Passes[6].Apply();
                                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);

                                DistortEffect.Parameters["position"].SetValue(new Vector2(0, 9));
                                DistortEffect.Parameters["ImageSize"].SetValue(new Vector2(0.707f) * -0.008f * ConfigSwooshInstance.distortFactor);
                                gd.SetRenderTarget(Main.screenTargetSwap);
                                gd.Clear(Color.Transparent);
                                DistortEffect.CurrentTechnique.Passes[5].Apply();
                                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

                                gd.SetRenderTarget(Main.screenTarget);
                                gd.Clear(Color.Transparent);
                                DistortEffect.CurrentTechnique.Passes[4].Apply();
                                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                                sb.End();
                                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                                sb.Draw(Instance.Render, Vector2.Zero, Color.White);
                                sb.End();
                            }
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
            //mylabel:

            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }

        static void DrawSwooshWithPlayer(Player drawPlayer)
        {
            var modPlayer = drawPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>();
            if (modPlayer.UseSlash || modPlayer.SwooshActive)
            {
                if (!TextureAssets.Item[drawPlayer.HeldItem.type].IsLoaded) TextureAssets.Item[drawPlayer.HeldItem.type] = Main.Assets.Request<Texture2D>("Images/Item_" + drawPlayer.HeldItem.type, ReLogic.Content.AssetRequestMode.AsyncLoad);
                var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
                if (modPlayer.colorInfo.type != drawPlayer.HeldItem.type)
                {
                    var w = itemTex.Width;
                    var h = itemTex.Height;
                    var cs = new Color[w * h];

                    itemTex.GetData(cs);
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
                        if (modPlayer.ConfigurationSwoosh.checkAir && Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && cs[n] != default && target == default)
                        {
                            target = cs[n];
                            modPlayer.colorInfo.checkAirFactor = coord.X;
                        }
                    }
                    vcolor /= count;
                    var newColor = modPlayer.colorInfo.color = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
                    /*var hslVec = */
                    modPlayer.hsl = Main.rgbToHsl(newColor);
                    //if (hslVec.Z < modPlayer.ConfigurationSwoosh.isLighterDecider) { modPlayer.colorInfo.color = Main.hslToRgb(hslVec with { Z = 0 }); }//MathHelper.Clamp(hslVec.Z * .25f, 0, 1)
                }
                try
                {
                    DrawSwoosh(drawPlayer);
                    //Main.NewText("!!!");
                }
                catch
                {

                }
            }
        }
        private void LegacyPlayerRenderer_DrawPlayerInternal(On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.orig_DrawPlayerInternal orig, LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float alpha, float scale, bool headOnly)
        {
            orig.Invoke(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, alpha, scale, headOnly);
            //if (drawPlayer.ShouldNotDraw)
            //    return;

            //PlayerDrawSet drawinfo = default(PlayerDrawSet);

            //var _drawData = (List<DrawData>)typeof(LegacyPlayerRenderer).GetField("_drawData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((LegacyPlayerRenderer)Main.PlayerRenderer);
            //_drawData.Clear();
            //var _dust = (List<int>)typeof(LegacyPlayerRenderer).GetField("_dust", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((LegacyPlayerRenderer)Main.PlayerRenderer);
            //_dust.Clear();
            //var _gore = (List<int>)typeof(LegacyPlayerRenderer).GetField("_gore", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((LegacyPlayerRenderer)Main.PlayerRenderer);

            //_gore.Clear();

            //if (headOnly)
            //{
            //    drawinfo.HeadOnlySetup(drawPlayer, _drawData, _dust, _gore, position.X, position.Y, alpha, scale);
            //}
            //else
            //{
            //    drawinfo.BoringSetup(drawPlayer, _drawData, _dust, _gore, position, shadow, rotation, rotationOrigin);
            //}

            //PlayerLoader.ModifyDrawInfo(ref drawinfo);


            //foreach (var layer in PlayerDrawLayerLoader.GetDrawLayers(drawinfo))
            //{
            //    if (!headOnly || layer.IsHeadLayer)
            //    {
            //        layer.DrawWithTransformationAndChildren(ref drawinfo);
            //    }
            //}

            //PlayerDrawLayers.DrawPlayer_MakeIntoFirstFractalAfterImage(ref drawinfo);

            //PlayerDrawLayers.DrawPlayer_TransformDrawData(ref drawinfo);

            //if (scale != 1f) 
            //{
            //    PlayerDrawLayers.DrawPlayer_ScaleDrawData(ref drawinfo, scale);
            //}

            // PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawinfo);
            //if (!drawinfo.drawPlayer.mount.Active || drawinfo.drawPlayer.mount.Type != 11)
            //    return;

            //for (int i = 0; i < 1000; i++)
            //{
            //    if (Main.projectile[i].active && Main.projectile[i].owner == drawinfo.drawPlayer.whoAmI && Main.projectile[i].type == 591)
            //        Main.instance.DrawProj(i);
            //}
            if (!drawPlayer.isFirstFractalAfterImage && shadow == 0f)
                DrawSwooshWithPlayer(drawPlayer);

        }

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
            if (Render != null) Render.Dispose();
            Render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth == 0 ? 1920 : Main.screenWidth, Main.screenHeight == 0 ? 1120 : Main.screenHeight);
            if (Render_AirDistort != null) Render_AirDistort.Dispose();
            Render_AirDistort = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth == 0 ? 1920 : Main.screenWidth, Main.screenHeight == 0 ? 1120 : Main.screenHeight);
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
        public RenderTarget2D Render_AirDistort
        {
            get => render_AirDistort ??= new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth == 0 ? 1920 : Main.screenWidth, Main.screenHeight == 0 ? 1120 : Main.screenHeight);
            set
            {
                render_AirDistort = value;
            }
        }

        public RenderTarget2D render_AirDistort;
        public override void Unload()
        {
            Instance = null;
        }
        public static Texture2D GetPureFractalProjTexs(int index)
        {
            if (Main.netMode == NetmodeID.Server) return CoolerItemVisualEffectMethods.GetTexture("FinalFractal");
            if (index > 21)
                return index switch
                {
                    22 => CoolerItemVisualEffectMethods.GetTexture("Tizona"),
                    23 => CoolerItemVisualEffectMethods.GetTexture("TrueTerraBlade"),
                    24 => CoolerItemVisualEffectMethods.GetTexture("PureFractal"),
                    25 => CoolerItemVisualEffectMethods.GetTexture("FinalFractal"),
                    _ => null,
                };
            int type = 0;
            switch (index)
            {
                case 0:
                    type = ItemID.CopperShortsword;
                    break;
                case 1:
                    type = ItemID.Starfury;
                    break;
                case 2:
                    type = ItemID.EnchantedSword;
                    break;
                case 3:
                    type = ItemID.BeeKeeper;
                    break;
                case 4:
                    type = ItemID.LightsBane;
                    break;
                case 5:
                    type = ItemID.BloodButcherer;
                    break;
                case 6:
                    type = ItemID.Muramasa;
                    break;
                case 7:
                    type = ItemID.BladeofGrass;
                    break;
                case 8:
                    type = ItemID.FieryGreatsword;
                    break;
                case 9:
                    type = ItemID.NightsEdge;
                    break;
                case 10:
                    type = ItemID.Excalibur;
                    break;
                case 11:
                    type = ItemID.TrueNightsEdge;
                    break;
                case 12:
                    type = ItemID.TrueExcalibur;
                    break;
                case 13:
                    type = ItemID.TerraBlade;
                    break;
                case 14:
                    type = ItemID.Seedler;
                    break;
                case 15:
                    type = ItemID.TheHorsemansBlade;
                    break;
                case 16:
                    type = ItemID.InfluxWaver;
                    break;
                case 17:
                    type = ItemID.StarWrath;
                    break;
                case 18:
                    type = ItemID.Meowmere;
                    break;
                case 19:
                    type = ItemID.Zenith;
                    break;
                case 20:
                    type = ItemID.Terragrim;
                    break;
                case 21:
                    type = ItemID.Arkhalis;
                    break;
            }
            if (TextureAssets.Item == null)
            {
                testString = "ArrayNull";
                return CoolerItemVisualEffectMethods.GetTexture("FinalFractal");
            }
            if (TextureAssets.Item[type] == null)
            {
                testString = "ValueNull";
                return CoolerItemVisualEffectMethods.GetTexture("FinalFractal");
            }
            if (!TextureAssets.Item[type].IsLoaded) TextureAssets.Item[type] = Main.Assets.Request<Texture2D>("Images/Item_" + type, ReLogic.Content.AssetRequestMode.AsyncLoad);
            return TextureAssets.Item[type].Value;
        }
        public static Texture2D GetPureFractalHeatMaps(int index)
        {
            if (PureFractalHeatMaps[index] == null) ChangePureHeatMap(index);
            return PureFractalHeatMaps[index];
        }

        public static Texture2D[] PureFractalHeatMaps = new Texture2D[26];
        public static Color[] PureFractalColors = new Color[26];
        public static float[] PureFractalAirFactorss = new float[26];

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            HandleNetwork.HandlePacket(reader, whoAmI);
            base.HandlePacket(reader, whoAmI);
        }
        public static string testString = "YEEE";
        internal static Effect ItemEffect => itemEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ItemGlowEffect").Value;
        internal static Effect ShaderSwooshEffect => shaderSwooshEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ShaderSwooshEffect").Value;
        internal static Effect ShaderSwooshEX => shaderSwooshEX ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ShaderSwooshEffectEX").Value;
        internal static Effect DistortEffect => distortEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/DistortEffect").Value;
        internal static Effect FinalFractalTailEffect => finalFractalTailEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/FinalFractalTailEffect").Value;
        internal static Effect ColorfulEffect => colorfulEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ColorfulEffect").Value;

        internal static Effect EightTrigramsFurnaceEffect => eightTrigramsFurnaceEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/EightTrigramsFurnaceEffect").Value;

        internal static Effect shaderSwooshEX;
        internal static Effect itemEffect;
        internal static Effect shaderSwooshEffect;
        internal static Effect distortEffect;
        internal static Effect finalFractalTailEffect;
        internal static Effect colorfulEffect;
        internal static Effect eightTrigramsFurnaceEffect;
        public static bool MeleeCheck(DamageClass damageClass) => damageClass == DamageClass.Melee 
            || damageClass.GetEffectInheritance(DamageClass.Melee) || !damageClass.GetModifierInheritance(DamageClass.Melee).Equals(StatInheritanceData.None);
        public static bool CanUseRender => Lighting.Mode != Terraria.Graphics.Light.LightMode.Retro && Lighting.Mode != Terraria.Graphics.Light.LightMode.Trippy && Main.WaveQuality != 0 && (byte)ConfigSwooshInstance.coolerSwooshQuality > 1;
        private void PlayerDrawLayers_DrawPlayer_27_HeldItem_WeaponDisplay(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_27_HeldItem orig, ref PlayerDrawSet drawinfo)
        {
            var drawPlayer = drawinfo.drawPlayer;
            var modPlayer = drawPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>();
            var instance = (Main.netMode == NetmodeID.SinglePlayer || drawPlayer.whoAmI == Main.myPlayer) ? ConfigSwooshInstance : modPlayer.ConfigurationSwoosh;
            Item heldItem = drawinfo.heldItem;
            bool flag = drawPlayer.itemAnimation > 0 && heldItem.useStyle != ItemUseStyleID.None;
            bool flag2 = heldItem.holdStyle != 0 && !drawPlayer.pulley;
            if (!drawPlayer.CanVisuallyHoldItem(heldItem))
            {
                flag2 = false;
            }
            bool flagMelee = true;
            //Main.NewText((drawPlayer.itemAnimation, drawPlayer.itemAnimationMax),Color.Red);

            if (heldItem.type.SpecialCheck() && drawPlayer.itemAnimation > 0 && instance.allowZenith && instance.CoolerSwooshActive)
            {
                if (!drawPlayer.isFirstFractalAfterImage)
                {
                    goto mylabel;

                }
            }// || heldItem.type == ItemID.Terragrim || heldItem.type == ItemID.Arkhalis
            flagMelee = drawPlayer.HeldItem.damage > 0 && drawPlayer.HeldItem.useStyle == ItemUseStyleID.Swing && drawPlayer.itemAnimation > 0 && modPlayer.IsMeleeBroadSword && !drawPlayer.HeldItem.noUseGraphic && instance.CoolerSwooshActive;
            if (instance.toolsNoUseNewSwooshEffect)
            {
                flagMelee = flagMelee && drawPlayer.HeldItem.axe == 0 && drawPlayer.HeldItem.hammer == 0 && drawPlayer.HeldItem.pick == 0;
            }
            bool shouldNotDrawItem = drawPlayer.frozen || !(flag || flag2) || heldItem.type <= ItemID.None || drawPlayer.dead || heldItem.noUseGraphic || drawPlayer.JustDroppedAnItem ||
                drawPlayer.wet && heldItem.noWet || drawPlayer.happyFunTorchTime && drawPlayer.inventory[drawPlayer.selectedItem].createTile == TileID.Torches && drawPlayer.itemAnimation == 0 ||
                !flagMelee;//drawinfo.shadow != 0f || 
            modPlayer.UseSlash = flagMelee;
            if (shouldNotDrawItem || !modPlayer.UseSlash)
            {
                if (!flagMelee || drawPlayer.isFirstFractalAfterImage)// || drawPlayer.isFirstFractalAfterImage
                    orig.Invoke(ref drawinfo);
                //// 如果只开启碰撞箱，显示原版挥剑效果，但是仍然要运行斩击代码来获取碰撞箱。所以不return
                //if (!ConfigGameplay.UseHitbox || shouldNotDrawItem)
                //    return;
                if (shouldNotDrawItem) return;
            }
        mylabel:
            if (drawinfo.shadow == 0f && flagMelee)
            {
                modPlayer.UseSlash = true;
                //var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
                //var w = itemTex.Width;
                //var h = itemTex.Height;
                //var cs = new Color[w * h];
                //itemTex.GetData(cs);
                //Vector4 vcolor = default;
                //float count = 0;
                //float airFactor = 1;
                //Color target = default;

                //for (int n = 0; n < cs.Length; n++)
                //{
                //    if (cs[n] != default && (n - w < 0 || cs[n - w] != default) && (n - 1 < 0 || cs[n - 1] != default) && (n + w >= cs.Length || cs[n + w] != default) && (n + 1 >= cs.Length || cs[n + 1] != default))
                //    {
                //        var weight = (float)((n + 1) % w * (h - n / w)) / w / h;
                //        vcolor += cs[n].ToVector4() * weight;
                //        count += weight;
                //    }
                //    Vector2 coord = new Vector2(n % w, n / w);
                //    coord /= new Vector2(w, h);
                //    if (instance.checkAir && Math.Abs(1 - coord.X - coord.Y) * 0.7071067811f < 0.05f && cs[n] != default && target == default)
                //    {
                //        target = cs[n];
                //        airFactor = coord.X;
                //    }
                //}
                //vcolor /= count;
                //var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
                //var hsl = Main.rgbToHsl(newColor);
                //try
                //{
                //    DrawSwoosh(drawPlayer, newColor, hsl.Z < instance.IsLighterDecider, airFactor, instance, out var rectangle);
                //    //Main.NewText("!!!");
                //}
                //catch
                //{

                //}
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
        public static void UpdateHeatMap(ref Texture2D texture, Vector3 hsl, ConfigurationSwoosh_Advanced config)
        {
            var colors = new Color[300];
            for (int i = 0; i < 300; i++)
            {
                var f = i / 299f;//分割成25次惹，f从1/25f到1//1 - 
                //f = f * f;// *f
                float h = (hsl.X + config.hueOffsetValue + config.hueOffsetRange * (2 * f - 1)) % 1;
                float s = MathHelper.Clamp(hsl.Y * config.saturationScalar, 0, 1);
                float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + config.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - config.luminosityRange), 0, 1);
                var currentColor = Main.hslToRgb(h, s, l);
                colors[i] = currentColor;
            }
            if (texture == null) texture = new Texture2D(Main.graphics.GraphicsDevice, 300, 1);
            texture.SetData(colors);
        }
        public static void WhenConfigSwooshChange()
        {
            ChangeAllPureHeatMap();
            if (Main.player != null)
                foreach (var player in Main.player)
                {
                    if (player.active)
                    {
                        var modPlr = player.GetModPlayer<CoolerItemVisualEffectPlayer>();
                        UpdateHeatMap(ref modPlr.colorInfo.tex, modPlr.hsl, modPlr.ConfigurationSwoosh);
                        modPlr.UpdateVertex();
                        modPlr.UpdateFactor();
                        if (modPlr.SwooshActive) modPlr.UpdateSwooshHM();
                    }
                }
        }
        public static void DrawSwooshContent(CoolerItemVisualEffectPlayer modPlayer, Matrix result, ConfigurationSwoosh_Advanced instance, SamplerState sampler, Texture2D itemTex, float checkAirFactor, int passCount, CustomVertexInfo[] array, bool distort = false, float scaler = 1f)
        {
            var distortScaler = distort ? instance.distortSize : 1;
            ShaderSwooshEX.Parameters["uTransform"].SetValue(result);
            ShaderSwooshEX.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
            ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir);
            ShaderSwooshEX.Parameters["airFactor"].SetValue(checkAirFactor);
            ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather && !distort);
            var _v = modPlayer.ConfigurationSwoosh.directOfHeatMap.ToRotationVector2();
            ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });
            ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
            ShaderSwooshEX.Parameters["distortScaler"].SetValue(distortScaler * scaler);
            Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + instance.ImageIndex);
            Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage($"AniTex_{modPlayer.ConfigurationSwoosh.AnimateIndex}");
            Main.graphics.GraphicsDevice.Textures[2] = itemTex;
            Main.graphics.GraphicsDevice.Textures[3] = modPlayer.colorInfo.tex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
            if (modPlayer.UseSlash)// && ((instance.swooshActionStyle != SwooshAction.向后倾一定角度后重击 && instance.swooshActionStyle != SwooshAction.两次普通斩击一次高速旋转) || modPlayer.Player.itemAnimation < 18)
            {
                ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, CreateTriList(array, modPlayer.Player.Center, distortScaler), 0, 88);
            }
            if (modPlayer.SwooshActive)
            {
                foreach (var ultraSwoosh in modPlayer.ultraSwooshes)
                {
                    if (ultraSwoosh != null && ultraSwoosh.Active)
                    {
                        ShaderSwooshEX.Parameters["airFactor"].SetValue(ultraSwoosh.checkAirFactor);
                        Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.Item[ultraSwoosh.type].Value;
                        Main.graphics.GraphicsDevice.Textures[3] = ultraSwoosh.heatMap;
                        ShaderSwooshEX.Parameters["lightShift"].SetValue(instance.IsDarkFade ? (ultraSwoosh.timeLeft / (float)ultraSwoosh.timeLeftMax) - 1f : 0);
                        ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, CreateTriList(ultraSwoosh.vertexInfos, ultraSwoosh.center, distortScaler, true), 0, 58);
                    }
                }
            }
        }
        public static CustomVertexInfo[] CreateTriList(CustomVertexInfo[] source, Vector2 center, float scaler, bool addedCenter = false)
        {
            var length = source.Length;
            CustomVertexInfo[] triangleList = new CustomVertexInfo[3 * length - 6];
            for (int i = 0; i < length - 2; i += 2)
            {
                triangleList[3 * i] = source[i];
                triangleList[3 * i + 1] = source[i + 2];
                triangleList[3 * i + 2] = source[i + 1];
                triangleList[3 * i + 3] = source[i + 1];
                triangleList[3 * i + 4] = source[i + 2];
                triangleList[3 * i + 5] = source[i + 3];
            }
            for (int n = 0; n < triangleList.Length; n++)
            {
                var vertex = triangleList[n];
                if (addedCenter)
                {
                    if (scaler != 1) vertex.Position = (vertex.Position - center) * scaler + center;
                }
                else
                {
                    if (scaler != 1) vertex.Position *= scaler;
                    vertex.Position += center;
                }
                triangleList[n] = vertex;
            }
            return triangleList;
        }
        public static void DrawSwoosh(Player drawPlayer)
        {
            if (ShaderSwooshEX == null) return;
            if (ItemEffect == null) return;
            if (DistortEffect == null) return;
            if (Main.GameViewMatrix == null) return;
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            var modPlayer = drawPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>();
            var instance = modPlayer.ConfigurationSwoosh;
            var newColor = modPlayer.colorInfo.color;
            var alphaBlend = modPlayer.hsl.Z < instance.isLighterDecider;
            var checkAirFactor = modPlayer.colorInfo.checkAirFactor;
            var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;
            var drawCen = drawPlayer.Center;
            var fac = modPlayer.FactorGeter;
            CustomVertexInfo[] c = new CustomVertexInfo[6];
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            if (modPlayer.colorInfo.tex == null || modPlayer.colorInfo.type != drawPlayer.HeldItem.type)
            {
                if (instance.swooshColorType == SwooshColorType.函数生成热度图 || instance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 || instance.swooshColorType == SwooshColorType.色调处理与对角线混合)
                {
                    UpdateHeatMap(ref modPlayer.colorInfo.tex, modPlayer.hsl, instance);
                }
                modPlayer.colorInfo.type = drawPlayer.HeldItem.type;
            }
            Matrix result = model * trans * projection;
            if (!Main.gamePaused) modPlayer.UpdateVertex();
            SamplerState sampler;
            switch (instance.swooshSampler)
            {
                default:
                case SwooshSamplerState.各向异性: sampler = SamplerState.AnisotropicWrap; break;
                case SwooshSamplerState.线性: sampler = SamplerState.LinearWrap; break;
                case SwooshSamplerState.点: sampler = SamplerState.PointWrap; break;
            }
            var (u, v) = modPlayer.vectors;

            bool useRender = (instance.distortFactor != 0 || instance.luminosityFactor != 0 || instance.maxCount > 1) && CanUseRender;
            var gd = Main.graphics.GraphicsDevice;
            var sb = Main.spriteBatch;
            var passCount = 0;
            switch (instance.swooshColorType)
            {
                case SwooshColorType.函数生成热度图: passCount = 2; break;
                case SwooshColorType.武器贴图对角线: passCount = 1; break;
                case SwooshColorType.色调处理与对角线混合: passCount = 3; break;
                case SwooshColorType.加权平均_饱和与色调处理: passCount = 4; break;

            }
            sb.End();
            if (useRender)
            {
                gd.SetRenderTarget(Instance.Render);
                gd.Clear(Color.Transparent);
            }
            sb.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
            DrawSwooshContent(modPlayer, result, instance, sampler, itemTex, checkAirFactor, passCount, modPlayer.vertexInfos, false, instance.onlyChangeSizeOfSwoosh ? modPlayer.RealSize : 1f);
            if (useRender)
            {
                if (instance.distortFactor != 0 && instance.distortSize != 1)
                {
                    sb.End();
                    if (useRender)
                    {
                        gd.SetRenderTarget(Instance.Render_AirDistort);
                        gd.Clear(Color.Transparent);
                    }
                    sb.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
                    DrawSwooshContent(modPlayer, result, instance, sampler, itemTex, checkAirFactor, passCount, modPlayer.vertexInfos, true, instance.onlyChangeSizeOfSwoosh ? modPlayer.RealSize : 1f);//
                }
                Vector2 direct = (instance.swooshFactorStyle == SwooshFactorStyle.每次开始时决定系数 ? modPlayer.kValue : ((modPlayer.kValue + modPlayer.kValueNext) * .5f)).ToRotationVector2() * -0.1f * instance.distortFactor;
                direct *= modPlayer.SwooshActive ? (modPlayer.currentSwoosh.timeLeft / (float)modPlayer.currentSwoosh.timeLeftMax) : (instance.coolerSwooshQuality == QualityType.极限ultra ? (1 - fac) : fac.SymmetricalFactor2(0.5f, 0.2f));
                switch (instance.coolerSwooshQuality)
                {
                    case QualityType.中medium:
                        for (int n = 0; n < instance.maxCount; n++)
                        {
                            if (instance.distortFactor != 0)
                            {
                                sb.End();
                                //然后在随便一个render里绘制屏幕，并把上面那个带弹幕的render传进shader里对屏幕进行处理
                                //原版自带的screenTargetSwap就是一个可以使用的render，（原版用来连续上滤镜）

                                gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
                                gd.Clear(Color.Transparent);//清空
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                                DistortEffect.Parameters["tex0"].SetValue(instance.distortSize != 1 ? Instance.Render_AirDistort : Instance.Render);//render可以当成贴图使用或者绘制。（前提是当前gd.SetRenderTarget的不是这个render，否则会报错）
                                DistortEffect.Parameters["offset"].SetValue(direct);//设置参数时间
                                DistortEffect.Parameters["invAlpha"].SetValue(0);
                                DistortEffect.CurrentTechnique.Passes[0].Apply();//ApplyPass
                                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//绘制原先屏幕内容
                                                                                      //pixelshader里处理
                            }

                            sb.End();

                            //最后在screenTarget上把刚刚的结果画上
                            gd.SetRenderTarget(Main.screenTarget);
                            gd.Clear(Color.Transparent);
                            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                            sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, alphaBlend ? 1 : 0));
                        }
                        break;
                    case QualityType.高high:
                    case QualityType.极限ultra:
                        for (int n = 0; n < instance.maxCount; n++)
                        {
                            sb.End();
                            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                            if (instance.luminosityFactor != 0)
                            {
                                gd.SetRenderTarget(Main.screenTargetSwap);
                                DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                                DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
                                DistortEffect.Parameters["position"].SetValue(new Vector2(0, 6));
                                DistortEffect.Parameters["tier2"].SetValue(instance.luminosityFactor);
                                gd.Clear(Color.Transparent);
                                DistortEffect.CurrentTechnique.Passes[7].Apply();
                                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                                gd.SetRenderTarget(Main.screenTarget);
                                gd.Clear(Color.Transparent);
                                DistortEffect.CurrentTechnique.Passes[6].Apply();
                                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
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

                            if (instance.distortFactor != 0)
                            {
                                gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
                                gd.Clear(Color.Transparent);//清空
                                //Vector2 direct = (instance.swooshFactorStyle == SwooshFactorStyle.每次开始时决定系数 ? modPlayer.kValue : ((modPlayer.kValue + modPlayer.kValueNext) * .5f)).ToRotationVector2() * -0.1f * fac.SymmetricalFactor2(0.5f, 0.2f) * instance.distortFactor;//(u + v)
                                DistortEffect.Parameters["offset"].SetValue(direct);//设置参数时间
                                DistortEffect.Parameters["invAlpha"].SetValue(0);
                                DistortEffect.Parameters["tex0"].SetValue(instance.distortSize != 1 ? Instance.Render_AirDistort : Instance.Render);
                                DistortEffect.CurrentTechnique.Passes[0].Apply();//ApplyPass
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
                            sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, alphaBlend ? 1 : 0));//
                                                                                                              //Main.instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                        }
                        break;
                }
            }


            if (modPlayer.UseSlash)
            {
                var num0 = modPlayer.negativeDir ? 1 : 0;
                float light = instance.glowLight;
                c[0] = new CustomVertexInfo(drawCen, newColor, new Vector3(0, 1, light));//因为零向量固定是左下角所以纹理固定(0,1)
                c[1] = new CustomVertexInfo(u + drawCen, newColor, new Vector3(num0 ^ 1, num0 ^ 1, light));//这一处也许有更优美的写法
                c[2] = new CustomVertexInfo(v + drawCen, newColor, new Vector3(num0, num0, light));
                c[3] = c[1];
                c[4] = new CustomVertexInfo(u + v + drawCen, newColor, new Vector3(1, 0, light));//因为u+v固定是右上角所以纹理固定(1,0)
                c[5] = c[2];
                //Main.spriteBatch.DrawLine(u + v + drawPlayer.Center, drawPlayer.Center, Color.Red);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, instance.itemAdditive ? BlendState.Additive : BlendState.AlphaBlend, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
                itemEffect.Parameters["uTransform"].SetValue(model * trans * projection);
                //将变换矩阵作用在正交投影矩阵上，具体结果以及意义我下次再想想
                //半年前就问过零群各位大佬，他们都说没必要搞懂，tr图像变换矩阵而已。
                itemEffect.Parameters["uTime"].SetValue((float)Main.time / 60 % 1);//传入时间偏移量
                itemEffect.Parameters["uItemColor"].SetValue(instance.itemHighLight ? Vector4.One : Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).ToVector4());
                //传入顶点绘制出的物品的颜色，这里采用环境光，和sb.Draw的那个color参数差不多(吧
                itemEffect.Parameters["uItemGlowColor"].SetValue(new Color(250, 250, 250, drawPlayer.HeldItem.alpha).ToVector4());

                Main.graphics.GraphicsDevice.Textures[0] = itemTex;//传入物品贴图
                Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("Style_12");//传入因时间而x纹理坐标发生偏移的灰度图，这里其实并不明显，你可以参考我mod里的无间之钟在黑暗环境下的效果
                Main.graphics.GraphicsDevice.Textures[2] = GetWeaponDisplayImage("Style_18");//传入固定叠加的灰度图
                var tex = emptyTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
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
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;

                itemEffect.CurrentTechnique.Passes[2].Apply();//这里是第三个pass，可以直接写下标不必写pass名(
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, c, 0, 2);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                modPlayer.direct = (u + v).ToRotation();
                modPlayer.HitboxPosition = (u + v) * (instance.onlyChangeSizeOfSwoosh ? instance.swooshSize : 1f);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)HandleNetwork.MessageType.rotationDirect);
                    packet.Write(modPlayer.direct);
                    packet.WritePackedVector2(modPlayer.HitboxPosition);
                    packet.Send(-1, -1);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        }
        public const string ImagePath = "CoolerItemVisualEffect/Shader/";
        public static Texture2D GetWeaponDisplayImage(string name) => ModContent.Request<Texture2D>(ImagePath + name).Value;
        #region 阿汪废弃的代码，为什么是注释而不是删掉呢？？反正下辈子都用不到的东西
        //public static void DrawSwoosh(Player drawPlayer)//, Color newColor, bool alphaBlend, float checkAirFactor, ConfigurationSwoosh_Advanced instance, out Rectangle bodyRec)
        //{
        //    //Main.NewText(checkAirFactor);
        //    //bodyRec = default;
        //    if (ShaderSwooshEX == null) return;
        //    if (ItemEffect == null) return;
        //    if (DistortEffect == null) return;
        //    if (Main.GameViewMatrix == null) return;
        //    //Main.NewText("!!!!");
        //    var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
        //    var modPlayer = drawPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>();//modplayer类
        //                                                                            //if (drawPlayer.itemAnimation == drawPlayer.itemAnimationMax - 1 && !Main.gamePaused)
        //                                                                            //{
        //                                                                            //    var vec = Main.MouseWorld - drawPlayer.Center;
        //                                                                            //    vec.Y *= drawPlayer.gravDir;
        //                                                                            //    drawPlayer.direction = Math.Sign(vec.X);
        //                                                                            //    modPlayer.negativeDir ^= true;
        //                                                                            //    modPlayer.rotationForShadow = vec.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
        //                                                                            //    modPlayer.kValue = Main.rand.NextFloat(1, 2);
        //                                                                            //}
        //    var instance = modPlayer.ConfigurationSwoosh;
        //    var newColor = modPlayer.colorInfo.color;
        //    var alphaBlend = modPlayer.hsl.Z > instance.isLighterDecider;
        //    var checkAirFactor = modPlayer.colorInfo.checkAirFactor;
        //    var fac = modPlayer.factorGeter;
        //    fac = modPlayer.negativeDir ? 1 - fac : fac;//每次挥动都会改变方向，所以插值函数方向也会一起变（原本是从1到0，反过来就是0到1(虽然说一般都是0到1



        //    //var fac = 1 - (cValue - 1) * (1 - factor) * (1 - factor) - (2 - cValue) * (1 - factor);//丢到另一个插值函数里了，可以自己画一下图像，这个插值效果比上面那个线性插值好//((float)Math.Sqrt(factor) + factor) * .5f;//(cValue - 1) * factor * factor + (2 - cValue) * factor
        //    //fac *= 3;
        //    //fac %= 1;
        //    //Main.NewText(new Vector2(fac,factor));

        //    //var drawCen = drawPlayer.gravDir == -1 ? new Vector2(drawPlayer.Center.X, (2 * (Main.screenPosition + new Vector2(960, 560)) - drawPlayer.Center - new Vector2(0, 96)).Y) : drawPlayer.Center;//2 * (Main.screenPosition + new Vector2(960, 560)) - drawPlayer.Center - new Vector2(0, 96)
        //    //var fac = (float)Math.Sqrt(factor);
        //    //var theta = (fac * -1.125f + (1 - fac) * 0.1125f) * Pi;、   adaw、
        //    var drawCen = drawPlayer.Center;
        //    float rotVel = instance.swooshActionStyle == SwooshAction.两次普通斩击一次高速旋转 && modPlayer.swingCount % 3 == 2 ? instance.rotationVelocity : 1;
        //    var theta = (1.2375f * fac * rotVel - 1.125f) * MathHelper.Pi;//线性插值后乘上一个系数，这里的起始角度和终止角度是试出来的（
        //    CustomVertexInfo[] c = new CustomVertexInfo[6];//顶点数组，绘制完整的物品需要两个三角形(六个顶点，两组重合
        //    var itemTex = TextureAssets.Item[drawPlayer.HeldItem.type].Value;//获取物品贴图
        //    float xScaler = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, fac) : modPlayer.kValue;//获取x轴方向缩放系数
        //    float scaler = itemTex.Size().Length() * drawPlayer.GetAdjustedItemScale(drawPlayer.HeldItem) / xScaler * 0.5f * instance.swooshSize * checkAirFactor;//对椭圆进行位似变换(你直接说坐标乘上一个系数不就好了吗，屑阿汪// * modPlayer.ScalerOfSword / 15f
        //    var cos = (float)Math.Cos(theta) * scaler;
        //    var sin = (float)Math.Sin(theta) * scaler;//这里(cos,sin)对应的位置就是我们希望贴图右上角所在的位置，而(0,0)对应的位置是贴图左下角所在的位置
        //    var rotator = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, fac) : modPlayer.rotationForShadow;
        //    var u = new Vector2(xScaler * (cos - sin), -cos - sin).RotatedBy(rotator);
        //    var v = new Vector2(-xScaler * (cos + sin), sin - cos).RotatedBy(rotator);//这里其实应该是都要除个二，或者上面scaler那里0.7改成0.5
        //                                                                              //此处u对应的是贴图左上角或者右下角(由方向决定,v同理。u+v就是贴图右上角(剑锋位置。因为我们希望画出来是椭圆，所以我们给x方向乘上个系数，然后在根据使用朝向进行旋转就好啦
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
        //    //假设这里丢进去是(x,y,z),出来的矩阵就是
        //    //( 1, 0, 0, 0
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


        //    //spriteBatch.End();
        //    //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

        //    List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

        //    var swooshAniFac = modPlayer.negativeDir ? 4 * fac - 3 : 4 * fac;////modPlayer.negativeDir ? 1 - (float)Math.Sqrt(1 - fac * fac) : 1 - (float)Math.Sqrt(1 - (fac - 1) * (fac - 1))
        //    swooshAniFac = MathHelper.Clamp(swooshAniFac, 0, 1);
        //    var theta3 = (1.2375f * swooshAniFac * rotVel - 1.125f) * MathHelper.Pi;//这里是又一处插值
        //    float xScaler3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.kValue, modPlayer.kValueNext, swooshAniFac) : modPlayer.kValue;
        //    var rotator3 = instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? MathHelper.Lerp(modPlayer.rotationForShadow, modPlayer.rotationForShadowNext, swooshAniFac) : modPlayer.rotationForShadow;
        //    //我们武器所在的角度是theta，我们拖尾的末端的角度就是上面的theta3，下面的theta2就是theta渐变到theta3

        //    //var cos2 = (float)Math.Cos(theta3) * scaler;
        //    //var sin2 = (float)Math.Sin(theta3) * scaler;
        //    //var u2 = new Vector2(cos2 * xScaler + sin2, -sin2 * xScaler + cos2).RotatedBy(modPlayer.rotationForShadow);
        //    //var v2 = new Vector2(cos2 * xScaler - sin2, -sin2 * xScaler - cos2).RotatedBy(modPlayer.rotationForShadow) * length;
        //    //var oldVec = u2 + v2;



        //    //Color realColor = newColor;
        //    //Vector3 hsl = Main.rgbToHsl(realColor);
        //    for (int i = 0; i < 45; i++)
        //    {
        //        var f = i / 44f;//分割成25次惹，f从1/25f到1
        //        var theta2 = f.Lerp(theta3, theta, true);//快乐线性插值
        //        var xScaler2 = (instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? f : 1).Lerp(xScaler3, xScaler, true);
        //        var rotator2 = (instance.swooshFactorStyle == SwooshFactorStyle.系数中间插值 ? f : 1).Lerp(rotator3, rotator, true);
        //        var cos2 = (float)Math.Cos(theta2) * scaler;
        //        var sin2 = (float)Math.Sin(theta2) * scaler;


        //        var u2 = new Vector2(xScaler2 * (cos2 - sin2), -cos2 - sin2).RotatedBy(rotator2);
        //        var v2 = new Vector2(-xScaler2 * (cos2 + sin2), sin2 - cos2).RotatedBy(rotator2);
        //        //和刚刚上面那里一样的流程，不要问我为什么不用一个数组存储已经算好的之前的u、v
        //        //因为那样的话如果你武器很快的话效果就很烂了（指不够平滑圆润
        //        //这种写法虽然对电脑不太友好但是效果好（x
        //        var newVec = u2 + v2;//不过这里我们只需要最后的结果(那为什么不直接(cos2 * xScaler,sin2)，阿汪你在干什么
        //        var alphaLight = alphaBlend ? Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).R / 255f * .6f : 0.6f;
        //        #region 傻宝才每帧重新寄蒜这些
        //        //if (instance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 || instance.swooshColorType == SwooshColorType.色调处理与对角线混合)
        //        //{
        //        //    float h = (hsl.X + instance.hueOffsetValue + instance.hueOffsetRange * (2 * f - 1)) % 1;
        //        //    float s = MathHelper.Clamp(hsl.Y * instance.saturationScalar, 0, 1);
        //        //    float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + instance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - instance.luminosityRange), 0, 1);
        //        //    realColor = Main.hslToRgb(h, s, l);
        //        //    //Main.NewText((h, s, l, realColor), realColor);
        //        //}
        //        #endregion

        //        var _f = f * f; ;//f;// 6 * f / (3 * f + 1)
        //        _f = MathHelper.Clamp(_f, 0, 1);
        //        //_f = ConfigurationSwoosh.instance.distortFactor != 0 ? _f : _f * _f;
        //        //realColor.A = (byte)(_f * 255);//.MultiplyRGBA(new Color(1,1,1,_f))
        //        bars.Add(new CustomVertexInfo(drawCen + newVec, newColor with { A = (byte)(_f * 255) }, new Vector3(1 - f, 1, alphaLight)));//(3 * f - 4) / (4 * f - 3)//快乐连顶点
        //        //realColor.A = 0;//.MultiplyRGBA(new Color(1, 1, 1, 0))
        //        bars.Add(new CustomVertexInfo(drawCen, newColor with { A = (byte)(_f * 255) }, new Vector3(0, 0, alphaLight)));// + newVec * (1 - f)
        //                                                                                                                       //bars.Add(new CustomVertexInfo(drawCen + newVec, alphaBlend ? new Color(realColor.R / 255f, realColor.G / 255f, realColor.B / 255f, _f) : realColor, new Vector3(1 - f, 1, alphaBlend ? alphaLight : _f)));//(3 * f - 4) / (4 * f - 3)//快乐连顶点
        //                                                                                                                       //bars.Add(new CustomVertexInfo(drawCen, alphaBlend ? new Color(realColor.R / 255f, realColor.G / 255f, realColor.B / 255f, 0) : realColor, new Vector3(0, 0, alphaBlend ? alphaLight : 0)));
        //                                                                                                                       //bars.Add(new CustomVertexInfo(drawCen + newVec, alphaBlend ? new Color(newColor.R / 255f, newColor.G / 255f, newColor.B / 255f, f) : newColor, new Vector3(1 - f, 1, alphaBlend ? alphaLight : 3 * f / (3 * f + 1))));//(3 * f - 4) / (4 * f - 3)//快乐连顶点
        //                                                                                                                       //bars.Add(new CustomVertexInfo(drawCen, alphaBlend ? new Color(newColor.R / 255f, newColor.G / 255f, newColor.B / 255f, 0) : newColor, new Vector3(0, 0, alphaBlend ? alphaLight : 0)));
        //                                                                                                                       //oldVec = newVec;
        //    }
        //    //Main.NewText(new Vector3(fac, MathHelper.Clamp(modPlayer.negativeDir ? (4 * fac - 3) : 4 * fac, 0, 1), modPlayer.negativeDir ? -1 : 1));
        //    SamplerState sampler;
        //    switch (instance.swooshSampler)
        //    {
        //        default:
        //        case SwooshSamplerState.各向异性: sampler = SamplerState.AnisotropicWrap; break;
        //        case SwooshSamplerState.线性: sampler = SamplerState.LinearWrap; break;
        //        case SwooshSamplerState.点: sampler = SamplerState.PointWrap; break;
        //    }
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


        //        //RenderTarget2D render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        //        bool useRender = (instance.distortFactor != 0 || instance.maxCount > 1) && CanUseRender;
        //        var gd = Main.graphics.GraphicsDevice;
        //        var sb = Main.spriteBatch;
        //        var passCount = 0;
        //        switch (instance.swooshColorType)
        //        {
        //            case SwooshColorType.函数生成热度图: passCount = 2; break;
        //            case SwooshColorType.武器贴图对角线: passCount = 1; break;
        //            case SwooshColorType.色调处理与对角线混合: passCount = 3; break;
        //        }
        //        if (instance.swooshColorType == SwooshColorType.函数生成热度图 || instance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 || instance.swooshColorType == SwooshColorType.加权平均_饱和与色调处理 && (modPlayer.colorInfo.tex == null || modPlayer.colorInfo.type != drawPlayer.HeldItem.type))
        //        {
        //            UpdateHeatMap(ref modPlayer.colorInfo.tex, modPlayer.hsl, instance);
        //            modPlayer.colorInfo.type = drawPlayer.HeldItem.type;
        //            //Main.graphics.GraphicsDevice.Textures[2] = colorBar;
        //            //sb.Draw(colorBar, new Vector2(240, 240), Color.White);
        //        }
        //        if (useRender)
        //        {
        //            #region MyRegion
        //            //gd.SetRenderTarget(Main.screenTargetSwap);
        //            //sp.End();

        //            //sp.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        //            //sp.Draw(Main.screenTarget, Vector2.Zero, Color.White);
        //            //sp.End();

        //            //gd.SetRenderTarget(render);
        //            //gd.Clear(Color.Transparent);


        //            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        //            //DistortEffect.Parameters["uTransform"].SetValue(model * projection);
        //            //DistortEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.06f);//-(float)Main.time * 0.06f
        //            //DistortEffect.Parameters["unit"].SetValue((u + v) * 0.005f);
        //            //Main.graphics.GraphicsDevice.Textures[0] = Main.screenTarget;
        //            //Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("BaseTex_7");
        //            //Main.graphics.GraphicsDevice.Textures[2] = GetWeaponDisplayImage("AniTex");
        //            //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
        //            //Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicWrap;
        //            //Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicWrap;
        //            //ShaderSwooshEX.CurrentTechnique.Passes[0].Apply();
        //            //var tris = new CustomVertexInfo[triangleList.Count];
        //            //for (int n = 0; n < tris.Length; n++)
        //            //{
        //            //    tris[n] = triangleList[n];
        //            //    tris[n].Color = new Vector4((tris[n].Position - Main.screenPosition) / new Vector2(1920, 1120), 0, 0).ToColor();
        //            //}
        //            //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, tris, 0, tris.Length / 3);
        //            //Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //            ////Main.spriteBatch.Draw(Main.screenTarget, new Vector2(64, 64), null, Color.White * .5f, 0, default, 1, 0, 0);
        //            //Main.spriteBatch.End();

        //            //gd.SetRenderTarget(Main.screenTarget);
        //            //sp.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        //            //sp.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //            //sp.Draw(render, Vector2.Zero, Color.White);
        //            #endregion
        //            //先在自己的render上画这个弹幕
        //            sb.End();
        //            gd.SetRenderTarget(Instance.Render);
        //            gd.Clear(Color.Transparent);
        //            sb.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
        //            ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
        //            //ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
        //            ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
        //            ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir);//-(float)Main.time * 0.06f
        //            ShaderSwooshEX.Parameters["airFactor"].SetValue(checkAirFactor);
        //            ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather);

        //            Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + instance.ImageIndex);//字面意义，base那个是不会随时间动的，ani那个会动//BaseTex//_7
        //            Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
        //            Main.graphics.GraphicsDevice.Textures[2] = itemTex;
        //            if (instance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = modPlayer.colorInfo.tex;


        //            Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
        //            Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
        //            Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
        //            Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;


        //            //if (alphaBlend) passCount += 2;
        //            ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
        //            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
        //            Main.graphics.GraphicsDevice.RasterizerState = originalState;

        //            switch (instance.coolerSwooshQuality)
        //            {
        //                case QualityType.中medium:
        //                    for (int n = 0; n < instance.maxCount; n++)
        //                    {
        //                        sb.End();
        //                        //然后在随便一个render里绘制屏幕，并把上面那个带弹幕的render传进shader里对屏幕进行处理
        //                        //原版自带的screenTargetSwap就是一个可以使用的render，（原版用来连续上滤镜）

        //                        gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
        //                        gd.Clear(Color.Transparent);//清空
        //                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        //                        DistortEffect.CurrentTechnique.Passes[0].Apply();//ApplyPass
        //                        DistortEffect.Parameters["tex0"].SetValue(Instance.Render);//render可以当成贴图使用或者绘制。（前提是当前gd.SetRenderTarget的不是这个render，否则会报错）
        //                        DistortEffect.Parameters["offset"].SetValue((u + v) * -0.002f * (1 - 2 * Math.Abs(0.5f - fac)) * instance.distortFactor);//设置参数时间
        //                        DistortEffect.Parameters["invAlpha"].SetValue(0);
        //                        sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//绘制原先屏幕内容
        //                                                                              //pixelshader里处理
        //                        sb.End();

        //                        //最后在screenTarget上把刚刚的结果画上
        //                        gd.SetRenderTarget(Main.screenTarget);
        //                        gd.Clear(Color.Transparent);
        //                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        //                        sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //                        //sb.End();

        //                        //Main.spriteBatch.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        //                        //Main.instance.GraphicsDevice.BlendState = BlendState.Additive;
        //                        sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, 0));//
        //                        //Main.instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        //                    }
        //                    break;
        //                case QualityType.高high:
        //                    for (int n = 0; n < instance.maxCount; n++)
        //                    {
        //                        sb.End();
        //                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        //                        gd.SetRenderTarget(Main.screenTargetSwap);
        //                        DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        //                        DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
        //                        DistortEffect.Parameters["position"].SetValue(new Vector2(0, 6));
        //                        DistortEffect.Parameters["tier2"].SetValue(instance.luminosityFactor);
        //                        gd.Clear(Color.Transparent);
        //                        DistortEffect.CurrentTechnique.Passes[7].Apply();
        //                        sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
        //                        gd.SetRenderTarget(Main.screenTarget);
        //                        gd.Clear(Color.Transparent);
        //                        DistortEffect.CurrentTechnique.Passes[6].Apply();
        //                        sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //                        DistortEffect.Parameters["position"].SetValue(new Vector2(0, 9));
        //                        DistortEffect.Parameters["ImageSize"].SetValue((u + v) * -0.0004f * (1 - 2 * Math.Abs(0.5f - fac)) * instance.distortFactor);
        //                        gd.SetRenderTarget(Main.screenTargetSwap);
        //                        gd.Clear(Color.Transparent);
        //                        DistortEffect.CurrentTechnique.Passes[5].Apply();
        //                        sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

        //                        gd.SetRenderTarget(Main.screenTarget);
        //                        gd.Clear(Color.Transparent);
        //                        DistortEffect.CurrentTechnique.Passes[4].Apply();
        //                        sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //                        #region 乱糟糟注释
        //                        //Main.spriteBatch.End();
        //                        //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        //                        ////Main.NewText(CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes.Count);

        //                        ////CoolerItemVisualEffect.DistortEffect.Parameters["offset"].SetValue(Rotation.ToRotationVector2() * -0.002f * useDistort);//* (1 - 2 * Math.Abs(0.5f - useDistort))
        //                        ////CoolerItemVisualEffect.DistortEffect.Parameters["invAlpha"].SetValue(0);


        //                        //CoolerItemVisualEffect.DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        //                        //CoolerItemVisualEffect.DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
        //                        //CoolerItemVisualEffect.DistortEffect.Parameters["position"].SetValue(new Vector2(0, 7));
        //                        //CoolerItemVisualEffect.DistortEffect.Parameters["tier2"].SetValue(0.4f);
        //                        //for (int n = 0; n < 3; n++)
        //                        //{
        //                        //    gd.SetRenderTarget(Main.screenTargetSwap);
        //                        //    gd.Clear(Color.Transparent);
        //                        //    CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes[7].Apply();
        //                        //    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);



        //                        //    gd.SetRenderTarget(Main.screenTarget);
        //                        //    gd.Clear(Color.Transparent);
        //                        //    CoolerItemVisualEffect.DistortEffect.CurrentTechnique.Passes[6].Apply();
        //                        //    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //                        //}
        //                        //sb.End();
        //                        //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        //                        //DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        //                        //DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
        //                        //DistortEffect.Parameters["position"].SetValue(new Vector2(0, 3));
        //                        //DistortEffect.Parameters["tier2"].SetValue(0.2f);
        //                        //for (int n = 0; n < 3; n++)
        //                        //{
        //                        //    gd.SetRenderTarget(Main.screenTargetSwap);
        //                        //    gd.Clear(Color.Transparent);
        //                        //    DistortEffect.CurrentTechnique.Passes[7].Apply();
        //                        //    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);



        //                        //    gd.SetRenderTarget(Main.screenTarget);
        //                        //    gd.Clear(Color.Transparent);
        //                        //    DistortEffect.CurrentTechnique.Passes[6].Apply();
        //                        //    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //                        //}
        //                        //DistortEffect.Parameters["position"].SetValue(new Vector2(0, 3));
        //                        //DistortEffect.Parameters["ImageSize"].SetValue((u + v) * -0.002f * (1 - 2 * Math.Abs(0.5f - fac)) * instance.distortFactor);
        //                        //for (int n = 0; n < 2; n++)
        //                        //{
        //                        //    gd.SetRenderTarget(Main.screenTargetSwap);
        //                        //    gd.Clear(Color.Transparent);
        //                        //    DistortEffect.CurrentTechnique.Passes[5].Apply();
        //                        //    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

        //                        //    gd.SetRenderTarget(Main.screenTarget);
        //                        //    gd.Clear(Color.Transparent);
        //                        //    DistortEffect.CurrentTechnique.Passes[4].Apply();
        //                        //    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //                        //}
        //                        //sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        //                        //sb.Draw(Instance.Render, Vector2.Zero, Color.White);
        //                        #endregion
        //                        sb.End();
        //                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        //                        sb.Draw(Instance.Render, Vector2.Zero, Color.White);
        //                    }
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            sb.End();
        //            sb.Begin(SpriteSortMode.Immediate, alphaBlend ? BlendState.NonPremultiplied : BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);//Main.DefaultSamplerState//Main.GameViewMatrix.TransformationMatrix
        //            ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);//
        //            //ShaderSwooshEX.Parameters["uLighter"].SetValue(instance.luminosityFactor);
        //            ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
        //            ShaderSwooshEX.Parameters["checkAir"].SetValue(instance.checkAir);
        //            ShaderSwooshEX.Parameters["airFactor"].SetValue(checkAirFactor);
        //            ShaderSwooshEX.Parameters["gather"].SetValue(instance.gather);


        //            Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + instance.ImageIndex);//字面意义，base那个是不会随时间动的，ani那个会动//BaseTex//_7
        //            Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
        //            Main.graphics.GraphicsDevice.Textures[2] = itemTex;
        //            if (instance.swooshColorType == SwooshColorType.函数生成热度图) Main.graphics.GraphicsDevice.Textures[3] = modPlayer.colorInfo.tex;
        //            //if (ConfigurationSwoosh.instance.swooshColorType == SwooshColorType.函数生成热度图) 
        //            //{
        //            //    var colorBar = new Texture2D(Main.graphics.GraphicsDevice,300,60);
        //            //    colorBar.SetData<Color>();
        //            //    Main.graphics.GraphicsDevice.Textures[3] = colorBar;
        //            //}


        //            Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
        //            Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
        //            Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
        //            Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;

        //            //var passCount = 0;
        //            //if (ConfigurationSwoosh.instance.swooshColorType == SwooshColorType.武器贴图对角线) passCount++;
        //            //if (alphaBlend) passCount += 2;
        //            ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
        //            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
        //            Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //            //ShaderSwooshEX.Parameters["uTransform"].SetValue(model * projection);
        //            //ShaderSwooshEX.Parameters["uLighter"].SetValue(ConfigurationSwoosh.instance.luminosityFactor);
        //            //ShaderSwooshEX.Parameters["uTime"].SetValue(0);//-(float)Main.time * 0.06f
        //            //Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_7");//字面意义，base那个是不会随时间动的，ani那个会动//BaseTex
        //            //Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("AniTex");
        //            //Main.graphics.GraphicsDevice.Textures[2] = itemTex;
        //            ////if (ConfigurationSwoosh.instance.swooshColorType == SwooshColorType.函数生成热度图) 
        //            ////{
        //            ////    var colorBar = new Texture2D(Main.graphics.GraphicsDevice,300,60);
        //            ////    colorBar.SetData<Color>();
        //            ////    Main.graphics.GraphicsDevice.Textures[3] = colorBar;
        //            ////}


        //            //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
        //            //Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicWrap;
        //            //Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicWrap;
        //            //Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicWrap;

        //            //var passCount = 0;
        //            //if (ConfigurationSwoosh.instance.swooshColorType == SwooshColorType.武器贴图对角线) passCount++;
        //            ////if (alphaBlend) passCount += 2;
        //            //ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
        //            //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
        //            //Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //        }

        //    }

        //    //var color = Main.hslToRgb(0.33f, 0.75f, 0.75f);
        //    //.5f
        //    var num0 = modPlayer.negativeDir ? 1 : 0;
        //    u /= checkAirFactor;
        //    v /= checkAirFactor;
        //    float light = instance.glowLight;
        //    c[0] = new CustomVertexInfo(drawCen, newColor, new Vector3(0, 1, light));//因为零向量固定是左下角所以纹理固定(0,1)
        //    c[1] = new CustomVertexInfo(u + drawCen, newColor, new Vector3(num0 ^ 1, num0 ^ 1, light));//这一处也许有更优美的写法
        //    c[2] = new CustomVertexInfo(v + drawCen, newColor, new Vector3(num0, num0, light));
        //    c[3] = c[1];
        //    c[4] = new CustomVertexInfo(u + v + drawCen, newColor, new Vector3(1, 0, light));//因为u+v固定是右上角所以纹理固定(1,0)
        //    c[5] = c[2];
        //    //Main.spriteBatch.DrawLine(u + v + drawPlayer.Center, drawPlayer.Center, Color.Red);
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, instance.itemAdditive ? BlendState.Additive : BlendState.AlphaBlend, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        //    itemEffect.Parameters["uTransform"].SetValue(model * trans * projection);
        //    //将变换矩阵作用在正交投影矩阵上，具体结果以及意义我下次再想想
        //    //半年前就问过零群各位大佬，他们都说没必要搞懂，tr图像变换矩阵而已。
        //    itemEffect.Parameters["uTime"].SetValue((float)Main.time / 60 % 1);//传入时间偏移量
        //    itemEffect.Parameters["uItemColor"].SetValue(instance.itemHighLight ? Vector4.One : Lighting.GetColor((drawPlayer.Center / 16).ToPoint().X, (drawPlayer.Center / 16).ToPoint().Y).ToVector4());
        //    //传入顶点绘制出的物品的颜色，这里采用环境光，和sb.Draw的那个color参数差不多(吧
        //    itemEffect.Parameters["uItemGlowColor"].SetValue(new Color(250, 250, 250, drawPlayer.HeldItem.alpha).ToVector4());

        //    Main.graphics.GraphicsDevice.Textures[0] = itemTex;//传入物品贴图
        //    Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage("Style_12");//传入因时间而x纹理坐标发生偏移的灰度图，这里其实并不明显，你可以参考我mod里的无间之钟在黑暗环境下的效果
        //    Main.graphics.GraphicsDevice.Textures[2] = GetWeaponDisplayImage("Style_18");//传入固定叠加的灰度图
        //    var tex = emptyTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
        //    tex.SetData(new Color[] { Color.Transparent });
        //    Main.graphics.GraphicsDevice.Textures[3] = tex;
        //    var g = drawPlayer.HeldItem.glowMask;
        //    if (g != -1)
        //    {
        //        //Main.graphics.GraphicsDevice.Textures[3] = TextureAssets.GlowMask[g].Value;
        //        Main.graphics.GraphicsDevice.Textures[3] = TextureAssets.GlowMask[g].Value;
        //    }
        //    if (drawPlayer.HeldItem.type == 3823)
        //    {
        //        //Main.graphics.GraphicsDevice.Textures[1] = TextureAssets.ItemFlame[3823].Value;
        //        Main.graphics.GraphicsDevice.Textures[3] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Shader/ItemFlame_3823").Value;

        //        //ItemEffect.Parameters["uItemGlowColor"].SetValue(new Color(100, 100, 100, 0).ToVector4());

        //    }
        //    //上面这两个灰度图叠加后作为插值的t，大概是这样的映射:t=0时最终物品上的颜色是0(黑色，additive模式下是透明的)，t=0.5时是color（顶点传入的color参数，不是上面uItemColor,t=1时是1(白色)
        //    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
        //    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
        //    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
        //    Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;

        //    itemEffect.CurrentTechnique.Passes[2].Apply();//这里是第三个pass，可以直接写下标不必写pass名(
        //    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, c, 0, 2);
        //    Main.graphics.GraphicsDevice.RasterizerState = originalState;

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
        //    //var vel = u + v;
        //    //bodyRec = drawPlayer.bodyFrame;
        //    //bodyRec.Y = 112 + 56 * (int)(Math.Abs(new Vector2(-vel.Y, vel.X).ToRotation()) / MathHelper.Pi * 3);
        //    //drawPlayer.bodyFrame.Y = 112 + 56 * (int)(Math.Abs(new Vector2(-vel.Y, vel.X).ToRotation()) / MathHelper.Pi * 3);
        //    modPlayer.direct = (u + v).ToRotation();
        //    modPlayer.HitboxPosition = u + v;
        //    if (Main.netMode == NetmodeID.MultiplayerClient)
        //    {
        //        ModPacket packet = Instance.GetPacket();
        //        packet.Write((byte)HandleNetwork.MessageType.rotationDirect);
        //        packet.Write(modPlayer.direct);
        //        packet.WritePackedVector2(modPlayer.HitboxPosition);
        //        packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
        //        //NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, drawPlayer.whoAmI); // 同步direction
        //    }
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        //}

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
    public class CoolerSystem : ModSystem
    {
        public static int ModTime;
        public override void UpdateUI(GameTime gameTime)
        {
            ModTime++;
        }
    }
    //public class MyModSystem : ModSystem
    //{
    //    public override void PostDrawInterface(SpriteBatch spriteBatch)
    //    {
    //        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, ConfigurationSwoosh.MagicConfigCounter.ToString(), new Vector2(240, 240), Main.DiscoColor);
    //    }
    //}
    //public class MyModMenu : ModMenu
    //{
    //    public override void PostDrawLogo(SpriteBatch spriteBatch, Vector2 logoDrawCenter, float logoRotation, float logoScale, Color drawColor)
    //    {
    //        base.PostDrawLogo(spriteBatch, logoDrawCenter, logoRotation, logoScale, drawColor);
    //        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, ConfigurationPreInstall.instance.preInstallSwoosh.ToString(), new Vector2(240, 240), Main.DiscoColor);
    //    }
    //}
}