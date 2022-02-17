using Microsoft.Xna.Framework;
using PathOfModifiers.UI.Chat;
using PathOfModifiers.UI.States;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace PathOfModifiers.Systems
{
    public class UI : ModSystem
    {
        GameTime _lastUpdateUiGameTime;

        static ModKeybind key_toggleDebugMenu;

        public static UserInterface DebugPanelInterface { get; private set; }
        public static DebugPanel DebugUIState { get; private set; }

        public static UserInterface ModifierForgeInterface { get; private set; }
        public static ModifierForge ModifierForgeState { get; private set; }
        public static bool IsModifierForgeOpen => ModifierForgeInterface.CurrentState != null;


        public EventHandler OnWorldLoaded;

        public override void OnWorldLoad()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                DebugUIState?.RecreateAffixElements();
            }
        }

        public override void OnWorldUnload()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ModifierForgeState.CurrentForgeTE = null;
                ModifierForgeInterface.SetState(null);
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

                DebugPanelInterface = new UserInterface();
                DebugUIState = new DebugPanel();
                DebugUIState.Activate();

                ModifierForgeInterface = new UserInterface();
                ModifierForgeState = new ModifierForge();
                ModifierForgeState.Activate();
            }
        }
        public override void Unload()
        {
            key_toggleDebugMenu = null;
            DebugPanelInterface = null;
            DebugUIState = null;

            ModifierForgeInterface = null;
            ModifierForgeState = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;

            if (key_toggleDebugMenu.JustPressed)
                DebugPanel.Toggle();
            DebugPanelInterface?.Update(gameTime);

            ModifierForgeInterface?.Update(gameTime);
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
                        if (_lastUpdateUiGameTime != null && DebugPanelInterface?.CurrentState != null)
                        {
                            DebugPanelInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));

                layers.Insert(++inventoryIndex, new LegacyGameInterfaceLayer(
                    "PathOfModifiers: Modifier Forge",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && ModifierForgeInterface?.CurrentState != null)
                        {
                            ModifierForgeInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}