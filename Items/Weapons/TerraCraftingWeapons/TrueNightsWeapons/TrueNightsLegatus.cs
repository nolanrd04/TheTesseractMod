using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Buffs.MinionBuffs;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.DungeonWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.TrueNightsWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons
{
    internal class TrueNightsLegatus : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 1;
            Item.UseSound = SoundID.Item44;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 37;
            Item.knockBack = 4f;
            Item.mana = 15;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<TrueNightsCrescentProj>();
            Item.buffType = ModContent.BuffType<TrueCrescentMinionBuff>();
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<NightsLegatus>());
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
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
