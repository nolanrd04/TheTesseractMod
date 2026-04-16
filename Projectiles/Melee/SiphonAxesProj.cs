using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Melee
{
    public class SiphonAxesProj : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 9999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Player player = Owner;
            if (player.dead || !player.active || player.HeldItem.type != ModContent.ItemType<Items.Weapons.Melee.SiphonAxes>())
            {
                Projectile.Kill();
                return;
            }

            Owner.heldProj = Projectile.whoAmI;

            Vector2 toMouse = Main.MouseWorld - player.MountedCenter;
            Projectile.velocity = toMouse.SafeNormalize(Vector2.UnitX);

            Projectile.Center = player.MountedCenter + Projectile.velocity + new Vector2(35f, 0).RotatedBy(Projectile.velocity.ToRotation()) + new Vector2(0, -25f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)                                                                                                                                  
        {                                                                                                                                                                                                              
            Vector2 direction = Projectile.rotation.ToRotationVector2();                                                                                                                                               
            Vector2 start = Projectile.Center - direction * Projectile.width / 2f;
            Vector2 end = Projectile.Center + direction * Projectile.width / 2f * 1.2f;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.height / 2f, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity.X >= 0)
            {
                Projectile.spriteDirection = -1;
            }
            else
            {
                Projectile.spriteDirection = 1;
            }

            bool facingLeft = Projectile.velocity.X < 0;
            SpriteEffects spriteEffects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float drawRotation = facingLeft ? Projectile.rotation + MathHelper.Pi : Projectile.rotation;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            float offsetX = 24f;
            origin.X = facingLeft ? sourceRectangle.Width - offsetX : offsetX;

            // draw main sprite
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, drawRotation, origin, Projectile.scale, spriteEffects, 0);

            
            
            int frameSpeed = 8;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.Kill();
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.CanBeChasedBy() && !target.friendly )
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), target.Center, new Vector2(10f, 0), ModContent.ProjectileType<SiphonAxeHealBeam>(), Projectile.damage, 0f, Projectile.owner);
            }
        }
    }
}