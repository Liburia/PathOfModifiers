using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace PathOfModifiers.Tiles
{
	public class MapBorder : ModTile
    {
        static List<Rectangle> activeBounds;

        public static void AddActiveBounds(Rectangle bounds)
        {
            activeBounds.Add(bounds);
        }
        public static void RemoveActiveBounds(Rectangle bounds)
        {
            activeBounds.Remove(bounds);
        }
        public static void ClearActiveBounds()
        {
            activeBounds.Clear();
            activeBounds.TrimExcess();
        }
        public static bool InteresectsOrContainsActiveBounds(Rectangle bounds)
        {
            foreach (var activeBound in activeBounds)
            {
                if (activeBound.Intersects(bounds) || activeBound.Contains(bounds) || bounds.Contains(activeBound))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsOnActiveBounds(int x, int y)
        {
            foreach (var bounds in activeBounds)
            {
                if (((x == bounds.Left || x == bounds.Right) && y >= bounds.Top && y <= bounds.Bottom) ||
                    ((y == bounds.Top || y == bounds.Bottom) && x >= bounds.Left && x <= bounds.Right))
                {
                    return true;
                }
            }
            return false;
        }

        public override void SetDefaults()
        {
            animationFrameHeight = 90;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			drop = mod.ItemType("MapBorder");
			AddMapEntry(new Color(100, 100, 100));

            activeBounds = new List<Rectangle>();
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (IsOnActiveBounds(i, j))
            {
                frameYOffset = animationFrameHeight;
            }
            else
            {
                frameYOffset = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (IsOnActiveBounds(i, j))
            {
                r = 0.506f;
                g = 0f;
                b = 0.78f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0;
            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return !IsOnActiveBounds(i, j);
        }

        public override bool CanExplode(int i, int j)
        {
            return !IsOnActiveBounds(i, j);
        }
    }
}