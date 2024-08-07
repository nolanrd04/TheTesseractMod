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
    internal class ColdRiftElemental :ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueCursedFlames] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbuePoison] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueVenom] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Wraith);
            NPC.damage = 115;
            NPC.defense = 100;
            NPC.lifeMax = 2700;
            NPC.value = 20000;
            NPC.knockBackResist = 0.1f;
            NPC.velocity = new Vector2(10f, 10f);
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.rarity = 0;

            AIType = NPCID.Wraith;
            AnimationType = NPCID.Wraith;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int chance = 0;
            if (spawnInfo.Player.ZoneSnow == true)
            {
                chance = 1;
            }
            return (SpawnCondition.OverworldNightMonster.Chance * NPC.downedMoonlord.ToInt() * chance * 0.5f);
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.position, new Vector3(0f, 1f, 1f));
            NPC.alpha = 60;
            Player target = Main.player[NPC.target];
            float x = target.position.X - NPC.Center.X;
            float y = target.position.Y - NPC.Center.Y;
            float distance = (float)Math.Sqrt(x * x + y * y);

            if (timer > 240)
            {
                timer = 0;
            }
            if (timer % 6 == 0)
            {
                //Dust.NewDust(NPC.position, NPC.width, NPC.height, 67);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RiftLightBlueDust>());
            }
            if (timer > 210 && distance < 600)
            {
                if (timer % 10 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 5f, ModContent.ProjectileType<DeadlyIcicleHostile>(), NPC.damage / 5, 2f);
                    }
                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                }
            }
            timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = new Color(0, 255, 255);

            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect");
            spriteBatch.Draw(glowTexture.Value,
                new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f, NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                 new Color(255, 255, 255, 0) * (1f - NPC.alpha / 255f), 0, glowTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

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
                Color color1 = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, 0, NPC.width, NPC.height), color1, NPC.rotation, drawOrigin, NPC.scale, effects, 0);
            }
                return true;

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow, BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				new FlavorTextBestiaryInfoElement("The coldness seaks you out."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ColdRiftFragment>(), 2, 1, 3));
        }
        public override void OnKill()
        {
            for (int i = 0; i < 17; i++)
            {
                Dust.NewDust(NPC.Center, 40, 80, ModContent.DustType<RiftLightBlueDust>());
            }
        }
    }
}
