global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.DataStructures;
global using Terraria.ID;
global using Terraria.ModLoader;
global using LogSpiralLibrary.CodeLibrary;
global using MeleeSequence = LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Sequence<LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Melee.MeleeAction>;

using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using static CoolerItemVisualEffect.ConfigurationCIVE;
using LogSpiralLibrary;
using System.IO;
using NetSimplified;
namespace CoolerItemVisualEffect
{
    public class CoolerItemVisualEffectMod : Mod
    {
        #region Effects
        internal static Effect ShaderSwooshEX => LogSpiralLibraryMod.ShaderSwooshEX;
        internal static Effect ShaderSwooshUL => LogSpiralLibraryMod.ShaderSwooshUL;
        internal static Effect RenderEffect => LogSpiralLibraryMod.RenderEffect;
        #endregion
        #region 辅助函数
        public override object Call(params object[] args)
        {
            if (args.Length == 0 || args[0] is not string str)
                return base.Call(args);
            switch (str)
            {
                case "RegisterModifyWeaponTex":
                    {
                        try
                        {

                            MeleeModifyPlayer.weaponGetFunctions.Add(((Func<Item, Texture2D> func, float priority))(args[1], args[2]));
                            MeleeModifyPlayer.RefreshWeaponTexFunc = true;
                        }
                        catch
                        {
                            return null;
                        }
                        return null;
                    }
                default: return null;
            }
        }
        #endregion
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetModule.ReceiveModule(reader, whoAmI);

            base.HandlePacket(reader, whoAmI);
        }
        public override void Load()
        {
            AddContent<NetModuleLoader>();

            base.Load();
        }
    }
}