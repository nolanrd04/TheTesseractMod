using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseract.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    public class CHLORO_NatureEssence : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.hide = true;
        }
        public float Lerp(float x, float y, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return x + amount * (y - x);
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.alpha += 5;

            Player player = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
            if (player.active && !player.dead)
            {
                Projectile.Center = player.Center;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            /*Main.EntitySpriteDraw(glowTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f), rotationFactor, glowTexture.Size() * 0.5f, scalingFactor * 0.75f * 0.75f, SpriteEffects.None, 0f);*/

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Projectile.ai[1] = (float)Math.Sin(Projectile.ai[0] * MathHelper.TwoPi / 120f); // fade
            Projectile.ai[1] = (Projectile.ai[1] + 1f) / 2f;
            Projectile.ai[2] = Lerp(0.25f, 0.5f, Projectile.ai[1]); // scaling factor
           

            Projectile.rotation += MathHelper.ToRadians(8f);

            var textureRequest = ModContent.Request<Texture2D>("TheTesseractMod/Projectiles/Enemy/RiftProjectile");
            
            Main.spriteBatch.Draw(textureRequest.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, 0, textureRequest.Value.Width, textureRequest.Value.Height),
                new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f),
                Projectile.rotation, textureRequest.Size() * 0.5f, Projectile.ai[2] * 0.25f, SpriteEffects.None, 0f);
            
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<CHLORO_HealBeam>(), Projectile.damage, 0f);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    if (index == Projectile.whoAmI)
                    {
                        overPlayers.Add(index);
                    }
                }
            }
        }

    }
}