using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs.TemporalGuardianBuffs
{
    internal class DimensionalIncompatability : ModBuff // Is applied to the player every update when they are too far from the boss.
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<_BuffsPlayerHander>().dimensionalIncompatability = true;
        }
    }
}
