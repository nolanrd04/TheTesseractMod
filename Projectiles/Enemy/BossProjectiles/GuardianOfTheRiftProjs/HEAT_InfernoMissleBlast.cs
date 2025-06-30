using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    internal class HEAT_InfernoMissleBlast : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.InfernoFriendlyBlast);
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 240;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
