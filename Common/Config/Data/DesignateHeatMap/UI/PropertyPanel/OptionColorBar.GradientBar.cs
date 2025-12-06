using CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.Vanilla;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.PropertyPanel;

public partial class OptionColorBar
{
    private class GradientBar : UIElementGroup
    {
        public OptionColorBar Owner { get; init; }

        public DesignateHeatMapData Data { get; init; }

        public void AddCurrentData()
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var c in Data.Colors)
            {
                var singleColor = new ColorCursor()
                {
                    ColorInfo = c,
                    Width = new Dimension(16),
                    Height = new Dimension(30, 0),
                    Left = new Anchor(0, c.Position - .5f, .5f),
                    Top = new Anchor(0, .5f),
                    Positioning = Positioning.Absolute,
                    Owner = Owner
                };
                AddChild(singleColor);
            }
        }

        public override void OnLeftMouseClick(SilkyUIFramework.UIMouseEvent evt)
        {
            base.OnLeftMouseClick(evt);

            if (evt.Source != this) return;

            var dimension = Bounds;
            var k = (Main.MouseScreen.X - dimension.X) / dimension.Width;
            var c = Data.GetValue(k);
            DesignateHeatMapData.ColorInfo info = new() { Color = c, Position = k };
            Data.Colors.Add(info);
            var singleColor = new ColorCursor()
            {
                ColorInfo = info,
                Width = new Dimension(16, 0),
                Height = new Dimension(30, 0),
                Left = new Anchor(0, k - .5f, .5f),
                Top = new Anchor(0, .5f),
                Positioning = Positioning.Absolute,
                Owner = Owner
            };
            AddChild(singleColor);
        }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            var dimension = Bounds;
            var w = dimension.Width;
            var h = dimension.Height;
            Data.PreGetValue();
            for (var n = 0; n < w; n++)
            {
                var c = Data.GetValue(n / (w - 1f));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position + new Vector2(n, 0), new Rectangle(0, 0, 1, (int)h / 2), c, 0, default, 1, 0, 0);
            }

        }
    }
}
