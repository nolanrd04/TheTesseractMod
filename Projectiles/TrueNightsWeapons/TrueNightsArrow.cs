using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Drawing;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.TrueNightsWeapons
{
    internal class TrueNightsArrow : ModProjectile
    {
        const int textureHeight = 14;
        float incrementalAngle = (float)Math.PI / 2;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 0;
            Projectile.alpha = 75;
            Projectile.scale = 1.3f;
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

            Dust topDust = Dust.NewDustPerfect(topSpawnPosition, DustID.CursedTorch, Vector2.Zero, 0, default(Color), 1.75f);
            topDust.noGravity = true;
            Dust bottomDust = Dust.NewDustPerfect(bottomSpawnPosition, DustID.CursedTorch, Vector2.Zero, 0, default(Color), 1.75f);
            bottomDust.noGravity = true;
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Terra, Vector2.Zero, 0, default(Color), 1f);
            dust.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 velocity = new Vector2(3f, 0);
            float angle = 0f;
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, velocity.RotatedBy(MathHelper.ToRadians(angle)), 0);
                dust.noGravity = true;
                angle += 36f;
            }

            target.AddBuff(BuffID.CursedInferno, 120);

            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
        }

        public override bool PreDraw(ref Color lightColor)
        {
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
            Vector2 velocity = Projectile.velocity/2;
            for (int i = 0; i < 10; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(36));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Terra, velocity.X, velocity.Y, 0, default(Color), 1f);
            }

            return true;
        }
    }
}
