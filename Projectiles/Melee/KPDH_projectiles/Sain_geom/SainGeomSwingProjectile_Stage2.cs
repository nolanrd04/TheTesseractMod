using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Weapons.Melee.KPDH;

namespace TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Sain_geom
{
    internal class SainGeomSwingProjectile_Stage2 : ModProjectile
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
        private float prepTime => 36f / Owner.GetTotalAttackSpeed(Projectile.DamageType); // how fast it grows in size after spawning
        private float execTime => 70f / Owner.GetTotalAttackSpeed(Projectile.DamageType); // how fast the swing executes
        private float hideTime => 36f / Owner.GetTotalAttackSpeed(Projectile.DamageType); // how fast it shrinks in size after swinging

        private float maxSize = 1.5f; // for easily ajusting the size of the projectile.

        public override string Texture => "TheTesseractMod/Items/Weapons/Melee/KPDH/Sain_geom_stage2";
        private Player Owner => Main.player[Projectile.owner];
        private float hiltOffset = 17; // where the player should be holding the hilt. Handled in PreDraw(), measured in pixels

        // Blade slash visualization
        private VertexStrip bladeStrip = new VertexStrip();
        private List<Vector2> swingTrailList = new List<Vector2>();
        private const int trailLength = 1000;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 8;
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

            // --- Update swing progression based on stage ---
            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PrepareStrike();
                    SetSwordPosition();
                    break;

                case AttackStage.Execute:
                    ExecuteStrike();
                    SetSwordPosition();

