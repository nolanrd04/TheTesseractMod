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
using Terraria.DataStructures;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.Magic.EtherealTomeProjectiles
{
    internal class FriendlyThunderbolt:ModProjectile
    {
        private int ConsecutiveNegative = 0;
        private int ConsecutivePositive = 0;

        NPC lastHit = null;
        private Random rand = new Random();
        
        public override void SetDefaults()
        {
            Projectile.damage = 100;
            Projectile.alpha = 0;
            Projectile.timeLeft = 120;
            Projectile.light = 0.9f;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.ai[0]++;

            
            if (Projectile.ai[0] % 10 == 0)
            {
                float rotation = MathHelper.ToRadians((float)(rand.NextDouble() * 50 - 25));
                NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center, lastHit);

                if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, 250f))
                {
                    Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 15f;
                    Projectile.velocity = Projectile.velocity.RotatedBy(rotation);
                    return;
                }
                if (ConsecutiveNegative == 2)
                {
                    rotation = MathHelper.ToRadians(30);
                    ConsecutiveNegative = 0;
                }
                if (ConsecutivePositive == 2)
                {
                    rotation = MathHelper.ToRadians(-30);
                    ConsecutivePositive = 0;
                }

                if (rotation > 0 && ConsecutiveNegative == 0)
                {
                    ConsecutivePositive++;
                }
                else
                {
                    ConsecutivePositive = 0;
                }

                if (rotation < 0 && ConsecutivePositive == 0)
                {
                    ConsecutiveNegative++;
                }
                else
                {
                    ConsecutiveNegative = 0;
                }

                Projectile.velocity = Projectile.velocity.RotatedBy(rotation);
            }

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.Center, 15, 15, ModContent.DustType<ElectricDust>(), 0, 0, 0, Color.Blue, .7f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            lastHit = target;
        }
    }
}
