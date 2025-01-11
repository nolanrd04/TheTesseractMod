using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Buffs.HolyBuffs;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.TrueExcaliburWeapons
{
    internal class TrueGoldenMageFriendlyMagic : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        private int[] buffList = { ModContent.BuffType<HolyRage>(), ModContent.BuffType<HolyRegen>(), ModContent.BuffType<HolySkin>(), ModContent.BuffType<HolySwiftness>(), ModContent.BuffType<HolyEndurance>(), ModContent.BuffType<HolyLifeForce>(), ModContent.BuffType<HolyWrath>() };
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Player target = Main.player[Projectile.owner];

            if (target.active)
            {
                Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .25f);
            }
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 0, Color.Yellow, .3f);



            if (Projectile.Colliding(Projectile.Hitbox, target.Hitbox))
            {
                int buffIndex = Main.rand.Next(7);

                switch (buffIndex)
                {
                    case 0:
                        if (target.HasBuff(BuffID.Rage))
                        {
                            if (target.HasBuff(buffList[buffIndex]))
                            {
                                target.ClearBuff(buffList[buffIndex]);
                            }
                            target.AddBuff(buffList[buffIndex], 300);
                        }
                        else
                        {
                            target.AddBuff(BuffID.Rage, 240);
                        }
                        break;

                    case 1:
                        if (target.HasBuff(BuffID.Regeneration))
                        {
                            if (target.HasBuff(buffList[buffIndex]))
                            {
                                target.ClearBuff(buffList[buffIndex]);
                            }
                            target.AddBuff(buffList[buffIndex], 480);
                        }
                        else
                        {
                            target.AddBuff(BuffID.Regeneration, 480);
                        }
                        break;

                    case 2:
                        if (target.HasBuff(BuffID.Ironskin))
                        {
                            if (target.HasBuff(buffList[buffIndex]))
                            {
                                target.ClearBuff(buffList[buffIndex]);
                            }
                            target.AddBuff(buffList[buffIndex], 300);
                        }
                        else
                        {
                            target.AddBuff(BuffID.Ironskin, 300);
                        }
                        break;

                    case 3:
                        if (target.HasBuff(BuffID.Swiftness))
                        {
                            if (target.HasBuff(buffList[buffIndex]))
                            {
                                target.ClearBuff(buffList[buffIndex]);
                            }
                            target.AddBuff(buffList[buffIndex], 420);
                        }
                        else
                        {
                            target.AddBuff(BuffID.Swiftness, 420);
                        }
                        break;

                    case 4:
                        if (target.HasBuff(BuffID.Endurance))
                        {
                            if (target.HasBuff(buffList[buffIndex]))
                            {
                                target.ClearBuff(buffList[buffIndex]);
                            }
                            target.AddBuff(buffList[buffIndex], 300);
                        }
                        else
                        {
                            target.AddBuff(BuffID.Endurance, 300);
                        }
                        break;

                    case 5:
                        if (target.HasBuff(BuffID.Lifeforce))
                        {
                            if (target.HasBuff(buffList[buffIndex]))
                            {
                                target.ClearBuff(buffList[buffIndex]);
                            }
                            target.AddBuff(buffList[buffIndex], 420);
                        }
                        else
                        {
                            target.AddBuff(BuffID.Lifeforce, 420);
                        }
                        break;

                    case 6:
                        if (target.HasBuff(BuffID.Wrath))
                        {
                            if (target.HasBuff(buffList[buffIndex]))
                            {
                                target.ClearBuff(buffList[buffIndex]);
                            }
                            target.AddBuff(buffList[buffIndex], 240);
                        }
                        else
                        {
                            target.AddBuff(BuffID.Wrath, 240);
                        }
                        break;
                }
                Projectile.Kill();
            }

            // Create a dust effect
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 90, Color.Yellow, .5f);
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}
