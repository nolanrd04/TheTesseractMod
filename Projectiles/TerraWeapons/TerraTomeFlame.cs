using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraTomeFlame : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Projectiles/TerraWeapons/TerraTorchProjImpact";

        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = 35;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Vector2 newPos = Projectile.position + new Vector2(Projectile.width / 2, Projectile.height / 2);
            if (Projectile.ai[0] < 25)
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(newPos, 20, 20, ModContent.DustType<RadialGlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.Lime, 1f);
                }
            }
            Projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 240);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.2f;
            return true;
        }
    }
}
