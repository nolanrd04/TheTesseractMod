using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Weapons.Melee.KPDH;
using TheTesseractMod.Items.Weapons.Melee.KPDH.Gok_do;
using TheTesseractMod.Projectiles.Melee.EtherealLanceProjectiles;
using TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Sain_geom;
using static tModPorter.ProgressUpdate;

namespace TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Gok_do
{
    internal class Gok_Do_Proj : ModProjectile
    {
        private const float HoldoutRangeMin_LowerCap = 50;
        private const float HoldoutRangeMin_UpperCap = 80f;
        private const float HoldoutRangeMax_LowerCap = 220;
        private const float HoldoutRangeMax_UpperCap = 250;
        private float holdoutMin;
        private float holdoutMax;
        private float progress;

        private VertexStrip strip = new VertexStrip();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.scale = 1.3f;
        }


        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            int duration = player.itemAnimationMax;

            player.heldProj = Projectile.whoAmI;

            if (Projectile.timeLeft > duration)
                Projectile.timeLeft = duration;

            Projectile.velocity = Vector2.Normalize(Main.MouseWorld - player.Center);

            float halfDuration = duration * 0.5f;

            if (Projectile.timeLeft < halfDuration)
                progress = Projectile.timeLeft / halfDuration;
            else
                progress = (duration - Projectile.timeLeft) / halfDuration;

            // dynamically calculate min/max holdout
            float distanceToMouse = Vector2.Distance(player.MountedCenter, Main.MouseWorld);

            // normalize distance into [0, 1] scale; tweak 400f as the "max considered distance"
            float t = MathHelper.Clamp(distanceToMouse / 200f, 0f, 1f);

            holdoutMin = MathHelper.Lerp(HoldoutRangeMin_LowerCap, HoldoutRangeMin_UpperCap, t);
            holdoutMax = MathHelper.Lerp(HoldoutRangeMax_LowerCap, HoldoutRangeMax_UpperCap, t);

            // Move spear according to cursor-based range
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * holdoutMin, Projectile.velocity * holdoutMax, progress);

            // Apply proper rotation
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                Projectile.rotation += MathHelper.ToRadians(135f);
            }

            return false; // skip vanilla AI
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Draw trail:
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => new Color(195, 92, 255, 0) * (1f - progress),
                progress => MathHelper.Lerp(15f, 7f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            // ***** Draw Blade ***** //
            Player player = Main.player[Projectile.owner];
            Vector2 pivot = player.MountedCenter;

            // Texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);

            // Origin is at the base of the spear (bottom center)
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            // Compute sprite effects based on player direction
            SpriteEffects spriteEffects = SpriteEffects.None;
            float rotationOffset;
            if (player.direction == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                rotationOffset = MathHelper.ToRadians(45f);
            }
            else
            {
                rotationOffset = MathHelper.ToRadians(135);
            }
            float tipDistance = MathHelper.SmoothStep(holdoutMin, holdoutMax, progress);

            // Position: pivot + direction * distance
            Vector2 drawPosition = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * holdoutMin, Projectile.velocity * holdoutMax, progress)
                + Projectile.velocity * -HoldoutRangeMin_UpperCap - Main.screenPosition;
            // Draw the spear
            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                sourceRectangle,
                Color.White,
                Projectile.velocity.ToRotation() + rotationOffset,
                origin,
                Projectile.scale,
                spriteEffects,
                0
            );

            // ***** Spawn Dust ***** //

            if (Main.rand.NextFloat() < .333f)
            {
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
            }
            if (Main.rand.NextFloat() < .15f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, new Color(238, 54, 255, 0), Main.rand.NextFloat(.7f) + .8f);
            }
            if (Main.rand.NextFloat() < .33333f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, DustID.PinkTorch, 0, 0, 0, default(Color), Main.rand.NextFloat(.4f) + .8f);
            }

            if (Main.rand.NextFloat() < .12f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, new Color(242, 218, 58, 0), Main.rand.NextFloat(.4f) + .4f);
            }

            return false; // prevent default draw
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.CanBeChasedBy() && GokDoStats.canBuildConsecutiveHits)
            {
                GokDoStats.consecutiveHits++;
                GokDoStats.comboExpireTimer = 0;
            }

            Vector2 spawnPos = target.Center + new Vector2(150 + Main.rand.Next(50), 0).RotatedBy(Main.rand.NextFloat(2 * (float)Math.PI));
            Vector2 direction = (target.Center - spawnPos);
            direction.Normalize();
            direction *= 10f;

            Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, direction, ModContent.ProjectileType<Sain_geom_star_1>(), Projectile.damage / 2, 0f);
        }
    }
}

