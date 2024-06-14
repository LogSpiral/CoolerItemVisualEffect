using System;

namespace CoolerItemVisualEffect
{
    public class SlashGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation)
        {
            bool useSlashEffect = item.useStyle == ItemUseStyleID.Swing && (CoolerItemVisualEffectMod.MeleeCheck(item.DamageType) || ConfigurationSwoosh.ConfigSwooshInstance.ignoreDamageType);
            if (ConfigurationSwoosh.ConfigSwooshInstance.toolsNoUseNewSwooshEffect)
            {
                useSlashEffect = useSlashEffect && item.axe == 0 && item.hammer == 0 && item.pick == 0;
            }
            return lateInstantiation && useSlashEffect;
        }
        /// <summary>
        /// <br>Åö×²ÏäÐÞ¸Äº¯Êý</br>
        /// <br>Modify The Hit Box</br>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="player"></param>
        /// <param name="hitbox"></param>
        /// <param name="noHitbox"></param>
        public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            var modPlayer = player.GetModPlayer<CoolerItemVisualEffectPlayer>();
            if (modPlayer.ConfigurationSwoosh.hitBoxStyle != ConfigurationSwoosh.HitBoxStyle.¾ØÐÎRectangle || !player.GetModPlayer<CoolerItemVisualEffectPlayer>().UseSlash)
            {
                return;
            }
            Vector2 hitboxpos = modPlayer.HitboxPosition;
            Vector2 hitboxSize = new Vector2(Math.Abs(hitboxpos.X), Math.Abs(hitboxpos.Y));
            hitbox.Width = (int)hitboxSize.X;
            hitbox.Height = (int)hitboxSize.Y;
            hitbox.X = (int)player.Center.X;
            if (hitboxpos.Y < -4)
            {
                hitbox.Y = (int)player.Center.Y - (int)hitboxSize.Y;
                if (player.gravDir == -1f)
                {
                    hitbox.Y += (int)hitboxSize.Y;
                }
            }
            else if (hitboxpos.Y > 4)
            {
                hitbox.Y = (int)player.Center.Y;
                if (player.gravDir == -1f)
                {
                    hitbox.Y -= (int)hitboxSize.Y;
                }
            }
            else
            {
                hitbox.Y = (int)player.Center.Y - 4;
                hitbox.Height = 8;
            }
            if (hitboxpos.X < -4)
            {
                hitbox.X -= (int)hitboxSize.X;
            }
            else if (hitboxpos.X > 4)
            {
            }
            else
            {
                hitbox.X = (int)player.Center.X - 4;
                hitbox.Width = 8;
            }
            base.UseItemHitbox(item, player, ref hitbox, ref noHitbox);
        }
    }
}