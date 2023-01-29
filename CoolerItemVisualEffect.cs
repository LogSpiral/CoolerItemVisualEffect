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
using System.Linq;
using Terraria.Localization;
using Terraria.Graphics.Renderers;
using MonoMod.Cil;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using System.ComponentModel;
using MonoMod.RuntimeDetour.HookGen;

namespace CoolerItemVisualEffect
{
    //TODO List计划表
    //
    public class CoolerItemVisualEffect : Mod
    {
        #region 基本量
        internal static CoolerItemVisualEffect Instance;
        public static int ModTime => CoolerSystem.ModTime;
        public static Texture2D emptyTex;
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

        public static BlendState AllOne;
        public CoolerItemVisualEffect()
        {
            AllOne = new BlendState();
            AllOne.ColorDestinationBlend = AllOne.AlphaDestinationBlend = AllOne.ColorSourceBlend = AllOne.AlphaSourceBlend = Blend.One;
        }
        public static bool CanUseRender => Lighting.Mode != Terraria.Graphics.Light.LightMode.Retro && Lighting.Mode != Terraria.Graphics.Light.LightMode.Trippy && Main.WaveQuality != 0 && (byte)ConfigSwooshInstance.coolerSwooshQuality > 1;
        #endregion
        #region Effects
        internal static Effect itemEffect;
        internal static Effect shaderSwooshEffect;//第一代刀光effect
        internal static Effect shaderSwooshEX;//第二代
        //↑但是很不幸的是，都丢失.fx了，等阿汪做出第三代吧
        internal static Effect distortEffect;
        internal static Effect finalFractalTailEffect;
        internal static Effect colorfulEffect;
        internal static Effect eightTrigramsFurnaceEffect;
        internal static Effect ItemEffect => itemEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ItemGlowEffect").Value;
        internal static Effect ShaderSwooshEffect => shaderSwooshEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ShaderSwooshEffect").Value;
        internal static Effect ShaderSwooshEX => shaderSwooshEX ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ShaderSwooshEffectEX").Value;
        internal static Effect DistortEffect => distortEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/DistortEffect").Value;
        internal static Effect FinalFractalTailEffect => finalFractalTailEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/FinalFractalTailEffect").Value;
        internal static Effect ColorfulEffect => colorfulEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ColorfulEffect").Value;
        internal static Effect EightTrigramsFurnaceEffect => eightTrigramsFurnaceEffect ??= ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/EightTrigramsFurnaceEffect").Value;


        #endregion
        #region 更新函数
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
                    if (PureFractalHeatMaps[n] == null) PureFractalHeatMaps[n] = new Texture2D(Main.graphics.GraphicsDevice, 300, 1);
                    UpdateHeatMap(ref PureFractalHeatMaps[n], hsl, ConfigSwooshInstance, itemTex);

                    //var colors = new Color[300];
                    //for (int i = 0; i < 300; i++)
                    //{
                    //    var f = i / 299f;//分割成25次惹，f从1/25f到1//1 - 
                    //    f = f * f;// *f
                    //    float h = (hsl.X + ConfigSwooshInstance.hueOffsetValue + ConfigSwooshInstance.hueOffsetRange * (2 * f - 1)) % 1;
                    //    float s = MathHelper.Clamp(hsl.Y * ConfigSwooshInstance.saturationScalar, 0, 1);
                    //    float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + ConfigSwooshInstance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - ConfigSwooshInstance.luminosityRange), 0, 1);
                    //    colors[i] = Main.hslToRgb(h, s, l);
                    //}
                    //PureFractalHeatMaps[n].SetData(colors);
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
                if (PureFractalHeatMaps[n] == null) PureFractalHeatMaps[n] = new Texture2D(Main.graphics.GraphicsDevice, 300, 1);
                UpdateHeatMap(ref PureFractalHeatMaps[n], hsl, ConfigSwooshInstance, itemTex);
                //var colors = new Color[300];
                //for (int i = 0; i < 300; i++)
                //{
                //    var f = i / 299f;//分割成25次惹，f从1/25f到1//1 - 
                //    f = f * f;// *f
                //    float h = (hsl.X + ConfigSwooshInstance.hueOffsetValue + ConfigSwooshInstance.hueOffsetRange * (2 * f - 1)) % 1;
                //    float s = MathHelper.Clamp(hsl.Y * ConfigSwooshInstance.saturationScalar, 0, 1);
                //    float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + ConfigSwooshInstance.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - ConfigSwooshInstance.luminosityRange), 0, 1);
                //    colors[i] = Main.hslToRgb(h, s, l);
                //}

