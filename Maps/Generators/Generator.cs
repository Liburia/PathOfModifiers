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

namespace PathOfModifiers.Maps.Generators
{

    public class Generator
    {
        public class PatchSettings
        {
            public int type;
            public int nBranches;
            public float minBranchLength;
            public float maxBranchLength;
            public float minBranchWidth;
            public float maxBranchWidth;
            public float branchWidthMultiplier;
            public float branchWidthLimitAdd;
            public int[] replaceTiles;

            public PatchSettings(int type, int nBranches, float minBranchLength, float maxBranchLength, float minBranchWidth, float maxBranchWidth, float branchWidthMultiplier, float branchWidthLimitAdd, int[] replaceTiles = null)
            {
                this.type = type;
                this.nBranches = nBranches;
                this.minBranchLength = minBranchLength;
                this.maxBranchLength = maxBranchLength;
                this.minBranchWidth = minBranchWidth;
                this.maxBranchWidth = maxBranchWidth;
                this.branchWidthMultiplier = branchWidthMultiplier;
                this.branchWidthLimitAdd = branchWidthLimitAdd;
                this.replaceTiles = replaceTiles;
            }
        }

        public Mod mod;

        public virtual void GenerateTerrain(Rectangle dimensions) { }
        public virtual void SpawnPacks(Rectangle dimensions, int nNPCs, Pack[] packs) { }

        //TODO: Players makes the borders. Or maybe create unbreakable border when the map is active.
        protected void GenerateBorders(Rectangle dimensions)
        {
            for (int i = 0; i < dimensions.Width + 2; i++)
            {
                PlaceTile(new Point(dimensions.X - 1 + i, dimensions.Y - 1), TileID.IceBrick);
                PlaceTile(new Point(dimensions.X - 1 + i, dimensions.Y + dimensions.Width), TileID.IceBrick);
            }
            for (int i = 1; i < dimensions.Height + 1; i++)
            {
                PlaceTile(new Point(dimensions.X - 1, dimensions.Y - 1 + i), TileID.IceBrick);
                PlaceTile(new Point(dimensions.X + dimensions.Height, dimensions.Y - 1 + i), TileID.IceBrick);
            }
        }

        protected void GeneratePatch(Rectangle dimensions, Point pos, PatchSettings patchSettings)
        {
            bool replace = patchSettings.replaceTiles != null;
            for (int i = 0; i < patchSettings.nBranches; i++)
            {
                Vector2 direction = Main.rand.NextVector2Unit();
                Vector2 direction90 = new Vector2(direction.Y, -direction.X);
                float length = Main.rand.NextFloat(patchSettings.minBranchLength, patchSettings.maxBranchLength);
                float width = Main.rand.NextFloat(patchSettings.minBranchWidth, patchSettings.maxBranchWidth);
                float widthLimit = width + patchSettings.branchWidthLimitAdd;
                bool decreaseWidth = patchSettings.branchWidthLimitAdd < 0;

                for (int j = 0; j < length; j++)
                {
                    float halfWidth = width / 2;
                    for (float k = -halfWidth; k < -halfWidth + width; k++)
                    {
                        Vector2 vTilePos = (direction * j) + (direction90 * k);
                        Point tilePos = new Point(dimensions.X + (int)Math.Round(pos.X + vTilePos.X), dimensions.Y + (int)Math.Round(pos.Y + vTilePos.Y));
                        if (dimensions.Contains(tilePos) && (!replace || patchSettings.replaceTiles.Contains(Main.tile[tilePos.X, tilePos.Y].type)))
                        {
                            PlaceTile(tilePos, patchSettings.type);
                        }
                    }
                    width *= patchSettings.branchWidthMultiplier;
                    if ((decreaseWidth && width <= widthLimit) || (!decreaseWidth && width >= widthLimit))
                        break;
                }
            }
        }

        protected void PlaceTiles(Point pos, Point size, int type, bool mute = true, bool force = true, int player = -1, int style = 0)
        {
            for(int x = pos.X; x < pos.X + size.X; x++)
            {
                for (int y = pos.Y; y < pos.Y + size.Y; y++)
                {
                    PlaceTile(new Point(x, y), type, mute, force, player, style);
                }
            }
        }
        protected void KillTiles(Point pos, Point size, bool isMined = false, bool noItem = true, bool effectOnly = false)
        {
            for (int x = pos.X; x < pos.X + size.X; x++)
            {
                for (int y = pos.Y; y < pos.Y + size.Y; y++)
                {
                    KillTile(new Point(x, y), isMined, noItem, effectOnly);
                }
            }
        }
        protected void PlaceTile(Point pos, int type, bool mute = true, bool force = true, int player = -1, int style = 0)
        {
            if (type == TileID.Grass)
                WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt, mute, force, player, style);
            WorldGen.PlaceTile(pos.X, pos.Y, type, mute, force, player, style);
        }
        protected void KillTile(Point pos, bool isMined = false, bool noItem = true, bool effectOnly = false)
        {
            WorldGen.KillTile(pos.X, pos.Y, isMined, effectOnly, noItem);
        }
        protected void PlaceWall(Point pos, int type, bool mute = true, bool force = true)
        {
            if (force)
                KillWall(pos);
            WorldGen.PlaceWall(pos.X, pos.Y, type, mute);
        }
        protected void KillWall(Point pos, bool isMined = false)
        {
            WorldGen.KillWall(pos.X, pos.Y, isMined);
        }

        protected void SpawnPack(Pack pack, Vector2 pos, float radius, bool clearSpace)
        {
            for (int i = 0; i < pack.npcCounts.Length; i++)
            {
                for(int j = 0; j < pack.npcCounts[i].Item2; j++)
                {
                    Vector2 spawnPosRand = new Vector2(Main.rand.NextFloat(-radius, radius), Main.rand.NextFloat(-radius, radius));
                    Vector2 spawnPos = pos + spawnPosRand;
                    NPC npc = SpawnNPC(spawnPos, pack.npcCounts[i].Item1);

                    if (npc == null)
                        return;

                    float diff = Math.Abs(spawnPosRand.X) - (radius - ((npc.width + 16) / 2f));
                    if (diff > 0)
                        npc.position.X += Math.Sign(spawnPosRand.X) * -diff;
                    diff = Math.Abs(spawnPosRand.Y) - (radius - ((npc.height + 16) / 2f));
                    if (diff > 0)
                        npc.position.Y += Math.Sign(spawnPosRand.Y) * -diff;

                    if (clearSpace)
                        ClearSpace(npc);
                }
            }
        }
        protected NPC SpawnNPC(Vector2 pos, int type)
        {
            int newNpcIndex = NPC.NewNPC((int)pos.X, (int)pos.Y, type);
            if (newNpcIndex == 200)
                return null;
            NPC newNPC = Main.npc[newNpcIndex];
            PoMNPC pomNPC = newNPC.GetGlobalNPC<PoMNPC>();
            pomNPC.mapNpc = true;
            return newNPC;
        }
        protected void ClearSpace(NPC npc)
        {
            KillTiles(
                new Point((int)Math.Floor(npc.position.X / 16) - 1, (int)Math.Floor(npc.position.Y / 16) - 1),
                new Point((int)Math.Ceiling(npc.width / 16f) + 2, (int)Math.Ceiling(npc.height / 16f) + 2));
        }
    }
}