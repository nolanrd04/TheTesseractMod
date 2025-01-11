using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Magic.EtherealStaffProjectile;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.JungleWeapons
{
    internal class CursedStaffProjChild : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 120);
        }
    }
}
