using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion
{
    internal class TerraDaggerChild : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.alpha = 75;
            Projectile.scale = .75f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Vector2 newVel = Projectile.velocity.RotatedBy(MathHelper.ToRadians(45f));
            Projectile.rotation = newVel.ToRotation();
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 170, Color.Lime, .5f);
            if (Projectile.ai[0] > 120)
            {
                Projectile.alpha += 1;
            }
            if (Projectile.alpha > 255)
            {
                Projectile.Kill();
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
                Color.White,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
