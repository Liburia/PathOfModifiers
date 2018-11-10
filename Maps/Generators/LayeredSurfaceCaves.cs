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
using PathOfModifiers.Utilities;
using PathOfModifiers.Utilities.Noise;

namespace PathOfModifiers.Maps.Generators
{
    public class LayeredSurfaceCaves : Generator
    {
        #region Sine settings
        bool sineSetup = false;
        float sineTotalAmpMult;
        float[] sineFrequencies;
        float[] sinePhases;
        float sineYOffset;
        #endregion
        #region Noise settings
        bool noiseSetup = false;
        float noiseScale;
        int noiseOctaves;
        float noiseThreshold;
        #endregion
        #region Bezier settings
        bool bezierSetup = false;
        int nBeziers;
        int bezierMinPoints;
        int bezierMaxPoints;
        int bezierWidth;
        #endregion
        #region Tile settings
        bool tilesSetup = false;
        bool tilesCreateTerrain;
        bool tilesCarveCaves;
        bool tilesGrowTrees;
        #endregion
        #region Pack settings
        int nPacks;
        bool packsSetup = false;
        #endregion

        public void SetupSineWaves(float yOffset = 0.5f, int nWaves = 10, float totalAmpMult = 25f, float maxFreq = 0.5f, float maxPhase = 6.283f)
        {
            sineYOffset = yOffset;
            sineTotalAmpMult = totalAmpMult;
            sineFrequencies = new float[nWaves];
            sinePhases = new float[nWaves];
            for (int i = 0; i < nWaves; i++)
            {
                sineFrequencies[i] = Main.rand.NextFloat(0, maxFreq);
                sinePhases[i] = Main.rand.NextFloat(0, maxPhase);
            }
            sineSetup = true;
        }
        public void SetupNoise(float scale = 10, int octaves = 3, float threshold = 0.35f)
        {
            noiseScale = scale;
            noiseOctaves = octaves;
            noiseThreshold = threshold;
            noiseSetup = true;
        }
        public void SetupBezier(int minBeziers = 0, int maxBeziers = 4, int minPoints = 1, int maxPoints = 10, int width = 5)
        {
            nBeziers = Main.rand.Next(minBeziers, maxBeziers + 1);
            bezierMinPoints = minPoints;
            bezierMaxPoints = maxPoints;
            bezierWidth = width;
            bezierSetup = true;
        }
        public void SetupTiles(bool createTerrain = true, bool carveCaves = true, bool growTrees = true)
        {
            //TODO: Setup custom tile layers
            tilesCreateTerrain = createTerrain;
            tilesCarveCaves = carveCaves;
            tilesGrowTrees = growTrees;
            tilesSetup = true;
        }
        public void SetupPacks()
        {
            packsSetup = true;
        }
        void ResetSetup()
        {
            sineSetup = false;
            noiseSetup = false;
            bezierSetup = false;
            tilesSetup = false;
        }

        float GetWaveValue(float x)
        {            
            float value = 0;
            for (int i = 0; i < sineFrequencies.Length; i++)
            {
                value += (float)Math.Sin(sineFrequencies[i] * x + sinePhases[i]);
            }
            //PathOfModifiers.Log($"{value}/{frequencies.Length}");

            return (value / sineFrequencies.Length / 2 + 0.5f) * sineTotalAmpMult;
        }

