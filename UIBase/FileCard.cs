using SilkyUIFramework.Elements;
using System.IO;
using Terraria.Audio;

namespace CoolerItemVisualEffect.UIBase;

public partial class FileCard:UIElementGroup
{
    public FileCard()
    {
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
    }

    private void RenameFile(object sender, SilkyUIFramework.ValueChangedEventArgs<string> e)
    {
        var oldPath = Path.Combine(FileFolder, $"{e.OldValue}{FileExtension}");
        var newPath = Path.Combine(FileFolder, $"{e.NewValue}{FileExtension}");
        if(File.Exists(oldPath))
            File.Move(oldPath, newPath);
    }

    public string FileName
    {
        get => NameBox.InnerText.Text;
        set => NameBox.InnerText.Text = value;
    }

    public string FileFolder { get; init; }

    public string FileExtension { get; init; }
}