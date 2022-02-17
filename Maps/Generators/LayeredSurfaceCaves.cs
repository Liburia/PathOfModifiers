using Microsoft.Xna.Framework;
using PathOfModifiers.Utilities;
using PathOfModifiers.Utilities.Noise;
using System;
using System.Collections.Generic;
using Terraria;

namespace PathOfModifiers.Maps.Generators
{
    public class LayerSetings
    {
        public int type;
        public int absoluteOffset;
        public bool useLastLayerWave;
        public bool useAbsoluteOffset;
        public float relativeOffset;

        public LayerSetings(int type, int absoluteOffset, bool useLastLayerWave = false, bool useAbsoluteOffset = true, float relativeOffset = 0)
        {
            this.type = type;
            this.absoluteOffset = absoluteOffset;
            this.useLastLayerWave = useLastLayerWave;
            this.useAbsoluteOffset = useAbsoluteOffset;
            this.relativeOffset = relativeOffset;
        }

        public int GetOffset(int mapHeight = 0)
        {
            return useAbsoluteOffset ? absoluteOffset : (int)Math.Round(relativeOffset * mapHeight);
        }
    }
    public class OreSetting
    {
        public float frequency;
        public Generator.PatchSettings patchSettings;

        public OreSetting(float frequency, Generator.PatchSettings patchSettings)
        {
            this.frequency = frequency;
            this.patchSettings = patchSettings;
        }
    }

