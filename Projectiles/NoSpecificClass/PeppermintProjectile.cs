using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.NoSpecificClass
{
    internal class PeppermintProjectile : ModProjectile
    {
        NPC lastHit = null;
        NPC secondLastHit = null;
        int nextTarget = -1;
        int hitTargetCount = 0;
        int detectRadius = 500;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.aiStyle = 0;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.light = 0.3f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitTargetCount++;
            secondLastHit = lastHit;
            lastHit = target;
            nextTarget = findTarget();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

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
            Projectile.ai[0]++;
            Projectile.rotation -= 0.4f;

            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20f;
            //************************************//

            if (hitTargetCount > 0)
            {
                NPC target = null;
                if (nextTarget != -1)
                {
                    target = Main.npc[nextTarget];
                }

                if (target.CanBeChasedBy() && !target.friendly && target.active && IsTargetValid(target))
                {
                    Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20;
                }
            }
            //little push
            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = 10f;
                Projectile.velocity.Y = 10f;
            }
        }
        public int findTarget() // returns the closest npc
        {
            Lighting.AddLight(Projectile.position, 1f, 1f, 1f);
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC && npc != lastHit)
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
        private bool IsTargetValid(NPC target) // a check to make sure the target exists, it is not behind a wall, and it was not the last hit npc.
        {
            bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height);
            float distance = Vector2.Distance(Projectile.position, target.position);
            bool canLockOnSecondLastHit;
            if (target == secondLastHit)
            {
                canLockOnSecondLastHit = distance > 10;
            }
            else
            {
                canLockOnSecondLastHit = true;
            }

            return target != null && target.active && !target.friendly && lineOfSight && target != lastHit && canLockOnSecondLastHit;
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
    }
}
