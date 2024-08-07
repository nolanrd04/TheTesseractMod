
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Projectiles.Enemy;

namespace TheTesseractMod.Projectiles.Melee.EtherealSwordProjectiles
{
    internal class EtherealSwordSwingProjectile : ModProjectile // CODE HEAAAVILY ADAPTED FROM EXAMPLE MOD!
    {
        // We define some constants that determine the swing range of the sword
        // Not that we use multipliers here since that simplifies the amount of tweaks for these interactions
        // You could change the values or even replace them entirely, but they are tweaked with looks in mind
        private const float SWINGRANGE = 1.67f * (float)Math.PI;
        private const float FIRSTHALFSWING = 0.45f;
        private const float SPINRANGE = 3.5f * (float)Math.PI;
        private const float WINDUP = 0.15f;
        private const float UNWIND = 0.4f;

        private enum AttackType
        {
            Swing,
            UpSwing,
        }

        private enum AttackStage
        {
            Prepare,
            Execute,
            Unwind
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        private AttackType CurrentAttack
        {
            get => (AttackType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private AttackStage CurrentStage
        {
            get => (AttackStage)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (float)value;
                Timer = 0;
            }
        }


        private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
        private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
        private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle
        private ref float Size => ref Projectile.localAI[2]; // Size of sword

        //How fast each stage is performed.
        private float prepTime => 4f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float execTime => 10f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 4f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

        private float maxSize = 2f; // for easily ajusting the size of the projectile.

        public override string Texture => "TheTesseractMod/Items/Weapons/Melee/EtherealSword";
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

            if (CurrentAttack == AttackType.UpSwing)
            {
                // For the upswing, starting angle is at the bottom of the swing
                InitialAngle = targetAngle + FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection;
            }
            else
            {
                if (Projectile.spriteDirection == 1)
                {
                    targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
                }
                else
                {
                    if (targetAngle < 0)
                    {
                        targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
                    }

                    targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
                }
                // For down swing, the starting angle depends on the mouse cursor (targetAngle)
                InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            // Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PrepareStrike();
                    break;
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                default:
                    UnwindStrike();
                    break;
            }

            SetSwordPosition();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? CanDamage()
        {
            if (CurrentStage == AttackStage.Prepare)
                return false;
            return base.CanDamage();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
        }
        public void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress; // Set projectile rotation

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
            Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers

            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
        }

        /****************************************SWING***************************************/

        //The swing animation has 3 stages, prepare strike, execute strike, and unwind strike.
        // Prepare strike grows the sword in size
        // Execute strike actually swings the sword.
        // Unwind strike shrinks the sword while continuing the swing animation.
        private void PrepareStrike()
        {
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime);// Calculates rotation from initial angle
            }
            else
            {
                Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime);// for swing up
            }

            Size = MathHelper.SmoothStep(0, maxSize, Timer / prepTime); // Make sword slowly increase in size as we prepare to strike until it reaches max

            if (Timer >= prepTime)
            {
                SoundEngine.PlaySound(SoundID.Item71); // Play sword sound here since playing it on spawn is too early
                CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
            }
        }

        private void ExecuteStrike()
        {
            /* lerp(initial position, ending position, amount) */
            ;
            // Swing down
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / execTime);

                if (Timer >= execTime)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }
            // Swing up
            else
            {
                Progress = MathHelper.Lerp(SWINGRANGE + 1f, 0, (1f - UNWIND) * Timer / execTime);
                if (Timer >= execTime)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }
            if (Timer == 1f)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(10f, 0f).RotatedBy((Main.MouseWorld - player.MountedCenter).ToRotation()) /*+ Main.player[Projectile.owner].velocity*/, ModContent.ProjectileType<EtherealSwordSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                if (CurrentAttack == AttackType.Swing)
                {
                    Random rand = new Random();
                    int projPicker = rand.Next(2);

                    if (projPicker == 0)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(10f, 0f).RotatedBy((Main.MouseWorld - player.MountedCenter).ToRotation() + MathHelper.ToRadians(7)), ModContent.ProjectileType<LightEtherealStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(10f, 0f).RotatedBy((Main.MouseWorld - player.MountedCenter).ToRotation() - MathHelper.ToRadians(7)), ModContent.ProjectileType<LightEtherealStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(10f, 0f).RotatedBy((Main.MouseWorld - player.MountedCenter).ToRotation() + MathHelper.ToRadians(7)), ModContent.ProjectileType<DarkEtherealStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(10f, 0f).RotatedBy((Main.MouseWorld - player.MountedCenter).ToRotation() - MathHelper.ToRadians(7)), ModContent.ProjectileType<DarkEtherealStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    
                }

            }
        }
        private void UnwindStrike()
        {
            /* lerp(initial position, ending position, amount) */

            // swing down
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) + UNWIND * Timer / hideTime);
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);

                if (Timer >= hideTime)
                {
                    Projectile.Kill();
                }
            }
            // swing up
            else
            {
                Progress = MathHelper.SmoothStep(SWINGRANGE, 1f, 1f - UNWIND + UNWIND * Timer / hideTime);
                Size = MathHelper.SmoothStep(maxSize, 0, Timer / hideTime);
                if (Timer >= hideTime)
                {
                    Projectile.Kill();
                }
            }
        }
        /*************************************************END SWING**************************************/

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            if (target.type != NPCID.TargetDummy)
            {
                owner.AddBuff(ModContent.BuffType<DeathProtectionTwo>(), 180);
            }

        }
    }
}
