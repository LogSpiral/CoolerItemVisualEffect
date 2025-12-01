using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.Layout;
using SilkyUIFramework.Elements;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
namespace CoolerItemVisualEffect.UIBase.WeaponGroup;

public class OptionWeaponList : PropertyOption
{
    private SUIScrollView ItemList { get; set; }
    protected override void FillOption()
    {
        base.FillOption();
        FitHeight = true;
        FitWidth = false;
        FlexDirection = FlexDirection.Row;
        FlexWrap = true;
        MainAlignment = MainAlignment.Start;


        ItemList = new SUIScrollView()
        {
            Width = new Dimension(0, 1),
            BackgroundColor = Color.Black * .2f,
            BorderRadius = new Vector4(8f),
            Height = new Dimension(200, 0)
        };
        ItemList.Container.MainAlignment = MainAlignment.Start;
        ItemList.Container.Padding = new Margin(8);
        ItemList.Container.Gap = new Vector2(8);
        SetupList();
        Add(ItemList);
    }


    private void SetupList()
    {
        ItemList.Container.RemoveAllChildren();

        List<string> weaponList = GetValue() as List<string>;
        int counter = 0;
        foreach (var item in weaponList)
        {
            ItemList.Container.Add(ItemNameToIcon(item, counter));
            counter++;
        }
        ItemList.Container.Add(ItemNameToIcon(null, counter));
    }

    private SUIImage ItemNameToIcon(string itemName, int idx)
    {
        var image = new SUIImage()
        {
            BorderRadius = new(8f),
            BackgroundColor = Color.Black * .2f,
            BorderColor = Color.White * .5f,
            ImageAlign = new Vector2(0.5f)
        };
        image.SetSize(60, 60);
        image.TextureChanged += (sender, oldTexture, newTexure) =>
        {
            if (newTexure.Width() > 44 || newTexure.Height() > 44)
            {
                var max = MathF.Max(newTexure.Width(), newTexure.Height());
                image.ImageScale = new Vector2(44f / max);
            }
        };
        image.OnUpdateStatus += delegate
        {
            image.ImageColor = Color.White * image.HoverTimer.Lerp(.5f,1f);
        };
        if (!string.IsNullOrEmpty(itemName))
        {
            var type = int.TryParse(itemName, out var t) ? t : ModContent.TryFind<ModItem>(itemName, out var result) ? result.Type : 0;
            Main.instance.LoadItem(type);
            image.Texture2D = TextureAssets.Item[type];
        }
        image.LeftMouseClick += delegate
        {
            int index = idx;
            List<string> list = GetValue() as List<string>;
            if (Main.mouseItem.type != ItemID.None)
            {
                if (Main.mouseItem != null && Main.mouseItem.damage > 0 && Main.mouseItem.useTime > 0)
                {
                    var content = Main.mouseItem.ModItem?.FullName ?? Main.mouseItem.type.ToString();
                    if (!list.Contains(content))
                    {
                        if (index < list.Count)
                        {
                            list[index] = content;
                            image.Texture2D = TextureAssets.Item[Main.mouseItem.type];
                        }
                        else
                        {
                            list.Add(content);
                            SetupList();
                        }
                        SetValue(list);
                    }
                }
            }
            else
            {
                if (index < list.Count)
                {
                    list.RemoveAt(index);
                    SetupList();
                    SetValue(list);
                }
            }
        };
        return image;
    }
}