using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.TrueExcaliburWeapons;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.Ranged;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    internal class Petashark : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item54;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 13f;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PetasharkBullet>(), damage, knockback);
            Projectile.NewProjectile(source, position, (velocity).RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)), ModContent.ProjectileType<PetasharkBubble>(), damage, knockback);
            Projectile.NewProjectile(source, position, (velocity).RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)), ModContent.ProjectileType<PetasharkBubble>(), damage, knockback);

            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.Y += 11;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() <= 0.25f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, 8);
        }
    }
}
