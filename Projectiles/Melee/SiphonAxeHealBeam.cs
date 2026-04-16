using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Melee
{
    public class SiphonAxeHealBeam : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        float speed = 10f;
        private VertexStrip strip = new VertexStrip();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 9999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity = player.Center - Projectile.Center;
            Projectile.velocity.Normalize();
            Projectile.velocity *= speed;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.Colliding(Projectile.Hitbox, player.Hitbox))
            {
                int healAmount = Projectile.damage / 20;

                player.statLife += healAmount;
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }

                // Sync health with the server
                player.HealEffect(healAmount); // Creates a visual effect and syncs the heal
                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, player.whoAmI, healAmount);

                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => new Color(255, 94, 247, 0) * (1f - progress),
                progress => MathHelper.Lerp(15f, 7f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            return false;
        }
    }
}