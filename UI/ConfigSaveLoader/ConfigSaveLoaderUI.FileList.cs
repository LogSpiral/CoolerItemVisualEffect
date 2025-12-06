using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.ConfigSaveLoader;
using CoolerItemVisualEffect.Common.MeleeModify;
using CoolerItemVisualEffect.UI.WeaponGroup;
using CoolerItemVisualEffect.UIBase;
using LogSpiralLibrary.CodeLibrary.Utilties;
using Newtonsoft.Json;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.Extensions;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.IO;
using Terraria.ModLoader.Config;

namespace CoolerItemVisualEffect.UI.ConfigSaveLoader;

public partial class ConfigSaveLoaderUI
{
    private bool _pendingUpdateFileList;
    public static string CurrentConfigName { get; set; }
    private void SetupFileList()
    {
        _pendingUpdateFileList = false;

        ItemList.Container.RemoveAllChildren();//清空
        if (!Directory.Exists(ManagerHelper.SavePath))
            Directory.CreateDirectory(ManagerHelper.SavePath);


        foreach (var path in Directory.GetFiles(ManagerHelper.SavePath))
        {
            var folder = ManagerHelper.SavePath;
            var fileName = Path.GetFileNameWithoutExtension(path);
            var fileCard = new FileCard()
            {
                FileName = fileName,
                FileFolder = folder,
                FileExtension = ManagerHelper.Extension
            };
            if (CurrentConfigName == fileName)
                fileCard.BackgroundColor = SUIColor.Background * .2f;
            fileCard.DeleteButton.LeftMouseClick += delegate
            {
                _pendingUpdateFileList = true;
            };
            fileCard.NameBox.InnerText.EndTakingInput += (sender, arg) =>
            {
                if (arg.OldValue == arg.NewValue) return;
                if (CurrentConfigName == arg.OldValue) CurrentConfigName = arg.NewValue;
                var mplr = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>();
                foreach (var w in mplr.WeaponGroups)
                {
                    if (w.BindConfigName == arg.OldValue)
                    {
                        w.BindConfigName = arg.NewValue;
                        WeaponGroupManagerUI.SaveWeaponGroup(w, true, false);
                    }
                }
                if (mplr.MeleeConfigs.TryGetValue(arg.OldValue, out var config))
                {
                    mplr.MeleeConfigs.Remove(arg.OldValue);
                    mplr.MeleeConfigs.Add(arg.NewValue, config);
                }
                mplr.WeaponGroupSyncing();
            };
            fileCard.EditButton.LeftMouseClick += delegate
            {
                var pth = fileCard.FileFullPath;
                if (!File.Exists(pth)) return;
                CurrentPath = pth;

                CurrentEditTarget = new MeleeConfig();
                ConfigSaveLoaderHelper.Load(CurrentEditTarget, fileCard.FileName, true, false);
                SwitchToEditPage();

                SoundEngine.PlaySound(SoundID.MenuOpen);
            };
            fileCard.RightMouseClick += delegate
            {
                SoundEngine.PlaySound(SoundID.ResearchComplete);
                ConfigSaveLoaderHelper.Load(MeleeConfig.Instance, fileCard.FileName, true, false);
                ConfigManager.Save(MeleeConfig.Instance);
                CurrentConfigName = fileCard.FileName;
                _pendingUpdateFileList = true;
            };
            ItemList.Container.AddChild(fileCard);//如果有就添加目标
        }
    }

    private string CurrentPath { get; set; }
    private MeleeConfig CurrentEditTarget { get; set; }

    private void RefreshPropertyPanelFiller()
    {
        PropertyPanel.Filler = new ObjectMetaDataFiller(CurrentEditTarget);
    }

    private void SwitchToEditPage()
    {
        this.AddBefore(PropertyPanel, ItemList);
        ItemList.RemoveFromParent();
        RefreshPropertyPanelFiller();
        BackButton.Join(EditButtonContainer);
        CreateNewButton.RemoveFromParent();
    }


    private void SwitchToMainPage()
    {
        this.AddBefore(ItemList, PropertyPanel);
        PropertyPanel.RemoveFromParent();
        _pendingUpdateFileList = true;
        BackButton.RemoveFromParent();
        SaveButton.RemoveFromParent();
        RevertButton.RemoveFromParent();
        CurrentEditTarget = null;
        CreateNewButton.Join(FunctionButtonContainer);
    }

}
