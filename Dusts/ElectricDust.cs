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
    internal class ElectricDust : ModDust // default alpha is 255. Assign a color in Dust.NewDust().
    {
        private int counter = 0;
        float rotation = 0f;
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale = 0.1f;
            dust.frame = new Rectangle(0, 0, 424, 424);
        }
        public override bool Update(Dust dust)
        {
            
            Lighting.AddLight(dust.position, 89 / 255f, 247 / 255f, 255 / 255f);
            dust.scale *= 0.95f;

            if (dust.scale < 0.03f)
            {
                dust.active = false;
            }

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color = dust.color;
            return color * (1f - dust.alpha / 255f);
        }

        public override bool PreDraw(Dust dust)
        {
            if (counter % 10 == 0)
            {
                Random rand = new Random();
                rotation = (float)(rand.NextDouble() * 360);
            }
            counter++;

            Color color = dust.color;
            Main.spriteBatch.Draw(Texture2D.Value,
                new Vector2(dust.position.X - Main.screenPosition.X, dust.position.Y - Main.screenPosition.Y),
                new Rectangle(0, 0, Texture2D.Value.Width, Texture2D.Value.Height),
                new Color(color.R, color.G, color.B, 0) * (1f - dust.alpha / 255f), MathHelper.ToRadians(rotation), Texture2D.Value.Size()*0.5f, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
