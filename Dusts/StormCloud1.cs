using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Dusts
{
    internal class StormCloud1 : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale = 0.1f;
            dust.frame = new Rectangle(0, 0, 32, 32);
            dust.alpha = 255;
            dust.scale = 2f;
            dust.fadeIn = 1f;
        }
        public override bool Update(Dust dust)
        {
            if (dust.fadeIn < 25)
            {
                dust.alpha -= 10;
                if (dust.alpha < 0)
                {
                    dust.alpha = 0;
                }
            }
            dust.rotation += (MathHelper.ToRadians(5) / (float)(dust.fadeIn * 0.7f));

            if (dust.fadeIn > 40)
            {
                dust.alpha += 25;
            }

            if (dust.alpha >= 255)
            {
                dust.active = false;
            }
            dust.position += dust.velocity;
            dust.velocity *= 0.97f;
            dust.fadeIn++;
            return false;

        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(lightColor.R, lightColor.G, lightColor.B, 255 - dust.alpha);
        }

        public override bool PreDraw(Dust dust)
        {
            // background texture
            Color color = dust.GetAlpha(dust.color);
            Main.spriteBatch.Draw(Texture2D.Value,
                new Vector2(dust.position.X - Main.screenPosition.X, dust.position.Y - Main.screenPosition.Y),
                new Rectangle(0, 0, Texture2D.Value.Width, Texture2D.Value.Height),
                color * 0.2f, 
                dust.rotation, Texture2D.Value.Size() * 0.5f, dust.scale * 1.3f, SpriteEffects.None, 0f);

            // main texture
            Main.spriteBatch.Draw(Texture2D.Value,
                new Vector2(dust.position.X - Main.screenPosition.X, dust.position.Y - Main.screenPosition.Y),
                new Rectangle(0, 0, Texture2D.Value.Width, Texture2D.Value.Height),
                color * 0.4f, dust.rotation, Texture2D.Value.Size()*0.5f, dust.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
