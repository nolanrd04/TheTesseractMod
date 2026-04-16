using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.Projectiles.Summoner;

namespace TheTesseractMod.Items.Weapons.Summoner
{
    public class WhipOfTheWildWest : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(WhipOfTheWestTagBuff.TagDamage);
        public override void SetDefaults() {
			// This method quickly sets the whip's properties.
			// Mouse over to see its parameters.
			Item.DefaultToWhip(ModContent.ProjectileType<WhipOfTheWestProj>(), 204, 4, 4);
			Item.rare = ItemRarityID.Red;
			Item.channel = true;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BlandWhip, 1)
                .AddIngredient(ModContent.ItemType<SoliumBar>(), 20)
                .AddIngredient(ModContent.ItemType<DustRiftFragment>(), 15)
                .AddIngredient(ModContent.ItemType<DeathRiftFragment>(), 15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}