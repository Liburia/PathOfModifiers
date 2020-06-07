using log4net.DateFormatter;
using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes;
using PathOfModifiers.Affixes.Items;
using System;
using Terraria;
using Terraria.Chat.Commands;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PathOfModifiers.ChatCommands
{
    public class Pom : ModCommand
    {
        static class CmdType
        {
            /// <summary>
            /// add 'p|s' affixType [ tier1 tierMultiplier1 tier2 tierMultiplier2 tier3 tierMultiplier3 ]
            /// </summary>
            public const string addAffix = "add";
            /// <summary>
            /// clear
            /// </summary>
            public const string clearAffixes = "clear";
            /// <summary>
            /// mod 'p|s' tier1 tierMultiplier1 tier2 tierMultiplier2 tier3 tierMultiplier3
            /// </summary>
            public const string modifyAffix = "mod";
        }

        public override string Command => "pom";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                Main.NewTextMultiline($"Commands: { CmdType.clearAffixes }, { CmdType.addAffix }, { CmdType.modifyAffix }");
                return;
            }

            Player player = Main.LocalPlayer;
            Item item = player.HeldItem;
            AffixItemItem pomItem = item.GetGlobalItem<AffixItemItem>();

            switch (args[0])
            {
                case CmdType.clearAffixes:
                    pomItem.ClearAffixes(item);
                    break;
                case CmdType.addAffix:
                    bool isPrefix = true;
                    if (args[1] == "s")
                    {
                        isPrefix = false;
                    }
                    string affixType = args[2];
                    Affix affix = PoMAffixController.GetAffix(pomItem, affixType, isPrefix);

                    if (args.Length > 7)
                    {
                        if (affix is AffixTiered<TTFloat, TTFloat, TTFloat> affixTieredFFF)
                        {
                            affixTieredFFF.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredFFF.SetTierMultiplier(float.Parse(args[4]), float.Parse(args[6]), float.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTInt, TTFloat, TTFloat> affixTieredIFF)
                        {
                            affixTieredIFF.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredIFF.SetTierMultiplier(int.Parse(args[4]), float.Parse(args[6]), float.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTFloat, TTInt, TTFloat> affixTieredFIF)
                        {
                            affixTieredFIF.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredFIF.SetTierMultiplier(float.Parse(args[4]), int.Parse(args[6]), float.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTInt, TTInt, TTFloat> affixTieredIIF)
                        {
                            affixTieredIIF.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredIIF.SetTierMultiplier(int.Parse(args[4]), int.Parse(args[6]), float.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTFloat, TTFloat, TTInt> affixTieredFFI)
                        {
                            affixTieredFFI.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredFFI.SetTierMultiplier(float.Parse(args[4]), float.Parse(args[6]), int.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTInt, TTFloat, TTInt> affixTieredIFI)
                        {
                            affixTieredIFI.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredIFI.SetTierMultiplier(int.Parse(args[4]), float.Parse(args[6]), int.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTFloat, TTInt, TTInt> affixTieredFII)
                        {
                            affixTieredFII.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredFII.SetTierMultiplier(float.Parse(args[4]), int.Parse(args[6]), int.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTInt, TTInt, TTInt> affixTieredIII)
                        {
                            affixTieredIII.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredIII.SetTierMultiplier(int.Parse(args[4]), int.Parse(args[6]), int.Parse(args[8]));
                        }
                    }
                    else if (args.Length > 5)
                    {
                        if (affix is AffixTiered<TTFloat, TTFloat> affixTieredFF)
                        {
                            affixTieredFF.SetTier(int.Parse(args[3]), int.Parse(args[5]));
                            affixTieredFF.SetTierMultiplier(float.Parse(args[4]), float.Parse(args[6]));
                        }
                        else if (affix is AffixTiered<TTInt, TTFloat> affixTieredIF)
                        {
                            affixTieredIF.SetTier(int.Parse(args[3]), int.Parse(args[5]));
                            affixTieredIF.SetTierMultiplier(int.Parse(args[4]), float.Parse(args[6]));
                        }
                        else if (affix is AffixTiered<TTFloat, TTInt> affixTieredFI)
                        {
                            affixTieredFI.SetTier(int.Parse(args[3]), int.Parse(args[5]));
                            affixTieredFI.SetTierMultiplier(float.Parse(args[4]), int.Parse(args[6]));
                        }
                        else if (affix is AffixTiered<TTInt, TTInt> affixTieredII)
                        {
                            affixTieredII.SetTier(int.Parse(args[3]), int.Parse(args[5]));
                            affixTieredII.SetTierMultiplier(int.Parse(args[4]), int.Parse(args[6]));
                        }
                    }
                    else if (args.Length > 3)
                    {
                        if (affix is AffixTiered<TTFloat> affixTieredF)
                        {
                            affixTieredF.SetTier(int.Parse(args[3]));
                            affixTieredF.SetTierMultiplier(float.Parse(args[4]));
                        }
                        else if (affix is AffixTiered<TTInt> affixTieredI)
                        {
                            affixTieredI.SetTier(int.Parse(args[3]));
                            affixTieredI.SetTierMultiplier(float.Parse(args[4]));
                        }
                    }

                    pomItem.AddAffix(affix, item);
                    break;
                case CmdType.modifyAffix:
                    isPrefix = true;
                    if (args[1] == "s")
                    {
                        isPrefix = false;
                    }

                    int affixIndex = int.Parse(args[2]);

                    if (isPrefix)
                    {
                        affix = pomItem.prefixes[affixIndex];
                    }
                    else
                    {
                        affix = pomItem.suffixes[affixIndex];
                    }
                    if (args.Length > 7)
                    {
                        if (affix is AffixTiered<TTFloat, TTFloat, TTFloat> affixTieredFFF)
                        {
                            affixTieredFFF.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredFFF.SetTierMultiplier(float.Parse(args[4]), float.Parse(args[6]), float.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTInt, TTFloat, TTFloat> affixTieredIFF)
                        {
                            affixTieredIFF.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredIFF.SetTierMultiplier(int.Parse(args[4]), float.Parse(args[6]), float.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTFloat, TTInt, TTFloat> affixTieredFIF)
                        {
                            affixTieredFIF.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredFIF.SetTierMultiplier(float.Parse(args[4]), int.Parse(args[6]), float.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTInt, TTInt, TTFloat> affixTieredIIF)
                        {
                            affixTieredIIF.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredIIF.SetTierMultiplier(int.Parse(args[4]), int.Parse(args[6]), float.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTFloat, TTFloat, TTInt> affixTieredFFI)
                        {
                            affixTieredFFI.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredFFI.SetTierMultiplier(float.Parse(args[4]), float.Parse(args[6]), int.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTInt, TTFloat, TTInt> affixTieredIFI)
                        {
                            affixTieredIFI.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredIFI.SetTierMultiplier(int.Parse(args[4]), float.Parse(args[6]), int.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTFloat, TTInt, TTInt> affixTieredFII)
                        {
                            affixTieredFII.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredFII.SetTierMultiplier(float.Parse(args[4]), int.Parse(args[6]), int.Parse(args[8]));
                        }
                        else if (affix is AffixTiered<TTInt, TTInt, TTInt> affixTieredIII)
                        {
                            affixTieredIII.SetTier(int.Parse(args[3]), int.Parse(args[5]), int.Parse(args[7]));
                            affixTieredIII.SetTierMultiplier(int.Parse(args[4]), int.Parse(args[6]), int.Parse(args[8]));
                        }
                    }
                    else if (args.Length > 5)
                    {
                        if (affix is AffixTiered<TTFloat, TTFloat> affixTieredFF)
                        {
                            affixTieredFF.SetTier(int.Parse(args[3]), int.Parse(args[5]));
                            affixTieredFF.SetTierMultiplier(float.Parse(args[4]), float.Parse(args[6]));
                        }
                        else if (affix is AffixTiered<TTInt, TTFloat> affixTieredIF)
                        {
                            affixTieredIF.SetTier(int.Parse(args[3]), int.Parse(args[5]));
                            affixTieredIF.SetTierMultiplier(int.Parse(args[4]), float.Parse(args[6]));
                        }
                        else if (affix is AffixTiered<TTFloat, TTInt> affixTieredFI)
                        {
                            affixTieredFI.SetTier(int.Parse(args[3]), int.Parse(args[5]));
                            affixTieredFI.SetTierMultiplier(float.Parse(args[4]), int.Parse(args[6]));
                        }
                        else if (affix is AffixTiered<TTInt, TTInt> affixTieredII)
                        {
                            affixTieredII.SetTier(int.Parse(args[3]), int.Parse(args[5]));
                            affixTieredII.SetTierMultiplier(int.Parse(args[4]), int.Parse(args[6]));
                        }
                    }
                    else if (args.Length > 3)
                    {
                        if (affix is AffixTiered<TTFloat> affixTieredF)
                        {
                            affixTieredF.SetTier(int.Parse(args[3]));
                            affixTieredF.SetTierMultiplier(float.Parse(args[4]));
                        }
                        else if (affix is AffixTiered<TTInt> affixTieredI)
                        {
                            affixTieredI.SetTier(int.Parse(args[3]));
                            affixTieredI.SetTierMultiplier(float.Parse(args[4]));
                        }
                    }
                    break;
            }

            pomItem.UpdateName(item);
        }
    }
}
