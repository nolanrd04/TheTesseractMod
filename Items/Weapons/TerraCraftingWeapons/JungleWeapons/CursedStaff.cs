using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.JungleWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.JungleWeapons
{
    internal class CursedStaff : ModItem // casts a poison energy sphere
    {
        public override void SetDefaults()
        {

            Item.staff[Item.type] = true;
            Item.damage = 15;
            Item.DamageType = DamageClass.Magic;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 54, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item120;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CursedStaffProj>();
            Item.shootSpeed = 15;
            Item.mana = 18;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 6);
            recipe.AddIngredient(ItemID.Stinger, 12);
            recipe.AddIngredient(ItemID.JungleSpores, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}
