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
using TheTesseractMod.Projectiles.Magic;
using TheTesseractMod.Projectiles.Enemy;
using Terraria.Audio;
using TheTesseractMod.Items.Materials;

namespace TheTesseractMod.NPCs.Enemies
{
    internal class LightRiftSlime :ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GoldenSlime);
            NPC.damage = 115;
            NPC.defense = 100;
            NPC.lifeMax = 1000;
            NPC.value = 10000;
            NPC.knockBackResist = 0.5f;
            NPC.velocity = new Vector2(10f, 10f);
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.rarity = 0;

            AIType = NPCID.Crimslime;
            AnimationType = NPCID.Crimslime;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (SpawnCondition.OverworldDay.Chance * NPC.downedMoonlord.ToInt()) * 0.17f;
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            float x = target.position.X - NPC.Center.X;
            float y = target.position.Y - NPC.Center.Y;
            float distance = (float)Math.Sqrt(x * x + y * y);
            Random rand = new Random();
            Lighting.AddLight(NPC.position, new Vector3(1f, 1f, 1f));
            if (timer % 5 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 204, NPC.velocity.X, NPC.velocity.Y, 150, default(Color), 1.5f);
            }
            if (timer % 240 == 0 && distance > 300)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float rotation = (float)(rand.NextDouble() * (2 * Math.PI));
                    Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, new Vector2(4f, 4f).RotatedBy(rotation), ModContent.ProjectileType<LightRiftProjectile>(), 50, 10);
                    rotation = (float)(rand.NextDouble() * (2 * Math.PI));
                    Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, new Vector2(4f, 4f).RotatedBy(rotation), ModContent.ProjectileType<LightRiftProjectile>(), 50, 10);
                    rotation = (float)(rand.NextDouble() * (2 * Math.PI));
                    Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, new Vector2(4f, 4f).RotatedBy(rotation), ModContent.ProjectileType<LightRiftProjectile>(), 50, 10);
                }
                SoundEngine.PlaySound(SoundID.Item165, NPC.position);
            }
            timer++;
            if (timer == 241)
            {
                timer = 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = Color.LightYellow;

            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect");
            spriteBatch.Draw(glowTexture.Value,
                new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f, NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                color, 0, glowTexture.Size() * 0.5f, 1.5f, SpriteEffects.None, 0f);

            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The light at the end of the tunnel was actually lookng to kill you."),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LightRiftFragment>(), 3, 1, 3));
        }
    }
}
