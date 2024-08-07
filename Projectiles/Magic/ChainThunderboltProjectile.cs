using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace TheTesseractMod.Projectiles.Magic
{
    internal class ChainThunderboltProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 1;
            
        }
        float floatScale = 1.2f;
        int alpha = 150;

        public override void AI()
        {
            if (alpha > 50)
            {
                alpha -= 5;
            }
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ElectricDust>(), Projectile.velocity.X *= 0.985f, Projectile.velocity.Y *= 0.985f, alpha, Color.White, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Magic/ChainThunderboltProjectile");
            Main.EntitySpriteDraw(texture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, texture.Value.Width, texture.Value.Height),
                new Color(92, 0, 0, 0) * (1f - Projectile.alpha / 255f), 0, texture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}