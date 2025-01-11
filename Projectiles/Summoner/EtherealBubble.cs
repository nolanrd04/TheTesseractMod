using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using TheTesseractMod.GlobalFuncitons;

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
            Vector2 newPosition = Projectile.position + new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);
            if (Projectile.ai[0] % 2 == 0)
            {
                Dust.NewDust(newPosition, Projectile.width, Projectile.height, ModContent.DustType<SharpRadialGlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.Aqua, .6f);
                Dust.NewDust(newPosition, Projectile.width, Projectile.height, ModContent.DustType<SharpRadialGlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, new Color(255 / 255f, 246 / 255f, 150 / 255f), .6f);
            }
            Projectile.ai[0]++;
            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 13f;
            //************************************//

            NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);

            if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, 600f))
            {
                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 13f;
                return;
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
            NPC newTarget = GlobalProjectileFunctions.findSecondClosestTarget(Projectile.Center);

            if (GlobalProjectileFunctions.IsTargetValid(newTarget, Projectile.Center, 250))
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<ChainEtherealBubble>(), Projectile.damage, 0);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(120)), ModContent.ProjectileType<ChainEtherealBubble>(), Projectile.damage, 0);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(240)), ModContent.ProjectileType<ChainEtherealBubble>(), Projectile.damage, 0);
            }
        }

    }
}
