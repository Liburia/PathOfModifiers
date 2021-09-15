using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;

namespace PathOfModifiers.Buffs
{
    public class Adrenaline : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("'+50% damage, +30% attack speed, +30% move speed, -10% damage taken");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<GenericDamageClass>() *= 1.5f;

            var affixItemPlayer = player.GetModPlayer<ItemPlayer>();
            affixItemPlayer.damageTaken += -0.1f;
            affixItemPlayer.useSpeed += 0.3f;
            affixItemPlayer.moveSpeed += 0.3f;
        }
    }
}
