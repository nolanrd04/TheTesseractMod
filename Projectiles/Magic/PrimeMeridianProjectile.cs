using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Magic
{
    internal class PrimeMeridianProjectile : ModProjectile
    {
        int counterForNoHoming = 7;
        bool rehome = false;
        NPC lastHit = null; // the npc the projectile just made contact with
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 200;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0.8f;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 4;
                Projectile.penetrate = 8;
            }
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 0.3f, 0.94f, 0.48f);
            if (Projectile.ai[0] % 3 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, Projectile.velocity.X, Projectile.velocity.Y, 115, default(Color), 1f);
            }
            
            Projectile.ai[0]++;
            Projectile.rotation += 0.4f * (float)Projectile.direction;
            if (counterForNoHoming > 0)
            {
                counterForNoHoming--;
            }
            if (counterForNoHoming <= 0)
            {
                lastHit = null;
            }

            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20f;
            //************************************//

            if (Projectile.ai[0] > 10)
            {
                NPC target = Main.npc[findTarget()];

                if (target.CanBeChasedBy() && !target.friendly && target.active && IsTargetValid(target))
                {
                    /*homing segment*/
                    float goToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                    float goToY = target.position.Y + (float)target.width * 0.5f - Projectile.Center.Y;
                    float distance = (float)Math.Sqrt(goToX * goToX + goToY * goToY);

                    if (distance < 2000 && distance > 0)
                    {
                        distance = 4f / distance;
                        goToX *= distance;
                        goToY *= distance;

                        Projectile.velocity.X += goToX / 0.6f; // higher int values make it turn slower
                        Projectile.velocity.Y += goToY / 0.6f;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            lastHit = target;
            counterForNoHoming = 7;
        }
        public int findTarget() // returns the closest npc
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC && npc != lastHit)
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
    }
}
