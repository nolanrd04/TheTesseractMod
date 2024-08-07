using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.Ranged;
using TheTesseractMod.Items.Weapons.Ranged;
using Terraria.Audio;
using System.ComponentModel.DataAnnotations;

namespace TheTesseractMod.Global.Projectiles.Magic
{
    public class ConjuringClimaxCalamityOverrider : GlobalProjectile
    {
        //variables used to keep track of projectile hits
        public static bool shotByConjuringClimax;
        public override void SetDefaults(Projectile entity)
        {
            if (entity.identity == ProjectileID.ShadowBeamFriendly && shotByConjuringClimax == true)
            {
                entity.penetrate = 25;
            }
            if (entity.identity == ProjectileID.InfernoFriendlyBlast && shotByConjuringClimax == true)
            {
                entity.usesLocalNPCImmunity = true;
                entity.localNPCHitCooldown = 10;
            }
        }
    }
}
