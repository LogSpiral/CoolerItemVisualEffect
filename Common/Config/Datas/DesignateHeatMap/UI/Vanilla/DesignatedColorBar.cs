using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace CoolerItemVisualEffect.Common.Config.Datas.DesignateHeatMap.UI.Vanilla;

public class DesignatedColorBar : UIElement
{
    public ConfigElement owner;

    public DesignateHeatMapData data;

    public override void OnInitialize()
    {
        base.OnInitialize();
    }

    public void AddCurrentDatas()
    {
        foreach (var c in data.colors)
        {
            var singleColor = new DesignatedSingleColor()
            {
                ColorInfo = c,
                Width = new(16, 0),
                Height = new(30, 0),
                Left = new(0, c.position - .5f),
                Top = new(0, .5f),
                HAlign = .5f,
                owner = owner
            };
            Append(singleColor);
        }
    }

    public override void LeftClick(UIMouseEvent evt)//单点添加单色
    {
        if (evt.Target != this) return;
        var dimension = GetDimensions();
        float k = (Main.MouseScreen.X - dimension.X) / dimension.Width;
        Color c = data.GetValue(k);
        DesignateHeatMapData.ColorInfo info = new() { color = c, position = k };
        data.colors.Add(info);
        var singleColor = new DesignatedSingleColor()
        {
            ColorInfo = info,
            Width = new(16, 0),
            Height = new(30, 0),
            Left = new(0, k - .5f),
            Top = new(0, .5f),
            HAlign = .5f,
            owner = owner
        };
        Append(singleColor);
        owner?.SetObject(data);
        base.LeftClick(evt);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)//需要预留一些空白空间来添加单色
    {
        var dimension = GetDimensions();
        float w = dimension.Width;
        float h = dimension.Height;
        data.PreGetValue();
        for (int n = 0; n < w; n++)
        {
            Color c = data.GetValue(n / (w - 1f));
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position() + new Vector2(n, 0), new Rectangle(0, 0, 1, (int)h / 2), c, 0, default, 1, 0, 0);
        }
        base.DrawSelf(spriteBatch);
    }
}
