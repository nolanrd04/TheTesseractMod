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
using TheTesseractMod.Projectiles.TerraWeapons;
using TheTesseractMod.Projectiles.TerraWeapons.TerraSpiritOffensiveMinion.Level1Attacks;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TerraWeapons
{
    internal class TerraTrident : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.damage = 60;
            Item.knockBack = 7;
            Item.crit = 4;

            Item.width = 80;
            Item.height = 80;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Summon;
            Item.UseSound = SoundID.Item1;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<TerraTridentProjectile>();
            Item.shootSpeed = 5f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 40;
            Projectile.NewProjectile(source, position + muzzleOffset, new Vector2(15f, 0f).RotatedBy((Main.MouseWorld - player.MountedCenter).ToRotation()), ModContent.ProjectileType<TerraSpear>(), damage, knockback);

            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 20);
            recipe.AddIngredient(ItemID.BrokenHeroSword);

        }
    }
}
