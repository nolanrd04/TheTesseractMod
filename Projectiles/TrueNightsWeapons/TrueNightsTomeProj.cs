using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using TheTesseractMod.GlobalFuncitons;
using TheTesseractMod.Projectiles.NightsWeapons;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.TrueNightsWeapons
{
    internal class TrueNightsTomeProj : ModProjectile
    {
        private int baseWidth;
        private int baseHeight;
        private int baseDamage;
        private float changeFactor;
        private int modVal;
        private float NextManaFrame
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            baseDamage = Projectile.damage;

            baseWidth = Projectile.width;
            baseHeight = Projectile.height;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];


            if (Projectile.owner == Main.myPlayer)
            {
                bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);
                if (ShouldConsumeMana())
                {
                    player.manaRegenDelay = 60f;
                }

                bool stillInUse = manaIsAvailable && player.channel && !player.noItems && !player.CCed;

                if (stillInUse)
                {
                    float distance = Vector2.Distance(Main.MouseWorld, player.MountedCenter);
                    if (distance < 60f)
                    {
                        changeFactor = 1f;
                    }
                    else if (distance < 700)
                    {
                        changeFactor = 1f - ((distance - 60f) / 715f);
                    }
                    else
                    {
                        changeFactor = 0.1f;
                    }

                    Projectile.damage = (int)((float)(player.HeldItem.damage) * changeFactor);
                    Projectile.width = (int)(baseWidth * changeFactor);
                    Projectile.height = (int)(baseHeight * changeFactor);
                    Projectile.localNPCHitCooldown = 20 - ((int)(10 * changeFactor));
                    modVal = 30 - (int)(30 * changeFactor);


                    Projectile.position = Main.MouseWorld + new Vector2(-Projectile.width / 2, -Projectile.height / 2);
                    Projectile.ai[0]++;

                    if (Projectile.ai[0] % 3 == 0)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 speed = new Vector2(10, 0) * (changeFactor / 4);
                            speed = speed.RotatedBy(i * 24);
                            bool colorGreen = Main.rand.NextBool();
                            if (colorGreen)
                            {
                                Dust.NewDust(Projectile.Center, 1, 1, DustID.Terra, speed.X * 2, speed.Y * 2, 0, default(Color), 1f);
                            }
                            else
                            {
                                Dust.NewDust(Projectile.Center, 1, 1, 27, speed.X, speed.Y, 0, default(Color), 1f);
                            }
                        }
                    }

                    if (Projectile.ai[0] % modVal == 0)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RadialGlowDust>(), 0, 0, 150, new Color(234 / 255f, 94 / 255f, 255 / 155f), 2f);
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RadialGlowDust>(), 0, 0, 150, new Color(182 / 255f, 255 / 255f, 36 / 255f), 2f);
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
        }
        private bool ShouldConsumeMana()
        {
            // If the mana consumption timer hasn't been initialized yet, initialize it and consume mana on frame 1.
            NextManaFrame++;
            if (NextManaFrame % 15 == 1f)
            {
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            NPC targetNew = GlobalProjectileFunctions.findSecondClosestTarget(Projectile.Center);

            if (Vector2.Distance(Projectile.Center, targetNew.Center) < 250)
            {
                Vector2 direction = Vector2.Normalize(targetNew.Center - Projectile.Center);
                Vector2 newVelocity = direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(90) - 45)) * 10f;

                if (Main.rand.Next(3) == 0)
                {
                    if (Main.rand.NextBool())
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, newVelocity, ModContent.ProjectileType<CustomShadowFlame>(), Projectile.damage, 0);
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, newVelocity, ModContent.ProjectileType<TrueCustomCursedFlame>(), Projectile.damage, 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item103, Projectile.Center);
                }
            }
        }
    }
}
