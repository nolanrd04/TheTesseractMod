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
using TheTesseractMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Projectiles.TerraWeapons
{
    internal class TerraTurretLeg : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private Vector2 offset;
        public override string Texture => "TheTesseractMod/Textures/TerraTurretLeg";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.timeLeft = 6;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnSpawn(IEntitySource source)
        {
            offset = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }


        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Owner.Center + new Vector2(0, Owner.gfxOffY) + offset;
            SetPosition();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void PostDraw(Color lightColor)
        {

            /*** DRAW LEG ***/
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            Rectangle frame = new Rectangle(0, 0, 22, 36);
            Vector2 origin = frame.Size() / 2f + new Vector2(0, -14);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame),
                Color.White,
                0, origin, Projectile.scale, SpriteEffects.None, 0);
        }
        public void SetPosition()
        {
            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
        }
    }
}
