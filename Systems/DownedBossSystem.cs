using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TheTesseractMod.Systems
{
    internal class DownedBossSystem : ModSystem
    {
        public static bool downedTemporalGuardian = false;
		public static bool downedFirstBoss = false;
		// public static bool downedOtherBoss = false;

		public override void ClearWorld() {
			downedTemporalGuardian = false;
			downedFirstBoss = false;
			// downedOtherBoss = false;
		}

		// We save our data sets using TagCompounds.
		// NOTE: The tag instance provided here is always empty by default.
		public override void SaveWorldData(TagCompound tag) {
			if (downedTemporalGuardian) {
				tag["downedTemporalGuardian"] = true;
			}
			if (downedFirstBoss) {
				tag["downedFirstBoss"] = true;
			}

			// if (downedOtherBoss) {
			//	tag["downedOtherBoss"] = true;
			// }
		}

		public override void LoadWorldData(TagCompound tag) {
			downedTemporalGuardian = tag.ContainsKey("downedTemporalGuardian");
			downedFirstBoss = tag.ContainsKey("downedFirstBoss");
			// downedOtherBoss = tag.ContainsKey("downedOtherBoss");
		}

		public override void NetSend(BinaryWriter writer) {
			// Order of parameters is important and has to match that of NetReceive
			writer.WriteFlags(downedTemporalGuardian, downedFirstBoss/*, downedOtherBoss*/);
			// WriteFlags supports up to 8 entries, if you have more than 8 flags to sync, call WriteFlags again.

			// If you need to send a large number of flags, such as a flag per item type or something similar, BitArray can be used to efficiently send them. See Utils.SendBitArray documentation.
		}

		public override void NetReceive(BinaryReader reader) {
			// Order of parameters is important and has to match that of NetSend
			reader.ReadFlags(out downedTemporalGuardian, out downedFirstBoss/*, out downedOtherBoss*/);
			// ReadFlags supports up to 8 entries, if you have more than 8 flags to sync, call ReadFlags again.
		}
    }
}