using Humanizer;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.UI;

namespace CoolerItemVisualEffect.ConfigSLer
{
    public class ModImageButton : UIElement
    {
        public string HoverText = "";
        public Func<Color> DrawColor;
        public Asset<Texture2D> Texture { get; private set; }
        public Color ColorActive { get; private set; }
        public Color ColorInactive { get; private set; }
        public SoundStyle? PlaySound { get; private set; }
        public Asset<Texture2D> BorderTexture { get; private set; }
        public Asset<Texture2D> BackgroundTexture { get; private set; }

        public ModImageButton(Asset<Texture2D> texture, Color activeColor = default, Color inactiveColor = default)
        {
            if (texture is not null)
            {
                Texture = texture;
                Width.Set(Texture.Width(), 0f);
                Height.Set(Texture.Height(), 0f);
            }
            ColorActive = activeColor;
            ColorInactive = inactiveColor;
            PlaySound = null;
        }

        #region 各种设置方法
        public void SetCenter(int x, int y)
        {
            CalculatedStyle dimensions = GetDimensions();
            Left.Set(x - dimensions.Width / 2, 0f);
            Top.Set(y - dimensions.Height / 2, 0f);
        }

        public void SetSound(SoundStyle sound)
        {
            PlaySound = sound;
        }

        public void SetColor(Color? activeColor = null, Color? inactiveColor = null)
        {
            ColorActive = activeColor ?? ColorActive;
            ColorInactive = inactiveColor ?? ColorActive;
        }

        public void SetBackgroundImage(Asset<Texture2D> texture)
        {
            BackgroundTexture = texture;
        }

        public void SetHoverImage(Asset<Texture2D> texture)
        {
            BorderTexture = texture;
        }

        public void SetImage(Asset<Texture2D> texture, bool changeSize = false)
        {
            Texture = texture;
            if (changeSize)
            {
                Width.Set(Texture.Width(), 0f);
                Height.Set(Texture.Height(), 0f);
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (!string.IsNullOrEmpty(HoverText))
                {
                    string text = HoverText;
                    if (HoverText.StartsWith("{$") && HoverText.EndsWith("}"))
                    {
                        text = Language.GetTextValue(HoverText.Substring("{$".Length, HoverText.Length - "{$}".Length));
                    }
                    Main.instance.MouseText(text);
                }
            }
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            var mainColor = IsMouseHovering ? ColorActive : ColorInactive;
            if (DrawColor is not null)
            {
                mainColor = DrawColor.Invoke();
            }

            if (BackgroundTexture is not null)
            {
                spriteBatch.Draw(BackgroundTexture.Value, dimensions.Center(), null, mainColor, 0f, BackgroundTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(Texture.Value, dimensions.Center(), null, mainColor, 0f, Texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            if (BorderTexture is not null && IsMouseHovering)
            {
                spriteBatch.Draw(BorderTexture.Value, dimensions.Center(), null, mainColor * 1.4f, 0f, BorderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (PlaySound.HasValue)
            {
                SoundEngine.PlaySound(PlaySound.Value);
            }
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }
    }
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class SUIScrollbar : UIElement
    {
        private float viewPosition; // 滚动条当前位置
        private float MaxViewPoisition => maxViewSize - viewSize;

        private float viewSize = 1f; // 显示出来的高度
        private float maxViewSize = 20f; // 控制元素的高度
        private float ViewScale => viewSize / maxViewSize;
        private bool innerHovered;

        // 用于拖动内滚动条
        private float offsetY;
        public bool dragging;

        public bool Visible;
        public int timer;
        public float factor => MathHelper.SmoothStep(0, 1, timer / 15f);
        public float ViewPosition
        {
            get => viewPosition;
            set => viewPosition = MathHelper.Clamp(value, 0f, MaxViewPoisition);
        }

        private float _bufferViewPosition;
        /// <summary>
        /// 缓冲距离, 不想使用动画就直接设置 <see cref="ViewPosition"/>
        /// </summary>
        public float BufferViewPosition
        {
            get => _bufferViewPosition;
            set => _bufferViewPosition = value;
        }

        public SUIScrollbar()
        {
            Visible = true;
            Width.Pixels = 20;
            MaxWidth.Pixels = 20;
            SetPadding(5);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            CalculatedStyle InnerDimensions = GetInnerDimensions();
            CalculatedStyle InnerRectangle = InnerDimensions;
            InnerRectangle.Y += (ViewPosition / MaxViewPoisition) * (InnerDimensions.Height * (1 - ViewScale));
            InnerRectangle.Height = InnerDimensions.Height * ViewScale;
            if (InnerRectangle.Contains(Main.MouseScreen))
            {
                if (!innerHovered)
                {
                    innerHovered = true;
                    InnerMouseOver();
                }
            }
            else
            {
                if (innerHovered)
                {
                    InnerMouseOut();
                }
                innerHovered = false;
            }
            timer = (int)MathHelper.Clamp(timer, 0, 15);
            base.Update(gameTime);

            if (dragging)
            {
                if (ViewScale != 1)
                    ViewPosition = (Main.MouseScreen.Y - InnerDimensions.Y - offsetY) / (InnerDimensions.Height * (1 - ViewScale)) * MaxViewPoisition;
            }

            if (BufferViewPosition != 0)
            {
                ViewPosition -= BufferViewPosition * 0.2f;
                BufferViewPosition *= 0.8f;
                if (MathF.Abs(BufferViewPosition) < 0.1f)
                {
                    ViewPosition = MathF.Round(ViewPosition, 1);
                    BufferViewPosition = 0;
                }
            }
        }

        public virtual void InnerMouseOver()
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            timer++;
            
        }

        public virtual void InnerMouseOut()
        {
            timer--;
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);

            if (!Visible)
                return;

            if (evt.Target == this)
            {
                CalculatedStyle InnerDimensions = GetInnerDimensions();

                if (InnerDimensions.Contains(Main.MouseScreen))
                {
                    dragging = true;
                    offsetY = evt.MousePosition.Y - InnerDimensions.Y - (InnerDimensions.Height * (1 - ViewScale) * (viewPosition / MaxViewPoisition));
                }
                BufferViewPosition = 0;
            }
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            if (!Visible)
                return;
            dragging = false;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (!Visible)
                return;
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIScrollbar");
        }

        public readonly Color hoveredColor = new(220, 220, 220);
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            CalculatedStyle dimension = GetDimensions();
            Vector2 position = dimension.Position();
            Vector2 size = dimension.Size();


            // 滚动条背板
            //PixelShader.DrawRoundRect(position, size, size.X / 2, UIColor.ScrollBarBackground, 3, UIColor.PanelBorder);

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 innerPosition = innerDimensions.Position();
            Vector2 innerSize = innerDimensions.Size();
            if (MaxViewPoisition != 0)
                innerPosition.Y += innerDimensions.Height * (1 - ViewScale) * (ViewPosition / MaxViewPoisition);
            innerSize.Y *= ViewScale;

            Color hoverColor = Color.Lerp(Color.Gray, hoveredColor, factor);

            // 滚动条拖动块
            //PixelShader.DrawRoundRect(innerPosition, innerSize, innerSize.X / 2, hoverColor);
        }

        public void SetView(float viewSize, float maxViewSize)
        {
            viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
            viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);

            this.viewSize = viewSize;
            this.maxViewSize = maxViewSize;
        }
    }
}
