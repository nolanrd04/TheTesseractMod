using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons;
using TheTesseractMod.Projectiles.TrueNightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons
{
    internal class TrueNightsRod : ModItem
    {
        public override void SetDefaults()
        {

            Item.staff[Item.type] = true;
            Item.damage = 35;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4.75f;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TrueNightsRodProj>();
            Item.shootSpeed = 15;
            Item.mana = 15;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<NightsRod>());
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(7)), type, damage, knockback);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-7)), type, damage, knockback);
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 60;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}
