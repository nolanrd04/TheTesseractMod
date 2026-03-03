using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using TheTesseractMod.Buffs.TemporalGuardianBuffs;
using TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.NPCs.Bosses.GuardianOfTheRift
{
    internal class GuardianOfTheRiftBody  : ModNPC 
    {
        // phase is a number 0-10 to represent which element to use: 0 heat, 1 dust, 2 light, 3 chloro, 4 aqua, 5 cold, 6 glow, 7 electric, 8 dark, 9 life , 10 death
        public int elementIdx = 0;
        private bool secondStage = false;
        private float moveSpeed = 10f;
        private int phaseTransitionTimer = 0; // for when the boss is moving into the second phase
        private bool doPhaseTransitionAnimation = false; // for knowing when to move to the next phase
        float inertia = 1f;
        bool assignedGoToPosition = false; // for determining whether or not a fixed go to position exists.
        Vector2 fixedGoToPosition; // some attacks require the boss to be in a certain position

        private int attackIdx = 0; // used to determine which attack to use for each phase. For example: For phase 0, flame barage = 0, Inferno storm = 1
        private int elementIdxTimer; // Continuously increments for each element.
        private int attackPhaseTimer; // used to keep track of how long each attack phase is. Resets on changing phases, and can either continuously increment, or increment based on conditions.
                                      // For example, it will continuously increment in elementIdx 0, attackIdx 0, but only increment in elementIdx0 attackIdx 1 if it is in the correct position.
        private float projShootSpeed;
        private int projDamage;
        private int shotProjectiles; // use to keep track of how many projectiles were shot;

        private bool shotTornado = false;
        public enum Element
        {
            Heat,
            Dust,
            Light,
            Chloro,
            Aqua,
            Cold, 
            Glow,
            Electric,
            Dark,
            Life,
            Death
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 120;
            NPC.defense = 50;
            NPC.lifeMax = 300000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.scale = 3f;
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 20f; // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Default buff immunities should be set in SetStaticDefaults through the NPCID.Sets.ImmuneTo{X} arrays.
            // To dynamically adjust immunities of an active NPC, NPC.buffImmune[] can be changed in AI: NPC.buffImmune[BuffID.OnFire] = true;
            // This approach, however, will not preserve buff immunities. To preserve buff immunities, use the NPC.BecomeImmuneTo and NPC.ClearImmuneToBuffs methods instead, as shown in the ApplySecondStageBuffImmunities method below.

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // Custom boss bar
            // NPC.BossBar = ModContent.GetInstance<MinionBossBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                // Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ropocalypse2");
            }
        }

        public override void AI()
        {
            
            /* ------- Targeting ------- */
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                NPC.velocity.Y -= 0.04f;
                NPC.EncourageDespawn(10);
                return;
            }
            CheckPlayerDistance(); // Adds a damaging debuff if the player is too far from the boss.

            /* ------- Movement and Attack ------- */

            switch (elementIdx)
            {
                case 0:
                    Heat_AI(player);
                    break;
                case 1:
                    Dust_AI(player);
                    break;
                case 2:
                    Light_AI(player);
                    break;
                case 3:
                    Chloro_AI(player);
                    break;
                case 4:
                    Aqua_AI(player);
                    break;
                case 5:
                    Cold_AI(player);
                    break;
                case 6:
                    Glow_AI(player);
                    break;
                case 7:
                    Electric_AI(player);
                    break;
                case 8:
                    Dark_AI(player);
                    break;
                case 9:
                    Life_AI(player);
                    break;
                case 10:
                    Death_AI(player);
                    break;
            }
            
            /* ------- Assigning Phases ------- */
            if (NPC.life <= NPC.lifeMax / 2 && !secondStage)
            {
                doPhaseTransitionAnimation = true;
            }

            if (doPhaseTransitionAnimation)
            {
                NPC.velocity = Vector2.Zero;
                NPC.dontTakeDamage = true;
                phaseTransitionTimer++;
                if (phaseTransitionTimer > 180)
                {
                    secondStage = true;
                    doPhaseTransitionAnimation = false;
                    MoveToNextElement();
                    elementIdx = 0;
                    NPC.dontTakeDamage = false;
                }
            }

            NPC.ai[0]++;
        }

        private void CheckPlayerDistance()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player != null && player.active)
                {
                    float distanceToPlayer = Vector2.Distance(NPC.Center, player.Center);

                    if (distanceToPlayer > 1200)
                    {
                        player.AddBuff(ModContent.BuffType<DimensionalIncompatability>(), 2);
                    }
                }
            }
        }

        /* ------ HEAT ------ */
        private void Heat_AI(Player player)
        {
            Heat_Movement(player, out float distanceFromPlayer, out bool inPosition);
            Heat_Attack(player, distanceFromPlayer, inPosition);
        }
        private void Heat_Movement(Player player, out float distanceFromPlayer, out bool inPosition)
        {
            distanceFromPlayer = Vector2.Distance(NPC.Center, player.Center);
            Vector2 goToPosition = Vector2.Zero;
            Vector2 vectorToPosition = Vector2.Zero;
            float distanceFromGoToPosition = float.MaxValue;
            inPosition = false;

            if (!secondStage)
            {
                // initialize values
                moveSpeed = 20f;
                inertia = 45f;

                // do movement (based on attack type)
                if (attackIdx == 0)
                {
                    goToPosition = player.Center + new Vector2(0, -300);
                    vectorToPosition = goToPosition - NPC.Center;
                    vectorToPosition.Normalize();
                    vectorToPosition *= moveSpeed;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, vectorToPosition, .01f);
                }
                else if (attackIdx == 1)
                {
                    goToPosition = fixedGoToPosition;
                    distanceFromGoToPosition = Vector2.Distance(goToPosition, NPC.Center);

                    float idleOffsetAmplitude = 6f;
                    float idleSpeed = 0.05f;
                    Vector2 idleOffset = new Vector2((float)Math.Sin(Main.GameUpdateCount * idleSpeed) * idleOffsetAmplitude, (float)Math.Cos(Main.GameUpdateCount * idleSpeed * 1.2f) * idleOffsetAmplitude * 0.5f);

                    if (distanceFromGoToPosition < 10f)
                    {
                        inPosition = true; 
                        Vector2 idleTarget = goToPosition + idleOffset;
                        Vector2 toIdle = idleTarget - NPC.Center;

                        NPC.velocity = toIdle * 0.1f;
                    }
                    else
                    {
                        vectorToPosition = goToPosition - NPC.Center;
                        vectorToPosition.Normalize();
                        vectorToPosition *= moveSpeed;
                        NPC.velocity = vectorToPosition;
                    }
                }
            }
            else
            {
                // initialize values
                moveSpeed = 25f;
            }
        }
        private void Heat_Attack(Player player, float distanceFromPlayer, bool inPosition)
        {
            if (!secondStage) // first phase attack sequence
            {
                /* - - - - - Handle attack types - - - - - */
                if (elementIdxTimer == 0) // First part of heat attacks
                {
                    // initialize values
                    attackIdx = 0;
                    projShootSpeed = 20f;
                    projDamage = 50;
                }
                if (elementIdxTimer >= 420) // second part of heat attacks
                {
                    // initialize values
                    if (!assignedGoToPosition)
                    {
                        fixedGoToPosition = player.Center + new Vector2(Main.rand.Next(300) - 150, Main.rand.Next(300) - 150);
                        assignedGoToPosition = true;
                    }
                    attackIdx = 1;
                    projShootSpeed = 10f;
                    projDamage = 65;
                }
                if (attackIdx == 1 && shotProjectiles >= 12) // Move onto next element
                {
                    MoveToNextElement();
                    return;
                }

                /* - - - - - Do attacks - - - - - */
                if (attackIdx == 0) // constantly shoots flames
                {

                    attackPhaseTimer++; // incremented first so it doesnt shoot on spawn
                    if (attackPhaseTimer % 20 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                        Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * projShootSpeed, ModContent.ProjectileType<HEAT_FlameBarageProj>(), projDamage, 8f);
                    }
                }

                if (attackIdx == 1 && inPosition) // Shoots an inferno barage
                {
                    if (attackPhaseTimer % 15 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                        Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(20) - 10)) * projShootSpeed, ModContent.ProjectileType<HEAT_InfernoMissle>(), projDamage, 10f);
                        shotProjectiles++;
                    }
                    attackPhaseTimer++;
                }
                elementIdxTimer++;
            }
            else // second phase attack sequence
            {

            }
        }

        /* ------ DUST ------ */
        private void Dust_AI(Player player)
        {
            Dust_Movement(player);
            Dust_Attack(player);
        }
        private void Dust_Movement(Player player)
        {
            if (attackPhaseTimer == 45)
            {
                fixedGoToPosition = player.Center + new Vector2(0, -300);
                NPC.Center = fixedGoToPosition;
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DustCloud>(), 0, 0, Main.rand.Next(50), Color.Orange, 1f);
                }
            }

            if (!assignedGoToPosition)
            {
                fixedGoToPosition = NPC.Center;
                assignedGoToPosition = true;
            }
            float idleOffsetAmplitude = 6f;
            float idleSpeed = 0.05f;
            Vector2 idleOffset = new Vector2((float)Math.Sin(Main.GameUpdateCount * idleSpeed) * idleOffsetAmplitude, (float)Math.Cos(Main.GameUpdateCount * idleSpeed * 1.2f) * idleOffsetAmplitude * 0.5f);

            Vector2 idleTarget = fixedGoToPosition + idleOffset;
            Vector2 toIdle = idleTarget - NPC.Center;

            NPC.velocity = toIdle * 0.1f;
        }
        private void Dust_Attack(Player player)
        {
            attackPhaseTimer++;

            if (attackPhaseTimer > 65)
            {

                if (!shotTornado)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(NPC), NPC.Center, new Vector2(15, 0), ModContent.ProjectileType<DUST_DustNadoBase_RIGHT>(), projDamage, 0f);
                    shotTornado = true;
                }
            }

            if (attackPhaseTimer > 110)
            {
                MoveToNextElement();
            }
        }
        /* ------ LIGHT ------ */

        private void Light_AI(Player player)
        {

        }

        private void Light_Movement(Player player)
        {

        }

        private void Light_Attack(Player player)
        {

        }

        /* ------ CHLORO ------ */

        private void Chloro_AI(Player player)
        {

        }

        private void Chloro_Movement(Player player)
        {

        }

        private void Chloro_Attack(Player player)
        {

        }

        /* ------ AQUA ------ */

        private void Aqua_AI(Player player)
        {

        }

        private void Aqua_Movement(Player player)
        {

        }

        private void Aqua_Attack(Player player)
        {

        }

        /* ------ COLD ------ */

        private void Cold_AI(Player player)
        {

        }

        private void Cold_Movement(Player player)
        {

        }

        private void Cold_Attack(Player player)
        {

        }

        /* ------ GLOW ------ */

        private void Glow_AI(Player player)
        {

        }

        private void Glow_Movement(Player player)
        {

        }

        private void Glow_Attack(Player player)
        {

        }

        /* ------ ELECTRIC ------ */

        private void Electric_AI(Player player)
        {

        }

        private void Electric_Movement(Player player)
        {

        }

        private void Electric_Attack(Player player)
        {

        }

        /* ------ DARK ------ */

        private void Dark_AI(Player player)
        {

        }

        private void Dark_Movement(Player player)
        {

        }

        private void Dark_Attack(Player player)
        {

        }

        /* ------ LIFE ------ */

        private void Life_AI(Player player)
        {

        }

        private void Life_Movement(Player player)
        {

        }

        private void Life_Attack(Player player)
        {

        }

        /* ------ DEATH ------ */

        private void Death_AI(Player player)
        {

        }

        private void Death_Movement(Player player)
        {

        }

        private void Death_Attack(Player player)
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // draw trail
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, NPC.height * 0.5f);

            //draw main
            Main.EntitySpriteDraw(texture,
                new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f, NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f + 230),
                NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, drawOrigin, NPC.scale, effects, 0);

            return false;

        }

        private void MoveToNextElement()
        {
            elementIdx = (elementIdx + 1) % 11;
            moveSpeed = 10f;
            assignedGoToPosition = false;

            attackIdx = 0;
            elementIdxTimer = 0;
            attackPhaseTimer = 0;

            projShootSpeed = 2f;
            projDamage = 10;
            shotProjectiles = 0;
            Main.NewText("Element Index:" + elementIdx);


            shotTornado = false;
        }
    }

}
