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
using PathOfModifiers.Maps;
using PathOfModifiers.Maps.Generators;

namespace PathOfModifiers
{
	public static class PoMDataLoader
    {
        public static List<Mod> mods = new List<Mod>();

        public static Dictionary<Type, int> affixMap;
        public static Affix[] affixes;
        public static Dictionary<Type, int> rarityMap;
        public static Rarity[] rarities;
        public static Dictionary<Type, int> generatorMap;
        public static Generator[] generators;
        public static Dictionary<Type, int> mapMap;
        public static Map[] maps;

        public static void Initialize()
        {
            LoadData();
        }

        public static void RegisterMod(Mod mod)
        {
            mods.Add(mod);
        }
        /// <summary>
        /// Loads affixes and rarities from loaded mods
        /// </summary>
        public static void LoadData()
        {
            affixMap = new Dictionary<Type, int>();
            List<Affix> affixList = new List<Affix>();

            rarityMap = new Dictionary<Type, int>();
            List<Rarity> rarityList = new List<Rarity>();

            generatorMap = new Dictionary<Type, int>();
            List<Generator> generatorList = new List<Generator>();

            mapMap = new Dictionary<Type, int>();
            List<Map> mapList = new List<Map>();

            int affixIndex = 0;
            int rarityIndex = 0;
            int mapIndex = 0;
            int generatorIndex = 0;
            Affix affix;
            Rarity rarity;
            Generator generator;
            Map map;
            foreach (Mod mod in mods)
            {
                var types = mod.Code.GetTypes().Where(t => t.IsClass && !t.IsAbstract);

                foreach(Type t in types)
                {
                    if (t.IsSubclassOf(typeof(Affix)) || t == typeof(Affix))
                    {
                        affix = (Affix)Activator.CreateInstance(t);
                        affix.mod = mod;
                        affixList.Add(affix);
                        affixMap.Add(t, affixIndex);
                        if (PathOfModifiers.logLoad)
                            PathOfModifiers.Log($"PathOfModifiers: Added affix {t.FullName} with index {affixIndex} from mod {mod.Name}");
                        affixIndex++;
                    }
                    else if(t.IsSubclassOf(typeof(Rarity)) && t != typeof(Rarity))
                    {
                        rarity = (Rarity)Activator.CreateInstance(t);
                        rarity.mod = mod;
                        rarityList.Add(rarity);
                        rarityMap.Add(t, rarityIndex);
                        if (PathOfModifiers.logLoad)
                            PathOfModifiers.Log($"PathOfModifiers: Added rarity {t.FullName} with index {rarityIndex} from mod {mod.Name}");
                        rarityIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(Generator)) && t != typeof(Generator))
                    {
                        generator = (Generator)Activator.CreateInstance(t);
                        generator.mod = mod;
                        generatorList.Add(generator);
                        generatorMap.Add(t, generatorIndex);
                        if (PathOfModifiers.logLoad)
                            PathOfModifiers.Log($"PathOfModifiers: Added generator {t.FullName} with index {generatorIndex} from mod {mod.Name}");
                        generatorIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(Map)) && t != typeof(Map))
                    {
                        map = (Map)Activator.CreateInstance(t);
                        map.mod = mod;
                        mapList.Add(map);
                        mapMap.Add(t, mapIndex);
                        if (PathOfModifiers.logLoad)
                            PathOfModifiers.Log($"PathOfModifiers: Added map {t.FullName} with index {mapIndex} from mod {mod.Name}");
                        mapIndex++;
                    }
                }
            }
            affixes = affixList.ToArray();
            rarities = rarityList.ToArray();
            generators = generatorList.ToArray();
            maps = mapList.ToArray();
        }
        public static void Unload()
        {
            mods = new List<Mod>();

            affixMap = new Dictionary<Type, int>();
            affixes = new Affix[0];

            rarityMap = new Dictionary<Type, int>();
            rarities = new Rarity[0];

            generatorMap = new Dictionary<Type, int>();
            generators = new Generator[0];

            mapMap = new Dictionary<Type, int>();
            maps = new Map[0];
        }

        public static void SendMaps(ModPacket packet)
        {
            packet.Write(affixes.Length);
            if (PathOfModifiers.logNetwork)
                PathOfModifiers.Log($"SendMaps: {affixes.Length} ");
            Affix affix;
            for (int i = 0; i < affixes.Length; i++)
            {
                affix = affixes[i];
                if (PathOfModifiers.logNetwork)
                    PathOfModifiers.Log($"SendMaps: {i} / {affix.GetType().FullName} from mod {affix.mod}");
                packet.Write(affix.mod.Name);
                packet.Write(affix.GetType().FullName);
            }

            packet.Write(rarities.Length);
            Rarity rarity;
            for (int i = 0; i < rarities.Length; i++)
            {
                rarity = rarities[i];
                packet.Write(rarity.mod.Name);
                packet.Write(rarity.GetType().FullName);
            }

            packet.Write(generators.Length);
            Generator generator;
            for (int i = 0; i < generators.Length; i++)
            {
                generator = generators[i];
                packet.Write(generator.mod.Name);
                packet.Write(generator.GetType().FullName);
            }

            packet.Write(maps.Length);
            Map map;
            for (int i = 0; i < maps.Length; i++)
            {
                map = maps[i];
                packet.Write(map.mod.Name);
                packet.Write(map.GetType().FullName);
            }
        }
        public static void ReceiveDataMaps(BinaryReader reader)
        {
            try
            {
                int length = reader.ReadInt32();
                if (PathOfModifiers.logNetwork)
                {
                    PathOfModifiers.Log($"ReceiveMaps: {length} ");
                    PathOfModifiers.Log($"LoadedData: {affixMap.Count} ");
                }

                Dictionary<Type, int> newAffixMap = new Dictionary<Type, int>(length);
                Affix[] newAffixes = new Affix[length];

                Type type;
                Mod mod;
                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    if (PathOfModifiers.logNetwork)
                        PathOfModifiers.Log($"ReceiveMaps: {i} / {type.FullName} from mod {mod}");

                    newAffixes[i] = affixes[affixMap[type]];
                    newAffixMap.Add(type, i);
                }

                affixMap = newAffixMap;
                affixes = newAffixes;

                length = reader.ReadInt32();

                Dictionary<Type, int> newRarityMap = new Dictionary<Type, int>(length);
                Rarity[] newRarities = new Rarity[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newRarities[i] = rarities[rarityMap[type]];
                    newRarityMap.Add(type, i);
                }

                rarityMap = newRarityMap;
                rarities = newRarities;

                length = reader.ReadInt32();

                Dictionary<Type, int> newGenratorMap = new Dictionary<Type, int>(length);
                Generator[] newGenerators = new Generator[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newGenerators[i] = generators[generatorMap[type]];
                    newGenratorMap.Add(type, i);
                }

                rarityMap = newRarityMap;
                rarities = newRarities;

                length = reader.ReadInt32();

                Dictionary<Type, int> newMapMap = new Dictionary<Type, int>(length);
                Map[] newMaps = new Map[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newMaps[i] = maps[mapMap[type]];
                    newMapMap.Add(type, i);
                }

                mapMap = newMapMap;
                maps = newMaps;
            }
            catch(Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
    }
}
