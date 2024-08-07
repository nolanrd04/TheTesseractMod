using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Ranged;

namespace TheTesseractMod.Global.Projectiles.Summon
{
    internal class ZenithMinionSummonGlobalOverride :GlobalProjectile
    {
        public static bool shotByZenithMinion = false;
        public override void SetDefaults(Projectile entity)
        {
            if (shotByZenithMinion)
            {
                // makes it so the summons spawned from the zenith minion will eventually despawn.
                entity.timeLeft = 500;
                entity.minionSlots = 0;
            }
        }
    }
}
