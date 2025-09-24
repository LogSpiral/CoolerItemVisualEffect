using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.ConfigSaveLoader;
using CoolerItemVisualEffect.Common.MeleeModify;
using CoolerItemVisualEffect.Common.WeaponGroup;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.PropertyPanelComponents;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using System;
using System.IO;
using Terraria.Audio;

namespace CoolerItemVisualEffect.UI.ConfigSaveLoader;

public partial class ConfigSaveLoaderUI
{
    public PropertyPanel PropertyPanel { get; set; }

    protected override void OnInitialize()
    {
        Instance = this;
        InitializeBody();
    }

    private void InitializeBody()
    {
        BackgroundColor = Color.MediumPurple * .25f;
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

            ManagerHelper.SaveConfig(MeleeConfig.Instance);
        };
        HelperButton.LeftMouseClick += delegate
        {
            if (ConfigSaveLoaderHelperUI.Active)
                ConfigSaveLoaderHelperUI.Close();
            else
                ConfigSaveLoaderHelperUI.Open();
        };


        SaveButton.LeftMouseClick += delegate
        {
            if (CurrentEditTarget != null)
            {
                var configName = Path.GetFileNameWithoutExtension(CurrentPath);
                ManagerHelper.SaveConfig(CurrentEditTarget, CurrentPath, false);
                Main.LocalPlayer
                    .GetModPlayer<MeleeModifyPlayer>()
                    .MeleeConfigs[configName] = (MeleeConfig)CurrentEditTarget.Clone();
                SyncWeaponGroup.Get().Send();
            }

            SaveButton.Remove();
            RevertButton.Remove();
        };

        RevertButton.LeftMouseClick += delegate
        {
            if (!File.Exists(CurrentPath)) return;
            CurrentEditTarget = new MeleeConfig();
            ConfigSaveLoaderHelper.Load(CurrentEditTarget, CurrentPath, false, false);
            RefreshPropertyPanelFiller();
            SaveButton.Remove();
            RevertButton.Remove();
        };

        BackButton.LeftMouseClick += delegate
        {
            SwitchToMainPage();
            SoundEngine.PlaySound(SoundID.MenuClose);
        };


        SaveButton.Remove();
        RevertButton.Remove();
        BackButton.Remove();
    }

    private void InitializePropertyPanel()
    {
        var writeObserver = new DelegateWriter();
        writeObserver.OnWriteValue += delegate
        {
            if (SaveButton.Parent == null)
            {
                EditButtonContainer.Add(RevertButton, 0);
                EditButtonContainer.Add(SaveButton, 0);
            }
        };
        var previewDecorator = new DelegateOptionDecorator();
        previewDecorator.OnPreFillOption += option =>
        {
            option.MouseEnter += delegate
            {
                previewingOption = option;
            };
            option.MouseLeave += delegate
            {
                previewingOption = null;
            };
        };
        PropertyPanel = new()
        {
            Width = new(0, 1),
            FlexGrow = 1,
            Writer = new CombinedWriter(DefaultWriter.Instance, writeObserver),
            OptionDecorator = new CombinedOptionDecorator(LabelOptionDecorator.NewLabelDecorator(),previewDecorator)
        };

    }
}
