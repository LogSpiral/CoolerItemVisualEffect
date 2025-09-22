using CoolerItemVisualEffect.UIBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    };
                    ItemList.Container.Add(fileCard);//如果有就添加目标

                }
            }
        }
    }
}
