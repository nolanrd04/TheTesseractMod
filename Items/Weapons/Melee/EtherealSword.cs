using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using TheTesseractMod.Projectiles.Melee.EtherealSwordProjectiles;
using TheTesseractMod.Items.Materials;

namespace TheTesseractMod.Items.Weapons.Melee
{
    internal class EtherealSword : ModItem // code heavily adapted from Example Mod
    {
        public int attackType = 0; // keeps track of which attack it is
        public int comboExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.autoReuse = true;
            Item.damage = 410;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.crit = 4;

            Item.shoot = ModContent.ProjectileType<EtherealSwordSwingProjectile>(); // The sword as a projectile
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
            attackType = (attackType + 1) % 2; // Increment attackType to make sure next swing is different
            comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
            return false; // return false to prevent original projectile from being shot
        }

        public override void UpdateInventory(Player player)
        {
            if (comboExpireTimer++ >= 120)
                attackType = 0;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(ModContent.ItemType<LightRiftFragment>(), 30);
            recipe.AddIngredient(ModContent.ItemType<DarkRiftFragment>(), 30);
            recipe.AddIngredient(ModContent.ItemType<DeathRiftFragment>(), 5);
            recipe.Register();

        }
    }
}
