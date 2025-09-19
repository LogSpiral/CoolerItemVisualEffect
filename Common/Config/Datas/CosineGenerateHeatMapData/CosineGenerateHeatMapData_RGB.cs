using System;
using System.Collections.Generic;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;

namespace CoolerItemVisualEffect.Common.Config.Datas.CosineGenerateHeatMapData;

public class CosineGenerateHeatMapData_RGB : ICosineData
{
    IReadOnlyList<CosineInfo> ICosineData.Cosines => [R.Combine(Global), G.Combine(Global), B.Combine(Global)];

    // Lime才是G为255的那个, 而不是Green
    Color[] ICosineData.LineColors => [Color.Red, Color.Lime, Color.Blue];

    Func<float, CosineInfo, Color>[] ICosineData.LineColorMethods => null;

    public CosineInfo R = new() { valueOffset = .731f, amplitude = .358f, frequence = 1.077f, phase = .965f };
    public CosineInfo G = new() { valueOffset = 1.098f, amplitude = 1.09f, frequence = .36f, phase = 2.265f };
    public CosineInfo B = new() { valueOffset = .192f, amplitude = 0.657f, frequence = .328f, phase = .837f };
    public CosineInfo Global = new() { valueOffset = 0, amplitude = 1f, frequence = 1f, phase = 0 };

    public Color GetValue(float t) =>
        new Vector3(
            R.Combine(Global).GetValue(t),
            G.Combine(Global).GetValue(t),
            B.Combine(Global).GetValue(t)
            ).ToColor();
}