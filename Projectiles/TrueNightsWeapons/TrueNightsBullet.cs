using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.TrueNightsWeapons
{
    internal class TrueNightsBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] % 2 == 0)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Terra, 0, 0, 0, default(Color), .6f);
            }
            Projectile.ai[0]++;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TrueNightsBulletExplosion>(), Projectile.damage / 2, 0);
            if (Main.rand.Next(4) == 0)
            {
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            return true;
        }
    }
}
