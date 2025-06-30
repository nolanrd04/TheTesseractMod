using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TheTesseractMod.ItemDropRulesANDConditions;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.DungeonWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.JungleWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueExcaliburWeapons;

namespace TheTesseractMod.Global.NPCs
{
    internal class ModifyVanillaNPCLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) // ADD MODDED DROPS TO VANILLA ENEMIES
        {
            if (npc.type == NPCID.FaceMonster || npc.type == NPCID.EaterofSouls) // ADD PURPLE HEART TO DROP POOLS
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PurpleHeart>(), 50, 1, 1));
            }

            if(npc.type == NPCID.SpikedJungleSlime) // ADD STINGER STORM TO DROP POOLS
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StingerStorm>(), 50, 1, 1));
            }

            if (RiftFragmentsFromVanillaEnemies.ChloroCheck(npc)) // ADD TRUE YELETES TO JUNGLE NPCS
            {
                npcLoot.Add(ItemDropRule.ByCondition(new AllMechsDefeated(), ModContent.ItemType<TrueYelets>(), 100));
            }

            if (npc.type == NPCID.DarkCaster) // ADD DARK CASTER STAFF TO DARK CASTER DROPS
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkCasterStaff>(), 25, 1, 1));
            }
        }

        
    }
    
    internal class AllMechsDefeated : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public AllMechsDefeated()
        {
            Description ??= Language.GetOrRegister("If all of the mechs have been defeated");
        }
        public bool CanDrop(DropAttemptInfo info)
        {
            return NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }
}
