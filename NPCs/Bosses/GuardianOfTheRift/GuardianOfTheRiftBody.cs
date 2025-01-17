using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TheTesseractMod.NPCs.Bosses.GuardianOfTheRift
{
    internal class GuardianOfTheRiftBody /* : ModNPC */
    {
        // phase is a number 0-10 to represent which element to use: 0 heat, 1 dust, 2 light, 3 chloro, 4 aqua, 5 cold, 6 glow, 7 electric, 8 dark, 9 life , 10 death
        public int elementIdx = 0;
        public enum Element
        {
            Heat,
            Dust,
            Light,
            Chloro,
            Aqua,
            Cold, 
            Glow,
            Electric,
            Dark,
            Life,
            Death
        }

        // boss has two phases (0,1)
        public int phase = 0;
    }
}
