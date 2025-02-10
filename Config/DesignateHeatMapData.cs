using CoolerItemVisualEffect.Config.ConfigSLer;
using LogSpiralLibrary.CodeLibrary.UIGenericConfig;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace CoolerItemVisualEffect.Config
{
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
            int count = colors.Count;
            if (colors == null || count == 0)
                return Color.Transparent;
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
    public class DesignatedSingleColor : UIElement
    {
        public ConfigElement owner;
        public bool Dragging;
        public DesignateHeatMapData.ColorInfo ColorInfo;
        public override void RightClick(UIMouseEvent evt)//打开当前颜色的编辑UI
        {
            owner.Height.Pixels = 270;
            int top = 100;
            var children = owner.Children;
            if (children.Count() > 1 && children is List<UIElement> list)
            {
                list.Last().Remove();
            }
            var memberInfo = new PropertyFieldWrapper(ColorInfo.GetType().GetField("color"));
            if (owner is DesignateColorConfigElement)
                UIModConfig.WrapIt(owner, ref top, memberInfo, ColorInfo, 0);
            else
                GenericConfigElement.WrapIt(owner, ref top, memberInfo, ColorInfo, 0, onSetObj: ConfigSLSystem.Instance.configSLUI.OnSetConfigElementValue, owner: WeaponSelectorSystem.Instance.WeaponSelectorUI);
            if (owner.Parent != null)
            {
                owner.Parent.Height.Pixels = 270;
                owner.Parent.Recalculate();
            }
            base.RightClick(evt);
        }
        public override void LeftMouseDown(UIMouseEvent evt) //开始拖动
        {
            if (evt.Target != this) return;
            Dragging = true;
            base.LeftMouseDown(evt);
        }
        public override void LeftMouseUp(UIMouseEvent evt) //结束
        {
            Dragging = false;
            var dimension = Parent.GetDimensions();
            float m = (Main.MouseScreen.Y - dimension.Y) / dimension.Height;
            if (owner is DesignateColorConfigElement designate1)
            {
                var data = designate1.designatedColorBar.data;
                if (m > 1 && data.colors.Count > 2)
                {
                    Remove();
                    data.colors.Remove(ColorInfo);
                }
                else
                    Top.Percent = .5f;
                designate1.Value = data;
            }
            else if (owner is DesignateColorConfigElement_Generic designate2)
            {
                var data = designate2.designatedColorBar.data;
                if (m > 1 && data.colors.Count > 2)
                {
                    Remove();
                    data.colors.Remove(ColorInfo);
                }
                else
                    Top.Percent = .5f;
                designate2.Value = data;
            }
            Recalculate();
            base.LeftMouseUp(evt);
        }
        public override void Update(GameTime gameTime) //调整位置或者移除
        {
            if (!Dragging || Parent == null) return;
            var dimension = Parent.GetDimensions();
            float k = (Main.MouseScreen.X - dimension.X) / dimension.Width;
            k = MathHelper.Clamp(k, 0, 1);
            float m = (Main.MouseScreen.Y - dimension.Y) / dimension.Height * 2 - 1f;

            ColorInfo.position = k;
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
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimension.Position() + new Vector2(2, 17), new Rectangle(0, 0, 1, 1), ColorInfo.color, 0, default, new Vector2(dimension.Width - 4, dimension.Height - 19), 0, 0);

            base.DrawSelf(spriteBatch);
        }
    }
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
    public class DesignateColorConfigElement : ConfigElement<DesignateHeatMapData>
    {
        public DesignatedColorBar designatedColorBar;
        public override void OnBind()
        {
            Height.Pixels = 100;
            designatedColorBar = new DesignatedColorBar()
            {
                data = Value,
                Width = new(-40, 1f),
                Height = new(50, 0f),
                Left = new(20, 0),
                Top = new(30, 0),
                owner = this
            };
            Append(designatedColorBar);
            designatedColorBar.AddCurrentDatas();
            base.OnBind();
        }
    }
    public class DesignateColorConfigElement_Generic : GenericConfigElement<DesignateHeatMapData>
    {
        public DesignatedColorBar designatedColorBar;
        public override void OnBind()
        {
            Height.Pixels = 100;
            designatedColorBar = new DesignatedColorBar()
            {
                data = Value,
                Width = new(-40, 1f),
                Height = new(50, 0f),
                Left = new(20, 0),
                Top = new(30, 0),
                owner = this
            };
            Append(designatedColorBar);
            designatedColorBar.AddCurrentDatas();
            base.OnBind();
        }
    }
}
