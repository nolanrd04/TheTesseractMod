using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.Enemy
{
    internal class DeadlyBubbleHostile:ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.damage = 100;
            Projectile.alpha = 0;
            Projectile.timeLeft = 60;
            Projectile.light = 0.9f;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 20;
            Projectile.height = 20;
        }
        public float Lerp(float x, float y, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return x + amount * (y - x);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 1f, 1f, 1f);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(223, 194, 255, 0) * (1f - Projectile.alpha / 255f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Random rand = new Random();
                float rotation = (float)(rand.NextDouble() * 360);
                Vector2 velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 34, velocity.X, velocity.Y, 0, default(Color), 1.2f);
                //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RiftLightBlueDust>(), velocity.X, velocity.Y, 0, default(Color), 1f);
            }
        }
    }
}
