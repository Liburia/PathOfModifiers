using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using PathOfModifiers;
using System.IO;
using System;
using Terraria.ModLoader.IO;

namespace PathOfModifiers.Items
{
	public class Map : ModItem
	{
        public Maps.Map map;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Map");
			Tooltip.SetDefault("Opens a new world.");
		}
		public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
			item.value = 500000;
			item.rare = 2;
            item.maxStack = 1;
            item.autoReuse = false;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = false;
        }

        void MapNullCheck()
        {
            if (map == null)
                map = PoMDataLoader.maps[PoMDataLoader.mapMap[typeof(Maps.Test)]].Clone();
        }

        public override bool UseItem(Player player)
        {
            //PathOfModifiers.Log("1");
            MapNullCheck();
            map.Generate(new Point(Player.tileTargetX, Player.tileTargetY));

            //PathOfModifiers.Log("2");
            return true;
        }

        public override TagCompound Save()
        {
            //PathOfModifiers.Log("3");
            MapNullCheck();
            TagCompound tag = new TagCompound();
            TagCompound mapTag;
            mapTag = new TagCompound();
            mapTag.Set("mapMod", map.mod.Name);
            mapTag.Set("mapFullName", map.GetType().FullName);
            map.Save(mapTag);
            tag.Set("mapTag", mapTag);
            //PathOfModifiers.Log("4");
            return tag;
        }
        public override void Load(TagCompound tag)
        {
            //PathOfModifiers.Log("5");
            TagCompound mapTag = tag.GetCompound("mapTag");
            Mod mod = ModLoader.GetMod(mapTag.GetString("mapMod"));
            if (mod == null)
            {
                PathOfModifiers.Log("PathOfModifiers: Mod not found");
                return;
            }
            Type type = mod.Code.GetType(mapTag.GetString("mapFullName"));
            if (type == null)
            {
                PathOfModifiers.Log("PathOfModifiers: Map not found");
                return;
            }
            map = PoMDataLoader.maps[PoMDataLoader.mapMap[type]].Clone();
            map.Load(mapTag);
            //PathOfModifiers.Log("6");
        }

        public override ModItem Clone(Item item)
        {
            //PathOfModifiers.Log("7");
            MapNullCheck();
            Map newMap = (Map)base.Clone(item);
            newMap.map = map.Clone();

            //PathOfModifiers.Log("8");
            return newMap;
        }
        
        public override void NetSend(BinaryWriter writer)
        {
           // PathOfModifiers.Log("9");
            try
            {
                MapNullCheck();
                writer.Write(PoMDataLoader.mapMap[map.GetType()]);
                map.NetSend(writer);
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            //PathOfModifiers.Log("10");
        }
        public override void NetRecieve(BinaryReader reader)
        {
            //PathOfModifiers.Log("11");
            try
            {
                map = PoMDataLoader.maps[reader.ReadInt32()].Clone();
                map.NetReceive(reader);
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            //PathOfModifiers.Log("12");
        }
    }
}