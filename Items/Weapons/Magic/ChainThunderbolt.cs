using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace TheTesseractMod.Items.Weapons.Magic
{
    internal class ChainThunderbolt : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 20;
            Item.useTime = 3;
            Item.useAnimation = 3;
            Item.mana = 4;
            Item.damage = 150;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.DD2_LightningBugZap;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<ChainThunderboltProjectile>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8f, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity)*50;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(3)), type, damage, knockback);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-3)), type, damage, knockback);
            return true;
        }
    }
}
