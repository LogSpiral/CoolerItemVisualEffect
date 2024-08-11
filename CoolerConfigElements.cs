//using CoolerItemVisualEffect.ConfigSLer;
//using Humanizer;
//using LogSpiralLibrary.CodeLibrary.DataStructures;
//using Microsoft.Xna.Framework.Graphics;
//using Newtonsoft.Json;
//using ReLogic.Content;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection.Emit;
//using System.Text;
//using System.Threading.Tasks;
//using static CoolerItemVisualEffect.ConfigurationCIVE;
//using Terraria.GameContent.UI.Elements;
//using Terraria.GameContent.UI.States;
//using Terraria.GameContent;
//using Terraria.ModLoader.Config.UI;
//using Terraria.ModLoader.Config;
//using Terraria.ModLoader.UI;
//using Terraria.UI.Chat;
//using Terraria.UI;
//using System.Reflection;

//namespace CoolerItemVisualEffect
//{

//    public enum ConfigTexStyle : byte
//    {
//        /*原始*/
//        Origin,
//        /*暗紫*/
//        DarkPurple,
//        /*深色金属*/
//        DarkMetal,
//        /*暗黑*/
//        Dark,
//        Purple,
//        Silver,
//        Holy
//    }
//    /// <summary>
//    /// 给<see cref="List{T}"/>类型的configitem用
//    /// </summary>
//    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
//    public class CollectionElementCustomItemAttribute : Attribute
//    {
//        public Type Type { get; }
//        public CollectionElementCustomItemAttribute(Type type)
//        {
//            Type = type;
//        }
//    }
//    public class CoolerPanelInfo : ComplexPanelInfo
//    {
//        public static Texture2D GetConfigStyleTex(ConfigTexStyle configTexStyle) => ModContent.Request<Texture2D>($"CoolerItemVisualEffect/ConfigTex/Template_{configTexStyle}").Value;
        
//        public override Texture2D StyleTexture { get => GetConfigStyleTex(configTexStyle); set { base.StyleTexture = value; Main.NewText("对这货赋值无效"); } }
//        public ConfigTexStyle configTexStyle = ConfigTexStyle.DarkPurple;
//        public override Rectangle DrawComplexPanel(SpriteBatch spriteBatch)
//        {
//            if (configTexStyle == 0)
//            {
//                ConfigElement.DrawPanel2(spriteBatch, destination.TopLeft(), TextureAssets.SettingsPanel.Value, destination.Width, destination.Height, mainColor);
//                //spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(960, 560), new Rectangle(0, 0, 1, 1), Color.Red, 0, new Vector2(.5f), 4f, 0, 0);
//                return destination;
//            }
//            else
//            {
//                return base.DrawComplexPanel(spriteBatch);
//            }
//        }
//    }
//    public class CoolerConfigElement : ConfigElement
//    {
//        public Asset<Texture2D> CopyTexture { get; set; }
//        public Asset<Texture2D> PlusUpDownTexture { get; set; }
//        public override void OnBind()
//        {
//            var path = "CoolerItemVisualEffect/ConfigTex/UI/CoolerUI_";

//            if (!KeepOrigin)
//            {
//                Height.Set(80, 0);
//                CollapsedTexture = ModContent.Request<Texture2D>($"{path}Collapsed");
//                DeleteTexture = ModContent.Request<Texture2D>($"{path}Delete");
//                ExpandedTexture = ModContent.Request<Texture2D>($"{path}Expanded");
//                PlayTexture = ModContent.Request<Texture2D>($"{path}Play");
//                PlusTexture = ModContent.Request<Texture2D>($"{path}Plus");
//                UpDownTexture = ModContent.Request<Texture2D>($"{path}UpDown");
//            }
//            CopyTexture = ModContent.Request<Texture2D>($"{path}Copy");

//            PlusUpDownTexture = ModContent.Request<Texture2D>($"{path}PlusUpDown");

//            base.OnBind();
//        }
//        public int[] timers = new int[1];
//        public int HoverCounter
//        {
//            get => timers[0];
//            set => timers[0] = value;
//        }
//        public float Scaler => KeepOrigin ? 1f : OverrideScaler;
//        public virtual float OverrideScaler => 1f;//MathHelper.SmoothStep(1, 1.1f, HoverCounter / 15f)
//        public virtual Color ModifyGlowColor(Color color)
//        {
//            return color;
//        }
//        public virtual float GlowShakingStrength => 0f;
//        public virtual Vector2 OverrideOffsetPosition
//        {
//            get
//            {
//                if (!IsMouseHovering) return default;
//                return default;
//                Rectangle rect = GetDimensions().ToRectangle();
//                Vector2 target = (new Vector2(Main.mouseX, Main.mouseY) - rect.Center()) / new Vector2(rect.Width, rect.Height) * 2;
//                Vector2 result = new Vector2(MathHelper.SmoothStep(0, 1, Math.Abs(target.X)) * Math.Sign(target.X), MathHelper.SmoothStep(0, 1, Math.Abs(target.Y)) * Math.Sign(target.Y));
//                result *= rect.Size();
//                result *= new Vector2(0.0625f, 0.25f);
//                //float right = (464f - rect.Width) / 2;
//                //if (result.X > right) result.X = right;
//                return result;
//                //Vector2 result = rect.Center();
//                //return (new Vector2(Main.mouseX, Main.mouseY) - result) * .0625f;
//            }
//        }
//        public Vector2 OffsetPosition
//        {
//            get
//            {
//                if (KeepOrigin) return default;
//                return currentOffset = Vector2.Lerp(currentOffset, OverrideOffsetPosition, 0.05f);
//            }
//        }
//        public Vector2 currentOffset
//        {
//            get;
//            private set;
//        }
//        public override void DrawSelf(SpriteBatch spriteBatch)
//        {

//            if (KeepOrigin)
//            {
//                base.DrawSelf(spriteBatch);
//                return;
//            }

//            #region 魔改列表间距
//            //if (!OtherConfigs.externedTheList)
//            if (!KeepOrigin)
//            {
//                var parent = Parent.Parent.Parent;
//                if (parent != null)
//                {
//                    if (parent is UIList list)
//                    {
//                        if (list != null)
//                        {
//                            CoolerItemVisualEffectMod.currentList = list;
//                            list.ListPadding = 16;
//                            list.Recalculate();
//                            list.RecalculateChildren();
//                            //OtherConfigs.externedTheList = true;
//                        }
//                    }
//                }
//            }

//            #endregion
//            #region 框框和label的绘制
//            HoverCounter += IsMouseHovering ? 1 : -1;
//            HoverCounter = (int)MathHelper.Clamp(HoverCounter, 0, 15);
//            CalculatedStyle dimensions = GetDimensions();
//            float width = dimensions.Width - 1f;
//            Color baseColor = (IsMouseHovering ? Color.White : Color.White * .9f);
//            if (!MemberInfo.CanWrite)
//            {
//                baseColor = Color.Gray;
//            }
//            Color mainColor = BackgroundColorAttribute.Color;
//            ConfigTexStyle configTexStyle = currentStyle;
//            float factor = HoverCounter / 15f;
//            Color color = (mainColor * MathHelper.SmoothStep(0.705f, 1f, factor)) with { A = mainColor.A };
//            var rect = dimensions.ToRectangle();
//            CoolerPanelInfo panelInfo = new CoolerPanelInfo() { configTexStyle = configTexStyle };
//            panelInfo.destination = rect;
//            panelInfo.scaler = Scaler;
//            panelInfo.offset = OffsetPosition;
//            panelInfo.glowEffectColor = KeepOrigin ? color : ModifyGlowColor(color);
//            panelInfo.glowShakingStrength = GlowShakingStrength;
//            panelInfo.glowHueOffsetRange = 0.2f;
//            panelInfo.backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/HotbarRadial_1").Value;
//            panelInfo.backgroundFrame = new Rectangle(4, 4, 28, 28);
//            panelInfo.backgroundUnitSize = new Vector2(28, 28) * 2f;
//            panelInfo.backgroundColor = ModifyGlowColor(color) with { A = 0 };
//            panelInfo.xBorderCount = 3;
//            rect = panelInfo.DrawComplexPanel(spriteBatch);
//            //DrawCoolerPanel(spriteBatch, ref rect, KeepOrigin ? color : ModifyGlowColor(color), Scaler, OffsetPosition, GlowShakingStrength, configTexStyle);
//            if (DrawLabel)
//            {
//                var position = rect.TopLeft() + new Vector2(32) * Scaler;
//                var str = TextDisplayFunction();

