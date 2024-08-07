using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Projectiles.Summoner;

namespace TheTesseractMod.Items.Weapons.Summoner
{
    public class EtherealBubbler : ModItem
    {
        public override void SetDefaults()
        {
            Item.staff[Item.type] = true;
            Item.width = 86;
            Item.height = 86;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item155;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 200;
            Item.knockBack = 0f;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = 10;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EtherealBubble>();
            Item.shootSpeed = 13f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(ModContent.ItemType<AquaRiftFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<LightRiftFragment>(), 8);
            recipe.AddIngredient(ModContent.ItemType<LifeRiftFragment>(), 5);
            recipe.AddIngredient(ModContent.ItemType<ElectricRiftFragment>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.statMana >= 55)
            {
                player.statMana -= 55;
            }

            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.statMana >= 55;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 55f;


            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);

        }
    }
}
