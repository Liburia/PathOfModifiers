using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Terraria.ID;
using System.Collections.Generic;

namespace PathOfModifiers.Tiles
{
	public class MapBorder : ModTile
	{
        List<Point> activeTiles;

        public void AddActiveBounds(Rectangle bounds)
        {
            for (int i = bounds.Left; i <= bounds.Right; i++)
            {
                activeTiles.Add(new Point(i, bounds.Top));
                activeTiles.Add(new Point(i, bounds.Bottom));
            }
            for (int j = bounds.Top + 1; j < bounds.Bottom; j++)
            {
                activeTiles.Add(new Point(bounds.Left, j));
                activeTiles.Add(new Point(bounds.Right, j));
            }
        }
        public void RemoveActiveBounds(Rectangle bounds)
        {
            var point = bounds.TopLeft().ToPoint();
            var index = activeTiles.IndexOf(point);
            activeTiles.RemoveRange(index, bounds.Width * 2 + bounds.Height * 2);
        }
        public void ClearActiveBounds()
        {
            activeTiles.Clear();
            activeTiles.TrimExcess();
        }

        public override void SetDefaults()
        {
            animationFrameHeight = 90;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			drop = mod.ItemType("MapBorder");
			AddMapEntry(new Color(100, 100, 100));

            activeTiles = new List<Point>();
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (activeTiles.Contains(new Point(i, j)))
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
            if (activeTiles.Contains(new Point(i, j)))
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
            return !activeTiles.Contains(new Point(i, j));
        }

        public override bool CanExplode(int i, int j)
        {
            return !activeTiles.Contains(new Point(i, j));
        }
    }
}