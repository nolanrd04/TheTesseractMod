using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using TheTesseractMod.Items.Consumables;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.Items.Placeable.Furniture;
using TheTesseractMod.Items.Weapons.Melee;
using TheTesseractMod.Items.Weapons.Ranged;
using TheTesseractMod.Items.Weapons.Summoner;
using TheTesseractMod.NPCs.Bosses.GuardianOfTheRift;
using TheTesseractMod.Pets;
using TheTesseractMod.Items.Materials;

namespace TheTesseractMod.Systems
{
	public class ModIntegrationsSystem : ModSystem
	{
		public override void PostSetupContent() {
			// Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees various data is initialized and set up properly

			// Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
			// https://forums.terraria.org/index.php?threads/.50668/
			DoBossChecklistIntegration();

			// We can integrate with other mods here by following the same pattern. Some modders may prefer a ModSystem for each mod they integrate with, or some other design.
		}

		private void DoBossChecklistIntegration() {
			// The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call
			// If we navigate the wiki, we can find the "LogBoss" method, which we want in this case
			// A feature of the call is that it will create an entry in the localization file of the specified NPC type for its spawn info, so make sure to visit the localization file after your mod runs once to edit it

			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod)) {
				return;
			}

			// For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod
			// Usually mods either provide that information themselves in some way, or it's found on the GitHub through commit history/blame
			if (bossChecklistMod.Version < new Version(2, 2)) {
				return;
			}

			// The "LogBoss" method requires many parameters, defined separately below:

			/******************** TEMPORAL GUARDIAN ***********************/
			string internalName = "TemporalGuardian";
			float weight = 19.1f; // https://github.com/JavidPack/BossChecklist/wiki/Boss-Progression-Values
			Func<bool> downed = () => DownedBossSystem.downedTemporalGuardian;
			int bossType = ModContent.NPCType<GuardianOfTheRiftBody>();

			// The item used to summon the boss with (if available)
			// int spawnItem = ModContent.ItemType<Content.Items.Consumables.MinionBossSummonItem>();

			// "collectibles" like relic, trophy, mask, pet
			List<int> collectibles = new List<int>()
			{
				ModContent.ItemType<TemporalGuardianRelic>(),
				ModContent.ItemType<EnchantedSunStone>(),
				ModContent.ItemType<PowerHammer>(),
                ModContent.ItemType<DragonsBreath>(),
                ModContent.ItemType<SquidOfTheAbyssScepter>(),
                ModContent.ItemType<SoliumBar>(),
                ModContent.ItemType<TemporalGuardianTreasureBag>(),
			};

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) => {
				Texture2D texture = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardianBase_v4").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				sb.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>() {
					/*["spawnItems"] = spawnItem,*/
					["collectibles"] = collectibles,
					["customPortrait"] = customPortrait
					// Other optional arguments as needed are inferred from the wiki
				}
			);

			// Other bosses or additional Mod.Call can be made here.
		}
	}
}