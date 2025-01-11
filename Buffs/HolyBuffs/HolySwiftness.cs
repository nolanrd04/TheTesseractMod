using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs.HolyBuffs
{
    internal class HolySwiftness : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.25f;
            player.runAcceleration *= 1.75f;
        }
    }
}
