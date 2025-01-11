using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.Summoner
{
    internal class LightDustStorm : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 140;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RadialGlowDust>(), 0, 0, 0, Color.Aqua, 1f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RadialGlowDust>(), 0, 0, 0, new Color(255 / 255f, 246 / 255f, 150 / 255f), 1f);
            }
        }
    }
}
