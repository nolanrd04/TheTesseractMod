using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Systems;

namespace TheTesseractMod.Global.NPCs
{
	public class TemporalOreGlobalNPC : GlobalNPC
	{
		// Vanilla main bosses only — excludes event/mini-bosses like Betsy, Mourning Wood, etc.
		private static readonly HashSet<int> VanillaMainBossIDs = new HashSet<int> {
			NPCID.KingSlime,
			NPCID.EyeofCthulhu,
			NPCID.EaterofWorldsHead,
			NPCID.BrainofCthulhu,
			NPCID.QueenBee,
			NPCID.SkeletronHead,
			NPCID.Deerclops,
			NPCID.WallofFlesh,
			NPCID.QueenSlimeBoss,
			NPCID.Retinazer,
			NPCID.Spazmatism,
			NPCID.TheDestroyer,
			NPCID.SkeletronPrime,
			NPCID.Plantera,
			NPCID.Golem,
			NPCID.HallowBoss, // Empress of Light
			NPCID.DukeFishron,
			NPCID.CultistBoss, // Lunatic Cultist
			NPCID.MoonLordCore,
		};

		private bool IsMainBoss(NPC npc) {
			if (!npc.boss) {
				return false;
			}

			// Vanilla: only allow known main bosses
			if (npc.ModNPC == null) {
				return VanillaMainBossIDs.Contains(npc.type);
			}

			// Modded: any modded NPC with boss = true counts as a main boss
			return true;
		}

		public override void OnKill(NPC npc) {
			if (DownedBossSystem.downedFirstBoss) {
				return;
			}

			if (!IsMainBoss(npc)) {
				return;
			}

			DownedBossSystem.downedFirstBoss = true;
			ModContent.GetInstance<TemporalOreSystem>().BlessWorldWithTemporalOre();

			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
		}
	}
}
