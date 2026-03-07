using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Layout;
using PropPanel = PropertyPanelLibrary.PropertyPanelComponents.PropertyPanel;
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
        MainAlignment = MainAlignment.Start;
        CrossAlignment = CrossAlignment.Start;
        SetHeight(100, 0);
        ColorBar = new GradientBar()
        {
            Data = GetValue() as DesignateHeatMapData,
            Width = new Dimension(-40, 1f),
            Height = new Dimension(50, 0f),
            Left = new Anchor(0, 0, .5f),
            Top = new Anchor(36, 0, 0f),
            Owner = this,
            Positioning = Positioning.Absolute
        };
        AddChild(ColorBar);
        ColorBar.AddCurrentData();

        InnerPanel = new PropPanel()
        {
            Width = new(0, 1),
            Top = new(0, 0, 1),
            Positioning = Positioning.Absolute
        };
        AddChild(InnerPanel);
    }
    private GradientBar ColorBar { get; set; }
    private DesignateHeatMapData.ColorInfo EditTarget { get; set; }
    private PropPanel InnerPanel { get; set; }
    private AnimationTimer ExpandTimer { get; } = new AnimationTimer(3);


    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
        ExpandTimer.Update(gameTime);
        // if (ExpandTimer.IsCompleted) return;
        var targetHeight = InnerPanel.OptionList.Container.OuterBounds.Height;
        SetHeight(100 + (targetHeight + 32) * ExpandTimer, 0);

        InnerPanel.SetHeight((targetHeight + 16) * ExpandTimer, 0);
        InnerPanel.SetMargin(0, 8 * ExpandTimer);
    }
}