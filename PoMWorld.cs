using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using PathOfModifiers.AffixesItem;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Maps;
using PathOfModifiers.Tiles;

namespace PathOfModifiers
{
    public class PoMWorld : ModWorld
    {
        //public Map[] openMaps;

        ////TODO: read maxOpenMaps from config
        ////TODO: receive maxOpenMaps from server
        //public int maxOpenMaps = 255;

        //public override void Initialize()
        //{
        //    openMaps = new Map[maxOpenMaps];
        //}

        //public int AddOpenMap(Map map, int ID = -1, bool overwrite = false)
        //{
        //    if (ID > -1)
        //    {
        //        if (!overwrite && openMaps[ID] != null)
        //            throw new Exception("Cannot add open map to the world by ID because the ID is already taken.");

        //        openMaps[ID] = map;
        //        return ID;
        //    }

        //    for (int i = 0; i < maxOpenMaps; i++)
        //    {
        //        if (openMaps[i] == null)
        //        {
        //            openMaps[i] = map;
        //            return i;
        //        }
        //    }
        //    return -1;
        //}
        //public void RemoveOpenMap(int ID)
        //{
        //    openMaps[ID] = null;
        //}

        public override void Initialize()
        {
            var mapBorder = mod.GetTile<MapBorder>();
            MapBorder.ClearActiveBounds();
        }
    }
}
