using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.Projectiles.Magic;

namespace TheTesseractMod.Items.Weapons.Magic
{
    public class StormOfThorns : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 230;
            Item.DamageType = DamageClass.Magic;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            // Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ThornStormMainHead>();
            Item.shootSpeed = 15f;
            Item.crit = 9;
            Item.mana = 22;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Vilethorn, 1)
                .AddIngredient(ModContent.ItemType<SoliumBar>(), 20)
                .AddIngredient(ModContent.ItemType<ChloroRiftFragment>(), 15)
                .AddIngredient(ItemID.JungleSpores, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)))  * 0.5f, ModContent.ProjectileType<ThornStormMini>(), damage / 2, knockback, player.whoAmI);
            }
            return true;
        }
    }
}