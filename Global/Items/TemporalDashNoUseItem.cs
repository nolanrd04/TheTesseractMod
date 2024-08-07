using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;

namespace TheTesseractMod.Global.Items
{
    internal class TemporalDashNoUseItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            return !player.HasBuff(ModContent.BuffType<TemporalDashBuff>());
        }
    }
}