using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraYoyoBurst : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Projectiles/TerraWeapons/TerraTorchProjImpact";
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.1f;
            Projectile.timeLeft = 100;
            Projectile.rotation = Main.rand.NextFloat(2 * (float)Math.PI);
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 7)
            {
                Projectile.friendly = true;
            }
            Projectile.velocity *= 0.95f;
            Projectile.scale *= 0.98f;
            Projectile.rotation += MathHelper.ToRadians(10);
            Projectile.ai[0]++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, texture.Width, texture.Height),
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
