using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PathOfModifiers.Items
{
    public class Map : ModItem
    {
        public Maps.Map map;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.value = 500000;
            Item.rare = 2;
            Item.maxStack = 1;
            Item.autoReuse = false;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = 3;
            Item.consumable = false;
        }

        void MapNullCheck()
        {
            if (map == null)
                map = DataManager.Map.GetNewMap(typeof(Maps.Plains));
        }

        public override bool? UseItem(Player player)
        {
            MapNullCheck();
            //if (player.whoAmI == Main.myPlayer)
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                map.Open(new Rectangle((int)player.position.X / 16 + 4, (int)player.position.Y / 16 + 4, 50, 50));
            }
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            map.DrawIcon(spriteBatch, position, Terraria.GameContent.TextureAssets.Item[Item.type].Value.Size(), 0, scale);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            //World drawing is different so just hardcode this.
            var iconOffset = new Vector2(0, 2);
            map.DrawIcon(spriteBatch, Item.position - Main.screenPosition + iconOffset, Terraria.GameContent.TextureAssets.Item[Item.type].Value.Size(), rotation, scale);
        }

        public override void SaveData(TagCompound tag)
        {
            //PathOfModifiers.Log("3");
            MapNullCheck();
            TagCompound mapTag;
            mapTag = new TagCompound();
            mapTag.Set("mapMod", map.mod.Name);
            mapTag.Set("mapFullName", map.GetType().FullName);
            map.Save(mapTag);
            tag.Set("mapTag", mapTag);
            //PathOfModifiers.Log("4");
        }
        public override void LoadData(TagCompound tag)
        {
            //PathOfModifiers.Log("5");
            TagCompound mapTag = tag.GetCompound("mapTag");
            var mapMod = mapTag.GetString("mapMod");
            ModLoader.TryGetMod(mapMod, out Mod mod);
            if (mod == null)
            {
                Mod.Logger.WarnFormat("Map mod \"{0}\" not found", mapMod);
                return;
            }
            var mapFullName = mapTag.GetString("mapFullName");
            Type type = mod.Code.GetType(mapFullName);
            if (type == null)
            {
                Mod.Logger.WarnFormat("Map \"{0}\" not found", mapFullName);
                return;
            }
            map = DataManager.Map.GetNewMap(type);
            map.Load(mapTag);
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
                writer.Write(DataManager.Map.GetMapIndex(map.GetType()));
                map.NetSend(writer);
            }
            catch (Exception e)
            {
                Mod.Logger.Error(e.ToString());
            }
            //PathOfModifiers.Log("10");
        }
        public override void NetReceive(BinaryReader reader)
        {
            //PathOfModifiers.Log("11");
            try
            {
                map = DataManager.Map.GetNewMap(reader.ReadInt32());
                map.NetReceive(reader);
            }
            catch (Exception e)
            {
                Mod.Logger.Error(e.ToString());
            }
            //PathOfModifiers.Log("12");
        }
    }
}
