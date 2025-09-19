using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingEffects;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    public const string CANVASNAMEPREFIX = "CoolerItemVisualEffect:MeleeModify_";

    public static string GetCanvasNameViaID(int whoami) => CANVASNAMEPREFIX + whoami;

    private readonly AirDistortEffect airDistortEffect = new();

    private readonly MaskEffect maskEffect = new();

    private readonly DyeEffect dyeEfect = new();

    private readonly BloomEffect bloomEffect = new();

    private IRenderEffect[][] RenderEffects => field ??= [[airDistortEffect], [maskEffect, dyeEfect, bloomEffect]];

    public void RefreshConfigEffects()
    {
        var config = ConfigurationSwoosh;
        config?.distortConfigs?.CopyToInstance(airDistortEffect);
        config?.maskConfigs?.CopyToInstance(maskEffect);
        config?.dyeConfigs?.CopyToInstance(dyeEfect);
        config?.bloomConfigs?.CopyToInstance(bloomEffect);
    }

    public void RegisterCurrentCanvas()
    {
        RenderCanvasSystem.RegisterCanvasFactory(GetCanvasNameViaID(Player.whoAmI), () => new(RenderEffects));
    }
}
