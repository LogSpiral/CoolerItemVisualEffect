using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.Vanilla;

public class DesignatedSingleColor : UIElement
{
    public ConfigElement Owner { get; init; }
    private bool _dragging;
    public DesignateHeatMapData.ColorInfo ColorInfo;

    public override void RightClick(UIMouseEvent evt)//打开当前颜色的编辑UI
    {
        Owner.Height.Pixels = 270;
        var top = 100;
        var children = Owner.Children;
        if (children.Count() > 1 && children is List<UIElement> list)
        {
            list.Last().Remove();
        }
        var memberInfo = new PropertyFieldWrapper(ColorInfo.GetType().GetField("color"));
        if (Owner is DesignateColorConfigElement)
            UIModConfig.WrapIt(Owner, ref top, memberInfo, ColorInfo, 0);
        //else
        //    GenericConfigElement.WrapIt(owner, ref top, memberInfo, ColorInfo, 0, onSetObj: ConfigSLSystem.Instance.configSLUI.OnSetConfigElementValue, owner: WeaponGroupSystem.Instance.WeaponGroupUI);
        if (Owner.Parent != null)
        {
            Owner.Parent.Height.Pixels = 270;
            Owner.Parent.Recalculate();
        }
        base.RightClick(evt);
    }

    public override void LeftMouseDown(UIMouseEvent evt) //开始拖动
    {
        if (evt.Target != this) return;
        _dragging = true;
        base.LeftMouseDown(evt);
    }

    public override void LeftMouseUp(UIMouseEvent evt) //结束
    {
        _dragging = false;
        var dimension = Parent.GetDimensions();
        var m = (Main.MouseScreen.Y - dimension.Y) / dimension.Height;
        if (Owner is DesignateColorConfigElement designate1)
        {
            var data = designate1.ColorBar.Data;
            if (m > 1 && data.Colors.Count > 2)
            {
                Remove();
                data.Colors.Remove(ColorInfo);
            }
            else
                Top.Percent = .5f;
            designate1.Value = data;
        }
        //else if (owner is DesignateColorConfigElement_Generic designate2)
        //{
        //    var data = designate2.designatedColorBar.data;
        //    if (m > 1 && data.colors.Count > 2)
        //    {
        //        Remove();
        //        data.colors.Remove(ColorInfo);
        //    }
        //    else
        //        Top.Percent = .5f;
        //    designate2.Value = data;
        //}
        Recalculate();
        base.LeftMouseUp(evt);
    }

    public override void Update(GameTime gameTime) //调整位置或者移除
    {
        if (!_dragging || Parent == null) return;
        var dimension = Parent.GetDimensions();
        var k = (Main.MouseScreen.X - dimension.X) / dimension.Width;
        k = MathHelper.Clamp(k, 0, 1);
        var m = (Main.MouseScreen.Y - dimension.Y) / dimension.Height * 2 - 1f;

        ColorInfo.Position = k;
        Left.Percent = k - .5f;
        if (m > .85f)
            Top.Percent = .5f + .5f * MathF.Pow(MathHelper.Clamp((m - .85f) / .15f, 0, 1), 2.0f);
        else
            Top.Percent = .5f;
        Recalculate();
        base.Update(gameTime);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)//绘制颜色标
    {
        var dimension = GetDimensions();
        //spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.ToRectangle(), Color.Red);
        spriteBatch.Draw(TextureAssets.Item[ItemID.LastPrism].Value, dimension.Position(), null, Color.White, 0, default, new Vector2(dimension.Width / 26f, 2 / 3f), 0, 0);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position() + new Vector2(0, 15), new Rectangle(0, 0, 1, 1), Color.White, 0, default, new Vector2(dimension.Width, dimension.Height - 15), 0, 0);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position() + new Vector2(2, 17), new Rectangle(0, 0, 1, 1), ColorInfo.Color, 0, default, new Vector2(dimension.Width - 4, dimension.Height - 19), 0, 0);

        base.DrawSelf(spriteBatch);
    }
}
