using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.ConfigSaveLoader;
using CoolerItemVisualEffect.Common.WeaponGroup;
using CoolerItemVisualEffect.MeleeModify;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.BuiltInGroups.Arguments;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.Definition;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.System;
using System.Collections.Generic;
using System.IO;
using Terraria.Localization;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public class MeleeModifierItem : GlobalItem
{
    private static bool CheckRightUse(Sequence sequence)
    {
        foreach (var group in sequence.Groups)
        {
            foreach (var pair in group.Contents)
            {
                if (!pair.Wrapper.Available) continue;
                if (pair.Argument is ConditionArg condition && condition.ConditionDefinition.Name == "MouseRight")
                    return true;

                if (pair.Argument is ConditionWeightArg weightArg && weightArg.ConditionDefinition.Name == "MouseRight")
                    return true;

                if (pair.Wrapper.Sequence is Sequence subSequence && CheckRightUse(subSequence))
                    return true;

            }
        }


        return false;
    }


    public override bool AltFunctionUse(Item item, Player player)
    {
        var mplr = player.GetModPlayer<MeleeModifyPlayer>();
        var key = $"{Mod.Name}/MeleeAction/{typeof(CIVESword).Name}";
        if (player.GetModPlayer<MeleeModifyPlayer>().SwooshActionStyle is SequenceDefinition<MeleeAction> definition)
            key = $"{definition.Mod}/MeleeAction/{definition.Name}";
        if (mplr.BeAbleToOverhaul && SequenceManager<MeleeAction>.Instance.Sequences.TryGetValue(key, out var value))
        {
            return CheckRightUse(value);
        }
        return base.AltFunctionUse(item, player);
    }

    public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
    {
        var mplr = player.GetModPlayer<MeleeModifyPlayer>();
        if (player.itemAnimation == player.itemAnimationMax && player.ownedProjectileCounts[ModContent.ProjectileType<CIVESword>()] == 0 && mplr.BeAbleToOverhaul)
        {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, default, ModContent.ProjectileType<CIVESword>(), player.HeldItem.damage, player.HeldItem.knockBack, player.whoAmI);
        }
        base.UseStyle(item, player, heldItemFrame);
    }

    //public override bool CanShoot(Item item, Player player)
    //{
    //    var mplr = player.GetModPlayer<MeleeModifyPlayer>();
    //    if (ConfigCIVEInstance.SwordModifyActive && mplr.IsMeleeBroadSword && vanillaSlashItems.Contains(item.type))
    //        return false;
    //    return base.CanShoot(item, player);
    //}
    public override bool? CanHitNPC(Item item, Player player, NPC target)
    {
        var mplr = player.GetModPlayer<MeleeModifyPlayer>();
        if (mplr.BeAbleToOverhaul)
            return false;
        return base.CanHitNPC(item, player, target);
    }

    public override bool CanHitPvp(Item item, Player player, Player target)
    {
        var mplr = player.GetModPlayer<MeleeModifyPlayer>();
        if (mplr.BeAbleToOverhaul)
            return false;
        return base.CanHitPvp(item, player, target);
    }


    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (Main.gameMenu) return;

        var plr = Main.LocalPlayer;
        var mplr = plr.GetModPlayer<MeleeModifyPlayer>();

        if (!mplr.CachedGrouping.TryGetValue(item.type, out var group))
        {
            foreach (var weaponGroup in mplr.WeaponGroups)
                if (weaponGroup.CheckAvailabe(item))
                {
                    mplr.CachedGrouping[item.type] = group = weaponGroup;
                    break;
                }
            if (group == null)
                mplr.CachedGrouping[item.type] = null;
        }


        if (group == null) return;

        string path = "Mods.CoolerItemVisualEffect.WeaponGroup";

        tooltips.Add(new TooltipLine(Mod, "Grouping",
            Language.GetTextValue($"{path}.CurrentGroupName", group.Name))
        { OverrideColor = Color.CornflowerBlue });

        tooltips.Add(new TooltipLine(Mod, "BindConfig",
            string.IsNullOrEmpty(group.BindConfigName)
            ? Language.GetTextValue($"{path}.DefaultConfig")
            : Language.GetTextValue($"{path}.BindConfigName", group.BindConfigName))
        { OverrideColor = Color.MediumPurple });

    }
}
