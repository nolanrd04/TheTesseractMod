using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Magic
{
    public class RiftFractureProj : ModProjectile
    {
        private VertexStrip strip = new VertexStrip();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0.8f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => new Color(255, 252, 153, 0) * (1f - progress),
                progress => MathHelper.Lerp(15f, 7f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2f;
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}