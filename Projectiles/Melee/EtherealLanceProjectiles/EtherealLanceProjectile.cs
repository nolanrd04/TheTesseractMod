using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using TheTesseractMod.Dusts;
using TheTesseractMod.Buffs;

namespace TheTesseractMod.Projectiles.Melee.EtherealLanceProjectiles
{
    internal class EtherealLanceProjectile : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 92f;
        protected virtual float HoldoutRangeMax => 164f;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 50;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                Projectile.rotation += MathHelper.ToRadians(135);
            }

            // Avoid spawning dusts on dedicated servers
            if (!Main.dedServ)
            {
                // These dusts are added later, for the 'ExampleMod' effect
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RiftLightBlueDust>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Alpha: 128, Scale: 1.2f);
                }

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RiftLightBlueDust>(), Alpha: 128, Scale: 0.3f);
                }
            }

            return false; // Don't execute vanilla AI.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<DeathProtectionOne>(), 90);

            target.AddBuff(BuffID.Frostburn, 180);
            for (int i = 0; i < 4; i++)
            {
                Vector2 newPosition = target.position + new Vector2(Main.rand.NextFloat(-200, 200), Main.rand.NextFloat(-200, 200));

                Vector2 direction = (target.Center - newPosition).SafeNormalize(Vector2.UnitX);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), newPosition, direction * 10f, ModContent.ProjectileType<DeadlyIcicleFriendly>(), Projectile.damage, Projectile.knockBack);
            }
        }
    }
}
