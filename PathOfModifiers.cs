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

namespace PathOfModifiers
{
	class PathOfModifiers : Mod
    {
        public static bool log = true;
        public static bool logLoad = false;
        public static bool logNetwork = false;
        public static bool disableVanillaModifiersWeapons = true;
        public static bool disableVanillaModifiersAccessories = true;

        public static PathOfModifiers Instance { get; private set; }
        
        public static UserInterface modifierForgeUI;

        public static void Log(string message)
        {
            if (log)
                ErrorLogger.Log(message);
        }

        public PathOfModifiers()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}
        
        public override void Load()
        {
            Instance = this;

            AddPrefix("", new PoMPrefix());

            PoMAffixController.RegisterMod(this);

            if (Main.netMode != 2)
            {
                new ModifierForgeUI().Initialize();
                modifierForgeUI = new UserInterface();
                ModifierForgeUI.Instance.Visible = false;
            }
        }
        public override void PostSetupContent()
        {
            PoMAffixController.Initialize();
        }
        public override void Unload()
        {
            Instance = null;
            PoMAffixController.Unload();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MsgType msg = (MsgType)reader.ReadByte();
            if (logNetwork)
                Log($"Msg Received: {Main.netMode.ToString()}/{msg.ToString()}");

            if (msg == MsgType.SyncMaps)
            {
                PoMAffixController.ReceiveMaps(reader);
            }
            else if (msg == MsgType.PlayerConnected)
            {
                int player = reader.ReadByte();
                ModPacket packet = GetPacket();
                packet.Write((byte)MsgType.SyncMaps);
                PoMAffixController.SendMaps(packet);
                packet.Send(player);
            }
            else if (msg == MsgType.SyncTEModifierForge)
            {
                int id = reader.ReadInt32();
                bool contains = reader.ReadBoolean();
                if (contains)
                {
                    TEModifierForge tileEntity;
                    tileEntity = (TEModifierForge)TileEntity.Read(reader, true);
                    tileEntity.ID = id;
                    TileEntity.ByID[tileEntity.ID] = tileEntity;
                    TileEntity.ByPosition[tileEntity.Position] = tileEntity;
                    tileEntity.Sync(tileEntity.ID, whoAmI);
                }
                else
                {
                    TileEntity tileEntity;
                    if (TileEntity.ByID.TryGetValue(id, out tileEntity) && tileEntity is ModTileEntity)
                    {
                        TileEntity.ByID.Remove(id);
                        TileEntity.ByPosition.Remove(tileEntity.Position);
                    }
                }
            }
            else if (msg == MsgType.AddDamageDoTDebuffNPC)
            {
                int npcID = reader.ReadInt32();
                int buffType = reader.ReadInt32();
                int damage = reader.ReadInt32();
                int time = reader.ReadInt32();

                DamageDoTDebuff debuff = BuffLoader.GetBuff(buffType) as DamageDoTDebuff;
                if (debuff == null)
                {
                    Log($"PathOfModifiers: Invalid buff packet received {buffType}");
                    goto SkipMsgIf;
                }
                NPC npc = Main.npc[npcID];
                PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
                pomNPC.AddDamageDoTBuff(npc, debuff, damage, time, false);

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = GetPacket();
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

                DamageDoTDebuff debuff = BuffLoader.GetBuff(buffType) as DamageDoTDebuff;
                if (debuff == null)
                {
                    Log($"PathOfModifiers: Invalid buff packet received {buffType}");
                    goto SkipMsgIf;
                }
                Player player = Main.player[playerID];
                PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
                pomPlayer.AddDamageDoTBuff(player, debuff, damage, time, false);

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = GetPacket();
                    packet.Write((byte)MsgType.AddDamageDoTDebuffPlayer);
                    packet.Write(playerID);
                    packet.Write(buffType);
                    packet.Write(damage);
                    packet.Write(time);
                    packet.Send(-1, whoAmI);
                }
            }

            SkipMsgIf:;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            modifierForgeUI?.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "PathOfModifiers: Modifier Forge",
                    delegate
                    {
                        if (ModifierForgeUI.Instance.Visible)
                        {
                            ModifierForgeUI.Instance.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void PreSaveAndQuit()
        {
            ModifierForgeUI.Instance.Visible = false;
        }
    }

    enum MsgType
    {
        SyncMaps,
        PlayerConnected,
        SyncTEModifierForge,
        AddDamageDoTDebuffNPC,
        AddDamageDoTDebuffPlayer,
    }
}
