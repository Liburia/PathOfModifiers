

using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace PathOfModifiers.Utilities.Extensions
{
    public static class Extensions
    {
        public static void Write(this BinaryWriter writer, Rectangle rect)
        {
            writer.Write(rect.X);
            writer.Write(rect.Y);
            writer.Write(rect.Width);
            writer.Write(rect.Height);
        }
        public static Rectangle ReadRectangle(this BinaryReader reader)
        {
            Rectangle rect = Rectangle.Empty;
            rect.X = reader.ReadInt32();
            rect.Y = reader.ReadInt32();
            rect.Width = reader.ReadInt32();
            rect.Height = reader.ReadInt32();
            return rect;
        }
    }
}