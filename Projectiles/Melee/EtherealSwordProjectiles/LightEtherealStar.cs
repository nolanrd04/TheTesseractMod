using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace TheTesseractMod.Projectiles.Melee.EtherealSwordProjectiles
{
    internal class LightEtherealStar : ModProjectile
    {
        private float scalingFactor;
        private float rotationFactor = 8;
        private int timer;
        private float begin = 0.2f;
        private float end = 0.35f;
        private float travelingSpeed = 10f;

        private float smallerBegin = .15f;
        private float smallerEnd = 0.3f;
        private float smallerScalingFactor;
        private float smallerRotationFactor;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 130;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            timer++;
            float fade = (float)Math.Sin(timer * MathHelper.TwoPi / 120f);
            fade = (fade + 1f) / 2f;
            scalingFactor = Lerp(0.25f, 0.5f, fade);

            rotationFactor += 8f;

            Asset<Texture2D> Texture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/EtherealSwordProjectiles/LightEtherealStar");
            Main.EntitySpriteDraw(Texture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, Texture.Value.Width, Texture.Value.Height),
                new Color(255, 252, 153, 0) * (1f - Projectile.alpha / 255f), rotationFactor, Texture.Size() * 0.5f, scalingFactor * 0.25f, SpriteEffects.None, 0f);

            /*Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f), rotationFactor, glowTexture.Size() * 0.5f, scalingFactor * 0.75f * 0.75f, SpriteEffects.None, 0f);*/

            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, new Vector3(255/255f, 252/255f, 153/255f));
            Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 204, Projectile.velocity.X*0.9f, Projectile.velocity.Y * 0.9f, 150, default(Color), 1.5f);
            Projectile.ai[0]++;
            
            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * travelingSpeed;
            //************************************//
            if (Projectile.ai[0] > 10)
            {
                NPC target = null;
                if (findTarget() != -1)
                {
                    target = Main.npc[findTarget()];
                }
                if (!IsTargetValid(target))
                {
                    Projectile.velocity = Projectile.oldVelocity;
                }

                if (target.CanBeChasedBy() && !target.friendly && target.active && IsTargetValid(target))
                {
                    /*homing segment*/
                    float goToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                    float goToY = target.position.Y + (float)target.width * 0.5f - Projectile.Center.Y;
                    float distance = (float)Math.Sqrt(goToX * goToX + goToY * goToY);

                    if (distance < 300 && distance > 0)
                    {
                        distance = 4f / distance;
                        goToX *= distance;
                        goToY *= distance;

                        Projectile.velocity.X += goToX; // higher int values make it turn slower
                        Projectile.velocity.Y += goToY;
                    }
                }
            }
            travelingSpeed *= 0.99f;
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
        public float Lerp(float x, float y, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return x + amount * (y - x);
        }
    }
}
