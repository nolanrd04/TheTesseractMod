using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TheTesseractMod.GlobalFuncitons;
using TheTesseractMod.Dusts;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraTorchFlamePositive : ModProjectile
    {
        private float incrementalAngle = (float)Math.PI/2;
        private float direction;
        private float scalingFactor;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
            Projectile.scale = 1.25f;
        }

        public override void AI()
        {
            Dust.NewDustPerfect(new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f), ModContent.DustType<TerraDust>(), Vector2.Zero, 170, Color.Aqua, 0.9f);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f), ModContent.DustType<TerraDust>(), Vector2.Zero, 120, Color.Lime, 0.6f);
            }

            if (Projectile.ai[0] == 0)
            {
                direction = Projectile.velocity.ToRotation();
            }
            incrementalAngle += 0.1f;

            Projectile.position.Y += (float)(Math.Sin(incrementalAngle) * Math.Cos(-direction) * 3);
            Projectile.position.X += (float)(Math.Sin(incrementalAngle) * Math.Sin(-direction) * 3);
            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fade = (float)Math.Sin(Projectile.ai[0] * MathHelper.TwoPi / 60f);
            fade = (fade + 1f) / 2f;
            scalingFactor = GlobalMathFunctions.Lerp(0.25f, 0.5f, fade);

            // Draw Circle Glow
            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Textures/InwardRadialGlow");
            Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                new Color(136, 237, 85, 0) * (1f - Projectile.alpha / 255f), 0, glowTexture.Size() * 0.5f, 0.05f, SpriteEffects.None, 0f);

            // Draw Main BG 
            Asset<Texture2D> mainTexture = ModContent.Request<Texture2D>("TheTesseractMod/Textures/SharpCenterOutwardRadialGlow");
            Main.EntitySpriteDraw(mainTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, mainTexture.Value.Width, mainTexture.Value.Height),
                new Color(136, 237, 85, 0) * (1f - Projectile.alpha / 255f), 0, mainTexture.Size() * 0.5f, scalingFactor * 0.225f, SpriteEffects.None, 0f);

            //Draw Main
            Main.EntitySpriteDraw(mainTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, mainTexture.Value.Width, mainTexture.Value.Height),
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f), 0, mainTexture.Size() * 0.5f, scalingFactor * 0.125f, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerraTorchProjImpact>(), Projectile.damage, 0);
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
    }
}
