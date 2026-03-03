using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs
{
    internal class Sealed : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.1f;
            player.lifeRegen += 4;
            player.statDefense += 8;
        }
    }
}