//                DrawCoolerColorCodedStringWithShadow(spriteBatch, currentStyleTex, FontAssets.ItemStack.Value, str, position, Color.Lerp(mainColor, Color.White, 0.5f), baseColor, 0f, Vector2.Zero, Vector2.One * Scaler, configTexStyle, width);
//                //DrawCoolerColorCodedStringWithShadow(spriteBatch, currentStyleTex, FontAssets.ItemStack.Value, TextDisplayFunction(), new Vector2(20, position.Y), Color.White, baseColor, 0f, Vector2.Zero, Vector2.One, configTexStyle, width);
//                //ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, TextDisplayFunction(), new Vector2(20, 260), baseColor, 0, default, Vector2.One);
//            }
//            #endregion
//            #region Tooltip
//            if (IsMouseHovering && TooltipFunction != null)//由于很迫真的原因，只能通过反射赋值了（感谢紫幽，这里可以直接赋值了\紫幽/\紫幽/\紫幽/\紫幽/\紫幽/\紫幽/\紫幽/
//            {
//                UIModConfig.Tooltip = TooltipFunction();
//                //Type uimodconfigType = UIModConfigType;
//                //if (uimodconfigType != null)
//                //{
//                //    var prop = uimodconfigType.GetProperty("Tooltip", BindingFlags.Static | BindingFlags.Public);
//                //    if (prop != null)
//                //    {
//                //        var str = TooltipFunction();
//                //        prop.SetValue(null, str);
//                //    }
//                //    else
//                //    {
//                //        _ = 0;
//                //    }
//                //}
//                //else
//                //{
//                //    Vector2 texVec = new Vector2(dimensions.X + dimensions.Width - 72, dimensions.Y + dimensions.Height * .5f);
//                //    ChatManager.DrawColorCodedString(spriteBatch, FontAssets.ItemStack.Value, typeof(ConfigElement).Assembly.FullName, texVec - new Vector2(384, 0), Color.White, 0f, Vector2.Zero, new Vector2(0.8f));
//                //}
//            }
//            #endregion
//        }
//    }
//    public class CoolerConfigElement<T> : CoolerConfigElement
//    {
//        protected virtual T Value
//        {
//            get => (T)GetObject();
//            set => SetObject(value);
//        }
//    }
//    public class CoolerBoolElement : CoolerConfigElement<bool>
//    {
//        private Asset<Texture2D> activeTex;
//        private Asset<Texture2D> containerTex;
//        public int ActiveCounter
//        {
//            get => timers[1];
//            set => timers[1] = value;
//        }
//        public float Factor => MathHelper.SmoothStep(0, 1, ActiveCounter / 15f);
//        public override Color ModifyGlowColor(Color color) => color with { A = 0 } * Factor;
//        public override float GlowShakingStrength => Factor;
//        public override Vector2 OverrideOffsetPosition => base.OverrideOffsetPosition;
//        public override float OverrideScaler => base.OverrideScaler;
//        public override void OnBind()
//        {
//            base.OnBind();
//            timers = new int[2];
//            activeTex = ModContent.Request<Texture2D>("CoolerItemVisualEffect/ConfigTex/Active");
//            containerTex = ModContent.Request<Texture2D>("CoolerItemVisualEffect/ConfigTex/Container");
//            OnLeftClick += (ev, v) => Value = !Value;
//        }
//        public override void DrawSelf(SpriteBatch spriteBatch)
//        {
//            base.DrawSelf(spriteBatch);

//            if (KeepOrigin)
//            {
//                var _toggleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle");
//                CalculatedStyle dimensions = GetDimensions();
//                // "Yes" and "No" since no "True" and "False" translation available
//                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Value ? Lang.menu[126].Value : Lang.menu[124].Value, new Vector2(dimensions.X + dimensions.Width - 60, dimensions.Y + 8f), Color.White, 0f, Vector2.Zero, new Vector2(0.8f));
//                Rectangle sourceRectangle = new Rectangle(Value ? ((_toggleTexture.Width() - 2) / 2 + 2) : 0, 0, (_toggleTexture.Width() - 2) / 2, _toggleTexture.Height());
//                Vector2 drawPosition = new Vector2(dimensions.X + dimensions.Width - sourceRectangle.Width - 10f, dimensions.Y + 8f);
//                spriteBatch.Draw(_toggleTexture.Value, drawPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
//            }
//            else
//            {
//                CalculatedStyle dimensions = GetDimensions();
//                Rectangle rectangle = dimensions.ToRectangle();
//                rectangle.Offset(OffsetPosition.ToPoint());
//                float scaler = Scaler;
//                rectangle = Utils.CenteredRectangle(rectangle.Center(), rectangle.Size() * scaler);
//                Color mainColor = BackgroundColorAttribute.Color;
//                #region 开关文本绘制
//                //CalculatedStyle dimensions = base.GetDimensions();
//                ActiveCounter += Value ? 1 : -1;
//                ActiveCounter = (int)MathHelper.Clamp(ActiveCounter, 0, 15);
//                var font = FontAssets.ItemStack.Value;
//                var text = Value ? Lang.menu[126].Value : Lang.menu[124].Value;
//                Vector2 texVec = new Vector2(rectangle.X + rectangle.Width - 80 * scaler, rectangle.Y + rectangle.Height * .5f);
//                float factor = Factor;
//                Vector2 drawPosition = new Vector2(rectangle.X + rectangle.Width - 32f * scaler, rectangle.Y + rectangle.Height * .5f);
//                float angle = 0f;
//                //var resultVec = ChatManager.DrawColorCodedString(spriteBatch, font, text, texVec, Color.Transparent, angle, Vector2.Zero, new Vector2(0.8f) * scaler);
//                {
//                    CoolerPanelInfo panelInfo = new CoolerPanelInfo();
//                    var size = font.MeasureString(text);
//                    var rect = new Rectangle(0, 0, (int)size.X, (int)size.Y);
//                    rect = Terraria.Utils.CenteredRectangle(rect.Center() + new Vector2(4, 0), rect.Size() + new Vector2(24, 16));
//                    panelInfo.destination = rect;
//                    panelInfo.xBorderCount = 1;
//                    panelInfo.yBorderCount = 1;
//                    panelInfo.scaler = Vector2.Dot(scaler * new Vector2(0.8f), Vector2.One) * .5f;
//                    panelInfo.offset = texVec;
//                    panelInfo.configTexStyle = currentStyle;
//                    panelInfo.backgroundColor = Color.White;
//                    panelInfo.backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/HotbarRadial_1").Value;
//                    panelInfo.backgroundFrame = new Rectangle(4, 4, 28, 28);
//                    panelInfo.backgroundUnitSize = new Vector2(28, 28) * 2f;
//                    panelInfo.backgroundColor = Color.Lerp(mainColor, Color.White, 0.5f);
//                    rect = panelInfo.DrawComplexPanel(spriteBatch);
//                }
//                //DrawCoolerTextBox(spriteBatch, currentStyleTex, font, text, texVec, Color.White, angle, default, scaler * new Vector2(0.8f));
//                if (ActiveCounter != 0)
//                {
//                    for (int n = 0; n < 4; n++)
//                        ChatManager.DrawColorCodedString(spriteBatch, font, text, texVec + Main.rand.NextFloat(0, factor * 4) * Main.rand.NextVector2Unit(), mainColor with { A = 0 } * .25f * factor, angle, Vector2.Zero, Vector2.One * scaler);
//                }
//                if (factor != 1)
//                {
//                    ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, texVec, Color.Black * (1 - factor), angle, Vector2.Zero, Vector2.One * scaler);
//                }
//                ChatManager.DrawColorCodedString(spriteBatch, font, text, texVec, Color.White, angle, Vector2.Zero, Vector2.One * scaler);
//                ////ChatManager.DrawColorCodedString(spriteBatch, FontAssets.ItemStack.Value, flag.ToString(), texVec - new Vector2(96, 0), Color.White, 0f, Vector2.Zero, new Vector2(0.8f));

