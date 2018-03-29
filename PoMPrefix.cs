using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers
{
    public class PoMPrefix : ModPrefix
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override bool CanRoll(Item item)
        {
            return false;
        }
        
        public override PrefixCategory Category { get { return PrefixCategory.Custom; } }

        public PoMPrefix()
        {
        }
    }
}