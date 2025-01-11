using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Buffs.MinionBuffs;
using TheTesseractMod.Dusts;
using TheTesseractMod.GlobalFuncitons;
using TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion.Level1Attacks;
using TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion.Level2Attacks;
using TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion.Level3Attacks;


namespace TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion
{
    // FRAMES: 
    // > for tiki:
    // > > Frame 0-1 idle
    // > > Frame 2: magic attack
    // > > Frame 3-5: melee attack
    // > for flames:
    // > > Frame 1-8: animation
    
    // ATTACK STAGES:
    // 0: idle
    // 1: magic 1
    // 2: magic 2
    // 3: melee

    internal class TerraSpiritOffenseMinion : ModProjectile
    {

        int phase = 0;
        float attackSight = 900f;
        float movementSpeed = 16f;
        float projectileSpeed = 3f;
        NPC target;
        float inertia = 16f;

        int idleCounter = 0;
        int timeForPhase = 0;
        int frameCounter;

        bool attacking = false;
        bool dashing = false;
        bool hitNPC = false;
        bool targetMarked = false;
        float slotsOccupied;
        int minionLevel; // 0-3 lvl 1, 4-6 lvl 2, 7+ lvl 3
        

        // flame
        Rectangle flameFrame = new Rectangle(0, 0, 22, 62);
        const int flameFrames = 8;
        int flameCounter;
        float rotation = MathHelper.ToRadians(Main.rand.Next(360));


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public sealed override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 22;
            Projectile.height = 62;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool MinionContactDamage()
        {
            return true;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                 owner.ClearBuff(ModContent.BuffType<TerraSpiritMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<TerraSpiritMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public override void AI()
        {
            targetMarked = false;
            // ai[0]: constantly counts up
            // ai[1]: acts as timer for attacks and the attack animations
            // ai[2]: for collision

            Player owner = Main.player[Projectile.owner];
            Projectile.friendly = false;
            
            LevelHandler(owner);

            if (!CheckActive(owner))
            {
                return;
            }

            // get the target
            if (owner.HasMinionAttackTargetNPC)
            {
                target = Main.npc[owner.MinionAttackTargetNPC];
            }
            else
            {
                target = GlobalProjectileFunctions.findClosestTargetInRange(owner.Center, attackSight);
            }

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc != null && npc.active && npc.HasBuff(ModContent.BuffType<TargetMarked>()) && npc.type != NPCID.TargetDummy)
                {
                    target = npc;
                    targetMarked = true;
                    break;
                }
            }

            // Behavior
            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);

            // Movement
            if (target != null)
            {
                idleCounter = 0;
                if (targetMarked)
                {
                    phase = 3;
                }

                else if (phase == 0)
                {
                    phase = Main.rand.Next(1, 4);
                }
                if (Projectile.ai[0] % 300 == 0)
                {
                    phase = Main.rand.Next(1, 4);
                }
                
                
                MovementAndAttack(owner, vectorToIdlePosition, distanceToIdlePosition);
            }
            else
            {
                Movement(owner, vectorToIdlePosition, distanceToIdlePosition);
            }
            // Visuals
            Visuals();
            Projectile.ai[0]++;
            Projectile.ai[1]++;
            if (phase == 1)
            {
                if (Projectile.ai[1] > 20 )
                {
                    Projectile.ai[1] = 0;
                    attacking = false;
                }
            }
            else if (phase == 2)
            {

                if (Projectile.ai[1] > 60)
                {
                    Projectile.ai[1] = 0;
                    attacking = false;
                }
            }
            else if (phase == 3)
            {

                if (Projectile.ai[1] > 60)
                {
                    Projectile.ai[1] = 0;
                    attacking = false;
                }
            }

