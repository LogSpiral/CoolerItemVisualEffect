using CoolerItemVisualEffect.Common.Config.Preview;
using LogSpiralLibrary.CodeLibrary.ConfigModification;
using System.ComponentModel;
using Terraria.ModLoader.Config;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace CoolerItemVisualEffect.Common.Config.Data.ByFuncHeatMap;

public class ByFuncHeatMapData
{
    public class HueInfo
    {
        [DefaultValue(.2f)]
        [Increment(0.01f)]
        [Range(-1f, 1f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float Range { get; set; } = .2f;

        [DefaultValue(0f)]
        [Increment(0.01f)]
        [Range(0f, 1f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float Offset { get; set; } = 0f;

        public float GetValue(float t, float orig) => (orig + (t - .5f) * Range + Offset) % 1;
    }

    public class SaturationInfo
    {
        [DefaultValue(1.2f)]
        [Increment(0.05f)]
        [Range(0f, 5f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float Scalar { get; set; } = 1.2f;

        [DefaultValue(0f)]
        [Increment(0.05f)]
        [Range(0f, 2f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float Range { get; set; } = 0f;

        [DefaultValue(0.2f)]
        [Increment(0.05f)]
        [Range(-1f, 1f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float Offset { get; set; } = .2f;

        public float GetValue(float t, float orig) => MathHelper.Clamp(orig * Scalar + (t - .5f) * Range + Offset, 0, 1);
    }

    public class LuminosityInfo
    {
        [DefaultValue(1f)]
        [Increment(0.01f)]
        [Range(0f, 5f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float Scalar { get; set; } = 1f;

        [DefaultValue(0.2f)]
        [Increment(0.05f)]
        [Range(0f, 2f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float Range { get; set; } = .2f;

        [DefaultValue(0f)]
        [Increment(0.05f)]
        [Range(-1f, 1f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float Offset { get; set; } = 0f;

        public float GetValue(float t, float orig) => MathHelper.Clamp(orig * Scalar + (t - .5f) * Range + Offset, 0, 1);
    }

    public HueInfo Hue { get; set; } = new();
    public SaturationInfo Saturation { get; set; } = new();
    public LuminosityInfo Luminosity { get; set; } = new();
    
    public Vector3 GetValue(float t, Vector3 orig) => new(Hue.GetValue(t, orig.X), Saturation.GetValue(t, orig.Y), Luminosity.GetValue(t, orig.Z));

    public Color GetColor(float t, Vector3 hsl) => Main.hslToRgb(GetValue(t, hsl));
}