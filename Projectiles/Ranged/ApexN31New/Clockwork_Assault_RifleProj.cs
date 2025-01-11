using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.Ranged.ApexN31New
{
    internal class Clockwork_Assault_RifleProj : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            Projectile.rotation = (Main.MouseWorld - Owner.MountedCenter).ToRotation();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.ai[0] == 0)
            {
                //int index = Main.rand.Next(15);
                //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(20f, 0).RotatedBy((Main.MouseWorld - Owner.MountedCenter).ToRotation()), arrayBulletID[index], Projectile.damage, Projectile.knockBack);
            }

            //Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            Projectile.Center = Owner.Center + new Vector2(0, Owner.gfxOffY) + new Vector2(-26, -38);
            Projectile.rotation = (Main.MouseWorld - Owner.MountedCenter).ToRotation();
            Owner.heldProj = Projectile.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(sourceRectangle),
                Color.White,
                Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}
