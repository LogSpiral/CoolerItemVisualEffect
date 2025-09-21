using System;
using Terraria.ModLoader.Config;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace CoolerItemVisualEffect.Common.Config.Data.CosineGenerateHeatMapData;

public class CosineInfo
{
    [Range(-5f, 5f)] 
    [Increment(0.01f)] 
    public float Offset { get; set; } = .5f;

    [Range(-5f, 5f)]
    [Increment(0.01f)]
    public float Amplitude { get; set; }= .5f;

    [Range(-5f, 5f)]
    [Increment(0.01f)]
    public float Frequency { get; set; }= 1f;

    [Range(-5f, 5f)]
    [Increment(0.01f)]
    public float Phase { get; set; }

    public float GetValue(float t) => MathF.Cos((Frequency * t + Phase) * MathHelper.TwoPi) * Amplitude + Offset;

    public CosineInfo Combine(CosineInfo other) => new()
    {
        Offset = other.Offset + Offset,
        Amplitude = other.Amplitude * Amplitude,
        Frequency = other.Frequency * Frequency,
        Phase = other.Phase + Phase
    };
}
