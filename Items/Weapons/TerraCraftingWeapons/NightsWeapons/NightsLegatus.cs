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
using TheTesseractMod.Projectiles.NightsWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.DungeonWeapons;
using TheTesseractMod.Buffs.MinionBuffs;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons
{
    internal class NightsLegatus : ModItem
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
            Item.damage = 18;
            Item.knockBack = 4f;
            Item.mana = 15;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<NightsCrescentProj>();
            Item.buffType = ModContent.BuffType<NightsCrescentMinionBuff>();
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.HornetStaff);
            recipe.AddIngredient(ModContent.ItemType<CreeperStaff>());
            recipe.AddIngredient(ModContent.ItemType<DarkCasterStaff>());
            recipe.AddIngredient(ItemID.ImpStaff);
            recipe.AddTile(TileID.DemonAltar);
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
