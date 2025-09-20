using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public Texture2D HeatMap
    {
        get
        {
            CoolerItemVisualEffectHelper.CreateHeatMapIfNull(ref field);
            return field;
        }
    }
    public Color MainColor { get; set; }
    public int LastWeaponHash { get; set; }
    public static int LastWeaponType
    {
        get => field == ItemID.None ? ItemID.TerraBlade : field;
        set;
    }
    public Vector3 WeaponHSL { get; set; }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (UseSwordModify)
            drawInfo.heldItem.type = ItemID.None;
        base.ModifyDrawInfo(ref drawInfo);
    }
}
