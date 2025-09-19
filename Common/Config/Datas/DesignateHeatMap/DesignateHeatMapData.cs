using System;
using System.Collections.Generic;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Common.Config.Datas.DesignateHeatMap;

public class DesignateHeatMapData
{
    public class ColorInfo
    {
        public Color color;

        [Range(0f, 1f)]
        public float position;
    }

    public List<ColorInfo> colors
        = [
            new() { color = Color.Blue,position = .0f},
            new() { color = Color.Green,position = .5f},
            new() { color = Color.Yellow,position = 1f}
          ];

    public void PreGetValue()
    {
        if (colors == null || colors.Count < 2) return;
        colors.Sort((c1, c2) => Math.Sign(c1.position - c2.position));
    }

    public Color GetValue(float t)
    {
        if (colors is null or { Count: 0 })
            return Color.Transparent;

        int count = colors.Count;

        if (count == 1) return colors[0].color;

        ColorInfo current = colors[0];
        ColorInfo previous = current;
        for (int u = 1; t > current.position; u++)
        {
            if (u == count)
            {
                previous = current;
                break;
            }
            previous = current;
            current = colors[u];
        }

        if (current == previous) return current.color;
        return Color.Lerp(previous.color, current.color, Utils.GetLerpValue(previous.position, current.position, t));
    }
}