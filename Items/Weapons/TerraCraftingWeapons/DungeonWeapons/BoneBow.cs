using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.DungeonWeapons
{
    internal class BoneBow : ModItem
    {
        public override void SetDefaults()
        {

            Item.damage = 29;
            Item.useAmmo = AmmoID.Arrow;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = 5;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(0, 1, 75, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 8;

            Item.noMelee = true;

        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(ItemID.Cobweb, 40);
            recipe.AddTile(TileID.Anvils);

            recipe.Register(); ;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ProjectileID.BoneArrow;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }
    }
}
