using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
    internal class Shin_Kal_Proj_Stage3 : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Shin_Kal/Shin_Kal_Proj_hiltOnly";

        private VertexStrip strip = new VertexStrip();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        private float speed;
        private bool canHome = true;

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
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
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
                if (Math.Abs(Vector2.Distance(Main.MouseWorld, Projectile.Center)) > 50f)
                {
                    Vector2 direction = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);

                    Vector2 velocity = direction * speed;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, .2f);
                }
                else
                {
                    // Main.NewText(Vector2.Distance(Main.MouseWorld, Projectile.Center));
                    canHome = false;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;

            if (Main.rand.NextFloat() < .1f)
            {
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.RainbowRodHit, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
            }
            if (Main.rand.NextFloat() < .09f)
            {
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
            }
            if (Main.rand.NextFloat() < .15f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, new Color(56, 184, 252, 0), Main.rand.NextFloat(.7f) + .8f);
            }
            if (Main.rand.NextFloat() < .33333f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, DustID.FireworksRGB, 0, 0, 0, maskColors[Main.rand.Next(7)], Main.rand.NextFloat(.6f) + .4f);
            }

            if (Main.rand.NextFloat() < .12f)
            {
                Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, maskColors[Main.rand.Next(7)], Main.rand.NextFloat(.4f) + .4f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // coloring
            int numColors = maskColors.Length;
            float fade = (Main.GameUpdateCount % 30) / 30f;
            int index = (int)((Main.GameUpdateCount / 30) % numColors);
            int nextIndex = (index + 1) % numColors;

            maskColor = Color.Lerp(maskColors[index], maskColors[nextIndex], fade);

            // Draw vertex strip
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => maskColor * (1f - progress),
                progress => MathHelper.Lerp(15f, 7f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            // draw hilt texture
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Asset<Texture2D> glowMask = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Shin_Kal/Shin_Kal_glowMask");
            Main.EntitySpriteDraw(glowMask.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                maskColor,
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
