using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Weapons.Melee.KPDH.Gok_do;
using TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Sain_geom;
using static tModPorter.ProgressUpdate;

namespace TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Gok_do
{
    internal class Gok_Do_ParryProj_Stage3 : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Gok_do/Gok_Do_Proj_Stage3_hiltOnly";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }


        private static readonly Color[] maskColors = {
            new Color(255, 203, 203), // light red
            new Color (255, 230, 204), // light orange
            new Color (254, 255, 204), // light yellow
            new Color (208, 255, 204), // light green
            new Color(204, 223, 255), // light blue
            new Color(231, 204, 255), // light purple
            new Color(255, 204, 243)  // light pink
        };

        Color maskColor;
        public override void SetDefaults()
        {
            Projectile.width = 129;
            Projectile.height = 135;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1.9f;
            Projectile.timeLeft = 30;
            Projectile.DamageType = DamageClass.Melee;

        }

        public override void AI()
        {
            if (Main.rand.Next(4) == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(10f, 0).RotatedByRandom(2 * Math.PI), ModContent.ProjectileType<Gok_Do_Star_Stage3>(), Projectile.damage, 0f);
            }

            Projectile.rotation += MathHelper.ToRadians(40f);

            Player owner = Main.player[Projectile.owner];
            Vector2 center = owner.RotatedRelativePoint(owner.MountedCenter);
            Projectile.Center = center;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (other.active && other.hostile)
                {
                    // If hitboxes overlap
                    if (Projectile.Hitbox.Intersects(other.Hitbox))
                    {
                        if (other.damage > 270)
                        {
                            other.damage = (int)(other.damage * 0.6f);
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
            // ***** Coloring: ***** //
            int numColors = maskColors.Length;
            float fade = (Main.GameUpdateCount % 30) / 30f;
            int index = (int)((Main.GameUpdateCount / 30) % numColors);
            int nextIndex = (index + 1) % numColors;

            maskColor = Color.Lerp(maskColors[index], maskColors[nextIndex], fade);

            // Draw hilt
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            // Draw glow mask
            Asset<Texture2D> glowMask = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Gok_do/Gok_Do_Proj_Stage3_glowMask");
            // Draw the spear
            Main.EntitySpriteDraw(
                glowMask.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                maskColor,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
