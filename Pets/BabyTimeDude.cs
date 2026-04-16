using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Pets
{
    public class BabyTimeDude : ModProjectile
    {
        private float rotation;
        private int dustTimer;

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!owner.active)
            {
                Projectile.active = false;
                return;
            }

            CheckActive(owner);
            HoverAboveHead(owner);

            // Constant rotation
            rotation += 0.07f;

            // White light
            Lighting.AddLight(Projectile.Center, 1f, 1f, 1f);

            // Occasionally spawn sharp radial glow dust
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 15 == 0)
            {
                Color color = Color.Red;
                int colorPicker = Main.rand.Next(8);
                switch (colorPicker)
                {
                    case 0: color = Color.Red; break;
                    case 1: color = Color.Orange; break;
                    case 2: color = Color.Yellow; break;
                    case 3: color = Color.Green; break;
                    case 4: color = Color.Cyan; break;
                    case 5: color = Color.Blue; break;
                    case 6: color = Color.Indigo; break;
                    case 7: color = Color.Magenta; break;
                }
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SharpRadialGlowDust>(), 0f, 0f, 50, color, .8f);
            }
        }

        private void CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<Buffs.BabyTimeDudeBuff>());
            }

            if (owner.HasBuff(ModContent.BuffType<Buffs.BabyTimeDudeBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.Kill();
            }
        }

        private void HoverAboveHead(Player owner)
        {
            Vector2 targetPos = owner.Center + new Vector2(0f, -70f);
            Vector2 direction = targetPos - Projectile.Center;
            float distance = direction.Length();

            if (distance > 1000f)
            {
                // Teleport if too far
                Projectile.Center = targetPos;
                Projectile.velocity = Vector2.Zero;
                return;
            }

            float speed = 15f;
            float inertia = 10f;

            if (distance > 10f)
            {
                direction.Normalize();
                direction *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + direction) / inertia;
            }
            else
            {
                // Gentle bobbing when close
                Projectile.velocity *= 0.9f;
                Projectile.position.Y += (float)System.Math.Sin(Main.GameUpdateCount * 0.05f) * 0.3f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(
                texture,
                drawPos,
                null,
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f),
                rotation,
                drawOrigin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}