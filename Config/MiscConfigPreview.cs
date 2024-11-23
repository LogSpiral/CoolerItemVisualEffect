using LogSpiralLibrary.CodeLibrary.ConfigModification;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Config
{
    public abstract class MiscPreview<T> : SimplePreview<T> 
    {
        public override bool usePreview => false;//MiscConfig.Instance.usePreview;
    }
    public class WeaponDisplayPreview : MiscPreview<bool> 
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, bool data, ModConfig pendingConfig)
        {
        }
    }
    public class WeaponScalePreview : MiscPreview<float> 
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, float data, ModConfig pendingConfig)
        {
        }
    }
    public class ItemEffectPreview : MiscPreview<bool> 
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, bool data, ModConfig pendingConfig)
        {
        }
    }
    public class ProjectileModificationPreview : MiscPreview<bool> 
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, bool data, ModConfig pendingConfig)
        {
        }
    }
    public class TeleportModificationPreview : MiscPreview<bool> 
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle drawRange, bool data, ModConfig pendingConfig)
        {
        }
    }
}
