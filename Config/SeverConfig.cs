using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.Config
{
    public class SeverConfig:ModConfig
    {
        public static SeverConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
            base.OnLoaded();
        }
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public enum MeleeModifyLevel 
        {
            Vanilla,
            VisualOnly,
            Overhaul
        }
        [DefaultValue(MeleeModifyLevel.VisualOnly)]
        [DrawTicks]
        public MeleeModifyLevel meleeModifyLevel = MeleeModifyLevel.VisualOnly;

        [DefaultValue(true)]
        public bool AutoBalanceData = true;
    }
}
