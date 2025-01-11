using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Ranged
{
    internal class PrototypeProjectile : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1500;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 70;
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RadialGlowDust>(), Vector2.Zero, 0, Color.Purple, .5f);
        }
    }
}
