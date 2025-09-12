using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Contents.Melee;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.BuiltInGroups.Arguments;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.Core.Definition;
using LogSpiralLibrary.CodeLibrary.DataStructures.SequenceStructures.System;

namespace CoolerItemVisualEffect.MeleeModify;

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
        string key = $"{Mod.Name}/{typeof(CIVESword).Name}";
        if (player.GetModPlayer<MeleeModifyPlayer>().ConfigurationSwoosh.swooshActionStyle is SequenceDefinition<MeleeAction> definition)
            key = $"{definition.Mod}/{definition.Name}";
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
}
