using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace TheTesseractMod.Projectiles.Melee
{
    internal class DeathFlameChild : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.velocity =  new Vector2(1, 2);
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            //Projectile.hostile = false;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 80;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        Color color1 = new Color(255, 0, 0);
        public override void AI()
        {
            /*if (Projectile.scale < 0.8f)
            {
                Projectile.scale += 0.1f;
            }*/
            //make the projectile light up red
            Lighting.AddLight(Projectile.position, 1f, 0f, 0f);
            //make the projectile shoot red dust
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 235, 0, 0, 150, color1, 0.7f);
            Projectile.ai[0]++;
            Projectile.rotation += 0.2f * (float)Projectile.direction;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            // custom drawing code
            Main.instance.LoadProjectile(ModContent.ProjectileType<DeathFlameChild>());
            Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<DeathFlameChild>()].Value;
            
            // this is stuff required by Main.EntitySpriteDraw
            Vector2 origin = new Vector2(9.5f, 26.5f);
            Color color = Lighting.GetColor(Projectile.position.ToTileCoordinates());

            // specifies the size and origin for the first frame
            Rectangle frame = new Rectangle(0, 0, 19, 53);
            // this is the line that actually draws the sprite
            Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition, frame, color, Projectile.rotation, origin, 0.3f, SpriteEffects.None, 0);

            // return false, as we have already drawn the projectile
            return false;
        }
    }
}
