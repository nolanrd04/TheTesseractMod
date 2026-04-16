using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    public class AQUA_Aquanado : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Cthulunado);
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0)
            {
                Projectile.scale = Projectile.ai[1];
            }
            Lighting.AddLight(Projectile.Center, 0.1f, 0.8f, 0.8f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int scaledWidth = (int)(160 * Projectile.scale);
            int scaledHeight = (int)(42 * Projectile.scale);

            Rectangle scaledHitbox = new Rectangle(
                (int)(Projectile.Center.X - scaledWidth / 2f),
                (int)(Projectile.Center.Y - scaledHeight / 2f),
                scaledWidth,
                scaledHeight
            );

            return scaledHitbox.Intersects(targetHitbox);
        }

         public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];

            Main.EntitySpriteDraw(texture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, Projectile.frame * frameHeight, 160, frameHeight),
                lightColor, Projectile.rotation, new Vector2(80, 21), Projectile.scale, SpriteEffects.None, 0f);


            int frameSpeed = 4;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            
            return false;
        }

    }
}