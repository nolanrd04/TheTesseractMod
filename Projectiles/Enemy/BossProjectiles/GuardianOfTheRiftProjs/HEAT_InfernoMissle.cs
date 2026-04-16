using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    internal class HEAT_InfernoMissle : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = Main.rand.Next(24, 36);
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 174, 0, 0, 100, default(Color), 1.4f);
                Main.dust[dust].noGravity = true;
                Dust dust2 = Main.dust[dust];
                dust2.velocity *= 0.5f;
                dust2 = Main.dust[dust];
                dust2.velocity += dust2.velocity * 0.1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HEAT_InfernoMissleBlast>(), Projectile.damage, 10f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
