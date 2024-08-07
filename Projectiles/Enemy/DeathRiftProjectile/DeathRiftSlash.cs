using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Enemy.DeathRiftProjectile
{
    internal class DeathRiftSlash : ModProjectile
    {
        int random = Main.rand.Next(2);
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults() 
        {
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.height = 49;
            Projectile.width = 49;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Player target = Main.player[GetClosetPlayer()];
            
            if (Projectile.ai[0] % 2 == 0)
            {
                //orange
                if (random == 0)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, direction, ModContent.ProjectileType<OrangeDeathSickle>(), 70, 2f);
                }
                //supposed to be purple but for now orange
                else
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, direction, ModContent.ProjectileType<OrangeDeathSickle>(), 70, 2f);
                }
                
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 5)
            {
                Projectile.Kill();
            }
        }

        public int GetClosetPlayer()
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];

                if (player.active)
                {
                    float distance = Vector2.Distance(Projectile.position, player.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }
            }
            return closestNPCIndex;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
