using CoolerItemVisualEffect.Common.MeleeModify;
using CoolerItemVisualEffect.Common.WeaponGroup;
using CoolerItemVisualEffect.UIBase;
using PropertyPanelLibrary.PropertyPanelComponents;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using SilkyUIFramework;
using Terraria.Localization;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;
namespace CoolerItemVisualEffect.UI.WeaponGroup;

public partial class WeaponGroupManagerUI
{
    public PropertyPanel PropertyPanel { get; set; }

    protected override void OnInitialize()
    {
        Instance = this;
        InitializeBody();
    }

    private void InitializeBody()
    {
        BackgroundColor = SUIColor.Background * .25f;
    }

    private void InitializeComponentExtra()
    {
        InitializeTitle();
        InitializeButtonVisual();
        InitializeButtonFunction();
        SetupFileList();

    }
    private void InitializeTitle()
    {
        Title.Text = ManagerHelper.GetLocalizationValue("Title");
        Title.UseDeathText();
        TitlePanel.ControlTarget = this;

        CloseButton.CrossBorderColor = SUIColor.Border * 0.75f;
        CloseButton.CrossBackgroundColor = SUIColor.Warn * 0.75f;
        CloseButton.CrossBorderHoverColor = SUIColor.Highlight;
        CloseButton.CrossBackgroundHoverColor = SUIColor.Warn;
        CloseButton.LeftMouseClick += delegate
        {
            Close();
        };
    }

    private void InitializeButtonVisual()
    {
        OpenFolderButton.Texture2D = ModAsset.Folder;
        CreateNewButton.Texture2D = ModAsset.CreateNewIcon;
        HelperButton.Texture2D = LogSpiralLibrary.ModAsset.Helper;
        SaveButton.Texture2D = LogSpiralLibrary.ModAsset.Save;
        RevertButton.Texture2D = LogSpiralLibrary.ModAsset.Revert;
        BackButton.Texture2D = ModAsset.BackIcon;
    }


    private void InitializeButtonFunction()
    {
        OpenFolderButton.LeftMouseClick += delegate
        {
            Utils.OpenFolder(ManagerHelper.SavePath);
        };
        CreateNewButton.LeftMouseClick += delegate
        {
            _pendingUpdateFileList = true;
            var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroups;
            var selector = new Weapon_Group();
            list.Insert(0, selector);
            ManagerHelper.SaveWeaponGroup(selector);
            SyncWeaponGroup.Get(Main.myPlayer, list, null).Send();
        };
        HelperButton.LeftMouseClick += delegate
        {
            if (WeaponGroupHelperUI.Active)
                WeaponGroupHelperUI.Close();
            else
                WeaponGroupHelperUI.Open();
        };

    }

    private void InitializePropertyPanel() 
    {
        var writeObserver = new DelegateWriter();
        writeObserver.OnWriteValue += delegate
        {
            if (SaveButton.Parent == null) 
            {
                EditButtonContainer.Add(RevertButton,0);
                EditButtonContainer.Add(SaveButton, 0);
            }
        };
        PropertyPanel = new()
        {
            Width = new(0, 1),
            FlexGrow = 1,
            Writer = writeObserver
        };
        
    }
}
