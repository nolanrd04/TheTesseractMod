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
    internal class LifeRiftElementalLarva :ModNPC
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.CaveBat);
            NPC.damage = 60;
            NPC.defense = 100;
            NPC.lifeMax = 500;
            NPC.value = 0;
            NPC.width = 8;
            NPC.height = 8;
            NPC.knockBackResist = 0.02f;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.rarity = 0;
            NPC.alpha = 0;
            NPC.noTileCollide = true;


            AIType = NPCID.CaveBat;
            AnimationType = NPCID.CaveBat;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.position, new Vector3(251 / 255f, 251 / 255f, 251 / 255f));
        }

        public override void OnKill()
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(NPC.position, 4, 4, DustID.PinkTorch);
            }
        }
    }
}
