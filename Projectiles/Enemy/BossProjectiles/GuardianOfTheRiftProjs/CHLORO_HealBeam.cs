using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.GlobalFuncitons;
using TheTesseractMod.NPCs.Bosses.GuardianOfTheRift;

namespace TheTesseract.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    public class CHLORO_HealBeam : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60000;
            Projectile.extraUpdates = 1;
            Projectile.damage = 0;
        }

        public override void AI()
        {       
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.JungleTorch, 0f, 0f, 0, default(Color), 1.5f);
                dust.noGravity = true;
                dust.scale = 1.5f;
                dust.velocity *= 0.5f;

            NPC boss = null;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type == ModContent.NPCType<GuardianOfTheRiftBody>() && Main.npc[i].active)
                {
                    boss = Main.npc[i];
                    break;
                }
            }

            // Home towards the boss
            if (boss != null)
            {
                Vector2 direction = (boss.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10f;
                Projectile.velocity = direction;

                if (Projectile.Colliding(Projectile.Hitbox, boss.Hitbox))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        
                        if (boss.life + Projectile.damage * 10 < boss.lifeMax)
                        {
                            boss.HealEffect(Projectile.damage * 10, true);
                            boss.life += Projectile.damage * 10;
                        }
                        boss.netUpdate = true;
                    }
                    Projectile.Kill();
                }
            }
        }
    }
}