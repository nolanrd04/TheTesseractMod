using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.CodeAnalysis;

namespace TheTesseractMod.Projectiles.Ranged.EtherealBlaster
{
    internal class GlowRiftProjectileFriendly : ModProjectile
    {
        private float rotation = 15; // in degrees
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SporeCloud);
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 125;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 1f, 1f);
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = MathHelper.ToRadians(Main.rand.Next(360));
            }
            Projectile.velocity *= 0.98f;

            Projectile.rotation += MathHelper.ToRadians(rotation);
            rotation *= 0.95f;

            if (Projectile.ai[0] > 90)
            {
                Projectile.alpha += 4;
            }
            Projectile.ai[0]++;
        }

        public float Lerp(float x, float y, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return x + amount * (y - x);
        }
    }
}