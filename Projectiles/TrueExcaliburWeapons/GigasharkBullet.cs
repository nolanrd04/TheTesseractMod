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
using Terraria.DataStructures;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.TrueExcaliburWeapons
{
    internal class GigasharkBullet : ModProjectile
    {
        private Color bulletColor;
        private bool canHome = false;
        private NPC Target;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.rand.NextBool()) // PINK
            {
                bulletColor = new Color(255, 0, 217);
            }
            else // BLUE
            {
                bulletColor = new Color(0, 89, 255);
            }
        }

        public override void AI()
        {
            if (canHome)
            {
                Projectile.velocity = (Target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 15f;
            }


            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] % 8 == 0)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.velocity.X, Projectile.velocity.Y, 0, bulletColor, .7f);
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
                new Color(bulletColor.R, bulletColor.G, bulletColor.B, 0) * (1f - Projectile.alpha / 255f),
                Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Target = GlobalProjectileFunctions.findSecondClosestTarget(Projectile.Center);
            if (GlobalProjectileFunctions.IsTargetValid(Target, Projectile.Center, 150))
            {
                canHome = true;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            return true;
        }
    }
}
