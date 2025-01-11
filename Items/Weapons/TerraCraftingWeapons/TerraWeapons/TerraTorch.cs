using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons;
using TheTesseractMod.Projectiles.TerraWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TerraWeapons
{
    internal class TerraTorch : ModItem
    {
        public override void SetDefaults()
        {

            Item.staff[Item.type] = true;
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.width = 82;
            Item.height = 82;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TerraTorchFlamePositive>();
            Item.shootSpeed = 16f;
            Item.mana = 12;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TrueNightsRod>());
            recipe.AddIngredient(ItemID.VenomStaff);
            recipe.AddRecipeGroup("DungeonStaff", 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TerraTorchFlameNegative>(), damage, knockback);
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 100;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}
