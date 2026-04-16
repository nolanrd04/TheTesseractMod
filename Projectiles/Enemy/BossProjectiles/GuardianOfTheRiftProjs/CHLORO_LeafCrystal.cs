using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    public class CHLORO_LeafCrystal : ModProjectile
    {
        private VertexStrip strip = new VertexStrip();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 0;
            Projectile.scale = 2f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] % 2 == 0)
            {
                Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, Color.LimeGreen, 1.5f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
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

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => Color.Lime * (1f - progress),
                progress => MathHelper.Lerp(15f, 8f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            // background texture
            var textureRequest = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Enemy/BossProjectiles/GuardianOfTheRiftProjs/CHLORO_LeafCrystalBackground");
            Vector2 drawPos = new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f);
            SpriteEffects effects = SpriteEffects.None;
            if (Projectile.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            Main.EntitySpriteDraw(textureRequest.Value,
                drawPos,
                new Rectangle(0, 0, textureRequest.Value.Width, textureRequest.Value.Height), new Color(255, 255, 255, 0) * 0.3f,
                Projectile.rotation, new Vector2(textureRequest.Value.Width * 0.5f, textureRequest.Value.Height * 0.5f), Projectile.scale * 1.05f, effects, 0);

            // main texture
            textureRequest = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Enemy/BossProjectiles/GuardianOfTheRiftProjs/CHLORO_LeafCrystalBase");
            Main.EntitySpriteDraw(textureRequest.Value,
                drawPos,
                new Rectangle(0, 0, textureRequest.Value.Width, textureRequest.Value.Height), Projectile.GetAlpha(Color.White), 
                Projectile.rotation, new Vector2(textureRequest.Value.Width * 0.5f, textureRequest.Value.Height * 0.5f), Projectile.scale, effects, 0);


            return false;
        }
    }
}