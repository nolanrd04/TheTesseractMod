using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.NPCs.Bosses.GuardianOfTheRift
{
    internal class DustBoulderMinion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 16;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.lifeMax = 9000;
            NPC.HitSound = SoundID.Item127;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f; // No knockback so position isn't disrupted
            NPC.scale = 3f;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                NPC.lifeMax = 5000;
            }
            NPC.dontTakeDamage = true; // Start invulnerable, will become vulnerable after a short time in AI
            NPC.active = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[1] = Main.rand.NextFloat(0.05f, 0.1f); // Random rotation speed for visual variety
        }

        public override void AI()
        {
            NPC.rotation += NPC.ai[1];
            // Emit dust particles for visual effect
            if (Main.rand.NextBool(3))
            {
                // Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DustCloud>(), 0, 0, Main.rand.Next(50), Color.Orange, 0.3f);
            }

            if (NPC.ai[0] > 10)
            {
                NPC.dontTakeDamage = false;
            }
            else
            {
                NPC.ai[0]++;
            }
        }
    }
}