using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraTorchProjImpact : ModProjectile
    {
        private int phase = 1;
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.1f;
            Projectile.timeLeft = 180;
            Projectile.rotation = Main.rand.NextFloat(2 * (float)Math.PI);
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 45)
            {
                Projectile.scale *= 0.95f;
                Projectile.rotation += 0.1f;
            }
            else
            {
                Projectile.rotation += 0.05f;
            }
            if (Projectile.ai[0] == 45)
            {
                Projectile.scale = 0.1f;
            }
            if (Projectile.ai[0] > 50)
            {
                Projectile.friendly = true;
                Projectile.alpha += 10;
            }

            if(Projectile.alpha > 255)
            {
                Projectile.Kill();
            }

            
            Projectile.ai[0]++;
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if  (phase == 1)
            {
                Main.EntitySpriteDraw(texture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, texture.Width, texture.Height),
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
