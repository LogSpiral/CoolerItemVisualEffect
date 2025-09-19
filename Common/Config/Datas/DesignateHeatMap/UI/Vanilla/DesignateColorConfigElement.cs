using CoolerItemVisualEffect.Common.Config.Datas.DesignateHeatMap;
using Terraria.ModLoader.Config.UI;

namespace CoolerItemVisualEffect.Common.Config.Datas.DesignateHeatMap.UI.Vanilla;

public class DesignateColorConfigElement : ConfigElement<DesignateHeatMapData>
{
    public DesignatedColorBar designatedColorBar;

    public override void OnBind()
    {
        Height.Pixels = 100;
        designatedColorBar = new DesignatedColorBar()
        {
            data = Value,
            Width = new(-40, 1f),
            Height = new(50, 0f),
            Left = new(20, 0),
            Top = new(30, 0),
            owner = this
        };
        Append(designatedColorBar);
        designatedColorBar.AddCurrentDatas();
        base.OnBind();
    }
}