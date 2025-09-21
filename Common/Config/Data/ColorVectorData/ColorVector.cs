using Terraria.ModLoader.Config;
using System.ComponentModel;
using Newtonsoft.Json;
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PossibleNullReferenceException

namespace CoolerItemVisualEffect.Common.Config.Data.ColorVectorData;

public class ColorVector
{
    [Range(0, 1f)] 
    [DefaultValue(1f)] 
    public float MapColorAlpha { get; set; } = 1f;

    [Range(0, 1f)]
    [DefaultValue(0.25f)]
    public float WeaponColorAlpha { get; set; }= .25f;

    [Range(0, 1f)]
    [DefaultValue(1f)]
    public float HeatMapAlpha { get; set; }= 1f;

    [DefaultValue(true)]
    public bool Normalize { get; set; } = true;

    [JsonIgnore]
    public Vector3 AlphaVector
    {
        get
        {
            var result = new Vector3(MapColorAlpha, WeaponColorAlpha, HeatMapAlpha);
            if (Normalize)
            {
                var sum = Vector3.Dot(Vector3.One, result);
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
            v1!.MapColorAlpha == v2!.MapColorAlpha &&
            v1.WeaponColorAlpha == v2.WeaponColorAlpha &&
            v1.HeatMapAlpha == v2.HeatMapAlpha &&
            v1.Normalize == v2.Normalize;
    }

    public static bool operator !=(ColorVector v1, ColorVector v2)
    {
        return !v1.Equals(v2);
    }

    public override bool Equals(object obj) => obj is ColorVector vec && this == vec;

    public override int GetHashCode()
    {
        return MapColorAlpha.GetHashCode() ^ WeaponColorAlpha.GetHashCode() ^ HeatMapAlpha.GetHashCode() ^ Normalize.GetHashCode();
    }
}