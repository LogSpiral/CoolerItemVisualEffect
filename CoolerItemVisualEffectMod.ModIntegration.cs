using CoolerItemVisualEffect.Common.MeleeModify;
using CoolerItemVisualEffect.Common.WeaponDisplay;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CoolerItemVisualEffect;

public partial class CoolerItemVisualEffectMod
{
    public override object Call(params object[] args)
    {
        var length = args.Length;
        if (length == 0 || args[0] is not string str)
            return base.Call(args);
        switch (str)
        {
            case "RegisterModifyWeaponTex":
                {
                    if (length < 3 || args[1] is not Func<Item, Texture2D> func || args[2] is not float priority)
                        return false;
                    ModIntegration.RegisterModifyWeaponTex(func, priority);
                    return true;
                }
            case "RegisterNoWeaponDisplayCondition":
                {
                    if (length < 3 || args[1] is not Func<Player, bool> condition || args[2] is not string name)
                        return false;
                    return ModIntegration.RegisterNoWeaponDisplayCondition(condition, name);
                }
            default: return null;
        }
    }

    public static class ModIntegration
    {
        public static void RegisterModifyWeaponTex(Func<Item, Texture2D> func, float priority) => MeleeModifyPlayerUtils.RegisterModifyWeaponTex(func, priority);

        public static bool RegisterNoWeaponDisplayCondition(Func<Player, bool> condition, string name) => WeaponDisplayLayer.RegisterNoWeaponDisplayCondition(condition, name);
    }
}
