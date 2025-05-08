using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Config
{
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
    public interface ICosineData
    {
        CosineInfo[] Cosines { get; }
        Func<float, CosineInfo, Color>[] LineColorMethods { get; }
        Color[] LineColors { get; }
        Color GetValue(float t);
    }
    public class CosineGenerateHeatMapData_RGB : ICosineData
    {
        [JsonIgnore]
        public CosineInfo[] Cosines => [R.Combine(Global), G.Combine(Global), B.Combine(Global)];
        [JsonIgnore]
        public Color[] LineColors => [Color.Red, Color.Lime, Color.Blue];//Lime才是G为255的那个
        [JsonIgnore]
        public Func<float, CosineInfo, Color>[] LineColorMethods => null;
        public CosineInfo R = new() { valueOffset = .731f, amplitude = .358f, frequence = 1.077f, phase = .965f };
        public CosineInfo G = new() { valueOffset = 1.098f, amplitude = 1.09f, frequence = .36f, phase = 2.265f };
        public CosineInfo B = new() { valueOffset = .192f, amplitude = 0.657f, frequence = .328f, phase = .837f };
        public CosineInfo Global = new() { valueOffset = 0, amplitude = 1f, frequence = 1f, phase = 0 };
        public Color GetValue(float t) => new Vector3(R.Combine(Global).GetValue(t), G.Combine(Global).GetValue(t), B.Combine(Global).GetValue(t)).ToColor();
    }
    public class CosineGenerateHeatMapData_HSL : ICosineData
    {
        [JsonIgnore]
        public CosineInfo[] Cosines => [H, S, L];

        [JsonIgnore]
        public Func<float, CosineInfo, Color>[] LineColorMethods => _lineColorMethods;

        readonly Func<float, CosineInfo, Color>[] _lineColorMethods
            = [
                (t,info)=>Main.hslToRgb(new Vector3((info.GetValue(t) % 1 + 1) % 1,1.0f,0.5f)),
                (t,info)=>Main.hslToRgb(new Vector3(Main.GlobalTimeWrappedHourly % 1,MathHelper.Clamp(info.GetValue(t),0,1),0.5f)),
                (t,info)=>Main.hslToRgb(new Vector3(0f,0.0f,MathHelper.Clamp(info.GetValue(t),0,1)))
              ];

        [JsonIgnore]
        public Color[] LineColors => null;

        public CosineInfo H = new();
        public CosineInfo S = new();
        public CosineInfo L = new();
        public Color GetValue(float t) => Main.hslToRgb(Vector3.Clamp(new Vector3(H.GetValue(t), S.GetValue(t), L.GetValue(t)), default, Vector3.One));
    }
}
