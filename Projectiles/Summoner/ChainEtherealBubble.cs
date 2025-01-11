using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.Summoner
{
    internal class ChainEtherealBubble : ModProjectile
    {
        private NPC original = null;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;
            Projectile.scale = 0.7f;
            Projectile.alpha = 75;
        }

        public override void OnSpawn(IEntitySource source)
        {
            original = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 1f, 1f, 1f);
            if (Projectile.ai[0] % 2 == 0)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 204, Projectile.velocity.X, Projectile.velocity.Y, 150, default(Color), 1f);
            }
            if (Projectile.ai[0] > 5)
            {
                Projectile.friendly = true;
            }

            Projectile.ai[0]++;
            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 13f;
            //************************************//

            NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center, original);

            if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, 300f))
            {
                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 13f;
                return;
            }
        }
    }
}
