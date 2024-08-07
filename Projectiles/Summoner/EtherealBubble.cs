using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Summoner
{
    internal class EtherealBubble : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.4f;
            Projectile.scale = 1f;
            Projectile.alpha = 75;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 1f, 1f, 1f);
            if (Projectile.ai[0] % 2 == 0)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 204, Projectile.velocity.X, Projectile.velocity.Y, 150, default(Color), 1f);
            }
            Projectile.ai[0]++;
            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 13f;
            //************************************//

            NPC target = Main.npc[findTarget()];

            if (target.CanBeChasedBy() && !target.friendly && target.active && IsTargetValid(target))
            {
                /*homing segment*/
                float goToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                float goToY = target.position.Y + (float)target.width * 0.5f - Projectile.Center.Y;
                float distance = (float)Math.Sqrt(goToX * goToX + goToY * goToY);

                if (distance < 600)
                {
                    distance = 4f / distance;
                    goToX *= distance;
                    goToY *= distance;

                    Projectile.velocity.X += goToX / 2; // higher int values make it turn slower
                    Projectile.velocity.Y += goToY / 2;
                }
            }
        }
        public int findTarget() // returns the closest npc
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC)
                {
                    float distance = Vector2.Distance(Projectile.position, npc.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }
            }
            return closestNPCIndex;
        }
        public int findSecondClosestTarget() // returns the closest npc
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;
            NPC closestTarget = Main.npc[findTarget()];

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC && npc != closestTarget)
                {
                    float distance = Vector2.Distance(Projectile.position, npc.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }
            }
            return closestNPCIndex;
        }

        private bool IsTargetValid(NPC target) // a check to make sure the target exists
        {
            return target != null && target.active && !target.friendly;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // spawn light dust storm
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LightDustStorm>(), Projectile.damage, 0);

            // do chain damage if in range
            NPC newTarget = Main.npc[findSecondClosestTarget()];

            if (newTarget.CanBeChasedBy() && IsTargetValid(newTarget))
            {
                /*homing segment*/
                float X = newTarget.position.X + (float)newTarget.width * 0.5f - Projectile.Center.X;
                float Y = newTarget.position.Y + (float)newTarget.width * 0.5f - Projectile.Center.Y;
                float distance = (float)Math.Sqrt(X * X + Y * Y);

                if (distance < 400)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<ChainEtherealBubble>(), Projectile.damage, 0);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(120)), ModContent.ProjectileType<ChainEtherealBubble>(), Projectile.damage, 0);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(240)), ModContent.ProjectileType<ChainEtherealBubble>(), Projectile.damage, 0);
                }
            }
        }

    }
}
