using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace TheTesseractMod.Projectiles.NightsWeapons
{
    internal class NightsRodSecondaryProj : ModProjectile
    {
        private float speed;
        private float distFromTarget;
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
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = 450;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 75;
        }

        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
        }

        public override void AI()
        {
            if (IsStickingToTarget)
            {
                StickyAI();
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                for (int i = 0; i < 3; i++)
                {
                    // Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RadialGlowDust>(), Vector2.Zero, 175, Color.Purple, .5f);
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0, 0, 150, default(Color), 1.3f);
                    Main.dust[dust].noGravity = true;
                }
                if (Projectile.ai[0] > 20)
                {
                    Vector2 desiredVelocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, .01f);
                }

                Projectile.ai[0]++;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size()/2f;


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Projectile.GetAlpha(new Color(255/255f, 130/255f, 251/255f)),
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale * .5f, SpriteEffects.None, 0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float distanceFromCenter = Vector2.Distance(Projectile.Center, target.Center);
            if (!hasHit)
            {
                IsStickingToTarget = true; // we are sticking to a target
                TargetWhoAmI = target.whoAmI; // Set the target whoAmI
                Projectile.velocity = new Vector2(distanceFromCenter, 0).RotatedBy(Projectile.rotation);
                Projectile.netUpdate = true; // netUpdate this javelin
                Projectile.timeLeft = 1000;
            }
            hasHit = true;
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
                Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 1.5f;
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
