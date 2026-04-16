using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using Terraria.DataStructures;
using TheTesseractMod.GlobalFuncitons;

namespace TheTesseractMod.Projectiles.Melee
{
    internal class PowerHammerLightning:ModProjectile
    {
        private int ConsecutiveNegative = 0;
        private int ConsecutivePositive = 0;

        List<float> rotations = new List<float>();
        private Random rand = new Random();
        
        public override void SetDefaults()
        {
            Projectile.damage = 100;
            Projectile.alpha = 0;
            Projectile.timeLeft = 60;
            Projectile.light = 0.9f;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.extraUpdates = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {

            // map out path on first frame
            if (Projectile.ai[0] == 0)
            {
                // map out rotations
                for (int i = 0; i < 4; i++)
                {
                    float rotation = MathHelper.ToRadians((float)(rand.NextDouble() * 100 - 50));
                    if (ConsecutiveNegative == 2)
                    {
                        rotation = MathHelper.ToRadians(30);
                        ConsecutiveNegative = 0;
                    }
                    if (ConsecutivePositive == 2)
                    {
                        rotation = MathHelper.ToRadians(-30);
                        ConsecutivePositive = 0;
                    }

                    if (rotation > 0 && ConsecutiveNegative == 0)
                    {
                        ConsecutivePositive++;
                    }
                    else
                    {
                        ConsecutivePositive = 0;
                    }

                    if (rotation < 0 && ConsecutivePositive == 0)
                    {
                        ConsecutiveNegative++;
                    }
                    else
                    {
                        ConsecutiveNegative = 0;
                    }

                    rotations.Add(rotation);
                }

                // move to new position based on rotation path
                for (int i = 0; i < rotations.Count; i++)
                {
                    float speed = Projectile.velocity.Length();
                    Projectile.position += new Vector2(speed, 0).RotatedBy(Projectile.velocity.ToRotation() + rotations[i]);
                }
            }

            // move back to original position following same path in reverse
            if (Projectile.ai[0] % 15 == 0 && rotations.Count > 0)
            {
                float rotation = rotations[rotations.Count - 1];
                rotations.RemoveAt(rotations.Count - 1);
                Projectile.velocity = Projectile.velocity.RotatedBy(-rotation);
            }
            Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<ElectricDust>(), 0, 0, 0, Color.Blue, .3f);
            
            Projectile.ai[0]++;
        }

        override public void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.friendly = false; // prevent hitting multiple NPCs in one swing
        }
    }
}
