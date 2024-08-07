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
    internal class LifeRiftElemental :ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GiantBat);
            NPC.damage = 60;
            NPC.defense = 100;
            NPC.lifeMax = 3000;
            NPC.value = 20000;
            NPC.width = 76;
            NPC.height = 68;
            NPC.knockBackResist = 0.02f;
            NPC.HitSound = SoundID.NPCHit44;
            NPC.DeathSound = SoundID.NPCDeath46;
            NPC.rarity = 0;
            NPC.alpha = 0;
            NPC.noTileCollide = true;


            AIType = NPCID.GiantBat;
            AnimationType = NPCID.GiantBat;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int chance = 0;
            if (spawnInfo.Player.ZoneHallow == true || spawnInfo.Player.ZoneShimmer == true)
            {
                chance = 1;
            }
            return (NPC.downedMoonlord.ToInt() * chance * 0.1f);
        }

        public override void AI()
        {
            if (timer > 330)
            {
                timer = 1;
            }
            Lighting.AddLight(NPC.position, new Vector3(251 / 255f, 251 / 255f, 251 / 255f));
            Player target = Main.player[NPC.target];
            float x = target.position.X - NPC.Center.X;
            float y = target.position.Y - NPC.Center.Y;
            float distance = (float)Math.Sqrt(x*x + y*y);
            if (timer >= 300 && distance < 700)
            {
                if ((timer - 300) % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<LifeRiftElementalLarva>());
                }
            }
            timer++;
            
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Enemies/LifeRiftElementalGLOW");
            Vector2 drawOrigin = new Vector2(glowTexture.Value.Width * 0.5f, NPC.height * 0.5f);
            Vector2 drawPos = NPC.position - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY+4);

            spriteBatch.Draw(glowTexture.Value,
                drawPos,
                NPC.frame,
                 new Color(255, 255, 255, 0) * (1f - NPC.alpha / 255f), NPC.rotation, drawOrigin, 1.1f, effects, 0f);

            return true;
        }


        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,

                new FlavorTextBestiaryInfoElement("The elements of life have conceived something that's so... unbelievably weird looking."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifeRiftFragment>(), 2, 1, 3));
        }
        public override void OnKill()
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(NPC.position, 60, 60, DustID.PinkTorch);
            }
        }
    }
}
