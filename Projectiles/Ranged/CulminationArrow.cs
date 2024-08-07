using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.CodeAnalysis;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.Ranged
{
    public class CulminationArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = 1;
            Projectile.width = 13;
            Projectile.height = 13;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }

        Color color = new Color(222, 120, 255);
        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.position, 0.9f, 0.5f, 1f);
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 112, Projectile.velocity.X, Projectile.velocity.Y, 150, color, 0.8f);
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, Projectile.velocity.X, Projectile.velocity.Y, 150, color, 0.4f);
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
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 112, Projectile.velocity.X *= 0.9f, Projectile.velocity.Y *= 0.9f, 150, color, 0.8f);
            }

            SoundEngine.PlaySound(SoundID.NPCDeath3, Projectile.position);
            return true;
        }
    }


}
