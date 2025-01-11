using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.HallowedWeapons
{
    internal class GoldenKnightMagic : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);

            if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, float.MaxValue))
            {
                Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .25f);
            }
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 0, Color.OrangeRed, .7f);
        }
    }
}
