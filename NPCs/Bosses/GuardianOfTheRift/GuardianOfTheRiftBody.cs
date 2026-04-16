using System;
using System.Collections.Generic;
using System.IO;
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
using TheTesseractMod.Systems;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Minimap;
using TheTesseractMod.Projectiles.Enemy;
using Humanizer;
using ReLogic.Utilities;
using Microsoft.CodeAnalysis.Differencing;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using TheTesseractMod.NPCs.CustomGore;
using TheTesseractMod.Items.Consumables;
using TheTesseractMod.Items.Weapons.Melee;
using TheTesseractMod.Items.Weapons.Magic;
using TheTesseractMod.Items.Weapons.Ranged;
using TheTesseractMod.Items.Weapons.Summoner;
using TheTesseractMod.Items.Ores;

namespace TheTesseractMod.NPCs.Bosses.GuardianOfTheRift
{
    [AutoloadBossHead]
    internal class GuardianOfTheRiftBody  : ModNPC
    {
        public override string Texture => "TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardianBase_v4";

        // phase is a number 0-10 to represent which element to use: 0 heat, 1 dust, 2 light, 3 chloro, 4 aqua, 5 cold, 6 glow, 7 electric, 8 dark, 9 life , 10 death
        public int elementIdx = (int)Element.Electric; // start with dust phase for testing, will change to 0 for heat when heat phase is readyr
        private int previousElementIdx = (int)Element.Electric - 1; // used to detect when the element changes, so that the attack pattern can be reset
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
        
        public static bool glowPhaseActive = false; // used by GuardianDarknessSystem to darken the screen
        public static float darkPhaseIntensity = 0f; // 0 = no darkness, 1 = full darkness. Used by GuardianDarknessSystem.
        public static bool temporalLoopBackActive = false; // used by GuardianDarknessSystem for blue pulse

        private int animationFrameCounter = 0; // Counter for 12 fps animation
        private float spikesRotation = 0f; // Rotation for spikes sprite

        // HEAT ATTACKS
        int flameRadiusMax = 1000;
        int flameRadiusMin = 400;
        int currentFlameRadius = 1000;
        int flameRadiusTimer = 120;
        int flameRadiusPhaseTimer = 0;

        // DUST ATTACKS
        private bool shotTornado = false;
        private int spawnedBoulders = 0;
        private int initialDamage = 0;
        private int initialDefense = 0;
        private float dustIncreaseInDefense = 0f;
        private float dustIncreaseInDamage = 0f;

        private int timeToKillBoulders = 1200; // 20 seconds
        private List<NPC> dustBoulders = new List<NPC>();
        float minionOrbitRadius = 200f;

        // LIGHT ATTACKS
        private int lightDeathRayChargeTime = 180; // how long it takes for the death ray to charge up. 60 frames is idle, then teleport, then charge for 120 frames
        private int lightDeathRayTimer = 180; // How long the death ray persists
        private float lightDeathRayAngle = 0f; // The angle at which the death ray shoots, and follows the direction of the player
        private bool cloakBoss = false; // whether or not the boss is currently cloaked (invisible and intangible) during the light phase teleport attack
        private bool spawnedTrueMinion = false;
        private List<NPC> spawnedMinions = new List<NPC>(); // All minion NPCs
        private List<int> minionPlayerAssignment = new List<int>(); // Which player each minion belongs to
        private List<int> minionPositionAssignment = new List<int>(); // Which position (0-3) each minion is at
        private NPC trueMinion = null; // The true minion NPC
        private int minionPositionSwitchTimer = 0; // Timer to switch positions every 120 frames (2 seconds)

        // COLD ATTACKS
        private NPC iceShield = null; // The ice shield minion NPC

        // ELECTRIC ATTACKS
        private List<Vector2> attackDirections = new List<Vector2>(); // The directions of the electric attacks, used to draw the telegraph line in the correct direction

        // DARK ATTACKS
        private int darkDeathRayTimer = 180;

        // GLOW ATTACKS
        private bool dashing = false;
        Vector2 dashDirection = Vector2.Zero;

        // LIFE ATTACKS
        List<NPC> lifeMinions = new List<NPC>();
        private int lifeMinionSpawned = 0;
        private int lifeRegenTimer = 0;

        // TEMPORAL LOOP BACK ATTACKS
        List<int> playerHealthCounts = new List<int>();
        List<Vector2> playerPositions = new List<Vector2>();
        List<Vector2> playerVelocities = new List<Vector2>();

