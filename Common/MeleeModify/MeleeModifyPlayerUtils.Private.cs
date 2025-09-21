using CoolerItemVisualEffect.Common.Config;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static CoolerItemVisualEffect.Common.Config.MeleeConfig;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public static partial class MeleeModifyPlayerUtils
{
    private static HashSet<int> VanillaSlashItems { get; }
    = [
        ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.TheHorsemansBlade,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            ItemID.TerraBlade
        ];

    private static HashSet<int> VanillaToolItems { get; }
        = [
            ItemID.GravediggerShovel,
            ItemID.Sickle,
            ItemID.BreathingReed,
            ItemID.StaffofRegrowth
            ];
    private static bool MeleeClassCheck(DamageClass damageClass) =>
    damageClass == DamageClass.Melee
    || damageClass.GetEffectInheritance(DamageClass.Melee)
    || !damageClass.GetModifierInheritance(DamageClass.Melee).Equals(StatInheritanceData.None);

    private static float GetHeatMapFactor(float t, int colorCount, HeatMapFactorStyle style) => style switch
    {
        HeatMapFactorStyle.Linear => t,
        HeatMapFactorStyle.Floor => (int)(t * (colorCount + 1)) / (float)colorCount,
        HeatMapFactorStyle.Quadratic => t * t,
        HeatMapFactorStyle.SquareRoot => MathF.Sqrt(t),
        HeatMapFactorStyle.SmoothFloor => (t * colorCount).SmoothFloor() / colorCount,
        _ => t
    };

    private static void FillHeatMap(Color[] colors, Vector3 hsl, Texture2D itemTexture, MeleeConfig config)
    {
        var length  = colors.Length;
        var mLength = length - 1f;
        switch (config.heatMapCreateStyle)
        {
            case HeatMapCreateStyle.FromTexturePixel:
                {
                    FromItemTexture(colors, hsl, config, itemTexture);
                    break;
                }
            case HeatMapCreateStyle.Designate:
                {
                    config.designateData.PreGetValue();
                    for (var i = 0; i < length; i++)
                        colors[i] = config.designateData.GetValue(GetHeatMapFactor(i / mLength, 6, config.heatMapFactorStyle));
                    break;
                }
            case HeatMapCreateStyle.CosineGenerate_RGB:
                {
                    for (var i = 0; i < length; i++)
                        colors[i] = config.rgbData.GetValue(GetHeatMapFactor(i / mLength, 6, config.heatMapFactorStyle));
                    break;
                }
            case HeatMapCreateStyle.CosineGenerate_HSL:
                {
                    for (var i = 0; i < length; i++)
                        colors[i] = config.hslData.GetValue(GetHeatMapFactor(i / mLength, 6, config.heatMapFactorStyle));
                    break;
                }
            case HeatMapCreateStyle.ByFunction:
            default:
                {
                    for (var i = 0; i < length; i++)
                        colors[i] = config.byFuncData.GetColor(GetHeatMapFactor(i / mLength, 6, config.heatMapFactorStyle), hsl);
                    break;
                }
        }
    }


    public static void FromItemTexture(Color[] colors, Vector3 hsl, MeleeConfig config, Texture2D itemTexture)
    {
        var w = itemTexture.Width;
        var h = itemTexture.Height;
        var cs = new Color[w * h];
        itemTexture.GetData(cs);
        var currentColor = new Color[5];
        var infos = new (float? distance, int? index)[w * h];
        for (var n = 0; n < w * h; n++)
        {
            var color = cs[n];
            if (color != default)
            {
                infos[n] = (hsl.DistanceColor(Main.rgbToHsl(color)), n);
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
                for (var n = 0; n < 3; n++)
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
        for (var n = 0; n < 3; n++)
        {
            currentColor[n + 1] = cs[_index[n]];
        }
        var length = colors.Length;
        var mLength = length - 1f;
        for (var n = 0; n < length; n++)
            colors[n] = GetHeatMapFactor(n / mLength, 6, config.heatMapFactorStyle).ArrayLerp(currentColor);
    }
}