//                #endregion
//                #region 开关特效绘制
//                if (currentStyleTex != null)
//                {
//                    spriteBatch.Draw(currentStyleTex, drawPosition, new Rectangle(84, 0, 40, 40), Color.White, 0, new Vector2(20), .8f * scaler, 0, 0);
//                }
//                spriteBatch.Draw(containerTex.Value, drawPosition, null, Color.White with { A = 0 }, Main.GlobalTimeWrappedHourly * MathHelper.Pi, containerTex.Size() * .5f, 128 / 3 / 236f * scaler, SpriteEffects.None, 0f);
//                var vec = new Vector2(1, 0);
//                for (int n = 0; n < 4; n++)
//                {
//                    spriteBatch.Draw(containerTex.Value, drawPosition + vec, null, mainColor with { A = 0 }, Main.GlobalTimeWrappedHourly * MathHelper.Pi, containerTex.Size() * .5f, 128 / 3 / 236f * scaler, SpriteEffects.None, 0f);
//                    vec = new Vector2(-vec.Y, vec.X);
//                }
//                if (ActiveCounter != 0)
//                {
//                    spriteBatch.Draw(activeTex.Value, drawPosition, null, Color.White with { A = 0 } * factor, -Main.GlobalTimeWrappedHourly * MathHelper.TwoPi, activeTex.Size() * .5f, 64 / 400f * scaler, SpriteEffects.None, 0f);
//                    spriteBatch.Draw(activeTex.Value, drawPosition, null, mainColor with { A = 0 } * factor, -Main.GlobalTimeWrappedHourly * MathHelper.Pi * 3, activeTex.Size() * .5f, 56 / 400f * scaler, SpriteEffects.None, 0f);
//                }
//                #endregion
//            }

//            //spriteBatch.Draw(TextureAssets.Item[4956].Value, new Vector2(960, drawPosition.Y) + (Main.GlobalTimeWrappedHourly).ToRotationVector2() * 256, Color.White);
//            //Parent.OverflowHidden = false;
//        }
//    }
//    public class CoolerEnumElement : CoolerConfigElement
//    {
//        private string[] valueStrings;
//        private int max;
//        private float maxWidth;
//        private int Value
//        {
//            get
//            {
//                int result = Array.IndexOf(Enum.GetValues(MemberInfo.Type), MemberInfo.GetValue(Item)); ;
//                return result;
//            }
//            set
//            {
//                if (!MemberInfo.CanWrite)
//                    return;
//                value %= max;
//                if (value < 0) value += max;
//                //int current = Value;
//                //int hash = GetHashCode();
//                MemberInfo.SetValue(Item, Enum.GetValues(MemberInfo.Type).GetValue(value));
//                //current= Value;
//                //_ = 0;
//                Interface.modConfig.SetPendingChanges();
//            }
//        }
//        public override void OnBind()
//        {
//            base.OnBind();
//            Height.Set(KeepOrigin ? 80 : 160, 0);
//            timers = new int[2];
//            #region 获取所有枚举名
//            valueStrings = Enum.GetNames(MemberInfo.Type);
//            var font = FontAssets.ItemStack.Value;
//            max = valueStrings.Length;

//            for (int i = 0; i < valueStrings.Length; i++)
//            {
//                var enumFieldMemberInfo = MemberInfo.Type.GetMember(valueStrings[i]).FirstOrDefault();
//                if (enumFieldMemberInfo != null)
//                {
//                    valueStrings[i] = ((LabelAttribute)Attribute.GetCustomAttribute(enumFieldMemberInfo, typeof(LabelAttribute)))?.Label ?? valueStrings[i];
//                }
//                var width = font.MeasureString(valueStrings[i]).X;
//                if (width > maxWidth) maxWidth = width;
//            }
//            if (!KeepOrigin)
//            {
//                var currentLeft = GetDimensions().X;
//                var totalWidth = maxWidth + 192 + FontAssets.ItemStack.Value.MeasureString(TextDisplayFunction()).X;
//                if (totalWidth > 530)
//                {
//                    MaxWidth.Set(totalWidth, 0);
//                    Width.Set(totalWidth, 0);
//                    Left.Set(currentLeft - totalWidth + 530 + 20, 0);
//                }
//            }
//            #endregion
//            //OnClick += (ev, v) =>
//            //{
//            //    Value++;
//            //    timers[1] += 15;
//            //};
//            //OnRightClick += (ev, v) =>
//            //{
//            //    Value--;
//            //    timers[1] -= 15;
//            //};
//            OnUpdate += (a) =>
//            {
//                if (timers[1] > 0) timers[1]--;
//                if (timers[1] < 0) timers[1]++;
//                if (IsMouseHovering)
//                {
//                    if (PlayerInput.Triggers.Current.MouseLeft && timers[1] <= 0)
//                    {
//                        Value++;
//                        timers[1] += 15;
//                    }
//                    if (PlayerInput.Triggers.Current.MouseRight && timers[1] >= 0)
//                    {
//                        Value--;
//                        timers[1] -= 15;
//                    }
//                }

//            };
//            //OnMouseDown += (ev, v) =>
//            //{
//            //    Value++;
//            //    timers[1] += 15;
//            //};
//        }

//        public override void DrawSelf(SpriteBatch spriteBatch)
//        {
//            base.DrawSelf(spriteBatch);
//            CalculatedStyle dimensions = GetDimensions();
//            Rectangle rectangle = dimensions.ToRectangle();
//            rectangle.Offset(OffsetPosition.ToPoint());
//            float scaler = Scaler;
//            rectangle = Utils.CenteredRectangle(rectangle.Center(), rectangle.Size() * scaler);
//            var font = FontAssets.ItemStack.Value;
//            Vector2 texVec = new Vector2(rectangle.X + rectangle.Width + (-maxWidth - 64) * scaler, rectangle.Y + rectangle.Height * .5f);
//            var texture = currentStyleTex;
//            var style = currentStyle;
//            #region 底
//            var boxVec = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height * .5f) - new Vector2(84 + maxWidth, KeepOrigin ? 32 : 64) * scaler;
//            var mainColor = BackgroundColorAttribute.Color;
//            Rectangle _rect = new Rectangle((int)boxVec.X, (int)boxVec.Y, (int)maxWidth + 40, KeepOrigin ? 64 : 128);
//            _rect.Offset((_rect.Size() * (scaler - 1) * .5f).ToPoint());
//            DrawCoolerPanel(spriteBatch, ref _rect, Main.Assets.Request<Texture2D>("Images/UI/HotbarRadial_1").Value, new Rectangle(4, 4, 28, 28), new Vector2(28, 28), mainColor with { A = 0 } * (.5f * ((scaler - 1) * 4 + 1)), currentStyle == 0 ? mainColor : Color.White, scaler, default, 0, currentStyle);
//            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, boxVec, new Rectangle(0, 0, 1, 1), Color.Red, 0, new Vector2(.5f), 4f, 0, 0);
//            #endregion
//            for (int n = -2; n < 3; n++)
//            {
//                float factor = n + timers[1] / 15f;
//                //factor %= 5;
//                //if (factor < 0) factor += 5;
//                //factor -= 2;
//                if (factor > 2 || factor < -2) continue;
//                var offsetY = MathF.Sin(factor / 2 * MathHelper.PiOver2);
//                var index = Value + n;
//                index %= max;
//                if (index < 0) index += max;
//                var scale = MathF.Sqrt(1 - offsetY * offsetY);


//                var position = texVec + offsetY * 72 * Vector2.UnitY * scaler * (KeepOrigin ? .5f : 1f);
//                var text = valueStrings[index];
//                var scalesqr = scale * scale;
//                if (texture != null)
//                {
//                    var size = new Vector2(maxWidth, font.MeasureString(text).Y);
//                    Vector2 _boxScaler = (size) / new Vector2(16, 10) * scaler * new Vector2(1f, scalesqr);
//                    var color = mainColor with { A = 0 } * scalesqr;
//                    spriteBatch.Draw(texture, position, new Rectangle(182, 0, 16, 40), color, 0, new Vector2(0, 13.5f), _boxScaler * new Vector2(0.875f, 1f), 0, 0);
//                    spriteBatch.Draw(texture, position, new Rectangle(172, 0, 14, 40), color, 0, new Vector2(14, 13.5f), new Vector2(MathF.Sqrt(_boxScaler.X), _boxScaler.Y), 0, 0);
//                    spriteBatch.Draw(texture, position, new Rectangle(198, 0, 10, 40), color, 0, new Vector2(-8 * MathF.Sqrt(_boxScaler.X), 13.5f), new Vector2(MathF.Sqrt(_boxScaler.X) * 1.75f, _boxScaler.Y), 0, 0);
//                }


//                DrawCoolerColorCodedStringWithShadow(spriteBatch, texture, font, text, position, Color.Lerp(mainColor, Color.White, 0.5f) * scalesqr, Color.White * scalesqr, 0, default, new Vector2(.8f) * scaler * scale, style);
//                //spriteBatch.Draw(TextureAssets.MagicPixel.Value, texVec + offsetY * 72 * Vector2.UnitY * scaler, new Rectangle(0, 0, 1, 1), Color.Red, 0, new Vector2(.5f), 4f, 0, 0);
//            }
//            if (MemberInfo.Type.Equals(typeof(ConfigTexStyle)))
//            {
//                currentStyle = (ConfigTexStyle)Value;
//            }
//            //DrawCoolerColorCodedStringWithShadow(spriteBatch, texture, font, (currentStyle, (Interface.modConfig.pendingConfig as ConfigurationSwoosh ?? ConfigSwooshInstance).otherConfigs.texStyle).ToString(), texVec + new Vector2(256 * scaler, 0), Color.White, Color.White, 0, default, Vector2.One * scaler, style); ;

