using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs.MinionBuffs;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.Summoner
{
    internal class SquidOfTheAbyssMinion : ModProjectile
    {
        // --- Scaling --- //
        // Tentacle count = minionSlots (1:1)
        // Damage increases 3% per extra slot (additive on base)
        // Scale grows slightly per slot
        // Tentacle range grows per slot
        private int TentacleCount => (int)Projectile.minionSlots;
        private float TentacleRange => 150f + (Projectile.minionSlots - 1) * 35f;
        private float DamageMultiplier => 1f + (Projectile.minionSlots - 1) * 0.03f;

        // --- Targeting --- //
        private NPC mainTarget;
        private int targetRefreshTimer = 0;
        private const int TARGET_REFRESH_DELAY = 60;
        private const float DETECTION_RANGE = 900f;

        // --- Movement --- //
        private float movementSpeed = 20f;
        private float inertia = 16f;

        // --- Animation --- //
        private int frameCounter = 0;
        private bool isMoving = false;
        private bool isAttacking = false;
        private bool isMovingToTarget = false;

        // --- Tentacle tracking --- //
        private List<int> activeTentacles = new List<int>();

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
        }

        public override bool? CanCutTiles() => true;
        public override bool MinionContactDamage() => false;

        private float GetTentacleDamageMultiplier(int tentacleCount)
        {
            return (1f + (1 / tentacleCount)) * DamageMultiplier;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<SquidOfTheAbyssMinionBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<SquidOfTheAbyssMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!CheckActive(owner))
                return;

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.8f);

            // Scale size based on slots
            Projectile.scale = 1f + (Projectile.minionSlots - 1) * 0.35f;

            // Target selection (refreshes every 60 frames)
            targetRefreshTimer++;
            if (targetRefreshTimer >= TARGET_REFRESH_DELAY || mainTarget == null || !mainTarget.active || !mainTarget.CanBeChasedBy())
            {
                UpdateTarget(owner);
                targetRefreshTimer = 0;
            }

            // Behavior
            if (mainTarget != null && mainTarget.active)
            {
                AttackBehavior(owner);
            }
            else
            {
                IdleBehavior(owner);
            }

            // Manage tentacles
            ManageTentacles(owner);

            // Rotation: face movement direction if moving to target, otherwise slight tilt
            if (Projectile.velocity.LengthSquared() > 1f)
            {
                isMoving = true;
                
                if (isMovingToTarget)
                {
                    // Moving to target: rotate fully to face movement direction
                    float targetRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.15f);
                }
                else
                {
                    // Idle movement: slight tilt in movement direction
                    float movementTilt = Projectile.direction * 0.2f; // tilt factor
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, movementTilt, 0.15f);
                }
            }
            else
            {
                isMoving = false;
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, 0f, 0.1f);
            }

            // Visuals
            UpdateAnimation();
        }

        private void UpdateTarget(Player owner)
        {
            // Priority: whip target > closest NPC
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC whipTarget = Main.npc[owner.MinionAttackTargetNPC];
                if (whipTarget.active && whipTarget.CanBeChasedBy())
                {
                    mainTarget = whipTarget;
                    return;
                }
            }

            mainTarget = GlobalProjectileFunctions.findClosestTargetInRange(Projectile.Center, DETECTION_RANGE);
        }

        private void AttackBehavior(Player owner)
        {
            isAttacking = true;

            // Find NPCs within tentacle range for KNN-style centering
            List<NPC> nearbyTargets = GetNearbyTargets();

            // Calculate centroid of all reachable targets
            Vector2 centroid = mainTarget.Center;
            if (nearbyTargets.Count > 1)
            {
                centroid = Vector2.Zero;
                foreach (NPC npc in nearbyTargets)
                {
                    centroid += npc.Center;
                }
                centroid /= nearbyTargets.Count;
            }

            // Clamp centroid so we stay within tentacle range of the main target
            Vector2 desiredPosition = centroid;
            if (Vector2.Distance(centroid, mainTarget.Center) > TentacleRange * 0.7f)
            {
                Vector2 dirToTarget = (mainTarget.Center - centroid).SafeNormalize(Vector2.Zero);
                desiredPosition = mainTarget.Center - dirToTarget * TentacleRange * 0.5f;
            }
            else if (nearbyTargets.Count == 1)
            {
                // Single target: maintain distance instead of moving into it
                Vector2 dirFromTarget = (Projectile.Center - mainTarget.Center).SafeNormalize(Vector2.Zero);
                float preferredDistance = TentacleRange * 0.5f;
                desiredPosition = mainTarget.Center + dirFromTarget * preferredDistance;
            }

            // Only lock into movement mode if far from desired position
            float distanceToDesiredPosition = Vector2.Distance(Projectile.Center, desiredPosition);
            isMovingToTarget = distanceToDesiredPosition > 100f;

            MoveToward(desiredPosition);

            // Teleport if way too far from owner
            if (Vector2.Distance(Projectile.Center, owner.Center) > 2000f)
            {
                Projectile.position = owner.Center - Projectile.Size / 2f;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }

        private void IdleBehavior(Player owner)
        {
            isAttacking = false;
            isMovingToTarget = false;

            Vector2 idlePosition = owner.Center + new Vector2(0, -100f);

            if (Vector2.Distance(Projectile.Center, idlePosition) > 2000f)
            {
                Projectile.position = idlePosition - Projectile.Size / 2f;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            MoveToward(idlePosition);
            CleanupTentacles();
        }

        private void MoveToward(Vector2 target)
        {
            float dist = Vector2.Distance(Projectile.Center, target);
            if (dist > 20f)
            {
                Vector2 direction = (target - Projectile.Center).SafeNormalize(Vector2.Zero);
                float speed = dist > 600f ? movementSpeed * 2.5f : movementSpeed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction * speed) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = Vector2.Zero;
            }
        }

        // --- KNN-inspired: find all valid NPCs within tentacle range of the squid --- //
        private List<NPC> GetNearbyTargets()
        {
            List<NPC> targets = new List<NPC>();
            if (mainTarget != null && mainTarget.active)
            {
                targets.Add(mainTarget);
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc != null && npc.active && npc.CanBeChasedBy() && npc != mainTarget)
                {
                    if (Vector2.Distance(Projectile.Center, npc.Center) <= TentacleRange)
                    {
                        targets.Add(npc);
                    }
                }
            }
            return targets;
        }

        // --- Tentacle management --- //
        private void ManageTentacles(Player owner)
        {
            if (!isAttacking || mainTarget == null || !mainTarget.active)
            {
                CleanupTentacles();
                return;
            }

            // Only manage tentacles if within attack range
            if (Vector2.Distance(Projectile.Center, mainTarget.Center) > TentacleRange)
            {
                CleanupTentacles();
                return;
            }

            // Remove dead tentacles from tracking list
            activeTentacles.RemoveAll(id =>
                id < 0 || id >= Main.maxProjectiles ||
                !Main.projectile[id].active ||
                Main.projectile[id].type != ModContent.ProjectileType<SquidOfTheAbyssMinionTentacle>());

            // Get nearby targets
            List<NPC> nearbyTargets = GetNearbyTargets();
            int tentacleCount = TentacleCount;

            // Build assignment list: distribute tentacles across NPCs, extras wrap via modular loop
            List<int> targetAssignments = new List<int>();
            for (int i = 0; i < tentacleCount; i++)
            {
                if (nearbyTargets.Count > 0)
                {
                    int idx = i % nearbyTargets.Count;
                    targetAssignments.Add(nearbyTargets[idx].whoAmI);
                }
            }

            if (Main.myPlayer != owner.whoAmI)
                return;

            // Kill excess tentacles
            while (activeTentacles.Count > tentacleCount)
            {
                int last = activeTentacles[activeTentacles.Count - 1];
                if (last >= 0 && last < Main.maxProjectiles && Main.projectile[last].active)
                {
                    Main.projectile[last].Kill();
                }
                activeTentacles.RemoveAt(activeTentacles.Count - 1);
            }

            // Update existing tentacle targets
            for (int i = 0; i < activeTentacles.Count && i < targetAssignments.Count; i++)
            {
                Projectile tentacle = Main.projectile[activeTentacles[i]];
                if (tentacle.active)
                {
                    tentacle.ai[1] = targetAssignments[i];
                }
            }

            // Spawn new tentacles if needed
            while (activeTentacles.Count < targetAssignments.Count)
            {
                int idx = activeTentacles.Count;
                int projId = Projectile.NewProjectile(
                    Projectile.InheritSource(Projectile),
                    Projectile.Center + new Vector2(0, Projectile.height / 2f), // spawn at bottom of squid
                    Vector2.Zero,
                    ModContent.ProjectileType<SquidOfTheAbyssMinionTentacle>(),
                    Projectile.damage,
                    0f,
                    Projectile.owner,
                    Projectile.whoAmI,        // ai[0] = parent squid index
                    targetAssignments[idx]    // ai[1] = target NPC index
                );
                if (projId >= 0)
                {
                    activeTentacles.Add(projId);
                }
            }
        }

        private void CleanupTentacles()
        {
            foreach (int id in activeTentacles)
            {
                if (id >= 0 && id < Main.maxProjectiles && Main.projectile[id].active)
                {
                    Main.projectile[id].Kill();
                }
            }
            activeTentacles.Clear();
        }

        // --- Animation --- //
        // 8 frames: all 8 while idle, lock frame 5 while moving to enemy, all 8 while attacking
        private void UpdateAnimation()
        {
            frameCounter++;
            if (!isMovingToTarget)
            {
                // Idle: cycle all 8 frames
                if (frameCounter % 5 == 0)
                {
                    Projectile.frame++;
                    if (Projectile.frame >= 8)
                        Projectile.frame = 0;
                }
            }
            else
            {
                // Moving to target enemy: lock frame 5
                Projectile.frame = 5;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle sourceRect = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 origin = new Vector2(texture.Width * 0.5f, frameHeight * 0.5f);

            SpriteEffects effects = Projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition,
                sourceRect,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0f);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            CleanupTentacles();
        }
    }
}