using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Systems;
using TheTesseractMod.Global.Items;
using TheTesseractMod.Players;

namespace TheTesseractMod.Buffs
{
    internal class StardustRelocatorBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] <= 0)
            {
                player.AddBuff(BuffID.ChaosState, 600);
                KeyBindPlayer.teleportCount = 0;
            }
        }
    }
}
