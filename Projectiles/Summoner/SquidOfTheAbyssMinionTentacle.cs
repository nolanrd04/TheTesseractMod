using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Summoner
{
    internal class SquidOfTheAbyssMinionTentacle : ModProjectile
    {
        // ai[0] = parent squid projectile index
        // ai[1] = target NPC index

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 300;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            int parentIndex = (int)Projectile.ai[0];
            int targetIndex = (int)Projectile.ai[1];

            // Validate parent squid
            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles ||
                !Main.projectile[parentIndex].active ||
                Main.projectile[parentIndex].type != ModContent.ProjectileType<SquidOfTheAbyssMinion>())
            {
                Projectile.Kill();
                return;
            }

            // Validate target NPC
            if (targetIndex < 0 || targetIndex >= Main.maxNPCs ||
                !Main.npc[targetIndex].active ||
                !Main.npc[targetIndex].CanBeChasedBy())
            {
                Projectile.Kill();
                return;
            }

            // Keep alive as long as parent lives
            Projectile.timeLeft = 2;

            // Position hitbox at the target NPC for damage
            Projectile.Center = Main.npc[targetIndex].Center;
        }

        // Collision along the tentacle line from squid to target
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int parentIndex = (int)Projectile.ai[0];
            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles || !Main.projectile[parentIndex].active)
                return false;

            // If the parent squid is already overlapping the target, allow damage
            if (Main.projectile[parentIndex].Hitbox.Intersects(targetHitbox))
                return true;

            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Main.projectile[parentIndex].Center,
                Projectile.Center,
                10f,
                ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int parentIndex = (int)Projectile.ai[0];
            int targetIndex = (int)Projectile.ai[1];

            if (parentIndex < 0 || parentIndex >= Main.maxProjectiles || !Main.projectile[parentIndex].active)
                return false;
            if (targetIndex < 0 || targetIndex >= Main.maxNPCs || !Main.npc[targetIndex].active)
                return false;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 startWorld = Main.projectile[parentIndex].Center;
            Vector2 endWorld = Main.npc[targetIndex].Center;
            Vector2 direction = endWorld - startWorld;
            float totalLength = direction.Length();

            if (totalLength < 1f)
                return false;

            float rotation = direction.ToRotation() + MathHelper.PiOver2;
            Vector2 unitDir = direction / totalLength;
            float step = texture.Height;
            if (step < 1f) step = 1f;

            int segments = (int)(totalLength / step) + 1;
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

            for (int i = 0; i < segments; i++)
            {
                float progress = i * step;
                if (progress > totalLength)
                    break;

                Vector2 drawPos = startWorld + unitDir * progress - Main.screenPosition;

                Main.EntitySpriteDraw(texture,
                    drawPos,
                    null,
                    lightColor,
                    rotation,
                    origin,
                    1f,
                    SpriteEffects.None,
                    0f);
            }

            return false;
        }
    }
}