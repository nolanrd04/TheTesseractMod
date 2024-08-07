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
    internal class RiftProjectile : ModProjectile
    {
        private float scalingFactor;
        private float rotationFactor;
        private int timer;
        private float begin = 0.15f;
        private float end = 0.3f;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.NebulaSphere);
            Projectile.damage = 100;
            Projectile.alpha = 0;
            Projectile.timeLeft = 400;
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
            float fade = timer / 60f;

            if (fade == 1)
            {
                timer = 1;
                float holder = begin;
                begin = end;
                end = holder;
            }
            else
            {
                scalingFactor = Lerp(begin, end, fade);
            }
            Color color = (Color)GetAlpha(Color.White);
            rotationFactor += 16f;

            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Enemy/RiftProjectile");
            Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                color, rotationFactor, glowTexture.Size() * 0.5f, scalingFactor, SpriteEffects.None, 0f);

            return false;
        }
        public override void AI()
        {
            Projectile.Center = Projectile.position + new Vector2(Projectile.width / 2, Projectile.height / 2);
            Lighting.AddLight(Projectile.position, 226/255f, 230/255f, 168/255f);
            //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 204, Projectile.velocity.X, Projectile.velocity.Y, 150, default(Color), 1.5f);
            //Projectile.rotation += MathHelper.ToRadians(4f);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }
    }
}