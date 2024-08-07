using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;

namespace TheTesseractMod.Projectiles.Melee.EtherealSwordProjectiles
{
    internal class EtherealSwordSlash : ModProjectile
    {
        private bool descending = true;
        private Color color1 = new Color(212, 168, 255);
        private Color color2 = new Color(254, 255, 168);

        private static Random rand = new Random();
        private int colorPicker = rand.Next(2);
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 120;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 20;
            Projectile.scale = 0.45f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0 && descending)
            {
                Projectile.alpha -= 12;
                Projectile.scale += 0.02f;
            }
            if (Projectile.alpha == 0)
            {
                descending = false;
            }
            if (!descending)
            {
                Projectile.alpha += 12;
                Projectile.scale -= 0.02f;
            }
            Projectile.velocity *= 0.95f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*Player player = Main.player[Projectile.owner];
            float targetAngle = (Main.MouseWorld - player.MountedCenter).ToRotation();*/
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/EtherealSwordProjectiles/EtherealSwordSlash");

            if (colorPicker == 0)
            {
                Lighting.AddLight(Projectile.position, color1.ToVector3());
                Main.spriteBatch.Draw(texture.Value,
                    new Vector2(Projectile.position.X - Main.screenPosition.X, Projectile.position.Y - Main.screenPosition.Y),
                    new Rectangle(0, 0, texture.Value.Width, texture.Value.Height),
                   new Color(color1.R, color1.G, color1.B, 0) * (1f - Projectile.alpha / 120f), Projectile.rotation + (float)(Math.PI / 2), texture.Value.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }

            if (colorPicker == 1)
            {
                Lighting.AddLight(Projectile.position, color2.ToVector3());
                Main.spriteBatch.Draw(texture.Value,
                    new Vector2(Projectile.position.X - Main.screenPosition.X, Projectile.position.Y - Main.screenPosition.Y),
                    new Rectangle(0, 0, texture.Value.Width, texture.Value.Height),
                   new Color(color2.R, color2.G, color2.B, 0) * (1f - Projectile.alpha / 120f), Projectile.rotation + (float)(Math.PI / 2), texture.Value.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
