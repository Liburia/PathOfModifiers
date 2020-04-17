using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ID;
using System.IO;
using System.Collections.Generic;

namespace PathOfModifiers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DisableAffix : Attribute
    {

    }
}
