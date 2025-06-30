using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs
{
    internal class _BuffsPlayerHander : ModPlayer
    {
        public bool dimensionalIncompatability;
        public bool diedToDimensionalIncompatibility = false;

        public override void ResetEffects()
        {
            dimensionalIncompatability = false;
            diedToDimensionalIncompatibility = false;
        }

        public override void UpdateBadLifeRegen()
        {
            if (dimensionalIncompatability)
            {
                Player.lifeRegen = 0;

                
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 160;
            }
        }

        
    }
}
