using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.NPCs.CustomGore;

namespace TheTesseractMod.NPCs.Bosses.GuardianOfTheRift
{
    internal class LifeMinion : ModNPC
    {
        private float glowPulseTimer;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.damage = 100;
            NPC.defense = 0;
            NPC.lifeMax = 12000;
            NPC.HitSound = SoundID.NPCHit44;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;
            NPC.scale = 2f;
            NPC.active = true;
        }

        public override void AI()
        {
            // On first tick, randomize wobble phase so each minion moves differently
            if (NPC.ai[0] == 0)
            {
                NPC.ai[1] = Main.rand.NextFloat(MathHelper.TwoPi); // random wobble phase offset
                NPC.ai[2] = Main.rand.NextFloat(0.1f, 0.2f); // random wobble speed
                NPC.ai[3] = Main.rand.NextFloat(2f, 4f); // random wobble amplitude
            }

            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];

            // Butterfly-like movement
            Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            float speed = 12f;
            float inertia = 20f;

            NPC.velocity = (NPC.velocity * (inertia - 1f) + direction * speed) / inertia;

            // Wobble with per-minion randomized phase, speed, and amplitude
            float wobble = (float)Math.Sin(NPC.ai[0] * NPC.ai[2] + NPC.ai[1]) * NPC.ai[3];
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
            NPC.velocity += perpendicular * wobble * 0.1f;

            // Repel away from nearby siblings to prevent clumping
            int minionType = ModContent.NPCType<LifeMinion>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];
                if (other.whoAmI != NPC.whoAmI && other.active && other.type == minionType)
                {
                    float dist = Vector2.Distance(NPC.Center, other.Center);
                    if (dist < 150f && dist > 0f)
                    {
                        Vector2 pushAway = (NPC.Center - other.Center).SafeNormalize(Vector2.UnitX);
                        float pushStrength = (150f - dist) / 150f * 3f;
                        NPC.velocity += pushAway * pushStrength;
                    }
                }
            }

            NPC.ai[0]++;

            // Face movement direction
            NPC.spriteDirection = NPC.velocity.X > 0 ? -1 : 1;
            NPC.rotation = NPC.velocity.X * 0.04f;

            if (NPC.ai[0] % 3 == 0)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PinkTorch, 0f, 0f, 0, default(Color), 2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            glowPulseTimer++;

            Texture2D texture = TextureAssets.Npc[Type].Value;
            int frameHeight = texture.Height / Main.npcFrameCount[Type];
            Rectangle sourceRect = new Rectangle(0, NPC.frame.Y, texture.Width, frameHeight);
            Vector2 origin = sourceRect.Size() / 2f;
            Vector2 drawPos = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY);

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            
            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Textures/SharpCenterOutwardRadialGlow");
            spriteBatch.Draw(glowTexture.Value,
                drawPos + new Vector2(0f, 20f), // Slight vertical offset for better visual alignment
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                new Color(255, 105, 180, 0) * (1f - NPC.alpha / 255f),
                0f,
                glowTexture.Size() * 0.5f,
                0.35f,
                SpriteEffects.None,
                0f);

            
            float fade = (float)Math.Sin(glowPulseTimer * MathHelper.TwoPi / 60f);
            fade = (fade + 1f) / 2f; // Normalize to 0-1
            float ghostScale = MathHelper.Lerp(1.1f, 1.3f, fade) * NPC.scale;

            spriteBatch.Draw(texture,
                drawPos,
                sourceRect,
                Color.White * 0.5f,
                NPC.rotation,
                origin,
                ghostScale,
                effects,
                0f);

            
            spriteBatch.Draw(texture,
                drawPos,
                sourceRect,
                Color.White,
                NPC.rotation,
                origin,
                NPC.scale,
                effects,
                0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.LifeMinionGoreHead, 0.5f, 2f);
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.LifeMinionGoreTail, 0.5f, 2f);
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.LifeMinionGoreThorax, 0.3f, 2f);
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.LifeMinionGoreWing, 0.3f, 2f);
            }
        }
    }
}