        // CLOCK HANDS VARIABLES:
        private float minuteHandRotation = 0f;
        private float hourHandRotation = 30f;
        private int timeTransitionTimer = 0;
        private bool elementTransitioning = false;

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
            Death,
            TemporalLoopBack
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				CustomTexturePath = "TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardianBase_v4",
				PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,
			};
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 85;
            NPC.height = 85;
            NPC.damage = 160;
            NPC.defense = 75;
            NPC.lifeMax = 410000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(platinum: 1, gold: 15);
            NPC.scale = 3f;
            NPC.SpawnWithHigherTime(30);
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                NPC.lifeMax += 35000;
                NPC.damage += 40;
                NPC.defense += 20;
            }

            NPC.boss = true;
            NPC.npcSlots = 20f; // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Default buff immunities should be set in SetStaticDefaults through the NPCID.Sets.ImmuneTo{X} arrays.
            // To dynamically adjust immunities of an active NPC, NPC.buffImmune[] can be changed in AI: NPC.buffImmune[BuffID.OnFire] = true;
            // This approach, however, will not preserve buff immunities. To preserve buff immunities, use the NPC.BecomeImmuneTo and NPC.ClearImmuneToBuffs methods instead, as shown in the ApplySecondStageBuffImmunities method below.

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            NPC.BossBar = ModContent.GetInstance<GuardianOfTheRiftBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                // Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ropocalypse2");
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("The ancient evil plaguing your world seems to have taken control of a powerful sundial. Awakend with rage by your slaughtering of its rift minions, it has traveled through space and time to ensure you do not succeed in galactic domination. What powers await upon its defeat?")
			});
        }
		
        public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

			// The order in which you add loot will appear as such in the Bestiary. To mirror vanilla boss order:
			// 1. Trophy
			// 2. Classic Mode ("not expert")
			// 3. Expert Mode (usually just the treasure bag)
			// 4. Master Mode (relic first, pet last, everything else in between)

			// Trophies are spawned with 1/10 chance
			// npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>(), 10));

			// All the Classic Mode drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Ores.TemporalOre>(), 1, 17, 26)); // Always drops between 20 and 30 Temporal Ore
            
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SoliumOre>(), 1, 20, 35));
            notExpertRule.OnSuccess(ItemDropRule.OneFromOptionsNotScalingWithLuck(1,
                ModContent.ItemType<PowerHammer>(),
                ModContent.ItemType<SiphonAxes>(),
                ModContent.ItemType<RiftFracture>(),
                ModContent.ItemType<StormOfThorns>(),
                ModContent.ItemType<DragonsBreath>(),
                ModContent.ItemType<BlizzardCannon>(),
                ModContent.ItemType<SquidOfTheAbyssScepter>(),
                ModContent.ItemType<WhipOfTheWildWest>()
            )); // Always drops one of the weapons, regardless of luck

			// Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
			// Boss masks are spawned with 1/7 chance
			// notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

			// This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
			// We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
			// which requires these parameters to be defined
			// int itemType = ModContent.ItemType<ExampleItem>();
			// var parameters = new DropOneByOne.Parameters() {
			// 	ChanceNumerator = 1,
			// 	ChanceDenominator = 1,
			// 	MinimumStackPerChunkBase = 1,
			// 	MaximumStackPerChunkBase = 1,
			// 	MinimumItemDropsCount = 12,
			// 	MaximumItemDropsCount = 15,
			// };

			// notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

			// Finally add the leading rule
			npcLoot.Add(notExpertRule);

			// Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TemporalGuardianTreasureBag>()));

			// ItemDropRule.MasterModeCommonDrop for the relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.TemporalGuardianRelic>()));

			// ItemDropRule.MasterModeDropOnAllPlayers for the pet
			npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Pets.EnchantedSunStone>(), 4));
		}

        public override void BossLoot(ref int potionType) {
			potionType = ItemID.SuperHealingPotion; 
		}
        public override void OnKill() {
			// Bless the world with Solium Ore on first kill
			if (!DownedBossSystem.downedTemporalGuardian) {
				ModContent.GetInstance<SoliumOreSystem>().BlessWorldWithSoliumOre();
			}

			// This sets downedTemporalGuardian to true, and if it was false before, it initiates a lantern night
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedTemporalGuardian, -1);

            // if I dont want a lantern night anymore, just do DownedBossSystem.downedTemporalGuardian = true;

			// Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
			// Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

			// If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
			/*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
		}

        public override void OnSpawn(IEntitySource source)
        {
            initialDamage = NPC.damage;
            initialDefense = NPC.defense;
            NPC.netUpdate = true;
        }

        public override bool CheckDead()
        {
            // Despawn minions when the boss dies
            foreach (NPC boulder in dustBoulders)
            {
                if (boulder.active)
                {
                    boulder.active = false;
                    boulder.life = 0;
                }
            }
            dustBoulders.Clear();
            glowPhaseActive = false;
            darkPhaseIntensity = 0f;
            temporalLoopBackActive = false;
            return base.CheckDead();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(elementIdx);
            writer.Write(attackIdx);
            writer.Write(elementIdxTimer);
            writer.Write(attackPhaseTimer);
            writer.Write(shotProjectiles);
            writer.Write(shotTornado);
            writer.Write(currentFlameRadius);
            writer.Write(spawnedBoulders);
            writer.Write(flameRadiusPhaseTimer);
            writer.Write(timeToKillBoulders);
            writer.Write(initialDamage);
            writer.Write(initialDefense);
            writer.Write(timeTransitionTimer);
            writer.Write(elementTransitioning);
            writer.Write(minuteHandRotation);
            writer.Write(hourHandRotation);
            writer.Write(lightDeathRayChargeTime);
            writer.Write(lightDeathRayTimer);
            writer.Write(lightDeathRayAngle);

            // Sync minion position switching
            writer.Write(spawnedTrueMinion);
            writer.Write(trueMinion != null ? trueMinion.whoAmI : -1);
            writer.Write(minionPositionSwitchTimer);
            writer.Write(spawnedMinions.Count);
            for (int i = 0; i < spawnedMinions.Count; i++)
            {
                writer.Write(spawnedMinions[i].whoAmI);
                writer.Write(minionPlayerAssignment[i]);
                writer.Write(minionPositionAssignment[i]);
            }
            writer.Write(cloakBoss);
            writer.Write(NPC.alpha);
            writer.Write(iceShield != null ? iceShield.whoAmI : -1);
            writer.Write(dashing);
            writer.Write(dashDirection.X);
            writer.Write(dashDirection.Y);
            writer.Write(darkDeathRayTimer);
            writer.Write(lifeMinionSpawned);
            writer.Write(lifeRegenTimer);
            writer.Write(lifeMinions.Count);
            for (int i = 0; i < lifeMinions.Count; i++)
            {
                writer.Write(lifeMinions[i].whoAmI);
            }
            writer.Write(playerHealthCounts.Count);
            for (int i = 0; i < playerHealthCounts.Count; i++)
            {
                writer.Write(playerHealthCounts[i]);
            }
            writer.Write(playerPositions.Count);
            for (int i = 0; i < playerPositions.Count; i++)
            {
                writer.Write(playerPositions[i].X);
                writer.Write(playerPositions[i].Y);
            }
            writer.Write(playerVelocities.Count);
            for (int i = 0; i < playerVelocities.Count; i++)
            {
                writer.Write(playerVelocities[i].X);
                writer.Write(playerVelocities[i].Y);
            }
            for (int i = 0; i < attackDirections.Count; i++)
            {
                writer.Write(attackDirections[i].X);
                writer.Write(attackDirections[i].Y);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            elementIdx = reader.ReadInt32();
            attackIdx = reader.ReadInt32();
            elementIdxTimer = reader.ReadInt32();
            attackPhaseTimer = reader.ReadInt32();
            shotProjectiles = reader.ReadInt32();
            shotTornado = reader.ReadBoolean();
            currentFlameRadius = reader.ReadInt32();
            spawnedBoulders = reader.ReadInt32();
            flameRadiusPhaseTimer = reader.ReadInt32();
            timeToKillBoulders = reader.ReadInt32();
            initialDamage = reader.ReadInt32();
            initialDefense = reader.ReadInt32();
            timeTransitionTimer = reader.ReadInt32();
            elementTransitioning = reader.ReadBoolean();
            minuteHandRotation = reader.ReadSingle();
            hourHandRotation = reader.ReadSingle();
            lightDeathRayChargeTime = reader.ReadInt32();
            lightDeathRayTimer = reader.ReadInt32();
            lightDeathRayAngle = reader.ReadSingle();

            // Sync minion position switching
            spawnedTrueMinion = reader.ReadBoolean();
            int trueMinionIdx = reader.ReadInt32();
            trueMinion = (trueMinionIdx > 0 && trueMinionIdx < Main.maxNPCs) ? Main.npc[trueMinionIdx] : null;
            minionPositionSwitchTimer = reader.ReadInt32();
            int minionCount = reader.ReadInt32();
            spawnedMinions.Clear();
            minionPlayerAssignment.Clear();
            minionPositionAssignment.Clear();
            for (int i = 0; i < minionCount; i++)
            {
                int minionIdx = reader.ReadInt32();
                if (minionIdx > 0 && minionIdx < Main.maxNPCs)
                {
                    spawnedMinions.Add(Main.npc[minionIdx]);
                }
                minionPlayerAssignment.Add(reader.ReadInt32());
                minionPositionAssignment.Add(reader.ReadInt32());
            }
            cloakBoss = reader.ReadBoolean();
            NPC.alpha = reader.ReadInt32();
            int iceShieldIndex = reader.ReadInt32();
            iceShield = (iceShieldIndex > 0 && iceShieldIndex < Main.maxNPCs) ? Main.npc[iceShieldIndex] : null;
            dashing = reader.ReadBoolean();
            dashDirection = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            darkDeathRayTimer = reader.ReadInt32();
            lifeMinionSpawned = reader.ReadInt32();
            lifeRegenTimer = reader.ReadInt32();
            lifeMinions.Clear();
            int lifeMinionCount = reader.ReadInt32();
            for (int i = 0; i < lifeMinionCount; i++)
            {
                int lifeMinionIdx = reader.ReadInt32();
                if (Main.npc[lifeMinionIdx].active)
                {
                    lifeMinions.Add(Main.npc[lifeMinionIdx]);
                }
            }
            int playerHealthCount = reader.ReadInt32();
            playerHealthCounts.Clear();
            for (int i = 0; i < playerHealthCount; i++)
            {
                playerHealthCounts.Add(reader.ReadInt32());
            }
            int playerPositionCount = reader.ReadInt32();
            playerPositions.Clear();
            for (int i = 0; i < playerPositionCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                playerPositions.Add(new Vector2(x, y));
            }
            int playerVelocityCount = reader.ReadInt32();
            playerVelocities.Clear();
            for (int i = 0; i < playerVelocityCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                playerVelocities.Add(new Vector2(x, y));
            }
            int attackDirectionCount = reader.ReadInt32();
            attackDirections.Clear();
            for (int i = 0; i < attackDirectionCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                attackDirections.Add(new Vector2(x, y));
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

            if (elementTransitioning)
            {
                TransitionToElement();
                return; // Skip the rest of the AI while transitioning
            }

            glowPhaseActive = elementIdx == (int)Element.Glow;
            temporalLoopBackActive = elementIdx == (int)Element.TemporalLoopBack;
            if (elementIdx != (int)Element.Dark)
                darkPhaseIntensity = 0f;

            switch (elementIdx)
            {
                case (int)Element.Heat:
                    Heat_AI(player);
                    break;
                case (int)Element.Dust:
                    Dust_AI(player);
                    break;
                case (int)Element.Light:
                    Light_AI(player);
                    break;
                case (int)Element.Chloro:
                    Chloro_AI(player);
                    break;
                case (int)Element.Aqua:
                    Aqua_AI(player);
                    break;
                case (int)Element.Cold:
                    Cold_AI(player);
                    break;
                case (int)Element.Glow:
                    Glow_AI(player);
                    break;
                case (int)Element.Electric:
                    Electric_AI(player);
                    break;
                case (int)Element.Dark:
                    Dark_AI(player);
                    break;
                case (int)Element.Life:
                    Life_AI(player);
                    break;
                case (int)Element.Death:
                    Death_AI(player);
                    break;
                case (int)Element.TemporalLoopBack:
                    TemporalLoopBack_AI(player);
                    break;
            }
            
            /* ------- Assigning Phases ------- */
            /*
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
                    elementIdx = (int)Element.Heat;
                    NPC.dontTakeDamage = false;
                }
            }
            */

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

                    if (distanceToPlayer > 1200 && !cloakBoss && elementIdx != (int)Element.Life && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        player.AddBuff(ModContent.BuffType<DimensionalIncompatability>(), 2);
                    }
                }
            }
        }

        /* ------ HEAT ------ */
        private void Heat_AI(Player player)
        {
            // Reset position tracking when entering heat phase
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
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
                    goToPosition = new Vector2(NPC.ai[1], NPC.ai[2]);
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
                if (elementIdxTimer >= 320) // second part of heat attacks
                {
                    // initialize values
                    if (!assignedGoToPosition)
                    {
                        Vector2 targetPos = player.Center + new Vector2(Main.rand.Next(300) - 150, Main.rand.Next(300) - 300);
                        NPC.ai[1] = targetPos.X;
                        NPC.ai[2] = targetPos.Y;
                        assignedGoToPosition = true;
                        NPC.netUpdate = true; // Force network sync
                    }
                    attackIdx = 1;
                    flameRadiusPhaseTimer = 0;
                    projShootSpeed = 10f;
                    projDamage = 65;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && attackIdx == 1 && shotProjectiles >= 12) // Move onto next element
                {
                    // Snapshot all player states for the temporal loop back
                    playerHealthCounts.Clear();
                    playerPositions.Clear();
                    playerVelocities.Clear();
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player p = Main.player[i];
                        playerHealthCounts.Add(p.active && !p.dead ? p.statLife : 0);
                        playerPositions.Add(p.active && !p.dead ? p.position : Vector2.Zero);
                        playerVelocities.Add(p.active && !p.dead ? p.velocity : Vector2.Zero);
                    }
                    elementTransitioning = true;
                    NPC.netUpdate = true;
                    return;
                }

                /* - - - - - Do attacks - - - - - */
                if (attackIdx == 0) // constantly shoots flames
                {

                    attackPhaseTimer++; // incremented first so it doesnt shoot on spawn
                    if (attackPhaseTimer % 20 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * projShootSpeed, ModContent.ProjectileType<HEAT_FlameBarageProj>(), projDamage, 8f);
                        }
                    }

                    
                }

                if (attackIdx == 1 && inPosition)
                {
                    // Phase 1: Shrinking radius
                    if (currentFlameRadius > flameRadiusMin)
                    {
                        // Decrease radius each frame
                        float radiusDecrement = (float)(flameRadiusMax - flameRadiusMin) / flameRadiusTimer;
                        currentFlameRadius -= (int)radiusDecrement;
                    }

                    // Spawn dusts around the boss in a circle (both phases)
                    int dustCount = 40;
                    for (int i = 0; i < dustCount; i++)
                    {
                        float angle = MathHelper.ToRadians(Main.rand.Next(360));
                        Vector2 dustPos = NPC.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * currentFlameRadius;

                        Dust torchDust = Dust.NewDustDirect(dustPos, 0, 0, DustID.Torch, 0, 0);
                        torchDust.noGravity = true;
                        torchDust.scale = 1.5f;

                        Dust redTorchDust = Dust.NewDustDirect(dustPos, 0, 0, DustID.RedTorch, 0, 0);
                        redTorchDust.noGravity = true;
                        redTorchDust.scale = 1.5f;
                    }

                    // Apply debuff to players outside the radius
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player p = Main.player[i];
                        if (p.active && !p.dead)
                        {
                            float distanceToPlayer = Vector2.Distance(NPC.Center, p.Center);
                            if (distanceToPlayer > currentFlameRadius && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                p.AddBuff(ModContent.BuffType<DimensionalIncompatability>(), 2);
                            }
                        }
                    }

                    // Phase 2: Shoot projectiles after radius shrinking is complete
                    if (currentFlameRadius <= flameRadiusMin)
                    {
                        if (attackPhaseTimer % 15 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(20) - 10)) * projShootSpeed, ModContent.ProjectileType<HEAT_InfernoMissle>(), projDamage, 10f);
                                shotProjectiles++;
                            }
                        }
                        attackPhaseTimer++;
                    }
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
            // Reset position tracking when entering dust phase
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Dust_Movement(player);
            Dust_Attack(player);
        }
        private void Dust_Movement(Player player)
        {
            // Set initial position on first frame
            if (!assignedGoToPosition)
            {
                fixedGoToPosition = NPC.Center;
                assignedGoToPosition = true;
                NPC.velocity = Vector2.Zero;
            }

            if (attackPhaseTimer == 120)
            {
                Vector2 teleportPos = player.Center + new Vector2(0, -300);
                TeleportToPosition(teleportPos);
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DustCloud>(), 0, 0, Main.rand.Next(50), Color.Orange, 1f);
                }
            }

            if (attackPhaseTimer > 120)
            {
                Heat_Movement(player, out float _, out bool _); // reuse heat movement for dust phase teleport movement
            }
            else
            {
                IdleMovement(fixedGoToPosition);
            }

            // Update boulder positions to orbit around boss (use synced attackPhaseTimer instead of GameUpdateCount)
            for (int i = 0; i < dustBoulders.Count; i++)
            {
                NPC boulder = dustBoulders[i];
                if (boulder.active)
                {
                    float angle = (float)(Math.PI * 2 / 4) * i + (attackPhaseTimer * 0.02f); // Use attackPhaseTimer (synced)
                    Vector2 orbitPos = NPC.Center + new Vector2((float)Math.Cos(angle) * minionOrbitRadius, (float)Math.Sin(angle) * minionOrbitRadius);
                    boulder.Center = orbitPos;
                    boulder.velocity = Vector2.Zero;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        boulder.netUpdate = true;
                    }
                }
            }
        }
        private void Dust_Attack(Player player)
        {
            attackPhaseTimer++;

            // Spawn boulders on sequential frames (one per frame)
            if (attackPhaseTimer >= 120 && attackPhaseTimer <= 123 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                float angle = (float)(Math.PI * 2 / 4) * (attackPhaseTimer - 120);
                Vector2 spawnPos = NPC.Center + new Vector2((float)Math.Cos(angle) * minionOrbitRadius, (float)Math.Sin(angle) * minionOrbitRadius);
                NPC boulderNPC = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<DustBoulderMinion>());
                if (boulderNPC.whoAmI != Main.maxNPCs)
                {
                    dustBoulders.Add(boulderNPC);
                }
                NPC.netUpdate = true;
            }

            // Track remaining boulders and apply buffs
            dustBoulders.RemoveAll(b => !b.active || b.type != ModContent.NPCType<DustBoulderMinion>());
            int remainingBoulders = dustBoulders.Count;
            spawnedBoulders = remainingBoulders;

            // Decrement time to kill boulders
            timeToKillBoulders--;

            // Move to next phase if all boulders are destroyed (after they spawn) OR time expires
            if (Main.netMode != NetmodeID.MultiplayerClient && attackPhaseTimer > 120 && (remainingBoulders == 0 || timeToKillBoulders <= 0))
            {
                // Apply damage and defense increase based on remaining boulders (more alive = more buffed)
                dustIncreaseInDefense = remainingBoulders * 30f;
                dustIncreaseInDamage = remainingBoulders * 25f;
                NPC.defense = initialDefense + (int)dustIncreaseInDefense;
                NPC.damage = initialDamage + (int)dustIncreaseInDamage;

                foreach (NPC boulder in dustBoulders)
                {
                    boulder.life = 0;
                    boulder.netUpdate = true;
                }
                dustBoulders.Clear();

                elementTransitioning = true;
                NPC.netUpdate = true;
            }

            if (attackPhaseTimer > 150 && (attackPhaseTimer - 150) % 60 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15f, ModContent.ProjectileType<DUST_dustcloud>(), NPC.damage, 0f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(30)) * 15f, ModContent.ProjectileType<DUST_dustcloud>(), NPC.damage, 0f);
                     Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-30)) * 15f, ModContent.ProjectileType<DUST_dustcloud>(), NPC.damage, 0f);
                
                }
            }
        }
        /* ------ LIGHT ------ */

        private void Light_AI(Player player)
        {
            // Reset position tracking when entering dust phase
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Light_Movement(player);
            Light_Attack(player);
        }

        private void Light_Movement(Player player)
        {
            if (attackIdx == 0)
            {
                // idle movement while charging death ray
                if (lightDeathRayChargeTime > 120) {
                    NPC.velocity = Vector2.Zero;  // Add this
                }
                if (lightDeathRayChargeTime == 120)
                {
                    fixedGoToPosition = player.Center + new Vector2(0, -300);
                    TeleportToPosition(fixedGoToPosition);
                }
                else if (lightDeathRayChargeTime > 120)
                {
                    IdleMovement(NPC.Center);
                }

                // charge up death ray for 2 seconds
                if (lightDeathRayChargeTime > 0)
                {
                    if (lightDeathRayChargeTime < 120)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 dustPos = NPC.Center + new Vector2(70, 0).RotatedByRandom(MathHelper.ToRadians(360));                                                                                                                 
                            Vector2 direction = (dustPos - NPC.Center).SafeNormalize(Vector2.Zero) * -7.5f; // Points towards center
                            int dust = Dust.NewDust(dustPos, 1, 1, ModContent.DustType<SharpRadialGlowDust>(), direction.X, direction.Y, 100, Color.Yellow, 1f);                                                                                                  
                            Main.dust[dust].noGravity = true; 
                        }
                    }
                    lightDeathRayChargeTime--;
                }
            }
            else if (attackIdx == 1)
            {
                float distanceToPlayer = Vector2.Distance(NPC.Center, player.Center);
                if (distanceToPlayer > 2000f)
                {
                    fixedGoToPosition = player.Center + new Vector2(0, -300).RotatedByRandom(MathHelper.ToRadians(360));
                    TeleportToPosition(fixedGoToPosition);
                }
                else
                {
                    Vector2 directionAwayFromPlayer = (NPC.Center - player.Center).SafeNormalize(Vector2.Zero);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, directionAwayFromPlayer * 5f, 0.02f);
                }
            }
            else if (attackIdx == 2)
            {
                if (cloakBoss) // if cloaked, just stay above the player
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = player.Center + new Vector2(0, -700);
                }
                else if (attackPhaseTimer == 30)
                {
                    Vector2 teleportPos = player.Center + new Vector2(0, -700); // teleport then cloak
                    TeleportToPosition(teleportPos);
                }
                else if (attackPhaseTimer < 30) // idle movement for first 30 frames
                {
                    IdleMovement(NPC.Center);
                }
            }
        }

        private void Light_Attack(Player player)
        {
            if (attackIdx == 0 && lightDeathRayChargeTime == 0) // death ray charge
            {
                attackIdx++;
                lightDeathRayTimer = 180;
                lightDeathRayAngle = (player.Center - NPC.Center).ToRotation();
                SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);

                // Spawn the death ray projectile
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float playerAngle = (player.Center - NPC.Center).ToRotation();
                    float initialRotation = playerAngle - MathHelper.ToRadians(-20);
                    int projIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LIGHT_lightDeathRay>(), NPC.damage * 2, 10f, ai0: NPC.whoAmI, ai1: initialRotation);
                    Main.projectile[projIndex].owner = player.whoAmI;  // Set owner to prevent despawn based on distance

                    initialRotation = playerAngle - MathHelper.ToRadians(100);
                    projIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LIGHT_lightDeathRay>(), NPC.damage * 2, 10f, ai0: NPC.whoAmI, ai1: initialRotation);
                    Main.projectile[projIndex].owner = player.whoAmI;  // Set owner to prevent despawn based on distance

                    initialRotation = playerAngle - MathHelper.ToRadians(220);
                    projIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LIGHT_lightDeathRay>(), NPC.damage * 2, 10f, ai0: NPC.whoAmI, ai1: initialRotation);
                    Main.projectile[projIndex].owner = player.whoAmI;  // Set owner to prevent despawn based on distance
                }                                                                                                                                                                                                                   
            }
            else if (attackIdx == 1) // death ray active
            {
                lightDeathRayTimer--;
                for (int i = 0; i < 5; i++)
                {                                                                                                                
                    Vector2 direction = new Vector2(15f, 0).RotatedByRandom(MathHelper.ToRadians(360)); // Random direction
                    int dust = Dust.NewDust(NPC.Center, 1, 1, ModContent.DustType<SharpRadialGlowDust>(), direction.X, direction.Y, 100, Color.Yellow, 1f);                                                                                                  
                    Main.dust[dust].noGravity = true; 
                }

                if (lightDeathRayTimer <= 0)
                {
                    attackIdx++;
                    NPC.netUpdate = true;
                }
            }
            else if (attackIdx == 2) // minion dance
            {
                // wait 30 frames, then:
                if (attackPhaseTimer == 30)
                {
                    cloakBoss = true;
                    NPC.dontTakeDamage = true;
                    NPC.alpha = 255; // Make boss invisible
                    NPC.netUpdate = true;
                    
                    int playerCount = Main.player.Length;

                    // Find active players and pick one random player + position for the true minion
                    int truePlayerIdx = -1;
                    int truePositionIdx = -1;

                    for (int i = 0; i < playerCount; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead)
                        {
                            if (truePlayerIdx == -1)
                            {
                                truePlayerIdx = i;
                            }
                        }
                    }

                    if (truePlayerIdx != -1)
                    {
                        truePositionIdx = Main.rand.Next(4);
                    }

                    // spawn minions around each active player in a cross pattern, with one random minion being the true one and the rest being fake ones
                    for (int i = 0; i < playerCount; i++)
                    {
                        Player p = Main.player[i];
                        if (p.active && !p.dead)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                Vector2 spawnPos = p.Center + new Vector2(0, -200).RotatedBy(MathHelper.ToRadians(90 * j)); // Spawn positions in a cross pattern around the player);

                                if (!spawnedTrueMinion && i == truePlayerIdx && j == truePositionIdx && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    // Spawn the one true minion
                                    NPC minionNPC = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<TrueMiniGuardian>());
                                    if (minionNPC.whoAmI != Main.maxNPCs)
                                    {
                                        minionNPC.target = p.whoAmI;
                                        spawnedMinions.Add(minionNPC);
                                        minionPlayerAssignment.Add(i);
                                        minionPositionAssignment.Add(j);
                                        trueMinion = minionNPC;
                                        spawnedTrueMinion = true;
                                    }
                                }
                                else
                                {
                                    // Spawn fake minions that just fly upwards and despawn after a short time
                                    NPC fakeMinionNPC = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<MiniGuardian>());
                                    if (fakeMinionNPC.whoAmI != Main.maxNPCs)
                                    {
                                        fakeMinionNPC.velocity = new Vector2(0, -5f);
                                        spawnedMinions.Add(fakeMinionNPC);
                                        minionPlayerAssignment.Add(i);
                                        minionPositionAssignment.Add(j);
                                    }
                                }
                            }
                        }
                    }
                    minionPositionSwitchTimer = 0; // Reset timer at spawn
                }

                // Check if true minion was killed
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    bool trueMinionAlive = trueMinion != null && trueMinion.active;

                    if (!trueMinionAlive && spawnedTrueMinion)
                    {
                        // True minion is dead - despawn all fake minions and move to next element
                        foreach (NPC minion in spawnedMinions)
                        {
                            if (minion.active)
                            {
                                minion.life = 0;
                                minion.netUpdate = true;
                            }
                        }

                        NPC.alpha = 0; // Make boss visible again
                        cloakBoss = false;
                        NPC.netUpdate = true;

                        // Force sync immediately
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }

                        elementTransitioning = true;
                        NPC.netUpdate = true;
                        return;
                    }
                }

                // Handle minion position switching every 2 seconds (120 frames)
                if (cloakBoss && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    minionPositionSwitchTimer++;

                    if (minionPositionSwitchTimer >= 120)
                    {
                        minionPositionSwitchTimer = 0;
                        SwitchMinionPositions();
                        NPC.netUpdate = true;
                    }
                }

                // Update minion positions and remove despawned ones
                for (int i = spawnedMinions.Count - 1; i >= 0; i--)
                {
                    NPC minion = spawnedMinions[i];
                    if (minion.active)
                    {
                        Player targetPlayer = Main.player[minionPlayerAssignment[i]];
                        int posIdx = minionPositionAssignment[i];
                        Vector2 targetPos = targetPlayer.Center + new Vector2(0, -200).RotatedBy(MathHelper.ToRadians(90 * posIdx));
                        minion.Center = targetPos;
                        minion.velocity = Vector2.Zero;
                    }
                    else
                    {
                        // Remove despawned minions from tracking
                        spawnedMinions.RemoveAt(i);
                        minionPlayerAssignment.RemoveAt(i);
                        minionPositionAssignment.RemoveAt(i);
                    }
                }

                attackPhaseTimer++;
            }
        }

        private void SwitchMinionPositions()
        {
            // Group minions by player
            Dictionary<int, List<int>> minionsByPlayer = new Dictionary<int, List<int>>();

            for (int i = 0; i < spawnedMinions.Count; i++)
            {
                int playerIdx = minionPlayerAssignment[i];
                if (!minionsByPlayer.ContainsKey(playerIdx))
                {
                    minionsByPlayer[playerIdx] = new List<int>();
                }
                minionsByPlayer[playerIdx].Add(i);
            }

            // Shuffle positions for each player's minions
            foreach (var kvp in minionsByPlayer)
            {
                List<int> minionListIndices = kvp.Value;
                if (minionListIndices.Count == 4) // Only shuffle if all 4 minions are still alive
                {
                    // Create a list of positions (0, 1, 2, 3) and shuffle them
                    List<int> positions = new List<int> { 0, 1, 2, 3 };
                    for (int i = positions.Count - 1; i > 0; i--)
                    {
                        int randomIdx = Main.rand.Next(i + 1);
                        // Swap
                        int temp = positions[i];
                        positions[i] = positions[randomIdx];
                        positions[randomIdx] = temp;
                    }

                    // Assign shuffled positions to minions
                    for (int i = 0; i < minionListIndices.Count; i++)
                    {
                        minionPositionAssignment[minionListIndices[i]] = positions[i];
                    }
                }
            }
        }

        /* ------ CHLORO ------ */

        private void Chloro_AI(Player player)
        {
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Chloro_Movement(player, out float distanceFromPlayer);
            Chloro_Attack(player, distanceFromPlayer);
        }

        private void Chloro_Movement(Player player, out float distanceFromPlayer)
        {
            moveSpeed = 10f;
            distanceFromPlayer = Vector2.Distance(NPC.Center, player.Center);
            Vector2 goToPosition = Vector2.Zero;
            Vector2 vectorToPosition = Vector2.Zero;

            goToPosition = player.Center;
            vectorToPosition = goToPosition - NPC.Center;
            vectorToPosition.Normalize();
            vectorToPosition *= moveSpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, vectorToPosition, .05f);
        }

        private void Chloro_Attack(Player player, float distanceFromPlayer)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 dustPos = NPC.Center + new Vector2(600, 0).RotatedByRandom(MathHelper.ToRadians(360));           
                int dust = Dust.NewDust(dustPos, 1, 1, DustID.JungleTorch, 0f, 0f, 100, default(Color), 1.5f);                                                                                                  
                Main.dust[dust].noGravity = true; 
            }

            if(distanceFromPlayer < 600f)
            {
                
                attackPhaseTimer++;
                if (attackPhaseTimer % 60 == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, player.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10f;
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CHLORO_NatureEssence>(), NPC.damage / 2, 0f);
                        Main.projectile[proj].Center = player.Center;
                    }
                }
                if (attackPhaseTimer % 2 == 0)
                {
                    Dust.NewDust(player.Center, 20, 20, DustID.JungleTorch, 0f, 0f, 0, Color.LimeGreen, 3f);
                }
            }

            // Every 45 frames, shoot homing leaf projectiles at the player
            if (elementIdxTimer % 45 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item101, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.player.Length; i++)
                    {
                        Player p = Main.player[i];
                        if (p.active && !p.dead)
                        {
                            Vector2 direction = (p.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction, ModContent.ProjectileType<CHLORO_LeafCrystal>(), NPC.damage, 0f);
                        }
                    }
                    NPC.netUpdate = true;
                }
            }
            if (elementIdxTimer >= 450)
            {
                TeleportToPosition(player.Center + new Vector2(0, -300));
                elementTransitioning = true;
            }
            NPC.netUpdate = true;
            elementIdxTimer++;
        }
        

        /* ------ AQUA ------ */

        private void Aqua_AI(Player player)
        {
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Aqua_Movement(player, out float distanceFromPlayer);
            Aqua_Attack(player, distanceFromPlayer);
        }

        private void Aqua_Movement(Player player, out float distanceFromPlayer)
        {
            distanceFromPlayer = Vector2.Distance(NPC.Center, player.Center);
            if (attackIdx == 0)
            {
                if (attackPhaseTimer == 0)
                {
                    Vector2 teleportPos = player.Center + new Vector2(0, -300);
                    TeleportToPosition(teleportPos);
                }
                if (attackPhaseTimer < 180)
                {
                    IdleMovement(NPC.Center);
                    NPC.dontTakeDamage = true;
                }
                else if (attackPhaseTimer >= 180)
                {
                    attackIdx++;
                    NPC.dontTakeDamage = false;
                    NPC.netUpdate = true;
                }
            }
            else
            {   
                moveSpeed = 20f;
                Vector2 goToPosition = player.Center + new Vector2(0, 450);
                Vector2 vectorToPosition = goToPosition - NPC.Center;
                vectorToPosition.Normalize();
                vectorToPosition *= moveSpeed;
                NPC.velocity = Vector2.Lerp(NPC.velocity, vectorToPosition, .025f);
            }
            attackPhaseTimer++;
        }

        private void Aqua_Attack(Player player, float distanceFromPlayer)
        {
            if(attackPhaseTimer == 60 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 direction = new Vector2(25f, 0f).RotatedBy(MathHelper.ToRadians(135));
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction, ModContent.ProjectileType<AQUA_AquanadoBolt>(), 0, 0f);

                direction = new Vector2(25f, 0f).RotatedBy(MathHelper.ToRadians(45));
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction, ModContent.ProjectileType<AQUA_AquanadoBolt>(), 0, 0f);
            }
            if (attackIdx == 1)
            {
                if (attackPhaseTimer % 80 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item155, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int min;
                        int max;
                        if (Main.masterMode)
                        {
                            min = 25;
                            max = 35;
                        }
                        else if (Main.expertMode)
                        {
                            min = 20;
                            max = 30;
                        }
                        else
                        {
                            min = 16;
                            max = 25;
                        }
                    

                        
                        int projCount = Main.rand.Next(min, max);
                        for (int i = 0; i < projCount; i++)
                        {
                            float speed = Main.rand.NextFloat(8f, 15f);
                            Vector2 direction = new Vector2(speed, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(60, 120)));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction, ModContent.ProjectileType<AQUA_AscendingBubble>(), NPC.damage, 0f);
                        }
                    }
                }
                if (attackPhaseTimer > 960)
                {
                    elementTransitioning = true;
                    NPC.netUpdate = true;
                    return;
                }
            }
        }

        /* ------ COLD ------ */

        private void Cold_AI(Player player)
        {
                if (previousElementIdx != elementIdx)
                {
                    assignedGoToPosition = false;
                    previousElementIdx = elementIdx;
                }
                Cold_Movement(player);
                Cold_Attack();
        }

        private void Cold_Movement(Player player)
        {
            NPC.dontTakeDamage = true;
            moveSpeed = 20f;
            Vector2 goToPosition = player.Center + new Vector2(0, 350);
            Vector2 vectorToPosition = goToPosition - NPC.Center;
            vectorToPosition.Normalize();
            vectorToPosition *= moveSpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, vectorToPosition, .025f);

            // ice shield
            if (iceShield != null && iceShield.active && iceShield.type == ModContent.NPCType<IceShield>())
            {
                // Position the ice shield minion above the boss
                iceShield.Center = NPC.Center;
                iceShield.velocity = Vector2.Zero;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    iceShield.netUpdate = true;
                }
            }

            if (attackPhaseTimer > 2 && (iceShield == null || !iceShield.active))
            {
                TeleportToPosition(player.Center + new Vector2(0, -300));
                 elementTransitioning = true;
                 NPC.netUpdate = true;
                 return;
            }
        }

        private void Cold_Attack()
        {
            // spawn shield
            if(attackPhaseTimer == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC shieldNPC = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<IceShield>());
                if (shieldNPC.whoAmI != Main.maxNPCs)
                {
                    iceShield = shieldNPC;
                }
                NPC.netUpdate = true;
            }

            // rain icicles
            if (attackPhaseTimer % 15 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.player.Length; i++)
                {
                    Player p = Main.player[i];
                    if (p.active && !p.dead)
                    {
                        Vector2 spawnPos = p.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), -1000f);
                        Vector2 direction = p.Center - spawnPos;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, direction * 10f, ProjectileID.FrostShard, NPC.damage / 2, 0f);

                    }
                }
            }

            if (attackPhaseTimer > 600)
            {
                 elementTransitioning = true;
                 NPC.netUpdate = true;
                 return;
            }

            attackPhaseTimer++;

        }

        private void DrawIceShield(Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/IceShield").Value;
            Main.EntitySpriteDraw(texture,
                new Vector2(NPC.position.X - screenPos.X + NPC.width * 0.5f, NPC.position.Y - screenPos.Y + NPC.height * 0.5f),
                null, new Color(.5f, .5f, .5f, .1f), NPC.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), NPC.scale, SpriteEffects.None, 0f);
        }

        /* ------ GLOW ------ */

        private void Glow_AI(Player player)
        {
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Glow_Movement(player);
            Glow_Attack(player);
        }

        private void Glow_Movement(Player player)
        {
            if (!dashing)
            {
                elementIdxTimer++;
            }
            if (elementIdxTimer % 30 == 0 && !dashing)
            {
                dashing = true;
            }
            if (!dashing)
            {
                IdleMovement(NPC.Center);
            }
            if (elementIdxTimer > 300)
            {
                elementTransitioning = true;
                NPC.netUpdate = true;
                return;
            }
        }

        private void Glow_Attack(Player player)
        {
            if(dashing)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SharpRadialGlowDust>(), 0f, 0f, 100, Color.Blue, 1.5f);
                if (attackPhaseTimer == 0)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    dashDirection = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    NPC.velocity = dashDirection * 32f;
                }

                // Decelerate during the last third of the dash
                if (attackPhaseTimer >= 20)
                {
                    float slowFactor = 1f - (float)(attackPhaseTimer - 20) / 10f;
                    NPC.velocity = dashDirection * 32f * slowFactor;
                }

                // spawn spore clouds
                if (attackPhaseTimer % 6 == 0)
                {
                    Vector2 upDirection = dashDirection.RotatedBy(MathHelper.ToRadians(90));
                    Vector2 downDirection = dashDirection.RotatedBy(MathHelper.ToRadians(-90));

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, upDirection * 10f * 0.99f, ModContent.ProjectileType<GlowRiftProjectile>(), NPC.damage, 0f);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, downDirection * 10f * 0.99f, ModContent.ProjectileType<GlowRiftProjectile>(), NPC.damage, 0f);
                    }
                }

                if (attackPhaseTimer >= 30)
                {
                    dashing = false;
                    NPC.velocity = Vector2.Zero;
                    attackPhaseTimer = 0;
                }
                else
                {
                    attackPhaseTimer++;
                }
            }
        }

        /* ------ ELECTRIC ------ */

        private void Electric_AI(Player player)
        {
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Electric_Movement(player);
            Electric_Attack(player);
        }

        private void Electric_Movement(Player player)
        {
            elementIdxTimer++;
            if (elementIdxTimer % 70 == 0)
            {
                Vector2 teleportPos = player.Center + new Vector2(0, -400).RotatedByRandom(MathHelper.ToRadians(360));
                TeleportToPosition(teleportPos);
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
            }
            else
            {
                IdleMovement(NPC.Center);
            }
            if (elementIdxTimer > 700)
            {
                elementTransitioning = true;
                NPC.netUpdate = true;
                return;
            }
        }

        private void Electric_Attack(Player player)
        {
            if (elementIdxTimer % 70 == 25)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    attackDirections.Clear();
                    for (int i = 0; i < Main.player.Length; i++)
                    {
                        Player p = Main.player[i];
                        if (p.active && !p.dead)
                        {
                            Vector2 direction = (p.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 5f;
                            attackDirections.Add(direction);
                        }
                    }
                }
            }
            if (elementIdxTimer % 70 == 55)
            {
                SoundEngine.PlaySound(SoundID.Thunder, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int directionIndex = 0;
                    for (int i = 0; i < Main.player.Length; i++)
                    {
                        Player p = Main.player[i];
                        if (p.active && !p.dead)
                        {
                            if (directionIndex < attackDirections.Count)
                            {
                                Vector2 direction = attackDirections[directionIndex];
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction, ModContent.ProjectileType<ELECTRIC_ElectricThunderbolt>(), NPC.damage, 0f);
                                for (int j = 0; j < 7; j++)
                                {
                                    Dust.NewDust(NPC.Center, 1, 1, ModContent.DustType<StormCloud1>(), direction.X, direction.Y, 50, default, 1f);
                                }
                            }
                            directionIndex++;
                        }
                    }
                    attackDirections.Clear();
                    NPC.netUpdate = true;
                }
            }
        }

        /* ------ DARK ------ */

        private void Dark_AI(Player player)
        {
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Dark_Movement(player);
            Dark_Attack(player);
        }

        private void Dark_Movement(Player player)
        {
            if (elementIdxTimer == 0)
            {
                flameRadiusMin = 550;
                Vector2 teleportPos = player.Center + new Vector2(0, -400).RotatedByRandom(MathHelper.ToRadians(360));
                TeleportToPosition(teleportPos);
            }
            else
            {
                IdleMovement(NPC.Center);
            }
        }

        private void Dark_Attack(Player player)
        {
            if (elementIdxTimer > 70)
            {
                // Darkness increases as radius shrinks: 0 at max radius, 1 at min radius
                darkPhaseIntensity = 1f - (float)(currentFlameRadius - flameRadiusMin) / (flameRadiusMax - flameRadiusMin);
                darkPhaseIntensity = MathHelper.Clamp(darkPhaseIntensity, 0f, 1f);

                // Shrinking radius + charge-up dust during shrink
                if (currentFlameRadius > flameRadiusMin)
                {
                    // Decrease radius each frame
                    float radiusDecrement = (float)(flameRadiusMax - flameRadiusMin) / flameRadiusTimer;
                    currentFlameRadius -= (int)radiusDecrement;

                    // Charge-up dust converging on boss while radius shrinks
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 chargeDustPos = NPC.Center + new Vector2(70, 0).RotatedByRandom(MathHelper.ToRadians(360));
                        Vector2 direction = (chargeDustPos - NPC.Center).SafeNormalize(Vector2.Zero) * -7.5f;
                        int dust = Dust.NewDust(chargeDustPos, 1, 1, ModContent.DustType<SharpRadialGlowDust>(), direction.X, direction.Y, 100, Color.Purple, 1f);
                        Main.dust[dust].noGravity = true;
                    }
                }

                // Spawn dusts around the boss in a circle (both phases)
                int dustCount = 80;
                for (int i = 0; i < dustCount; i++)
                {
                    float angle = MathHelper.ToRadians(Main.rand.Next(360));
                    Vector2 dustPos = NPC.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * currentFlameRadius;

                    Dust shadowDust = Dust.NewDustDirect(dustPos, 0, 0, DustID.Shadowflame, 0, 0);
                    shadowDust.noGravity = true;
                    shadowDust.scale = 1.5f;
                }

                // Apply debuff to players outside the radius
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];
                    if (p.active && !p.dead)
                    {
                        float distanceToPlayer = Vector2.Distance(NPC.Center, p.Center);
                        if (distanceToPlayer > currentFlameRadius && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                                p.AddBuff(ModContent.BuffType<DimensionalIncompatability>(), 2);
                        }
                    }
                }

                // Fire dark death rays the moment radius finishes shrinking
                if (currentFlameRadius <= flameRadiusMin)
                {
                    // Fire once on the first frame the radius hits min
                    if (darkDeathRayTimer == 180)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float playerAngle = (player.Center - NPC.Center).ToRotation();
                            float initialRotation = playerAngle - MathHelper.ToRadians(60);
                            int projIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DARK_DarkDeathRay>(), NPC.damage * 2, 10f, ai0: NPC.whoAmI, ai1: initialRotation);
                            Main.projectile[projIndex].owner = player.whoAmI;

                            initialRotation = playerAngle - MathHelper.ToRadians(180);
                            projIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DARK_DarkDeathRay>(), NPC.damage * 2, 10f, ai0: NPC.whoAmI, ai1: initialRotation);
                            Main.projectile[projIndex].owner = player.whoAmI;

                            initialRotation = playerAngle - MathHelper.ToRadians(300);
                            projIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DARK_DarkDeathRay>(), NPC.damage * 2, 10f, ai0: NPC.whoAmI, ai1: initialRotation);
                            Main.projectile[projIndex].owner = player.whoAmI;
                        }
                    }
                    else if (darkDeathRayTimer <= 0)
                    {
                        elementTransitioning = true;
                        NPC.netUpdate = true;
                        return;
                    }

                    darkDeathRayTimer--;
                }
            }
            elementIdxTimer++;
        }

        /* ------ LIFE ------ */

        private void Life_AI(Player player)
        {
            NPC.dontTakeDamage = true;
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Life_Movement(player);
            Life_Attack(player);
        }

        private void Life_Movement(Player player)
        {
            if (elementIdxTimer == 0)
            {
                Vector2 teleportPos = player.Center + new Vector2(0, -500);
                TeleportToPosition(teleportPos);
            }
            else
            {
                IdleMovement(NPC.Center);
            }
        }

        private void Life_Attack(Player player)
        {
            int maxMinions = Main.masterMode ? 10 : Main.expertMode ? 8 : 6;

            // Spawn minions on interval until we've spawned them all
            if (elementIdxTimer % 45 == 0 && lifeMinionSpawned < maxMinions)
            {
                SoundEngine.PlaySound(SoundID.Item44, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC minionNPC = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LifeMinion>());
                    if (minionNPC.whoAmI != Main.maxNPCs && minionNPC.whoAmI > 0)
                    {
                        lifeMinions.Add(minionNPC);
                        lifeMinionSpawned++;
                    }
                }
            }

            // Clean up dead minions from the list
            lifeMinions.RemoveAll(m => !m.active || m.type != ModContent.NPCType<LifeMinion>());

            bool anyMinionsAlive = lifeMinions.Count > 0;

            // Regen while minions are alive — grows exponentially, clamped to a max
            if (anyMinionsAlive)
            {
                Vector2 chargeDustPos = NPC.Center + new Vector2(80, 0).RotatedByRandom(MathHelper.ToRadians(360));
                Vector2 direction = (chargeDustPos - NPC.Center).SafeNormalize(Vector2.Zero) * -7.5f;
                int dust = Dust.NewDust(chargeDustPos, 1, 1, ModContent.DustType<SharpRadialGlowDust>(), direction.X, direction.Y, 100, new Color(255, 33, 244), .8f);
                Main.dust[dust].noGravity = true;
                lifeRegenTimer++;
                int maxRegen = Main.masterMode ? 80 : Main.expertMode ? 70 : 60;
                int regenAmount = (int)Math.Min(Math.Pow(1.02, lifeRegenTimer), maxRegen);
                NPC.life = Math.Min(NPC.life + regenAmount, NPC.lifeMax);
            }

            // All minions spawned and killed — move to next phase
            if (lifeMinionSpawned >= maxMinions && !anyMinionsAlive)
            {
                lifeMinionSpawned = 0;
                lifeRegenTimer = 0;
                elementTransitioning = true;
            }

            elementIdxTimer++;
        }

        /* ------ DEATH ------ */

        private void Death_AI(Player player)
        {
            if (previousElementIdx != elementIdx)
            {
                assignedGoToPosition = false;
                previousElementIdx = elementIdx;
            }
            Death_Movement(player);
            Death_Attack(player);
        }

        private void Death_Movement(Player player)
        {
            Main.GraveyardVisualIntensity = 1f; // fully apply graveyard visual filter during this phase
            if (elementIdxTimer == 0)
            {
                attackPhaseTimer++;
                NPC.velocity = Vector2.Zero;
                TeleportToPosition(player.Center);
                flameRadiusMax = 1200;
                currentFlameRadius = flameRadiusMax;
                flameRadiusMin= 700;
            }

            if (elementIdxTimer < 60)
            {
                Main.GraveyardVisualIntensity = (float)elementIdxTimer / 60f; // fade in graveyard visual filter over the first second
                IdleMovement(NPC.Center);

                // Shrink radius during fade-in (60 ticks)
                if (currentFlameRadius > flameRadiusMin)
                {
                    float radiusDecrement = (float)(flameRadiusMax - flameRadiusMin) / 60f;
                    currentFlameRadius -= (int)Math.Max(1, radiusDecrement);
                }

                // Spawn dusts around the boss in a circle
                int dustCount = 40;
                for (int i = 0; i < dustCount; i++)
                {
                    float angle = MathHelper.ToRadians(Main.rand.Next(360));
                    Vector2 dustPos = NPC.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * currentFlameRadius;

                    Dust dust = Dust.NewDustDirect(dustPos, 0, 0, DustID.WhiteTorch, 0, 0);
                    dust.noGravity = true;
                    dust.scale = 1.5f;
                }

                // Apply debuff to players outside the radius
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];
                    if (p.active && !p.dead)
                    {
                        float distanceToPlayer = Vector2.Distance(NPC.Center, p.Center);
                        if (distanceToPlayer > currentFlameRadius && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            p.AddBuff(ModContent.BuffType<DimensionalIncompatability>(), 2);
                        }
                    }
                }
            }
            else if (elementIdxTimer >= 60 && elementIdxTimer < 720)
            {
                attackPhaseTimer++;
                IdleMovement(NPC.Center);

                // Keep dust circle at fixed radius
                int dustCount = 40;
                for (int i = 0; i < dustCount; i++)
                {
                    float angle = MathHelper.ToRadians(Main.rand.Next(360));
                    Vector2 dustPos = NPC.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * currentFlameRadius;

                    Dust dust = Dust.NewDustDirect(dustPos, 0, 0, DustID.WhiteTorch, 0, 0);
                    dust.noGravity = true;
                    dust.scale = 1.5f;
                }

                // Apply debuff to players outside the radius
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];
                    if (p.active && !p.dead)
                    {
                        float distanceToPlayer = Vector2.Distance(NPC.Center, p.Center);
                        if (distanceToPlayer > currentFlameRadius && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            p.AddBuff(ModContent.BuffType<DimensionalIncompatability>(), 2);
                        }
                    }
                }
            }
            else if (elementIdxTimer >= 720 && elementIdxTimer < 900)
            {
                Main.GraveyardVisualIntensity = 1f - (float)(elementIdxTimer - 720) / 180f; // fade out graveyard visual filter over the last second
                IdleMovement(NPC.Center);
            }
            else
            {
                Main.GraveyardVisualIntensity = 0f;
                elementTransitioning = true;
                NPC.netUpdate = true;
                return;
            }

        }

        private void Death_Attack(Player player)
        {
            if (attackPhaseTimer % 10 == 0)
            {
                Vector2 spawnPos = NPC.Center + new Vector2(Main.rand.NextFloat(1200f, 1400f), 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                Vector2 direction = (NPC.Center - spawnPos).SafeNormalize(Vector2.Zero) * 5f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, direction, ModContent.ProjectileType<DEATH_SinisterSkull>(), NPC.damage, 0f);
                }
            }
            
            elementIdxTimer++;
        }

        private const int loopBackDuration = 300; // 3 seconds at 60fps
        private bool[] loopBackVelocityApplied;

        private void TemporalLoopBack_AI(Player player)
        {
            
            if (elementIdxTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.Center);
                SoundEngine.PlaySound(new SoundStyle("TheTesseractMod/Sounds/TickTockSlowing")
                {
                    Volume = 1.2f,
                }, player.Center);

                TeleportToPosition(player.Center + new Vector2(0, -300));
                loopBackVelocityApplied = new bool[Main.maxPlayers];
            }

            NPC.Center = player.Center + new Vector2(0, -300);

            if (elementIdxTimer < loopBackDuration)
                {
                    // Animate clock hands spinning backwards (rewinding time)
                    minuteHandRotation += 1.2f;
                    hourHandRotation += 0.05f;
                }

            float progress = Math.Clamp((float)elementIdxTimer / loopBackDuration, 0f, 1f);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (!p.active || p.dead)
                    continue;
                if (i >= playerPositions.Count || i >= playerHealthCounts.Count)
                    continue;

                if (elementIdxTimer < loopBackDuration)
                {
                    // Animate clock hands spinning backwards (rewinding time)
                    minuteHandRotation += 1.2f;
                    hourHandRotation += 0.1f;
                    Dust.NewDust(p.position, p.width, p.height, ModContent.DustType<SharpRadialGlowDust>(), 0f, 0f, 0, Color.Blue, 1f);

                    // Lerp position toward saved position
                    p.position = Vector2.Lerp(p.position, playerPositions[i], progress * 0.05f);
                    p.velocity = Vector2.Zero;
                    p.fallStart = (int)(p.position.Y / 16f); // prevent fall damage

                    // Make player immune to damage
                    p.immune = true;
                    p.immuneTime = 2;
                    p.noFallDmg = true;

                    // Lerp health toward saved health
                    int targetHealth = playerHealthCounts[i];
                    int healthDiff = targetHealth - p.statLife;
                    if (healthDiff != 0)
                    {
                        // Gradually change health over the duration
                        int healthChangePerTick = Math.Max(1, Math.Abs(healthDiff) / Math.Max(1, loopBackDuration - elementIdxTimer));
                        if (healthDiff < 0)
                        {
                            p.statLife = Math.Max(p.statLife - healthChangePerTick, targetHealth);
                        }
                            
                    }
                }
                else if (!loopBackVelocityApplied[i])
                {
                    // Snap to final position and apply saved velocity once
                    p.position = playerPositions[i];
                    if (i < playerVelocities.Count)
                        p.velocity = playerVelocities[i];
                    p.statLife = playerHealthCounts[i];
                    loopBackVelocityApplied[i] = true;
                    CombatText.NewText(p.Hitbox, new Color(3, 86, 252), "Let's do this again!", true);
                }
            }

            elementIdxTimer++;

            // End the phase a short time after rewind completes
            if (elementIdxTimer > loopBackDuration + 30)
            {
                MoveToNextElement();
                NPC.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 drawPos = new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f, NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f);

            if (elementIdx != (int)Element.Glow && elementIdx != (int)Element.Life && elementIdx != (int)Element.TemporalLoopBack)
            {
                // Load spikes texture and draw behind
                var spikesTexture = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardianSpikes_v4").Value;
                Vector2 spikesOrigin = new Vector2(spikesTexture.Width * 0.5f, spikesTexture.Height * 0.5f);
                spikesRotation += 0.08f; // Adjust rotation speed as needed


                Main.EntitySpriteDraw(spikesTexture,
                    drawPos,
                    null, NPC.GetAlpha(Color.White), spikesRotation, spikesOrigin, NPC.scale * 0.98f, effects, 0);
            }

            // Load and draw animated center sprite
            var centerTexture = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardianBase_CenterAnimated_v4").Value;

            // Calculate animation frame (12 fps = 5 ticks per frame at 60 fps)
            animationFrameCounter++;
            int frameHeight = centerTexture.Height / 4; // Assuming 4 frames vertical
            int currentFrame = (animationFrameCounter / 5) % 4;
            Rectangle frameRect = new Rectangle(0, currentFrame * frameHeight, centerTexture.Width, frameHeight);

            Vector2 centerOrigin = new Vector2(centerTexture.Width * 0.5f, frameHeight * 0.5f);

            

            if (elementIdx == (int)Element.Glow && dashing)
            {
                
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 oldPos = NPC.oldPos[k] - Main.screenPosition + new Vector2(NPC.width * 0.5f, NPC.height * 0.5f + NPC.gfxOffY);
                    Color color1 = NPC.GetAlpha(Color.White) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    Main.EntitySpriteDraw(centerTexture, oldPos, frameRect, color1, NPC.rotation, centerOrigin, NPC.scale, effects, 0);
                }
            }

            // Pulsing ghost layer behind center texture while life minions are alive
            if (lifeMinions.Count > 0)
            {
                float fade = (float)Math.Sin(Main.GameUpdateCount * MathHelper.TwoPi / 60f);
                fade = (fade + 1f) / 2f;
                float ghostScale = MathHelper.Lerp(1.1f, 1.3f, fade) * NPC.scale;

                Main.EntitySpriteDraw(centerTexture,
                    drawPos,
                    frameRect, NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, centerOrigin, ghostScale, effects, 0);
            }

            Color mainColor = NPC.GetAlpha(Color.White);

            if (elementIdx == (int)Element.TemporalLoopBack)
            {
               centerTexture = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardianBase_CenterWhite").Value;
               // Pulse between blue and white
               float pulse = (float)(Math.Sin(Main.GameUpdateCount * 0.1f) + 1f) / 2f; // 0 to 1
               mainColor = Color.Lerp(new Color(80, 130, 255), Color.White, pulse);
            }
            Main.EntitySpriteDraw(centerTexture,
            drawPos,
            frameRect, mainColor, NPC.rotation, centerOrigin, NPC.scale, effects, 0);

            

            // Draw Ice Shield if NPC is active and is actually an IceShield
            if (iceShield != null && iceShield.active && iceShield.type == ModContent.NPCType<IceShield>())
            {
                DrawIceShield(Main.screenPosition, drawColor);
            }


            if (!elementTransitioning && elementIdx != (int)Element.TemporalLoopBack)
            {
                // Draw element fragment on top
                DrawCenterElement(drawPos, effects);
            }
            else
            {
                // draw clock hands
                var minuteHandRequest = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardian_MinuteHand");
                var hourHandRequest = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Bosses/GuardianOfTheRift/TemporalGuardian_HourHand");

                if (minuteHandRequest.IsLoaded && hourHandRequest.IsLoaded)
                {
                    var minuteHandTexture = minuteHandRequest.Value;
                    var hourHandTexture = hourHandRequest.Value;

                    Main.EntitySpriteDraw(hourHandTexture,
                    drawPos,
                    null, NPC.GetAlpha(Color.White), MathHelper.ToRadians(hourHandRotation), new Vector2(hourHandTexture.Width * 0.5f, hourHandTexture.Height * 0.5f), NPC.scale, effects, 0);

                    Main.EntitySpriteDraw(minuteHandTexture,
                    drawPos,
                    null, NPC.GetAlpha(Color.White), MathHelper.ToRadians(minuteHandRotation), new Vector2(minuteHandTexture.Width * 0.5f, minuteHandTexture.Height * 0.5f), NPC.scale, effects, 0);
                }

            }

            return false;

        }

        private void DrawCenterElement(Vector2 drawPos, SpriteEffects effects)
        {
            string texturePath = elementIdx switch
            {
                0 => "TheTesseractMod/Items/Materials/HeatRiftFragment",
                1 => "TheTesseractMod/Items/Materials/DustRiftFragment",
                2 => "TheTesseractMod/Items/Materials/LightRiftFragment",
                3 => "TheTesseractMod/Items/Materials/ChloroRiftFragment",
                4 => "TheTesseractMod/Items/Materials/AquaRiftFragment",
                5 => "TheTesseractMod/Items/Materials/ColdRiftFragment",
                6 => "TheTesseractMod/Items/Materials/GlowRiftFragment",
                7 => "TheTesseractMod/Items/Materials/ElectricRiftFragment",
                8 => "TheTesseractMod/Items/Materials/DarkRiftFragment",
                9 => "TheTesseractMod/Items/Materials/LifeRiftFragment",
                10 => "TheTesseractMod/Items/Materials/DeathRiftFragment",
                _ => "TheTesseractMod/Items/Materials/HeatRiftFragment"
            };

            var fragmentTexture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 fragmentOrigin = new Vector2(fragmentTexture.Width * 0.5f, fragmentTexture.Height * 0.5f);

            // Calculate pulsing scale (oscillates between 0.8 and 1.2)
            float pulseScale = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.2f;

            Main.EntitySpriteDraw(fragmentTexture,
                drawPos,
                null, NPC.GetAlpha(Color.White), NPC.rotation, fragmentOrigin, NPC.scale * pulseScale, effects, 0);
        }

        private void TransitionToElement()
        // takes place over 135 frames
        {
            if (timeTransitionTimer == 0)
            {
                // Store current position as fixed position to transition around
                fixedGoToPosition = Main.player[NPC.target].Center + new Vector2(0, -300);
                TeleportToPosition(fixedGoToPosition);
                SoundEngine.PlaySound(new SoundStyle("TheTesseractMod/Sounds/TickTock3")
                {
                    Volume = 1.2f,
                }, NPC.Center);
            }
            else
            {
                IdleMovement(NPC.Center); // Stay in place but with idle movement while transitioning
            }
            if (timeTransitionTimer < 90)
            {
                minuteHandRotation += 4f;
                hourHandRotation += 1f/3f;
                timeTransitionTimer++;

                elementTransitioning = true;
                NPC.dontTakeDamage = true;
            }
            else if (timeTransitionTimer < 135)
            {
                timeTransitionTimer++;
            }
            else
            {
                elementTransitioning = false;
                NPC.dontTakeDamage = false;
                timeTransitionTimer = 0;
                // Main.NewText("Transitioning to element: " + (elementIdx + 1) % 11);
                MoveToNextElement();
            }
            NPC.netUpdate = true; // Sync transition state
        }
        private void IdleMovement(Vector2 targetPosition)
        {
            float idleOffsetAmplitude = 6f;
            float idleSpeed = 0.05f;
            Vector2 idleOffset = new Vector2((float)Math.Sin(Main.GameUpdateCount * idleSpeed) * idleOffsetAmplitude, (float)Math.Cos(Main.GameUpdateCount * idleSpeed * 1.2f) * idleOffsetAmplitude * 0.5f);

            Vector2 idleTarget = targetPosition + idleOffset;
            Vector2 toIdle = idleTarget - NPC.Center;

            NPC.velocity = toIdle * 0.1f;
        }

        private void TeleportToPosition(Vector2 position)
        {
            NPC.velocity = Vector2.Zero;
            NPC.Center = position;
            // Store position in ai array for network syncing
            NPC.ai[1] = position.X;
            NPC.ai[2] = position.Y;
            fixedGoToPosition = position;
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NPC.netUpdate = true;
                NPC.netSpam = 0;  // Reset spam counter to allow immediate send
            }

             // SoundEngine.PlaySound(SoundID.Item117, NPC.Center);
        }

        private void MoveToNextElement()
        {
            elementIdx = (elementIdx + 1) % 12;
            if (elementIdx == (int)Element.Heat)
            {
                NPC.defense = initialDefense - (int)dustIncreaseInDefense;
                NPC.damage = initialDamage - (int)dustIncreaseInDamage;
                dustIncreaseInDamage = 0;
                dustIncreaseInDefense = 0;
            }
            moveSpeed = 10f;
            assignedGoToPosition = false;

            // Reset position values
            NPC.ai[1] = 0;
            NPC.ai[2] = 0;

            attackIdx = 0;
            elementIdxTimer = 0;
            attackPhaseTimer = 0;
            flameRadiusPhaseTimer = 0;
            currentFlameRadius = flameRadiusMax;

            projShootSpeed = 2f;
            projDamage = 10;
            shotProjectiles = 0;
            // Main.NewText("Element Index:" + elementIdx);

            // Clean up dust boulders
            foreach (NPC boulder in dustBoulders)
            {
                if (boulder.active)
                {
                    boulder.life = 0;
                    boulder.netUpdate = true;
                }
            }
            dustBoulders.Clear();
            timeToKillBoulders = 1200; // Reset timer for next dust phase
            shotTornado = false;

            lightDeathRayChargeTime = 180;
            lightDeathRayTimer = 180;
            cloakBoss = false;
            spawnedTrueMinion = false;
            NPC.alpha = 0;

            // Kill and clear minions
            foreach (NPC minion in spawnedMinions)
            {
                if (minion.active)
                {
                    minion.life = 0;
                    minion.netUpdate = true;
                }
            }
            spawnedMinions.Clear();
            minionPlayerAssignment.Clear();
            minionPositionAssignment.Clear();
            trueMinion = null;
            minionPositionSwitchTimer = 0;

            dashing = false;

            // Clean up ice shield
            if (iceShield != null && iceShield.active)
            {
                iceShield.life = 0;
                iceShield.netUpdate = true;
            }
            iceShield = null;

            darkDeathRayTimer = 180;

            flameRadiusMin = 400;

            // last-stitch clean up for minions:
            
            foreach (NPC npc in Main.npc) 
            {
                if (!npc.active) continue;

                if (npc.type == ModContent.NPCType<DustBoulderMinion>() ||
                    npc.type == ModContent.NPCType<TrueMiniGuardian>() ||
                    npc.type == ModContent.NPCType<MiniGuardian>() ||
                    npc.type == ModContent.NPCType<IceShield>() ||
                    npc.type == ModContent.NPCType<LifeMinion>()) 
                    {
                        npc.active = false;
                    }
            }

            lifeMinionSpawned = 0;

            // Sync after cleanup
            NPC.netUpdate = true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.TemporalGuardianBaseGore1, 0.3f, 3f);
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.TemporalGuardianBaseGore2, 0.3f, 3f);
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.TemporalGuardianBaseGore3, 0.3f, 3f);
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.TemporalGuardianBaseGore4, 0.3f, 3f);
                CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.TemporalGuardianBaseGore5, 0.3f, 3f);

                for (int i = 0; i < 4; i++)
                {
                    CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.TemporalGuardianSpikeGore1, 1f, 3f);
                    CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.TemporalGuardianSpikeGore2, 1f, 3f);
                    CustomGoreManager.SpawnCustomGore(NPC, NPC.position, NPC.velocity, CustomGoreManager.TemporalGuardianSpikeGore2, 1f, 3f);
                }
            }
        }
    }

}


