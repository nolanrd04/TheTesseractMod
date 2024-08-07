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
    internal class DarkRiftProjectile : ModProjectile
    {
        private float scalingFactor;
        private float rotationFactor;
        private int timer;
        private float begin = 0.2f;
        private float end = 0.35f;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.NebulaSphere);
            Projectile.damage = 100;
            Projectile.alpha = 0;
            Projectile.timeLeft = 200;
            Projectile.light = 0.9f;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
        }
        public float Lerp(float x, float y, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return x + amount * (y - x);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            timer++;
            float fade = (float)Math.Sin(timer * MathHelper.TwoPi / 120f);
            fade = (fade + 1f) / 2f;
            scalingFactor = Lerp(0.25f, 0.5f, fade);

            Color color = (Color)GetAlpha(Color.White);

            rotationFactor += 8f;

            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Enemy/RiftProjectile");
            Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                new Color(92, 0, 204, 0) * (1f - Projectile.alpha / 255f), rotationFactor, glowTexture.Size() * 0.5f, scalingFactor * 0.25f, SpriteEffects.None, 0f); ;

            Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f), rotationFactor, glowTexture.Size() * 0.5f, scalingFactor * 0.75f * 0.25f, SpriteEffects.None, 0f);

            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 226/255f, 230/255f, 168/255f);

            if (timer % 3 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, Projectile.velocity.X, Projectile.velocity.Y, 150, default(Color), 1f);
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(223, 194, 255, 0) * (1f - Projectile.alpha / 255f);
        }
    }
}