using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Global.Projectiles.Magic;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Projectiles.Magic.EtherealStaffProjectile;

namespace TheTesseractMod.Items.Weapons.Magic
{
    public class EtherealStaff : ModItem
    {
        public override void SetDefaults()
        {

            Item.staff[Item.type] = true;
            Item.damage = 165;
            Item.DamageType = DamageClass.Magic;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(gold: 25);

            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MoltenSphere>();
            Item.shootSpeed = 11;
            Item.mana = 20;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(ModContent.ItemType<DustRiftFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<HeatRiftFragment>(), 15);
            recipe.Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

    }
}