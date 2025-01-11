using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Dusts
{
    internal class RadialGlowDustShrink : ModDust
    {
        public override string Texture => "TheTesseractMod/Dusts/RadialGlowDust";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale = dust.scale * 0.08f;
            dust.frame = new Rectangle(0, 0, 435, 435);
        }
        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, dust.color.R / 128, dust.color.G / 128, dust.color.B / 128);
            dust.alpha += 8;
            if (dust.alpha >= 255)
            {
                dust.active = false;
            }

            dust.position += dust.velocity *= 0.95f;
            dust.scale *= 0.6f;

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color = dust.color;
            return color * (1f - dust.alpha / 255f);
        }

        public override bool PreDraw(Dust dust)
        {

            Color color = dust.color;
            Main.spriteBatch.Draw(Texture2D.Value,
                new Vector2(dust.position.X - Main.screenPosition.X, dust.position.Y - Main.screenPosition.Y),
                new Rectangle(0, 0, Texture2D.Value.Width, Texture2D.Value.Height),
                new Color(color.R, color.G, color.B, 0) * (1f - dust.alpha / 255f), dust.rotation, Texture2D.Value.Size() * 0.5f, dust.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Texture2D.Value,
                new Vector2(dust.position.X - Main.screenPosition.X, dust.position.Y - Main.screenPosition.Y),
                new Rectangle(0, 0, Texture2D.Value.Width, Texture2D.Value.Height),
                new Color(255, 255, 255, 0) * (1f - dust.alpha / 255f), dust.rotation, Texture2D.Value.Size() * 0.5f, dust.scale * 0.7f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
