//using Microsoft.Xna.Framework.Graphics;
//using System;
//using Terraria.GameContent.UI;

//namespace CoolerItemVisualEffect.ItemInWorld
//{
//    public class ItemLight : GlobalItem
//    {
//        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
//        {
//            if (CoolerItemVisualEffect.Config.LightItem)
//            {
//                if (item.type != ItemID.Heart && item.type != ItemID.CopperCoin && item.type != ItemID.SilverCoin && item.type != ItemID.GoldCoin && item.type != ItemID.PlatinumCoin && item.type != ItemID.Star && item.type != ItemID.CandyCane && item.type != ItemID.SugarPlum && item.type != ItemID.NurseShirt && item.type != ItemID.SoulCake && item.type != ItemID.NebulaPickup1 && item.type != ItemID.NebulaPickup2 && item.type != ItemID.NebulaPickup3)
//                {
//                    int lightStyle = CoolerItemVisualEffect.Config.LightItemNum;
//                    Texture2D texture = ModContent.Request<Texture2D>("CoolerItemVisualEffect/ItemInWorld/Light" + $"{lightStyle}").Value;
//                    Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
//                    int rare = item.rare;
//                    Color baseColor = ItemRarity.GetColor(rare);
//                    float lightscale = (float)Math.Sqrt(Math.Sqrt(rare * rare)) / 5 + 0.4f;

//                    if (item.expert)
//                    {
//                        baseColor = Main.DiscoColor;
//                        lightscale = (float)Math.Sqrt(12) / 5 + 0.4f;
//                    }
//                    if (item.master)
//                    {
//                        baseColor = new Color(255, (byte)(Main.masterColor * 200f), 0, 0);
//                    }

//                    baseColor.A = 0;
//                    float k = (lightColor.R / 255f + lightColor.G / 255f + lightColor.B / 255f) / 3f;

//                    spriteBatch.Draw(texture, item.Center - Main.screenPosition, rectangle, baseColor * k, (float)(Main.time * 0.005f), texture.Size() * 0.5f, lightscale * scale, SpriteEffects.None, 0f);
//                    if (lightStyle == 2)
//                    {
//                        spriteBatch.Draw(texture, item.Center - Main.screenPosition, rectangle, baseColor * k, (float)(Main.time * 0.005f) + MathHelper.PiOver2, texture.Size() * 0.5f, lightscale * scale, SpriteEffects.None, 0f);
//                    }
//                }
//            }
//            return true;
//        }
//    }
//}