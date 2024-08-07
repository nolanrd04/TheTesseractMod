using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.Ranged;
using TheTesseractMod.Items.Weapons.Ranged;
using Terraria.Audio;
using System.ComponentModel.DataAnnotations;
using TheTesseractMod.Buffs;

namespace TheTesseractMod.Global.Projectiles.Ranged
{
    public class ApexN31GlobalProjectile : GlobalProjectile // used to count when an APEXN31 bullet collides with an enemy
    {
        //variables used to keep track of projectile hits
        public static bool isRangedAttack = false; // used to change other class's projectiles to ranged when the apex n31 is being used.
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            int bullet = ModContent.ProjectileType<ApexN31Bullet>(); //get the type of the bullet
            if (target.type != NPCID.TargetDummy)
            {
                if (projectile.type == bullet)
                {
                    if (EditPlayer.ApexN31BulletHits < 150 && !player.HasBuff(ModContent.BuffType<ApexOverclockBuff>())) //the code will only work if the bullet hits <40. so at 40 it stops running
                    {
                        EditPlayer.ApexN31BulletHits++;
                        if (EditPlayer.ApexN31BulletHits % 15 == 0) //for every four hits:
                        {
                            SoundEngine.PlaySound(SoundID.MaxMana, player.position);
                            EditPlayer.ApexN31LoadedRocketCount++;
                            EditPlayer.maxRocketIdx++;
                        }
                    }

                    if (EditPlayer.maxRocketIdx == 10)
                    {
                        EditPlayer.maxRocketIdx++;
                        SoundEngine.PlaySound(SoundID.Item29, player.position); //only runs once. the right click code in ApexN31 will reset this
                    }
                }
            }
        }

        public override void SetDefaults(Projectile entity)
        {
            if (isRangedAttack && entity.type == ProjectileID.SolarWhipSwordExplosion)
            {
                entity.DamageType = DamageClass.Ranged;
            }
            else if (!isRangedAttack && entity.type == ProjectileID.SolarWhipSwordExplosion)
            {
                entity.DamageType = DamageClass.Melee;
            }
        }
    }
}
