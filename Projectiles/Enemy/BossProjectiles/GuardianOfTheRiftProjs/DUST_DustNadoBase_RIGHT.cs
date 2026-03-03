using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    internal class DUST_DustNadoBase_RIGHT : ModProjectile // This will be a dust cloud that moves to the right of the boss. When it gets to a certain position it will spawn a dust tornado.
        // dust tornado will be a stationary invisible projectile that is compensated visually with dust effects.
    {
        private float travelingSpeed;
        public override string Texture => "TheTesseractMod/Textures/empty";
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
        }
        public override void OnSpawn(IEntitySource source)
        {
            travelingSpeed = Projectile.velocity.Length();
        }
        public override void AI()
        {
            if(Main.rand.Next(1) == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DustCloud>(), 0, 0, Main.rand.Next(50), Color.Orange, 1f);
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(50), default(Color), 3f);
                Main.dust[dust].noGravity = true;
            }

            Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];
            Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * travelingSpeed;
        }
    }
}
