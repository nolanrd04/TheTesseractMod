using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion.Level3Attacks
{
    internal class TerraDaggerLevel3 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Vector2 newVel = Projectile.velocity.RotatedBy(MathHelper.ToRadians(45f));
            Projectile.rotation = newVel.ToRotation();
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 120, Color.Lime, .7f);
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 offset = new Vector2(200f, 0);
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawnPos = Projectile.Center + offset.RotatedByRandom(2 * Math.PI);
                Vector2 direction = Projectile.Center - spawnPos;
                direction.Normalize();
                direction *= 3f;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, direction, ModContent.ProjectileType<TerraDaggerChild>(), Projectile.damage, 0f);
            }
        }
    }
}
