using LogSpiralLibrary;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace CoolerItemVisualEffect
{
    public class CoolerItemDrawEffect : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public Color mainColor; 
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!ConfigurationSwoosh.ConfigSwooshInstance.ItemInventoryEffectActive) goto mylabel;
            if (mainColor == default)
            {
                var itemTex = TextureAssets.Item[item.type].Value;
                if (itemTex == null) goto mylabel;
                var w = itemTex.Width;
                var he = itemTex.Height;
                var cs = new Color[w * he];
                itemTex.GetData(cs);
                Vector4 vcolor = default;
                float count = 0;
                for (int i = 0; i < cs.Length; i++)
                {
                    if (cs[i] != default && (i - w < 0 || cs[i - w] != default) && (i - 1 < 0 || cs[i - 1] != default) && (i + w >= cs.Length || cs[i + w] != default) && (i + 1 >= cs.Length || cs[i + 1] != default))
                    {
                        var weight = (float)((i + 1) % w * (he - i / w)) / w / he;
                        vcolor += cs[i].ToVector4() * weight;
                        count += weight;
                    }
                }
                vcolor /= count;
                mainColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
            }
            item.ShaderItemEffectInventory(spriteBatch, position, origin, LogSpiralLibraryMod.Misc[0].Value, mainColor, scale);
        mylabel:
            base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (!ConfigurationSwoosh.ConfigSwooshInstance.ItemDropEffectActive) goto mylabel;
            if (mainColor == default)
            {
                var itemTex = TextureAssets.Item[item.type].Value;
                if (itemTex == null) goto mylabel;
                var w = itemTex.Width;
                var he = itemTex.Height;
                var cs = new Color[w * he];
                itemTex.GetData(cs);
                Vector4 vcolor = default;
                float count = 0;
                for (int i = 0; i < cs.Length; i++)
                {
                    if (cs[i] != default && (i - w < 0 || cs[i - w] != default) && (i - 1 < 0 || cs[i - 1] != default) && (i + w >= cs.Length || cs[i + w] != default) && (i + 1 >= cs.Length || cs[i + 1] != default))
                    {
                        var weight = (float)((i + 1) % w * (he - i / w)) / w / he;
                        vcolor += cs[i].ToVector4() * weight;
                        count += weight;
                    }
                }
                vcolor /= count;
                mainColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
            }
            item.ShaderItemEffectInWorld(spriteBatch, LogSpiralLibraryMod.Misc[0].Value, mainColor, rotation);
        mylabel: 
            base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
        }
    }
}
