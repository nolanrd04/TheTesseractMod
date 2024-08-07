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
    public class SuperCulminationArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = 0; 
            Projectile.width = 27;
            Projectile.height = 27;
            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.scale = 1.4f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Ranged;
        }

        Color color = new Color(0, 255, 69);
        public override void AI()
        {
            
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.position, 0f, 1f, 0.3f);
            
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 74, Projectile.velocity.X, Projectile.velocity.Y, 150, color, 0.9f);
            Main.dust[dust2].noGravity = true;
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 74, Projectile.velocity.X * 0.95f, Projectile.velocity.Y * 0.95f, 150, color, 1f);
            }
            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 40; i ++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 74, Projectile.velocity.X *=0.95f, Projectile.velocity.Y*=0.95f, 150, color, 1f);
            }

            SoundEngine.PlaySound(SoundID.NPCDeath3, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            return true;
        }
    }

    
}
