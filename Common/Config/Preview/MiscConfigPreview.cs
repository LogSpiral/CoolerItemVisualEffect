using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.ConfigModification;
using LogSpiralLibrary.CodeLibrary.Utilties.Extensions;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;

namespace CoolerItemVisualEffect.Common.Config.Preview;

public abstract class MiscPreview<T> : SimplePreview<T>
{
    public override bool UsePreview => MiscConfig.Instance.UsePreview;
}

public class WeaponDisplayPreview : MiscPreview<bool>
{
    private Player plr;

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, bool data, OptionMetaData metaData)
    {
        if (plr == null)
        {
            plr = new Player
            {
                armor =
                {
                    [1] = new Item(ItemID.HallowedPlateMail),
                    [2] = new Item(ItemID.NebulaLeggings),
                    [3] = new Item(ItemID.HeroShield),
                    [4] = new Item(ItemID.PrinceCape),
                    [5] = new Item(ItemID.LeinforsWings)
                },
                dye =
                {
                    [1] = new Item(ItemID.ReflectiveSilverDye),
                    [3] = new Item(ItemID.PurpleDye),
                    [4] = new Item(ItemID.PurpleDye)
                },
                skinColor = new Color(255, 125, 90, 255),
                eyeColor = new Color(38, 38, 38, 255),
                hairColor = new Color(38, 38, 38, 255),
                hair = 85
            };
            //plr.isFirstFractalAfterImage = true;
            plr.ResetVisibleAccessories();
            plr.UpdateDyes();
            plr.DisplayDollUpdate();
            plr.UpdateSocialShadow();
            plr.PlayerFrame();
            plr.active = true;
            plr.inventory[0].SetDefaults(ItemID.TerraBlade);
        }
        var s = MiscConfig.Instance.weaponScale;
        MiscConfig.Instance.weaponScale = PreviewHelper.WeaponScalePVAssistant;
        plr.inventory[0].damage = data ? 1 : 0;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        Main.PlayerRenderer.DrawPlayer(Main.Camera, plr, dimension.Center() + Main.screenPosition - new Vector2(21, 28), 0, default);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        MiscConfig.Instance.weaponScale = s;
    }
}

public class WeaponScalePreview : MiscPreview<float>
{
    private Player plr;

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, float data, OptionMetaData metaData)
    {
        if (plr == null)
        {
            plr = new Player
            {
                armor =
                {
                    [1] = new Item(ItemID.HallowedPlateMail),
                    [2] = new Item(ItemID.NebulaLeggings),
                    [3] = new Item(ItemID.HeroShield),
                    [4] = new Item(ItemID.PrinceCape),
                    [5] = new Item(ItemID.LeinforsWings)
                },
                dye =
                {
                    [1] = new Item(ItemID.ReflectiveSilverDye),
                    [3] = new Item(ItemID.PurpleDye),
                    [4] = new Item(ItemID.PurpleDye)
                },
                skinColor = new Color(255, 125, 90, 255),
                eyeColor = new Color(38, 38, 38, 255),
                hairColor = new Color(38, 38, 38, 255),
                hair = 85
            };
            //plr.isFirstFractalAfterImage = true;
            plr.ResetVisibleAccessories();
            plr.UpdateDyes();
            plr.DisplayDollUpdate();
            plr.UpdateSocialShadow();
            plr.PlayerFrame();
            plr.active = true;
            plr.inventory[0].SetDefaults(ItemID.TerraBlade);
        }
        var s = MiscConfig.Instance.weaponScale;
        MiscConfig.Instance.weaponScale = data;
        PreviewHelper.WeaponScalePVAssistant = data;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        Main.PlayerRenderer.DrawPlayer(Main.Camera, plr, dimension.Center() + Main.screenPosition - new Vector2(21, 28), 0, default);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        MiscConfig.Instance.weaponScale = s;
    }
}

public class ItemEffectPreview : MiscPreview<bool>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, bool data, OptionMetaData metaData)
    {
        var center = dimension.Center() + new Vector2(-144, 80);
        if (Main.gameMenu)
        {
            GlobalTimeSystem.GlobalTime += .33f;
            center += new Vector2(880, 280);
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        spriteBatch.Draw(TextureAssets.Item[ItemID.TerraBlade].Value, center, null, Color.White, 0, new Vector2(23, 27), 1, 0, 0);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        if (data)
        {
            var item = new Item(ItemID.TerraBlade)
            {
                Center = center + Main.screenPosition + new Vector2(0, 12)
            };
            item.ShaderItemEffectInWorld(spriteBatch, LogSpiralLibraryMod.Misc[0].Value, Color.Green, 0);
        }
    }
}

