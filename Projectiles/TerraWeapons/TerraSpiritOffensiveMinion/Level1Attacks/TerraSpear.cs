using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using TheTesseractMod.Buffs;
using Terraria.Audio;

namespace TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion.Level1Attacks
{
    internal class TerraSpear : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Projectiles/TerraWeapons/TerraSpiritOffensiveMinion/TerraSpear";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 30;
            Projectile.height = 30;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Terra);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() + new Vector2(-15, -7.5f) ;
            

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc != null && npc.active && npc.HasBuff(ModContent.BuffType<TargetMarked>()) && npc.type != NPCID.TargetDummy)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int j = 0; j < npc.buffType.Length; j++)
                        {
                            if (npc.buffType[j] == ModContent.BuffType<TargetMarked>())
                            {
                                npc.DelBuff(j);
                                break;
                            }
                        }
                    }
                }
            }
            target.AddBuff(ModContent.BuffType<TargetMarked>(), 180);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Vector2 velocity = Projectile.velocity;
            for (int i = 0; i < 20; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(18));
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Terra, velocity.X, velocity.Y, 0, default(Color), 1f);
            }
        }
    }
}
