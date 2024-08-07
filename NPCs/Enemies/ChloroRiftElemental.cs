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
using TheTesseractMod.Projectiles.Enemy.DeathRiftProjectile;
using Terraria.Audio;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Dusts;
using Terraria.GameContent;

namespace TheTesseractMod.NPCs.Enemies
{
    internal class ChloroRiftElemental :ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueCursedFlames] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbuePoison] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueVenom] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GiantTortoise);
            NPC.damage = 200;
            NPC.defense = 150;
            NPC.lifeMax = 3000;
            NPC.value = 20000;
            NPC.knockBackResist = 0f;
            NPC.width = 104;
            NPC.height = 46;
            NPC.lavaImmune = true;
            NPC.rarity = 0;
            NPC.boss = false;

            AIType = NPCID.GiantTortoise;
            AnimationType = NPCID.GiantTortoise;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int chance = 0;
            if (spawnInfo.Player.ZoneJungle)
            {
                chance = 1;
            }
            return (NPC.downedMoonlord.ToInt() * chance * SpawnCondition.HardmodeJungle.Chance * 0.5f);
        }

        public override void AI()
        {
            Dust.NewDust(new Vector2((NPC.position.X + 10), NPC.position.Y), 90, 30, DustID.CursedTorch, NPC.velocity.X, NPC.velocity.Y, 0, default(Color), 2f);
            /*Player target = Main.player[NPC.target];
            float x = target.position.X - NPC.Center.X;
            float y = target.position.Y - NPC.Center.Y;
            float distance = (float)Math.Sqrt(x * x + y * y);

            if (distance < 140)
            {
                if (timer > 30)
                {
                    int xOffset = 0;
                    int yOffset = -40;
                    
                    if (NPC.spriteDirection == 1)
                    {
                        xOffset = -65;
                    }
                    else
                    {
                        xOffset = 65;
                    }

                    Vector2 direction = (NPC.Center + new Vector2(xOffset, yOffset) - NPC.Center).SafeNormalize(Vector2.UnitX);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(xOffset, yOffset), direction * -30f, ModContent.ProjectileType<DeathRiftSlash>(), 0, 6f);
                    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                    timer = 1;
                }
            }
            timer++;*/
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, NPC.height * 0.5f);

            //draw main
            Main.EntitySpriteDraw(texture,
                new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f, NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f),
                NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, drawOrigin, NPC.scale, effects, 0);

            return false;

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,

                new FlavorTextBestiaryInfoElement("Nuclear experiments seemed to have an effect on the Giant Tortoises."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChloroRiftFragment>(), 2, 1, 3));
        }

    }
}
