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
            public class CommonDropScalingWithValue : CommonDrop
            {
                float multiplyValueBy;

                public CommonDropScalingWithValue(int itemId, int chanceDenominator, int amountDroppedMinimum, int amountDroppedMaximum, float multiplyValueBy = 0.005f)
                    : base(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum)
                {
                    this.multiplyValueBy = multiplyValueBy;
                }

                public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
                {
                    ItemDropAttemptResult result;
                    if (info.player.RollLuck(chanceDenominator) < chanceNumerator)
                    {
                        var min = (int)(amountDroppedMinimum * info.npc.value * multiplyValueBy);
                        var max = (int)(amountDroppedMaximum * info.npc.value * multiplyValueBy);
                        CommonCode.DropItem(info, itemId, info.rng.Next(min, max + 1));
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
                public const float multiplyPerValue = 0.005f;
                public const int fromBoss = 50;
                public const int fromBossHardmode = 100;
                public const int fromBossPostPlantera = 200;
            }
        }
    }
}