public class ProjectileModificationPreview : MiscPreview<bool>
{
    public static bool PVDrawing;

    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, bool data, OptionMetaData metaData)
    {
        var terraBeam = new Projectile();
        terraBeam.SetDefaults(ProjectileID.TerraBeam);
        terraBeam.Center = dimension.Center() + Main.screenPosition;
        if (Main.gameMenu)
        {
            GlobalTimeSystem.GlobalTime += .33f;
            terraBeam.Center += new Vector2(880, 280);
        }

        terraBeam.alpha = 0;
        terraBeam.velocity = new Vector2(8, -8);

        terraBeam.position += new Vector2(-40, 40);
        var flag = MiscConfig.Instance.VanillaProjectileDrawModifyActive;
        PVDrawing = true;
        MiscConfig.Instance.VanillaProjectileDrawModifyActive = data;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.DrawProjDirect(terraBeam);
        terraBeam.type = ProjectileID.SuperStar;
        terraBeam.position.X -= 120;
        terraBeam.position.Y -= 40;

        for (var n = 0; n < terraBeam.oldPos.Length; n++)
        {
            terraBeam.oldPos[n] = terraBeam.Center + new Vector2(-16, 16) * n;
            terraBeam.oldRot[n] = n * .15f;
        }
        Main.instance.DrawProjDirect(terraBeam);
        var pendVec = terraBeam.position;
        terraBeam.SetDefaults(ProjectileID.HolyArrow);
        terraBeam.position = pendVec + new Vector2(128);
        terraBeam.rotation = MathHelper.PiOver2;
        terraBeam.velocity = new Vector2(1, 0);
        for (var n = 0; n < terraBeam.oldPos.Length; n++)
        {
            terraBeam.oldPos[n] = terraBeam.Center + new Vector2(-16, 0) * n;
            terraBeam.oldRot[n] = MathHelper.PiOver2;
        }
        Main.instance.DrawProjDirect(terraBeam);

        //DrawingMethods.DrawQuadraticLaser_PassNormal(spriteBatch, terraBeam.Center, Vector2.UnitX, Color.Red, LogSpiralLibraryMod.AniTex[10].Value);
        //Color color = Color.White;
        //Main.instance.DrawProj_DrawNormalProjs(terraBeam, terraBeam.Center.X, terraBeam.Center.Y, terraBeam.Center, ref color);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        MiscConfig.Instance.VanillaProjectileDrawModifyActive = flag;
        PVDrawing = false;
    }
}

public class TeleportModificationPreview : MiscPreview<bool>
{
    public override void Draw(SpriteBatch spriteBatch, CalculatedStyle dimension, bool data, OptionMetaData metaData)
    {
        if (Main.gameMenu)
            GlobalTimeSystem.GlobalTime += .33;
        var fac = (float)(LogSpiralLibraryMod.ModTime % 60 / 60);
        var _fac = (fac * 2 % 1).HillFactor2() * (fac < .5f ? .5f : 1f);
        var rotation = (float)LogSpiralLibraryMod.ModTime * .05f;
        var scale = 3f * _fac;
        SpriteEffects dir = 0;
        var mainColor = Color.Cyan;

        var center = dimension.Center();

        var colorVortex = mainColor * 0.8f;
        colorVortex.A /= 2;
        var color1 = Color.Lerp(mainColor, Color.Black, 0.5f);
        color1.A = mainColor.A;
        var sinValue = 0.95f + (rotation * 0.75f).ToRotationVector2().Y * 0.1f;
        color1 *= sinValue;
        var scale1 = 0.6f + scale * 0.6f * sinValue;
        var voidTex = ModAsset.Extra_50.Value;
        var voidOrigin = voidTex.Size() / 2f;
        var vortexTex = ModAsset.Projectile_578.Value;//TextureAssets.Projectile[ProjectileID.DD2ApprenticeStorm].Value;//;
        spriteBatch.Draw(voidTex, center, null, color1, -rotation + 0.35f, voidOrigin, scale1, dir ^ SpriteEffects.FlipHorizontally, 0);
        spriteBatch.Draw(voidTex, center, null, mainColor, -rotation, voidOrigin, scale, dir ^ SpriteEffects.FlipHorizontally, 0);
        spriteBatch.Draw(voidTex, center, null, mainColor * 0.8f, rotation * 0.5f, voidOrigin, scale * 0.9f, dir, 0);
        spriteBatch.Draw(vortexTex, center, null, colorVortex, -rotation * 0.7f, vortexTex.Size() * .5f, scale, dir ^ SpriteEffects.FlipHorizontally, 0);
        spriteBatch.Draw(vortexTex, center, null, Color.White with { A = 0 }, -rotation * 1.4f, vortexTex.Size() * .5f, scale * .85f, dir ^ SpriteEffects.FlipHorizontally, 0);
    }
}