// Attack Descriptions:

// HEAT:
// Attack 1: Constantly shoots homing flames at the player. After a certain amount of time, it will switch to attack 2.
// Attack 2: The boss will pick a random location near the player and move there. Once it reaches the location, 
// it will start shrinking a large radius around itself. If the player is outside of this radius, they will get a debuff. 
// Once the radius is fully shrunk, the boss will shoot a barrage of inferno missiles towards the player. 
// After shooting a certain amount of missiles, it will switch to the next element.
//
// DUST:
// Spawns 4 Dust Boulders that increase damage, defense, and the number of projectiles that can spawn during the element phase.
// If the boulders are not broken in a certain time, the boss will have an increase in defense and damage for the remainder of the
// element attack cycle. The increase is dependent on how many boulders are still active. 
// Attack 1: Shoots dust clouds towards the player.
//
// LIGHT:
// Attack 1: Shoots a death ray that moves in a sweeping circle.
// Attack 2: Spawns illusion clones that shoot projectiles towards the player. If the player can find the real boss and do enough damaeg to it,
// the clones will disappear and move to the next phase. If all clones are killed it will also move to the next phase.
//
// CHLORO:
// Attack 1: Spawns a damaging aura around itself that harms the player. If the player takes damage inside this aura the boss gets healed.
// Attack 2: Shoots homing leaf projectiles at the player. After a certain amount of time, it will switch to the next element.
//
// AQUA:
// Attack 1: Shoots out two projectiles that spawn two tornadoes.
// Attack 2: goes below the player and shoots bubbles down that move upwards.
// 
// COLD:
// use DD2_WitherBeastCrystalImpact for ice impact sound
// forms an ice shield around that takes damage for the boss. After a certain time, the shield will break and send out ice crystals. the amount of
// crystals is dependent on how much health the shield had left when it broke. 
// Icicles will also rain down from the sky around all players.
// 
// GLOW:
// dashes towardsthe player leaving behind damaging spore clouds. After 5 dashes it switches elements
// 
// ELECTRIC:
// (lightning attacks)
// Shoots lightning bolts towards all players. Stands still but telelports before each attack.
// 
// DARK:
// attack 1: shoots another death ray
// 
// LIFE:
// Goes invincible and spawns minions. It will heal until all minions have been killed. 
// 
// DEATH:
// Bullet hell