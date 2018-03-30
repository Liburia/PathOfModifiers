using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using System.IO;
using PathOfModifiers.Rarities;

namespace PathOfModifiers
{
	class PathOfModifiers : Mod
	{
        public static bool log = true;
        public static bool disableVanillaModifiersWeapons = true;
        public static bool disableVanillaModifiersAccessories = true;

        public static PathOfModifiers Instance { get; private set; }

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
        }
    }

    enum MsgType
    {
        SyncMaps,
        PlayerConnected,
    }
}
