using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.NightsWeapons;
using Microsoft.Xna.Framework;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.JungleWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons
{
    internal class NightsTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<NightsTomeProjectile>();
            Item.shootSpeed = 10;
            Item.mana = 8;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StingerStorm>());
            recipe.AddIngredient(ModContent.ItemType<PurpleHeart>());
            recipe.AddIngredient(ItemID.WaterBolt);
            recipe.AddIngredient(ItemID.DemonScythe);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld + new Vector2(32, 32);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<NightsTomeProjectile>()] <= 0;
        }
    }
}