//        }
//    }
//    public abstract class CoolerRangeElement : CoolerConfigElement
//    {
//        private static CoolerRangeElement rightLock;
//        private static CoolerRangeElement rightHover;

//        protected Color SliderColor { get; set; } = Color.White;
//        protected Utils.ColorLerpMethod ColorMethod { get; set; }

//        internal bool DrawTicks { get; set; }

//        public abstract int NumberTicks { get; }
//        public abstract float TickIncrement { get; }

//        protected abstract float Proportion { get; set; }

//        public CoolerRangeElement()
//        {
//            ColorMethod = new Utils.ColorLerpMethod((percent) => Color.Lerp(Color.Black, SliderColor, percent));
//        }

//        public override void OnBind()
//        {
//            base.OnBind();

//            DrawTicks = Attribute.IsDefined(MemberInfo.MemberInfo, typeof(DrawTicksAttribute));
//            SliderColor = ConfigManager.GetCustomAttributeFromMemberThenMemberType<SliderColorAttribute>(MemberInfo, Item, List)?.Color ?? BackgroundColorAttribute.Color;
//            //ConfigManager.GetCustomAttributeFromMemberThenMemberType
//        }

//        public float DrawValueBar(SpriteBatch sb, float scale, float perc, int lockState = 0, Utils.ColorLerpMethod colorMethod = null)
//        {
//            perc = Utils.Clamp(perc, -.05f, 1.05f);

//            var color = BackgroundColorAttribute.Color;
//            if (colorMethod != null) color = colorMethod(perc);

//            if (colorMethod == null)
//                colorMethod = new Utils.ColorLerpMethod(Utils.ColorLerp_BlackToWhite);

//            Texture2D colorBarTexture = TextureAssets.ColorBar.Value;
//            Vector2 vector = new Vector2((float)colorBarTexture.Width, (float)colorBarTexture.Height) * scale;
//            IngameOptions.valuePosition.X -= (float)((int)vector.X);
//            var rect = new Rectangle((int)IngameOptions.valuePosition.X - 8, (int)(IngameOptions.valuePosition.Y - 8 - vector.Y * .5f), (int)vector.X + 16, (int)vector.Y + 16);

//            if (!KeepOrigin)
//                DrawCoolerPanel(sb, ref rect, Main.Assets.Request<Texture2D>("Images/UI/HotbarRadial_1").Value, new Rectangle(4, 4, 28, 28), new Vector2(28), color with { A = 0 } * .5f, color with { A = 0 }, 1, default, perc, currentStyle);

//            Rectangle rectangle = new Rectangle((int)IngameOptions.valuePosition.X, (int)IngameOptions.valuePosition.Y - (int)vector.Y / 2, (int)vector.X, (int)vector.Y);
//            Rectangle destinationRectangle = rectangle;
//            int num = 167;
//            float num2 = rectangle.X + 5f * scale;
//            float num3 = rectangle.Y + 4f * scale;

//            if (DrawTicks)
//            {
//                int numTicks = NumberTicks;
//                if (numTicks > 1)
//                {
//                    for (int tick = 0; tick < numTicks; tick++)
//                    {
//                        float percent = tick * TickIncrement;

//                        if (percent <= 1f)
//                            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(num2 + num * percent * scale), rectangle.Y - 2, 2, rectangle.Height + 4), Color.White);
//                    }
//                }
//            }

//            sb.Draw(colorBarTexture, rectangle, Color.White);

//            for (float num4 = 0f; num4 < (float)num; num4 += 1f)
//            {
//                float percent = num4 / (float)num;
//                sb.Draw(TextureAssets.ColorBlip.Value, new Vector2(num2 + num4 * scale, num3), null, colorMethod(percent), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
//            }

//            rectangle.Inflate((int)(-5f * scale), 2);

//            //rectangle.X = (int)num2;
//            //rectangle.Y = (int)num3;

//            bool flag = rectangle.Contains(new Point(Main.mouseX, Main.mouseY));

//            if (lockState == 2)
//            {
//                flag = false;
//            }

//            if (flag || lockState == 1)
//            {
//                sb.Draw(TextureAssets.ColorHighlight.Value, destinationRectangle, Main.OurFavoriteColor);
//            }

//            var colorSlider = TextureAssets.ColorSlider.Value;

//            sb.Draw(colorSlider, new Vector2(num2 + 167f * scale * perc, num3 + 4f * scale), null, Color.White, 0f, colorSlider.Size() * 0.5f, scale, SpriteEffects.None, 0f);


//            if (Main.mouseX >= rectangle.X && Main.mouseX <= rectangle.X + rectangle.Width)
//            {
//                IngameOptions.inBar = flag;
//                return (Main.mouseX - rectangle.X) / (float)rectangle.Width;
//            }

//            IngameOptions.inBar = false;

//            if (rectangle.X >= Main.mouseX)
//            {
//                return 0f;
//            }

//            return 1f;
//        }

//        public override void DrawSelf(SpriteBatch spriteBatch)
//        {
//            base.DrawSelf(spriteBatch);
//            float num = 6f;
//            int num2 = 0;

//            rightHover = null;

//            if (!Main.mouseLeft)
//            {
//                rightLock = null;
//            }

//            if (rightLock == this)
//            {
//                num2 = 1;
//            }
//            else if (rightLock != null)
//            {
//                num2 = 2;
//            }

//            CalculatedStyle dimensions = GetDimensions();
//            float num3 = dimensions.Width + 1f;
//            Vector2 vector = new Vector2(dimensions.X, dimensions.Y);
//            bool flag2 = IsMouseHovering;
//            //spriteBatch.DrawString(FontAssets.MouseText.Value, TextDisplayFunction(), dimensions.ToRectangle().TopLeft() + Main.rand.NextVector2Unit(), Color.Red);

//            if (num2 == 1)
//            {
//                flag2 = true;
//            }

//            if (num2 == 2)
//            {
//                flag2 = false;
//            }

//            Vector2 vector2 = vector;
//            vector2.X += 8f;
//            vector2.Y += 2f + num;
//            vector2.X -= 17f;
//            //TextureAssets.ColorBar.Value.Frame(1, 1, 0, 0);
//            vector2 = new Vector2(dimensions.X + dimensions.Width - 32f, dimensions.Y + dimensions.Height * .5f);
//            IngameOptions.valuePosition = vector2;
//            float obj = DrawValueBar(spriteBatch, 1f, Proportion, num2, ColorMethod);

//            if (IngameOptions.inBar || rightLock == this)
//            {
//                rightHover = this;
//                if (PlayerInput.Triggers.Current.MouseLeft && rightLock == this)
//                {
//                    Proportion = obj;
//                }
//            }

//            if (rightHover != null && rightLock == null && PlayerInput.Triggers.JustPressed.MouseLeft)
//            {
//                rightLock = rightHover;
//            }
//        }
//    }
//    public abstract class CoolerPrimitiveRangeElement<T> : CoolerRangeElement where T : IComparable<T>
//    {
//        public T Min { get; set; }
//        public T Max { get; set; }
//        public T Increment { get; set; }
//        public IList<T> TList { get; set; }

//        public override void OnBind()
//        {
//            base.OnBind();

//            TList = (IList<T>)List;

//            TextDisplayFunction = () => MemberInfo.Name + ": " + GetValue();
//            if (Label != null)
//            {
//                TextDisplayFunction = () => Label + ": " + GetValue();
//            }

//            if (TList != null)
//            {
//                TextDisplayFunction = () => Index + 1 + ": " + TList[Index];
//            }



//            if (RangeAttribute != null && RangeAttribute.Min is T && RangeAttribute.Max is T)
//            {
//                Min = (T)RangeAttribute.Min;
//                Max = (T)RangeAttribute.Max;
//            }
//            if (IncrementAttribute != null && IncrementAttribute.Increment is T)
//            {
//                Increment = (T)IncrementAttribute.Increment;
//            }
//        }

//        protected virtual T GetValue() => (T)GetObject();

//        protected virtual void SetValue(object value)
//        {
//            if (value is T t)
//                SetObject(Utils.Clamp(t, Min, Max));
//        }
//    }
//    public class CoolerFloatElement : CoolerPrimitiveRangeElement<float>
//    {
//        public override int NumberTicks => (int)((Max - Min) / Increment) + 1;
//        public override float TickIncrement => (Increment) / (Max - Min);

