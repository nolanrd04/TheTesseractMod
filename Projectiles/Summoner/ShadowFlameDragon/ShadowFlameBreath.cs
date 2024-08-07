using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Summoner.ShadowFlameDragon
{
    internal class ShadowFlameBreath : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.extraUpdates = 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.Center, 10, 10, DustID.Shadowflame);
            Dust.NewDust(Projectile.Center, 10, 10, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 2f);
        }
    }
}
