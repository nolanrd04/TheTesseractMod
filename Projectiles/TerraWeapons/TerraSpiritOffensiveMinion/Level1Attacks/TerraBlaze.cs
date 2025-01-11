using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TheTesseractMod.GlobalFuncitons;
using Terraria.DataStructures;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion.Level1Attacks
{
    internal class TerraBlaze : ModProjectile
    {
        const int frameSpeed = 15;
        int projectileFrameCounter;
        float scaleVal_1 = 1f;
        float scaleVal_2 = -1;
        const int textureHeight = 30;
        float incrementalAngle = (float)Math.PI / 2;
        float velocity;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 3;
        }

        public override void OnSpawn(IEntitySource source)
        {
            velocity = Projectile.velocity.Length();
        }

        public override void AI()
        {
            Visuals();

            // --- Spawn fluctuating dust --- //
            float sineFactor = (float)(Math.Sin(incrementalAngle) * textureHeight / 2); // Adjust 0.1f to change fluctuation speed
            incrementalAngle += 0.1f;
            float rotation = Projectile.velocity.ToRotation();

            // Compute the offset for the top and bottom spawn points based on rotation
            Vector2 topOffset = new Vector2(0, -sineFactor).RotatedBy(rotation);
            Vector2 bottomOffset = new Vector2(0, sineFactor).RotatedBy(rotation);

            // Compute the actual spawn positions
            Vector2 topSpawnPosition = Projectile.Center + topOffset;
            Vector2 bottomSpawnPosition = Projectile.Center + bottomOffset;

            Dust topDust = Dust.NewDustPerfect(topSpawnPosition, DustID.GreenTorch, Projectile.velocity, 0, default(Color), 1.5f);
            topDust.noGravity = true;
            Dust bottomDust = Dust.NewDustPerfect(bottomSpawnPosition, DustID.GreenTorch, Projectile.velocity, 0, default(Color), 1.5f);
            bottomDust.noGravity = true;


            // --- Spawn bg dust --- //
            if (Main.rand.Next(3) == 0)
            {
                int terraDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Terra, Projectile.velocity.X, Projectile.velocity.Y, 160, default(Color), 1.5f);
                Main.dust[terraDust].noGravity = true;
                Main.dust[terraDust].velocity *= 0.2f;
            }
            if (Main.rand.Next(3) == 0)
            {
                int greenDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PureSpray, Projectile.velocity.X, Projectile.velocity.Y, 160, default(Color), 1.5f);
                Main.dust[greenDust].noGravity = true;
                Main.dust[greenDust].velocity *= 0.2f;
            }

            // --- Home --- //
            NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);

            if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, 300f))
            {
                Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .03f);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Vector2 velocity = Projectile.velocity;
            for (int i = 0; i < 20; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(18));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Terra, velocity.X, velocity.Y, 0, default(Color), 1f);
            }
        }

        private void Visuals()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }

            }
        }
    }
}
