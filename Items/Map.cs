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
            item.useTime = 15;
            item.useStyle = 3;
            item.consumable = false;
        }

        //TODO: Remove? Should never be null; just crash.
        void MapNullCheck()
        {
            if (map == null)
                map = PoMDataLoader.maps[PoMDataLoader.mapMap[typeof(Maps.Plains)]].Clone();
        }

        public override bool UseItem(Player player)
        {
            MapNullCheck();
            //if (player.whoAmI == Main.myPlayer)
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                map.Open(new Rectangle((int)player.position.X / 16 + 4, (int)player.position.Y / 16 + 4, 50, 50));
            }
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
            var mapMod = mapTag.GetString("mapMod");
            Mod mod = ModLoader.GetMod(mapMod);
            if (mod == null)
            {
                mod.Logger.WarnFormat("Map mod \"{0}\" not found", mapMod);
                return;
            }
            var mapFullName = mapTag.GetString("mapFullName");
            Type type = mod.Code.GetType(mapFullName);
            if (type == null)
            {
                mod.Logger.WarnFormat("Map \"{0}\" not found", mapFullName);
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
                mod.Logger.Error(e.ToString());
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
                mod.Logger.Error(e.ToString());
            }
            //PathOfModifiers.Log("12");
        }
    }
}
