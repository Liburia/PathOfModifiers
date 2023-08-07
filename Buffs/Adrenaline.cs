using PathOfModifiers.Affixes.Items;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Buffs
{
    public class Adrenaline : ModBuff
    {
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
