using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Drawing;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsArrowProj : ModProjectile
    {
        /* vertex strip creation attempt 1 */
        const int textureHeight = 14;
        float incrementalAngle = (float)Math.PI / 2;
        private List<float> oldRotations = new List<float>(); // Store past rotations
        private List<Vector2> oldPositions = new List<Vector2>(); // Store past positions

        private VertexStrip strip = new VertexStrip();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 0;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            // --- Spawn fluctuating dust --- //
            float sineFactor = (float)(Math.Sin(incrementalAngle) * textureHeight / 2); // Adjust 0.1f to change fluctuation speed
            incrementalAngle += 0.1f;
            float rotation = Projectile.velocity.ToRotation();

            // Compute the offset for the top and bottom spawn points based on rotation
            Vector2 topOffset = new Vector2(0, -sineFactor).RotatedBy(rotation);
            Vector2 bottomOffset = new Vector2(0, sineFactor).RotatedBy(rotation);

            // Compute the actual spawn positions
            Vector2 topSpawnPosition = Projectile.Center + topOffset;
            Vector2 bottomSpawnPosition = Projectile.Center + bottomOffset;

            Dust topDust = Dust.NewDustPerfect(topSpawnPosition, DustID.Shadowflame, Vector2.Zero, 0, default(Color), 1.5f);
            topDust.noGravity = true;
            Dust bottomDust = Dust.NewDustPerfect(bottomSpawnPosition, DustID.Shadowflame, Vector2.Zero, 0, default(Color), 1.5f);
            bottomDust.noGravity = true;
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Vector2.Zero, 0, default(Color), 1f);
            dust.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 velocity = new Vector2(3f, 0);
            float angle = 0f;
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, velocity.RotatedBy(MathHelper.ToRadians(angle)), 0);
                angle += 36f;
            }

            target.AddBuff(BuffID.ShadowFlame, 90);

            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.NightsEdge, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["RainbowRod"].Apply();

            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => new Color(105, 13, 224, 0) * (1f - progress),
                progress => MathHelper.Lerp(15f, 8f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            // DrawTrail();
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(lightColor),
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            /*
            VertexStrip strip = new VertexStrip();
            strip.PrepareStrip(
                oldPos,
                oldRot,
                StripColor,
                StripWidth,
                -Main.screenPosition,
                oldPos.Length,
                true
            );

            strip.DrawTrail();
            */
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
            Vector2 velocity = Projectile.velocity/2;
            for (int i = 0; i < 10; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(36));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, velocity.X, velocity.Y, 0, default(Color), 1f);
            }

            return true;
        }

        private void DrawTrail()
        {
            if (oldPositions.Count < 2)
                return; // No trail to draw if fewer than 2 points

            VertexStrip vertexStrip = new VertexStrip();

            // Use a color gradient for the trail
            Color TrailColorFunction(float progress)
            {
                return Color.Lerp(Color.Blue, Color.Transparent, progress); // Fades out over distance
            }

            // Adjust trail width
            float TrailWidthFunction(float progress)
            {
                return MathHelper.Lerp(24f, 4f, progress); // Starts wide, then narrows
            }

            // Convert lists to arrays for Vertex Strip
            Vector2[] positionsArray = oldPositions.ToArray();
            float[] rotationsArray = oldRotations.ToArray(); // Use stored rotations

            /*// Draw the vertex strip
            vertexStrip.PrepareStrip(
                positionsArray,
                rotationsArray, // Now we pass the correct rotations!
                TrailColorFunction,
                TrailWidthFunction,
                -Main.screenPosition, // Offset for world position
                oldPositions.Count,
                true // Include backsides
            );

            vertexStrip.DrawTrail();*/
        }

    }

    
}
