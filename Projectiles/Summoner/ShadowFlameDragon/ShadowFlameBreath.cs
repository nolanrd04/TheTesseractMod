using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.Summoner.ShadowFlameDragon
{
    internal class ShadowFlameBreath : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.extraUpdates = 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 90;
            Projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.Center, 5, 5,  ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 100, Color.Purple, 1.3f);
            Dust.NewDust(Projectile.Center, 5, 5, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 100, Color.Orange, 1f);
        }
    }
}
