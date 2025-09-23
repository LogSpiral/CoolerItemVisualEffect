using SilkyUIFramework.Elements;
using System.IO;
using Terraria.Audio;
using Terraria.Localization;

namespace CoolerItemVisualEffect.UIBase;

public partial class FileCard : UIElementGroup
{
    public FileCard()
    {
        static void SetHoverEffect(SUIImage image) => image.OnUpdateStatus += delegate { image.ImageColor = Color.White * image.HoverTimer.Lerp(.5f, 1f); };
        InitializeComponent();
        EditButton.Texture2D = ModAsset.EditIcon;
        DeleteButton.Texture2D = ModAsset.DeleteIcon;
        NameBox.BackgroundColor = Color.Transparent;
        NameBox.InnerText.EndTakingInput += RenameFile;
        DeleteButton.LeftMouseClick += delegate
        {
            var filePath = Path.Combine(FileFolder, $"{FileName}{FileExtension}");
            if (File.Exists(filePath))
                File.Delete(filePath);
            SoundEngine.PlaySound(SoundID.Tink);
        };
        SetHoverEffect(EditButton);
        SetHoverEffect(DeleteButton);
    }

    private void RenameFile(object sender, SilkyUIFramework.ValueChangedEventArgs<string> e)
    {
        if (e.OldValue == e.NewValue) return;
        var oldPath = Path.Combine(FileFolder, $"{e.OldValue}{FileExtension}");
        var newPath = Path.Combine(FileFolder, $"{e.NewValue}{FileExtension}");
        if (File.Exists(newPath))
        {
            if (sender is SUIEditText text)
            {
                text.Text = e.OldValue;
                Main.NewText(Language.GetTextValue("Mods.CoolerItemVisualEffect.ConfigSaveLoader.RenameTip.Exists"), Color.Red);
            }
            return;
        }
        if (File.Exists(oldPath))
            File.Move(oldPath, newPath);
    }

    public string FileName
    {
        get => NameBox.InnerText.Text;
        set => NameBox.InnerText.Text = value;
    }

    public string FileFolder { get; init; }

    public string FileExtension { get; init; }

    public string FileFullPath => Path.Combine(FileFolder, $"{FileName}{FileExtension}");
}