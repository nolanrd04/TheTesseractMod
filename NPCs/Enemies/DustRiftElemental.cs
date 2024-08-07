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
    internal class DustRiftElemental :ModNPC
    {
        private int dustTimer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 17;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueCursedFlames] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.WeaponImbueFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.SolarSolenian);
            NPC.damage = 100;
            NPC.defense = 100;
            NPC.lifeMax = 3700;
            NPC.value = 0;
            NPC.width = 74;
            NPC.height = 74;
            NPC.knockBackResist = 0.01f;
            NPC.rarity = 0;
            NPC.alpha = 0;
            NPC.value = 20000;

            AIType = NPCID.SolarSolenian;
            AnimationType = NPCID.SolarSolenian;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int chance = 0;
            if (spawnInfo.Player.ZoneDesert || spawnInfo.Player.ZoneUndergroundDesert)
            {
                chance = 1;
            }
            return (NPC.downedMoonlord.ToInt() * chance * 0.2f);
        }
        public override void AI()
        {
            Lighting.AddLight(NPC.position, new Vector3(255 / 255f, 201 / 255f, 99 / 255f));
            if (dustTimer % 12 == 0)
            {
                Dust.NewDust(NPC.Center, 20, 20, DustID.Sand);
            }
            dustTimer++;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int offset = 0;
            if (NPC.spriteDirection == 1)
            {
                offset = 10;
            }
            else
            {
                offset = -10;
            }
            Asset<Texture2D> eyeTexture = ModContent.Request<Texture2D>("TheTesseractMod/NPCs/Enemies/DustRiftElementalEyes");
            spriteBatch.Draw(eyeTexture.Value,
                new Vector2((NPC.position.X - Main.screenPosition.X + NPC.width * 0.5f) + offset, (NPC.position.Y - Main.screenPosition.Y + NPC.height * 0.5f) + 5),
                new Rectangle(0, 0, eyeTexture.Value.Width, eyeTexture.Value.Height),
                new Color(255, 255, 255, 0) * (1f - NPC.alpha / 255f), 0, eyeTexture.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DustRiftFragment>(), 2, 1, 3));
        }

        public override void OnKill()
        {
            for (int i = 0; i < 13; i++)
            {
                Dust.NewDust(NPC.Center, 74, 74, DustID.Sand);
            }
        }
    }
}
