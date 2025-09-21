using System;
using System.Collections.Generic;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace CoolerItemVisualEffect.Common.Config.Data.CosineGenerateHeatMapData;

public class CosineGenerateHeatMapData_RGB : ICosineData
{
    IReadOnlyList<CosineInfo> ICosineData.Cosines => [Red.Combine(Global), Green.Combine(Global), Blue.Combine(Global)];

    // Lime才是G为255的那个, 而不是Green
    Color[] ICosineData.LineColors => [Color.Red, Color.Lime, Color.Blue];

    Func<float, CosineInfo, Color>[] ICosineData.LineColorMethods => null;

    public CosineInfo Red { get; set; } = new() { Offset = .731f, Amplitude = .358f, Frequency = 1.077f, Phase = .965f };
    public CosineInfo Green{ get; set; }  = new() { Offset = 1.098f, Amplitude = 1.09f, Frequency = .36f, Phase = 2.265f };
    public CosineInfo Blue { get; set; } = new() { Offset = .192f, Amplitude = 0.657f, Frequency = .328f, Phase = .837f };
    public CosineInfo Global { get; set; }  = new() { Offset = 0, Amplitude = 1f, Frequency = 1f, Phase = 0 };

    public Color GetValue(float t) =>
        new Vector3(
            Red.Combine(Global).GetValue(t),
            Green.Combine(Global).GetValue(t),
            Blue.Combine(Global).GetValue(t)
            ).ToColor();
}