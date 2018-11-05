using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using PathOfModifiers.Affixes;
using PathOfModifiers.Rarities;
using PathOfModifiers.Maps.Generators;
using Terraria.ModLoader.IO;
using Terraria.ID;

namespace PathOfModifiers.Maps
{
    public class Test : Map
    {
        public override Type generatorType => typeof(Generators.Test);

        public override void Generate(Point pos)
        {
            size = new Point(100, 50);
            base.Generate(pos);
        }
    }
}