//        protected override float Proportion
//        {
//            get => Utils.GetLerpValue(Min, Max, GetValue());
//            set => SetValue((float)Math.Round((value * (Max - Min) + Min) * (1 / Increment)) * Increment);
//        }

//        public CoolerFloatElement()
//        {
//            Min = 0;
//            Max = 1;
//            Increment = 0.01f;
//        }
//    }
//    public class CoolerIntElement : CoolerPrimitiveRangeElement<int>
//    {
//        public override int NumberTicks => (int)((Max - Min) / Increment) + 1;
//        public override float TickIncrement => (Increment) / (Max - Min);

//        protected override float Proportion
//        {
//            get => Utils.GetLerpValue(Min, Max, GetValue());//(GetValue() - Min) / (Max - Min)
//            set => SetValue((int)(Math.Round(MathHelper.Lerp(Min, Max, value) / Increment) * Increment));
//        }

//        public CoolerIntElement()
//        {
//            Min = 0;
//            Max = 1;
//            Increment = 1;
//        }
//    }
//    public abstract class CoolerCollectionElement : CoolerConfigElement
//    {
//        /// <summary>
//        /// 优化版的WrapIt函数
//        /// 支持自己抄写再魔改的<see cref="CollectionElement">
//        /// 
//        /// </summary>
//        /// <param name="parent"></param>
//        /// <param name="top"></param>
//        /// <param name="memberInfo"></param>
//        /// <param name="item"></param>
//        /// <param name="order"></param>
//        /// <param name="list"></param>
//        /// <param name="arrayType"></param>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        public static Tuple<UIElement, UIElement> WrapIt(UIElement parent, ref int top, PropertyFieldWrapper memberInfo, object item, int order, object list = null, Type arrayType = null, int index = -1)
//        {
//            int elementHeight;
//            Type type = memberInfo.Type;

//            if (arrayType != null)
//            {
//                type = arrayType;
//            }

//            UIElement e;

//            // TODO: Other common structs? -- Rectangle, Point
//            var customUI = ConfigManager.GetCustomAttributeFromMemberThenMemberType<CustomModConfigItemAttribute>(memberInfo, null, null);
//            var customElementUI = ConfigManager.GetCustomAttributeFromMemberThenMemberType<CollectionElementCustomItemAttribute>(memberInfo, null, null);

//            if (arrayType != null && customElementUI != null)
//            {
//                Type customUIType = customElementUI.Type;
//                if (typeof(ConfigElement).IsAssignableFrom(customUIType))
//                {
//                    ConstructorInfo ctor = customUIType.GetConstructor(Array.Empty<Type>());

//                    if (ctor != null)
//                    {
//                        object instance = ctor.Invoke(new object[0]);
//                        e = instance as UIElement;
//                    }
//                    else
//                    {
//                        e = new UIText($"{customUIType.Name} specified via CustomModConfigItem for {memberInfo.Name} does not have an empty constructor.");
//                    }
//                }
//                else
//                {
//                    e = new UIText($"{customUIType.Name} specified via CustomModConfigItem for {memberInfo.Name} does not inherit from ConfigElement.");
//                }
//            }
//            else if (customUI != null && arrayType == null)
//            {
//                Type customUIType = customUI.Type;

//                if (typeof(ConfigElement).IsAssignableFrom(customUIType))
//                {
//                    ConstructorInfo ctor = customUIType.GetConstructor(Array.Empty<Type>());

//                    if (ctor != null)
//                    {
//                        object instance = ctor.Invoke(new object[0]);
//                        e = instance as UIElement;
//                    }
//                    else
//                    {
//                        e = new UIText($"{customUIType.Name} specified via CustomModConfigItem for {memberInfo.Name} does not have an empty constructor.");
//                    }
//                }
//                else
//                {
//                    e = new UIText($"{customUIType.Name} specified via CustomModConfigItem for {memberInfo.Name} does not inherit from ConfigElement.");
//                }
//            }
//            else if (item.GetType() == typeof(HeaderAttribute))
//            {
//                e = new HeaderElement((string)memberInfo.GetValue(item));
//            }
//            else if (type == typeof(ItemDefinition))
//            {
//                e = new ItemDefinitionElement();
//            }
//            else if (type == typeof(ProjectileDefinition))
//            {
//                e = new ProjectileDefinitionElement();
//            }
//            else if (type == typeof(NPCDefinition))
//            {
//                e = new NPCDefinitionElement();
//            }
//            else if (type == typeof(PrefixDefinition))
//            {
//                e = new PrefixDefinitionElement();
//            }
//            else if (type == typeof(Color))
//            {
//                e = new ColorElement();
//            }
//            else if (type == typeof(Vector2))
//            {
//                e = new Vector2Element();
//            }
//            else if (type == typeof(bool)) // isassignedfrom?
//            {
//                e = new BooleanElement();
//            }
//            else if (type == typeof(float))
//            {
//                e = new FloatElement();
//            }
//            else if (type == typeof(byte))
//            {
//                e = new ByteElement();
//            }
//            else if (type == typeof(uint))
//            {
//                e = new UIntElement();
//            }
//            else if (type == typeof(int))
//            {
//                SliderAttribute sliderAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<SliderAttribute>(memberInfo, item, list);

//                if (sliderAttribute != null)
//                    e = new IntRangeElement();
//                else
//                    e = new IntInputElement();
//            }
//            else if (type == typeof(string))
//            {
//                OptionStringsAttribute ost = ConfigManager.GetCustomAttributeFromMemberThenMemberType<OptionStringsAttribute>(memberInfo, item, list);
//                if (ost != null)
//                    e = new StringOptionElement();
//                else
//                    e = new StringInputElement();
//            }
//            else if (type.IsEnum)
//            {
//                if (list != null)
//                    e = new UIText($"{memberInfo.Name} not handled yet ({type.Name}).");
//                else
//                    e = new EnumElement();
//            }
//            else if (type.IsArray)
//            {
//                e = new ArrayElement();
//            }
//            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
//            {
//                e = new ListElement();
//            }
//            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
//            {
//                e = new SetElement();
//            }
//            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
//            {
//                e = new DictionaryElement();
//            }
//            else if (type.IsClass)
//            {
//                e = new ObjectElement(/*, ignoreSeparatePage: ignoreSeparatePage*/);
//            }
//            else if (type.IsValueType && !type.IsPrimitive)
//            {
//                e = new UIText($"{memberInfo.Name} not handled yet ({type.Name}) Structs need special UI.");
//                //e.Top.Pixels += 6;
//                e.Height.Pixels += 6;
//                e.Left.Pixels += 4;

//                //object subitem = memberInfo.GetValue(item);
//            }
//            else
//            {
//                e = new UIText($"{memberInfo.Name} not handled yet ({type.Name})");
//                e.Top.Pixels += 6;
//                e.Left.Pixels += 4;
//            }

//            if (e != null)
//            {
//                if (e is ConfigElement configElement)
//                {
//                    configElement.Bind(memberInfo, item, (IList)list, index);
//                    configElement.OnBind();
//                }

//                e.Recalculate();

//                elementHeight = (int)e.GetOuterDimensions().Height;

//                var container = UIModConfig.GetContainer(e, index == -1 ? order : index);
//                container.Height.Pixels = elementHeight;

//                if (parent is UIList uiList)
//                {
//                    uiList.Add(container);
//                    uiList.GetTotalHeight();
//                }
//                else
//                {
//                    // Only Vector2 and Color use this I think, but modders can use the non-UIList approach for custom UI and layout.
//                    container.Top.Pixels = top;
//                    container.Width.Pixels = -20;
//                    container.Left.Pixels = 20;
//                    top += elementHeight + 4;
//                    parent.Append(container);
//                    parent.Height.Set(top, 0);
//                }

//                var tuple = new Tuple<UIElement, UIElement>(container, e);

//                if (parent == Interface.modConfig.mainConfigList)
//                {
//                    Interface.modConfig.mainConfigItems.Add(tuple);
//                }

//                return tuple;
//            }
//            return null;
//        }
//        private CoolerImageButton initializeButton;//初始化按钮
//        private CoolerImageButton addButton;//增加项按钮
//        private CoolerImageButton deleteButton;//删除项按钮
//        private CoolerImageButton expandButton;//展开按钮
//        private CoolerImageButtonSplit upDownButton;//上下按钮
//        private bool expanded = true;
//        /// <summary>
//        /// <see cref="Update(GameTime)"/>处使用，如果发生更改就重新生成整个ui
//        /// </summary>
//        private bool pendingChanges = false;

