using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;

namespace PathOfModifiers.Buffs
{
    public class Adrenaline : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("'+50% damage, +50% attack speed, +30% move speed, -10% damage taken");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.allDamage += 0.5f;

            var affixItemPlayer = player.GetModPlayer<AffixItemPlayer>();
            affixItemPlayer.damageTaken += -0.1f;
            affixItemPlayer.useSpeed += 0.5f;
            affixItemPlayer.moveSpeed += 0.3f;
        }
    }
}
