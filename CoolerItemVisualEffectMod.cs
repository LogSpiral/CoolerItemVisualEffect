global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.DataStructures;
global using Terraria.ID;
global using Terraria.ModLoader;
using LogSpiralLibrary;
using Microsoft.Xna.Framework.Graphics;
using NetSimplified;
using System.IO;

namespace CoolerItemVisualEffect;

public partial class CoolerItemVisualEffectMod : Mod
{

    #region Effects

    internal static Effect ShaderSwooshEX => LogSpiralLibraryMod.ShaderSwooshEX;
    internal static Effect ShaderSwooshUL => LogSpiralLibraryMod.ShaderSwooshUL;
    internal static Effect RenderEffect => LogSpiralLibraryMod.RenderEffect;

    #endregion Effects

    #region NetSimplified

    public override void HandlePacket(BinaryReader reader, int whoAmI) => NetModule.ReceiveModule(reader, whoAmI);
    public override void Load() => AddContent<NetModuleLoader>();

    #endregion

}