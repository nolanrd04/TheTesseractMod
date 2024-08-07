using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using TheTesseractMod.Dusts;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.Melee.EtherealLanceProjectiles
{
    internal class DeadlyIcicleFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.damage = 100;
            Projectile.alpha = 250;
            Projectile.timeLeft = 120;
            Projectile.light = 0.9f;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0.5f;
            Projectile.width = 32;
            Projectile.height = 32;
        }
        public float Lerp(float x, float y, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return x + amount * (y - x);
        }
        
        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            Lighting.AddLight(Projectile.position, 194/255f, 252/255f, 255 / 255f);
            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1.2f);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(223, 194, 255, 0) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Main.EntitySpriteDraw(texture, Projectile.position, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 3; i++)
            {
                Random rand = new Random();
                float rotation = (float)(rand.NextDouble() * 360);
                Vector2 velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, velocity.X, velocity.Y, 0, default(Color), 1.2f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RiftLightBlueDust>(), velocity.X, velocity.Y, 0, default(Color), 1f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
