using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using TheTesseractMod.Dusts;
using TheTesseractMod.GlobalFuncitons;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace TheTesseractMod.Projectiles.TrueExcaliburWeapons
{
    internal class TrueGoldenMageMagic : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        private Color color;
        private NPC lastHit;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            bool colorPicker = Main.rand.NextBool();
            if (colorPicker) // blue
            {
                color = Color.Blue;
            }
            else
            {
                color = Color.DeepPink;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            lastHit = target;
        }

        public override void AI()
        {
            NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center, lastHit);

            if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, float.MaxValue))
            {
                Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .25f);
            }
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 0, color, .7f);
        }
    }
}
