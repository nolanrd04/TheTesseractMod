using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using TheTesseractMod.Projectiles.TrueExcaliburWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueExcaliburWeapons
{
    internal class ShatterPoint : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.width = 40;
            Item.height = 40;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.knockBack = 3f;
            Item.crit = 4;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.UseSound = SoundID.Item9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.shootSpeed = 11f;
            Item.shoot = ModContent.ProjectileType<ShatterPointCrystal>();
            Item.mana = 5;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrystalStorm);
            recipe.AddIngredient(ItemID.GoldenShower);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 24);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
