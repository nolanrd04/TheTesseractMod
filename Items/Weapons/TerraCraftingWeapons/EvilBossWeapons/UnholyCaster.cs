
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.EvilWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons
{
    internal class UnholyCaster : ModItem // casts a rainbow missle
    {
        public override void SetDefaults()
        {

            Item.staff[Item.type] = true;
            Item.damage = 32;
            Item.crit = 4;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5.5f;
            Item.value = Item.sellPrice(0, 0, 27, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<UnholyCasterProj>();
            Item.shootSpeed = 10;
            Item.mana = 8;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.AmberStaff);
            recipe.AddRecipeGroup("EvilBar", 10);
            recipe.AddRecipeGroup("evilitem", 5);
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
