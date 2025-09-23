using CoolerItemVisualEffect.Common.Config;
using CoolerItemVisualEffect.Common.ConfigSaveLoader;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
namespace CoolerItemVisualEffect.UIBase.WeaponGroup;

public class OptionBindConfig:OptionDropdownList
{
    protected override void Register(Mod mod)
    {
        
    }
    private static string SavePath { get; } = Path.Combine(Main.SavePath, "Mods", nameof(CoolerItemVisualEffect), "MeleeConfig");

    protected override void FillOption()
    {
        HashSet<string> options = [""];

        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);


        foreach (var path in Directory.GetFiles(SavePath))
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            options.Add(fileName);
        }
        OptionLabelsAttribute = new OptionStringsAttribute([.. options]);
        base.FillOption();
    }
}