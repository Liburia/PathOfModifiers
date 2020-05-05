using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using System.IO;
using PathOfModifiers.Rarities;
using Terraria.UI;
using Terraria.ID;
using System.Collections.Generic;
using PathOfModifiers.UI;
using PathOfModifiers.Tiles;
using Terraria.DataStructures;
using PathOfModifiers.Buffs;
using Terraria.Utilities;

namespace PathOfModifiers
{
    public static class PoMDebug
    {
        public static List<Point> tiles = new List<Point>();
        public static List<Rectangle> tileBounds = new List<Rectangle>();

        public static List<Rectangle> recs = new List<Rectangle>();

        public static void Draw(SpriteBatch sb)
        {
            if (tileBounds.Count == 0)
            {
                List<Point> tiles = new List<Point>();
                UnifiedRandom rand = new UnifiedRandom(52222222);
                foreach (Rectangle rec in tileBounds)
                {
                    Color color = new Color(rand.Next(256), rand.Next(256), rand.Next(256));
                    for (int x = 0; x <= rec.Width; x++)
                    {
                        Vector2 drawPos = new Vector2((rec.X + x) * 16, rec.Y * 16) - Main.screenPosition;
                        Point p1 = new Point((int)drawPos.X + 2, (int)drawPos.Y + 2);
                        int size1 = 12;
                        if (tiles.Contains(p1))
                        {
                            size1 -= 2;
                        }
                        else
                        {
                            tiles.Add(p1);
                        }
                        Point p2 = new Point((int)drawPos.X + 2, (int)drawPos.Y + rec.Height * 16 + 2);
                        int size2 = 12;
                        if (tiles.Contains(p2))
                        {
                            size2 -= 2;
                        }
                        else
                        {
                            tiles.Add(p2);
                        }
                        sb.Draw(Main.magicPixel, new Rectangle(p1.X, p1.Y, size1, size1), color);
                        sb.Draw(Main.magicPixel, new Rectangle(p2.X, p2.Y, size2, size2), color);
                    }
                    for (int y = 0; y <= rec.Height; y++)
                    {
                        Vector2 drawPos = new Vector2(rec.X * 16, (rec.Y + y) * 16) - Main.screenPosition;
                        Point p1 = new Point((int)drawPos.X + 2, (int)drawPos.Y + 2);
                        int size1 = 12;
                        if (tiles.Contains(p1))
                        {
                            size1 -= 2;
                        }
                        else
                        {
                            tiles.Add(p1);
                        }
                        Point p2 = new Point((int)drawPos.X + rec.Width * 16 + 2, (int)drawPos.Y + 2);
                        int size2 = 12;
                        if (tiles.Contains(p2))
                        {
                            size2 -= 2;
                        }
                        else
                        {
                            tiles.Add(p2);
                        }
                        sb.Draw(Main.magicPixel, new Rectangle(p1.X, p1.Y, size1, size1), color);
                        sb.Draw(Main.magicPixel, new Rectangle(p2.X, p2.Y, size2, size2), color);
                    }
                }
            }

            foreach (Rectangle rect in recs)
            {
                Rectangle sourceRect = new Rectangle(0, 0, 1, 1);
                Rectangle screenRect = new Rectangle(rect.X - (int)Main.screenPosition.X, rect.Y - (int)Main.screenPosition.Y, rect.Width, rect.Height);
                sb.Draw(Main.magicPixel, screenRect, sourceRect, Color.Red);
            }
        }
    }
}
