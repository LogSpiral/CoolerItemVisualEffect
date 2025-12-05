global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.DataStructures;
global using Terraria.ID;
global using Terraria.ModLoader;
using LogSpiralLibrary;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using NetSimplified;
using System;
using System.IO;
using System.Reflection;

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
    public override void Load()
    {
        AddContent<NetModuleLoader>();

        if (ModLoader.TryGetMod("CalamityMod", out var calamity) && calamity.TryFind<GlobalProjectile>("CalamityGlobalProjectile", out var gbProj))
        {
            MonoModHooks.Modify(gbProj.GetType().GetMethod("PreDraw"), il => 
            {
                var cursor = new ILCursor(il);
                var startLabel = cursor.MarkLabel();
                cursor.Index = 0;
                cursor.EmitLdsfld(typeof(Main).GetField(nameof(Main.gameMenu), BindingFlags.Static | BindingFlags.Public));
                cursor.EmitBrfalse(startLabel);
                cursor.EmitLdcI4(1);
                cursor.EmitRet();
            });

            MonoModHooks.Modify(gbProj.GetType().GetMethod("GetAlpha"), il =>
            {
                var cursor = new ILCursor(il);
                var startLabel = cursor.MarkLabel();
                cursor.Index = 0;
                cursor.EmitLdsfld(typeof(Main).GetField(nameof(Main.gameMenu), BindingFlags.Static | BindingFlags.Public));
                cursor.EmitBrfalse(startLabel);
                cursor.EmitLdnull();
                cursor.EmitRet();
            });
        }
    }
    #endregion

}