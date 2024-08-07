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
    internal class HeatRiftElemental :ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueCursedFlames] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbuePoison] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueVenom] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Herpling);
            NPC.damage = 150;
            NPC.defense = 160;
            NPC.lifeMax = 2800;
            NPC.value = 20000;
            NPC.knockBackResist = 0.1f;
            NPC.HitSound = SoundID.NPCHit20;
            NPC.DeathSound = SoundID.NPCDeath23;
            NPC.width = 30;
            NPC.height = 52;
            NPC.lavaImmune = true;
            NPC.rarity = 0;
            AIType = NPCID.Herpling;
            AnimationType = NPCID.Zombie;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int chance = 0;
            if (spawnInfo.Player.ZoneUnderworldHeight == true)
            {
                chance = 1;
            }
            return (NPC.downedMoonlord.ToInt() * chance);
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.position, 255 / 255f, 117 / 255f, 31 / 255f);
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, NPC.velocity.X, NPC.velocity.Y, 0, default(Color), 2f);
                //Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RiftLightBlueDust>());
            }
            if (distance < 600 && distance > 200)
            {
                bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
                if (timer % 20 == 0 && lineOfSight)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 15f, ModContent.ProjectileType<DeadlyFlame>(), NPC.damage / 10, 2f);
                    }
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
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
                Color color1 = Color.Orange * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,

				new FlavorTextBestiaryInfoElement("The magma has awoken and is way faster than it should be..."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HeatRiftFragment>(), 2, 1, 3));
        }

        public override void OnKill()
        {
            WorldGen.PlaceLiquid(NPC.position.ToTileCoordinates().X, NPC.position.ToTileCoordinates().Y, (byte)LiquidID.Lava, 16);
            for (int i = 0; i < 16; i++)
            {
                Dust.NewDust(NPC.position, 30, 30, DustID.Torch);
            }
        }
    }
}
