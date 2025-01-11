using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion.Level1Attacks
{
    internal class TerraMagicPositive : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        private float incrementalAngle = (float)Math.PI / 2;
        private float direction;
        private float scalingFactor;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 1800;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Dust.NewDustPerfect(new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f), ModContent.DustType<RadialGlowDust>(), Vector2.Zero, 120, Color.Aqua, 0.65f);
            Dust.NewDustPerfect(new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f), ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 0, Color.Lime, 0.5f);
            

            if (Projectile.ai[0] == 0)
            {
                direction = Projectile.velocity.ToRotation();
            }
            incrementalAngle += 0.1f;

            Projectile.position.Y += (float)(Math.Sin(incrementalAngle) * Math.Cos(-direction) * 1.5f);
            Projectile.position.X += (float)(Math.Sin(incrementalAngle) * Math.Sin(-direction) * 1.5f);
            Projectile.ai[0]++;
        }
    }
}
