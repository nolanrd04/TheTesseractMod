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
using TheTesseractMod.Projectiles.Summoner.ShadowFlameDragon;

namespace TheTesseractMod.Items.Weapons.Summoner
{
    public class EtherealCommander : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }


        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 74;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 1;
            Item.UseSound = SoundID.Item44;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 200;
            Item.knockBack = 5f;
            Item.mana = 15;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<ShadowFlameDragonMinion>();
            Item.buffType = ModContent.BuffType<ShadowFlameDragonBuff>();
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(ModContent.ItemType<LifeRiftFragment>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DarkRiftFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<HeatRiftFragment>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);

            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, -10f);
        }
    }
}
