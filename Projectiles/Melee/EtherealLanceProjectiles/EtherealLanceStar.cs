using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.Melee.EtherealLanceProjectiles
{
    internal class EtherealLanceStar : ModProjectile
    {
        private float scalingFactor;
        private float rotationFactor = 8;
        private int timer;
        private float begin = 0.2f;
        private float end = 0.35f;
        private float travelingSpeed = 10f;

        private float smallerBegin = .15f;
        private float smallerEnd = 0.3f;
        private float smallerScalingFactor;
        private float smallerRotationFactor;

        Color color = Color.White;
        private static readonly Color[] colors = {
            new Color(255, 252, 153),
            new Color (255, 174, 107),
            new Color (157, 238, 250)
        };
        private VertexStrip strip = new VertexStrip();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 130;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => color * (1f - progress),
                progress => MathHelper.Lerp(15f, 7f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            timer++;
            float fade = (float)Math.Sin(timer * MathHelper.TwoPi / 120f);
            fade = (fade + 1f) / 2f;
            scalingFactor = GlobalMathFunctions.Lerp(0.25f, 0.5f, fade);
            rotationFactor += 8f;

            float fading = (timer % 60) / 60f;
            int index = (int)((timer / 60) % 3);
            int nextIndex = (index + 1) % 3;
            color = Color.Lerp(colors[index], colors[nextIndex], fading);

            Asset<Texture2D> Texture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/EtherealLanceProjectiles/EtherealLanceStar");
            Main.EntitySpriteDraw(Texture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, Texture.Value.Width, Texture.Value.Height),
                new Color(color.R, color.G, color.B, 0) * (1f - Projectile.alpha / 255f), rotationFactor, Texture.Size() * 0.5f, scalingFactor * 0.25f, SpriteEffects.None, 0f);

            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, new Vector3(color.R/255f, color.G/255f, color.B/255f));
            Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, color, .4f);
            Projectile.ai[0]++;
            
            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * travelingSpeed;
            //************************************//
            if (Projectile.ai[0] > 10)
            {
                NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);

                if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, 300f))
                {
                    Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * travelingSpeed;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .3f);
                }
            }
            travelingSpeed *= 0.99f;
        }
    }
}
