using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace TheTesseractMod.Projectiles.Melee.ZenithYoYoChildProjectiles
{
    internal class ZenithYoYoStinger : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Stinger);
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
        }
    }
}
