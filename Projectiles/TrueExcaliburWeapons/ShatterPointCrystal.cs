using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;

namespace TheTesseractMod.Projectiles.TrueExcaliburWeapons
{
    internal class ShatterPointCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 70;
            Projectile.timeLeft = 60;
            Projectile.friendly = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.9f;
            Lighting.AddLight(Projectile.position, 250f / 255, 135f / 255, 250f / 255);

            Vector2 offsetPosition = Projectile.Center + new Vector2(Main.rand.Next(-15, 15), Main.rand.Next(-15, 15));
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, new ParticleOrchestraSettings { PositionInWorld = offsetPosition, MovementVector = Vector2.Zero });

            if (Projectile.ai[0] % 5 == 0)
            {
                Dust.NewDust(Projectile.position, 1, 1, 204, Projectile.velocity.X *= 0.98f, Projectile.velocity.Y *= 0.98f, 0, default(Color), 1f);
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PaladinsHammer, new ParticleOrchestraSettings { PositionInWorld = offsetPosition, MovementVector = Vector2.Zero });

            }
            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 180);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ModContent.ProjectileType<ShatterPointCrystalChild>(), Projectile.damage / 2, Projectile.knockBack);
            }
            for (int i = 0; i < 3; i++)
            {
                Vector2 offsetPosition = Projectile.Center + new Vector2(Main.rand.Next(-35, 35), Main.rand.Next(-35, 35));
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.TrueExcalibur, new ParticleOrchestraSettings { PositionInWorld = offsetPosition, MovementVector = Vector2.Zero });
            }
        }
    }
}
