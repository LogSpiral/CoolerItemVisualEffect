using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.Vanilla;

public class DesignatedColorBar : UIElement
{
    public ConfigElement Owner { get; init; }

    public DesignateHeatMapData Data { get; init; }

    public void AddCurrentData()
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var c in Data.Colors)
        {
            var singleColor = new DesignatedSingleColor()
            {
                ColorInfo = c,
                Width = new StyleDimension(16, 0),
                Height = new StyleDimension(30, 0),
                Left = new StyleDimension(0, c.Position - .5f),
                Top = new StyleDimension(0, .5f),
                HAlign = .5f,
                Owner = Owner
            };
            Append(singleColor);
        }
    }

    public override void LeftClick(UIMouseEvent evt)//单点添加单色
    {
        if (evt.Target != this) return;
        var dimension = GetDimensions();
        var k = (Main.MouseScreen.X - dimension.X) / dimension.Width;
        var c = Data.GetValue(k);
        DesignateHeatMapData.ColorInfo info = new() { Color = c, Position = k };
        Data.Colors.Add(info);
        var singleColor = new DesignatedSingleColor()
        {
            ColorInfo = info,
            Width = new StyleDimension(16, 0),
            Height = new StyleDimension(30, 0),
            Left = new StyleDimension(0, k - .5f),
            Top = new StyleDimension(0, .5f),
            HAlign = .5f,
            Owner = Owner
        };
        Append(singleColor);
        Owner?.SetObject(Data);
        base.LeftClick(evt);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)//需要预留一些空白空间来添加单色
    {
        var dimension = GetDimensions();
        var w = dimension.Width;
        var h = dimension.Height;
        Data.PreGetValue();
        for (var n = 0; n < w; n++)
        {
            var c = Data.GetValue(n / (w - 1f));
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position() + new Vector2(n, 0), new Rectangle(0, 0, 1, (int)h / 2), c, 0, default, 1, 0, 0);
        }
        base.DrawSelf(spriteBatch);
    }
}
