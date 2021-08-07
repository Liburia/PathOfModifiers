using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers
{
    public class PoMPrefix : ModPrefix
    {
        public override PrefixCategory Category { get { return PrefixCategory.Custom; } }

        public PoMPrefix()
        {

        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }

        public override bool CanRoll(Item item)
        {
            return false;
        }
    }
}