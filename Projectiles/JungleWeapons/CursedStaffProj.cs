using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Magic.EtherealStaffProjectile;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.JungleWeapons
{
    internal class CursedStaffProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            // Lighting.AddLight(Projectile.position, 255 / 255f, 48 / 255f, 20 / 255f);
            if (Projectile.ai[0] % 4 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ElectricDust>(), 0, 0, 0, Color.DarkGreen, 1f);
            }
            
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 120);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit53, Projectile.position);
            float rotation = 0f;
            for (int i = 0; i < 6; i++)
            {
                rotation = Main.rand.Next(360);
                Vector2 velocity = new Vector2(4f, 4f).RotatedBy(MathHelper.ToRadians(rotation));
                Dust.NewDust(Projectile.Center, Projectile.width * 2, Projectile.height * 2, ModContent.DustType<ElectricDust>(), velocity.X, velocity.Y, 0, Color.DarkGreen, 1f);
                if (i % 2 == 0)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, velocity, ModContent.ProjectileType<CursedStaffProjChild>(), Projectile.damage, 0f);
                }
                
            }
        }
    }
}
