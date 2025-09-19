using CoolerItemVisualEffect.Common.Config.Preview;
using LogSpiralLibrary.CodeLibrary.ConfigModification;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.Config.Datas.ByFuncHeatMap;

public class ByFuncHeatMapData
{
    public class HueInfo
    {
        [DefaultValue(.2f)]
        [Increment(0.01f)]
        [Range(-1f, 1f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float range = .2f;

        [DefaultValue(0f)]
        [Increment(0.01f)]
        [Range(0f, 1f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float offset = 0f;

        public float GetValue(float t, float orig) => (orig + (t - .5f) * range + offset) % 1;
    }

    public class SaturationInfo
    {
        [DefaultValue(1.2f)]
        [Increment(0.05f)]
        [Range(0f, 5f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float scalar = 1.2f;

        [DefaultValue(0f)]
        [Increment(0.05f)]
        [Range(0f, 2f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float range = 0f;

        [DefaultValue(0.2f)]
        [Increment(0.05f)]
        [Range(-1f, 1f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float offset = .2f;

        public float GetValue(float t, float orig) => MathHelper.Clamp(orig * scalar + (t - .5f) * range + offset, 0, 1);
    }

    public class LuminosityInfo
    {
        [DefaultValue(1f)]
        [Increment(0.01f)]
        [Range(0f, 5f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float scalar = 1f;

        [DefaultValue(0.2f)]
        [Increment(0.05f)]
        [Range(0f, 2f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float range = .2f;

        [DefaultValue(0f)]
        [Increment(0.05f)]
        [Range(-1f, 1f)]
        [CustomPreview<HeatMapRelatedDatePreview>]
        public float offset;

        public float GetValue(float t, float orig) => MathHelper.Clamp(orig * scalar + (t - .5f) * range + offset, 0, 1);
    }

    public HueInfo H = new();
    public SaturationInfo S = new();
    public LuminosityInfo L = new();

    public Vector3 GetValue(float t, Vector3 orig) => new(H.GetValue(t, orig.X), S.GetValue(t, orig.Y), L.GetValue(t, orig.Z));

    public Color GetColor(float t, Vector3 hsl) => Main.hslToRgb(GetValue(t, hsl));
}