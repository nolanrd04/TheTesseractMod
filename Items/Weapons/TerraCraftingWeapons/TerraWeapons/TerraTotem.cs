using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using TheTesseractMod.Buffs.MinionBuffs;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.HallowedWeapons;
using TheTesseractMod.Projectiles.TrueExcaliburWeapons;
using Microsoft.Xna.Framework;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueExcaliburWeapons;
using TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TerraWeapons
{
    internal class TerraTotem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = 1;
            Item.UseSound = SoundID.Item44;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 70;
            Item.knockBack = 5.5f;
            Item.mana = 15;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<TerraSpiritOffenseMinion>();
            Item.buffType = ModContent.BuffType<TerraSpiritMinionBuff>();
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TrueNightsLegatus>());
            recipe.AddIngredient(ModContent.ItemType<TrueHolyCommander>());
            recipe.AddIngredient(ItemID.PygmyStaff);
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

            int newDamage = (int)(damage * (player.maxMinions / 7f));
            var projectile = Projectile.NewProjectile(source, position, velocity, type, newDamage, knockback, Main.myPlayer);

            return false;
        }
    }
}
