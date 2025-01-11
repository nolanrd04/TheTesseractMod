using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraYoyoProj : ModProjectile
    {
        private int hits = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 450f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 18f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4; // The length of old position to be recorded
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
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.ai[2]++;
            if (Projectile.ai[2] % 4 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, Color.Lime, .3f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 newVel = Projectile.velocity.RotateRandom(MathHelper.ToRadians(360)) / 3;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RadialGlowDust>(), newVel.X, newVel.Y, 0, Color.Lime, .3f);
            }

            hits++;
            if (hits % 10 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, new Vector2(7.5f, 0).RotatedByRandom(MathHelper.ToRadians(360)), ModContent.ProjectileType<TerraYoyoBurst>(), Projectile.damage / 2, Projectile.knockBack);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("TheTesseractMod/Textures/BlankYoyoProj").Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = new Color(Color.Green.R, Color.Green.G, Color.Green.B, .4f) * (1f - Projectile.alpha / 255f) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
