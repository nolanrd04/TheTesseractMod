using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Weapons.Ranged;

namespace TheTesseractMod.Projectiles.Ranged
{
    public class BlizzardCannonProj : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        public override string Texture => "TheTesseractMod/Items/Weapons/Ranged/BlizzardCannon";



        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 44;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 9999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.85f;
        }

        public override void AI()
        {
            Player player = Owner;
            if (player.dead || !player.active || player.HeldItem.type != ModContent.ItemType<BlizzardCannon>() || player.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            Owner.heldProj = Projectile.whoAmI;

            bool facingRight = player.direction == 1;
            float fireAngle = facingRight ? -MathHelper.ToRadians(35f) : -MathHelper.ToRadians(155f);
            Projectile.velocity = fireAngle.ToRotationVector2();

            Projectile.Center = player.MountedCenter + new Vector2(0f, 0f);
            Projectile.spriteDirection = player.direction;
            Projectile.rotation = facingRight ? -MathHelper.ToRadians(35f) : MathHelper.ToRadians(35f);

            Vector2 dustDirection = fireAngle.ToRotationVector2() * 10f;

            Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<DustCloud>(), dustDirection.X, dustDirection.Y, 0, Color.White, 1.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            bool facingRight = Projectile.spriteDirection == 1;
            SpriteEffects spriteEffects = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2f;

            Vector2 drawOffset = new Vector2(Owner.direction * 15f, 0f);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + drawOffset,
                null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}