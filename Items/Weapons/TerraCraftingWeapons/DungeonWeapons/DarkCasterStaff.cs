using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.EvilWeapons;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.DungeonWeapons;
using TheTesseractMod.Buffs.MinionBuffs;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.DungeonWeapons
{
    internal class DarkCasterStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }


        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 1, 75, 0);
            Item.rare = ItemRarityID.Green;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 30;
            Item.knockBack = 4f;
            Item.mana = 15;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<DarkCaster>();
            Item.buffType = ModContent.BuffType<DarkCasterMinionBuff>();
            Item.autoReuse = true;
            Item.crit = 6;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bone, 75);
            recipe.AddIngredient(ItemID.Robe);
            recipe.AddIngredient(ItemID.WaterCandle);
            recipe.AddTile(TileID.Anvils);
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
    }
}
