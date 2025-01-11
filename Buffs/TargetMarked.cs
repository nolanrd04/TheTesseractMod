using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs
{
    internal class TargetMarked : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.buffTime[buffIndex] % 3 == 0)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Terra, 0, 0, 150, default(Color), 1f);
            }
        }
    }
}
