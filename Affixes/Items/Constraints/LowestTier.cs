using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using System.IO;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.UI;
using System.Linq;

namespace PathOfModifiers.Affixes.Items.Constraints
{
    public class LowestTier : Constraint
    {
        protected override IEnumerable<Affix> ProcessInner(IEnumerable<Affix> input)
        {
            if (input.Any())
            {
                return new[] {
                    input.Aggregate(delegate (Affix a1, Affix a2)
                    {
                        int t1 = a1 is AffixTiered at1 ? at1.CompoundTier : 0;
                        int t2 = a2 is AffixTiered at2 ? at2.CompoundTier : 0;
                        return t1 < t2 ? a1 : a2;
                    })
                };
            }
            else
            {
                return new Affix[] { };
            }
        }
    }
}
