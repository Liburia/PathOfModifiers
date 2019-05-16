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
    //TODO: deal with collisionless NPCs(can't force collisions because players can shoot outside the map)
    public class Pack
    {
        public Tuple<int, int>[] npcCounts;
        public float weight;
        public float radius;
        public bool clearSpace;

        public Pack(Tuple<int, int>[] npcCounts, float weight = 1, float radius = 100, bool clearSpace = true)
        {
            this.npcCounts = npcCounts;
            this.weight = weight;
            this.radius = radius;
            this.clearSpace = clearSpace;
        }
    }

    public class OpenMap
    {
        public readonly Rectangle dimensions;

        public OpenMap(Rectangle dimensions)
        {
            this.dimensions = dimensions;
        }
    }

    public class Map
    {
        public Mod mod;

        public Texture2D iconTexture { get; private set; }
        public virtual string iconTextureName => GetType().Name;

        public virtual Type generatorType => typeof(Generator);

        /// <summary>
        /// Per 10k tiles
        /// </summary>
        public virtual float baseNPCFrequency => 0;
        public virtual Pack[] packs => new Pack[0];
        public virtual Pack[] bossPacks => new Pack[0];

        Generator _generator;
        public virtual Generator generator
        {
            get
            {
                if (_generator == null)
                    _generator = PoMDataLoader.generators[PoMDataLoader.generatorMap[generatorType]];
                return _generator;
            }
        }
        
        public OpenMap openMap;
        public bool isOpened => openMap != null;

        /// <summary>
        /// Load the texture from textureName
        /// </summary>
        public virtual void Initialize()
        {
            if (Main.netMode != NetmodeID.Server)
                iconTexture = mod.GetTexture($"{PathOfModifiers.pathMapIcons}{iconTextureName}");
        }

        //Drawn the map icon on top of the map
        public virtual void DrawIcon(SpriteBatch spriteBatch, Vector2 mapPosition, Vector2 mapSize, float rotation, float scale)
        {
            var iconTextureSize = iconTexture.Size();
            var iconHalfSize = iconTextureSize / 2;
            Vector2 iconPosition = mapPosition + (((mapSize - iconTextureSize) / 2 + iconHalfSize) * scale);
            spriteBatch.Draw(
                iconTexture,
                iconPosition,
                null,
                Color.Red,
                rotation,
                iconHalfSize,
                scale,
                SpriteEffects.None,
                0);
        }

        public virtual bool Open(Rectangle dimensions)
        {
            if (isOpened)
                throw new Exception("Cannot open map because it is alread opened.");

            //PoMWorld pomWorld = PathOfModifiers.Instance.GetModWorld<PoMWorld>();
            //int ID = pomWorld.AddOpenMap(this);

            //if (ID < 0)
            //    return false;

            generator.GenerateTerrain(dimensions);
            int npcs = GetNPCFrequency(dimensions);
            generator.SpawnPacks(dimensions, npcs, MakePackArray(npcs));


            openMap = new OpenMap(dimensions);


            //This method should never run on a client, so only case is SP/Server
            if (Main.netMode == NetmodeID.Server)
                PoMNetMessage.SyncOpenedMap(dimensions);

            return true;
        }

        public virtual void Close()
        {
            if (!isOpened)
                throw new Exception("Cannot close map because it is not opened.");

            generator.ClearMap(openMap.dimensions);


            if (Main.netMode == NetmodeID.Server)
                PoMNetMessage.SyncOpenedMap(openMap.dimensions, true);


            //PoMWorld pomWorld = PathOfModifiers.Instance.GetModWorld<PoMWorld>();
            //pomWorld.RemoveOpenMap(openMap.ID);

            openMap = null;
        }

        public virtual Pack[] MakePackArray(int npcs)
        {
            List<Pack> wPacks = new List<Pack>();

            Tuple<Pack, double>[] packsWeights = packs
                .Where(p => p.weight > 0)
                .Select(p => new Tuple<Pack, double>(p, p.weight))
                .ToArray();
            WeightedRandom<Pack> wRandom = new WeightedRandom<Pack>(Main.rand, packsWeights);

            int npcsRemaining = npcs;
            for (int i = 0; i < bossPacks.Length; i++)
            {
                Pack p = bossPacks[i];
                for (int j = 0; j < p.npcCounts.Length; j++)
                {
                    npcsRemaining -= p.npcCounts[i].Item2;
                }
                if (npcsRemaining >= 0)
                    wPacks.Add(p);
            }
            while (npcsRemaining > 0)
            {
                Pack p = wRandom;
                for(int i = 0; i < p.npcCounts.Length; i++)
                {
                    npcsRemaining -= p.npcCounts[i].Item2;
                }
                if (npcsRemaining >= 0)
                    wPacks.Add(p);
            }

            return wPacks.ToArray();
        }

        public int GetNPCFrequency(Rectangle dimensions)
        {
            return (int)Math.Round(baseNPCFrequency * ((dimensions.Width + 1) * (dimensions.Height + 1) / 10000f)) + 1;
        }

        public virtual Item MakeItem()
        {
            return new Item();
        }

        public virtual void Save(TagCompound tag)
        {
            tag.Set("isOpened", isOpened);

            if (isOpened)
            {
                var openMapTag = new TagCompound();
                openMapTag.Set("dimensions", openMap.dimensions);
                //openMapTag.Set("ID", openMap.ID);
                //TODO: save/load NPCs
                tag.Set("openMap", openMapTag);
            }
        }
        public virtual void Load(TagCompound tag)
        {
            Initialize();
            if (tag.GetBool("isOpened"))
            {
                var openMapTag = tag.GetCompound("openMap");

                //PoMWorld pomWorld = PathOfModifiers.Instance.GetModWorld<PoMWorld>();

                //int ID = openMapTag.GetInt("ID");
                //pomWorld.AddOpenMap(this, ID);

                openMap = new OpenMap(openMapTag.Get<Rectangle>("dimensions"));
            }
        }

        public virtual Map Clone()
        {
            Map newMap = (Map)Activator.CreateInstance(GetType());
            newMap.mod = mod;
            newMap.iconTexture = iconTexture;
            newMap._generator = generator;
            newMap.openMap = null;
            return newMap;
        }

        public virtual void NetSend(BinaryWriter writer)
        {
            writer.Write(isOpened);
            if (isOpened)
            {
                //writer.Write(openMap.ID);
                writer.Write(openMap.dimensions.X);
                writer.Write(openMap.dimensions.Y);
                writer.Write(openMap.dimensions.Width);
                writer.Write(openMap.dimensions.Height);
            }
            //TODO: send/receive NPCs
        }
        public virtual void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                //PoMWorld pomWorld = PathOfModifiers.Instance.GetModWorld<PoMWorld>();

                //int ID = reader.ReadInt32();
                //pomWorld.AddOpenMap(this, ID, true);
                openMap = new OpenMap(
                    new Rectangle(
                        reader.ReadInt32(),
                        reader.ReadInt32(),
                        reader.ReadInt32(),
                        reader.ReadInt32()));
            }
        }
    }
}