//        protected object Data { get; set; }
//        protected UIElement DataListElement { get; set; }
//        protected NestedUIList DataList { get; set; }
//        protected float Scale { get; set; } = 1f;
//        protected DefaultListValueAttribute DefaultListValueAttribute { get; set; }
//        protected JsonDefaultListValueAttribute JsonDefaultListValueAttribute { get; set; }

//        protected virtual bool CanAdd => true;

//        public override void OnBind()
//        {
//            base.OnBind();
//            //{
//            //    var texture = base.CollapsedTexture.Value;
//            //    var name = "Collapsed";
//            //    int i = 0;
//            //    while (i < 6) 
//            //    {
//            //        FileStream fileStream = new FileStream($"E:/tmlUI/CoolerUI_{name}.png", FileMode.OpenOrCreate);
//            //        texture.SaveAsPng(fileStream, texture.Width, texture.Height);
//            //        fileStream.Dispose();
//            //        i++;
//            //        texture = (i switch
//            //        {
//            //            1 => base.DeleteTexture,
//            //            2 => base.ExpandedTexture,
//            //            3 => base.PlayTexture,
//            //            4 => base.PlusTexture,
//            //            5 or _ => base.UpDownTexture
//            //        }).Value;
//            //        name = i switch
//            //        {
//            //            1 => "Delete",
//            //            2 => "Expanded",
//            //            3 => "Play",
//            //            4 => "Plus",
//            //            5 or _ => "UpDown"
//            //        };
//            //    }

//            //}
//            var expandAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<ExpandAttribute>(MemberInfo, Item, List);//检查有没有展开特性，用来设置展开的默认值
//            if (expandAttribute != null)
//                expanded = expandAttribute.Expand;

//            Data = MemberInfo.GetValue(Item);//Member是Item的成员，通过Item实例获取成员的值
//            DefaultListValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<DefaultListValueAttribute>(MemberInfo, null, null);//获取链表默认值

//            MaxHeight.Set(300, 0f);//设置最大高度为300像素
//            DataListElement = new UIElement();//列表主体
//            DataListElement.Width.Set(-10f, 1f);
//            DataListElement.Left.Set(10f, 0f);
//            DataListElement.Height.Set(KeepOrigin ? -30 : -80, 1f);
//            DataListElement.Top.Set(KeepOrigin ? 30 : 80, 0f);

//            //panel.SetPadding(0);
//            //panel.BackgroundColor = Microsoft.Xna.Framework.Color.Transparent;
//            //panel.BorderColor =  Microsoft.Xna.Framework.Color.Transparent;

//            if (Data != null && expanded)
//                Append(DataListElement);

//            DataListElement.OverflowHidden = true;

//            DataList = new NestedUIList();//嵌在内部的列表
//            DataList.Width.Set(-20, 1f);
//            DataList.Left.Set(0, 0f);
//            DataList.Height.Set(0, 1f);
//            DataList.ListPadding = 5f;
//            DataListElement.Append(DataList);

//            UIScrollbar scrollbar = new UIScrollbar();//滑动条
//            scrollbar.SetView(100f, 1000f);
//            scrollbar.Height.Set(-16f, 1f);
//            scrollbar.Top.Set(6f, 0f);
//            scrollbar.Left.Pixels -= 3;
//            scrollbar.HAlign = 1f;
//            DataList.SetScrollbar(scrollbar);
//            DataListElement.Append(scrollbar);

//            PrepareTypes();//准备类型（？
//                           // allow null collections to simplify modder code for OnDeserialize and allow null and empty lists to have different meanings, etc.
//            SetupList();//准备列表

//            if (CanAdd)//是否可加，像数组就整不了喽(乐
//            {
//                //初始化按钮
//                initializeButton = new CoolerImageButton(PlayTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = "Initialize" };
//                initializeButton.Top.Pixels += 4;
//                initializeButton.Left.Pixels -= 3;
//                initializeButton.HAlign = 1f;
//                initializeButton.OnLeftClick += (a, b) =>
//                {
//                    SoundEngine.PlaySound(SoundID.Tink);
//                    InitializeCollection();
//                    SetupList();
//                    Interface.modConfig.RecalculateChildren(); // not needed?
//                    Interface.modConfig.SetPendingChanges();
//                    expanded = true;
//                    pendingChanges = true;
//                };
//                //添加元素按钮
//                addButton = new CoolerImageButton(PlusTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = "Add" };
//                addButton.Top.Set(20, 0f);
//                addButton.Left.Set(-52, 1f);
//                addButton.OnLeftClick += (a, b) =>
//                {
//                    SoundEngine.PlaySound(SoundID.Tink);
//                    AddItem();
//                    SetupList();
//                    Interface.modConfig.RecalculateChildren();
//                    Interface.modConfig.SetPendingChanges();
//                    expanded = true;
//                    pendingChanges = true;
//                };

//                deleteButton = new CoolerImageButton(DeleteTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = "Clear" };
//                deleteButton.Top.Set(47, 0f);
//                deleteButton.Left.Set(-52, 1f);
//                deleteButton.OnLeftClick += (a, b) =>
//                {
//                    SoundEngine.PlaySound(SoundID.Tink);
//                    if (NullAllowed)
//                        NullCollection();//使集合为空
//                    else
//                        ClearCollection();//清空集合
//                    SetupList();
//                    Interface.modConfig.RecalculateChildren();
//                    Interface.modConfig.SetPendingChanges();
//                    pendingChanges = true;
//                };
//            }

//            expandButton = new CoolerImageButton(expanded ? ExpandedTexture : CollapsedTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = expanded ? "Collapse" : "Expand" };
//            expandButton.Top.Set(47, 0f); // 10, -25: 4, -52
//            expandButton.Left.Set(-79, 1f);
//            expandButton.OnLeftClick += (a, b) =>
//            {
//                expanded = !expanded;
//                pendingChanges = true;
//            };

//            upDownButton = new CoolerImageButtonSplit(UpDownTexture, Color.White, BackgroundColorAttribute.Color) { HoverTextUp = "Scale Up", HoverTextDown = "Scale Down" };
//            upDownButton.Top.Set(20, 0f);
//            upDownButton.Left.Set(-79, 1f);
//            upDownButton.OnLeftClick += (a, b) =>
//            {
//                Rectangle r = b.GetDimensions().ToRectangle();

//                if (a.MousePosition.Y < r.Y + r.Height / 2)
//                {
//                    Scale = Math.Min(2f, Scale + 0.5f);
//                }
//                else
//                {
//                    Scale = Math.Max(1f, Scale - 0.5f);
//                }
//                //dataListPanel.RecalculateChildren();
//                ////dataList.RecalculateChildren();
//                //float h = dataList.GetTotalHeight();
//                //MinHeight.Set(Math.Min(Math.Max(h + 84, 100), 300) * scale, 0f);
//                //Recalculate();
//                //if (Parent != null && Parent is UISortableElement) {
//                //	Parent.Height.Pixels = GetOuterDimensions().Height;
//                //}
//            };
//            //Append(upButton);

//            //var aasdf = Texture2D.FromStream(Main.instance.GraphicsDevice, Assembly.GetExecutingAssembly().GetManifestResourceStream("Terraria.ModLoader.Config.UI.ButtonDecrement.png"));
//            //for (int i = 0; i < 100; i++) {
//            //	var vb = Texture2D.FromStream(Main.instance.GraphicsDevice, Assembly.GetExecutingAssembly().GetManifestResourceStream("Terraria.ModLoader.Config.UI.ButtonDecrement.png"));
//            //}

//            pendingChanges = true;
//            Recalculate(); // Needed?
//        }

//        protected object CreateCollectionElementInstance(Type type)
//        {
//            object toAdd;

//            if (DefaultListValueAttribute != null)
//            {
//                toAdd = DefaultListValueAttribute.Value;
//            }
//            else
//            {
//                toAdd = ConfigManager.AlternateCreateInstance(type);

//                if (!type.IsValueType && type != typeof(string))
//                {
//                    string json = JsonDefaultListValueAttribute?.Json ?? "{}";

//                    JsonConvert.PopulateObject(json, toAdd, ConfigManager.serializerSettings);
//                }
//            }

//            return toAdd;
//        }

//        // SetupList called in base.ctor, but children need Types.
//        protected abstract void PrepareTypes();

//        protected abstract void AddItem();

//        protected abstract void InitializeCollection();

//        protected virtual void NullCollection()
//        {
//            Data = null;
//            SetObject(Data);
//        }
//        protected abstract void ClearCollection();

//        protected abstract void SetupList();

