using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TheTesseractMod.Global.Projectiles.Magic;

namespace TheTesseractMod.Projectiles.Magic
{
    internal class ConjuringClimaxProjectile : ModProjectile
    {
        public override void SetDefaults() // first projectile ever made. Forgive the messiness
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.CloneDefaults(ProjectileID.RainbowRodBullet);
            AIType = ProjectileID.RainbowRodBullet;

            Projectile.width = 50;
            Projectile.height = 50;
            //Projectile.aiStyle = 5;
            Projectile.friendly = true;
            //Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        float vX = 6f;
        float vY = 6f;
        public override void OnKill(int timeLeft)
        {
            ConjuringClimaxCalamityOverrider.shotByConjuringClimax = true;
            int damage = Projectile.damage / 2;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                damage = Projectile.damage;
            }
            vX *= 0.8f;
            vY *= 0.8f;
            Vector2 launchVelocity = new Vector2(0, 0);
            Vector2 launchVelocity1 = new Vector2(vX, vY);
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVelocity, 729, damage, Projectile.knockBack, Projectile.owner);
            }
            /*if (ModLoader.TryGetMod("CalamityMod", out calamityMod))
            {
                return;
            }*/
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVelocity, 296, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            ConjuringClimaxCalamityOverrider.shotByConjuringClimax = false;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 62, Projectile.velocity.X, Projectile.velocity.Y, 150, default(Color), 0.9f);
        }

        /*
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(EntitySource source, Projectile.Center, Projectile.velocity, 79, Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        */

        //public override Texture2D Texture => "Terraria/Images/Projectile_" + ProjectileID.RainbowRodBullet;
    }
}

