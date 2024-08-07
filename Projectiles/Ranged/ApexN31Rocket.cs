using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Ranged
{
    internal class ApexN31Rocket : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            //Projectile.CloneDefaults(ProjectileID.RocketIII);
            //AIType = ProjectileID.Typhoon;
            Projectile.velocity = new Vector2(0.5f, 0.5f);
            Projectile.width = 20;
            Projectile.height = 20;
            // Projectile.aiStyle = 5;
            Projectile.friendly = true;
            //Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }


        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, Projectile.velocity.X, Projectile.velocity.Y, 70, default, 0.8f);
            Projectile.ai[0]++;

            if (Projectile.ai[0] > 25)
            {
                if (Projectile.velocity.X <= 1f || Projectile.velocity.Y <= 1f)
                {
                    Projectile.velocity.X *= 1.05f;
                    Projectile.velocity.Y *= 1.05f;
                }



                for (int i = 0; i < 200; i++)
                {
                    NPC target = Main.npc[i];

                    if (Main.npc[i].CanBeChasedBy(this, false))
                    {
                        float goToX = target.position.X + target.width * 0.5f - Projectile.Center.X;
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

        public override void OnKill(int timeLeft)
        {
            Vector2 explosionVector = new Vector2(0.1f, 0.1f);
            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, explosionVector, ProjectileID.SolarWhipSwordExplosion, Projectile.damage, Projectile.knockBack, Projectile.owner);
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, explosionVector, ModContent.ProjectileType<ApexN31Explosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }

}