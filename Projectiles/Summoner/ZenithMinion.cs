using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Global.Projectiles.Summon;

namespace TheTesseractMod.Projectiles.Summoner
{
    internal class ZenithMinion : ModProjectile
    {
        Vector2 rainbowMissleSpeed = new Vector2(4f, 4f);
        List <Color> colorsForDust = new List <Color> ();
        int colorsIDX = 0;
        int counterForRainbowMissle = 0;
        float speed = 60f;          // Speed multiplier of the minion
        float farSpeed = 60f;       // Temporary speed value used in distance calculations (So we don't change our original speed)
        float inertia = 20f;        // Determines how long an object will move after having velocity applied
        float farInertia = 20f;     // Temporary intertia used in distance calculations (So we don't change our original interia)
        float attackSight = 600f;   // How far away an enemy must be for the minion to "see" it
        float idleRange = 60f;      // The range in which the minion will idle over the player
        float deadzoneRange = 40f;  // The deadzone range in which the minion will not latch onto an enemy
        float orbitSpeed = 0.125f;  // degrees of position of minion incremented each update. Used in Movement()
        float newAngle = 0f;        // new angle in degrees that will be added to the position.
        //------------------------------------------------------------------------------------------------------------------------

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;

            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public sealed override void SetDefaults()
        {
            Projectile.scale = 1.7f;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.tileCollide = false; 
            Projectile.friendly = true; 
            Projectile.minion = true; 
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Projectile.localNPCHitCooldown = 15;
            }
            colorsForDust.Add(Color.Red); colorsForDust.Add(Color.Orange); colorsForDust.Add(Color.Yellow); colorsForDust.Add(Color.Green); colorsForDust.Add(Color.Turquoise); colorsForDust.Add(Color.Blue); colorsForDust.Add(Color.Purple); colorsForDust.Add(Color.Magenta);
            colorsForDust.Add(Color.Red); colorsForDust.Add(Color.Orange); colorsForDust.Add(Color.Yellow); colorsForDust.Add(Color.Green); colorsForDust.Add(Color.Turquoise); colorsForDust.Add(Color.Blue); colorsForDust.Add(Color.Purple); colorsForDust.Add(Color.Magenta);
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
            Projectile.rotation += 0.15f * (float)Projectile.direction;
            
            Player owner = Main.player[Projectile.owner];
            if (!CheckActive(owner))
            {
                return;
            }

            Random random = new Random();
            int rand = random.Next(700);
            if (rand == 100)
            {
                SpawnMinion();
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<ZenithMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<ZenithMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            /***THE FOLLOWING CODE IS FOR MY OWN AI WHERE I WANT THE MINIONS TO CIRCLE THE PLAYER WHEN IDLE***/
            /*************THE ORIGINAL AI CODE WILL BE BELOW THE LINE OF STARS, COMMENTED OUT*****************/
            float orbitRadius = 100f; // distance from player
            //newAngle += orbitSpeed;
            int numOfSameMinion = 0; // how many of this specific minion exist. calulated below
            int thisIndex = 0; // the index of this projectile relative to the same type.

            // calculate the angle between minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == Projectile.type)
                {
                    numOfSameMinion++;
                }
            }
            float angleBetweenMinions = (float)(Math.PI * 2 / numOfSameMinion); // angle between each minion. if one minion exists then 360 deg, if 2 minions exist then 180 deg, etc.

