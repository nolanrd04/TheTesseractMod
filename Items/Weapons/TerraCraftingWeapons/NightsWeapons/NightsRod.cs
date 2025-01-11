using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.NightsWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.JungleWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons
{
    internal class NightsRod : ModItem // casts a slow-moving shadow orb that goes to the mouse cursor
    {
        public override void SetDefaults()
        {

            Item.staff[Item.type] = true;
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.width = 26;
            Item.height = 26;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NightsRodBeam>();
            Item.shootSpeed = 10;
            Item.mana = 13;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CursedStaff>());
            recipe.AddIngredient(ModContent.ItemType<UnholyCaster>());
            recipe.AddIngredient(ItemID.MagicMissile);
            recipe.AddIngredient(ItemID.Flamelash);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
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
