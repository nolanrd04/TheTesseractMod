using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using TheTesseractMod.Projectiles.Melee;

namespace TheTesseractMod.Projectiles.Melee
{
    internal class DeathFlameMain : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            //Projectile.CloneDefaults(ProjectileID.Typhoon);
            //AIType = ProjectileID.Typhoon;
            Projectile.velocity = new Vector2(4f, 4f);
            Projectile.width = 15;
            Projectile.height = 15;
            // Projectile.aiStyle = 5;
            Projectile.friendly = true;
            //Projectile.hostile = false;
            Projectile.penetrate = 100;
            Projectile.timeLeft = 80;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.scale = 0.4f;
        }
        Color color = new Color(255, 0, 0);
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 1f, 0f, 0f);
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 235, 0, 0, 150, color, 0.9f);
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0, 0, 150, color, 0.9f);
            Projectile.ai[0]++;
            Projectile.rotation += 0.2f * (float)Projectile.direction;
            if (Projectile.ai[0]%15==0)
            {
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
            }
            if (Projectile.ai[0]%2==0)
            {
                
                Random randX = new Random();
                Random randY = new Random();
                int randomX  = randX.Next(-20, 20);
                int randomY = randY.Next(-20, 20);
                Vector2 newPos = new Vector2(Projectile.position.X+randomX, Projectile.position.Y+randomY);
            }
            if (Projectile.ai[0] > 10)
            {
                for (int i = 0; i < 200; i++)
                {
                    NPC target = Main.npc[i];

                    if (Main.npc[i].CanBeChasedBy(this, false))
                    {
                        float goToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                        float goToY = target.position.Y - Projectile.Center.Y;
                        float distance = (float)Math.Sqrt(goToX * goToX + goToY * goToY);

                        if (distance < 400 && !target.friendly && target.active)
                        {
                            distance = 3f / distance;
                            goToX *= distance * 5;
                            goToY *= distance * 5;

                            Projectile.velocity.X = goToX;
                            Projectile.velocity.Y = goToY;
                        }

                    }
                }
            }
        }
    }
}
