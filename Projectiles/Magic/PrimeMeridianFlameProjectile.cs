using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Magic
{
    internal class PrimeMeridianFlameProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.velocity = new Vector2(2f, 2f);
            AIType = ProjectileID.ShadowFlame;
            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 170;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        float floatScale = 1.2f;

        public override void AI()
        {
            Projectile.ai[1]++;
            floatScale *= 0.98f;

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 74, Projectile.velocity.X *= 0.985f, Projectile.velocity.Y *= 0.985f, 150, default(Color), floatScale);
        }
    }
}