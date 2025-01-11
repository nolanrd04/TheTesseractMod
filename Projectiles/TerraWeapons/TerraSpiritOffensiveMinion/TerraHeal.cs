using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion
{
    internal class TerraHeal : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        float moveSpeed;
        public override void SetDefaults()
        {
            Projectile.height = 20;
            Projectile.width = 20;
            Projectile.timeLeft = 60;
        }
        public override void OnSpawn(IEntitySource source)
        {
            moveSpeed = Projectile.velocity.Length();
        }
        public override void AI()
        {
            Player target = Main.player[Projectile.owner];

            if (target.active)
            {
                Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * moveSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .25f);
            }
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.GemEmerald);
                Main.dust[dust.dustIndex].noGravity = true;
            }



            if (Projectile.Colliding(Projectile.Hitbox, target.Hitbox))
            {
                int healAmount = Projectile.damage / 10;

                target.statLife += healAmount;
                if (target.statLife > target.statLifeMax2)
                {
                    target.statLife = target.statLifeMax2;
                }

                // Sync health with the server
                target.HealEffect(healAmount); // Creates a visual effect and syncs the heal
                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, target.whoAmI, healAmount);

                Projectile.Kill();
            }
        }
    }
}