//        public override void Update(GameTime gameTime)
//        {
//            base.Update(gameTime);
//            #region 重设挂起
//            if (!pendingChanges)
//                return;
//            pendingChanges = false;
//            #endregion

//            #region 移除已存元素
//            if (CanAdd)
//            {
//                RemoveChild(initializeButton);
//                RemoveChild(addButton);
//                RemoveChild(deleteButton);
//            }
//            RemoveChild(expandButton);
//            RemoveChild(upDownButton);
//            RemoveChild(DataListElement);
//            #endregion

//            #region 重新载入元素，重点是recalculate来重新排布(x
//            if (Data == null)
//            {
//                Append(initializeButton);
//            }
//            else
//            {
//                if (CanAdd)
//                {
//                    Append(addButton);
//                    Append(deleteButton);
//                }
//                Append(expandButton);
//                if (expanded)
//                {
//                    Append(upDownButton);
//                    Append(DataListElement);
//                    expandButton.HoverText = "Collapse";
//                    expandButton.SetImage(ExpandedTexture);
//                }
//                else
//                {
//                    expandButton.HoverText = "Expand";
//                    expandButton.SetImage(CollapsedTexture);
//                }
//            }
//            #endregion
//        }

//        public override void Recalculate()
//        {
//            base.Recalculate();

//            float defaultHeight = KeepOrigin ? 30 : 80;
//            float h = DataListElement.Parent != null ? DataList.GetTotalHeight() + defaultHeight : defaultHeight; // 24 for UIElement

//            h = Utils.Clamp(h, defaultHeight, defaultHeight * 10 * Scale);

//            MaxHeight.Set(defaultHeight * 10 * Scale, 0f);
//            Height.Set(h, 0f);

//            if (Parent != null && Parent is UISortableElement)
//            {
//                Parent.Height.Set(h, 0f);
//            }
//        }
//    }
//    public class CoolerListElement : CoolerCollectionElement
//    {
//        private Type listType;

//        protected override void PrepareTypes()
//        {
//            listType = MemberInfo.Type.GetGenericArguments()[0];//获取列表类型
//            JsonDefaultListValueAttribute = ConfigManager.GetCustomAttributeFromCollectionMemberThenElementType<JsonDefaultListValueAttribute>(MemberInfo.MemberInfo, listType);
//        }

//        protected override void AddItem()
//        {
//            ((IList)Data).Add(CreateCollectionElementInstance(listType));//将该类型的默认值加入链表
//        }

//        protected override void InitializeCollection()
//        {
//            Data = Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
//            SetObject(Data);
//        }

//        protected override void ClearCollection()
//        {
//            ((IList)Data).Clear();
//        }

//        protected override void SetupList()
//        {
//            DataList.Clear();
//            int top = 0;

//            if (Data != null)//如果我们的列表不为空
//            {
//                for (int i = 0; i < ((IList)Data).Count; i++)//遍历列表的每一个元素
//                {
//                    int index = i;//↓               ui的父级  顶部高度？ 成员信息  config实例
//                    bool flag = listType == null;
//                    var wrapped = WrapIt(DataList, ref top, MemberInfo, Item, 0, Data, listType, index);//获取打包对，item1是容器♥，item2是内容

//                    wrapped.Item2.Left.Pixels += 80;
//                    wrapped.Item2.Width.Pixels -= 96;
//                    var (i1, i2) = (wrapped.Item1, wrapped.Item2);
//                    // Add delete button.
//                    CoolerImageButton deleteButton = new CoolerImageButton(DeleteTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = "移除Remove" };
//                    deleteButton.VAlign = 0.25f;
//                    deleteButton.HAlign = .025f;
//                    deleteButton.OnLeftClick += (a, b) => { ((IList)Data).RemoveAt(index); SetupList(); Interface.modConfig.SetPendingChanges(); };
//                    wrapped.Item1.Append(deleteButton);
//                    CoolerImageButton copyButton = new CoolerImageButton(CopyTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = "复制Copy" };
//                    copyButton.VAlign = 0.25f;
//                    copyButton.HAlign = .075f;
//                    copyButton.OnLeftClick += (a, b) => { var list = (IList)Data; list.Insert(index, list[index]); SetupList(); Interface.modConfig.SetPendingChanges(); };
//                    wrapped.Item1.Append(copyButton);

//                    CoolerImageButtonSplit upDownButton = new CoolerImageButtonSplit(UpDownTexture, Color.White, BackgroundColorAttribute.Color) { HoverTextUp = "与上方元素交换", HoverTextDown = "与下方元素交换" };
//                    upDownButton.VAlign = 0.75f;
//                    upDownButton.HAlign = .025f;
//                    upDownButton.OnLeftClick += (a, b) =>
//                    {
//                        Rectangle r = b.GetDimensions().ToRectangle();
//                        var list = (IList)Data;
//                        bool flag = false;
//                        bool up = false;
//                        if (a.MousePosition.Y < r.Y + r.Height / 2)
//                        {
//                            if (index != 0)
//                            {
//                                flag = true;
//                                up = true;
//                            }
//                        }
//                        else
//                        {
//                            if (index != list.Count - 1)
//                                flag = true;
//                        }
//                        if (flag)
//                        {
//                            var value = list[index];
//                            list.RemoveAt(index);
//                            list.Insert(index + (up ? -1 : 1), value);
//                            SoundEngine.PlaySound(SoundID.Unlock);
//                            SetupList(); Interface.modConfig.SetPendingChanges();
//                        }
//                        else
//                            SoundEngine.PlaySound(SoundID.Bird);
//                    };
//                    wrapped.Item1.Append(upDownButton);
//                    CoolerImageButtonSplit upDownPlusButton = new CoolerImageButtonSplit(PlusUpDownTexture, Color.White, BackgroundColorAttribute.Color) { HoverTextUp = "向上添加新元素", HoverTextDown = "向下添加新元素" };
//                    upDownPlusButton.VAlign = .75f;
//                    upDownPlusButton.HAlign = .075f;
//                    upDownPlusButton.OnLeftClick += (a, b) =>
//                    {
//                        Rectangle r = b.GetDimensions().ToRectangle();
//                        var _index = index;
//                        var list = (IList)Data;
//                        list.Insert(index + (a.MousePosition.Y >= r.Y + r.Height / 2 ? 1 : 0), CreateCollectionElementInstance(listType));
//                        SetupList(); Interface.modConfig.SetPendingChanges();
//                    };
//                    wrapped.Item1.Append(upDownPlusButton);
//                }
//            }
//        }

//    }
//    public class CoolerObjectElement : CoolerConfigElement<object>
//    {
//        protected Func<string> AbridgedTextDisplayFunction { get; set; }

//        private readonly bool ignoreSeparatePage;
//        //private SeparatePageAttribute separatePageAttribute;
//        //private object data;
//        private bool separatePage;
//        private bool pendingChanges;
//        private bool expanded = true;
//        private NestedUIList dataList;
//        private CoolerImageButton initializeButton;
//        private CoolerImageButton deleteButton;
//        private CoolerImageButton expandButton;
//        private UIPanel separatePagePanel;
//        private CoolerUITextPanel<FuncStringWrapper> separatePageButton;

//        // Label:
//        //  Members
//        //  Members
//        public CoolerObjectElement()
//        {
//        }

//        public override void OnBind()
//        {
//            base.OnBind();

//            if (List != null)
//            {
//                // TODO: only do this if ToString is overriden.

//                var listType = MemberInfo.Type.GetGenericArguments()[0];

//                System.Reflection.MethodInfo methodInfo = listType.GetMethod("ToString", Array.Empty<Type>());
//                bool hasToString = methodInfo != null && methodInfo.DeclaringType != typeof(object);

//                if (hasToString)
//                {
//                    TextDisplayFunction = () => Index + 1 + ": " + (List[Index]?.ToString() ?? "null");
//                    AbridgedTextDisplayFunction = () => (List[Index]?.ToString() ?? "null");
//                }
//                else
//                {
//                    TextDisplayFunction = () => Index + 1 + ": ";
//                }
//            }
//            else
//            {
//                bool hasToString = MemberInfo.Type.GetMethod("ToString", Array.Empty<Type>()).DeclaringType != typeof(object);

//                if (hasToString)
//                {
//                    TextDisplayFunction = () => Label + (Value == null ? "" : ": " + Value.ToString());
//                    AbridgedTextDisplayFunction = () => Value?.ToString() ?? "";
//                }
//            }

//            // Null values without AllowNullAttribute aren't allowed, but could happen with modder mistakes, so not automatically populating will hint to modder the issue.
//            if (Value == null && List != null)
//            {
//                // This should never actually happen, but I guess a bad Json file could.
//                object data = Activator.CreateInstance(MemberInfo.Type, true);
//                string json = JsonDefaultValueAttribute?.Json ?? "{}";

