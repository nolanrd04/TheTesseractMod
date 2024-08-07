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
    internal class MoltenSphere : ModProjectile
    {
        private float rotationFactor = 8f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 125;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
           // Lighting.AddLight(Projectile.position, 255 / 255f, 48 / 255f, 20 / 255f);
            Dust.NewDust(Projectile.position, Projectile.width/2, Projectile.height/2, 6, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            rotationFactor += 0.5f;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, rotationFactor, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture,
                (Projectile.position - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY),
                null,
                new Color(255f, 255f, 255f, 0) * (1f - Projectile.alpha / 255f), rotationFactor, drawOrigin, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Main.EntitySpriteDraw(texture,
                (Projectile.position - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY),
                null,
                new Color(255f, 255f, 255f, 0) * (1f - Projectile.alpha / 255f), rotationFactor, drawOrigin, 1f, SpriteEffects.None, 0f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            float rotation = 0f;
            for (int i = 0; i < 16; i ++)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, new Vector2(4f, 4f).RotatedBy(MathHelper.ToRadians(rotation)), ModContent.ProjectileType<HotMetalShard>(), Projectile.damage/3, Projectile.knockBack);
                rotation += 22.5f;
            }
        }
    }
}
