using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.NPCs.Bosses.GuardianOfTheRift;

namespace TheTesseractMod.NPCs.CustomGore
{
    public class CustomGoreManager
    {
        public const string LifeMinionGoreHead = "TheTesseractMod/LifeMinionGore1";
        public const string LifeMinionGoreTail = "TheTesseractMod/LifeMinionGore2";
        public const string LifeMinionGoreThorax = "TheTesseractMod/LifeMinionGore3";
        public const string LifeMinionGoreWing = "TheTesseractMod/LifeMinionGore4";
        public const string TemporalGuardianBaseGore1 = "TheTesseractMod/TemporalGuardianBaseGore1";
        public const string TemporalGuardianBaseGore2 = "TheTesseractMod/TemporalGuardianBaseGore2";
        public const string TemporalGuardianBaseGore3 = "TheTesseractMod/TemporalGuardianBaseGore3";
        public const string TemporalGuardianBaseGore4 = "TheTesseractMod/TemporalGuardianBaseGore4";
        public const string TemporalGuardianBaseGore5 = "TheTesseractMod/TemporalGuardianBaseGore5";
        public const string TemporalGuardianSpikeGore1 = "TheTesseractMod/TemporalGuardianSpikeGore1";
        public const string TemporalGuardianSpikeGore2 = "TheTesseractMod/TemporalGuardianSpikeGore2";

        public static void SpawnCustomGore(NPC npc, Vector2 position, Vector2 velocity, string goreType, float spawnChance = 1f, float scale = 1f)
        {
            if (Main.rand.NextFloat() > spawnChance)
            {
                return;
            }
            int goreIdx = Terraria.Gore.NewGore(npc.GetSource_Death(), position, velocity, ModContent.Find<ModGore>(goreType).Type);
            Main.gore[goreIdx].scale = scale; // Adjust scale as needed
     }
    }
    public class LifeMinionGore1 : ModGore { }
    public class LifeMinionGore2 : ModGore { }
    public class LifeMinionGore3 : ModGore { }
    public class LifeMinionGore4 : ModGore { }
    public class TemporalGuardianBaseGore1 : ModGore { }
    public class TemporalGuardianBaseGore2 : ModGore { }
    public class TemporalGuardianBaseGore3 : ModGore { }
    public class TemporalGuardianBaseGore4 : ModGore { }
    public class TemporalGuardianBaseGore5 : ModGore { }
    public class TemporalGuardianSpikeGore1 : ModGore { }
    public class TemporalGuardianSpikeGore2 : ModGore { }
}