using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons;
using TheTesseractMod.Projectiles.NightsWeapons;
using TheTesseractMod.Projectiles.TrueNightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons
{
    internal class TrueNightsSixShooter : ModItem

    {
        private int shotIndex = 0;
        private int typeIndex = 0;
        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.useAnimation = 48;
            Item.crit = 12;
            Item.useTime = 8;
            Item.reuseDelay = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<NightsBullet>();
            Item.shootSpeed = 15;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<NightsSixShooter>());
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 offset = new Vector2(-16, 4);
            if (player.direction == 1)
            {
                offset.X = -offset.X;
            }

            if (typeIndex == 0) // PURPLE
            {
                if (Main.rand.NextBool(4))
                {
                    Projectile.NewProjectile(source, position, velocity * 1.3f, ModContent.ProjectileType<SuperNightsBullet>(), damage, knockback);
                }
                Projectile.NewProjectile(source, position, offset, ModContent.ProjectileType<TrueNightsSixShooterPurpleProj>(), damage, knockback);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<NightsBullet>(), damage, knockback);

                Vector2 vel;
                vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15));
                Dust.NewDust(position, 1, 1, 27, vel.X / 2, vel.Y / 2, 0, default(Color), 1f);
            }
            else // GREEN
            {
                Projectile.NewProjectile(source, position, offset, ModContent.ProjectileType<TrueNightsSixShooterGreenProj>(), damage, knockback);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TrueNightsBullet>(), damage, knockback);

                Vector2 vel;
                vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15));
                Dust.NewDust(position, 1, 1, DustID.Terra, vel.X, vel.Y, 0, default(Color), .8f);
            }

            shotIndex = (shotIndex + 1) % 6;
            if (shotIndex == 0)
            {
                typeIndex = (typeIndex + 1) % 2;
            }

            SoundEngine.PlaySound(SoundID.Item11, position);
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return shotIndex == 0;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 4);
        }
    }
}
