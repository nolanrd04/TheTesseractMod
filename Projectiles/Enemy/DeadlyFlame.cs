using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Enemy
{
    internal class DeadlyFlame : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Flames);
            Projectile.height = 32;
            Projectile.width = 32;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, 16, 16, DustID.Torch, 0, 0, 0, default(Color), 2f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
