using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Enemy;


namespace TheTesseractMod.NPCs.Bosses.GuardianOfTheRift
{
    public class MiniGuardian : ModNPC
    {
        public override string Texture => "TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardianBase_v4";

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 40000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1; // Custom AI
            NPC.dontTakeDamage = true; // Start invulnerable, will become vulnerable after a short time in AI
            NPC.active = true;
        }

        public override void AI()
        {
            NPC.ai[0]++; // Increment timer
            if (NPC.ai[0] >= 120 && Main.netMode != NetmodeID.MultiplayerClient) // After 2 seconds, despawn if in multiplayer (prevents lingering minions if something goes wrong)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LightRiftProjectile>(), 80, 0f);
                NPC.ai[0] = 0; // Reset timer to prevent multiple projectiles from spawning
            }
            if (NPC.ai[0] < 60) // First 60 frames, fade out 
            {
                NPC.alpha = (int)(255 * (1f - NPC.ai[0] / 60f)); // Fade in over 60 frames
            }
            else if (NPC.ai[0] > 75) // After 75 frames, fade back in
            {
                NPC.alpha = (int)(255 * ((NPC.ai[0] - 75f) / 45f)); // Fade out over 45 frames
            }
            NPC.netUpdate = true;

            // scale factor
            NPC.ai[1] = 1f + (float)Math.Sin(NPC.ai[0] * 0.2f) * 0.5f;
            if (NPC.ai[0] > 10)
            {
                NPC.dontTakeDamage = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 drawPos = new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f, NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f);
            var textureRequest = ModContent.Request<Texture2D>(Texture);
            // draw background 
            
            Main.EntitySpriteDraw(textureRequest.Value,
            drawPos,
            new Rectangle(0, 0, textureRequest.Value.Width, textureRequest.Value.Height), NPC.GetAlpha(Color.White) * 0.5f, 
            NPC.rotation, new Vector2(textureRequest.Value.Width * 0.5f, textureRequest.Value.Height * 0.5f), NPC.scale * NPC.ai[1], effects, 0);

            // draw main texture
            Main.EntitySpriteDraw(textureRequest.Value,
            drawPos,
            new Rectangle(0, 0, textureRequest.Value.Width, textureRequest.Value.Height), NPC.GetAlpha(Color.White), 
            NPC.rotation, new Vector2(textureRequest.Value.Width * 0.5f, textureRequest.Value.Height * 0.5f), NPC.scale, effects, 0);

            return false;
        }
    }
    
}