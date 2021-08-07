//using Microsoft.Xna.Framework.Graphics;
//using Terraria.ModLoader;
//using Microsoft.Xna.Framework;
//using System;
//using Terraria.UI.Chat;
//using Terraria;
//using System.IO;
//using PathOfModifiers.Rarities;
//using Terraria.UI;
//using Terraria.ID;
//using System.Collections.Generic;
//using PathOfModifiers.UI;
//using PathOfModifiers.Tiles;
//using Terraria.DataStructures;
//using PathOfModifiers.Buffs;
//using Terraria.Localization;
//using PathOfModifiers.ModNet;
//using Terraria.ModLoader.IO;

//namespace PathOfModifiers
//{
//    class PoMSystem : ModSystem
//    {
//        public override void UpdateUI(GameTime gameTime)
//        {
//            PathOfModifiers.modifierForgeUI?.Update(gameTime);
//            PathOfModifiers.mapDeviceUI?.Update(gameTime);
//        }
//        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
//        {
//            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
//            if (inventoryIndex != -1)
//            {
//                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
//                    "PathOfModifiers: Modifier Forge",
//                    delegate
//                    {
//                        if (ModifierForgeUI.Instance.IsVisible)
//                        {
//                            ModifierForgeUI.Instance.Draw(Main.spriteBatch);
//                        }
//                        return true;
//                    },
//                    InterfaceScaleType.UI)
//                );
//                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
//                    "PathOfModifiers: Map Device",
//                    delegate
//                    {
//                        if (MapDeviceUI.Instance.IsVisible)
//                        {
//                            MapDeviceUI.Instance.Draw(Main.spriteBatch);
//                        }
//                        return true;
//                    },
//                    InterfaceScaleType.UI)
//                );
//            }
//        }

//        public override void PreSaveAndQuit()
//        {
//            ModifierForgeUI.HideUI();
//            MapDeviceUI.HideUI();
//        }

//        public override void PostDrawInterface(SpriteBatch spriteBatch)
//        {
//#if DEBUG
//            PoMDebug.Draw(spriteBatch);
//#endif
//        }
//    }
//}
