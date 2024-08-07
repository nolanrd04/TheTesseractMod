using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Global.Projectiles.Ranged;
using TheTesseractMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Magic.EtherealTomeProjectiles
{    internal class RainDrop : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }
        public override void AI()
        {
            Projectile.ai[0]++;
            Lighting.AddLight(Projectile.Center, 0f, 0f, 1f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] % 3 == 0)
            {
                Dust.NewDust(Projectile.Center, 15, 15, 172, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 200, default(Color), 1f);
                Dust.NewDust(Projectile.Center, 15, 15, DustID.Shadowflame, Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 225, default(Color), 1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.Center, 10, 10, 33, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1f);
            }
        }
    }
}
