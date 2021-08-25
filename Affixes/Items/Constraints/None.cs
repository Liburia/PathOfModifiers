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
    public class None : Constraint
    {
        public override Constraint Then(Constraint nextConstraint)
        {
            return nextConstraint;
        }
    }
}
