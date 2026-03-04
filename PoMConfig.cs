using Newtonsoft.Json;
using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace PathOfModifiers
{
    public class PoMConfigServer : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Try disable vanilla prefixes on weapons")]
        [DefaultValue(true)]
        public bool DisableVanillaPrefixesWeapons;

        [Label("Try disable vanilla prefixes on accessories")]
        [DefaultValue(true)]
        public bool DisableVanillaPrefixesAccessories;

        [Label("Modifier Forge cost difficulty")]
        [DefaultValue(UI.States.ModifierForge.Difficulty.Normal)]
        public UI.States.ModifierForge.Difficulty ModifierForgeCostDifficulty;

        [Label("Enable debug panel hotkey")]
        [DefaultValue(false)]
        public bool EnableDebugPanel;


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

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
        {
            return false;
        }
    }
}