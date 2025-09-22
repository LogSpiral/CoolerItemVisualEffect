using CoolerItemVisualEffect.MeleeModify;
using MonoMod.Cil;
using System;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public override void Load()
    {
        On_Player.ItemCheck_EmitUseVisuals += On_Player_ItemCheck_EmitUseVisuals_CIVEMelee;
        IL_Player.ItemCheck_OwnerOnlyCode += ProjectileShootBan;
        ModifyActiveKeybind = KeybindLoader.RegisterKeybind(Mod, "ModifyActive", "I");

        CoolerItemVisualEffectMod.ModIntegration.RegisterNoWeaponDisplayCondition(
            player => player.GetModPlayer<MeleeModifyPlayer>().UseSwordModify,
            $"{nameof(CoolerItemVisualEffect)}:MeleeModify");

        MigrateOldGroupPath();

        base.Load();
    }

    private void ProjectileShootBan(ILContext il)
    {
        var c = new ILCursor(il);
        if (!c.TryGotoNext(i => i.MatchCall<Player>("ItemCheck_Shoot")))//找到ItemCheck_Shoot的位置
            return;
        if (!c.TryGotoPrev(i => i.MatchAnd()))//找到它前面最近的一次取与
            return;
        c.Index++;

        c.EmitLdarg0();
        c.EmitDelegate<Func<Player, bool>>(
            player => !player.GetModPlayer<MeleeModifyPlayer>().BeAbleToOverhaul);
        c.EmitAnd();
    }

    private Rectangle On_Player_ItemCheck_EmitUseVisuals_CIVEMelee(On_Player.orig_ItemCheck_EmitUseVisuals orig, Player self, Item sItem, Rectangle itemRectangle)
    {
        if (self.ownedProjectileCounts[ModContent.ProjectileType<CIVESword>()] < 1)
            orig(self, sItem, itemRectangle);
        return itemRectangle;
    }

    public override void Unload()
    {
        On_Player.ItemCheck_EmitUseVisuals -= On_Player_ItemCheck_EmitUseVisuals_CIVEMelee;
        IL_Player.ItemCheck_OwnerOnlyCode -= ProjectileShootBan;
        base.Unload();
    }
}
