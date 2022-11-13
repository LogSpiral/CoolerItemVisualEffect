using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolerItemVisualEffect.Weapons
{
    internal class Azure : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "蔚蕴");
            Tooltip.AddTranslation(7, "你听见灵魂在嘶鸣...");
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 60;
            Item.crit = 0;
            Item.knockBack = 6;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.shoot = ModContent.ProjectileType<AzureProj>();
            Item.shootSpeed = 10;
        }
    }
}
