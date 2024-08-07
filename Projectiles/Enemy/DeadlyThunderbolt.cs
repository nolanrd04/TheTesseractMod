using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.Enemy
{
    internal class DeadlyThunderbolt:ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.damage = 100;
            Projectile.alpha = 0;
            Projectile.timeLeft = 180;
            Projectile.light = 0.9f;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 10 == 0)
            {
                Random rand = new Random();
                float rotation = (float)(rand.NextDouble() * 100 - 50);
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
            }
            Dust.NewDust(Projectile.position, Projectile.width-15, Projectile.height-15, ModContent.DustType<BlueElectricDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1f);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width-15, Projectile.height-15, ModContent.DustType<ElectricDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1f);
            }
            

            if (Projectile.ai[0] == 20)
            {
                Random rand = new Random();
                float rotation = (float)(rand.NextDouble() * 180 - 90);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation)), ModContent.ProjectileType<DeadlyThunderboltBranch>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                rotation = (float)(rand.NextDouble() * 180 - 90);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation)), ModContent.ProjectileType<DeadlyThunderboltBranch>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