            // calculate the angle of the position
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == Projectile.type)
                {
                    thisIndex++;
                }
                if (Main.projectile[i].whoAmI == Projectile.whoAmI)
                {
                    break;
                }
            }
            float positionAngle = thisIndex * angleBetweenMinions;
            //positionAngle += MathHelper.ToRadians(newAngle);// rotates the position

            // Calculate positions
            Vector2 idlePosition = owner.Center + new Vector2((float)Math.Cos(positionAngle) * orbitRadius, (float)Math.Sin(positionAngle) * orbitRadius);
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }


            /***********************************************************************************************/

            /*float orbitRadius = 70f;
            float orbitSpeed = 0.02f; 

            // Calculate the angle based on the minion's position
            float angle = (float)(Projectile.ai[1] + (Math.PI * 2 / owner.numMinions));

            // Calculate the circular position
            Vector2 idlePosition = owner.Center + new Vector2((float)Math.Cos(angle) * orbitRadius, (float)Math.Sin(angle) * orbitRadius);

            // Store the angle for the next update
            Projectile.ai[1] += orbitSpeed;

            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }*/

            /*************************************************************************************************/



            /*Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 70f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            //float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += Projectile.minionPos; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)
            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
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
            }*/
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            distanceFromTarget = attackSight;
            targetCenter = Projectile.position;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 1000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget)/* && (lineOfSight || closeThroughWall)*/) //uncomment this code to make it so the minion can't see thru walls
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            if (foundTarget)
            {
                //counterForRainbowMissle++;
                if (counterForRainbowMissle % 300 == 0)
                {
                    /*Shoots rainbow missle*/
                    //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<ZenithMinionRainbowMissle>(), 850, Projectile.knockBack, Projectile.owner);
                    //SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                    
                }
                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > deadzoneRange)
                {
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia-1) + direction) / inertia;
                    Projectile.rotation += 0.5f * (float)Projectile.direction;
                }
            }
            else
            {
                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > attackSight)
                {
                    // Speed up the minion if it's away from the player
                    speed = farSpeed;
                    inertia = farInertia;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = farSpeed / 3;
                    inertia = farInertia / 1.25f;
                }


                if (distanceToIdlePosition > idleRange)
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
                    Projectile.velocity.X = 0.15f;
                    Projectile.velocity.Y = 0.05f;
                }
            }
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            //Projectile.rotation = Projectile.velocity.X * 0.05f;
            
            int frameSpeed = 4;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                //Lighting.AddLight(Projectile.position, colorsForDust[colorsIDX].ToVector3());
                //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 263, Projectile.velocity.X, Projectile.velocity.Y, 150, colorsForDust[colorsIDX], 1f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 63, Projectile.velocity.X, Projectile.velocity.Y, 100, default(Color), 1f);
                Projectile.frameCounter = 0;
                Projectile.frame++;
                colorsIDX++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                    colorsIDX = 0;
                }
                
            }
            Lighting.AddLight(Projectile.position, Color.White.ToVector3());
        }

        private void SpawnMinion()
        {
            Random rand = new Random();
            int random = rand.Next(13);
            int projectile = ModContent.ProjectileType<ZenithMinionRainbowMissle>();
            ZenithMinionSummonGlobalOverride.shotByZenithMinion = true;
            switch (random)
            {
                case 0:
                    projectile = ProjectileID.AbigailMinion;
                    break;
                case 1:
                    projectile = ProjectileID.FlinxMinion;
                    break;
                case 2:
                    projectile = ProjectileID.BabySlime;
                    break;
                case 3:
                    projectile = 390 + rand.Next(3); // Baby spider

                    break;
                case 4:
                    projectile = 755; // sanguine bat

                    break;
                case 5:
                    projectile = 864; // enchanted dagger

                    break;
                case 6:
                    projectile = 387 + rand.Next(2); // twins pet

                    break;
                case 7:
                    projectile = 191 + rand.Next(4); // pygmy

                    break;
                case 8:
                    projectile = ProjectileID.DeadlySphere;
                    break;
                case 9:
                    projectile = 407; // mini sharknado
                    break;
                case 10:
                    projectile = ProjectileID.StardustCellMinion;
                    break;
                case 11:
                    projectile = 946; // Terraprisma
                    break;
                case 12:
                    projectile = ProjectileID.RainbowCrystal;
                    break;
                default:
                    projectile = ModContent.ProjectileType<ZenithMinionRainbowMissle>();
                    break;

            }
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, projectile, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            ZenithMinionSummonGlobalOverride.shotByZenithMinion = false;
            SoundEngine.PlaySound(SoundID.Item44, Projectile.position);
        }
    }
}
