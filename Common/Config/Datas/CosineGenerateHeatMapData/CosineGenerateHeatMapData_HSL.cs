using System;
using System.Collections.Generic;

namespace CoolerItemVisualEffect.Common.Config.Datas.CosineGenerateHeatMapData;

public class CosineGenerateHeatMapData_HSL : ICosineData
{
    IReadOnlyList<CosineInfo> ICosineData.Cosines => [H, S, L];

    Func<float, CosineInfo, Color>[] ICosineData.LineColorMethods => _lineColorMethods;

    private readonly Func<float, CosineInfo, Color>[] _lineColorMethods
        = [
            (t,info)=>Main.hslToRgb(new Vector3((info.GetValue(t) % 1 + 1) % 1,1.0f,0.5f)),
            (t,info)=>Main.hslToRgb(new Vector3(Main.GlobalTimeWrappedHourly % 1,MathHelper.Clamp(info.GetValue(t),0,1),0.5f)),
            (t,info)=>Main.hslToRgb(new Vector3(0f,0.0f,MathHelper.Clamp(info.GetValue(t),0,1)))
          ];

    Color[] ICosineData.LineColors => null;

    public CosineInfo H = new();
    public CosineInfo S = new();
    public CosineInfo L = new();

    public Color GetValue(float t) =>
        Main.hslToRgb(
            Vector3.Clamp(
                new Vector3(
                    H.GetValue(t),
                    S.GetValue(t),
                    L.GetValue(t)
                    ),
                default,
                Vector3.One));
}