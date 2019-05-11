using Microsoft.Xna.Framework;
using System;

namespace PathOfModifiers.Utilities
{
    public static class Bezier
    {
        public static Vector2 Bezier2D(Vector2[] points, float t)
        {
            if (points.Length > 20)
                throw new ArgumentOutOfRangeException("points.Length", "cannot calculate bezier where points.Length > 20");

            int nPoints = points.Length - 1;

            float temp = (float)Math.Pow(1 - t, nPoints);
            Vector2 bezier = new Vector2(
                temp * points[0].X,
                temp * points[0].Y);

            for (int i = 1; i < nPoints; i++)
            {
                temp = (float)(Bernstein(nPoints, i) * Math.Pow(1 - t, nPoints - i) * Math.Pow(t, i));
                bezier.X += temp * points[i].X;
                bezier.Y += temp * points[i].Y;
            }
            temp = (float)Math.Pow(t, nPoints);
            bezier.X += temp * points[nPoints].X;
            bezier.Y += temp * points[nPoints].Y;

            return bezier;
        }

        static double Bernstein(int nPoints, int currentPoint)
        {
            return Factorial(nPoints) / (double)(Factorial(currentPoint) * Factorial(nPoints - currentPoint));
        }

        static ulong Factorial(int i)
        {
            if (i > 20)
                throw new ArgumentOutOfRangeException("i", "cannot calculate factorial where i > 20");
            if (i <= 1)
                return 1;
            return (ulong)i * Factorial(i - 1);
        }
    }
}