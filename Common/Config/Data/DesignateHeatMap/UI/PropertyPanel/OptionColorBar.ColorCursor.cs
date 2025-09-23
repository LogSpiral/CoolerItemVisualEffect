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

namespace CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.PropertyPanel;

public partial class OptionColorBar
{
    private class ColorCursor : UIView
    {
        public OptionColorBar Owner { get; init; }
        private bool _dragging;
        public DesignateHeatMapData.ColorInfo ColorInfo;
        //public override void OnRightMouseClick(UIMouseEvent evt)
        //{
        //    base.OnRightMouseClick(evt);
        //    Owner.Height.Pixels = 270;
        //    var top = 100;
        //    var children = Owner.Children;
        //    if (children.Count() > 1 && children is List<UIElement> list)
        //    {
        //        list.Last().Remove();
        //    }
        //    var memberInfo = new PropertyFieldWrapper(ColorInfo.GetType().GetField("color"));
        //    if (Owner is DesignateColorConfigElement)
        //        UIModConfig.WrapIt(Owner, ref top, memberInfo, ColorInfo, 0);
        //    //else
        //    //    GenericConfigElement.WrapIt(owner, ref top, memberInfo, ColorInfo, 0, onSetObj: ConfigSLSystem.Instance.configSLUI.OnSetConfigElementValue, owner: WeaponGroupSystem.Instance.WeaponGroupUI);
        //    if (Owner.Parent != null)
        //    {
        //        Owner.Parent.Height.Pixels = 270;
        //        Owner.Parent.Recalculate();
        //    }
        //}
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
                    Remove();
                    data.Colors.Remove(ColorInfo);
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
            spriteBatch.Draw(TextureAssets.Item[ItemID.LastPrism].Value, dimension.Position, null, Color.White, 0, default, new Vector2(dimension.Width / 26f, 2 / 3f), 0, 0);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position + new Vector2(0, 15), new Rectangle(0, 0, 1, 1), Color.White, 0, default, new Vector2(dimension.Width, dimension.Height - 15), 0, 0);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position + new Vector2(2, 17), new Rectangle(0, 0, 1, 1), ColorInfo.Color, 0, default, new Vector2(dimension.Width - 4, dimension.Height - 19), 0, 0);

        }
    }
}
