using LogSpiralLibrary.CodeLibrary.ConfigModification;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Graphics2D;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace CoolerItemVisualEffect.UI.ConfigSaveLoader;

public partial class ConfigSaveLoaderUI
{
    public AnimationTimer SwitchTimer { get; init; } = new(3);

    private PropertyOption previewingOption { get; set; }

    #region 开启UI的淡入淡出等

    protected override void UpdateStatus(GameTime gameTime)
    {
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

        if (CurrentEditTarget != null && previewingOption != null && previewingOption.IsMouseHovering)
        {
            var meta = previewingOption.MetaData;
            OptionMetaData metaData = 
                new(
                    meta.VariableInfo,
                    meta.Item, 
                    meta is PropertyOption.ListValueHandler listHandler ? listHandler.List : null, 
                    meta is PropertyOption.ListValueHandler listHandler2 ? listHandler2.Index : -1, 
                    CurrentEditTarget);
            var pvAttribute = metaData.GetAttribute<CustomPreviewAttribute>();
            var bounds = previewingOption.Bounds;
            if (pvAttribute != null)
                ConfigPreviewSystem.PreviewDrawing(pvAttribute, new CalculatedStyle(bounds.Right + 40, bounds.Y, 480, 240), metaData);
        }
    }

    #endregion 毛玻璃效果
}