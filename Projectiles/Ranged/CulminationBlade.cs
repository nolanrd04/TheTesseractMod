using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheTesseractMod.Projectiles.Ranged
{
    public class CulminationBlade : ModProjectile // NOT USED AT ALL
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.width = 115;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            
            Projectile.light = 1f;
            Random rand = new Random();
            int divisorScale = rand.Next(5, 8);
            Projectile.scale = 18.0f / ((float)(divisorScale));
        }

        Color color = new Color(222, 120, 255);

        public override void AI()
        {
            
            Projectile.ai[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] > 3)
            {
                    Projectile.velocity *= 0.5f;
            }
            

            
            //Lighting.AddLight(Projectile.position, 0.9f, 0.5f, 1f);
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 112, Projectile.velocity.X, Projectile.velocity.Y, 150, color, 0.8f);
            int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 27, Projectile.velocity.X, Projectile.velocity.Y, 150, color, 0.4f);
            Main.dust[dust].noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            
            return false;
        }
    }
}
