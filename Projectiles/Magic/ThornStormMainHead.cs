using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Magic
{
    internal class ThornStormMainHead : ModProjectile
    {
        private const int TRAIL_LENGTH = 15; // Number of segments in the trail
        private const int FRAMES_PER_SEGMENT = 3; // Capture every N frames for consistent spacing
        private float fixedRotation = 0f;
        private int frameCounter = 0;
        private List<Vector2> trailPositions = new List<Vector2>();
        private List<float> trailRotations = new List<float>();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TRAIL_LENGTH;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 210;
            Projectile.alpha = 20;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Main head movement logic
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Capture position every N frames while moving
            if (Projectile.ai[0] < 80)
            {
                if(Main.rand.NextBool(25))
                {
                    float rotation = 0f;
                    if (Main.rand.NextBool())
                    {
                        rotation = Main.rand.Next(12, 20);
                    }
                    else
                    {
                        rotation = Main.rand.Next(-20, -12);
                    }
                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation)) * 0.5f, ModContent.ProjectileType<ThornStormMini>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                if (frameCounter >= FRAMES_PER_SEGMENT)
                {
                    trailPositions.Add(Projectile.position);
                    trailRotations.Add(Projectile.rotation);
                    frameCounter = 0;
                }
                frameCounter++;
            }

            if (Projectile.ai[0] == 80)
            {
                fixedRotation = Projectile.rotation;
            }
            if (Projectile.ai[0] >= 80)
            {
                Projectile.rotation = fixedRotation;
                Projectile.velocity = Vector2.Zero;
                Projectile.alpha +=2;
            }
            Projectile.ai[0]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Check main head collision
            if (projHitbox.Intersects(targetHitbox))
                return true;

            // Check trail segment collisions
            for (int i = 0; i < trailPositions.Count; i++)
            {
                Vector2 segmentCenter = trailPositions[i] + Projectile.Size * 0.5f;
                Rectangle segmentHitbox = new Rectangle((int)(segmentCenter.X - Projectile.width / 2), (int)(segmentCenter.Y - Projectile.height / 2), Projectile.width, Projectile.height);

                if (segmentHitbox.Intersects(targetHitbox))
                    return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Magic/ThornStormMain");
            Vector2 drawOrigin = texture.Size() * 0.5f;

            // Draw trail from stored positions
            for (int i = 0; i < trailPositions.Count; i++)
            {
                Vector2 currentPos = trailPositions[i] + Projectile.Size * 0.5f;
                Vector2 drawPos = currentPos - Main.screenPosition;
                float rotation = i < trailRotations.Count ? trailRotations[i] : Projectile.rotation;

                // Fade out older segments
                float alpha = i / (float)(trailPositions.Count + 1);
                Color color = lightColor * alpha;

                Lighting.AddLight(currentPos, 90f/255f, 255f/255f, 48f/255f);

                // Spawn dust along the trail
                if (Main.rand.NextBool(12))
                {
                    int dust = Dust.NewDust(trailPositions[i], Projectile.width, Projectile.height, DustID.JungleTorch, 0, 0, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                }

                Main.EntitySpriteDraw(texture.Value, drawPos, null, Projectile.GetAlpha(Color.White * alpha), rotation, drawOrigin, 1.03f, SpriteEffects.None, 0f);
            }

            // Draw the main head
            Texture2D mainTexture = TextureAssets.Projectile[Type].Value;
            Vector2 headDrawPos = Projectile.Center - Main.screenPosition;
            Lighting.AddLight(headDrawPos, 90f/255f * Projectile.alpha / 255f, 255f/255f * Projectile.alpha / 255f, 48f/255f * Projectile.alpha / 255f);
            Main.EntitySpriteDraw(mainTexture, headDrawPos, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, 1.03f, SpriteEffects.None, 0f);

            return false; // Don't draw default sprite
        }
    }
}