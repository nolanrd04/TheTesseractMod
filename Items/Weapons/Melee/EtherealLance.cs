using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using TheTesseractMod.Projectiles.Melee.EtherealLanceProjectiles;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Projectiles.Melee.EtherealSwordProjectiles;

namespace TheTesseractMod.Items.Weapons.Melee
{
    internal class EtherealLance : ModItem // code heavily adapted from Example Mod
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.damage = 250;
            Item.knockBack = 7;
            Item.crit = 4;

            Item.width = 80;
            Item.height = 80;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item1;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<EtherealLanceProjectile>();
            Item.shootSpeed = 5f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 60;
            Projectile.NewProjectile(source, position + muzzleOffset, new Vector2(7f, 0f).RotatedBy((Main.MouseWorld - player.MountedCenter).ToRotation()), ModContent.ProjectileType<EtherealLanceStar>(), damage / 2, knockback);
            
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(ModContent.ItemType<LightRiftFragment>(), 7);
            recipe.AddIngredient(ModContent.ItemType<DeathRiftFragment>(), 11);
            recipe.AddIngredient(ModContent.ItemType<ColdRiftFragment>(), 15);
            recipe.Register();

        }
    }
}
