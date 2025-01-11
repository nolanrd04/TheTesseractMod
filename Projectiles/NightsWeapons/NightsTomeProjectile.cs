using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsTomeProjectile : ModProjectile
    {
        private int baseWidth;
        private int baseHeight;
        private int baseDamage;
        private float changeFactor;
        private float NextManaFrame
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            baseDamage = Projectile.damage;

            baseWidth = Projectile.width;
            baseHeight = Projectile.height;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];


            if (Projectile.owner == Main.myPlayer)
            {
                bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);
                if(ShouldConsumeMana())
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



                    Projectile.position = Main.MouseWorld + new Vector2(-Projectile.width / 2, -Projectile.height / 2);
                    Projectile.ai[0]++;

                    if (Projectile.ai[0] % 3 == 0)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            Vector2 speed = new Vector2(10, 0) * (changeFactor / 4);
                            speed = speed.RotatedBy(i * 24);
                            Dust.NewDust(Projectile.Center, 1, 1, 27, speed.X, speed.Y, 0, default(Color), 1f);
                        }
                    }
                    

                    Dust.NewDust(player.MountedCenter, 15, 15, 27);
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
    }
}
