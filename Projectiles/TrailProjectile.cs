using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles
{
    public class TrailProjectile : ModProjectile
    {
        private VertexStrip strip = new VertexStrip();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override string Texture => "TheTesseractMod/Textures/empty";
        private float speed;
        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;  // smoother trail
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0f, 0.6f, 0.9f);

            Vector2 target = Main.MouseWorld;
            Projectile.velocity = Vector2.Normalize(target - Projectile.Center) * speed;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.5f + progress) % 1f, 1f, 0.5f)
                * (1f - progress),
                progress => {
                    float peak = 0.2f; // where in the trail the width is widest
                    float maxWidth = 32f;
                    float minWidth = 8f;

                    // Scale factor: small at ends, big in the middle
                    float factor = 1f - Math.Abs(progress - peak) / (1f - peak);
                    factor = MathHelper.Clamp(factor, 0f, 1f);

                    return MathHelper.Lerp(minWidth, maxWidth, factor);
                },

                - Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();


            return true;
        }
    }
}
