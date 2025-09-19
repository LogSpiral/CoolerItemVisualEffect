using CoolerItemVisualEffect.Common.Config;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace CoolerItemVisualEffect.Common.TeleportEffect
{
    internal class TeleportLayer : PlayerDrawLayer
    {
        public override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            var fac = player.itemAnimation / (float)player.itemAnimationMax;
            var _fac = (fac * 2 % 1).HillFactor2() * (fac < .5f ? .5f : 1f);
            float rotation = (float)LogSpiralLibraryMod.ModTime * .05f;
            float scale = 2f * _fac;
            SpriteEffects dir = 0;
            Color mainColor = Color.White;
            if (true)
                mainColor = player.HeldItem.type switch
                {
                    ItemID.MagicMirror or ItemID.IceMirror or ItemID.CellPhone or ItemID.MagicConch or ItemID.ShellphoneOcean or ItemID.Shellphone or ItemID.RecallPotion or ItemID.WormholePotion => Color.Cyan,
                    ItemID.DemonConch or ItemID.ShellphoneHell => Color.Red,
                    ItemID.ShellphoneSpawn => Color.Lime,
                    ItemID.TeleportationPotion or ItemID.PotionOfReturn => Color.MediumPurple,
                    _ => Color.White
                };

            Vector2 center = player.MountedCenter + new Vector2(0, player.gfxOffY) - Main.screenPosition;

            Color colorVortex = mainColor * 0.8f;
            colorVortex.A /= 2;
            Color color1 = Color.Lerp(mainColor, Color.Black, 0.5f);
            color1.A = mainColor.A;
            float sinValue = 0.95f + (rotation * 0.75f).ToRotationVector2().Y * 0.1f;
            color1 *= sinValue;
            float scale1 = 0.6f + scale * 0.6f * sinValue;
            Texture2D voidTex = ModAsset.Extra_50.Value;
            Vector2 voidOrigin = voidTex.Size() / 2f;
            Texture2D vortexTex = ModAsset.Projectile_578.Value;//TextureAssets.Projectile[ProjectileID.DD2ApprenticeStorm].Value;//;
            drawInfo.DrawDataCache.Add(new(voidTex, center, null, color1, -rotation + 0.35f, voidOrigin, scale1, dir ^ SpriteEffects.FlipHorizontally, 0));
            drawInfo.DrawDataCache.Add(new(voidTex, center, null, mainColor, -rotation, voidOrigin, scale, dir ^ SpriteEffects.FlipHorizontally, 0));
            drawInfo.DrawDataCache.Add(new(voidTex, center, null, mainColor * 0.8f, rotation * 0.5f, voidOrigin, scale * 0.9f, dir, 0));
            drawInfo.DrawDataCache.Add(new(vortexTex, center, null, colorVortex, -rotation * 0.7f, vortexTex.Size() * .5f, scale, dir ^ SpriteEffects.FlipHorizontally, 0));
            drawInfo.DrawDataCache.Add(new(vortexTex, center, null, Color.White with { A = 0 }, -rotation * 1.4f, vortexTex.Size() * .5f, scale * .85f, dir ^ SpriteEffects.FlipHorizontally, 0));
        }

        public override Position GetDefaultPosition() => new Multiple()
        {
            { new Between(PlayerDrawLayers.Tails, PlayerDrawLayers.Wings),info => info.drawPlayer.itemAnimationMax != 0 && info.drawPlayer.itemAnimation / (float)info.drawPlayer.itemAnimationMax <= .5f},
            { new Between(PlayerDrawLayers.BeetleBuff, PlayerDrawLayers.EyebrellaCloud),info => info.drawPlayer.itemAnimationMax != 0 && info.drawPlayer.itemAnimation / (float)info.drawPlayer.itemAnimationMax > .5f}
        };

        private static readonly int[] TeleportItems
            = [ItemID.MagicMirror,ItemID.CellPhone,ItemID.MagicConch, ItemID.IceMirror,ItemID.DemonConch,
            ItemID.Shellphone,ItemID.ShellphoneHell,ItemID.ShellphoneOcean,ItemID.ShellphoneSpawn,ItemID.RecallPotion,ItemID.WormholePotion,ItemID.TeleportationPotion, ItemID.PotionOfReturn];

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            var plr = drawInfo.drawPlayer;
            return MiscConfig.Instance.TeleprotEffectActive && plr.ItemAnimationActive && TeleportItems.Contains(plr.HeldItem.type);
        }
    }
}