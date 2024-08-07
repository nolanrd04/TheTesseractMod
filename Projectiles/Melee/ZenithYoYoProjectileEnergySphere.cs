using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;

namespace TheTesseractMod.Projectiles.Melee
{
    internal class ZenithYoYoProjectileEnergySphere : ModProjectile
    {
        private int counter = 0;
        private float Alpha = 1f;
        private float movementSpeed = 4f;
        
    public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode

        }
        public override void SetDefaults()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                movementSpeed = 6f;
            }
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.aiStyle = -1;
            Projectile.timeLeft = 80;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            /*if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Projectile.localNPCHitCooldown = 8;
            }*/
        }

        public override bool PreDraw(ref Color lightColor)
        {
            counter++;
            if (counter > 68)
            {
                Alpha *= 0.8f;
            }
            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/ZenithYoYoProjectileGlow");
            Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                (new Color(213, 255, 115, 0) * (1f - Projectile.alpha / 255f)) * Alpha, Projectile.rotation, glowTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);


            Vector2 drawOrigin = new Vector2(glowTexture.Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(glowTexture.Value, drawPos, null, (new Color(213, 255, 115, 0) * (1f - Projectile.alpha / 255f)) * (Alpha / k), Projectile.rotation, drawOrigin, Projectile.scale - k * 0.1f, SpriteEffects.None, 0);
            }


            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect");
            Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                (new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f)) * Alpha, Projectile.rotation, glowTexture.Size() * 0.5f, 0.2f, SpriteEffects.None, 0f);
        }

        public override void PostAI()
        {
            Lighting.AddLight(Projectile.position, Color.White.ToVector3());
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * movementSpeed;
            //************************************//

            
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

                    if (distance < 200 && distance > 0)
                    {
                        distance = movementSpeed / distance;
                        goToX *= distance;
                        goToY *= distance;

                        Projectile.velocity.X += goToX / 2.5f; // higher int values make it turn slower
                        Projectile.velocity.Y += goToY / 2.5f;
                    }
                }
            
            //little push
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
