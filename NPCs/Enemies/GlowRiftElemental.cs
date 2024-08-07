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
    internal class GlowRiftElemental :ModNPC
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
            NPC.CloneDefaults(NPCID.BoundGoblin);
            NPC.damage = 60;
            NPC.defense = 100;
            NPC.lifeMax = 1500;
            NPC.value = 20000;
            NPC.width = 32;
            NPC.height = 32;
            NPC.knockBackResist = 0.02f;
            NPC.DeathSound = SoundID.NPCDeath28;
            NPC.friendly = false;
            NPC.rarity = 0;

            AIType = NPCID.BoundGoblin;
            AnimationType = NPCID.Wraith;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (NPC.downedMoonlord.ToInt() * SpawnCondition.UndergroundMushroom.Chance * 4);
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.position, 0f, 0f, 1f);
            Player target = Main.player[NPC.target];
            float x = target.position.X - NPC.Center.X;
            float y = target.position.Y - NPC.Center.Y;
            float distance = (float)Math.Sqrt(x*x + y*y);

            if (timer > 270)
            {
                timer = 1;
            }

            if (distance < 800 && timer % 90 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Random rand = new Random();
                    float xOffset = (float)(rand.NextDouble() * 240 - 120);
                    float yOffset = (float)(rand.NextDouble() * 240 - 120);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(target.position.X + xOffset, target.position.Y + yOffset - 40), new Vector2(0f, 2f), ModContent.ProjectileType<GlowRiftProjectile>(), NPC.damage / 4, 1f);
                }
                SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
            }
            timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Enemies/GlowRiftElementalGLOW");
            Vector2 drawOrigin = new Vector2(glowTexture.Value.Width * 0.5f, NPC.height * 0.5f);
            Vector2 drawPos = NPC.position - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4.5f);
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            spriteBatch.Draw(glowTexture.Value,
                drawPos,
                NPC.frame,
                 new Color(255, 255, 255, 0) * (1f - NPC.alpha / 255f), NPC.rotation, drawOrigin, 1.1f, effects, 0f);
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundMushroom,

				new FlavorTextBestiaryInfoElement("You will know it's nearby if mushroom spores start raining down on top of you!"),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GlowRiftFragment>(), 2, 1, 3));
        }
        public override void OnKill()
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(NPC.position, 26, 26, DustID.GlowingMushroom);
            }
        }
    }
}
