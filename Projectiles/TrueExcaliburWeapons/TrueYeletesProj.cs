using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Drawing;

namespace TheTesseractMod.Projectiles.TrueExcaliburWeapons
{
    internal class TrueYeletesProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 16f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 432f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 18f;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 23f;
            }
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Projectile.localNPCHitCooldown = 15;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            if (Projectile.ai[2] % 5 == 0)
            {
                if(Main.rand.NextBool())
                {
                    ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
                }

                if (Main.rand.NextBool())
                {
                    ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Vector2.Zero });
                }
            }

            Projectile.ai[2]++;
        }
    }
}
