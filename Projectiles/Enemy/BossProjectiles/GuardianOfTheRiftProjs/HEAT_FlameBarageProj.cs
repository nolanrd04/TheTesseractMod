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
using Terraria.Graphics;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    internal class HEAT_FlameBarageProj : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 105;
            Projectile.alpha = 255;
        }

        public override void AI()
        {

            // FinalFractalHelper.FinalFractalProfile finalFractalProfile = FinalFractalHelper.GetFinalFractalProfile((int)ai[1]);
            Vector2 newPos = Projectile.position + new Vector2(Projectile.width / 2, Projectile.height / 2);
            if (Projectile.ai[0] < 95)
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(newPos, 2, 2, ModContent.DustType<RadialGlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.OrangeRed, 1f);
                }
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(newPos, 2, 2, ModContent.DustType<RadialGlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.Red, 1f);
                }

                if (Main.rand.Next(3) == 0)
                {
                    int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.RedTorch, 0, 0, 0, default(Color), 3f);
                    Main.dust[dust].noGravity = true;
                }
            }
            Projectile.ai[0]++;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
