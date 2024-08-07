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
using Terraria.Audio;
using TheTesseractMod.Projectiles.Melee;
using TheTesseractMod.Projectiles.Melee.ZenithYoYoChildProjectiles;

namespace TheTesseractMod.Projectiles.Developer
{
    
    internal class ZenithYoYoProjectileDEV : ModProjectile
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

        public override void PostAI()
        {
            Random rand = new Random();
            float rotation = (float)(rand.NextDouble() * 360);
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
                //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position, new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(rotation)), ModContent.ProjectileType<ZenithYoYoProjectileEnergySphere>(), damage / 2, Projectile.knockBack, Projectile.owner);
            }
            counter++;

            Projectile.ai[2]++;
            // generate unique attack effect
            if (Projectile.ai[2] % 600 == 0)
            {
                attackTypeCounter = 0;
                attackType ++;  // 0 = evil, 1 = Amazon, 2 = hive five, 3 = chik, 4 = hel-fire, 5 = amarok, 6 = eye of cthulu, 7 = evil corruption
            }
            if (attackType > 7)
            {
                attackType = 0;
            }
            
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (attackType == 3) // chik
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(20f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ProjectileID.CrystalStorm, Projectile.damage / 4, 0f);
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
        private void EvilCrimson()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, DustID.FireworksRGB, 0, 0, 125, Color.Red, 1f);
                }
            }


            if (attackTypeCounter % 40 == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(15f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ProjectileID.GoldenShowerFriendly, Projectile.damage / 4, 0f);
            }
            attackTypeCounter++;
        }

        private void Amazon()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(200, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, DustID.FireworksRGB, 0, 0, 125, Color.DarkGreen, 1f);
                }
            }


            if (attackTypeCounter % 20 == 0 && TargetInRange(200))
            {
                NPC target = Main.npc[findTarget()];
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();

                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, direction * 10f, ModContent.ProjectileType<ZenithYoYoStinger>(), Projectile.damage / 4, 0f);
            }
            attackTypeCounter++;
        }

        private void HiveFive()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(300, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, DustID.FireworksRGB, 0, 0, 125, Color.Gold, 1f);
                }
            }

            if (attackTypeCounter % 20 == 0 && TargetInRange(300))
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(20f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), new Vector2(15f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ProjectileID.Bee, Projectile.damage / 4, 0f);
            }
            attackTypeCounter++;
        }

        private void Chik()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, DustID.FireworksRGB, 0, 0, 125, Color.Violet, 1f);
                }
            }
            attackTypeCounter++;
        }

        private void Amarok()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, DustID.FireworksRGB, 0, 0, 0, Color.Blue, 1f);
                }
            }
            attackTypeCounter++;
        }

        private void HelFire()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, DustID.FireworksRGB, 0, 0, 125, Color.OrangeRed, 1f);
                }
            }
            attackTypeCounter++;
        }

        private void EOC()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(70, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, DustID.FireworksRGB, 0, 0, 0, Color.White, 1f);
                }
            }

            if (attackTypeCounter % 20 == 0)
            {
                Vector2 offset = Projectile.Center + new Vector2(60, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                Vector2 direction = Projectile.Center - offset;
                direction.Normalize();
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), offset, direction * 20f, ModContent.ProjectileType<ZenithYoYoEye>(), Projectile.damage / 4, 0f);
            }
            attackTypeCounter++;
        }
        private void EvilCorruption()
        {
            if (attackTypeCounter % 4 == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(i * 18)), 1, 1, DustID.FireworksRGB, 0, 0, 0, Color.Purple, 1f);
                }
            }

            if (attackTypeCounter % 30 == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(15f, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), ProjectileID.CursedFlameFriendly, Projectile.damage / 4, 0f);
            }
            attackTypeCounter++;
        }

        private bool TargetInRange(int range)
        {
            bool inRange = false;
            int targetType = findTarget();
            if (targetType != -1)
            {
                NPC target = Main.npc[findTarget()];
                float dist = Vector2.Distance(target.Center, Projectile.Center);
                if (dist < range)
                {
                    return true;
                }
            }
            return inRange;
        }
        public int findTarget() // returns the closest npc
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC && npc.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(Projectile.position, npc.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }
            }
            return closestNPCIndex;
        }
    }
}
