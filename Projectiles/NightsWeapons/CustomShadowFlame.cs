using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TheTesseractMod.GlobalFuncitons;
using Terraria.ID;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class CustomShadowFlame : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        float speed;
        NPC target;

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = false;
            Projectile.timeLeft = 100;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1.8f;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
            target = GlobalProjectileFunctions.findSecondClosestTarget(Projectile.Center);
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 4)
            {
                Projectile.friendly = true;
            }
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.ShadowbeamStaff, 0, 0, 50, default(Color), Projectile.scale);
                int dust2 = Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, 0, 0, 50, default(Color), Projectile.scale);
                Main.dust[dust].noGravity = true;
                Main.dust[dust2].noGravity = true;
            }

            if (GlobalProjectileFunctions.IsTargetValid(target, Projectile.Center, 1000f))
            {
                Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .02f);
            }

            Projectile.scale *= .985f;
            Projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                target.AddBuff(BuffID.ShadowFlame, Main.rand.Next(60) + 60);
            }
        }
    }
}
