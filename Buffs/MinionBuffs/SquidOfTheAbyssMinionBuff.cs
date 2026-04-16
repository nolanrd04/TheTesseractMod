using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Summoner;

namespace TheTesseractMod.Buffs.MinionBuffs
{
    internal class SquidOfTheAbyssMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SquidOfTheAbyssMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}