                //PureFractalHeatMaps[n].SetData(colors);
            }
            );
        }
        private static float GetHeatMapFactor(float t, int colorCount, HeatMapFactorStyle style) => style switch
        {
            HeatMapFactorStyle.线性Linear => t,
            HeatMapFactorStyle.分块Floor => (int)(t * (colorCount + 1)) / (float)colorCount,
            HeatMapFactorStyle.二次Quadratic => t * t,
            HeatMapFactorStyle.平方根SquareRoot => MathF.Sqrt(t),
            HeatMapFactorStyle.柔和分块SmoothFloor => (t * colorCount).SmoothFloor() / colorCount,
            _ => t
        };
        public static void UpdateHeatMap(ref Texture2D texture, Vector3 hsl, ConfigurationSwoosh config, Texture2D itemTexture)
        {
            var colors = new Color[300];
            ref Vector3 _color = ref hsl;
            switch (config.heatMapCreateStyle)
            {
                case HeatMapCreateStyle.贴图生成:
                    {
                        var w = itemTexture.Width;
                        var h = itemTexture.Height;
                        var cs = new Color[w * h];
                        itemTexture.GetData(cs);
                        var currentColor = new Color[5];
                        var infos = new (float? distance, int? index)[w * h];
                        for (int n = 0; n < w * h; n++)
                        {
                            var color = cs[n];
                            if (color != default)
                            {
                                infos[n] = (hsl.DistanceColor(Main.rgbToHsl(color)), n);//Main.hslToRgb(hsl).DistanceColor(color,0)
                            }
                        }
                        var (distanceMin, distanceMax, indexMin, indexMax) = (114514f, 0f, 0, 0);
                        foreach (var info in infos)
                        {
                            if (info.distance != null)
                            {
                                if (info.distance < distanceMin)
                                {
                                    distanceMin = info.distance.Value;
                                    indexMin = info.index.Value;
                                }
                                if (info.distance > distanceMax)
                                {
                                    distanceMax = info.distance.Value;
                                    indexMax = info.index.Value;
                                }
                            }
                        }
                        currentColor[4] = cs[indexMin];
                        currentColor[0] = cs[indexMax];

                        var _dis = new float[] { 114514, 114514, 114514 };
                        var _target = new float[] { distanceMax * .75f + distanceMin * .25f, distanceMax * .5f + distanceMin * .5f, distanceMax * .25f + distanceMin * .75f };
                        var _index = new int[] { -1, -1, -1 };
                        foreach (var info in infos)
                        {
                            if (info.distance != null)
                            {
                                for (int n = 0; n < 3; n++)
                                {
                                    var d = Math.Abs(info.distance.Value - _target[n]);
                                    if (d < _dis[n])
                                    {
                                        _dis[n] = d;
                                        _index[n] = info.index.Value;
                                    }
                                }
                            }
                        }
                        for (int n = 0; n < 3; n++)
                        {
                            currentColor[n + 1] = cs[_index[n]];
                        }
                        switch (config.heatMapFactorStyle)
                        {
                            case HeatMapFactorStyle.线性Linear:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        colors[n] = (n / 299f).GetLerpArrayValue(currentColor);
                                    }
                                    break;
                                }
                            case HeatMapFactorStyle.分块Floor:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        colors[n] = currentColor[n / 60];
                                    }
                                    break;
                                }
                            case HeatMapFactorStyle.二次Quadratic:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        var fac = n / 299f;
                                        fac *= fac;
                                        colors[n] = fac.GetLerpArrayValue(currentColor);
                                    }
                                    break;
                                }
                            case HeatMapFactorStyle.平方根SquareRoot:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        colors[n] = MathF.Sqrt(n / 299f).GetLerpArrayValue(currentColor);
                                    }
                                    break;
                                }
                            case HeatMapFactorStyle.柔和分块SmoothFloor:
                                {
                                    for (int n = 0; n < 300; n++)
                                    {
                                        colors[n] = ((n / 299f * 5).SmoothFloor() / 5f).GetLerpArrayValue(currentColor);
                                    }
                                    break;
                                }
                        }


                        break;
                    }
                case HeatMapCreateStyle.指定:
                    {
                        var list = config.heatMapColors;
                        int count = list.Count;
                        switch (count)
                        {
                            case 0:
                                {
                                    Main.NewText("更新热度图失败！请确保至少有一个颜色。", Color.DarkRed);
                                    Main.NewText("Failed To Update The Heat Map! Please Ensure There's At Least One Color.", Color.DarkRed);
                                    return;
                                }
                            case 1:
                                {
                                    var color = list[0];
                                    for (int i = 0; i < 300; i++)
                                    {
                                        colors[i] = color;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    var color1 = list[0];
                                    var color2 = list[1];
                                    for (int i = 0; i < 300; i++)
                                    {
                                        colors[i] = Color.Lerp(color1, color2, GetHeatMapFactor(i / 299f, 2, config.heatMapFactorStyle));
                                    }
                                    break;
                                }
                            default:
                                {
                                    var array = list.ToArray();
                                    for (int i = 0; i < 300; i++)
                                    {
                                        colors[i] = GetHeatMapFactor(i / 299f, list.Count, config.heatMapFactorStyle).GetLerpArrayValue(array);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        for (int i = 0; i < 300; i++)
                        {
                            var f = GetHeatMapFactor(i / 299f, 6, config.heatMapFactorStyle);//分割成25次惹，f从1/25f到1//1 - 
                                                                                             //f = f * f;// *f
                            float h = (hsl.X + config.hueOffsetValue + config.hueOffsetRange * (2 * f - 1)) % 1;
                            float s = MathHelper.Clamp(hsl.Y * config.saturationScalar, 0, 1);
                            float l = MathHelper.Clamp(f > 0.5f ? hsl.Z * (2 - f * 2) + (f * 2 - 1) * Math.Max(hsl.Z, 0.5f + config.luminosityRange) : f * 2 * hsl.Z + (1 - f * 2) * Math.Min(hsl.Z, 0.5f - config.luminosityRange), 0, 1);
                            while (h < 0) h++;
                            var currentColor = Main.hslToRgb(h, s, l);
                            colors[i] = currentColor;
                        }
                        break;
                    }
            }
            if (texture == null)
            {
                try
                {
                    texture = new Texture2D(Main.graphics.GraphicsDevice, 300, 1);
                }
                catch
                {
                    Texture2D texdummy = null;
                    Main.RunOnMainThread(() => { texdummy = new Texture2D(Main.graphics.GraphicsDevice, 300, 1); });
                    texture = texdummy;
                }
            }
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
                        UpdateHeatMap(ref modPlr.colorInfo.tex, modPlr.hsl, modPlr.ConfigurationSwoosh, TextureAssets.Item[player.HeldItem.type].Value);
                        modPlr.UpdateVertex();
                        modPlr.UpdateFactor();
                        if (modPlr.SwooshActive) modPlr.UpdateSwooshHM();
                    }
                }
        }
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
            if (heldItem.type.SpecialCheck() && drawPlayer.itemAnimation > 0 && instance.allowZenith && instance.CoolerSwooshActive)
            {
                if (!drawPlayer.isFirstFractalAfterImage)
                {
                    goto mylabel;

                }
            }
            flagMelee = drawPlayer.HeldItem.damage > 0 && drawPlayer.HeldItem.useStyle == ItemUseStyleID.Swing && drawPlayer.itemAnimation > 0 && modPlayer.IsMeleeBroadSword && !drawPlayer.HeldItem.noUseGraphic && instance.CoolerSwooshActive;
            if (instance.toolsNoUseNewSwooshEffect)
            {
                flagMelee = flagMelee && drawPlayer.HeldItem.axe == 0 && drawPlayer.HeldItem.hammer == 0 && drawPlayer.HeldItem.pick == 0;
            }
            bool shouldNotDrawItem = drawPlayer.frozen || !(flag || flag2) || heldItem.type <= ItemID.None || drawPlayer.dead || heldItem.noUseGraphic || drawPlayer.JustDroppedAnItem ||
                drawPlayer.wet && heldItem.noWet || drawPlayer.happyFunTorchTime && drawPlayer.inventory[drawPlayer.selectedItem].createTile == TileID.Torches && drawPlayer.itemAnimation == 0 ||
                !flagMelee;
            modPlayer.UseSlash = flagMelee;
            if (shouldNotDrawItem || !modPlayer.UseSlash)
            {
                if (!flagMelee || drawPlayer.isFirstFractalAfterImage)
                    orig.Invoke(ref drawinfo);
                if (shouldNotDrawItem) return;
            }
        mylabel:
            if (drawinfo.shadow == 0f && flagMelee)
            {
                modPlayer.UseSlash = true;
            }
        }

        #endregion
        #region 初始化
        public override void Load()
        {
            Instance = this;
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_27_HeldItem += PlayerDrawLayers_DrawPlayer_27_HeldItem_WeaponDisplay;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayerInternal += LegacyPlayerRenderer_DrawPlayerInternal;
            On.Terraria.Graphics.Effects.FilterManager.EndCapture += FilterManager_EndCapture_CoolerSwoosh;
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles_CoolerSwoosh;
            Filters.Scene["CoolerItemVisualEffect:InvertGlass"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("CoolerItemVisualEffect/Shader/ZenithGlass", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "Zenith"), EffectPriority.Medium);
            Filters.Scene["CoolerItemVisualEffect:InvertGlass"].Load();
        }
        public override void Unload()
        {
            //HookEndpointManager.Remove<hook_UnderworldLayer>(targetMethod, MyUnderLayer);
            Instance = null;
        }
        #endregion
        #region 绘制

        #region 弹幕部分
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
                            if (ConfigSwooshInstance.swooshColorType == SwooshColorType.单向渐变 || ConfigSwooshInstance.swooshColorType == SwooshColorType.单向渐变与对角线混合)
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
                switch (ConfigurationSwoosh.ConfigSwooshInstance.swooshColorType)
                {
                    case SwooshColorType.热度图:
                        {
                            sampler = SamplerState.AnisotropicClamp;
                            break;
                        }
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
                        case SwooshColorType.热度图: passCount = 2; break;
                        case SwooshColorType.武器贴图对角线: passCount = 1; break;
                        case SwooshColorType.单向渐变与对角线混合: passCount = 3; break;
                        case SwooshColorType.单向渐变: passCount = 4; break;
                    }
                    ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                    //ShaderSwooshEX.Parameters["uLighter"].SetValue(ConfigSwooshInstance.luminosityFactor);
                    ShaderSwooshEX.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
                    ShaderSwooshEX.Parameters["checkAir"].SetValue(ConfigSwooshInstance.checkAir);
                    ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
                    ShaderSwooshEX.Parameters["gather"].SetValue(ConfigSwooshInstance.gather);
                    ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
                    ShaderSwooshEX.Parameters["distortScaler"].SetValue(0);
                    var _v = ConfigSwooshInstance.directOfHeatMap.ToRotationVector2();
                    ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });
                    ShaderSwooshEX.Parameters["alphaFactor"].SetValue(ConfigSwooshInstance.alphaFactor);
                    ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(ConfigSwooshInstance.alphaFactor == 0);
                    Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + ConfigSwooshInstance.ImageIndex);
                    Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage($"AniTex_{ConfigSwooshInstance.AnimateIndex}");
                    Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Weapons/FirstZenithProj_5").Value;
                    Main.graphics.GraphicsDevice.Textures[3] = Main.LocalPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>().colorInfo.tex;
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
        private void FilterManager_EndCapture_CoolerSwoosh(On.Terraria.Graphics.Effects.FilterManager.orig_EndCapture orig, Terraria.Graphics.Effects.FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
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
                SpriteBatch spriteBatch = Main.spriteBatch;
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
                            if (ConfigSwooshInstance.swooshColorType == SwooshColorType.单向渐变 || ConfigSwooshInstance.swooshColorType == SwooshColorType.单向渐变与对角线混合)
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
                        case SwooshColorType.热度图: passCount = 2; break;
                        case SwooshColorType.武器贴图对角线: passCount = 1; break;
                        case SwooshColorType.单向渐变与对角线混合: passCount = 3; break;
                        case SwooshColorType.单向渐变: passCount = 4; break;
                    }
                    ShaderSwooshEX.Parameters["uTransform"].SetValue(model * trans * projection);
                    //ShaderSwooshEX.Parameters["uLighter"].SetValue(ConfigSwooshInstance.luminosityFactor);
                    ShaderSwooshEX.Parameters["uTime"].SetValue(-CoolerSystem.ModTime * 0.03f);
                    ShaderSwooshEX.Parameters["checkAir"].SetValue(ConfigSwooshInstance.checkAir);
                    ShaderSwooshEX.Parameters["airFactor"].SetValue(1);
                    ShaderSwooshEX.Parameters["gather"].SetValue(ConfigSwooshInstance.gather);
                    ShaderSwooshEX.Parameters["alphaFactor"].SetValue(ConfigSwooshInstance.alphaFactor);
                    ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(ConfigSwooshInstance.alphaFactor == 0);
                    var _v = ConfigSwooshInstance.directOfHeatMap.ToRotationVector2();
                    ShaderSwooshEX.Parameters["heatRotation"].SetValue(Matrix.Identity with { M11 = _v.X, M12 = -_v.Y, M21 = _v.Y, M22 = _v.X });

                    ShaderSwooshEX.Parameters["lightShift"].SetValue(0);
                    ShaderSwooshEX.Parameters["distortScaler"].SetValue(0);

                    Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + ConfigSwooshInstance.ImageIndex);
                    Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage($"AniTex_{ConfigSwooshInstance.AnimateIndex}");
                    Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CoolerItemVisualEffect/Weapons/FirstZenithProj_5").Value;
                    Main.graphics.GraphicsDevice.Textures[3] = Main.LocalPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>().colorInfo.tex;
                    Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
                    switch (ConfigurationSwoosh.ConfigSwooshInstance.swooshColorType)
                    {
                        case SwooshColorType.热度图:
                            {
                                sampler = SamplerState.AnisotropicClamp;
                                break;
                            }
                    }
                    Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
                    ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleList.ToArray(), 0, _triangleList.Count / 3);
                    sb.End();

                    if (disFlag)
                    {
                        gd.SetRenderTarget(Instance.Render_AirDistort);
                        gd.Clear(Color.Transparent);
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                        ShaderSwooshEX.Parameters["distortScaler"].SetValue(ConfigSwooshInstance.distortSize);
                        ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
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
                                    DistortEffect.Parameters["offset"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                                    DistortEffect.Parameters["tex0"].SetValue(Instance.Render);
                                    DistortEffect.Parameters["position"].SetValue(new Vector2(0, 6));
                                    DistortEffect.Parameters["tier2"].SetValue(ConfigSwooshInstance.luminosityFactor);
                                    gd.Clear(Color.Transparent);
                                    DistortEffect.CurrentTechnique.Passes[7].Apply();
                                    sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                                    gd.SetRenderTarget(Main.screenTarget);
                                    gd.Clear(Color.Transparent);
                                    DistortEffect.CurrentTechnique.Passes[6].Apply();
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
                                    DistortEffect.Parameters["offset"].SetValue(new Vector2(0.707f) * -0.09f * ConfigSwooshInstance.distortFactor);//设置参数时间
                                    DistortEffect.Parameters["invAlpha"].SetValue(0);
                                    DistortEffect.Parameters["tex0"].SetValue(ConfigSwooshInstance.distortSize != 1 ? Instance.Render_AirDistort : Instance.Render);
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
            //mylabel:

            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
        #endregion

        #region 玩家部分
        /// <summary>
        /// 实际绘制所有应该绘制的刀光内容的函数
        /// </summary>
        /// <param name="modPlayer"></param>
        /// <param name="result">世界变换矩阵</param>
        /// <param name="instance"></param>
        /// <param name="sampler">采样模式</param>
        /// <param name="itemTex">物品贴图</param>
        /// <param name="checkAirFactor">是否检测空心</param>
        /// <param name="passCount">采用第几号通道</param>
        /// <param name="array">顶点</param>
        /// <param name="distort">是否为了空气扭曲绘制</param>
        /// <param name="scaler">将大小再插值回来</param>
        public static void DrawSwooshContent(CoolerItemVisualEffectPlayer modPlayer, Matrix result, ConfigurationSwoosh instance, SamplerState sampler, Texture2D itemTex, float checkAirFactor, int passCount, CustomVertexInfo[] array, bool distort = false, float scaler = 1f)
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
            ShaderSwooshEX.Parameters["alphaFactor"].SetValue(instance.alphaFactor);
            ShaderSwooshEX.Parameters["heatMapAlpha"].SetValue(instance.alphaFactor == 0);
            Main.graphics.GraphicsDevice.Textures[0] = GetWeaponDisplayImage("BaseTex_" + instance.ImageIndex);
            Main.graphics.GraphicsDevice.Textures[1] = GetWeaponDisplayImage($"AniTex_{modPlayer.ConfigurationSwoosh.AnimateIndex}");
            Main.graphics.GraphicsDevice.Textures[2] = itemTex;
            Main.graphics.GraphicsDevice.Textures[3] = modPlayer.colorInfo.tex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[1] = sampler;
            Main.graphics.GraphicsDevice.SamplerStates[2] = sampler;
            switch (instance.swooshColorType)
            {
                case SwooshColorType.热度图:
                    {
                        sampler = SamplerState.AnisotropicClamp;
                        break;
                    }
            }
            Main.graphics.GraphicsDevice.SamplerStates[3] = sampler;
            if (modPlayer.UseSlash)// && ((instance.swooshActionStyle != SwooshAction.向后倾一定角度后重击 && instance.swooshActionStyle != SwooshAction.两次普通斩击一次高速旋转) || modPlayer.Player.itemAnimation < 18)
            {
                ShaderSwooshEX.CurrentTechnique.Passes[passCount].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, CreateTriList(array, modPlayer.Player.Center, distortScaler), 0, 88);
            }
            if (modPlayer.SwooshActive)
            {
                //foreach (var ultraSwoosh in modPlayer.ultraSwooshes)
                //{
                //    if (ultraSwoosh != null && ultraSwoosh.Active)
                //    {
                //        Main.spriteBatch.Draw(ultraSwoosh.heatMap, ultraSwoosh.center - Main.screenPosition, null, Color.White, 0, default, 1, 0, 0);
                //        Utils.DrawBorderString(Main.spriteBatch, ultraSwoosh.timeLeft.ToString(), ultraSwoosh.center - Main.screenPosition, Color.Red);
                //        var list = CreateTriList(ultraSwoosh.vertexInfos, ultraSwoosh.center, distortScaler, true);
                //        for (int n = 0; n < list.Length - 1; n++) 
                //        {
                //            Main.spriteBatch.DrawLine(list[n].Position, list[n + 1].Position, Color.Red, 2, false, -Main.screenPosition);
                //        }
                //    }
                //}
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
        /// <summary>
        /// 通过玩家实例绘制刀光
        /// </summary>
        /// <param name="drawPlayer"></param>
        public static void DrawSwoosh(Player drawPlayer)
        {
            if (ShaderSwooshEX == null) return;
            if (ItemEffect == null) return;
            if (DistortEffect == null) return;
            if (Main.GameViewMatrix == null) return;

            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            var modPlayer = drawPlayer.GetModPlayer<CoolerItemVisualEffectPlayer>();
            //Main.NewText((drawPlayer.itemAnimation, drawPlayer.itemAnimationMax, drawPlayer.itemAnimation / (float)drawPlayer.itemAnimationMax));
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
                if (instance.swooshColorType == SwooshColorType.热度图 ||
                    instance.swooshColorType == SwooshColorType.单向渐变 ||
                    instance.swooshColorType == SwooshColorType.单向渐变与对角线混合)
                {
                    UpdateHeatMap(ref modPlayer.colorInfo.tex, modPlayer.hsl, instance, itemTex);
                }
                modPlayer.colorInfo.type = drawPlayer.HeldItem.type;
            }
            Matrix result = model * trans * projection;
            //if (!Main.gamePaused) modPlayer.UpdateVertex();
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
                itemEffect.Parameters["uTime"].SetValue(CoolerSystem.ModTime / 60f % 1);//传入时间偏移量
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
            var passCount = 0;
            switch (instance.swooshColorType)
            {
                case SwooshColorType.热度图: passCount = 2; break;
                case SwooshColorType.武器贴图对角线: passCount = 1; break;
                case SwooshColorType.单向渐变与对角线混合: passCount = 3; break;
                case SwooshColorType.单向渐变: passCount = 4; break;

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
                Vector2 direct = (instance.swooshFactorStyle == SwooshFactorStyle.每次开始时决定系数 ? modPlayer.rotationForShadow : ((modPlayer.rotationForShadow + modPlayer.rotationForShadowNext) * .5f)).ToRotationVector2() * -0.1f * instance.distortFactor;
                direct *= modPlayer.SwooshActive ? (modPlayer.currentSwoosh.timeLeft / (float)modPlayer.currentSwoosh.timeLeftMax) : (instance.coolerSwooshQuality == QualityType.极限ultra ? (1 - fac) : fac.SymmetricalFactor2(0.5f, 0.2f));
                switch (instance.coolerSwooshQuality)
                {
                    case QualityType.中medium:
                        for (int n = 0; n < instance.maxCount; n++)
                        {

                            sb.End();
                            //然后在随便一个render里绘制屏幕，并把上面那个带弹幕的render传进shader里对屏幕进行处理
                            //原版自带的screenTargetSwap就是一个可以使用的render，（原版用来连续上滤镜）

                            gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
                            gd.Clear(Color.Transparent);//清空
                            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                            if (instance.distortFactor != 0)
                            {
                                DistortEffect.Parameters["tex0"].SetValue(instance.distortSize != 1 ? Instance.Render_AirDistort : Instance.Render);//render可以当成贴图使用或者绘制。（前提是当前gd.SetRenderTarget的不是这个render，否则会报错）
                                DistortEffect.Parameters["offset"].SetValue(direct);//设置参数时间
                                DistortEffect.Parameters["invAlpha"].SetValue(0);
                                DistortEffect.CurrentTechnique.Passes[0].Apply();//ApplyPass
                            }

                            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//绘制原先屏幕内容
                                                                                  //pixelshader里处理

                            sb.End();
                            //if (instance.distortFactor != 0) 
                            //{
                            //最后在screenTarget上把刚刚的结果画上
                            gd.SetRenderTarget(Main.screenTarget);
                            gd.Clear(Color.Transparent);
                            //}

                            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                            sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, alphaBlend ? 1 : 0));
                        }
                        break;
                    case QualityType.高high:
                    case QualityType.极限ultra:
                        //Mark2
                        for (int n = 0; n < instance.maxCount; n++)
                        {
                            if (n == 0)
                            {
                                sb.End();
                                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                            }
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

                            if (instance.distortFactor == 0 && instance.luminosityFactor == 0 && n == 0)
                            {
                                gd.SetRenderTarget(Main.screenTargetSwap);//将画布设置为这个
                                gd.Clear(Color.Transparent);
                                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                                gd.SetRenderTarget(Main.screenTarget);//将画布设置为这个
                                gd.Clear(Color.Transparent);
                                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                            }
                            sb.Draw(Instance.Render, Vector2.Zero, new Color(1f, 1f, 1f, alphaBlend ? 1 : 0));//
                                                                                                              //Main.instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                        }
                        break;
                }
            }




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, trans);
        }
        /// <summary>
        /// 加入一定准备工作
        /// </summary>
        /// <param name="drawPlayer"></param>
        internal static void DrawSwooshWithPlayer(Player drawPlayer)
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
        /// <summary>
        /// 最后在这里画上
        /// </summary>
        private void LegacyPlayerRenderer_DrawPlayerInternal(On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.orig_DrawPlayerInternal orig, LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float alpha, float scale, bool headOnly)
        {
            orig.Invoke(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, alpha, scale, headOnly);
            if (!drawPlayer.isFirstFractalAfterImage && shadow == 0f && !headOnly)
                DrawSwooshWithPlayer(drawPlayer);

        }
        #endregion

        #endregion
        #region 辅助函数
        public const string ImagePath = "CoolerItemVisualEffect/Shader/";
        public static Texture2D GetWeaponDisplayImage(string name) => ModContent.Request<Texture2D>(ImagePath + name).Value;
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
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            HandleNetwork.HandlePacket(reader, whoAmI);
            base.HandlePacket(reader, whoAmI);
        }
        #region 纯粹分形
        public static Texture2D[] PureFractalHeatMaps = new Texture2D[26];
        public static Color[] PureFractalColors = new Color[26];
        public static float[] PureFractalAirFactorss = new float[26];
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
                return CoolerItemVisualEffectMethods.GetTexture("FinalFractal");
            }
            if (TextureAssets.Item[type] == null)
            {
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
        #endregion
        public static bool MeleeCheck(DamageClass damageClass) => damageClass == DamageClass.Melee
    || damageClass.GetEffectInheritance(DamageClass.Melee) || !damageClass.GetModifierInheritance(DamageClass.Melee).Equals(StatInheritanceData.None);
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

        #endregion
    }
    public class CoolerSystem : ModSystem
    {
        public static int ModTime;
        public override void UpdateUI(GameTime gameTime)
        {
            ModTime++;
            //Main.NewText(Filters.Scene["CoolerItemVisualEffect:InvertGlass"].GetShader().CombinedOpacity);
        }
        public static bool UseInvertGlass;
        public override void PreUpdateEntities()
        {
            //UseInvertGlass = true;
            ControlScreenShader("CoolerItemVisualEffect:InvertGlass", UseInvertGlass);
        }
        private void ControlScreenShader(string name, bool state)
        {
            if (!Filters.Scene[name].IsActive() && state)
            {
                Filters.Scene.Activate(name);
            }
            if (Filters.Scene[name].IsActive() && !state)
            {
                Filters.Scene.Deactivate(name);
            }
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            //取消注释看好康的
            //var list = new List<VertexTriangle>();
            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, 1920*4, 1120*4), Color.Black);
            //var cen = new Vector2(500, 1200) + Main.screenPosition;
            //new VertexTriangle(cen, new Vector2(1080, 0) + cen, new Vector2(7.3f, -3.27f) * 216 + cen).GenerateFractal(list, MathHelper.Clamp((ModTime / 1200f).UpAndDown() * 7 - 1,0,5));//new Vector2(540, 540 * 1.732f)
            //var cs = new CustomVertexInfo[list.Count * 3];
            //for (int n = 0; n < list.Count; n++)
            //{
            //    cs[3 * n] = new CustomVertexInfo(list[n].position0, new Vector3(0, 0, 1));
            //    cs[3 * n + 1] = new CustomVertexInfo(list[n].position1, new Vector3(1, 0, 1));
            //    cs[3 * n + 2] = new CustomVertexInfo(list[n].position2, new Vector3(0, 1, 1));
            //}
            ////for (int i = 0; i < cs.Length - 1; i++)
            ////{
            ////    spriteBatch.DrawLine(cs[i].Position, cs[i + 1].Position, Color.Red, 2, false, -Main.screenPosition);
            ////}
            ////spriteBatch.DrawLine(new Vector2(0, 160), new Vector2(960, 960), Color.Red);
            //Effect effect = CoolerItemVisualEffect.ShaderSwooshEffect;
            //if (effect == null) return;

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            //RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            //RasterizerState rasterizerState = new RasterizerState
            //{
            //    CullMode = CullMode.None
            //};
            //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

            //var offset = new Vector2(10, 10);
            //var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            //var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));

            //effect.Parameters["uTransform"].SetValue(model * projection);
            //effect.Parameters["uTime"].SetValue(0);
            //Main.graphics.GraphicsDevice.Textures[0] = CoolerItemVisualEffectMethods.GetTexture("BaseTex_8");
            //Main.graphics.GraphicsDevice.Textures[1] = CoolerItemVisualEffectMethods.GetTexture("BaseTex_8");
            //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            //Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            //effect.CurrentTechnique.Passes[0].Apply();

            //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, cs, 0, list.Count);
            //Main.graphics.GraphicsDevice.RasterizerState = originalState;

            //spriteBatch.End();
            //spriteBatch.Begin();
        }
    }
    public class ReturnBag : ModItem
    {
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Item.ShaderItemEffectInventory(spriteBatch, position, origin, CoolerItemVisualEffectMethods.GetTexture("ItemEffectTex_0"), Color.Lerp(new Color(0, 162, 232), new Color(34, 177, 76), (float)Math.Sin(MathHelper.Pi / 60 * CoolerSystem.ModTime) / 2 + 0.5f), scale);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.ShaderItemEffectInWorld(spriteBatch, CoolerItemVisualEffectMethods.GetTexture("ItemEffectTex_0"), Color.Lerp(new Color(0, 162, 232), new Color(34, 177, 76), (float)Math.Sin(MathHelper.Pi / 60 * CoolerSystem.ModTime) / 2 + 0.5f), rotation);
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.value = 1;
            Item.rare = 11;
            Item.maxStack = 114514;
        }
        public virtual string ReturnName => "我不知道啊啊啊啊啊啊";
        //WitheredWoodSword:朽木之刃
        //LivingWoodSword:灵木之刃
        //SereStoneSword:枯石之刃
        //MossStoneSword:苔石之刃
        //RustySteelBlade:锈钢之刃
        //RefinedSteelBlade:精钢之刃
        //PureFractal:纯粹分形
        //FirstZenith:初源峰巅
        //FinalFractal:最终分形
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("返还袋ReturnBag");
            Tooltip.SetDefault("");
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var time = ((float)Math.Sin(CoolerSystem.ModTime / 60f * MathHelper.TwoPi) + 1) * .5f;
            Color color;
            if (time < 0.5f) color = Color.Lerp(Color.Cyan, Color.Green, time * 2f);
            else color = Color.Lerp(Color.Green, Color.Yellow, time * 2f - 1);

            var str = Language.GetTextValue("Mods.CoolerItemVisualEffect.ItemName." + ReturnName);

            tooltips.Add(new TooltipLine(Mod, "YEEEEE", $"反还{str}的材料\nreturn the material of {str}") { OverrideColor = color });
        }
        public override string Texture => "Terraria/Images/Item_" + ItemID.CultistBossBag;
        public override bool CanRightClick()
        {
            return true;
        }
        public override void RightClick(Player player)
        {
            for (int n = 0; n < 500; n++)
            {
                var fac = n / 500f;
                var color = Main.hslToRgb(fac, 1f, .75f);
                fac *= MathHelper.TwoPi;
                var position = (fac * 3).ToRotationVector2() * (MathF.Sin(5 * fac) - .5f);
                Dust dust = Dust.NewDustPerfect(player.Center + position * 256, 278, new Vector2(-position.Y, position.X), 100, color, 1f);
                dust.scale = 0.4f + Main.rand.NextFloat(-1, 1) * 0.1f;
                dust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.3f;
                dust.fadeIn *= .5f;
                dust.noGravity = true;
                dust.velocity *= (3f + Main.rand.NextFloat() * 4f) * 2;
            }
        }
    }
    public class WWBag : ReturnBag
    {
        public override string ReturnName => "WitheredWoodSword";
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.WoodenSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.BorealWoodSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.PalmWoodSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.RichMahoganySword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.ShadewoodSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.PearlwoodSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.CactusSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.Mushroom, 50);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.GlowingMushroom, 50);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.Acorn, 50);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.BambooBlock, 15);
            base.RightClick(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Weapons.WitheredWoodSword_Old>()).Register();
        }
    }
    public class LWBag : ReturnBag
    {
        public override string ReturnName => "LivingWoodSword";
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.BrokenHeroSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ModContent.ItemType<Weapons.WitheredWoodSword_Old>());
            base.RightClick(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Weapons.LivingWoodSword_Old>()).Register();
        }
    }
    public class SSBag : ReturnBag
    {
        public override string ReturnName => "SereStoneSword";
        public override void RightClick(Player player)
        {
            for (int n = 0; n < 6; n++)
                player.QuickSpawnItem(Item.GetSource_GiftOrReward(), 3764 + n);//六种晶光刃
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.OrangePhasesaber);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.BoneSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.AntlionClaw);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.BeamSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.PurpleClubberfish);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.Bladetongue);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.StoneBlock, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.EbonstoneBlock, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.CrimstoneBlock, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.PearlstoneBlock, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.Sandstone, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.CorruptSandstone, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.CrimsonSandstone, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.HallowSandstone, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.Granite, 500);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.Obsidian, 50);
            base.RightClick(player);

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Weapons.LivingWoodSword_Old>()).Register();
        }
    }
    public class MSBag : ReturnBag
    {
        public override string ReturnName => "MossStoneSword";
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.BrokenHeroSword);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ModContent.ItemType<Weapons.SereStoneSword_Old>());
            base.RightClick(player);

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Weapons.MossStoneSword_Old>()).Register();
        }
    }
    public class RSBag : ReturnBag
    {
        public override string ReturnName => "RustySteelBlade";
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(),
ItemID.CopperBroadsword,
ItemID.TinBroadsword,
ItemID.IronBroadsword,
ItemID.LeadBroadsword,
ItemID.SilverBroadsword,
ItemID.TungstenBroadsword,
ItemID.GoldBroadsword,
ItemID.PlatinumBroadsword,
ItemID.Gladius,
ItemID.Katana,
ItemID.DyeTradersScimitar,
ItemID.FalconBlade,
ItemID.CobaltSword,
ItemID.PalladiumSword,
ItemID.MythrilSword,
ItemID.OrichalcumSword,
ItemID.BreakerBlade,
ItemID.Cutlass,
ItemID.AdamantiteSword,
ItemID.TitaniumSword,
ItemID.ChlorophyteSaber,
ItemID.ChlorophyteClaymore);
            base.RightClick(player);

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Weapons.RustySteelBlade_Old>()).Register();
        }
    }
    public class RSEXBag : ReturnBag
    {
        public override string ReturnName => "RefinedSteelBlade";
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.BrokenHeroSword, 3);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ModContent.ItemType<Weapons.RustySteelBlade_Old>());
            base.RightClick(player);

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Weapons.RefinedSteelBlade_Old>()).Register();
        }
    }
    public class PFBag : ReturnBag
    {
        public override string ReturnName => "PureFractal";
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.Zenith);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ModContent.ItemType<Weapons.FirstFractal_CIVE>());
            base.RightClick(player);

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Weapons.PureFractal_Old>()).Register();
        }
    }
    public class FZBag : ReturnBag
    {
        public override string ReturnName => "FirstZenith";
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ItemID.Zenith);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ModContent.ItemType<Weapons.FirstFractal_CIVE>());
            base.RightClick(player);

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Weapons.FirstZenith_Old>()).Register();
        }
    }
    public class FFBag : ReturnBag
    {
        public override string ReturnName => "FinalFractal";
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), 4144);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), 3368);
            player.QuickSpawnItem(Item.GetSource_GiftOrReward(), ModContent.ItemType<Weapons.FirstZenith_Old>());
            base.RightClick(player);

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<FinalFractal.FinalFractal_Old>()).Register();
        }
    }
}