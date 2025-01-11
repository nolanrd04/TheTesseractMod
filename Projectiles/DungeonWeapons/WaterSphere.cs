using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.DungeonWeapons
{
    internal class WaterSphere : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/Vanilla/Water_Sphere";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 2;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(5);
            if (Projectile.ai[0] % 3 == 0 )
            {
                Dust.NewDust(Projectile.Center, 6, 6, 59, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1.5f);
            }
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 10, 10, 59, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1.5f);
                Main.dust[dust].noGravity = true;
            }
            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(lightColor),
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