                    // calculate current tip
                    Vector2 motionComp = Owner.velocity / (Projectile.extraUpdates + 1);
                    Vector2 tip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.width * Projectile.scale);
                    tip -= motionComp; // remove some of the positional jump


                    if (swingTrailList.Count > 0)
                    {
                        Vector2 lastTip = swingTrailList[swingTrailList.Count - 1];
                        float distance = Vector2.Distance(lastTip, tip);

                        // More subdivisions if the player is moving fast
                        int steps = Math.Max(1, (int)(distance / 2f));
                        for (int i = 1; i <= steps; i++)
                        {
                            float t = i / (float)(steps + 1);
                            swingTrailList.Add(Vector2.Lerp(lastTip, tip, t));
                        }

                    }

                    // always add the current tip
                    swingTrailList.Add(tip);

                    // trim trail to maximum length
                    while (swingTrailList.Count > trailLength)
                    {
                        swingTrailList.RemoveAt(0);
                    }

                    break;

                case AttackStage.Unwind:
                    UnwindStrike();
                    SetSwordPosition(); // update sword during unwind
                    break;
            }

            Timer++;

        }


        public override bool PreDraw(ref Color lightColor)
        {
            // draw trail
            if (swingTrailList.Count > 2)
            {
                GameShaders.Misc["RainbowRod"].Apply();

                float[] rotations = new float[swingTrailList.Count];
                for (int i = 0; i < rotations.Length; i++)
                {
                    rotations[i] = Projectile.rotation;
                }

                Vector2[] positions = swingTrailList.ToArray();
                Array.Reverse(positions);

                bladeStrip.PrepareStrip(
                    positions,
                    rotations,
                    progress => new Color(56, 184, 252, 50) * (1f - progress),
                    progress => MathHelper.Lerp(30f, 25f, progress),
                    -Main.screenPosition,
                    swingTrailList.Count,
                    includeBacksides: true
                );

                bladeStrip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }
            // Draw Sword
            // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;
            float bgRotationOffset1;
            float bgRotationOffset2;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(hiltOffset, Projectile.height - hiltOffset);
                rotationOffset = MathHelper.ToRadians(45f);

                if (CurrentAttack == AttackType.Swing)
                {
                    bgRotationOffset1 = MathHelper.ToRadians(38);
                    bgRotationOffset2 = MathHelper.ToRadians(31);
                }
                else
                {
                    bgRotationOffset1 = MathHelper.ToRadians(52);
                    bgRotationOffset2 = MathHelper.ToRadians(57);

                }
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width - hiltOffset, Projectile.height - hiltOffset);
                rotationOffset = MathHelper.ToRadians(135f);
                if (CurrentAttack == AttackType.Swing)
                {
                    bgRotationOffset1 = MathHelper.ToRadians(142);
                    bgRotationOffset2 = MathHelper.ToRadians(149);
                }
                else
                {
                    bgRotationOffset1 = MathHelper.ToRadians(128);
                    bgRotationOffset2 = MathHelper.ToRadians(121);
                }
                effects = SpriteEffects.FlipHorizontally;
            }

            // ************************************************** //
            // draw bg
            Asset<Texture2D> bg = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/KPDH_projectiles/Sain_geom/Geom_bw");
            Main.spriteBatch.Draw(bg.Value, Projectile.Center - Main.screenPosition, default, new Color(56, 184, 252, 75) * .2f, Projectile.rotation + bgRotationOffset1, origin, Projectile.scale * 1.05f, effects, 0);
            Main.spriteBatch.Draw(bg.Value, Projectile.Center - Main.screenPosition, default, new Color(56, 184, 252, 75) * .1f, Projectile.rotation + bgRotationOffset2, origin, Projectile.scale * 1.1f, effects, 0);

            // draw main
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            // ************************************************* //
            if (CurrentStage == AttackStage.Execute)
            {
                // spawn particles
                float bladeLength = texture.Height * Projectile.scale;

                // et direction the blade is facing
                Vector2 bladeDir = Projectile.rotation.ToRotationVector2();

                // World position of the tip of the blade, multiplied by a random float value so it spawns anywhere along the blade
                Vector2 bladePos = Projectile.Center + bladeDir * bladeLength * (Main.rand.NextFloat(.6f) + .4f);
                if (Main.rand.NextFloat() < .333f)
                {
                    ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings { PositionInWorld = bladePos, MovementVector = Vector2.Zero });
                }
                if (Main.rand.NextFloat() < .15f)
                {
                    Dust.NewDust(bladePos, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, new Color(56, 184, 252, 0), Main.rand.NextFloat(.7f) + .8f);
                }
                if (Main.rand.NextFloat() < .33333f)
                {
                    Dust.NewDust(bladePos, 0, 0, DustID.UltraBrightTorch, 0, 0, 0, default(Color), Main.rand.NextFloat(.6f) + .4f);
                }

                if (Main.rand.NextFloat() < .12f)
                {
                    Dust.NewDust(bladePos, 0, 0, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, new Color(127, 68, 252, 0), Main.rand.NextFloat(.4f) + .4f);
                }

            }
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
                SoundEngine.PlaySound(SoundID.Item105); // Play sword sound here since playing it on spawn is too early
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
        }
        private void UnwindStrike()
        {
            /* lerp(initial position, ending position, amount) */

            // swing down
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, 1f - UNWIND + UNWIND * Timer / hideTime);
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
            if (target.CanBeChasedBy())
            {
                Player owner = Main.player[Projectile.owner];
                owner.AddBuff(ModContent.BuffType<Attuned>(), 300);
            }

            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings { PositionInWorld = target.Center, MovementVector = Vector2.Zero, UniqueInfoPiece = 150 });

            if (target.CanBeChasedBy() && GeomSwordStats.canBuildConsecutiveHits)
            {
                GeomSwordStats.consecutiveHits++;
                GeomSwordStats.comboExpireTimer = 0;
            }

            Vector2 spawnPos = target.Center + new Vector2(150 + Main.rand.Next(50), 0).RotatedBy(Main.rand.NextFloat(2 * (float)Math.PI));
            Vector2 direction = (target.Center - spawnPos);
            direction.Normalize();
            direction *= 10f;

            Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, direction, ModContent.ProjectileType<Sain_geom_star_2>(), Projectile.damage / 2, 0f);
        }
    }
}
