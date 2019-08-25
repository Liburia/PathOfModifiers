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
using Terraria.Localization;

namespace PathOfModifiers
{
	class PathOfModifiers : Mod
    {
        //TODO: config
        public const bool disableVanillaModifiersWeapons = true;
        public const bool disableVanillaModifiersAccessories = true;
        public const bool disableMaps = true;

        public static string pathMapIcons = "Images/MapIcons/";

        public static PathOfModifiers Instance { get; private set; }

        public static UserInterface modifierForgeUI;
        public static UserInterface mapDeviceUI;
        
        public PathOfModifiers()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

        public override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Copper Bar", new int[]
            {
            ItemID.CopperBar,
            ItemID.TinBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:CopperBar", group);

            group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Silver Bar", new int[]
            {
            ItemID.SilverBar,
            ItemID.TungstenBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:SilverBar", group);

            group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Gold Bar", new int[]
            {
            ItemID.GoldBar,
            ItemID.PlatinumBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:GoldBar", group);
        }

        public override void Load()
        {
            Instance = this;

            AddPrefix("", new PoMPrefix());

            PoMDataLoader.RegisterMod(this);
        }
        public override void PostSetupContent()
        {
            PoMDataLoader.Initialize();

            if (Main.netMode != 2)
            {
                new ModifierForgeUI().Initialize();
                modifierForgeUI = new UserInterface();
                ModifierForgeUI.HideUI();

                new MapDeviceUI().Initialize();
                mapDeviceUI = new UserInterface();
                MapDeviceUI.HideUI();
            }
        }
        public override void Unload()
        {
            Instance = null;
            modifierForgeUI = null;
            mapDeviceUI = null;
            PoMDataLoader.Unload();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PoMNetMessage.HandlePacket(reader, whoAmI);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            modifierForgeUI?.Update(gameTime);
            mapDeviceUI?.Update(gameTime);
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
                        if (ModifierForgeUI.Instance.IsVisible)
                        {
                            ModifierForgeUI.Instance.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "PathOfModifiers: Map Device",
                    delegate
                    {
                        if (MapDeviceUI.Instance.IsVisible)
                        {
                            MapDeviceUI.Instance.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void PreSaveAndQuit()
        {
            ModifierForgeUI.HideUI();
            MapDeviceUI.HideUI();
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
#if DEBUG
            PoMDebug.Draw(spriteBatch);
#endif
        }
    }
}
