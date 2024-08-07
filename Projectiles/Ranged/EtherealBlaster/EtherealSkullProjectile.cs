using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
namespace TheTesseractMod.Projectiles.Ranged.EtherealBlaster
{
    internal class EtherealSkullProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.8f;
            Projectile.scale = 2f;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 0f, 1f, 1f);
            Dust.NewDust(Projectile.Center, 4, 4, DustID.SolarFlare, 0, 0, 115, default(Color), 0.75f);
            Dust.NewDust(Projectile.Center, 4, 4, 180, 0, 0, 115, default(Color), 0.75f);
            
            Projectile.ai[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 15f;
            //************************************//

            NPC target = Main.npc[findTarget()];

            if (target.CanBeChasedBy() && !target.friendly && target.active && IsTargetValid(target))
            {
                /*homing segment*/
                float goToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                float goToY = target.position.Y + (float)target.width * 0.5f - Projectile.Center.Y;
                float distance = (float)Math.Sqrt(goToX * goToX + goToY * goToY);

                if (distance < 400)
                {
                    distance = 4f / distance;
                    goToX *= distance;
                    goToY *= distance;

                    Projectile.velocity.X += goToX / 2; // higher int values make it turn slower
                    Projectile.velocity.Y += goToY / 2;
                }
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.velocity.X < 0)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.Center, 25, 25, DustID.SolarFlare);
            }

            for (int i = 0; i < 20; i ++)
            {
                Dust.NewDust(Projectile.Center, 25, 25, 180);
            }

            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))) / 5, ModContent.ProjectileType<GlowRiftProjectileFriendly>(), Projectile.damage, Projectile.knockBack);
            }
            SoundEngine.PlaySound(SoundID.Item107, Projectile.position);
        }
        public int findTarget() // returns the closest npc
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC)
                {
                    float distance = Vector2.Distance(Projectile.position, npc.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }
            }
            return closestNPCIndex;
        }

        private bool IsTargetValid(NPC target) // a check to make sure the target exists
        {
            return target != null && target.active && !target.friendly;
        }
    }
}
