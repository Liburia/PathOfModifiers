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
using PathOfModifiers.AffixesItem;
using PathOfModifiers.Rarities;
using Terraria.ID;
using Terraria.DataStructures;
using PathOfModifiers.Tiles;
using PathOfModifiers;
using PathOfModifiers.Buffs;
using Terraria.ModLoader.IO;

namespace PathOfModifiers
{
    public static class PoMNetMessage
    {
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MsgType msg = (MsgType)reader.ReadByte();
            PathOfModifiers.Instance.Logger.Debug($"Msg Received: {Main.netMode.ToString()}/{msg.ToString()}");

            //TODO: Make into switch
            if (msg == MsgType.cSyncDataMaps)
            {
                PoMDataLoader.ReceiveDataMaps(reader);
            }
            else if (msg == MsgType.PlayerConnected)
            {
                int player = reader.ReadByte();
                ModPacket packet = PathOfModifiers.Instance.GetPacket();
                packet.Write((byte)MsgType.cSyncDataMaps);
                PoMDataLoader.SendMaps(packet);
                packet.Send(player);
            }
            else if (msg == MsgType.AddDamageDoTDebuffNPC)
            {
                int npcID = reader.ReadInt32();
                int buffType = reader.ReadInt32();
                int damage = reader.ReadInt32();
                int time = reader.ReadInt32();

#pragma warning disable IDE0019 // Use pattern matching
                DamageDoTDebuff debuff = BuffLoader.GetBuff(buffType) as DamageDoTDebuff;
#pragma warning restore IDE0019 // Use pattern matching
                if (debuff == null)
                {
                    PathOfModifiers.Instance.Logger.Warn($"Invalid buff packet received {buffType}");
                    goto SkipMsgIf;
                }
                NPC npc = Main.npc[npcID];
                PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
                pomNPC.AddDamageDoTBuff(npc, debuff, damage, time, false);

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = PathOfModifiers.Instance.GetPacket();
                    packet.Write((byte)MsgType.AddDamageDoTDebuffNPC);
                    packet.Write(npcID);
                    packet.Write(buffType);
                    packet.Write(damage);
                    packet.Write(time);
                    packet.Send(-1, whoAmI);
                }
            }
            else if (msg == MsgType.AddDamageDoTDebuffPlayer)
            {
                int playerID = reader.ReadInt32();
                int buffType = reader.ReadInt32();
                int damage = reader.ReadInt32();
                int time = reader.ReadInt32();

#pragma warning disable IDE0019 // Use pattern matching
                DamageDoTDebuff debuff = BuffLoader.GetBuff(buffType) as DamageDoTDebuff;
#pragma warning restore IDE0019 // Use pattern matching
                if (debuff == null)
                {
                    PathOfModifiers.Instance.Logger.Warn($"Invalid buff packet received {buffType}");
                    goto SkipMsgIf;
                }
                Player player = Main.player[playerID];
                PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
                pomPlayer.AddDamageDoTBuff(player, debuff, damage, time, false);

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = PathOfModifiers.Instance.GetPacket
();
                    packet.Write((byte)MsgType.AddDamageDoTDebuffPlayer);
                    packet.Write(playerID);
                    packet.Write(buffType);
                    packet.Write(damage);
                    packet.Write(time);
                    packet.Send(-1, whoAmI);
                }
            }
            else if (msg == MsgType.AddMoveSpeedBuffPlayer)
            {
                int playerID = reader.ReadInt32();
                float speedMultiplier = reader.ReadInt32();
                int time = reader.ReadInt32();

                Player player = Main.player[playerID];
                PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
                pomPlayer.AddMoveSpeedBuff(player, speedMultiplier, time, false);

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = PathOfModifiers.Instance.GetPacket
();
                    packet.Write((byte)MsgType.AddMoveSpeedBuffPlayer);
                    packet.Write(playerID);
                    packet.Write(speedMultiplier);
                    packet.Write(time);
                    packet.Send(-1, whoAmI);
                }
            }
            else if (msg == MsgType.sOpenMapDeviceMap)
            {
                int mdID = reader.ReadInt32();
                var mapDevice = (MapDeviceTE)TileEntity.ByID[mdID];
                mapDevice.OpenMap();
            }
            else if (msg == MsgType.sCloseMapDeviceMap)
            {
                int mdID = reader.ReadInt32();
                var mapDevice = (MapDeviceTE)TileEntity.ByID[mdID];
                mapDevice.CloseMap();
            }
            else if (msg == MsgType.sModifierForgeModifiedItemChanged)
            {
                byte ignoreClient = reader.ReadByte();
                int mfID = reader.ReadInt32();
                var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
                mf.modifiedItem = ItemIO.Receive(reader, true);
                mf.SendToClients(ignoreClient);
            }
            else if (msg == MsgType.sModifierForgeModifierItemChanged)
            {
                byte ignoreClient = reader.ReadByte();
                int mfID = reader.ReadInt32();
                var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
                mf.modifierItem = ItemIO.Receive(reader, true);
                mf.SendToClients(ignoreClient);
            }
            else if (msg == MsgType.sMapDeviceMapItemChanged)
            {
                byte ignoreClient = reader.ReadByte();
                int mdID = reader.ReadInt32();
                var md = (MapDeviceTE)TileEntity.ByID[mdID];
                md.mapItem = ItemIO.Receive(reader, true);
                md.SendToClients(ignoreClient);
            }
            else if (msg == MsgType.cNPCSyncAffixes)
            {
                var npc = Main.npc[reader.ReadByte()];
                var pomNPC = npc.GetGlobalNPC<PoMNPC>();
                pomNPC.NetReceive(reader, npc);
            }
            else if (msg == MsgType.sSpawnNPC)
            {
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var type = reader.ReadInt32();

                var npc = PoMHelper.SpawnNPC(x, y, type, false, true);

                if (npc == null)
                    goto SkipMsgIf;

                var pomNPC = npc.GetGlobalNPC<PoMNPC>();

                var rarityID = reader.ReadInt32();
                pomNPC.rarity = PoMDataLoader.raritiesNPC[rarityID];

                var affixesLength = reader.ReadInt32();
                for (int i = 0; i < affixesLength; i++)
                {
                    var affixID = reader.ReadInt32();
                    var affixTier = reader.ReadInt32();
                    var affixTierMultiplier = reader.ReadSingle();
                    var affix = PoMDataLoader.affixesNPC[affixID].Clone();
                    if (affix is ITieredStatFloatAffix fAffix)
                    {
                        TieredAffixHelper.SetTier(fAffix, affixTier);
                        TieredAffixHelper.SetTierMultiplier(fAffix, affixTierMultiplier);
                    }
                    if (affix is ITieredStatIntAffix iAffix)
                    {
                        TieredAffixHelper.SetTier(iAffix, affixTier);
                        TieredAffixHelper.SetTierMultiplier(iAffix, affixTierMultiplier);
                    }
                    if (affix is ITieredStatIntValueAffix ivAffix)
                    {
                        TieredAffixHelper.SetTier(ivAffix, affixTier);
                    }

                    pomNPC.AddAffix(affix, npc);
                }
                pomNPC.InitializeNPC(npc);
                pomNPC.UpdateName(npc);
                cNPCSyncAffixes(npc, pomNPC);
            }
            else if (msg == MsgType.SyncHealEffect)
            {
                //TODO: send bytes for player IDs not ints
                int playerID = reader.ReadInt32();
                int amount = reader.ReadInt32();

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = PathOfModifiers.Instance.GetPacket();
                    packet.Write((byte)MsgType.SyncHealEffect);
                    packet.Write(playerID);
                    packet.Write(amount);
                    packet.Send(-1, whoAmI);
                }
                else
                {
                    Player player = Main.player[playerID];
                    player.HealEffect(amount, false);
                    for (int i = 0; i < 7; i++)
                    {
                        Vector2 dustPosition = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(0, player.height));
                        Vector2 dustVelocity = new Vector2(0, -Main.rand.NextFloat(0.5f, 2.5f));
                        float dustScale = Main.rand.NextFloat(1f, 2.5f);
                        Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.HealEffect>(), dustVelocity, Scale: dustScale);
                    }
                }
            }

        SkipMsgIf:;
        }

        public static void PlayerConnected(int whoAmI)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.PlayerConnected);
            packet.Write((byte)whoAmI);
            packet.Send();
        }
        public static void AddDamageDoTDebuffPlayer(int whoAmI, int buffType, int damage, int time)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.AddDamageDoTDebuffPlayer);
            packet.Write(whoAmI);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(time);
            packet.Send();
        }
        public static void AddDamageDoTDebuffNPC(int whoAmI, int buffType, int damage, int time)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.AddDamageDoTDebuffNPC);
            packet.Write(whoAmI);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(time);
            packet.Send();
        }
        public static void AddMoveSpeedBuffPlayer(int whoAmI, float speedMultiplier, int time)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.AddMoveSpeedBuffPlayer);
            packet.Write(whoAmI);
            packet.Write(speedMultiplier);
            packet.Write(time);
            packet.Send();
        }
        /// <summary>
        /// Syncs all map tiles, walls and NPCs to the clients.
        /// </summary>
        public static void SyncOpenedMap(Rectangle dimensions, bool closeMap = false)
        {
            NetMessage.SendTileRange(-1, dimensions.X - 1, dimensions.Y - 1, dimensions.Width + 2, dimensions.Height + 2);

            ModPacket packet = PathOfModifiers.Instance.GetPacket();

            var mapBounds = new Rectangle(dimensions.X * 16, dimensions.Y * 16, dimensions.Width * 16, dimensions.Height * 16);
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (mapBounds.Intersects(npc.Hitbox))
                {
                    if (closeMap)
                    {
                        npc.active = false;
                    }
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
                }
            }
            //NetMessage.CompressTileBlock();
            //NetMessage.SendData(MessageID.TileSection, -1, whoAmI, null, )
            //NetMessage.SendTileRange(-1, pos.X - 1, pos.Y - 1, size.X + 2, size.Y + 2);
        }
        /// <summary>
        /// Asks the server to open the map located in the map device.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="map"></param>
        public static void OpenMapDeviceMap(int mapDeviceID)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.sOpenMapDeviceMap);
            packet.Write(mapDeviceID);
            packet.Send();
        }
        public static void CloseMapDeviceMap(int mapDeviceID)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.sCloseMapDeviceMap);
            packet.Write(mapDeviceID);
            packet.Send();
        }
        public static void sModifierForgeModifiedItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.sModifierForgeModifiedItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        public static void sModifierForgeModifierItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.sModifierForgeModifierItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        public static void sMapDeviceMapItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.sMapDeviceMapItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        public static void cNPCSyncAffixes(NPC npc, PoMNPC pomNPC)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.cNPCSyncAffixes);
            packet.Write((byte)npc.whoAmI);
            pomNPC.NetSend(packet);
            packet.Send();
        }
        public static void sSpawnNPC(int x, int y, int type, int rarityID, (int, int, float)[] affixes)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.sSpawnNPC);
            packet.Write(x);
            packet.Write(y);
            packet.Write(type);
            packet.Write(rarityID);
            packet.Write(affixes.Length);
            for (int i = 0; i < affixes.Length; i++)
            {
                var idValue = affixes[i];
                packet.Write(idValue.Item1);
                packet.Write(idValue.Item2);
                packet.Write(idValue.Item3);
            }
            packet.Send();
        }
        public static void SyncHealEffect(int whoAmI, int amount)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.SyncHealEffect);
            packet.Write(whoAmI);
            packet.Write(amount);
            packet.Send();
        }
    }

    /// <summary>
    /// First letter tells which direction the message is sent. c = to the cient.
    /// </summary>
    enum MsgType
    {
        cSyncDataMaps,
        PlayerConnected,
        AddDamageDoTDebuffNPC,
        AddDamageDoTDebuffPlayer,
        AddMoveSpeedBuffPlayer,
        sOpenMapDeviceMap,
        sCloseMapDeviceMap,
        sModifierForgeModifiedItemChanged,
        sModifierForgeModifierItemChanged,
        sMapDeviceMapItemChanged,
        cNPCSyncAffixes,
        sSpawnNPC,
        SyncHealEffect,
    }
}
