using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;

namespace TheTesseractMod.Projectiles.Summoner.ShadowFlameDragon
{
    internal class ShadowFlameDragonMinion : ModProjectile
    {
        float farSpeed = 60f;       // Temporary speed value used in distance calculations (So we don't change our original speed)
        float farInertia = 20f;     // Temporary intertia used in distance calculations (So we don't change our original interia)
        float attackSight = 600f;   // How far away an enemy must be for the minion to "see" it
        float idleRange = 60f;      // The range in which the minion will idle over the player
        float deadzoneRange = 40f;  // The deadzone range in which the minion will not latch onto an enemy
        int attackStage = 0;        // the type of attack this minion will use. 1 is dash, 0 is breath fire.
        int attackStageCounter;     //the counter that keeps track of how long the minion has been on an attack stage
        float rotation = MathHelper.ToRadians(Main.rand.Next(360)); // For attack stage 0. The minion will circle the target at a certain distance and breathe fire.
        bool dashing = false;       // used for attack stage 1. Checking to see if the minion is currently dashing
        Vector2 dashDirection = Vector2.Zero;        // used to fix the direction of dash is attack() updates every tick
        int dashCount = 0;          // after 3 dashes, stop dashing.
        int idleCounter = 0;        // For keeping track of how long a minion is idle for. After a certain time the attack stage will revert.

        //------------------------------------------------------------------------------------------------------------------------

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public sealed override void SetDefaults()
        {
            Projectile.scale = 1.7f;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false; 
            Projectile.friendly = true; 
            Projectile.minion = true; 
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

       
        public override bool MinionContactDamage()
        {
            return true;
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            rotation = MathHelper.ToDegrees(rotation);
            rotation++;
            rotation = MathHelper.ToRadians(rotation);

            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out NPC target);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition, target);
            Visuals();
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<ShadowFlameDragonBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<ShadowFlameDragonBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;

            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX;

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.08f;

            // Fix overlap with other minions
            foreach (var other in Main.ActiveProjectiles)
            {
                if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out NPC target)
        {
            // Starting search distance
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;
            target = null;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 600f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                    target = npc;
                }
            }

            if (!foundTarget)
            {
                // find target
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if ((closest && inRange) || !foundTarget)
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                            target = npc;
                        }
                    }
                }
            }
            Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition, NPC target)
        {
            // Default movement parameters (here for attacking)
            float speed = 17f;
            float inertia = 10f;
            Vector2 goToPosition;
            float distanceFromPosition;

            if (foundTarget)
            {
                if (attackStage == 0) // FIRE BREATH STAGE
                {
                    goToPosition = targetCenter + new Vector2(100, 0).RotatedBy(rotation); // The desired position is a spot 100 pixels away at a random angle.
                    distanceFromPosition = Vector2.Distance(Projectile.Center, goToPosition);

                    if (distanceFromPosition > 100f) // go to desired position
                    {
                        Vector2 direction = goToPosition - Projectile.Center;
                        direction.Normalize();
                        direction *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

                        if (target.boss && attackStageCounter % 14 == 0) // if its a boss, breathe fire anyways.
                        {
                            direction = targetCenter - Projectile.Center;
                            direction.Normalize();
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction * 10f, ModContent.ProjectileType<ShadowFlameBreath>(), Projectile.damage, 0f);
                            SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                        }
                    }
                    else // within desired range, so breath fire
                    {
                        // spawn fire breath
                        if (attackStageCounter % 14 == 0)
                        {
                            Vector2 direction = targetCenter - Projectile.Center;
                            direction.Normalize();
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction * 10f, ModContent.ProjectileType<ShadowFlameBreath>(), Projectile.damage, 0f);
                            SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                        }
                    }
                    attackStageCounter++;
                    // after time, enter dash phase
                    if (attackStageCounter > 200)
                    {
                        attackStageCounter = 0;
                        attackStage++;
                    }
                }
                else // DASH STAGE
                {
                    distanceFromPosition = Vector2.Distance(Projectile.Center, targetCenter);

                    // first, check if dashing to avoid idle movement
                    if (dashing)
                    {

                        Projectile.usesLocalNPCImmunity = true;
                        Projectile.localNPCHitCooldown = 3;

                        Vector2 direction = targetCenter - Projectile.Center;
                        direction.Normalize();
                        if (attackStageCounter == 0)
                        {
                            dashDirection = direction * 25f;
                        }
                        Projectile.velocity = dashDirection;
                        attackStageCounter++;
                        
                        if (attackStageCounter > 10)
                        {
                            dashCount++;
                            dashing = false; // stop dashing
                            attackStageCounter = 0;
                        }
                    }

                    else // if not dashing, plot new dash course or switch stages.
                    {
                        if (distanceFromPosition > 90f) // get within range
                        {
                            Vector2 direction = targetCenter - Projectile.Center;
                            direction.Normalize();
                            direction *= speed;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                        }
                        else // when in range, dash
                        {
                            dashing = true;
                        }
                    }
                    if (dashCount >= 3)
                    {
                        Projectile.usesLocalNPCImmunity = false;
                        attackStage--;
                        dashCount = 0;
                        dashing = false;
                    }
                }
            }
            else
            {
                // reset attack stage after being idle
                idleCounter++;
                if (idleCounter > 10)
                {
                    attackStage = 0;
                    dashing = false;
                }

                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 40f;
                    inertia = 200f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 4f;
                    inertia = 80f;
                }

                if (distanceToIdlePosition > 20f)
                {
                    // The immediate range around the player (when it passively floats about)

                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
        }

        private void Visuals()
        {
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.X >= 0)
            {
                Projectile.spriteDirection = -1;
            }
            else
            {
                Projectile.spriteDirection = 1;
            }
            
            int frameSpeed = 4;
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
            Lighting.AddLight(Projectile.Center, 1f, 1f, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;


            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            float offsetX = 24f;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            // draw back sprites only if dashing
            if (dashing) 
            {
                Texture2D bgTexture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Summoner/ShadowFlameDragon/ShadowFlameDragonMinionBG").Value;
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + origin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(bgTexture, drawPos, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            // draw main sprite
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}
