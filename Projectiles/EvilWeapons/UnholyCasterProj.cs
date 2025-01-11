using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.EvilWeapons;

internal class UnholyCasterProj : ModProjectile
{
    private bool color = !WorldGen.crimson;
    public override void SetDefaults()
    {
        Projectile.DamageType = DamageClass.Magic;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = 2;
        Projectile.timeLeft = 180;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.alpha = 255;
    }

    public override void AI()
    {
        if (color) // purple
        {
            Lighting.AddLight(Projectile.position, 127 / 255f, 54 / 255f, 255 / 255f);
            for (int i = 0; i < 3; i++)
            {
                int dustID = Dust.NewDust(Projectile.position, 1, 1, 27, 0, 0, 0, default(Color), 1f);
                Main.dust[dustID].noGravity = true;
            }
            
        }
        else // red
        {
            Lighting.AddLight(Projectile.position, 1f, 0f, 0f);
            for (int i = 0; i < 3;  i++)
            {
                int dustID = Dust.NewDust(Projectile.position, 1, 1, DustID.GemRuby, 0, 0, 0, default(Color), 1f);
                Main.dust[dustID].noGravity = true;
            }
        }
        Projectile.ai[0]++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        float rotation = 0f;
        for (int i = 0; i < 10; i++)
        {
            Vector2 velocity = new Vector2(2f, 2f).RotatedBy(MathHelper.ToRadians(rotation));
            if (color) // purple
            {
                Dust.NewDust(Projectile.position, 0, 0, 27, velocity.X, velocity.Y, 0, default(Color), 1f);
            }
            else // red
            {
                int dustID = Dust.NewDust(Projectile.position, 1, 1, DustID.GemRuby, velocity.X, velocity.Y, 0, default(Color), 1f);
                Main.dust[dustID].noGravity = true;
            }
        }
    }
}
