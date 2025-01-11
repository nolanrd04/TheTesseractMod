using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Linq.Expressions;

namespace TheTesseractMod.Projectiles.NightsWeapons;

internal class NightsRodBeam : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.DamageType = DamageClass.Magic;
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 150;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.alpha = 255;
    }

    public override void AI()
    {
        //************************************//
        if (Projectile.ai[0] > 10)
        {
            Vector2 target = Main.MouseWorld;
            Projectile.velocity = Vector2.Normalize(target - Projectile.Center) * 10f;
        }

        Lighting.AddLight(Projectile.position, 127 / 255f, 54 / 255f, 255 / 255f);
        int dustID = Dust.NewDust(Projectile.position, 1, 1, 27, 0, 0, 0, default(Color), 2f);
        Main.dust[dustID].noGravity = true;
        Projectile.ai[0]++;
    }

    public override void OnKill(int timeLeft)
    {
        float rotation = 0f;
        for (int i = 0; i < 10; i++)
        {
            Vector2 velocity = new Vector2(2f, 2f).RotatedBy(MathHelper.ToRadians(rotation));
            Dust.NewDust(Projectile.position, 0, 0, 27, velocity.X, velocity.Y, 0, default(Color), 1f);
        }
    }
}
