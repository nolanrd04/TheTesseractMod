using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.Drawing;

namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] % 3 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Terra, Vector2.Zero, 0, default(Color), .5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Vector2 velocity = new Vector2(5f, 0);
            int count = Main.rand.Next(6) + 10;
            float deg = 360 / count;
            for (int i = 0; i < count; i++)
            {
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Terra, velocity.X, velocity.Y, 0, default(Color), .5f);
                velocity = velocity.RotatedBy(deg * i);
            }
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.TerraBlade, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.TerraBlade, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });

        }
    }
}
