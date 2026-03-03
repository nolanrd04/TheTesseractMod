using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Weapons.Melee.KPDH.Shin_kal;

namespace TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Shin_Kal
{
    internal class Shin_Kal_Proj_Stage2 : ModProjectile
    {
        private VertexStrip strip = new VertexStrip();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        private float speed;
        private bool canHome = true;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
            canHome = Projectile.ai[0] == 1f;
        }
        public override void AI()
        {
            if (canHome)
            {
                // NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);

                // home, only under certain conditions (not close to target)
                if (Math.Abs(Vector2.Distance(Main.MouseWorld, Projectile.Center)) > 30f)
                {
                    Vector2 direction = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);

                    Vector2 velocity = direction * speed;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, .09f);
                }
                else
                {
                    // Main.NewText(Vector2.Distance(Main.MouseWorld, Projectile.Center));
                    canHome = false;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;

            if (Main.rand.NextFloat() < .13f)
            {
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
            }
            if (Main.rand.NextFloat() < .15f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, new Color(56, 184, 252, 0), Main.rand.NextFloat(.5f) + .6f);
            }
            if (Main.rand.NextFloat() < .33333f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, DustID.UltraBrightTorch, 0, 0, 0, default(Color), Main.rand.NextFloat(.4f) + .8f);
            }

            if (Main.rand.NextFloat() < .12f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, new Color(127, 68, 252, 0), Main.rand.NextFloat(.3f) + .4f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Draw vertex strip
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => new Color(65, 225, 255, 0) * (1f - progress),
                progress => MathHelper.Lerp(15f, 7f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            // draw main texture
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.CanBeChasedBy())
            {
                int divisor = 5;
                if (Projectile.ai[0] == 0f)
                {
                    divisor = 1;
                }
                Projectile.vampireHeal(damageDone / divisor, Projectile.Center, Entity);
            }

            if (target.CanBeChasedBy() && ShinKalStats.canBuildConsecutiveHits)
            {
                ShinKalStats.consecutiveHits++;
                ShinKalStats.comboExpireTimer = 15;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
            }
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);

            int numDivisions = Main.rand.Next(8) + 7;

            for (int i = 0; i < numDivisions; i++)
            {
                float rotation = 360f / numDivisions;
                rotation *= i;

                Vector2 velocity = new Vector2(5f, 0);
                velocity = velocity.RotatedBy(rotation);

                if (Main.rand.NextBool())
                {
                    Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), velocity.X, velocity.Y, 0, new Color(56, 184, 252, 0), Main.rand.NextFloat(.4f) + .4f);
                }
                else
                {
                    Dust.NewDust(Projectile.Center, 0, 0, DustID.UltraBrightTorch, velocity.X, velocity.Y, 0, default(Color), Main.rand.NextFloat(.8f) + .4f);
                }
            }

        }
    }
}
