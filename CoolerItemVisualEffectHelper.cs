using Microsoft.Xna.Framework.Graphics;

namespace CoolerItemVisualEffect;

public static class CoolerItemVisualEffectHelper
{
    public static void CreateHeatMapIfNull(ref Texture2D texture2D)
    {
        texture2D ??= new Texture2D(Main.instance.GraphicsDevice, 300, 1);
        // if (texture2D != null) return;
        // Texture2D dummy = null;
        // Main.RunOnMainThread(() => dummy = new Texture2D(Main.instance.GraphicsDevice, 300, 1));
        // texture2D = dummy;
    }
    public static Color CalculateWeightedMean(Texture2D texture)
    {
        var w = texture.Width;
        var he = texture.Height;
        var cs = new Color[w * he];
        texture.GetData(cs);
        Vector4 vcolor = default;
        float count = 0;
        for (int i = 0; i < cs.Length; i++)
        {
            if (cs[i] != default
                && (i - w < 0 || cs[i - w] != default)
                && (i - 1 < 0 || cs[i - 1] != default)
                && (i + w >= cs.Length || cs[i + w] != default)
                && (i + 1 >= cs.Length || cs[i + 1] != default))
            {
                var weight = (float)((i + 1) % w * (he - i / w)) / w / he;
                vcolor += cs[i].ToVector4() * weight;
                count += weight;
            }
        }
        vcolor /= count;
        return new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
    }
}
