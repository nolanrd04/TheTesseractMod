using Microsoft.Xna.Framework;
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

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueExcaliburWeapons
{
    internal class Gigashark : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 15f;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool())
            {
                return true;
            }
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<GigasharkBullet>(), damage, knockback);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Megashark);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 24);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() <= 0.50f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
    }
}
