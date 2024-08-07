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
using Mono.Cecil;
using System.Security.Cryptography.X509Certificates;

namespace TheTesseractMod.Global.Projectiles.Ranged
{
    public class CulminationProjectileEdit : GlobalProjectile
    {
        public static bool ingoreTiles = false;
        // this class was meant to try to remove tilecollide from falling star projectiles. Currently does not work.
        public override void SetDefaults(Projectile entity)
        {
            if ((entity.type == ProjectileID.StarWrath || entity.type == ModContent.ProjectileType<CulminationArrow>()) && ingoreTiles)
            {
                entity.tileCollide = false;
            }
        }

    }
}
