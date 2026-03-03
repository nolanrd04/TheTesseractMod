using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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

namespace TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Sain_geom
{
    internal class Sain_geom_projThrown : ModProjectile
    {

        private float speed;
        private float distFromTarget;
        private VertexStrip strip = new VertexStrip();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        private static readonly Color[] maskColors = {
            new Color (126, 249, 255),
            new Color(126, 212, 255),
            new Color (126, 170, 255),
            new Color(126, 212, 255)
        };

        Color maskColor;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 69;
            Projectile.height = 69;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();

        }

        public override void AI()
        {

            Projectile.rotation += MathHelper.ToRadians(15f);
            if (Projectile.ai[0] > 20)
            {
                Vector2 desiredVelocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .25f);
            }

            if (Main.rand.Next(6) == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerSpark, 0, 0, 0, maskColor, 1f);
            }

            Projectile.ai[0]++;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            // strip
            GameShaders.Misc["RainbowRod"].Apply();

            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => new Color(80, 68, 252, 0) * (1f - progress),
                progress => MathHelper.Lerp(15f, 16f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            // coloring
            int numColors = maskColors.Length;
            float fade = (Main.GameUpdateCount % 60) / 60f;
            int index = (int)((Main.GameUpdateCount / 60) % numColors);
            int nextIndex = (index + 1) % numColors;

            maskColor = Color.Lerp(maskColors[index], maskColors[nextIndex], fade);

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            // draw hilt
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            // draw blade
            Asset<Texture2D> glowMask = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Sain_geom/Sain_geom_projThrown_mask");
            Main.EntitySpriteDraw(glowMask.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                maskColor,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
