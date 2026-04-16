using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    internal class DEATH_SinisterSkull : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 17;
            Projectile.height = 17;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 600;
            Projectile.scale = 4f;
        }

        public override void AI()
        {
            // Face direction of movement but stay upright (never upside down)
            float angle = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.velocity.X >= 0 ? 1 : -1;
            // If moving left, flip the angle so the sprite stays upright
            if (Projectile.spriteDirection == -1)
                angle += MathHelper.Pi;
            Projectile.rotation = angle;
            Dust.NewDust(Projectile.Center, 0, 0, DustID.WhiteTorch, -Projectile.velocity.X * 0.5f, -Projectile.velocity.Y * 0.5f, 0, default(Color), 1.2f);

            // Animate between 2 frames
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6) // switch frame every 6 ticks
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            Rectangle sourceRect = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 origin = sourceRect.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}