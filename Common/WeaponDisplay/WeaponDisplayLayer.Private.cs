using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.MeleeModify;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace CoolerItemVisualEffect.Common.WeaponDisplay;

public partial class WeaponDisplayLayer
{
    private static Vector2 DrawPlayer_Head_GetSpecialDrawPosition(ref PlayerDrawSet drawinfo, Vector2 helmetOffset, Vector2 hatOffset)
    {
        var value = Main.OffsetsPlayerHeadgear[drawinfo.drawPlayer.bodyFrame.Y / drawinfo.drawPlayer.bodyFrame.Height] * drawinfo.drawPlayer.Directions;
        var vector = drawinfo.Position - Main.screenPosition + helmetOffset + new Vector2((float)(-(float)drawinfo.drawPlayer.bodyFrame.Width / 2 + drawinfo.drawPlayer.width / 2), drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4) + hatOffset * drawinfo.drawPlayer.Directions + value;
        vector = vector.Floor();
        vector += drawinfo.drawPlayer.headPosition + drawinfo.headVect;
        if (drawinfo.drawPlayer.gravDir == -1f)
        {
            vector.Y += 12f;
        }
        vector = vector.Floor();
        return vector;
    }

    private static void DrawWeapon(Player Player, Item holditem, PlayerDrawSet drawInfo)
    {
        var texture = MeleeModifyPlayerUtils.GetWeaponTextureFromItem(holditem);
        Rectangle rectangle = new(0, 0, texture.Width, texture.Height);
        var origin = rectangle.Size() / 2f;
        var value5 = DrawPlayer_Head_GetSpecialDrawPosition(ref drawInfo, Vector2.Zero, new Vector2(0f, 8f));
        var rot = MathHelper.Pi;
        if (holditem.DamageType == DamageClass.Ranged || (holditem.axe > 0 || holditem.pick > 0) && holditem.channel ||
            holditem.useStyle == ItemUseStyleID.Shoot && !Item.staff[holditem.type] && holditem.DamageType == DamageClass.Magic
            || holditem.type == ItemID.KOCannon || holditem.type == ItemID.GolemFist)
        {
            if (Player.gravDir == -1f)
            {
                rot += MathHelper.PiOver4;
                if (Player.direction < 0) rot -= MathHelper.PiOver2;
            }
            else
            {
                rot -= MathHelper.PiOver4;
                if (Player.direction < 0) rot += MathHelper.PiOver2;
            }
        }

        var animation = Main.itemAnimations[holditem.type];
        if (animation != null)
        {
            // 动态武器
            rectangle = animation.GetFrame(texture);
            origin = animation.GetFrame(texture).Size() * .5f;
        }
        DrawData item = new(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, MiscConfig.Instance.weaponScale * holditem.scale, drawInfo.playerEffect);
        drawInfo.DrawDataCache.Add(item);
        if (holditem.glowMask >= 0)
        {
            var glow = TextureAssets.GlowMask[holditem.glowMask].Value;
            DrawData itemglow = new(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, MiscConfig.Instance.weaponScale * holditem.scale, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(itemglow);
        }
        if (holditem.ModItem != null && ModContent.HasAsset(holditem.ModItem.Texture + "_Glow"))
        {
            var glow = ModContent.Request<Texture2D>(holditem.ModItem.Texture + "_Glow").Value;
            DrawData itemglow = new(glow, value5, new Rectangle?(rectangle), Color.White * (1 - drawInfo.shadow), rot, origin, MiscConfig.Instance.weaponScale * holditem.scale, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(itemglow);
        }
    }
}
