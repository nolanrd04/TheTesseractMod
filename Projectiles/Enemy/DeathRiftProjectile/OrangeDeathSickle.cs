using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Enemy.DeathRiftProjectile
{
    internal class OrangeDeathSickle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 540;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 255 / 255f, 150 / 255f, 46 / 255f);
            if (Projectile.ai[0] % 3 == 0)
            {
                Dust.NewDust(Projectile.position, 24, 24, DustID.OrangeTorch);
            }
            Projectile.ai[0] += 1f;
            float length = (float)Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y);

            if (Projectile.ai[0] > 60 && length < 10f)
            {
                Projectile.velocity *= 1.02f;
            }
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;

            
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            float offsetX = 24f;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            int degree = 36;
            Vector2 velocity = Projectile.velocity;
            for (int i = 0; i < 10; i++)
            {
                velocity.RotatedBy(MathHelper.ToRadians(i * degree));
                Dust.NewDust(Projectile.position, 16, 16, DustID.OrangeTorch, velocity.X, velocity.Y);
            }
        }
    }
}

