using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsSlingProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 14f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 280f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 14f;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                target.AddBuff(BuffID.ShadowFlame, 120);
            }
        }
    }
}