    public class LayeredSurfaceCaves : Generator
    {
        #region Terrain settings
        bool sineSetup = false;
        float sineTotalAmpMult;
        float[] sineFrequencies;
        float[] sinePhases;
        float sineYOffset;
        #endregion
        #region Cave settings
        bool caveSetup = false;
        float caveNoiseScale;
        int caveNoiseOctaves;
        float caveNoiseThreshold;
        int caveNBeziers;
        int caveBezierMinPoints;
        int caveBezierMaxPoints;
        int caveBezierWidth;
        #endregion
        #region Tile settings
        bool tileSetup = false;
        LayerSetings[] tileLayers;
        LayerSetings[] wallLayers;
        bool tilesMakeTerrain;
        bool tilesMakeCaves;
        bool tilesMakeOres;
        bool tilesMakeTrees;
        #endregion
        #region Ore settings
        bool oreSetup = false;
        OreSetting[] ores;
        #endregion
        #region Pack settings
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
        public void SetupCaves(float scale = 10, int octaves = 3, float threshold = 0.35f, int minBeziers = 0, int maxBeziers = 4, int minPoints = 1, int maxPoints = 10, int width = 5)
        {
            caveNoiseScale = scale;
            caveNoiseOctaves = octaves;
            caveNoiseThreshold = threshold;

            caveNBeziers = Main.rand.Next(minBeziers, maxBeziers + 1);
            caveBezierMinPoints = minPoints;
            caveBezierMaxPoints = maxPoints;
            caveBezierWidth = width;

            caveSetup = true;

        }
        public void SetupTiles(LayerSetings[] tileLayers, LayerSetings[] wallLayers, bool createTerrain = true, bool makeOres = true, bool carveCaves = true, bool growTrees = true)
        {
            this.tileLayers = tileLayers;
            this.wallLayers = wallLayers;

            tilesMakeTerrain = createTerrain;
            tilesMakeOres = makeOres;
            tilesMakeCaves = carveCaves;
            tilesMakeTrees = growTrees;
            tileSetup = true;
        }
        public void SetupOres(OreSetting[] ores)
        {
            this.ores = ores;
            oreSetup = true;
        }
        public void SetupPacks()
        {
            packsSetup = true;
        }
        void ResetSetup()
        {
            sineSetup = false;
            caveSetup = false;
            tileSetup = false;
            oreSetup = false;
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
            Point mapSize = new Point(dimensions.Width + 1, dimensions.Height + 1);
            if (!sineSetup)
                SetupSineWaves();
            if (!caveSetup)
                SetupCaves();
            if (!tileSetup)
                SetupTiles(new LayerSetings[0], new LayerSetings[0]);
            if (!oreSetup)
                SetupOres(new OreSetting[0]);

            //GenerateBorders(dimensions);

            if (tilesMakeTerrain)
            {
                for (int x = 0; x < mapSize.X; x++)
                {
                    int[] tileLayerSineOffsets = new int[tileLayers.Length];
                    float waveValue = 0;
                    for (int i = 0; i < tileLayerSineOffsets.Length; i++)
                    {
                        if (!tileLayers[i].useLastLayerWave)
                            waveValue = GetWaveValue(x + mapSize.X * i);
                        tileLayerSineOffsets[i] = (int)Math.Ceiling(waveValue + mapSize.Y * sineYOffset - tileLayers[i].GetOffset(mapSize.Y));
                    }

                    int[] wallLayerSineOffsets = new int[wallLayers.Length];
                    waveValue = 0;
                    for (int i = 0; i < wallLayerSineOffsets.Length; i++)
                    {
                        if (!wallLayers[i].useLastLayerWave)
                            waveValue = GetWaveValue(x - mapSize.X * (i + 1));
                        wallLayerSineOffsets[i] = (int)Math.Ceiling(waveValue + mapSize.Y * sineYOffset - wallLayers[i].GetOffset(mapSize.Y));
                    }
                    //PathOfModifiers.Log(
                    //    $"{GetWaveValue((float)x / dimensions.Width * (float)Math.PI * 2)}/" +
                    //    $"{waveValue}/" +
                    //    $"{surfaceHeight}/" +
                    //    $"{x}/" +
                    //    $"{x}");

                    for (int y = 0; y < mapSize.Y; y++)
                    {
                        int wallToPlace = 0;
                        bool removeWall = false;

                        int tileToPlace = 0;
                        bool removeTile = false;

                        Point tilePos = new Point(dimensions.X + x, dimensions.Y + y);

                        if (y <= mapSize.Y - tileLayerSineOffsets[0])
                        {
                            removeTile = true;
                        }
                        else
                        {
                            for (int i = 0; i < tileLayerSineOffsets.Length; i++)
                            {
                                if (y > mapSize.Y - tileLayerSineOffsets[i])
                                {
                                    tileToPlace = tileLayers[i].type;
                                }
                            }
                        }

                        if (y <= mapSize.Y - wallLayerSineOffsets[0])
                        {
                            removeWall = true;
                        }
                        else
                        {
                            for (int i = 0; i < wallLayerSineOffsets.Length; i++)
                            {
                                if (y > mapSize.Y - wallLayerSineOffsets[i])
                                {
                                    wallToPlace = wallLayers[i].type;
                                }
                            }
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
            if (tilesMakeCaves)
            {
                float noiseSeed = Main.rand.NextFloat(0, 255);
                for (int x = 0; x < mapSize.X; x++)
                {
                    for (int y = 0; y < mapSize.Y; y++)
                    {
                        Point tilePos = new Point(dimensions.X + x, dimensions.Y + y);
                        if (Noise.GetOctaveNoise(x / caveNoiseScale, y / caveNoiseScale, noiseSeed, caveNoiseOctaves) < caveNoiseThreshold)
                            KillTile(tilePos);
                    }
                }

                //((PathOfModifiers)mod).test = new List<Point>();
                Vector2[][] beziers = new Vector2[caveNBeziers][];
                for (int i = 0; i < beziers.Length; i++)
                {
                    beziers[i] = new Vector2[Main.rand.Next(caveBezierMinPoints, caveBezierMaxPoints + 1)];
                    Vector2[] bezier = beziers[i];
                    for (int j = 0; j < bezier.Length; j++)
                        bezier[j] = new Vector2(Main.rand.NextFloat(0, mapSize.X), Main.rand.NextFloat(0, mapSize.Y));

                    float increment = 1f / (Math.Max(mapSize.X, mapSize.Y) * 5);
                    Vector2 vTilePos = Bezier.Bezier2D(bezier, 0);
                    Vector2 newVTilePos = Bezier.Bezier2D(bezier, increment);
                    KillTilesLine(vTilePos, newVTilePos, caveBezierWidth, dimensions);
                    for (float t = increment; t < 1; t += increment)
                    {
                        vTilePos = newVTilePos;
                        float newT = t + increment;
                        if (newT > 1)
                            newT = 1;
                        newVTilePos = Bezier.Bezier2D(bezier, newT);
                        KillTilesLine(vTilePos, newVTilePos, caveBezierWidth, dimensions);
                    }
                }
            }
            if (tilesMakeOres)
            {
                for (int i = 0; i < ores.Length; i++)
                {
                    int nPatches = Main.rand.Next((int)Math.Round(ores[i].frequency * (mapSize.X * mapSize.Y / 10000f)) + 1);
                    for (int j = 0; j < nPatches; j++)
                    {
                        Point tilePos = new Point(Main.rand.Next(mapSize.X), Main.rand.Next(mapSize.Y));
                        GeneratePatch(dimensions, tilePos, ores[i].patchSettings);
                    }
                }
            }
            if (tilesMakeTrees)
            {
                for (int x = 0; x < mapSize.X; x++)
                {
                    for (int y = 0; y < mapSize.Y; y++)
                    {
                        Point tilePos = new Point(dimensions.X + x, dimensions.Y + y);
                        WorldGen.GrowTree(tilePos.X, tilePos.Y);
                    }
                }
            }

            ResetSetup();
        }
        public override NPC[] SpawnPacks(Rectangle dimensions, int nNPCs, Pack[] packs)
        {
            List<NPC> npcs = new List<NPC>();

            Vector2 mapPos = new Vector2(dimensions.X * 16, dimensions.Y * 16);
            Vector2 mapSize = new Vector2(dimensions.Width * 16, dimensions.Height * 16);
            //PoMDebug.recs = new List<Rectangle>();
            for (int i = 0; i < packs.Length; i++)
            {
                Pack p = packs[i];
                if (mapSize.X < p.radius * 2 || mapSize.Y < p.radius * 2)
                    continue;
                Vector2 packPos = new Vector2(
                    Main.rand.NextFloat(mapPos.X + p.radius + 1, mapPos.X + mapSize.X - p.radius + 15),
                    Main.rand.NextFloat(mapPos.Y + p.radius + 1, mapPos.Y + mapSize.Y - p.radius + 15));
                //packPos = new Vector2(mapPos.X + p.radius + 1, mapPos.Y + mapSize.Y - p.radius + 15);
                //packPos = new Vector2(mapPos.X + mapSize.X - p.radius + 15, mapPos.Y + p.radius + 1);
                //PoMDebug.recs.Add(new Rectangle((int)(packPos.X - p.radius), (int)(packPos.Y - p.radius), (int)(p.radius * 2), (int)(p.radius * 2)));
                npcs.AddRange(SpawnPack(p, packPos, p.radius, p.clearSpace));
            }
            return npcs.ToArray();
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
                for (int j = -halfWidth; j < -halfWidth + width; j++)
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