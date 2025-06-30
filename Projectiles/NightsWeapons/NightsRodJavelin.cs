using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using Terraria.DataStructures;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsRodJavelin : ModProjectile
    {
        private bool IsStickingToTarget;

        public float StickTimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private const int StickTime = 240;

        private bool hasHit = false;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.friendly = true;
            Projectile.scale = .75f;
            Projectile.alpha = 125;
        }

        public override void AI()
        {
            if (IsStickingToTarget)
            {
                StickyAI();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] > 60)
            {
                Projectile.alpha++;
            }

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }

            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() + new Vector2(-25, -25f);


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                new Color(234 / 255f, 94 / 255f, 255 / 155f, (255- Projectile.alpha) / 255f),
                Projectile.rotation + MathHelper.ToRadians(45f), origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float distanceFromCenter = Vector2.Distance(Projectile.Center, target.Center);
            if (!hasHit)
            {
                IsStickingToTarget = true; // we are sticking to a target
                TargetWhoAmI = target.whoAmI; // Set the target whoAmI
                Projectile.velocity = new Vector2(distanceFromCenter * .95f, 0).RotatedBy(Projectile.rotation);
                Projectile.netUpdate = true; // netUpdate this javelin
            }
            hasHit = true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Vector2 velocity = new Vector2(5, 0);
            for (int i = 0; i < 20; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(18));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, velocity.X, velocity.Y, 0, default(Color), 1f);
            }
        }

        private void StickyAI()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            StickTimer += 1f;

            // Every 30 ticks, the javelin will perform a hit effect
            // bool hitEffect = StickTimer % 30f == 0f;
            int npcTarget = TargetWhoAmI;
            if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200)
            { // If the index is past its limits, kill it
                Projectile.Kill();
            }
            else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
            {
                // If the target is active and can take damage
                // Set the projectile's position relative to the target's center
                Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                /*if (hitEffect)
                {
                    // Perform a hit effect here, causing the npc to react as if hit.
                    // Note that this does NOT damage the NPC, the damage is done through the debuff.
                    Main.npc[npcTarget].HitEffect(0, 1.0);
                }*/
            }
            else
            { // Otherwise, kill the projectile
                Projectile.Kill();
            }
        }
    }
}
