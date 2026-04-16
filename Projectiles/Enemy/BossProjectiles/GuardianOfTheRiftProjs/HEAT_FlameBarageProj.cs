using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria.Graphics;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    internal class HEAT_FlameBarageProj : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
        }

        

        public override void AI()
        {

            // FinalFractalHelper.FinalFractalProfile finalFractalProfile = FinalFractalHelper.GetFinalFractalProfile((int)ai[1]);
            Vector2 newPos = Projectile.position + new Vector2(Projectile.width / 2, Projectile.height / 2);
            if (Projectile.ai[0] < 95)
            {
                for (int i = 0; i < 1; i++)
                {
                    Dust.NewDust(newPos, 2, 2, ModContent.DustType<RadialGlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.OrangeRed, 1f);
                }
                for (int i = 0; i < 1; i++)
                {
                    Dust.NewDust(newPos, 2, 2, ModContent.DustType<RadialGlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.Red, 1f);
                    
                }
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.RedTorch, 0, 0, 0, default(Color), 3f);
                int dust_torch = Dust.NewDust(Projectile.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust_torch].noGravity = true;
                
            }

            // Home towards nearest player
            Player closestPlayer = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    float distance = Vector2.Distance(Projectile.Center, player.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = player;
                    }
                }
            }

            if (closestPlayer != null && Projectile.ai[0] < 65) // Start homing after 10 frames to allow initial movement
            {
                // Calculate direction to player
                Vector2 directionToPlayer = (closestPlayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Vector2 currentDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);

                // Maximum rotation per frame (in radians) - adjust this value to control turn rate
                float maxTurnAngle = 0.015f;

                // Calculate angle between current direction and target direction
                float angleToTarget = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X) - (float)Math.Atan2(currentDirection.Y, currentDirection.X);

                // Normalize angle to -PI to PI range
                while (angleToTarget > MathHelper.Pi) angleToTarget -= MathHelper.TwoPi;
                while (angleToTarget < -MathHelper.Pi) angleToTarget += MathHelper.TwoPi;

                // Clamp the turn angle to max turn rate
                float clampedAngle = MathHelper.Clamp(angleToTarget, -maxTurnAngle, maxTurnAngle);

                // Apply rotation to velocity
                float newAngle = (float)Math.Atan2(currentDirection.Y, currentDirection.X) + clampedAngle;
                float speed = Projectile.velocity.Length();
                Projectile.velocity = new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle)) * speed;
            }

            Projectile.ai[0]++;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
