using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons;
using TheTesseractMod.Projectiles.TerraWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueExcaliburWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TerraWeapons
{
    internal class TerraTurret : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 43;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 17f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<TerraTurretProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, new Vector2(0, -40), ModContent.ProjectileType<TerraTurretProj>(), damage, knockback);
            Projectile.NewProjectile(source, position, new Vector2(0, -40), ModContent.ProjectileType<TerraTurretLeg>(), damage, knockback);
            Vector2 direction = (Main.MouseWorld - position).SafeNormalize(Vector2.Zero);
            velocity = direction * Item.shootSpeed;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback);

            if (Main.rand.Next(4) == 0)
            {
                Projectile.NewProjectile(source, position, velocity * 1.25f, ModContent.ProjectileType<TerraBullet>(), damage, knockback);
            }
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -40);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TrueNightsSixShooter>());
            recipe.AddIngredient(ModContent.ItemType<Gigashark>());
            recipe.AddIngredient(ItemID.ChainGun);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() <= 0.50f;
        }
    }
}
