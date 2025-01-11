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
using TheTesseractMod.Buffs.MinionBuffs;
using TheTesseractMod.GlobalFuncitons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons;
using TheTesseractMod.Projectiles.Summoner.ShadowFlameDragon;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsCrescentProj : ModProjectile
    {
        float attackSight = 900f;   // How far away an enemy must be for the minion to "see" it
        float idleRange = 60f;      // The range in which the minion will idle over the player
        float deadzoneRange = 40f;  // The deadzone range in which the minion will not latch onto an enemy
        float speed = 10f;
        int attackStage = 0;        // the type of attack this minion will use. 1 is dash, 0 is shoot meteors.
        int attackStageCounter;     // the counter that keeps track of how long the minion has been on an attack stage 
        int idleCounter = 0;        // For keeping track of how long a minion is idle for. After a certain time the attack stage will revert.
        bool recentlyHit = false;
        int initialDamage;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            initialDamage = Projectile.damage;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
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

            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            if (recentlyHit)
            {
                Projectile.ai[1]++;
                if (attackStage == 0)
                {
                    if (Projectile.ai[1] % 4 == 0)
                    {
                        Projectile.ai[1] = 0;
                        recentlyHit = false;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 10;
                    }
                }
                else
                {
                    if (Projectile.ai[1] % 10 == 0)
                    {
                        Projectile.ai[1] = 0;
                        recentlyHit = false;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 10;
                    }
                }
            }
            else
            {
                Movement(distanceToIdlePosition, vectorToIdlePosition, owner);
            }
            Visuals();
            Projectile.ai[0]++;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<NightsCrescentMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<NightsCrescentMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            // set idle position
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 70f;
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

        

        private void Movement(float distanceToIdlePosition, Vector2 vectorToIdlePosition, Player owner)
        {
            NPC target;
            if (owner.HasMinionAttackTargetNPC)
            {
                target = Main.npc[owner.MinionAttackTargetNPC];
            }
            else
            {
                target = GlobalProjectileFunctions.findClosestTargetInRange(Projectile.Center, attackSight);
            }

            bool foundTarget = target != null;

            float inertia;
            Vector2 goToPosition;
            float distanceFromPosition;

            if (foundTarget)
            {
                speed = 30f;
                Projectile.friendly = true;
                Vector2 targetCenter = target.Center;
                goToPosition = targetCenter;
                if (attackStage == 0) // Meteor stage
                {
                    Projectile.damage = initialDamage;
                    inertia = 30f;
                    Vector2 direction = goToPosition - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    attackStageCounter++;
                    if (attackStageCounter > 300)
                    {
                        attackStageCounter = 0;
                        attackStage = 1;
                    }
                }
                else // DASHING STAGE
                {
                    Projectile.damage = (int)(initialDamage * 2f);
                    inertia = 25f;
                    distanceFromPosition = Vector2.Distance(Projectile.Center, targetCenter);
                    Vector2 direction = goToPosition - Projectile.Center;
                    direction.Normalize();
                    direction *= (speed * 2.5f);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    attackStageCounter++;

                    if (attackStageCounter > 180)
                    {
                        attackStage = 0;
                        attackStageCounter = 0;
                    }
                }
            }
            else
            {
                Projectile.friendly = false;
                // reset attack stage after being idle
                idleCounter++;
                if (idleCounter > 10)
                {
                    attackStage = 0;
                }

                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 40f;
                    inertia = 20f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 4f;
                    inertia = 40f;
                }

                if (distanceToIdlePosition > 20f)
                {
                    inertia = 25f;
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
            if (attackStage == 0)
            {
                Projectile.rotation += MathHelper.ToRadians(9);
                Projectile.frame = 0;
                if (Projectile.ai[0] % 4 == 0)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, 0, 0, Main.rand.Next(45) + 80, default(Color), 1f);
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0, 0, 0, default(Color), 1f);
                }
            }
            else
            {
                Projectile.rotation += MathHelper.ToRadians(18);
                Projectile.frame = 1;
                if (Projectile.ai[0] % 2 == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurificationPowder, 0, 0, 0, default(Color), 1f);
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0, 0, 0, default(Color), 1f);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;


            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            // draw back sprites only if dashing
            if (attackStage == 1)
            {
                Texture2D bgTexture = TextureAssets.Projectile[Type].Value;
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
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            recentlyHit = true;
        }
    }
}
