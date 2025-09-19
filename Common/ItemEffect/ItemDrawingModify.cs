using CoolerItemVisualEffect.Common.Config;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;

namespace CoolerItemVisualEffect.Common.ItemEffect;

public class ItemDrawingModify : GlobalItem
{
    private static Dictionary<int, Color> MainColorLookup { get; } = [];
    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (!MiscConfig.Instance.ItemInventoryEffectActive) return;
        if (!MainColorLookup.TryGetValue(item.type, out var mainColor) && TextureAssets.Item[item.type].Value is { } itemTex)
            mainColor = MainColorLookup[item.type] = CoolerItemVisualEffectHelper.CalculateWeightedMean(itemTex);
        item.ShaderItemEffectInventory(spriteBatch, position, origin, LogSpiralLibraryMod.Misc[0].Value, mainColor, scale);
    }

    public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        if (!MiscConfig.Instance.ItemDropEffectActive) return;
        if (!MainColorLookup.TryGetValue(item.type, out var mainColor) && TextureAssets.Item[item.type].Value is { } itemTex)
            mainColor = MainColorLookup[item.type] = CoolerItemVisualEffectHelper.CalculateWeightedMean(itemTex);
        item.ShaderItemEffectInWorld(spriteBatch, LogSpiralLibraryMod.Misc[0].Value, mainColor, rotation);
    }
}