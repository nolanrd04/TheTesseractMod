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
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Gok_do
{
    internal class Gok_Do_Star_Stage2 : ModProjectile
    {
        private VertexStrip strip = new VertexStrip();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        private float speed;
        public override string Texture => "TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Sain_geom/Sain_geom_star_1";
        public override void SetDefaults()
        {
            Projectile.width = 11;
            Projectile.height = 11;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.scale = 1.5f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Melee;

        }

        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 20)
            {
                NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);

                if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, float.MaxValue))
                {
                    Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .1f);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

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

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                new Color(65, 225, 255, 0),
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
