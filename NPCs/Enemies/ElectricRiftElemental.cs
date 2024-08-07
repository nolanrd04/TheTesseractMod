using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TheTesseractMod.Projectiles.Enemy;
using Terraria.Audio;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Dusts;
using Terraria.GameContent;

namespace TheTesseractMod.NPCs.Enemies
{
    internal class ElectricRiftElemental : ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Cursed] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.CaveBat);
            NPC.damage = 60;
            NPC.defense = 100;
            NPC.lifeMax = 1500;
            NPC.value = 20000;
            NPC.width = 40;
            NPC.height = 40;
            NPC.knockBackResist = 0.01f;
            NPC.velocity = new Vector2(10f, 10f);
            NPC.HitSound = SoundID.NPCHit53;
            NPC.DeathSound = SoundID.NPCDeath56;
            NPC.rarity = 0;

            AIType = NPCID.CaveBat;
            AnimationType = NPCID.CaveBat;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int chance = 0;
            int position = spawnInfo.Player.position.ToTileCoordinates().Y;
            if (position < Main.worldSurface * 0.35f)
            {
                chance = 1;
            }

            return (NPC.downedMoonlord.ToInt() * chance * 0.5f);
        }

        public override void AI()
        {
            NPC.alpha = 0;
            Player target = Main.player[NPC.target];
            float x = target.position.X - NPC.Center.X;
            float y = target.position.Y - NPC.Center.Y;
            float distance = (float)Math.Sqrt(x*x + y*y);
            if (timer >= 240)
            {
                timer = 0;
            }

            if (distance < 500 && timer % 120 == 0)
            {
                Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
                if (lineOfSight)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 15f, ModContent.ProjectileType<DeadlyThunderbolt>(), NPC.damage / 4, 1f);
                    }
                    SoundEngine.PlaySound(SoundID.Thunder, NPC.Center);
                }
            }
            timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            if (timer % 6 == 0)
            {
                //Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<BlueElectricDust>(), NPC.velocity.X, NPC.velocity.Y, 0, default(Color), 0.4f);
                Dust.NewDust(NPC.position, NPC.width-20, NPC.height-20, ModContent.DustType<ElectricDust>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1f);
            }
            Lighting.AddLight(NPC.position, new Vector3(1f, 1f, 1f));

            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky, BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
				new FlavorTextBestiaryInfoElement("Death from above."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ElectricRiftFragment>(), 2, 1, 3));
        }
    }
}
