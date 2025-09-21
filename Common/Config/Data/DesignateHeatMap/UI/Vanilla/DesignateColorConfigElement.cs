using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace CoolerItemVisualEffect.Common.Config.Data.DesignateHeatMap.UI.Vanilla;

public class DesignateColorConfigElement : ConfigElement<DesignateHeatMapData>
{
    public DesignatedColorBar ColorBar { get; private set; }

    public override void OnBind()
    {
        Height.Pixels = 100;
        ColorBar = new DesignatedColorBar()
        {
            Data = Value,
            Width = new StyleDimension(-40, 1f),
            Height = new StyleDimension(50, 0f),
            Left = new StyleDimension(20, 0),
            Top = new StyleDimension(30, 0),
            Owner = this
        };
        Append(ColorBar);
        ColorBar.AddCurrentData();
        base.OnBind();
    }
}