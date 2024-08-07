using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using TheTesseractMod.Dusts;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.Magic.EtherealStaffProjectile
{
    internal class HotMetalShard : ModProjectile
    {
        private int timer = 0;
        Color projectileColor = Color.White;
        private static readonly Color[] colors = {
            new Color(255, 255, 255),
            new Color (255, 119, 28),
            new Color(247, 30, 10),
            new Color(156, 147, 146)
        };
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16; //14
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 25;
            Projectile.scale = 2f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.98f;
           // Lighting.AddLight(Projectile.position, 255 / 255f, 48 / 255f, 20 / 255f);
           if (Projectile.ai[0] % 3 == 0)
            {
                Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, ModContent.DustType<MoltenSphereDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, projectileColor, 1f);
            }
            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            timer++;
            float fading = (timer % 60) / 30f;
            int index = (int)((timer / 60) % 4);
            int nextIndex = (index + 1) % 4;
            projectileColor = Color.Lerp(colors[index], colors[nextIndex], fading);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            Main.EntitySpriteDraw(texture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, texture.Width, texture.Height),
            new Color(projectileColor.R, projectileColor.G, projectileColor.B, 0) * (1f - Projectile.alpha / 255f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
        }
    }
}