//                JsonConvert.PopulateObject(json, data, ConfigManager.serializerSettings);

//                Value = data;
//            }

//            separatePage = ConfigManager.GetCustomAttributeFromMemberThenMemberType<SeparatePageAttribute>(MemberInfo, Item, List) != null;

//            //separatePage = separatePage && !ignoreSeparatePage;
//            //separatePage = (SeparatePageAttribute)Attribute.GetCustomAttribute(memberInfo.MemberInfo, typeof(SeparatePageAttribute)) != null;

//            if (separatePage && !ignoreSeparatePage)
//            {
//                // TODO: UITextPanel doesn't update...
//                separatePageButton = new CoolerUITextPanel<FuncStringWrapper>(new FuncStringWrapper(TextDisplayFunction));
//                separatePageButton.HAlign = 0.5f;
//                //e.Recalculate();
//                //elementHeight = (int)e.GetOuterDimensions().Height;
//                separatePageButton.OnLeftClick += (a, c) =>
//                {
//                    UIModConfig.SwitchToSubConfig(this.separatePagePanel);
//                    /*	Interface.modConfig.uIElement.RemoveChild(Interface.modConfig.configPanelStack.Peek());
//                        Interface.modConfig.uIElement.Append(separateListPanel);
//                        Interface.modConfig.configPanelStack.Push(separateListPanel);*/
//                    //separateListPanel.SetScrollbar(Interface.modConfig.uIScrollbar);

//                    //UIPanel panel = new UIPanel();
//                    //panel.Width.Set(200, 0);
//                    //panel.Height.Set(200, 0);
//                    //panel.Left.Set(200, 0);
//                    //panel.Top.Set(200, 0);
//                    //Interface.modConfig.Append(panel);

//                    //Interface.modConfig.subMenu.Enqueue(subitem);
//                    //Interface.modConfig.DoMenuModeState();
//                };
//                //e = new UIText($"{memberInfo.Name} click for more ({type.Name}).");
//                //e.OnLeftClick += (a, b) => { };
//            }

//            //data = _GetValue();// memberInfo.GetValue(this.item);
//            //drawLabel = false;

//            if (List == null)
//            {
//                // Member > Class
//                var expandAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<ExpandAttribute>(MemberInfo, Item, List);
//                if (expandAttribute != null)
//                    expanded = expandAttribute.Expand;
//            }
//            else
//            {
//                // ListMember's ExpandListElements > Class
//                var listType = MemberInfo.Type.GetGenericArguments()[0];
//                var expandAttribute = (ExpandAttribute)Attribute.GetCustomAttribute(listType, typeof(ExpandAttribute), true);
//                if (expandAttribute != null)
//                    expanded = expandAttribute.Expand;
//                expandAttribute = (ExpandAttribute)Attribute.GetCustomAttribute(MemberInfo.MemberInfo, typeof(ExpandAttribute), true);
//                if (expandAttribute != null && expandAttribute.ExpandListElements.HasValue)
//                    expanded = expandAttribute.ExpandListElements.Value;
//            }

//            dataList = new NestedUIList();
//            dataList.Width.Set(-14, 1f);
//            dataList.Left.Set(14, 0f);
//            dataList.Height.Set(-30, 1f);
//            dataList.Top.Set(30, 0);
//            dataList.ListPadding = 5f;

//            if (expanded)
//                Append(dataList);

//            //string name = memberInfo.Name;
//            //if (labelAttribute != null) {
//            //	name = labelAttribute.Label;
//            //}
//            if (List == null)
//            {
//                // drawLabel = false; TODO uncomment
//            }

//            initializeButton = new CoolerImageButton(PlayTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = "Initialize" };
//            initializeButton.Top.Pixels += 4;
//            initializeButton.Left.Pixels -= 3;
//            initializeButton.HAlign = 1f;
//            initializeButton.OnLeftClick += (a, b) =>
//            {
//                SoundEngine.PlaySound(21);

//                object data = Activator.CreateInstance(MemberInfo.Type, true);
//                string json = JsonDefaultValueAttribute?.Json ?? "{}";

//                JsonConvert.PopulateObject(json, data, ConfigManager.serializerSettings);

//                Value = data;

//                //SeparatePageAttribute here?

//                pendingChanges = true;
//                //RemoveChild(initializeButton);
//                //Append(deleteButton);
//                //Append(expandButton);

//                SetupList();
//                Interface.modConfig.RecalculateChildren();
//                Interface.modConfig.SetPendingChanges();
//            };

//            expandButton = new CoolerImageButton(expanded ? ExpandedTexture : CollapsedTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = expanded ? "Collapse" : "Expand" };
//            expandButton.Top.Set(4, 0f); // 10, -25: 4, -52
//            expandButton.Left.Set(-52, 1f);
//            expandButton.OnLeftClick += (a, b) =>
//            {
//                expanded = !expanded;
//                pendingChanges = true;
//            };

//            deleteButton = new CoolerImageButton(DeleteTexture, Color.White, BackgroundColorAttribute.Color) { HoverText = "Clear" };
//            deleteButton.Top.Set(4, 0f);
//            deleteButton.Left.Set(-25, 1f);
//            deleteButton.OnLeftClick += (a, b) =>
//            {
//                Value = null;
//                pendingChanges = true;

//                SetupList();
//                //Interface.modConfig.RecalculateChildren();
//                Interface.modConfig.SetPendingChanges();
//            };

//            if (Value != null)
//            {
//                //Append(expandButton);
//                //Append(deleteButton);
//                SetupList();
//            }
//            else
//            {
//                Append(initializeButton);
//                //sortedContainer.Append(initializeButton);
//            }

//            pendingChanges = true;
//            Recalculate();
//        }

//        public override void Update(GameTime gameTime)
//        {
//            base.Update(gameTime);

//            if (!pendingChanges)
//                return;
//            pendingChanges = false;
//            DrawLabel = !separatePage || ignoreSeparatePage;

//            RemoveChild(deleteButton);
//            RemoveChild(expandButton);
//            RemoveChild(initializeButton);
//            RemoveChild(dataList);
//            if (separatePage && !ignoreSeparatePage)
//                RemoveChild(separatePageButton);
//            if (Value == null)
//            {
//                Append(initializeButton);
//                DrawLabel = true;
//            }
//            else
//            {
//                if (List == null && !(separatePage && ignoreSeparatePage) && NullAllowed)
//                    Append(deleteButton);

//                if (!separatePage || ignoreSeparatePage)
//                {
//                    if (!ignoreSeparatePage)
//                        Append(expandButton);
//                    if (expanded)
//                    {
//                        Append(dataList);
//                        expandButton.HoverText = "Collapse";
//                        expandButton.SetImage(ExpandedTexture);
//                    }
//                    else
//                    {
//                        RemoveChild(dataList);
//                        expandButton.HoverText = "Expand";
//                        expandButton.SetImage(CollapsedTexture);
//                    }
//                }
//                else
//                {
//                    Append(separatePageButton);
//                }
//            }
//        }

//        private void SetupList()
//        {
//            dataList.Clear();

//            object data = Value;

//            if (data != null)
//            {
//                if (separatePage && !ignoreSeparatePage)
//                {
//                    separatePagePanel = UIModConfig.MakeSeparateListPanel(Item, data, MemberInfo, List, Index, AbridgedTextDisplayFunction);
//                }
//                else
//                {
//                    int order = 0;
//                    foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(data))
//                    {
//                        if (Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)))
//                            continue;

//                        int top = 0;

//                        UIModConfig.HandleHeader(dataList, ref top, ref order, variable);

//                        var wrapped = UIModConfig.WrapIt(dataList, ref top, variable, data, order++);

//                        if (List != null)
//                        {
//                            //wrapped.Item1.Left.Pixels -= 20;
//                            wrapped.Item1.Width.Pixels += 20;
//                        }
//                        else
//                        {
//                            //wrapped.Item1.Left.Pixels += 20;
//                            //wrapped.Item1.Width.Pixels -= 20;
//                        }
//                    }
//                }
//            }
//        }

//        public override void Recalculate()
//        {
//            base.Recalculate();

//            float defaultHeight = separatePage ? 40 : 30;
//            float h = dataList.Parent != null ? dataList.GetTotalHeight() + defaultHeight : defaultHeight;

//            Height.Set(h, 0f);

//            if (Parent != null && Parent is UISortableElement)
//            {
//                Parent.Height.Set(h, 0f);
//            }
//        }
//    }
//}
