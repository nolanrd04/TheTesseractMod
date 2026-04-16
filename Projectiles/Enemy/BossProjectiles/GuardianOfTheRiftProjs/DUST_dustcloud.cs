using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    internal class DUST_dustcloud : ModProjectile // This will be a dust cloud that moves to the right of the boss. When it gets to a certain position it will spawn a dust tornado.
        // dust tornado will be a stationary invisible projectile that is compensated visually with dust effects.
    {
        private float travelingSpeed;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 180;
            Projectile.scale = 4f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            travelingSpeed = Projectile.velocity.Length();
        }
        public override void AI()
        {

            Projectile.rotation += 0.05f;

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(50), default(Color), 3f);
            Main.dust[dust].noGravity = true;

            if(Main.rand.Next(4) == 0)
            {
                // Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DustCloud>(), 0, 0, Main.rand.Next(50), Color.Orange, .7f);
                
            }
            if (Main.rand.Next(4) == 0)
            {
                Dust.NewDust(Projectile.Center, 0, 0, DustID.Dirt, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(50), default(Color), Main.rand.NextFloat(1.5f, 2.5f));
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {   
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture,
             new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
             new Rectangle(0, 0, texture.Width, texture.Height),
             lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
