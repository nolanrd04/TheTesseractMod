using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = 1;
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 50;
        }

        Color color = new Color(222, 120, 255);
        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 newPosition = Projectile.position + new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);
            Dust.NewDustPerfect(newPosition, ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 0, Color.Lime, 0.5f);
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
                new Color(87 / 255f, 255 / 255f, 233 / 255f) * (1f - Projectile.alpha / 255f), 
                Projectile.rotation, origin, Projectile.scale * 1.2f, spriteEffects, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.Lime * (1f - Projectile.alpha / 255f),
                Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
            Vector2 velocity = Projectile.velocity;
            for (int i = 0; i < 20; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(18));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Terra, velocity.X, velocity.Y, 0, default(Color), 1f);
            }
        }
    }
}
