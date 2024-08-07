using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheTesseractMod.Global.Projectiles.Ranged;
using Terraria.DataStructures;

namespace TheTesseractMod.Projectiles.Ranged
{
    internal class ApexN31Bullet : ModProjectile
    {
        public int whoShot = -1;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            //Projectile.CloneDefaults(ProjectileID.IchorBullet);
            //AIType = ProjectileID.IchorBullet;

            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            //Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }
        public override void OnSpawn(IEntitySource source)
        {
            whoShot = Projectile.owner;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 explosionVector = new Vector2(0.1f, 0.1f);

            // set solar explosion to range projectile
            ApexN31GlobalProjectile.isRangedAttack = true;
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, explosionVector, ProjectileID.SolarWhipSwordExplosion, Projectile.damage/2, Projectile.knockBack, Projectile.owner);
            
            // change calamity damage
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, explosionVector, ModContent.ProjectileType<ApexN31Explosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            ApexN31GlobalProjectile.isRangedAttack = false;

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 explosionVector = new Vector2(0.1f, 0.1f);
            ApexN31GlobalProjectile.isRangedAttack = true;
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, explosionVector, ProjectileID.SolarWhipSwordExplosion, Projectile.damage/2, Projectile.knockBack, Projectile.owner);
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, explosionVector, ModContent.ProjectileType<ApexN31Explosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            ApexN31GlobalProjectile.isRangedAttack = false;
            return true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, Projectile.velocity.X, Projectile.velocity.Y, 150, default(Color), 0.4f);
            Lighting.AddLight(Projectile.position, 0.3f, 0.7f, 0.3f);
            Lighting.Brightness(1, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
                
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        /*
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(EntitySource source, Projectile.Center, Projectile.velocity, 79, Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        */

        //public override Texture2D Texture => "Terraria/Images/Projectile_" + ProjectileID.RainbowRodBullet;
    }
}
