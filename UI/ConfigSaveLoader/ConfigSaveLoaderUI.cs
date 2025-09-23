using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using Terraria.Audio;

namespace CoolerItemVisualEffect.UI.ConfigSaveLoader;
[RegisterUI("Vanilla: Radial Hotbars",$"{nameof(CoolerItemVisualEffect)}:{nameof(ConfigSaveLoaderUI)}")]
public partial class ConfigSaveLoaderUI:BaseBody
{
    public static ConfigSaveLoaderUI Instance { get; private set; }

    public static bool Active { get; set; }

    public override bool Enabled
    {
        get => Active || !SwitchTimer.IsReverseCompleted;
        set => Active = value;
    }

    private void ReloadContent()
    {
        RemoveAllChildren();
        _contentLoaded = false;
        InitializeComponent();
        InitializeComponentExtra();
    }

    public static void Open()
    {
        // 为了方便测试用
        Instance?.ReloadContent();
        if (!Active)
            SoundEngine.PlaySound(SoundID.MenuOpen);
        Active = true;
    }

    public static void Close(bool silent = false)
    {
        if (Active && !silent)
            SoundEngine.PlaySound(SoundID.MenuClose);
        Active = false;
        ConfigSaveLoaderHelperUI.Close();
    }

    protected override void UpdateChildrenStatus(GameTime gameTime)
    {
        base.UpdateChildrenStatus(gameTime);
        if (_pendingUpdateFileList)
            SetupFileList();

    }
}