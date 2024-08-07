using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Weapons.NoSpecificClass;

namespace TheTesseractMod.Global.Bosses
{
    internal class GlobalBoss : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            //Adds atom of time drop to every boss
            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AtomOfTime>(), 1, 5, 8));
            }

            if (npc.type == NPCID.MoonLordCore)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThePeppermint>()));
            }
        }
    }
}
