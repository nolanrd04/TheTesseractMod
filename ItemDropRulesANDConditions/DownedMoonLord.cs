using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace TheTesseractMod.ItemDropRulesANDConditions
{
    internal class DownedMoonLord : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public DownedMoonLord()
        {
            Description ??= Language.GetOrRegister("Moon Lord Defeated");
        }
        public bool CanDrop(DropAttemptInfo info)
        {
            return NPC.downedMoonlord;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }
}
