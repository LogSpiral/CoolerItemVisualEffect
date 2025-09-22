using CoolerItemVisualEffect.Common.Config;

namespace CoolerItemVisualEffect.Common.MeleeModify;
public partial class MeleeModifyPlayer : ModPlayer
{
    /// <summary>
    /// 判定当前手持武器是否可以进行修改
    /// </summary>
    public bool IsMeleeBroadSword
    {
        get
        {
            if (WeaponGroups != null)
                foreach (var selector in WeaponGroups)
                    if (selector.CheckAvailabe(Player.HeldItem))
                        return true;

            return MeleeModifyPlayerUtils.MeleeBroadSwordCheck(Player.HeldItem);
        }
    }


    /// <summary>
    /// 判定当前是否可以处于近战序列状态
    /// </summary>
    public bool BeAbleToOverhaul =>
        ServerConfig.Instance.meleeModifyLevel == ServerConfig.MeleeModifyLevel.Overhaul
        && ConfigurationSwoosh.SwordModifyActive
        && IsMeleeBroadSword;

    /// <summary>
    /// 判定当前是否正处于近战序列状态
    /// </summary>
    public bool UseSwordModify =>
        BeAbleToOverhaul
        && Player.itemAnimation > 0;

    public override void PostUpdate()
    {
        if (IsMeleeBroadSword)
            ItemID.Sets.SkipsInitialUseSound[Player.HeldItem.type] =
                ServerConfig.Instance.meleeModifyLevel == ServerConfig.MeleeModifyLevel.Overhaul
                && ConfigurationSwoosh.SwordModifyActive;
        MeleeModifyPlayerUtils.CheckItemChange(Player);
        base.PostUpdate();
    }
}