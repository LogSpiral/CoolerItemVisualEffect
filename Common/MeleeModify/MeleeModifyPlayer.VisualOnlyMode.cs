using CoolerItemVisualEffect.Common.Config;
using LogSpiralLibrary;
using LogSpiralLibrary.CodeLibrary.DataStructures.Drawing.RenderDrawingContents;
using Terraria.GameContent;

namespace CoolerItemVisualEffect.Common.MeleeModify;

public partial class MeleeModifyPlayer
{
    private UltraSwoosh currentSwoosh;
    private UltraSwoosh extraSwoosh;

    public override void PreUpdate()
    {
        configurationSwoosh ??= Main.myPlayer == Player.whoAmI ? MeleeConfig.Instance : new MeleeConfig();

        if (SeverConfig.Instance.meleeModifyLevel != SeverConfig.MeleeModifyLevel.VisualOnly) return;
        if (!configurationSwoosh.SwordModifyActive) return;
        if (!MeleeModifyPlayerUtils.MeleeBroadSwordCheck(Player.HeldItem)) return;

        if (Player.itemAnimationMax != 0 && Player.itemAnimation == Player.itemAnimationMax)
        {
            var timeLeft = configurationSwoosh.swooshTimeLeft;
            var length = TextureAssets.Item[Player.HeldItem.type].Value.Size().Length() * 1.2f;
            var aniIdx = configurationSwoosh.animateIndexSwoosh;
            var baseIdx = configurationSwoosh.baseIndexSwoosh;
            var alphaVec = configurationSwoosh.colorVector.AlphaVector;
            var eVec = alphaVec with { Y = 0 };
            if (eVec.X == 0 && eVec.Z == 0)
                eVec = new Vector3(.5f, 0, .5f);

            var canvasName = GetCanvasNameViaID(Player.whoAmI);

            var swoosh = currentSwoosh = UltraSwoosh.NewUltraSwoosh(canvasName, timeLeft, length, Player.Center, (-1.5f, .25f));
            swoosh.heatMap = HeatMap;
            swoosh.negativeDir = Player.direction != -1;
            swoosh.rotation = 0f;
            swoosh.xScaler = 1f;
            swoosh.aniTexIndex = aniIdx;
            swoosh.baseTexIndex = baseIdx;
            swoosh.ColorVector = eVec;

            if (Player.HeldItem.type == ItemID.TrueExcalibur)
            {
                swoosh = extraSwoosh = UltraSwoosh.NewUltraSwoosh(canvasName, timeLeft, length * 1.5f, Player.Center, (-1.5f, .25f));
                swoosh.heatMap = LogSpiralLibraryMod.HeatMap[5].Value;
                swoosh.negativeDir = Player.direction != -1;
                swoosh.rotation = 0f;
                swoosh.xScaler = 1f;
                swoosh.aniTexIndex = aniIdx;
                swoosh.baseTexIndex = baseIdx;
                swoosh.ColorVector = alphaVec;
            }
        }
        if (Player.itemAnimation > 0 && currentSwoosh != null)
        {
            currentSwoosh.timeLeft++;
            var k = 1 - (float)Player.itemAnimation / Player.itemAnimationMax;

            if (Player.direction == 1)
                currentSwoosh.angleRange = (1.5f - k * .5f, -.0625f - MathHelper.Lerp(-1f, .25f, k));
            else
                currentSwoosh.angleRange = (2.5f - k * .5f, 1 - .0625f - MathHelper.Lerp(-1f, .25f, k));
            currentSwoosh.center = Player.Center;
            currentSwoosh.ColorVector = configurationSwoosh.colorVector.AlphaVector * k;

            if (extraSwoosh != null)
            {
                extraSwoosh.timeLeft++;
                extraSwoosh.angleRange = currentSwoosh.angleRange;
                extraSwoosh.center = currentSwoosh.center;
                var eVec = currentSwoosh.ColorVector with { Y = 0 };
                if (eVec.X == 0 && eVec.Z == 0)
                    eVec = new Vector3(.5f, 0, .5f) * k;
                extraSwoosh.ColorVector = eVec;
            }
        }
        base.PreUpdate();
    }
}
