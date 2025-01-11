using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Buffs.MinionBuffs;
using TheTesseractMod.GlobalFuncitons;
using TheTesseractMod.Projectiles.HallowedWeapons;

namespace TheTesseractMod.Projectiles.TrueExcaliburWeapons
{
    internal class TrueGoldenMageMinion : ModProjectile
    {
        int phase = 1; // 0 = attack, 1 = defend
        float attackSight = 900f;
        float movementSpeed = 9f;
        float projectileSpeed = 4f;
        NPC target;
        int timeForPhase = 0;
        Rectangle wingFrame = new Rectangle(0, 0, 98, 46);
        int idleCounter = 0;
        float inertia = 10f;
        float rotation = MathHelper.ToRadians(Main.rand.Next(360));
        int wingCounter = 0;

        //------------------------------------------------------------------------------------------------------------------------

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 32;
            Projectile.height = 48;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
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


            if (owner.HasMinionAttackTargetNPC)
            {
                target = Main.npc[owner.MinionAttackTargetNPC];
            }
            else
            {
                target = GlobalProjectileFunctions.findClosestTargetInRange(owner.Center, attackSight);
            }

            if (timeForPhase == 0)
            {
                timeForPhase = Main.rand.Next(600, 1800);
                phase = (phase + 1) % 2;
            }
            else
            {
                timeForPhase--;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            if (target != null)
            {
                MovementAndAttack(owner, vectorToIdlePosition, distanceToIdlePosition);
                if (phase == 0)
                {
                    if (Projectile.ai[0] == 0)
                    {
                        Projectile.ai[0] = 30;
                    }
                    if (Projectile.ai[0] == 15)
                    {
                        Projectile.frame = 0;
                    }
                    Projectile.ai[0]--;
                }
                else
                {
                    if (Projectile.ai[0] == 0)
                    {
                        Projectile.ai[0] = 60;
                    }
                    if (Projectile.ai[0] == 40)
                    {
                        Projectile.frame = 0;
                    }
                    Projectile.ai[0]--;
                }
                idleCounter = 0;
            }
            else
            {
                Movement(owner, vectorToIdlePosition, distanceToIdlePosition);
            }
            Visuals();
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<TrueGoldenMageMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<TrueGoldenMageMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void Visuals()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(255 / 255f, 252 / 255f, 161 / 255f));
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            wingCounter++;
        }

        private void Attack()
        {
            Vector2 targetCenter = target.Center;
            Vector2 direction = targetCenter - Projectile.Center;
            direction.Normalize();
            direction *= projectileSpeed;
            Vector2 offset;

            if (direction.X > 0)
            {
                offset = new Vector2(10, 15);
            }
            else
            {
                offset = new Vector2(-10, 15);
            }

            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + offset, direction, ModContent.ProjectileType<TrueGoldenMageMagic>(), Projectile.damage, Projectile.knockBack);
            SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
        }

        private void Buff(Player owner)
        {
            Vector2 ownerCenter = owner.Center;
            Vector2 direction = ownerCenter - Projectile.Center;
            direction.Normalize();
            direction *= projectileSpeed;

            Vector2 offset;

            if (direction.X > 0)
            {
                offset = new Vector2(10, 15);
            }
            else
            {
                offset = new Vector2(-10, 15);
            }

            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + offset, direction, ModContent.ProjectileType<TrueGoldenMageFriendlyMagic>(), Projectile.damage, Projectile.knockBack);
            SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
        }

        private void Movement(Player owner, Vector2 vectorToIdlePosition, float distanceToIdlePosition)
        {
            Projectile.friendly = false;
            // reset attack stage after being idle
            idleCounter++;
            // Main.NewText("Idle counter: " + idleCounter);
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
                movementSpeed = 10f;
                inertia = 40f;
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
            if (phase == 0) // If attacking
            {
                Vector2 targetCenter = target.Center;
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

                if (distanceFromTarget <= 350f && Projectile.ai[0] == 0)
                {
                    Vector2 direction = targetCenter - Projectile.Center;
                    Attack();
                    if (direction.X > 0)
                    {
                        Projectile.frame = 1;
                    }
                    else
                    {
                        Projectile.frame = 2;
                    }
                }
            }
            else // defending
            {
                if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
                {
                    Projectile.position = owner.Center;
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
                if (distanceToIdlePosition > 200f)
                {
                    // Speed up the minion if it's away from the player
                    movementSpeed = 40f;
                    inertia = 20f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    movementSpeed = 10f;
                    inertia = 40f;
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

                if (Projectile.ai[0] == 0)
                {
                    Vector2 direction = owner.Center - Projectile.Center;
                    Buff(owner);
                    if (direction.X > 0)
                    {
                        Projectile.frame = 1;
                    }
                    else
                    {
                        Projectile.frame = 2;
                    }
                }
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D wingTexture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/TrueExcaliburWeapons/TrueGoldenKnightProjWings").Value;
            Vector2 drawOrigin = new Vector2(wingTexture.Width * 0.5f, Projectile.height * 0.5f);

            int wingFrameHeight = wingTexture.Height / 4; // Assuming 4 frames for the wings
            wingFrame.Y = (wingCounter / 6 % 4) * wingFrameHeight; // Update wing animation frame
            wingFrame.Width = wingTexture.Width;
            wingFrame.Height = wingFrameHeight;

            // Draw Wings
            Main.EntitySpriteDraw(wingTexture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                wingFrame,
                Color.White, Projectile.rotation, new Vector2(wingTexture.Width * 0.5f, wingFrameHeight * 0.5f), Projectile.scale, SpriteEffects.None, 0f);


            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type]; // Assuming 3 frames
            Rectangle mainFrame = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);

            // Draw Main Projectile
            Main.EntitySpriteDraw(texture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                mainFrame,
                lightColor, Projectile.rotation, new Vector2(texture.Width * 0.5f, frameHeight * 0.5f), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
