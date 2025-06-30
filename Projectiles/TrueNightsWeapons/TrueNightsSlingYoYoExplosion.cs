using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;

namespace TheTesseractMod.Projectiles.TrueNightsWeapons
{
    internal class TrueNightsSlingYoYoExplosion : ModProjectile
    {
        const int frameSpeed = 4;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 28;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            Vector2 velocity = new Vector2(4f, 0);
            for (int i = 0; i < 20; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(18));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Terra, velocity.X, velocity.Y, 0, default(Color), 1f);
            }
        }

        public override void AI()
        {
            Visuals();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        private void Visuals()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }

            }
        }
    }
}
