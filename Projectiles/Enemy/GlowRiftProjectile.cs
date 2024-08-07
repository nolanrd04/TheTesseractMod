using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.CodeAnalysis;

namespace TheTesseractMod.Projectiles.Enemy
{
    internal class GlowRiftProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SporeCloud);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Lighting.AddLight(Projectile.position, 0f, 0f, 1f);
            //draw glow
            Texture2D glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect").Value;
            Main.EntitySpriteDraw(glowTexture,
               new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Width, glowTexture.Height),
                Color.Blue, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return true;
        }

        public float Lerp(float x, float y, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return x + amount * (y - x);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Projectile.position, 15, 15, DustID.GlowingMushroom);
            }
        }
    }
}