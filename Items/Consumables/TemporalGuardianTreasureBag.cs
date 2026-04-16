using TheTesseractMod.NPCs.Bosses.GuardianOfTheRift;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Items.Consumables
{
	// Basic code for a boss treasure bag
	public class TemporalGuardianTreasureBag : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.BossBag[Type] = true;

			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults() {
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Expert;
			Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
		}

		public override bool CanRightClick() {
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) {
			// We have to replicate the expert drops from MinionBossBody here
            
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<GuardianOfTheRiftBody>()));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Ores.TemporalOre>(), 1, 19, 30));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Ores.SoliumOre>(), 1, 20, 35));

			itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, 
				ModContent.ItemType<Weapons.Melee.PowerHammer>(),
				ModContent.ItemType<Weapons.Melee.SiphonAxes>(),
				ModContent.ItemType<Weapons.Magic.RiftFracture>(),
				ModContent.ItemType<Weapons.Magic.StormOfThorns>(),
				ModContent.ItemType<Weapons.Ranged.DragonsBreath>(),
				ModContent.ItemType<Weapons.Ranged.BlizzardCannon>(),
				ModContent.ItemType<Weapons.Summoner.SquidOfTheAbyssScepter>(),
				ModContent.ItemType<Weapons.Summoner.WhipOfTheWildWest>()
			));
		}
	}
}