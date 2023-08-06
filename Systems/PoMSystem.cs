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
using PathOfModifiers.ModNet;
using Terraria.ModLoader.IO;

namespace PathOfModifiers
{
    class PoMSystem : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup group = new(() => Language.GetTextValue("LegacyMisc.37") + " Copper Bar", new int[]
            {
            ItemID.CopperBar,
            ItemID.TinBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:CopperBar", group);

            group = new(() => Language.GetTextValue("LegacyMisc.37") + " Silver Bar", new int[]
            {
            ItemID.SilverBar,
            ItemID.TungstenBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:SilverBar", group);

            group = new(() => Language.GetTextValue("LegacyMisc.37") + " Gold Bar", new int[]
            {
            ItemID.GoldBar,
            ItemID.PlatinumBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:GoldBar", group);
        }
    }
}
