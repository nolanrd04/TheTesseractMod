using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.GlobalFuncitons;
using Microsoft.Xna.Framework;
using System.Net.Mail;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using rail;
using Terraria.Audio;
using TheTesseractMod.Buffs.MinionBuffs;

namespace TheTesseractMod.Projectiles.DungeonWeapons
{
    internal class DarkCaster : ModProjectile
    {
        int phase = 0;
        bool hasCollided = false;
        float attackSight = 900f;
        float projectileSpeed = 10f;
        NPC target;

        //------------------------------------------------------------------------------------------------------------------------

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 32;
            Projectile.height = 48;
            Projectile.tileCollide = true;
            Projectile.friendly = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            
        }

        public override bool? CanCutTiles()
        {
            return false;
        }


        public override bool MinionContactDamage()
        {
            return true;
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
            {
                return;
            }
            
            target = GlobalProjectileFunctions.findClosestTargetInRange(owner.Center, attackSight);
            if (target != null)
            {
                MovementAndAttack(owner);
                Projectile.ai[0]--;
            }
            else
            {
                if(Projectile.frame > 0)
                {
                    Projectile.frame = 0;
                    phase = 0;
                    Projectile.ai[0] = 0;
                }
                Movement(owner);
            }
            Visuals();
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<DarkCasterMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<DarkCasterMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void Visuals()
        {
            if (target != null)
            {
                if (Projectile.position.X > target.position.X)
                {
                    Projectile.spriteDirection = 1;
                }
                else
                {
                    Projectile.spriteDirection = -1;
                }
            }

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 59, 0, 0, 0, default(Color), 1.5f);
            Main.dust[dust].noGravity = true;
        }

        private void Attack()
        {
            Vector2 targetCenter = target.Center;
            Vector2 direction = targetCenter - (Projectile.Center + new Vector2(0, -25));
            direction.Normalize();
            direction *= projectileSpeed;
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(0, -25), direction, ModContent.ProjectileType<WaterSphere>(), Projectile.damage, Projectile.knockBack);
            SoundEngine.PlaySound(SoundID.Item8, Projectile.position);

        }

        private void Movement(Player owner)
        {
            if (IsOnBlock())
            {
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.velocity = new Vector2(0, 9.81f);
            }

            float distanceFromOwner = Vector2.Distance(Projectile.Center, owner.Center);
            if (distanceFromOwner > 1000)
            {
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                Vector2 teleportPosition = FindRandomTeleportLocation(owner);
                Projectile.position = teleportPosition;
                Projectile.ai[1] = 0;
            }
        }

        private void MovementAndAttack(Player owner)
        {
            if (phase == 0) // TELEPORT
            {
                if (Projectile.ai[0] == 0) // set the phase timer
                {
                    Projectile.ai[0] = 60;
                }
                if (Projectile.ai[0] % 1 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                    Vector2 teleportPosition = FindRandomTeleportLocation(owner);
                    Projectile.position = teleportPosition;
                    phase = 1;
                    Projectile.ai[0] = 120;
                    Projectile.ai[1] = 0;
                }
            }
            else
            {
                if (Projectile.ai[0] == 60)
                {
                    Attack();
                    Projectile.frame++;
                }
                else if (Projectile.ai[0] == 45)
                {
                    Attack();
                }
                else if (Projectile.ai[0] == 30)
                {
                    Attack();
                }
                else if (Projectile.ai[0] == 15)
                {
                    Projectile.frame--;
                    phase = 0;
                }
            }
            if (IsOnBlock())
            {
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.velocity = new Vector2(0, 9.81f);
            }
        }




        private Vector2 FindRandomTeleportLocation(Player player)
        {
            Vector2 teleportPos = player.position + new Vector2(Projectile.width, Projectile.height);
            for (int i = 0; i < 1000; i++)
            {
                Vector2 randomPos = player.Center + new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));


                int tileX = (int)(randomPos.X / 16);
                int tileXbehind = (int)(randomPos.X / 16);
                int tileXinFront = (int)(randomPos.X / 16);
                int tileY = (int)(randomPos.Y / 16);

                Tile tileXMain = Framing.GetTileSafely(tileX, tileY);
                Tile tileBehind = Framing.GetTileSafely(tileXbehind, tileY);
                Tile tileInFront = Framing.GetTileSafely(tileXinFront, tileY);

                if (ValidTile(tileXMain) && ValidTile(tileBehind) && ValidAirSpace(tileX, tileY) && ValidAirSpace(tileXbehind, tileY))
                {
                    teleportPos = randomPos;
                    break;
                }
                else if (ValidTile(tileXMain) && ValidTile(tileInFront) && ValidAirSpace(tileX, tileY) && ValidAirSpace(tileXinFront, tileY))
                {
                    teleportPos = randomPos;
                    break;
                }
            }
            return teleportPos + new Vector2(0, -64);
        }
        private bool Blacklisted(Tile tile)
        {
            return tile.TileType == 3 || tile.TileType == 73/*grass*/ || tile.TileType == 4 /*torches*/ || tile.TileType == 5 /*trees*/ || tile.TileType == 10 || tile.TileType == 11 /*doors*/ || tile.TileType == 21 /*chests*/ ||
                tile.TileType == 24 /*corrupt grass*/ || tile.TileType == 28 /*pots*/ || tile.TileType == 61  || tile.TileType == 62  || tile.TileType == 74 /*jungle grass*/ || tile.TileType == 71 /*mushrooms*/; 
        }
        private bool ValidTile(Tile tile)
        {
            return (WorldGen.SolidTile(tile) && !tile.IsActuated && !Blacklisted(tile)) || tile.TileType == TileID.Platforms ;
        }
        private bool ValidAirSpace(int posX, int posY)
        {
            Tile X1 = Main.tile[posX, posY - 1];
            Tile X2 = Main.tile[posX, posY - 2];
            Tile X3 = Main.tile[posX, posY - 3];


            if (!X1.HasTile && !X2.HasTile && !X3.HasTile)
            {
                return true;
            }
            return false;
        }

        private bool IsOnBlock()
        {
            int tileX = (int)(Projectile.position.X / 16f);
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            int tileX2 = (int)((Projectile.position.X + 16) / 16f);

            Tile tile = Framing.GetTileSafely(tileX, tileY);
            Tile tile2 = Framing.GetTileSafely(tileX2, tileY);

            if (((tile != null && tile.HasTile) || tile.TileType == TileID.Platforms) || ((tile2 != null && tile2.HasTile) || tile2.TileType == TileID.Platforms))
            {
                float platformTop = (tileY * 16f) - Projectile.height;
                if (Projectile.position.Y + Projectile.height >= platformTop)
                {
                    Projectile.position.Y = platformTop + 1f; // Align with platform
                    return true;
                }
            }
            return false;
        }
        private bool IsOnPlatform()
        {
            // Calculate tile coordinates
            int tileX = (int)(Projectile.position.X / 16f);
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            int tileX2 = (int)((Projectile.position.X + 16) / 16f);

            // Get the tile at the projectiles position
            Tile tile = Framing.GetTileSafely(tileX, tileY);
            Tile tile2 = Framing.GetTileSafely(tileX2, tileY);

            if ((tile != null && tile.HasTile && tile.TileType == TileID.Platforms) || (tile2 != null && tile2.HasTile && tile2.TileType == TileID.Platforms))
            {
                float platformTop = (tileY * 16f) - Projectile.height;
                if (Projectile.position.Y + Projectile.height >= platformTop)
                {
                    Projectile.position.Y = platformTop + 1f; // Align with platform
                    return true;
                }
            }
            return false;
        }
    }
}
