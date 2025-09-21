using System;
using System.Collections.Generic;
using Terraria.ModLoader.Config;
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap;

public class DesignateHeatMapData
{
    public class ColorInfo
    {
        public Color Color { get; set; }

        [Range(0f, 1f)] 
        public float Position { get; set; }
    }

    public List<ColorInfo> Colors { get; set; } =
        [
            new() { Color = Color.Blue, Position = .0f },
            new() { Color = Color.Green, Position = .5f },
            new() { Color = Color.Yellow, Position = 1f }
        ];

    public void PreGetValue()
    {
        if (Colors == null || Colors.Count < 2) return;
        Colors.Sort((c1, c2) => Math.Sign(c1.Position - c2.Position));
    }

    public Color GetValue(float t)
    {
        if (Colors is null or { Count: 0 })
            return Color.Transparent;

        var count = Colors.Count;

        if (count == 1) return Colors[0].Color;

        var current = Colors[0];
        var previous = current;
        for (var u = 1; t > current.Position; u++)
        {
            if (u == count)
            {
                previous = current;
                break;
            }
            previous = current;
            current = Colors[u];
        }

        if (current == previous) return current.Color;
        return Color.Lerp(previous.Color, current.Color, Utils.GetLerpValue(previous.Position, current.Position, t));
    }
}