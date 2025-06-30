using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsBullet : ModProjectile
    {
        private int bounces = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.Purple.R / 255f, Color.Purple.G / 255f, Color.Purple.B / 255f);
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
                Color.White,
                Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            if (bounces < 1)
            {
                // If the projectile hits the left or right side of the tile, reverse the X velocity
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }

                for (int i = 0; i < 3; i ++)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))) / 2f, ModContent.ProjectileType<NightsBulletShard>(), Projectile.damage/2, Projectile.knockBack);
                }

                bounces++;
                return false;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(4) == 0)
            {
                target.AddBuff(BuffID.ShadowFlame, 60);
            }

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))) / 2f, ModContent.ProjectileType<NightsBulletShard>(), Projectile.damage / 2, Projectile.knockBack);
            }
        }
    }
}
