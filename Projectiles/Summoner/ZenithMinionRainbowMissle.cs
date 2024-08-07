using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Summoner
{
    internal class ZenithMinionRainbowMissle : ModProjectile
    {
        List<Color> colorsForDust = new List<Color>();
        int colorsIDX = 0;
        int counterForColorChange = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            //AIType = ProjectileID.RainbowRodBullet;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 240;
            Projectile.light = 0.9f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            colorsForDust.Add(Color.Red); colorsForDust.Add(Color.Orange); colorsForDust.Add(Color.Yellow); colorsForDust.Add(Color.Green); colorsForDust.Add(Color.Turquoise); colorsForDust.Add(Color.Blue); colorsForDust.Add(Color.Purple); colorsForDust.Add(Color.Magenta);
        }

        public override void AI()
        {
            /***********COLORING*************/
            Projectile.rotation += 0.15f * (float)Projectile.direction;
            Lighting.AddLight(Projectile.position, Color.White.ToVector3());
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, Projectile.velocity.X, Projectile.velocity.Y, 100, colorsForDust[colorsIDX], 1.5f);
            counterForColorChange++;
            if (counterForColorChange%5 == 0)
            {
                colorsIDX++;
            }
            if (colorsIDX > 7)
            {
                colorsIDX = 0;
            }
            /********************************/

            //***Will speed up proj if too slow***//
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 15f;
            //************************************//


            /*for (int i = 0; i < Main.maxNPCs; i++)
            {*/
            NPC target = Main.npc[findTarget()];

                if (target.CanBeChasedBy())
                {
                /*homing segment*/
                    float goToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                    float goToY = target.position.Y - Projectile.Center.Y;
                    float distance = (float)Math.Sqrt(goToX * goToX + goToY * goToY);

                    if (distance < 2000 && !target.friendly && target.active && distance > 100)
                    {
                        distance = 4f / distance;
                        goToX *= distance * 59;
                        goToY *= distance * 59;

                        Projectile.velocity.X += goToX / 60;
                        Projectile.velocity.Y += goToY / 60;
                    }

                    if (Projectile.velocity.LengthSquared() > 225)
                    {
                        Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 15f;
                    }

            }
            //}

            //little push
            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = 4f;
                Projectile.velocity.Y = 4f;
            }
            
        }
        public int findTarget()
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC)
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
