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

namespace PathOfModifiers.Maps
{
    public class Map
    {
        public Mod mod;

        public virtual Type generatorType => typeof(Generator);

        Generator _generator;
        public Generator generator
        {
            get
            {
                if (_generator == null)
                    _generator = PoMDataLoader.generators[PoMDataLoader.generatorMap[generatorType]];
                return _generator;
            }
        }

        public Point size;

        public virtual Map Clone()
        {
            Map newMap = (Map)Activator.CreateInstance(GetType());
            newMap.mod = mod;
            newMap._generator = generator;
            return newMap;
        }

        public virtual void Generate(Point pos)
        {
            generator.Generate(new Rectangle(pos.X, pos.Y, size.X, size.Y));
        }
        
        public virtual Item MakeItem()
        {
            return new Item();
        }

        public virtual void Save(TagCompound tag)
        {
        }
        public virtual void Load(TagCompound tag)
        {
        }

        public virtual void NetSend(BinaryWriter writer)
        {
        }
        public virtual void NetReceive(BinaryReader reader)
        {
        }
    }
}