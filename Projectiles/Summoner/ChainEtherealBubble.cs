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
    internal class ChainEtherealBubble : ModProjectile
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
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;
            Projectile.scale = 0.7f;
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

            NPC target = Main.npc[findSecondClosestTarget()];
            float distance = Vector2.Distance(Projectile.Center, target.Center);

            if (target.CanBeChasedBy() && !target.friendly && target.active && IsTargetValid(target) && distance < 300f)
            {
                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 13f;
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

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC && npc != Main.npc[findTarget()])
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
