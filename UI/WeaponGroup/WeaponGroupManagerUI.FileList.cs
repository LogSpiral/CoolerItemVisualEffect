using CoolerItemVisualEffect.UIBase;
using SilkyUIFramework.Extensions;
using System.IO;
using LogSpiralLibrary.CodeLibrary.Utilties;
using Weapon_Group = CoolerItemVisualEffect.Common.WeaponGroup.WeaponGroup;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using CoolerItemVisualEffect.Common.MeleeModify;

namespace CoolerItemVisualEffect.UI.WeaponGroup;

public partial class WeaponGroupManagerUI
{
    private bool _pendingUpdateFileList;

    private void SetupFileList()
    {
        _pendingUpdateFileList = false;

        ItemList.Container.RemoveAllChildren();//清空
        if (!Directory.Exists(ManagerHelper.SavePath))
            Directory.CreateDirectory(ManagerHelper.SavePath);

        var tablePath = Path.Combine(ManagerHelper.SavePath, "indexTable.txt");
        if (File.Exists(tablePath))
        {
            var indexTable = File.ReadAllLines(tablePath);
            foreach (string path in indexTable)
            {
                var pth = Path.Combine(ManagerHelper.SavePath, path + ManagerHelper.Extension);
                if (File.Exists(pth))
                {
                    var fileCard = new FileCard()
                    {
                        FileName = path,
                        FileFolder = ManagerHelper.SavePath,
                        FileExtension = ManagerHelper.Extension
                    };
                    fileCard.DeleteButton.LeftMouseClick += delegate
                    {
                        _pendingUpdateFileList = true;
                        var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroups;
                        foreach (var s in list)
                        {
                            if (s.Name == fileCard.FileName)
                            {
                                list.Remove(s);
                                break;
                            }
                        }
                        Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroupSyncing();
                        string indexTable = "";
                        foreach (var pair in list)
                        {
                            indexTable += $"{pair.Name}\n";
                        }
                        File.WriteAllText(Path.Combine(ManagerHelper.SavePath, "indexTable.txt"), indexTable);
                    };
                    fileCard.NameBox.InnerText.EndTakingInput += (sender, arg) =>
                    {
                        if (arg.OldValue == arg.NewValue) return;
                        var list = Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroups;
                        foreach (var s in list)
                        {
                            if (s.Name == arg.OldValue)
                                s.Name = arg.NewValue;
                        }
                        string indexTable = "";
                        foreach (var pair in list)
                            indexTable += $"{pair.Name}\n";

                        Main.LocalPlayer.GetModPlayer<MeleeModifyPlayer>().WeaponGroupSyncing();
                        File.WriteAllText(Path.Combine(ManagerHelper.SavePath, "indexTable.txt"), indexTable);
                    };
                    fileCard.EditButton.LeftMouseClick += delegate
                    {
                        var path = fileCard.FileFullPath;
                        if (!File.Exists(path)) return;
                        CurrentPath = path;
                        CurrentEditTarget = Weapon_Group.Load(path);
                        SwitchToEditPage();
                    };
                    ItemList.Container.Add(fileCard);//如果有就添加目标

                }
            }
        }
    }

    private string CurrentPath { get; set; }
    private Weapon_Group CurrentEditTarget { get; set; }

    private void RefreshPropertyPanelFiller() 
    {
        /*PropertyPanel.Filler = new DesignatedMemberFiller([
            (CurrentEditTarget,
            [nameof(Weapon_Group.BasedOnDefaultCondition),
            nameof(Weapon_Group.WhiteList),
            nameof(Weapon_Group.BindConfigName)])
        ]);*/

        PropertyPanel.Filler = new ObjectMetaDataFiller(CurrentEditTarget);
    }

    private void SwitchToEditPage()
    {
        this.AddBefore(PropertyPanel, ItemList);
        ItemList.Remove();
        RefreshPropertyPanelFiller();
        BackButton.Join(EditButtonContainer);
        CreateNewButton.Remove();
    }


    private void SwitchToMainPage() 
    {
        this.AddBefore(ItemList, PropertyPanel);
        PropertyPanel.Remove();
        _pendingUpdateFileList = true;
        BackButton.Remove();
        SaveButton.Remove();
        RevertButton.Remove();
        CurrentEditTarget = null;
        CreateNewButton.Join(FunctionButtonContainer);
    }

}