            // collision reset
            if (hitNPC)
            {
                Projectile.ai[2]++;
            }
            if (Projectile.ai[2] > 30)
            {
                dashing = false;
                attacking = false;
                hitNPC = false;
                Projectile.ai[2] = 0;
            }
        }
        private void Movement(Player owner, Vector2 vectorToIdlePosition, float distanceToIdlePosition)
        {
            Projectile.friendly = false;
            // reset attack stage after being idle
            idleCounter++;
            float ogSpeed = movementSpeed;
            float ogInertia = inertia;

            if (idleCounter > 10)
            {
                phase = 0;

            }
            // Minion doesn't have a target: return to player and idle
            if (distanceToIdlePosition > 600f)
            {
                // Speed up the minion if it's away from the player
                movementSpeed = 40f;
                inertia = 20f;
            }
            else
            {
                // Slow down the minion if closer to the player
                movementSpeed = ogSpeed;
                inertia = ogInertia;
            }

            if (distanceToIdlePosition > 20f)
            {
                inertia = 25f;
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= movementSpeed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                // If there is a case where it's not moving at all, give it a little "poke"
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
        }

        private void MovementAndAttack(Player owner, Vector2 vectorToIdlePosition, float distanceToIdlePosition)
        {
            if (target == null)
            {
                return;
            }
            Vector2 targetCenter = target.Center;
            // handle rotation offset
            float deg = MathHelper.ToDegrees(rotation);
            if (phase == 2 || phase == 3)
            {
                deg++;
            }
            else if (phase == 1)
            {
                deg += 5;
            }
            rotation = MathHelper.ToRadians(deg);

            // handle movement for all phases except idle.

            if (phase == 1) // attack magic 1
            {
                Vector2 goToPosition = targetCenter + new Vector2(100, 0).RotatedBy(rotation); // The desired position is a spot 100 pixels away at a random angle.
                float distanceFromTarget = Vector2.Distance(Projectile.Center, goToPosition);

                // first checks to see how to move
                if (distanceFromTarget > 100f)
                {
                    Vector2 direction = goToPosition - Projectile.Center;
                    direction.Normalize();
                    direction *= movementSpeed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }

                if (distanceFromTarget <= 350f && Projectile.ai[1] == 0)
                {
                    Vector2 direction = targetCenter - Projectile.Center;
                    Attack();
                    attacking = true;
                }
            }
            else if (phase == 2) // attack magic 2
            {
                Vector2 goToPosition = targetCenter + new Vector2(250, 0).RotatedBy(rotation); // The desired position is a spot 100 pixels away at a random angle.
                float distanceFromTarget = Vector2.Distance(Projectile.Center, goToPosition);

                // first checks to see how to move
                if (distanceFromTarget > 250f)
                {
                    Vector2 direction = goToPosition - Projectile.Center;
                    direction.Normalize();
                    direction *= movementSpeed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }

                if (distanceFromTarget <= 500f && Projectile.ai[1] == 0)
                {
                    Vector2 direction = targetCenter - Projectile.Center;
                    Attack();
                    attacking = true;
                }
            }
            else if (phase == 3) // attack melee
            {
                if (targetMarked)
                {
                    targetCenter = target.Center;
                    Vector2 goToPosition = targetCenter;
                    Vector2 direction = goToPosition - Projectile.Center;
                    Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * movementSpeed * 2f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .1f);
                    Projectile.friendly = true;
                }
                else
                {
                    Vector2 goToPosition = targetCenter + new Vector2(0, -250f); // The desired position is a spot 100 pixels away at a random angle.
                    float distanceFromTarget = Vector2.Distance(Projectile.Center, goToPosition);

                    // first checks to see how to move
                    if (distanceFromTarget > 0f)
                    {
                        Vector2 direction = goToPosition - Projectile.Center;
                        direction.Normalize();
                        direction *= movementSpeed / 2;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    }

                    if (distanceFromTarget <= 600f && Projectile.ai[1] == 0)
                    {
                        Vector2 direction = targetCenter - Projectile.Center;
                        Attack();
                        attacking = true;
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            // set idle position
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 100f;
            idlePosition.X += Projectile.minionPos;
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            // Teleport to player if too far
            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            float overlapVelocity = 0.06f;

            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
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

        private void LevelHandler(Player owner) // handles how strong the minion is and determines its functions
        {
            // --- handle the minion slots --- //
            if (Projectile.minionSlots != owner.maxMinions) // Avoid redundant recalculations
            {
                Projectile.minionSlots = owner.maxMinions;
            }
            // ------------------------------- //
            slotsOccupied = Projectile.minionSlots;
            if (slotsOccupied < 4)
            {
                minionLevel = 1;
                Projectile.scale = 1f;
                movementSpeed = 16f;
                inertia = 16f;
                projectileSpeed = 3f;
            }
            else if (slotsOccupied > 3 && slotsOccupied < 7)
            {
                minionLevel = 2;
                Projectile.scale = 1.5f;
                movementSpeed = 18f;
                inertia = 18f;
                projectileSpeed = 4f;
            }
            else
            {
                minionLevel = 3;
                Projectile.scale = 2f;
                movementSpeed = 20f;
                inertia = 20f;
                projectileSpeed = 5f;
            }
        }

        private void Attack()
        {
            // --- Set up directions --- //
            Vector2 targetCenter = target.Center;
            Vector2 direction = targetCenter - Projectile.Center;
            direction.Normalize();
            direction *= projectileSpeed;
            Vector2 offset; // where the projectile will shoot from

            if (direction.X > 0)
            {
                offset = new Vector2(10, 15);
            }
            else
            {
                offset = new Vector2(-10, 15);
            }

            // --- Perform Magic 1 attack --- ///

            if (phase == 1)
            {
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                if (minionLevel == 1)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction, ModContent.ProjectileType<TerraDaggerLevel1>(), Projectile.damage, Projectile.knockBack);
                    for (int i = 0; i < 14; i++)
                    {
                        Vector2 newVel = direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15));
                        Dust.NewDust(Projectile.Center, 4, 4, ModContent.DustType<RadialGlowDust>(), newVel.X, newVel.Y, 0, Color.Lime, 0.7f);
                    }
                }
                else if (minionLevel == 2)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction, ModContent.ProjectileType<TerraDaggerLevel2>(), Projectile.damage, Projectile.knockBack);
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 newVel = direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15));
                        Dust.NewDust(Projectile.Center, 4, 4, ModContent.DustType<RadialGlowDust>(), newVel.X, newVel.Y, 0, Color.Lime, 0.7f);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 newVel = direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15));
                        Dust.NewDust(Projectile.Center, 4, 4, ModContent.DustType<RadialGlowDust>(), newVel.X, newVel.Y, 0, Color.Aqua, 0.7f);
                    }
                }
                else if (minionLevel == 3)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction, ModContent.ProjectileType<TerraDaggerLevel3>(), Projectile.damage, Projectile.knockBack);
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 newVel = direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15));
                        Dust.NewDust(Projectile.Center, 4, 4, ModContent.DustType<RadialGlowDust>(), newVel.X, newVel.Y, 0, Color.Lime, 0.7f);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 newVel = direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15));
                        Dust.NewDust(Projectile.Center, 4, 4, ModContent.DustType<RadialGlowDust>(), newVel.X, newVel.Y, 0, Color.Aqua, 0.7f);
                    }
                }
            }

            // --- Perform Magic 2 attack --- ///

            else if (phase == 2)
            {
                SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                if (minionLevel == 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (Main.rand.NextBool())
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)) * .75f, ModContent.ProjectileType<TerraMagicNegative>(), Projectile.damage * 2, Projectile.knockBack);
                        }
                        else
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)) * .75f, ModContent.ProjectileType<TerraMagicPositive>(), Projectile.damage * 2, Projectile.knockBack);
                        }
                    }

                    Vector2 launchVector = new Vector2(0f, -projectileSpeed).RotatedBy(MathHelper.ToRadians(45));
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<TerraBlaze>(), Projectile.damage, Projectile.knockBack); 
                    launchVector = new Vector2(0f, -projectileSpeed).RotatedBy(MathHelper.ToRadians(-45));
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<TerraBlaze>(), Projectile.damage, Projectile.knockBack);
                }
                else if (minionLevel == 2)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (Main.rand.NextBool())
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)) * .75f, ModContent.ProjectileType<TerraMagicNegative>(), Projectile.damage * 2, Projectile.knockBack);
                        }
                        else
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)) * .75f, ModContent.ProjectileType<TerraMagicPositive>(), Projectile.damage * 2, Projectile.knockBack);
                        }
                    }

                    Vector2 launchVector = new Vector2(0f, -projectileSpeed).RotatedBy(MathHelper.ToRadians(45));
                    if (Main.rand.NextFloat(1f) < .25f)
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<SuperTerraBlaze>(), Projectile.damage * 4, Projectile.knockBack);
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<TerraBlaze>(), Projectile.damage * 2, Projectile.knockBack);
                    }
                    launchVector = new Vector2(0f, -projectileSpeed).RotatedBy(MathHelper.ToRadians(-45));
                    if (Main.rand.NextFloat(1f) < .25f)
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<SuperTerraBlaze>(), Projectile.damage * 4, Projectile.knockBack);
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<TerraBlaze>(), Projectile.damage * 2, Projectile.knockBack);
                    }
                }
                else if (minionLevel == 3)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (Main.rand.NextBool())
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)) * .75f, ModContent.ProjectileType<TerraMagicNegative>(), Projectile.damage * 2, Projectile.knockBack);
                        }
                        else
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)) * .75f, ModContent.ProjectileType<TerraMagicPositive>(), Projectile.damage * 2, Projectile.knockBack);
                        }
                    }
                    Vector2 launchVector = new Vector2(0f, -projectileSpeed).RotatedBy(MathHelper.ToRadians(45));
                    if (Main.rand.NextFloat(1f) < .4f)
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<SuperTerraBlaze>(), Projectile.damage * 4, Projectile.knockBack);
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<TerraBlaze>(), Projectile.damage * 2, Projectile.knockBack);
                    }
                    launchVector = new Vector2(0f, -projectileSpeed).RotatedBy(MathHelper.ToRadians(-45));
                    if (Main.rand.NextFloat(1f) < .4f)
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<SuperTerraBlaze>(), Projectile.damage * 4, Projectile.knockBack);
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVector, ModContent.ProjectileType<TerraBlaze>(), Projectile.damage * 2, Projectile.knockBack);
                    }
                }
            }

            // --- perform Melee attack --- //
            else if (phase == 3)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction * projectileSpeed * 0.5f, ModContent.ProjectileType<TerraSpear>(), Projectile.damage, Projectile.knockBack);
                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
            }
        }
        private void Visuals()
        {
            FrameHandler();
            VisualDustHandler();
        }
        private void FrameHandler()
        {

            // idle visuals
            if (phase == 0)
            {
                if (Projectile.ai[0] % 20 == 0)
                {
                    Projectile.frame++;
                }
                if (Projectile.frame > 1)
                {
                    Projectile.frame = 0;
                }
            }
            // magic 1 visuals
            if (attacking)
            {
                if (phase == 1)
                {
                    if (Projectile.ai[1] < 10)
                    {
                        Projectile.frame = 2;
                    }
                    else
                    {
                        Projectile.frame = 1;
                    }
                }
                // magic 2 visuals
                else if (phase == 2)
                {
                    if (Projectile.ai[1] < 30)
                    {
                        Projectile.frame = 2;
                    }
                    else
                    {
                        Projectile.frame = 1;
                    }
                }
                // melee visuals
                else if (phase == 3)
                {
                    if (Projectile.ai[1] < 10)
                    {
                        Projectile.frame = 3;
                    }
                    else if (Projectile.ai[1] < 20)
                    {
                        Projectile.frame = 4;
                    }
                    else if (Projectile.ai[1] < 30)
                    {
                        Projectile.frame = 5;
                    }
                    else
                    {
                        Projectile.frame = 1;
                    }
                }
            }
        }
        private void VisualDustHandler()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 1f);
            if (minionLevel == 2 && Projectile.ai[0] % 15 == 0)
            {
                Dust.NewDust(Projectile.position, 20, 60, ModContent.DustType<SharpRadialGlowDust>(), 0, -1 * (Main.rand.NextFloat() * 3f + 3f), 0, Color.Lime, 0.5f);
            }
            else if (minionLevel == 3 && Projectile.ai[0] % 10 == 0)
            {
                Dust.NewDust(Projectile.position, 20, 60, ModContent.DustType<SharpRadialGlowDust>(), 0, -1 * (Main.rand.NextFloat() * 3f + 3f), 0, Color.Lime, 0.5f);
                Dust.NewDust(Projectile.position, 20, 60, ModContent.DustType<SharpRadialGlowDust>(), 0, -1 * (Main.rand.NextFloat() * 3f + 3f), 0, Color.SkyBlue, 0.3f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (Projectile.direction == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            // ----- draw flames ------ //
            Texture2D flameTexture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/TerraWeapons/TerraSpiritOffensiveMinion/TerraSpiritFlameGreen").Value;
            Vector2 drawOrigin = new Vector2(flameTexture.Width * 0.5f, Projectile.height * 0.5f);
            int frameHeight = flameTexture.Height / 8; // Assuming 4 frames for the wings
            flameFrame.Y = (flameCounter / 6 % 8) * frameHeight; // Update wing animation frame
            flameFrame.Width = flameTexture.Width;
            flameFrame.Height = frameHeight;

            // Draw flame
            Main.EntitySpriteDraw(flameTexture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                flameFrame,
                Color.White, Projectile.rotation, new Vector2(flameTexture.Width * 0.5f, frameHeight * 0.5f), Projectile.scale, effects, 0f);

            // ----- draw main texture ------ //
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            frameHeight = texture.Height / Main.projFrames[Projectile.type]; // Assuming 3 frames
            Rectangle mainFrame = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);

            if (targetMarked)
            {
                lightColor = Color.LimeGreen;
            }
            else
            {
                lightColor = Color.White;
            }
            // Draw Main Projectile
            Main.EntitySpriteDraw(texture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                mainFrame,
                lightColor, Projectile.rotation, new Vector2(texture.Width * 0.5f, frameHeight * 0.5f), Projectile.scale, effects, 0f);

            flameCounter++;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (targetMarked && Main.rand.Next(4) == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(10, 0), ModContent.ProjectileType<TerraHeal>(), Projectile.damage, 0f);
            }
        }
    }
}
