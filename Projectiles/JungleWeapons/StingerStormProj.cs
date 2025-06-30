using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.JungleWeapons
{
    internal class StingerStormProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 2 == 0)
            {
                Dust.NewDust(Projectile.Center, 1, 1, 7, 0, 0, 0, default(Color), 0.7f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture,
            new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
            new Rectangle(0, 0, texture.Width, texture.Height),
            Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 180);
        }
    }
}
