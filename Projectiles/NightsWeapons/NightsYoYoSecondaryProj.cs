using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsYoYoSecondaryProj : ModProjectile
    {
        float rotation = MathHelper.ToRadians(15);
        public override string Texture => "TheTesseractMod/Textures/BlankYoyoProj";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.alpha = 70;
            Projectile.penetrate = 4;
        }

        public override void AI()
        {
            if (Projectile.alpha > 255)
            {
                Projectile.Kill();
            }

            Projectile.rotation += rotation;
            rotation *= .95f;
            Lighting.AddLight(Projectile.position, new Vector3(255 / 255f, 130 / 255f, 251 / 255f));
            Projectile.alpha+=2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(new Color(66 / 255f, 4 / 255f, 217 / 255f)),
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(new Color(255 / 255f, 130 / 255f, 251 / 255f)),
                Projectile.rotation, origin, Projectile.scale * .9f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(new Color(102 / 255f, 9 / 255f, 217 / 255f)),
                Projectile.rotation, origin, Projectile.scale * .75f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(new Color(66 / 255f, 4 / 255f, 217 / 255f)),
                Projectile.rotation, origin, Projectile.scale * .4f, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.NightsEdge, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
        }
    }
}
