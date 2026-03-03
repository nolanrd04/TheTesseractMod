using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
using TheTesseractMod.Items.Weapons.Melee.KPDH;

namespace TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Sain_geom
{
    internal class Sain_geom_dashProj : ModProjectile
    {
        private VertexStrip strip = new VertexStrip();

        public override string Texture => "TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Sain_geom/Sain_geom_projThrown";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        private static readonly Color[] maskColors = {
            new Color(255, 203, 203), // light red
            new Color (255, 230, 204), // light orange
            new Color (254, 255, 204), // light yellow
            new Color (208, 255, 204), // light green
            new Color(204, 223, 255), // light blue
            new Color(231, 204, 255), // light purple
            new Color(255, 204, 243)  // light pink
        };

        Color maskColor;

        public override void SetDefaults()
        {
            Projectile.width = 55;
            Projectile.height = 55;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Melee;

        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center; 

            float desiredRotation = Projectile.rotation - MathHelper.PiOver2;
            if (owner.direction == -1)
            {
                desiredRotation = MathHelper.Pi - desiredRotation;
                owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, desiredRotation);
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, desiredRotation);
            }
            else
            {
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, desiredRotation);
                owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, desiredRotation);
            }

            // Make sure the player holds this projectile
            owner.heldProj = Projectile.whoAmI;

            // coloring
            int numColors = maskColors.Length;
            float fade = (Main.GameUpdateCount % 30) / 30f;
            int index = (int)((Main.GameUpdateCount / 30) % numColors);
            int nextIndex = (index + 1) % numColors;

            maskColor = Color.Lerp(maskColors[index], maskColors[nextIndex], fade);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // drawing trail
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.5f + progress) % 1f, .5f, 0.5f)
                * (1f - progress),
                progress => {
                    float peak = 0.2f; // where in the trail the width is widest
                    float maxWidth = 30f;
                    float minWidth = 8f;

                    // Scale factor: small at ends, big in the middle
                    float factor = 1f - Math.Abs(progress - peak) / (1f - peak);
                    factor = MathHelper.Clamp(factor, 0f, 1f);

                    return MathHelper.Lerp(minWidth, maxWidth, factor);
                },

                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: false
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            // drawing main
            Player owner = Main.player[Projectile.owner];

            float rotation;
            Vector2 offset;
            if (owner.direction == 1)
            {
                rotation = (float)-Math.PI;
                offset = new Vector2(-25, 30);
            }
            else
            {
                rotation = (float)Math.PI / 2;
                offset = new Vector2(-30, 25);
            }
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                rotation, origin + offset, Projectile.scale, SpriteEffects.None, 1f);

            // draw blade (glow mask)
            Asset<Texture2D> glowMask = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Sain_geom/Sain_geom_projThrown_mask");
            Main.EntitySpriteDraw(glowMask.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                maskColor,
                rotation, origin + offset, Projectile.scale, SpriteEffects.None, 1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.RainbowRodHit, new ParticleOrchestraSettings { PositionInWorld = target.Center, MovementVector = Vector2.Zero });

            Player owner = Main.player[Projectile.owner];
            owner.immune = true;
            owner.immuneTime = 6;

            Vector2 spawnPos = target.Center + new Vector2(150 + Main.rand.Next(50), 0).RotatedBy(Main.rand.NextFloat(2 * (float)Math.PI));
            Vector2 direction = (target.Center - spawnPos);
            direction.Normalize();
            direction *= 10f;

            Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, direction, ModContent.ProjectileType<Sain_geom_star_3>(), Projectile.damage, 0f);
        }
    }
}
