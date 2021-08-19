using Microsoft.Xna.Framework;
using PathOfModifiers.UI;
using PathOfModifiers.UI.Chat;
using PathOfModifiers.UI.States;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace PathOfModifiers.Systems
{
	public class UI : ModSystem
	{
        GameTime _lastUpdateUiGameTime;

        static ModKeybind key_toggleDebugMenu;
        static UserInterface debugPanelInterface;
        static DebugPanel debugUIState;
        public static void ToggleDebugPanel()
        {
            if (debugPanelInterface.CurrentState == null)
                debugPanelInterface?.SetState(debugUIState);
            else
                debugPanelInterface?.SetState(null);
        }

        static ModKeybind key_toggleModifierForgeMenu;
        static UserInterface modifierForgeInterface;
        static ModifierForge modifierForgeState;
        public static void ToggleModifierForgeState()
        {
            if (modifierForgeInterface.CurrentState == null)
                modifierForgeInterface?.SetState(modifierForgeState);
            else
                modifierForgeInterface?.SetState(null);
        }

        public EventHandler OnWorldLoaded;

        public override void OnWorldLoad()
        {
            debugUIState?.RecreateAffixElements();
        }

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Terraria.UI.Chat.ChatManager.Register<KeywordTagHandler>(new string[1] { "pomkw" });

                //TODO: Remove keybinds? Only allow in debug build?
                key_toggleDebugMenu = KeybindLoader.RegisterKeybind(Mod, "Toggle Debug Menu", Microsoft.Xna.Framework.Input.Keys.None);
                debugPanelInterface = new UserInterface();
                debugUIState = new DebugPanel();
                debugUIState.Activate();

                key_toggleModifierForgeMenu = KeybindLoader.RegisterKeybind(Mod, "Toggle Modifier Forge", Microsoft.Xna.Framework.Input.Keys.None);
                modifierForgeInterface = new UserInterface();
                modifierForgeState = new ModifierForge();
                modifierForgeState.Activate();
            }
        }
        public override void Unload()
        {
            key_toggleDebugMenu = null;
            debugPanelInterface = null;
            debugUIState = null;

            key_toggleModifierForgeMenu = null;
            modifierForgeInterface = null;
            modifierForgeState = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;

            if (key_toggleDebugMenu.JustPressed)
                ToggleDebugPanel();
            debugPanelInterface?.Update(gameTime);

            if (key_toggleModifierForgeMenu.JustPressed)
                ToggleModifierForgeState();
            modifierForgeInterface?.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(++inventoryIndex, new LegacyGameInterfaceLayer(
                    "PathOfModifiers: Debug Panel",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && debugPanelInterface?.CurrentState != null)
                        {
                            debugPanelInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));

                layers.Insert(++inventoryIndex, new LegacyGameInterfaceLayer(
                    "PathOfModifiers: Modifier Forge",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && modifierForgeInterface?.CurrentState != null)
                        {
                            modifierForgeInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}