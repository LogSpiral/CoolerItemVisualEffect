using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.UI.ConfigSaveLoader;
using CoolerItemVisualEffect.UI.WeaponGroup;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolerItemVisualEffect.UI.ManagerEntry;

[RegisterUI("Vanilla: Mouse Text", $"{nameof(CoolerItemVisualEffect)}: {nameof(ManagerEntryUI)}")]
public partial class ManagerEntryUI : BaseBody
{
    public override bool Enabled => MiscConfig.Instance.ShowEntryUI;
    protected override void OnInitialize()
    {
        base.OnInitialize();
        InitializeComponent();

        Title.ControlTarget = this;

        Title.OnUpdateStatus += delegate
        {
            Title.BackgroundColor = Color.Black* Title.HoverTimer.Lerp(0, 0.35f);
        };

        ConfigManagerEntry.OnUpdateStatus += delegate
        {
            ConfigManagerEntry.ImageColor = Color.White * ConfigManagerEntry.HoverTimer.Lerp(0.5f, 1.0f);
        };
        ConfigManagerEntry.LeftMouseClick += delegate
        {
            if (ConfigSaveLoaderUI.Active)
                ConfigSaveLoaderUI.Close();
            else
                ConfigSaveLoaderUI.Open();
        };
        ConfigManagerEntry.Texture2D = ModAsset.ConfigSaveLoader;
        GroupManagerEntry.OnUpdateStatus += delegate
        {
            GroupManagerEntry.ImageColor = Color.White * GroupManagerEntry.HoverTimer.Lerp(0.5f, 1.0f);
        };
        GroupManagerEntry.LeftMouseClick += delegate
        {
            if (WeaponGroupManagerUI.Active)
                WeaponGroupManagerUI.Close();
            else
                WeaponGroupManagerUI.Open();
        };
        GroupManagerEntry.Texture2D = ModAsset.WeaponGroupManager;
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        BackgroundColor = SUIColor.Background * Title.HoverTimer.Lerp(0, 0.25f);
        BorderColor = SUIColor.Border * Title.HoverTimer.Lerp(0, 1);
        base.UpdateStatus(gameTime);
    }
}
