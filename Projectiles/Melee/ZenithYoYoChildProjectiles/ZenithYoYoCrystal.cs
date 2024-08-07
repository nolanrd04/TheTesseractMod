using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Melee.ZenithYoYoChildProjectiles
{
    internal class ZenithYoYoCrystal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.CrystalStorm);
            Projectile.DamageType = DamageClass.Melee;
        }
    }
}
