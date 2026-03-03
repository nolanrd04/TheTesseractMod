using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Gok_do
{
    internal class Gok_Do_ParryProj_Stage1 : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Gok_do/Gok_Do_Proj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 129;
            Projectile.height = 135;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1.5f;
            Projectile.timeLeft = 30;
            Projectile.DamageType = DamageClass.Melee;

        }

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(40f);

            Player owner = Main.player[Projectile.owner];
            Vector2 center = owner.RotatedRelativePoint(owner.MountedCenter); 
            Projectile.Center = center;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                // Valid hostile projectile?
                if (other.active && other.hostile)
                {
                    // If hitboxes overlap
                    if (Projectile.Hitbox.Intersects(other.Hitbox))
                    {
                        if(other.damage > 300)
                        {
                            other.damage = (int)(other.damage * 0.8f);
                            other.netUpdate = true;
                        }

                        else
                        {
                            other.Kill();
                        }
                    }
                }
            }
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
