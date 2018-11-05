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
using Terraria.ID;
using PathOfModifiers.Utilities.Noise;

namespace PathOfModifiers.Maps.Generators
{
    public class SinWaves : Generator
    {
        float totalAmpMult;
        float[] frequencies;
        float[] phases;
        float yOffset;

        public void Setup(float yOffset, int waveNumber, float totalAmpMult, float minFreq = 0, float maxFreq = 1, float minPhase = -6.283f, float maxPhase = 6.283f)
        {
            this.yOffset = yOffset;
            this.totalAmpMult = totalAmpMult;
            frequencies = new float[waveNumber];
            phases = new float[waveNumber];

            for(int i = 0; i < waveNumber; i++)
            {
                frequencies[i] = Main.rand.NextFloat(minFreq, maxFreq);
                phases[i] = Main.rand.NextFloat(minPhase, maxPhase);
            }
        }

        float GetWaveValue(float x)
        {
            if (frequencies == null || frequencies.Length == 0)
                throw new Exception("Cannot get wave value without setting up waves.");
            
            float value = 0;
            for (int i = 0; i < frequencies.Length; i++)
            {
                value += (float)Math.Sin(frequencies[i] * x + phases[i]);
            }
            //PathOfModifiers.Log($"{value}/{frequencies.Length}");

            return (value / frequencies.Length / 2 + 0.5f) * totalAmpMult;
        }

        public override void Generate(Rectangle dimensions)
        {            
            for (int x = 0; x < dimensions.Width; x++)
            {
                float waveValue = GetWaveValue(x) * dimensions.Height;
                int surfaceHeight = (int)Math.Round(waveValue + dimensions.Height * yOffset);

                waveValue = GetWaveValue(x + dimensions.Width) * dimensions.Height;
                int stoneHeight = (int)Math.Round(waveValue + dimensions.Height * yOffset);
                stoneHeight -= 10;
                //PathOfModifiers.Log(
                //    $"{GetWaveValue((float)x / dimensions.Width * (float)Math.PI * 2)}/" +
                //    $"{waveValue}/" +
                //    $"{surfaceHeight}/" +
                //    $"{x}/" +
                //    $"{x}");

                for (int y = 0; y < dimensions.Height; y++)
                {
                    int wallToPlace = 0;
                    bool removeWall = false;

                    int tileToPlace = 0;
                    bool removeTile = false;

                    Point tilePos = new Point(dimensions.X + x, dimensions.Y + y);

                    if (y <= dimensions.Height - surfaceHeight)
                    {
                        removeWall = true;
                        removeTile = true;
                    }
                    else if (y == dimensions.Height - (surfaceHeight - 1))
                    {
                        removeWall = true;
                        tileToPlace = TileID.Grass;
                    }
                    else if (y > dimensions.Height - (surfaceHeight - 1) && y <= dimensions.Height - stoneHeight)
                    {
                        wallToPlace = WallID.Dirt;
                        tileToPlace = TileID.Dirt;
                    }
                    else if (y > dimensions.Height - stoneHeight)
                    {
                        wallToPlace = WallID.Stone;
                        tileToPlace = TileID.Stone;
                    }

                    if (removeWall)
                        WallRemove(tilePos);
                    else
                        WallPlace(tilePos, wallToPlace);

                    if (removeTile)
                        TileRemove(tilePos);
                    else
                        TilePlace(tilePos, tileToPlace);
                }
            }

            ///CAVES GO HERE

            for (int x = 0; x < dimensions.Width; x++)
            {
                for (int y = 0; y < dimensions.Height; y++)
                {
                    Point tilePos = new Point(dimensions.X + x, dimensions.Y + y);
                    WorldGen.GrowTree(tilePos.X, tilePos.Y);
                }
            }
        }
    }
}