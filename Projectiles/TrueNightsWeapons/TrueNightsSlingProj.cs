using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.TrueNightsWeapons
{
    internal class TrueNightsSlingProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 368f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16f;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }

        public override void AI()
        {
            Projectile.ai[2]++;
            if (Projectile.ai[2] % 3 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0, 0, 0, default(Color), 2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(4) == 0)
            {
                target.AddBuff(BuffID.ShadowFlame, 120);
            }
            else if (Main.rand.Next(4) == 1)
            {
                target.AddBuff(BuffID.CursedInferno, 120);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("TheTesseractMod/Textures/BlankYoyoProj").Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = new Color(Color.Lime.R, Color.Lime.G, Color.Lime.B, .4f) * (1f - Projectile.alpha / 255f) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
