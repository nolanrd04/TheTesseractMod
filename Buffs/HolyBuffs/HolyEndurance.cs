using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs.HolyBuffs
{
    internal class HolyEndurance : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.05f;
        }
    }
}
