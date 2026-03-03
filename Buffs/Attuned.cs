using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs
{
    internal class Attuned : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.07f;
            player.lifeRegen += 2;
            player.statDefense += 6;
        }
    }
}
