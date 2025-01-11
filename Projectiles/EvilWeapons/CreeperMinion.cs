using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TheTesseractMod.GlobalFuncitons;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using TheTesseractMod.Buffs.MinionBuffs;

namespace TheTesseractMod.Projectiles.EvilWeapons
{
    internal class CreeperMinion : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/Vanilla/Creeper";
        float speed = 30f;          // Speed multiplier of the minion
        float farSpeed = 60f;       // Temporary speed value used in distance calculations (So we don't change our original speed)
        float inertia = 25f;        // Determines how long an object will move after having velocity applied
        float farInertia = 30f;     // Temporary intertia used in distance calculations (So we don't change our original interia)
        float attackSight = 700f;   // How far away an enemy must be for the minion to "see" it
        float idleRange = 60f;      // The range in which the minion will idle over the player
        float deadzoneRange = 40f;  // The deadzone range in which the minion will not latch onto an enemy
        bool recentlyHit = false;

        //------------------------------------------------------------------------------------------------------------------------

        public override void SetStaticDefaults()
        {
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public sealed override void SetDefaults()
        {
            Projectile.scale = .7f;
            Projectile.width = 25;
            Projectile.height = 25;
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
            if (recentlyHit)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] % 4 == 0)
                {
                    Projectile.ai[0] = 0;
                    recentlyHit = false;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= speed;
                }
            }
            else
            {
                GoToTarget();
            }
            Visuals();
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<CreeperMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<CreeperMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public void GoToTarget()
        {
            NPC npc = GlobalProjectileFunctions.findClosestTargetInRange(Projectile.Center, attackSight);
            Player owner = Main.player[Projectile.owner];
            Vector2 targetCenter = Vector2.Zero;
            Vector2 playerPosition = owner.Center;
            float distanceFromOwner = Vector2.Distance(Projectile.position, playerPosition);

            if (owner.HasMinionAttackTargetNPC)
            {
                npc = Main.npc[owner.MinionAttackTargetNPC];
                targetCenter = npc.Center;
                Vector2 direction = targetCenter - Projectile.Center;
                direction.Normalize();
                direction *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
            }
            else if (npc != null && distanceFromOwner < 700f)
            {
                targetCenter = npc.Center;
                if (Vector2.Distance(playerPosition, targetCenter) < attackSight)
                {
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                targetCenter = playerPosition;
                Vector2 direction = targetCenter - Projectile.Center;
                direction.Normalize();
                direction *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
            }

            if (distanceFromOwner < 16f)
            {
                recentlyHit = true;
            }
            if(distanceFromOwner > 2000)
            {
                Projectile.position = owner.position + new Vector2(50, 0);
            }

            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture;
            if (WorldGen.crimson)
            {
                texture = ModContent.Request<Texture2D>(Texture).Value;
            }
            else
            {
                texture = ModContent.Request<Texture2D>("TheTesseractMod/Textures/Vanilla/CreeperPurple").Value;
            }
            
            Main.EntitySpriteDraw(texture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            recentlyHit = true;
        }


        private void Visuals()
        {
        }
    }
}
