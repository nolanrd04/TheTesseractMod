using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Projectiles.Melee
{
    public class PowerHammerProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15; 
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        private const float ReturnSpeed = 16f;
        private const int OutwardTime = 45; // frames before returning

        public override void SetDefaults() {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // ai[0] = 0: flying outward, ai[0] = 1: returning
            Projectile.ai[1]++; // frame timer

            // Spin the projectile
            Projectile.rotation += 0.4f * Projectile.direction;

            if (Projectile.ai[0] == 0) {
                // Flying outward — switch to return after OutwardTime frames
                if (Projectile.ai[1] >= OutwardTime) {
                    Projectile.ai[0] = 1;
                }
            }

            if (Projectile.ai[0] == 1) {
                // Returning to player
                Vector2 toPlayer = owner.Center - Projectile.Center;
                float dist = toPlayer.Length();

                if (dist < 24f) {
                    Projectile.Kill();
                    return;
                }

                toPlayer.Normalize();
                toPlayer *= ReturnSpeed;
                Projectile.velocity = (Projectile.velocity * 6f + toPlayer) / 7f;
            }

            // Effects
            if (Projectile.ai[1] % 36 == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.position);
            }
            if (Projectile.ai[1] % 12 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust);
            }
            else if (Main.rand.NextBool(12))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ElectricDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, new Color(144, 89, 255), 1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // trail
            var textureBG = ModContent.Request<Texture2D>(Texture + "BG").Value;
            for (int k = 0; k < Projectile.oldPos.Length; k += 3)
            {
                Vector2 drawTrailPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = new Color(144, 89, 255) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(textureBG, drawTrailPos, null, color, Projectile.rotation, new Vector2(textureBG.Width / 2, textureBG.Height / 2), Projectile.scale, SpriteEffects.None, 0);
            }



            // Background
            int frameHeight = textureBG.Height / Main.projFrames[Type];
            Rectangle sourceRect = new Rectangle(0, Projectile.frame * frameHeight, textureBG.Width, frameHeight);
            Vector2 origin = sourceRect.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(textureBG, drawPos, sourceRect, Color.White, Projectile.rotation, origin, Projectile.scale*1.1f, effects, 0);

            // main
            var texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }

        override public void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0] = 0;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            int randNum = Main.rand.Next(1, 4);
                for (int i = 0; i < randNum; i++)
                {
                    Vector2 velocity = new Vector2(5f, 0).RotatedByRandom(MathHelper.ToRadians(360));
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, velocity, ModContent.ProjectileType<PowerHammerLightning>(), Projectile.damage, 0f, Main.myPlayer);
                }
        }
    }
}