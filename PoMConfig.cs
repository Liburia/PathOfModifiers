using Newtonsoft.Json;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace PathOfModifiers
{
    public class PoMConfigServer : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Disable vanilla prefixes on weapons")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool DisableVanillaPrefixesWeapons;

        [Label("Disable vanilla prefixes on accessories")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool DisableVanillaPrefixesAccessories;

        [Label("Disable maps")]
        [DefaultValue(true)]
        [ReloadRequired]
        [JsonIgnore]
        public bool DisableMaps;
        [Label("Disable NPC modifiers")]
        [DefaultValue(true)]
        [ReloadRequired]
        [JsonIgnore]
        public bool DisableNPCModifiers;

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            return false;
        }
    }
}