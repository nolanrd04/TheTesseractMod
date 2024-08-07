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
    internal class AquaRiftElemental :ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueCursedFlames] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbuePoison] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueVenom] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Arapaima);
            NPC.damage = 60;
            NPC.defense = 100;
            NPC.lifeMax = 2600;
            NPC.value = 20000;
            NPC.width = 118;
            NPC.height = 44;
            NPC.knockBackResist = 0.02f;
            NPC.velocity = new Vector2(10f, 10f);
            NPC.DeathSound = SoundID.NPCDeath28;
            NPC.rarity = 0;

            AIType = NPCID.Arapaima;
            AnimationType = NPCID.Wraith;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int chance = 0;
            if (spawnInfo.Player.ZoneBeach == true)
            {
                chance = 1;
            }
            return (NPC.downedMoonlord.ToInt() * SpawnCondition.OverworldDay.Chance * SpawnCondition.WaterCritter.Chance * chance);
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            float x = target.position.X - NPC.Center.X;
            float y = target.position.Y - NPC.Center.Y;
            float distance = (float)Math.Sqrt(x*x + y*y);
            if (timer > 240)
            {
                timer = 0;
            }
            if (timer % 6 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 34);
            }

            if (distance < 400 && timer % 15 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Random rand = new Random();
                    float rotation = (float)(rand.NextDouble() * 10 - 5);
                    Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (direction * 8f).RotatedBy(MathHelper.ToRadians(rotation)), ModContent.ProjectileType<DeadlyBubbleHostile>(), NPC.damage / 6, 1f);
                }
                    
                SoundEngine.PlaySound(SoundID.Item54, NPC.Center);
            }
            timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Lighting.AddLight(NPC.position, new Vector3(0f, 0f, 1f));
            Color color = new Color(0, 0, 255);

            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect");
            spriteBatch.Draw(glowTexture.Value,
                new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f, NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                color, 0, glowTexture.Size() * 0.5f, 1.5f, SpriteEffects.None, 0f);

            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, NPC.height * 0.5f);
            Vector2 drawPos = NPC.position - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);

            Main.EntitySpriteDraw(texture, drawPos, NPC.frame, Color.White, NPC.rotation, drawOrigin, 1.04f, effects, 0);

            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean, BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

				new FlavorTextBestiaryInfoElement("Death from the deep."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AquaRiftFragment>(), 2, 1, 3));
        }

        
    }
}
