using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Graphics2D;
using System.IO;
using Terraria.Audio;
using Terraria.Localization;

namespace CoolerItemVisualEffect.UI.WeaponGroup;
[RegisterUI("Vanilla: Mouse Text", $"{nameof(CoolerItemVisualEffect)}: {nameof(WeaponGroupHelperUI)}")]
public partial class WeaponGroupHelperUI : BaseBody
{
    #region 属性
    public static WeaponGroupHelperUI Instance { get; set; }
    public static bool Active { get; set; }
    public AnimationTimer SwitchTimer { get; init; } = new(3);

    public override bool Enabled
    {
        get => Active || !SwitchTimer.IsReverseCompleted;
        set => Active = value;
    }

    #endregion 属性

    public static void SetHelpHintKey(string key) => Instance?.HelpHintKey = key;
    public string HelpHintKey
    {
        get;
        set
        {
            if (value != field && HintTextTitle != null)
            {
                var key = value ?? "HelpPanel";
                HintTextTitle.Text = Language.GetTextValue($"Mods.CoolerItemVisualEffect.WeaponGroup.Help.{key}.DisplayName");
                HintTextContent.Text = Language.GetTextValue($"Mods.CoolerItemVisualEffect.WeaponGroup.Help.{key}.Tooltip");
            }
            field = value;
        }
    }

    #region 初始化 开启关闭

    protected override void OnInitialize()
    {
        base.OnInitialize();
        Instance = this;
    }

    private void InitializeComponentExtra()
    {
        RectangleRender.ShadowColor = Color.Black * .1f;
        RectangleRender.ShadowSize = 12f;
        BackgroundColor = Color.CornflowerBlue * .25f;
        BorderColor = SUIColor.Border;
        CloseButton.CrossBorderColor = SUIColor.Border * 0.75f;
        CloseButton.CrossBackgroundColor = SUIColor.Warn * 0.75f;
        CloseButton.CrossBorderHoverColor = SUIColor.Highlight;
        CloseButton.CrossBackgroundHoverColor = SUIColor.Warn;
        TitlePanel.ControlTarget = this;
        Title.Text = Language.GetTextValue("Mods.CoolerItemVisualEffect.WeaponGroup.Help.Help");
        Title.UseDeathText();
        CloseButton.LeftMouseClick += delegate
        {
            Close();
        };
        HintTextTitle.Text = Language.GetTextValue("Mods.CoolerItemVisualEffect.WeaponGroup.Help.HelpPanel.DisplayName");
        HintTextContent.Text = Language.GetTextValue("Mods.CoolerItemVisualEffect.WeaponGroup.Help.HelpPanel.Tooltip");
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

    public static void Close()
    {
        if (Active)
            SoundEngine.PlaySound(SoundID.MenuClose);
        Active = false;
    }

    #endregion 初始化 开启关闭

    #region 开启UI的淡入淡出等

    protected override void UpdateStatus(GameTime gameTime)
    {
        HandleTextManually();


        if (Active) SwitchTimer.StartUpdate();
        else SwitchTimer.StartReverseUpdate();

        SwitchTimer.Update(gameTime);

        UseRenderTarget = SwitchTimer.IsUpdating;
        Opacity = SwitchTimer.Lerp(0f, 1f);

        var center = Bounds.Center * Main.UIScale;
        RenderTargetMatrix =
            Matrix.CreateTranslation(-center.X, -center.Y, 0) *
            Matrix.CreateScale(SwitchTimer.Lerp(0.95f, 1f), SwitchTimer.Lerp(0.95f, 1f), 1) *
            Matrix.CreateTranslation(center.X, center.Y, 0);
        base.UpdateStatus(gameTime);
    }

    #endregion 开启UI的淡入淡出等

    #region 毛玻璃效果

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (BlurMakeSystem.BlurAvailable)
        {
            if (BlurMakeSystem.SingleBlur)
            {
                var batch = Main.spriteBatch;
                batch.End();
                BlurMakeSystem.KawaseBlur();
                batch.Begin();
            }

            SDFRectangle.SampleVersion(BlurMakeSystem.BlurRenderTarget,
                Bounds.Position * Main.UIScale, Bounds.Size * Main.UIScale, BorderRadius * Main.UIScale, Matrix.Identity);
        }

        base.Draw(gameTime, spriteBatch);
    }

    #endregion 毛玻璃效果


    #region 手动显示提示文本
    static void HandleTextManually()
    {
        if (!WeaponGroupManagerUI.Active) return;
        Vector2 mousePosition = Main.MouseScreen;
        var instance = WeaponGroupManagerUI.Instance;
    }
    #endregion
}