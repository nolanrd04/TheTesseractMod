using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.EvilWeapons;
using TheTesseractMod.Buffs.MinionBuffs;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons
{
    internal class CreeperStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }


        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 27, 0);
            Item.rare = ItemRarityID.Green;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 1;
            Item.UseSound = SoundID.Item44;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 9;
            Item.knockBack = 2.5f;
            Item.mana = 15;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<CreeperMinion>();
            Item.buffType = ModContent.BuffType<CreeperMinionBuff>();
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.AbigailsFlower);
            if (WorldGen.crimson)
            {
                recipe.AddIngredient(ItemID.CrimtaneBar, 10);
                recipe.AddIngredient(ItemID.TissueSample, 7);
            }
            else
            {
                recipe.AddIngredient(ItemID.DemoniteBar, 10);
                recipe.AddIngredient(ItemID.ShadowScale, 7);
            }
            
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(WorldGen.crimson)
            {
                player.AddBuff(Item.buffType, 2);
            }
            else
            {
                player.AddBuff(ModContent.BuffType<CreeperMinionBuffPurple>(), 2);
            }
            
            var projectile = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);

            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, -10f);
        }
    }
}
