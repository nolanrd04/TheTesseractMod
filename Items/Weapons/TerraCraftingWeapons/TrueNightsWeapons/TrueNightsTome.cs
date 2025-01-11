using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.TrueNightsWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons
{
    internal class TrueNightsTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4.75f;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TrueNightsTomeProj>();
            Item.shootSpeed = 10;
            Item.mana = 9;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<NightsTome>());
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld + new Vector2(32, 32);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<TrueNightsTomeProj>()] <= 0;
        }
    }
}
