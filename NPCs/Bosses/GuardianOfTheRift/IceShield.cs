using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs;

namespace TheTesseractMod.NPCs.Bosses.GuardianOfTheRift
{
    internal class IceShield : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 109;
            NPC.height = 109;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 60000;
            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f; // No knockback so position isn't disrupted
            NPC.scale = 3f;
            NPC.alpha = 255;
            NPC.dontTakeDamage = true; // Start invulnerable, will become vulnerable after a short time in AI
            NPC.active = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Microsoft.Xna.Framework.Color drawColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Microsoft.Xna.Framework.Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture,
                new Vector2(NPC.position.X - screenPos.X + NPC.width * 0.5f, NPC.position.Y - screenPos.Y + NPC.height * 0.5f),
                null, NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), NPC.scale, SpriteEffects.None, 0f);
        }


        public override void AI()
        {
            if (NPC.ai[0] >= 600)
            {
                int projCount = Math.Max(Math.Min(36, NPC.life / 2000 ), 6);
                for (int i = 0; i < projCount; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float angle = (2f * (float)Math.PI / projCount) * i + Main.rand.NextFloat(-0.1f, 0.1f);
                        Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 25f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<COLD_IceShieldSpike>(), 170, 0f);
                    }
                }

                NPC.life = 0; // Kill the NPC after spawning projectiles
                NPC.netUpdate = true;
                return;
            }

            if (NPC.ai[0] > 10)
            {
                NPC.dontTakeDamage = false;
            }

            NPC.ai[0]++;
        }

        public override void OnKill()
        {
            SoundEngine.PlaySound(SoundID.Item27, NPC.position);
            // Spawn explosion effect
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, velocity.X, velocity.Y, 150, default(Color), 1.5f);
            }
        }
    }
}

