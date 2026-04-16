using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System;
using System.IO;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
    public class AQUA_AquanadoBolt : ModProjectile
    {
        private float lastSpawnY = float.NaN;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // scale of the projectiles that this bolt spawns
            Projectile.ai[2] = 1f;
        }

        public override void AI()
        {
            Projectile.ai[0] ++;
            if (Projectile.ai[0] % 3 == 0 && Projectile.ai[1] == 0f)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.UltraBrightTorch, 0f, 0f, 0, default(Color), 1.2f);
            }

            if (Projectile.ai[1] == 1f)
            {
                Projectile.velocity = new Vector2(0f, -10f);
                if (Projectile.ai[0] % 5 == 0)
                {
                    float spawnProjScale = Projectile.ai[2];
                    float currentHalfHeight = 21f * spawnProjScale;
                    float previousHalfHeight = 21f * (spawnProjScale / 1.05f);

                    float spawnY;
                    if (float.IsNaN(lastSpawnY))
                    {
                        spawnY = Projectile.Center.Y;
                    }
                    else
                    {
                        spawnY = lastSpawnY - (previousHalfHeight + currentHalfHeight);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, spawnY), Vector2.Zero, ModContent.ProjectileType<AQUA_Aquanado>(), 200, 0f);
                        Main.projectile[proj].ai[1] = spawnProjScale;
                    }

                    lastSpawnY = spawnY;
                    Projectile.ai[2] *= 1.05f;
                }
            }

            if (Projectile.ai[0] >= 60)
            {
                Projectile.ai[1] = 1f;
                Projectile.alpha = 255;
            }
            else
            {
                Projectile.ai[1] = 0f;
                Projectile.alpha = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];

            for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(31, 31) + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture,
                        drawPos,
                        new Rectangle(0, Projectile.frame * frameHeight, 61, frameHeight),
                        color, Projectile.rotation, new Vector2(31, 31), Projectile.scale, SpriteEffects.None, 0f);
                }

            Main.EntitySpriteDraw(texture,
                new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f),
                new Rectangle(0, Projectile.frame * frameHeight, 61, frameHeight),
                Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(31, 31), Projectile.scale, SpriteEffects.None, 0f);


            int frameSpeed = 4;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(lastSpawnY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            lastSpawnY = reader.ReadSingle();
        }
    }
}