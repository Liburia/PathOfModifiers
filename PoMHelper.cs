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

namespace PathOfModifiers
{
	public static class PoMHelper
    {
        public static int CountBuffs(int[] buffs)
        {
            int buffCount = 0;
            for (int i = 0; i < buffs.Length; i++)
            {
                if (buffs[i] > 0)
                {
                    buffCount++;
                }
            }
            return buffCount;
        }
	}
}
