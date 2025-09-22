using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using Terraria.Audio;

namespace CoolerItemVisualEffect.UI.WeaponGroup;
[RegisterUI("Vanilla: Radial Hotbars",$"{nameof(CoolerItemVisualEffect)}:{nameof(WeaponGroupManagerUI)}")]
public partial class WeaponGroupManagerUI:BaseBody
{
    public static WeaponGroupManagerUI? Instance { get; private set; }

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
        WeaponGroupHelperUI.Close();
    }

    protected override void UpdateChildrenStatus(GameTime gameTime)
    {
        base.UpdateChildrenStatus(gameTime);
        if (_pendingUpdateFileList)
            SetupFileList();

    }
}