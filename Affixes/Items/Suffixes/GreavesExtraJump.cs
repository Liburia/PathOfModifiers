using Microsoft.Xna.Framework;
using rail;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class GreavesExtraJump : AffixTiered<TTInt>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(1, 1, 3),
                new TTInt.WeightedTier(2, 2, 2.5),
                new TTInt.WeightedTier(3, 3, 2),
                new TTInt.WeightedTier(4, 4, 1.5),
                new TTInt.WeightedTier(5, 5, 1),
                new TTInt.WeightedTier(6, 6, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Hopping", 0.5),
            new WeightedTierName("of Skipping", 1),
            new WeightedTierName("of Bouncing", 1.5),
            new WeightedTierName("of Jumping", 2),
            new WeightedTierName("of Vaulting", 2.5),
            new WeightedTierName("of Leaping", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsLegArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return Language.GetText("Mods.PathOfModifiers.Affixes.Suffixes.GreavesExtraJump").Format( valueRange1 );
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            var jumpPlayer = player.Player.GetModPlayer<GreavesExtraJumpPlayer>();
            jumpPlayer.maxJumpsRemaining = Type1.GetValue();
            player.Player.GetJumpState<GreavesMultipleExtraJump>().Enable();
        }


        public class GreavesMultipleExtraJump : ExtraJump
        {
            public override Position GetDefaultPosition() => new After(BlizzardInABottle);

            public override float GetDurationMultiplier(Player player)
            {
                // Each successive jump has weaker power
                return player.GetModPlayer<GreavesExtraJumpPlayer>().jumpsRemaining * 0.2f;
            }

            public override void OnRefreshed(Player player)
            {
                player.GetModPlayer<GreavesExtraJumpPlayer>().Refresh();
            }

            public override void OnStarted(Player player, ref bool playSound)
            {
                ref int jumps = ref player.GetModPlayer<GreavesExtraJumpPlayer>().jumpsRemaining;

                // Spawn rings of fire particles
                int offsetY = player.height;
                if (player.gravDir == -1f)
                    offsetY = 0;

                offsetY -= 16;

                Vector2 center = player.Top + new Vector2(0, offsetY);

                if (jumps == 1)
                {
                    const int numDusts = 24;
                    for (int i = 0; i < numDusts; i++)
                    {
                        (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians(i * 360 / numDusts));

                        float amplitudeX = cos * (player.width - 4) / 2f;
                        float amplitudeY = sin * 3;

                        Dust dust = Dust.NewDustPerfect(center + new Vector2(amplitudeX, amplitudeY), DustID.BlueFlare, -player.velocity * 0.2f, Scale: 1f);
                        dust.noGravity = true;
                    }
                }
                else if (jumps == 2)
                {
                    const int numDusts = 30;
                    for (int i = 0; i < numDusts; i++)
                    {
                        (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians(i * 360 / numDusts));

                        float amplitudeX = cos * (player.width + 2) / 2f;
                        float amplitudeY = sin * 5;

                        Dust dust = Dust.NewDustPerfect(center + new Vector2(amplitudeX, amplitudeY), DustID.BlueFlare, -player.velocity * 0.35f, Scale: 1f);
                        dust.noGravity = true;
                    }
                }
                else
                {
                    const int numDusts = 40;
                    for (int i = 0; i < numDusts; i++)
                    {
                        (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians(i * 360 / numDusts));

                        float amplitudeX = cos * (player.width + 10) / 2f;
                        float amplitudeY = sin * 6;

                        Dust dust = Dust.NewDustPerfect(center + new Vector2(amplitudeX, amplitudeY), DustID.BlueFlare, -player.velocity * 0.5f, Scale: 1f);
                        dust.noGravity = true;
                    }
                }

                // Play a different sound
                playSound = false;

                float pitch = jumps switch
                {
                    1 => 0.5f,
                    2 => 0.1f,
                    3 => -0.2f,
                    _ => -0.4f,
                };

                SoundEngine.PlaySound(SoundID.Item8 with { Pitch = pitch, PitchVariance = 0.04f }, player.Bottom);

                // Decrement the jump counter
                jumps--;

                // Allow the jump to be used again while the jump counter is > 0
                if (jumps > 0)
                    player.GetJumpState(this).Available = true;
            }
        }

        public class GreavesExtraJumpPlayer : ModPlayer
        {
            public int jumpsRemaining;
            public int maxJumpsRemaining;

            public void Refresh()
            {
                this.jumpsRemaining = this.maxJumpsRemaining;
            }
        }
    }
}
