using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.TrueNightsWeapons
{
    internal class TrueNightsBulletExplosion : ModProjectile
    {
        const int frameSpeed = 4;
        int projectileFrameCounter;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 20;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.alpha = 75;
        }

        public override void AI()
        {
            Visuals();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0);
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
