using System;
using System.Collections.Generic;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable InconsistentNaming

namespace CoolerItemVisualEffect.Common.Config.Data.CosineGenerateHeatMapData;

public class CosineGenerateHeatMapData_HSL : ICosineData
{
    IReadOnlyList<CosineInfo> ICosineData.Cosines => [Hue, Saturation, Luminosity];

    Func<float, CosineInfo, Color>[] ICosineData.LineColorMethods { get; } =
    [
        (t,info)=>Main.hslToRgb(new Vector3((info.GetValue(t) % 1 + 1) % 1,1.0f,0.5f)),
        (t,info)=>Main.hslToRgb(new Vector3(Main.GlobalTimeWrappedHourly % 1,MathHelper.Clamp(info.GetValue(t),0,1),0.5f)),
        (t,info)=>Main.hslToRgb(new Vector3(0f,0.0f,MathHelper.Clamp(info.GetValue(t),0,1)))
    ];

    Color[] ICosineData.LineColors => null;

    public CosineInfo Hue { get; set; } = new();
    public CosineInfo Saturation { get; set; } = new();
    public CosineInfo Luminosity { get; set; } = new();

    public Color GetValue(float t) =>
        Main.hslToRgb(
            Vector3.Clamp(
                new Vector3(
                    Hue.GetValue(t),
                    Saturation.GetValue(t),
                    Luminosity.GetValue(t)
                    ),
                default,
                Vector3.One));
}