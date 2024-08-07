using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs
{
    internal class TemporalDashBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed *= 4;
            player.runAcceleration *= 1.1f;
            player.wingTime += 0.6f;
            player.lifeRegen += 10;
            if (player.buffTime[buffIndex] % 2 == 0)
            {
                Dust.NewDust(player.position, player.width, player.height, 45, 0, 0, 150, default(Color), 1.5f);
                Dust.NewDust(player.position, player.width, player.height, 15, 0, 0, 150, default(Color), 1.5f);
            }

            if (player.buffTime[buffIndex] <=0)
            {
                player.AddBuff(ModContent.BuffType<TemporalDashCooldownDebuff>(), 3600);
            }
        }
    }
}
