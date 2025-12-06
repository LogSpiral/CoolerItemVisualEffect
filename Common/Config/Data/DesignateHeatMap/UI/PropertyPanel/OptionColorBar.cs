using CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.Vanilla;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.Layout;
using SilkyUIFramework.Elements;
using Terraria.UI;

namespace CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.PropertyPanel;

public partial class OptionColorBar : PropertyOption
{
    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(DesignateHeatMapData));
    }

    protected override void FillOption()
    {
        base.FillOption();
        CrossAlignment = CrossAlignment.Start;
        SetHeight(100, 0);
        UIElementGroup mask = new()
        {
            Width = new(20, 0),
            Height = new(20, 0),
            BackgroundColor = Color.Transparent
        };
        AddChild(mask);
        ColorBar = new GradientBar()
        {
            Data = GetValue() as DesignateHeatMapData,
            Width = new Dimension(-40, 1f),
            Height = new Dimension(50, 0f),
            Left = new Anchor(0, 0, .5f),
            Top = new Anchor(-16, 0, 1f),
            Owner = this,
            Positioning = Positioning.Absolute
        };
        AddChild(ColorBar);
        ColorBar.AddCurrentData();
    }
    private GradientBar ColorBar { get; set; }

}