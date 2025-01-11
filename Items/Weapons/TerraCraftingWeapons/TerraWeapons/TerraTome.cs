using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.TerraWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueExcaliburWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TerraWeapons
{
    internal class TerraTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TerraTomeFlame>();
            Item.shootSpeed = 20f;
            Item.mana = 8;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TrueNightsTome>());
            recipe.AddIngredient(ModContent.ItemType<ShatterPoint>());
            recipe.AddIngredient(ItemID.MagnetSphere);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
