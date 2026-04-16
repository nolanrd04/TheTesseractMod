using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Summoner
{
    internal class ZenithWhipBurstController : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        private int staggerInterval;
        private float whipSize = .75f;
        private int customWhipType;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300; // Last long enough for burst
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead)
                return;

            // Get values from ai
            Vector2 velocity = new Vector2(Projectile.ai[0], Projectile.ai[1]);
            customWhipType = (int)Projectile.ai[2];

            Item heldItem = player.HeldItem;
            int damage = heldItem.damage;
            float knockback = heldItem.knockBack;

            // Calculate stagger interval (1/3 of use time)
            staggerInterval = heldItem.useTime / 3;

            int burstTimer = (int)Projectile.localAI[0];

            // Spawn whips at staggered intervals
            if (burstTimer == 0)
            {
                // Type 1 - dark harvest
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, velocity * whipSize, 849, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, -0.5f * velocity * whipSize, 849, damage, knockback, player.whoAmI);
            }
            else if (burstTimer == staggerInterval)
            {
                // Type 2
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, velocity * whipSize, customWhipType, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, -0.5f * velocity * whipSize * 1.1f, customWhipType, damage, knockback, player.whoAmI);
            }
            else if (burstTimer == staggerInterval * 2)
            {
                // Type 3
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, (velocity + (velocity * (0.5f))) * whipSize, 913, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, -0.5f * (velocity + (velocity * (0.5f))) * whipSize, 913, damage, knockback, player.whoAmI);
                Projectile.Kill(); // Burst complete, kill this controller

                return;
            }

            Projectile.localAI[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false; // Don't draw this projectile
        }
    }
}