        public override void GenerateTerrain(Rectangle dimensions)
        {
            if (!sineSetup)
                SetupSineWaves();
            if (!noiseSetup)
                SetupNoise();
            if (!bezierSetup)
                SetupBezier();
            if (!tilesSetup)
                SetupTiles();
            
            GenerateBorders(dimensions);
            
            if (tilesCreateTerrain)
            {
                for (int x = 0; x < dimensions.Width; x++)
                {
                    float waveValue = GetWaveValue(x);
                    int surfaceHeight = (int)Math.Round(waveValue + dimensions.Height * sineYOffset);

                    waveValue = GetWaveValue(x + dimensions.Width);
                    int stoneHeight = (int)Math.Round(waveValue + dimensions.Height * sineYOffset);
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
                            KillWall(tilePos);
                        else
                            PlaceWall(tilePos, wallToPlace);

                        if (removeTile)
                            KillTile(tilePos);
                        else
                            PlaceTile(tilePos, tileToPlace);
                    }
                }
            }
            if (tilesCarveCaves)
            {
                float noiseSeed = Main.rand.NextFloat(0, 255);
                for (int x = 0; x < dimensions.Width; x++)
                {
                    for (int y = 0; y < dimensions.Height; y++)
                    {
                        Point tilePos = new Point(dimensions.X + x, dimensions.Y + y);
                        if (Noise.GetOctaveNoise(x / noiseScale, y / noiseScale, noiseSeed, noiseOctaves) < noiseThreshold)
                            KillTile(tilePos);
                    }
                }

                //((PathOfModifiers)mod).test = new List<Point>();
                Vector2[][] beziers = new Vector2[nBeziers][];
                for (int i = 0; i < beziers.Length; i++)
                {
                    beziers[i] = new Vector2[Main.rand.Next(bezierMinPoints, bezierMaxPoints + 1)];
                    Vector2[] bezier = beziers[i];
                    for (int j = 0; j < bezier.Length; j++)
                        bezier[j] = new Vector2(Main.rand.NextFloat(0, dimensions.Width), Main.rand.NextFloat(0, dimensions.Height));

                    float increment = 1f / (Math.Max(dimensions.Width, dimensions.Height) * 5);
                    Vector2 vTilePos = Bezier.Bezier2D(bezier, 0);
                    Vector2 newVTilePos = Bezier.Bezier2D(bezier, increment);
                    KillTilesLine(vTilePos, newVTilePos, bezierWidth, dimensions);
                    for (float t = increment; t < 1; t += increment)
                    {
                        vTilePos = newVTilePos;
                        float newT = t + increment;
                        if (newT > 1)
                            newT = 1;
                        newVTilePos = Bezier.Bezier2D(bezier, newT);
                        KillTilesLine(vTilePos, newVTilePos, bezierWidth, dimensions);
                    }
                }
            }
            if (tilesGrowTrees)
            {
                for (int x = 0; x < dimensions.Width; x++)
                {
                    for (int y = 0; y < dimensions.Height; y++)
                    {
                        Point tilePos = new Point(dimensions.X + x, dimensions.Y + y);
                        WorldGen.GrowTree(tilePos.X, tilePos.Y);
                    }
                }
            }

            ResetSetup();
        }
        public override void SpawnPacks(Rectangle dimensions, int nNPCs, Pack[] packs)
        {
            Vector2 mapPos = new Vector2(dimensions.X * 16, dimensions.Y * 16);
            Vector2 mapSize = new Vector2(dimensions.Width * 16, dimensions.Height * 16);
            for (int i = 0; i < packs.Length; i++)
            {
                Pack p = packs[i];
                Vector2 packPos = new Vector2(
                    Main.rand.NextFloat(mapPos.X + p.radius, mapPos.X + mapSize.X - p.radius),
                    Main.rand.NextFloat(mapPos.Y + p.radius, mapPos.Y + mapSize.Y - p.radius));
                SpawnPack(p, packPos, p.radius, p.clearSpace);
            }
        }

        void KillTilesLine(Vector2 startPos, Vector2 endPos, int width, Rectangle dimensions)
        {
            Vector2 direction = endPos - startPos;
            float magnitude = direction.Length();
            direction /= magnitude;
            Vector2 direction90 = new Vector2(direction.Y, -direction.X);
            int halfWidth = width / 2;

            for (int i = 0; i < magnitude; i++)
            {
                for(int j = -halfWidth; j < -halfWidth + width; j++)
                {
                    Vector2 vTilePos = (direction * i) + (direction90 * j);
                    Point tilePos = new Point(dimensions.X + (int)Math.Round(startPos.X + vTilePos.X), dimensions.Y + (int)Math.Round(startPos.Y + vTilePos.Y));
                    //((PathOfModifiers)mod).test.Add(new Point(tilePos.X * 16, tilePos.Y * 16));
                    //((PathOfModifiers)mod).test2.Add(new Vector2((int)(dimensions.X + startPos.X + vTilePos.X) * 16, (int)(dimensions.Y + startPos.Y + vTilePos.Y) * 16));
                    if (dimensions.Contains(tilePos))
                        KillTile(tilePos);
                }
            }

        }
    }
}