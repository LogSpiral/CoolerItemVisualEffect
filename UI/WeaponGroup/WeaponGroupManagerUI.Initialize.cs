using CoolerItemVisualEffect.Common.MeleeModify;
using CoolerItemVisualEffect.Common.WeaponGroup;
using PropertyPanelLibrary.PropertyPanelComponents;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using System.IO;
using System.Linq;
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
        BackgroundColor = Color.CornflowerBlue * .25f;
    }

    private void InitializeComponentExtra()
    {
        InitializeTitle();
        InitializeButtonVisual();
        InitializeButtonFunction();
        SetupFileList();
        InitializePropertyPanel();
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
        static void SetHoverEffect(SUIImage image) => image.OnUpdateStatus += delegate { image.ImageColor = Color.White * image.HoverTimer.Lerp(.5f, 1f); };

        OpenFolderButton.Texture2D = ModAsset.Folder;
        CreateNewButton.Texture2D = ModAsset.CreateNewIcon;
        HelperButton.Texture2D = LogSpiralLibrary.ModAsset.Helper;
        SaveButton.Texture2D = LogSpiralLibrary.ModAsset.Save;
        RevertButton.Texture2D = LogSpiralLibrary.ModAsset.Revert;
        BackButton.Texture2D = ModAsset.BackIcon;

        SetHoverEffect(OpenFolderButton);
        SetHoverEffect(CreateNewButton);
        SetHoverEffect(HelperButton);
        SetHoverEffect(SaveButton);
        SetHoverEffect(RevertButton);
        SetHoverEffect(BackButton);
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


        SaveButton.LeftMouseClick += delegate
        {
            if (CurrentEditTarget != null)
            {
                var mplr = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
                var list = mplr.WeaponGroups;
                ManagerHelper.SaveWeaponGroup(CurrentEditTarget, true);
                var group = list.FirstOrDefault(g => g.Name == CurrentEditTarget.Name, null);
                if (group != null)
                    Weapon_Group.Load(group, CurrentPath);
                SyncWeaponGroup.Get(Main.myPlayer, list, null).Send();
                mplr.CachedGrouping.Clear();
            }
            SaveButton.RemoveFromParent();
            RevertButton.RemoveFromParent();
        };

        RevertButton.LeftMouseClick += delegate
        {
            if (!File.Exists(CurrentPath)) return;
            CurrentEditTarget = Weapon_Group.Load(CurrentPath);
            RefreshPropertyPanelFiller();
            SaveButton.RemoveFromParent();
            RevertButton.RemoveFromParent();
        };

        BackButton.LeftMouseClick += delegate
        {
            SwitchToMainPage();
        };


        SaveButton.RemoveFromParent();
        RevertButton.RemoveFromParent();
        BackButton.RemoveFromParent();
    }

    private void InitializePropertyPanel()
    {
        var writeObserver = new DelegateWriter();
        writeObserver.OnWriteValue += delegate
        {
            if (SaveButton.Parent == null)
            {
                EditButtonContainer.AddChild(RevertButton, 0);
                EditButtonContainer.AddChild(SaveButton, 0);
            }
        };
        PropertyPanel = new()
        {
            Width = new(0, 1),
            FlexGrow = 1,
            Writer = new CombinedWriter(DefaultWriter.Instance, writeObserver)
        };

    }
}
