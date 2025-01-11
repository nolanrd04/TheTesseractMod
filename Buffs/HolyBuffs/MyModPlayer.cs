using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs.HolyBuffs
{
    public class MyModPlayer : ModPlayer
    {
        public int critBonus = 0;
        public float damageBonus = 0f;

        public override void ResetEffects()
        {
            critBonus = 0;
            damageBonus = 0f;
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            crit += critBonus;
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            damage += damageBonus;
        }
    }
}
