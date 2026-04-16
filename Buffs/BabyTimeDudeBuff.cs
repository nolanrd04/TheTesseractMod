using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Pets;

namespace TheTesseractMod.Buffs
{
    public class BabyTimeDudeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<BabyTimeDude>());
        }
    }
}