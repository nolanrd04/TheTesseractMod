using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;

namespace TheTesseractMod.Projectiles.Ranged
{
    internal class PetasharkBubble : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/VanillaBubble";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bubble);
            Projectile.DamageType = DamageClass.Ranged;
        }
    }
}
