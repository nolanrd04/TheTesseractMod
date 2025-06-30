using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsBulletShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 70;
            Projectile.timeLeft = 20;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 5)
            {
                Projectile.tileCollide = true;
                Projectile.friendly = true;
            }

            Projectile.rotation += 0.9f;
            Lighting.AddLight(Projectile.position, 250f / 255, 135f / 255, 250f / 255);
            Projectile.velocity *= 0.95f;
            Projectile.ai[0]++;
        }
    }
}
