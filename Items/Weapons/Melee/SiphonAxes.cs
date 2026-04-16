using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.Projectiles.Melee;

namespace TheTesseractMod.Items.Weapons.Melee
{
    public class SiphonAxes : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 245;
            Item.DamageType = DamageClass.Melee;
            Item.width = 86;
            Item.height = 76;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Red;
            // Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.crit = 7;
            Item.shoot = ModContent.ProjectileType<SiphonAxesProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item71 with { Pitch = 0.5f, PitchVariance = 0.3f, Volume = 0.8f }, player.position);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FetidBaghnakhs, 1)
                .AddIngredient(ModContent.ItemType<SoliumBar>(), 20)
                .AddIngredient(ModContent.ItemType<LifeRiftFragment>(), 15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}