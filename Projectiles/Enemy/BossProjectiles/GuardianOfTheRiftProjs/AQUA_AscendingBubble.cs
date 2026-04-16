using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    public class AQUA_AscendingBubble : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            Projectile.velocity.Y -= 0.15f;

            float wobbleAmplitude = 1.5f;
            float wobbleSpeed = 0.08f;
            Projectile.velocity.X += (float)System.Math.Sin(Projectile.ai[0] * wobbleSpeed) * wobbleAmplitude * 0.1f;

            if (Projectile.velocity.Y < -8f)
                Projectile.velocity.Y = -8f;

            
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 4 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.UltraBrightTorch, 0f, 0f, 0, default(Color), 0.8f);
            }

            Lighting.AddLight(Projectile.Center, 0.1f, 0.8f, 0.8f);
        }
    }
}