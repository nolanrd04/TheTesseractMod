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
using Terraria.UI;
using TheTesseractMod.ItemDropRulesANDConditions;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.Items.Weapons.NoSpecificClass;
using TheTesseractMod.Items.Weapons.Ranged;

namespace TheTesseractMod.Global.Bosses
{
    internal class GlobalBoss : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {
            if (entity.type == NPCID.Golem && !ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                entity.lifeMax = (int)(entity.lifeMax * 1.5f);
                entity.defense += 25;
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            //Adds atom of time drop to every boss
            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TemporalOre>(), 1, 4, 9));
            }

            if (npc.type == NPCID.MoonLordCore)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThePeppermint>()));
            }

            // add Petashark to Duke Fishron drop loot
            if (npc.type == NPCID.DukeFishron)
            {
                npcLoot.Add(ItemDropRule.ByCondition(new IsNormalMode(), ModContent.ItemType<Petashark>(), 6));
                
            }
        }
    }

    public class BossBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            // Add Petashark to Duke Fishron Bag
            if (item.type == ItemID.FishronBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Petashark>(), 6));
                LeadingConditionRule notExpert = new LeadingConditionRule(new Conditions.NotExpert());
                notExpert.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Petashark>(), 6));
            }
        }
    }
}
