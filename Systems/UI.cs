using Microsoft.Xna.Framework;
using PathOfModifiers.UI;
using PathOfModifiers.UI.Chat;
using PathOfModifiers.UI.States;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
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

        static UserInterface modifierForgeInterface;
        static ModifierForge modifierForgeState;
        public static bool IsModifierForgeOpen => modifierForgeInterface.CurrentState != null;
        public static void OpenModifierForge(Tiles.ModifierForgeTE forge)
        {
            modifierForgeState.CurrentForgeTE = forge;
            modifierForgeInterface?.SetState(modifierForgeState);
            Main.playerInventory = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
        }
        public static void CloseModifierForge()
        {
            modifierForgeState.CurrentForgeTE = null;
            modifierForgeInterface?.SetState(null);
            SoundEngine.PlaySound(SoundID.MenuClose);
        }

        public EventHandler OnWorldLoaded;

        public override void OnWorldLoad()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                debugUIState?.RecreateAffixElements();
            }
        }

        public override void Load()
        {
            //TODO: Remove keybinds? Only allow in debug build?
            key_toggleDebugMenu = KeybindLoader.RegisterKeybind(Mod, "Toggle Debug Menu", Microsoft.Xna.Framework.Input.Keys.None);

            if (Main.netMode != NetmodeID.Server)
            {
                Terraria.UI.Chat.ChatManager.Register<KeywordTagHandler>(new string[1] { "pomkw" });
                Terraria.UI.Chat.ChatManager.Register<TierTagHandler>(new string[1] { "pomtier" });
                Terraria.UI.Chat.ChatManager.Register<ValueRangeTagHandler>(new string[1] { "pomvr" });

                debugPanelInterface = new UserInterface();
                debugUIState = new DebugPanel();
                debugUIState.Activate();

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

            modifierForgeInterface = null;
            modifierForgeState = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;

            if (key_toggleDebugMenu.JustPressed)
                ToggleDebugPanel();
            debugPanelInterface?.Update(gameTime);

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