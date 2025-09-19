using System;
using Terraria.ModLoader.Config;
using System.ComponentModel;
using Newtonsoft.Json;

namespace CoolerItemVisualEffect.Common.Config.Datas.ColorVectorData;

public class ColorVector
{
    [Range(0, 1f)]
    [DefaultValue(1f)]
    public float mapColorAlpha = 1f;

    [Range(0, 1f)]
    [DefaultValue(0.25f)]
    public float weaponColorAlpha = .25f;

    [Range(0, 1f)]
    [DefaultValue(1f)]
    public float heatMapAlpha = 1f;

    [DefaultValue(true)]
    public bool normalize = true;

    [JsonIgnore]
    public Vector3 AlphaVector
    {
        get
        {
            var result = new Vector3(mapColorAlpha, weaponColorAlpha, heatMapAlpha);
            if (normalize)
            {
                float sum = Vector3.Dot(Vector3.One, result);
                if (sum == 0) return Vector3.One * .33f;
                return result / sum;
            }
            else
            {
                return result;
            }
        }
    }

    public static bool operator ==(ColorVector v1, ColorVector v2)
    {
        return
            v1.mapColorAlpha == v2.mapColorAlpha &&
            v1.weaponColorAlpha == v2.weaponColorAlpha &&
            v1.heatMapAlpha == v2.heatMapAlpha &&
            v1.normalize == v2.normalize;
    }

    public static bool operator !=(ColorVector v1, ColorVector v2)
    {
        return !v1.Equals(v2);
    }

    public override bool Equals(object obj) => obj is ColorVector vec && this == vec;

    public override int GetHashCode()
    {
        return mapColorAlpha.GetHashCode() ^ weaponColorAlpha.GetHashCode() ^ heatMapAlpha.GetHashCode() ^ normalize.GetHashCode();
    }
}