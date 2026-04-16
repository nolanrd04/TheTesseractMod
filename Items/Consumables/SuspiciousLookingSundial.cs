using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.NPCs.Bosses.GuardianOfTheRift;

namespace TheTesseractMod.Items.Consumables
{
    public class SuspiciousLookingSundial : ModItem
    {
        public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 3;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
		}
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.maxStack = 20;
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<GuardianOfTheRiftBody>());
        }

        public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				// If the player using the item is the client
				// (explicitly excluded serverside here)
				SoundEngine.PlaySound(SoundID.Roar, player.position);

				int type = ModContent.NPCType<GuardianOfTheRiftBody>();

				if (Main.netMode != NetmodeID.MultiplayerClient) {
					// If the player is not in multiplayer, spawn directly
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else {
					// If the player is in multiplayer, request a spawn
					// This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in GuardianOfTheRiftBody
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
				}
			}

			return true;
		}

        public override void AddRecipes() {
            CreateRecipe()
            .AddIngredient(ItemID.LunarBar, 10)
            .AddIngredient(ModContent.ItemType<LightRiftFragment>(), 10)
            .AddIngredient(ModContent.ItemType<TemporalBar>(), 10)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}