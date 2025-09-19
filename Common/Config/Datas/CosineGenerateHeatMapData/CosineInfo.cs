using System;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.Config.Datas.CosineGenerateHeatMapData;

public class CosineInfo
{
    [Range(-5f, 5f)]
    [Increment(0.01f)]
    public float valueOffset = .5f;

    [Range(-5f, 5f)]
    [Increment(0.01f)]
    public float amplitude = .5f;

    [Range(-5f, 5f)]
    [Increment(0.01f)]
    public float frequence = 1f;

    [Range(-5f, 5f)]
    [Increment(0.01f)]
    public float phase = 0f;

    public float GetValue(float t) => MathF.Cos((frequence * t + phase) * MathHelper.TwoPi) * amplitude + valueOffset;

    public CosineInfo Combine(CosineInfo other) => new()
    {
        valueOffset = other.valueOffset + valueOffset,
        amplitude = other.amplitude * amplitude,
        frequence = other.frequence * frequence,
        phase = other.phase + phase
    };
}
