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
    public abstract class Constraint
    {
        List<Constraint> nextConstraints = new List<Constraint>();
        public virtual Constraint Then(Constraint nextConstraint)
        {
            nextConstraints.Add(nextConstraint);
            return this;
        }

        public IEnumerable<Affix> Process(IEnumerable<Affix> input)
        {
            var output = ProcessInner(input);

            foreach (var c in nextConstraints)
            {
                output = c.Process(output);
            }

            return output;
        }

        protected virtual IEnumerable<Affix> ProcessInner(IEnumerable<Affix> input)
        {
            return input;
        }
    }
}
