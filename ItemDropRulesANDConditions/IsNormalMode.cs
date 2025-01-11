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
    internal class IsNormalMode : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public IsNormalMode()
        {
            Description ??= Language.GetOrRegister("If the world is in normal mode.");
        }
        public bool CanDrop(DropAttemptInfo info)
        {
            return !Main.expertMode;
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
