using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraArrow : ModProjectile
    {
        private VertexStrip strip = new VertexStrip();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 50;
            Projectile.scale = 1.25f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        Color color = new Color(222, 120, 255);
        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 newPosition = Projectile.position + new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);
            Dust.NewDustPerfect(newPosition, ModContent.DustType<SharpRadialGlowDust>(), Vector2.Zero, 0, Color.Lime, 0.5f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["RainbowRod"].Apply();
            strip.PrepareStrip(
                Projectile.oldPos,
                Projectile.oldRot,
                progress => Color.Lime * (1f - progress),
                progress => MathHelper.Lerp(15f, 8f, progress),
                -Main.screenPosition + Projectile.Size / 2f,
                Projectile.oldPos.Length,
                includeBacksides: true
            );

            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                new Color(87 / 255f, 255 / 255f, 233 / 255f) * (1f - Projectile.alpha / 255f), 
                Projectile.rotation, origin, Projectile.scale * 1.2f, spriteEffects, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.Lime * (1f - Projectile.alpha / 255f),
                Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
            Vector2 velocity = Projectile.velocity;
            for (int i = 0; i < 20; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(18));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Terra, velocity.X, velocity.Y, 0, default(Color), 1f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.TerraBlade, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
        }
    }
}
