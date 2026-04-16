using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TheTesseractMod.Dusts;
using TheTesseractMod.GlobalFuncitons;
using Microsoft.Xna.Framework.Graphics;

namespace TheTesseractMod.Projectiles.Magic
{
    internal class PrimeMeridianProjectile : ModProjectile
    {
        int counterForNoHoming = 7;
        bool rehome = false;
        NPC lastHit = null; // the npc the projectile just made contact with
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 200;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0.8f;
            Projectile.usesLocalNPCImmunity = true;
             Projectile.localNPCHitCooldown = 25;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 0.3f, 0.94f, 0.48f);
            if (Projectile.ai[0] % 3 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, ModContent.DustType<SharpRadialGlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.Lime, 1f);
            }
            
            Projectile.ai[0]++;
            Projectile.rotation += 0.4f * (float)Projectile.direction;
            if (counterForNoHoming > 0)
            {
                counterForNoHoming--;
            }
            if (counterForNoHoming <= 0)
            {
                lastHit = null;
            }

            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20f;
            //************************************//

            if (Projectile.ai[0] > 10)
            {
                NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center, lastHit);

                if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, 1000f, lastHit))
                {
                    Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .25f);
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            lastHit = target;
        }

        public override bool PreDraw(ref Color lightColor)
        {

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
    }
}
