using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Mono.Cecil;
using Microsoft.CodeAnalysis;
using Terraria.DataStructures;
using TheTesseractMod.Projectiles.Melee.ZenithYoYoChildProjectiles;
using Terraria.Audio;
using TheTesseractMod.Dusts;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.Melee
{
    
    internal class ZenithYoYoProjectile : ModProjectile
    {
        private static int counter = 0;
        private int attackType = 0; // 0 = evil, 1 = Amazon, 2 = hive five, 3 = chik, 4 = hel-fire, 5 = amarok, 6 = eye of cthulu
        private int attackTypeCounter = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 600f;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 750f;
                ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 10f;
            }
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20f;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Melee/ZenithYoYoProjectileGlow");
            Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f), Projectile.rotation, glowTexture.Size() * 0.5f, 1.1f, SpriteEffects.None, 0f);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (attackType == 3) // chik
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(20f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ProjectileID.CrystalStorm, Projectile.damage, 0f);
                }
            }

            if (attackType == 4) // helfire
            {
                target.AddBuff(BuffID.OnFire, 120);
            }

            if (attackType == 5) // amarok
            {
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }
        public override void PostAI()
        {
            float rotation = Main.rand.Next(360);
            
            if (counter == 15)
            {
                counter = 0;
            }

            Lighting.AddLight(Projectile.position, Color.White.ToVector3());
            if (counter % 8 == 0)
            {
                int damage = 0;
                float speed = 4f;
                if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
                {
                    damage = Projectile.damage;
                    speed = 6f;
                }
                else
                {
                    damage = Projectile.damage / 2;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(rotation)), ModContent.ProjectileType<ZenithYoYoProjectileEnergySphere>(), damage, Projectile.knockBack, Projectile.owner);
                }
            }
            counter++;

            // generate unique attack effect
            if (Projectile.ai[2] % 180 == 0)
            {
                attackTypeCounter = 0;
                attackType = Main.rand.Next(8);  // 0 = evil, 1 = Amazon, 2 = hive five, 3 = chik, 4 = hel-fire, 5 = amarok, 6 = eye of cthulu, 7 = evil corruption
            }
            Projectile.ai[2]++;
            switch (attackType)
            {
                case 0:
                    EvilCrimson();
                    break;
                case 1:
                    Amazon();
                    break;
                case 2:
                    HiveFive();
                    break;
                case 3:
                    Chik();
                    break;
                case 4:
                    HelFire();
                    break;
                case 5:
                    Amarok();
                    break;
                case 6:
                    EOC();
                    break;
                case 7:
                    EvilCorruption();
                    break;
            }
        }

        private void EvilCrimson()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(i * 36)), 1, 1, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 125, Color.Red, .5f);
                }
            }


            if (attackTypeCounter % 30 == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(15f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ProjectileID.GoldenShowerFriendly, Projectile.damage, 0f);
            }
            attackTypeCounter++;
        }

        private void Amazon()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(200, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 125, Color.Green, .5f);
                }
            }


            if (attackTypeCounter % 20 == 0 && TargetInRange(200))
            {
                NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();

                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction * 10f, ModContent.ProjectileType<ZenithYoYoStinger>(), Projectile.damage, 0f);
            }
            attackTypeCounter++;
        }

        private void HiveFive()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(300, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 125, Color.Gold, .5f);
                }
            }

            if (attackTypeCounter % 20 == 0 && TargetInRange(300))
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(20f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), new Vector2(15f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ProjectileID.Bee, Projectile.damage, 0f);
            }
            attackTypeCounter++;
        }

        private void Chik()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(i * (360 / 8))), 1, 1, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 125, Color.Pink, .5f);
                }
            }
            attackTypeCounter++;
        }

        private void Amarok()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(i * (360 / 8))), 1, 1, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, Color.SkyBlue, .5f);
                }
            }
            attackTypeCounter++;
        }

        private void HelFire()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(i * (360 / 8))), 1, 1, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 125, Color.OrangeRed, .5f);
                }
            }
            attackTypeCounter++;
        }

        private void EOC()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(70, 0).RotatedBy(MathHelper.ToRadians(i * (360 / 15))), 1, 1, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, Color.White, .5f);
                }
            }

            if (attackTypeCounter % 20 == 0)
            {
                Vector2 offset = Projectile.Center + new Vector2(60, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                Vector2 direction = Projectile.Center - offset;
                direction.Normalize();
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), offset, direction * 20f, ModContent.ProjectileType<ZenithYoYoEye>(), Projectile.damage, 0f);
            }
            attackTypeCounter++;
        }
        private void EvilCorruption()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(i * (360/14))), 1, 1, ModContent.DustType<SharpRadialGlowDust>(), 0, 0, 0, Color.Purple, .5f);
                }
            }

            if (attackTypeCounter % 30 == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(15f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ProjectileID.CursedFlameFriendly, Projectile.damage, 0f); 
            }
            attackTypeCounter++;
        }

        private bool TargetInRange(int range)
        {
            bool inRange = false;
            NPC target = GlobalProjectileFunctions.findClosestTarget(Projectile.Center);
            if (target != null)
            {
                float dist = Vector2.Distance(target.Center, Projectile.Center);
                if (dist < range)
                {
                    return true;
                }
            }
            return inRange;
        }
        
    }
}
