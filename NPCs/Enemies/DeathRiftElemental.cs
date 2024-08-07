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
    internal class DeathRiftElemental :ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 16;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbuePoison] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueVenom] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.BoneLee);
            NPC.damage = 150;
            NPC.defense = 100;
            NPC.lifeMax = 2800;
            NPC.value = 20000;
            NPC.knockBackResist = 0.04f;
            NPC.width = 44;
            NPC.height = 48;
            NPC.lavaImmune = true;
            NPC.rarity = 0;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;

            AIType = NPCID.BoneLee;
            AnimationType = NPCID.BoneLee;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int chance = 0;
            if (spawnInfo.Player.ZoneGraveyard == true || spawnInfo.Player.ZoneCorrupt == true || spawnInfo.Player.ZoneCrimson == true)
            {
                chance = 1;
            }
            return (NPC.downedMoonlord.ToInt() * chance * 0.2f);
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
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
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 direction = (NPC.Center + new Vector2(xOffset, yOffset) - NPC.Center).SafeNormalize(Vector2.UnitX);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(xOffset, yOffset), direction * -30f, ModContent.ProjectileType<DeathRiftSlash>(), 0, 6f);
                    }
                    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                    timer = 1;
                }
            }
            timer++;
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
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = (NPC.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, NPC.gfxOffY);
                Color color1 = Color.White * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, NPC.frame, color1, NPC.rotation, drawOrigin, NPC.scale, effects, 0);
            }

            //draw main
            Main.EntitySpriteDraw(texture,
                new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f, NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f),
                NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, drawOrigin, NPC.scale, effects, 0);

            return false;

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption, BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson, BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Graveyard, BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement("I AM DEATH. STRAIGHT. UP."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeathRiftFragment>(), 2, 1, 3));
        }

    }
}
