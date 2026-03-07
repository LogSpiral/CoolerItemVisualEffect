using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using Terraria.GameContent;

namespace CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.PropertyPanel;

public partial class OptionColorBar
{
    private class ColorCursor : UIView
    {
        public OptionColorBar Owner { get; init; }
        private bool _dragging;
        public DesignateHeatMapData.ColorInfo ColorInfo;
        public override void OnRightMouseClick(UIMouseEvent evt)
        {
            base.OnRightMouseClick(evt);

            if (Owner.EditTarget == ColorInfo)
            {
                Owner.EditTarget = null;
                Owner.ExpandTimer.StartReverseUpdate();
            }
            else
            {
                Owner.EditTarget = ColorInfo;
                Owner.InnerPanel.Filler = new ObjectMetaDataFiller(ColorInfo);
                var cursorPositionSetter = new DelegateWriter();
                cursorPositionSetter.OnWriteValue += delegate
                {
                    SetLeft(0, ColorInfo.Position - 0.5f);
                    Owner.SetValue(Owner.GetValue());
                };
                Owner.InnerPanel.Writer = new CombinedWriter(DefaultWriter.Instance, cursorPositionSetter);
                Owner.ExpandTimer.StartUpdate();
            }
        }
        public override void OnLeftMouseDown(UIMouseEvent evt)
        {
            base.OnLeftMouseDown(evt);

            if (evt.Source != this) return;
            _dragging = true;
        }


        public override void OnLeftMouseUp(UIMouseEvent evt)
        {
            base.OnLeftMouseUp(evt);
            _dragging = false;
            var dimension = Parent.Bounds;
            var m = (Main.MouseScreen.Y - dimension.Y) / dimension.Height;
            if (Owner is not null)
            {
                var data = Owner.ColorBar.Data;
                if (m > 1 && data.Colors.Count > 2)
                {
                    RemoveFromParent();
                    data.Colors.Remove(ColorInfo);


                    if (Owner.EditTarget == ColorInfo)
                    {
                        Owner.EditTarget = null;
                        Owner.ExpandTimer.StartReverseUpdate();
                    }

                }
                else
                    SetTop(percent: 0.5f);


                Owner.SetValue(data);
            }
        }
        protected override void UpdateStatus(GameTime gameTime)
        {
            base.UpdateStatus(gameTime);
            if (!_dragging || Parent == null) return;
            var dimension = Parent.Bounds;
            var k = (Main.MouseScreen.X - dimension.X) / dimension.Width;
            k = MathHelper.Clamp(k, 0, 1);
            var m = (Main.MouseScreen.Y - dimension.Y) / dimension.Height * 2 - 1f;

            ColorInfo.Position = k;
            SetLeft(percent: k - .5f);
            if (m > .85f)
                SetTop(percent: .5f + .5f * MathF.Pow(MathHelper.Clamp((m - .85f) / .15f, 0, 1), 2.0f));
            else
                SetTop(percent: .5f);
        }
        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var dimension = Bounds;

            var prism = TextureAssets.Item[ItemID.LastPrism];
            if (!prism.IsLoaded)
                Main.instance.LoadItem(ItemID.LastPrism);

            var frameColor = Color.White;
            if (Owner is not null && Owner.EditTarget == ColorInfo)
                frameColor = Color.Lerp(Main.DiscoColor, Color.White, 0.5f);
            spriteBatch.Draw(prism.Value, dimension.Position, null, Color.White, 0, default, new Vector2(dimension.Width / 26f, 2 / 3f), 0, 0);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position + new Vector2(0, 15), new Rectangle(0, 0, 1, 1), frameColor, 0, default, new Vector2(dimension.Width, dimension.Height - 15), 0, 0);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position + new Vector2(2, 17), new Rectangle(0, 0, 1, 1), ColorInfo.Color, 0, default, new Vector2(dimension.Width - 4, dimension.Height - 19), 0, 0);

        }
    }
}
