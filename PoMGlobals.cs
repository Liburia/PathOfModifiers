using PathOfModifiers.Affixes.Items;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace PathOfModifiers
{
    public static class PoMGlobals
    {
        public const int ailmentDuration = 300;
        public const float lowHPThreshold = 0.5f;

        public const double tickMS = 1000 / 60d;


        public static class Path
        {
            public static class Image
            {
                public static class UI
                {
                    public const string CloseButton = "PathOfModifiers/UI/Elements/CloseButton";
                    public const string AddButton = "PathOfModifiers/UI/Elements/AddButton";
                }

                public const string MapIcons = "PathOfModifiers/Images/MapIcons/";
            }
        }

        public static class DropRate
        {
            public class FragmentDropScalingWithValue : CommonDrop
            {
                float multiplyValueBy;

                public FragmentDropScalingWithValue(int itemId, int chanceDenominator, int amountDroppedMinimum, int amountDroppedMaximum, float multiplyValueBy = 0.005f)
                    : base(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum)
                {
                    this.multiplyValueBy = multiplyValueBy;
                }

                public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
                {
                    ItemDropAttemptResult result;
                    if (info.player.RollLuck(chanceDenominator) < chanceNumerator)
                    {
                        var playerMultiplier = info.player.GetModPlayer<ItemPlayer>().fragmentDropMultiplier;
                        var min = (int)(amountDroppedMinimum * info.npc.value * multiplyValueBy * playerMultiplier);
                        var max = (int)(amountDroppedMaximum * info.npc.value * multiplyValueBy * playerMultiplier);
                        CommonCode.DropItem(info, itemId, info.rng.Next(Math.Max(min, 1), max + 1));
                        result = default(ItemDropAttemptResult);
                        result.State = ItemDropAttemptResultState.Success;
                        return result;
                    }

                    result = default(ItemDropAttemptResult);
                    result.State = ItemDropAttemptResultState.FailedRandomRoll;
                    return result;
                }
            }

            public static class Fragment
            {
                public const int chanceDenominator = 7;
                public const int baseMin = 1;
                public const int baseMax = 5;
                public const float multiplyPerValue = 0.003f;
                public const float multiplyPerValueBoss = 0.00025f;
                public const float multiplyPerValueBossHardmode = 0.0005f;
                public const float multiplyPerValueBossPostPlantera = 0.001f;
            }
        }
    }
}
