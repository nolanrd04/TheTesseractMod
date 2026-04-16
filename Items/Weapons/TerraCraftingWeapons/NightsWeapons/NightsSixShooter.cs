using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.DungeonWeapons;
using TheTesseractMod.Projectiles.Ranged;
using TheTesseractMod.Projectiles.NightsWeapons;
using TheTesseractMod.Projectiles.TerraWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons
{
    internal class NightsSixShooter : ModItem
    {
        private int shotIndex = 0;
        public override void SetDefaults()
        {
            Item.damage = 31;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.useAnimation = 60;
            Item.crit = 10;
            Item.useTime = 10;
            Item.reuseDelay = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<NightsBullet>();
            Item.shootSpeed = 15;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Boomstick);
            if (!WorldGen.crimson)
            {
                recipe.AddIngredient(ItemID.Musket);
            }
            else
            {
                recipe.AddIngredient(ItemID.TheUndertaker);
            }
            recipe.AddIngredient(ItemID.PhoenixBlaster);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.Next(6) == 0)
            {
                Projectile.NewProjectile(source, position, velocity * 1.3f, ModContent.ProjectileType<SuperNightsBullet>(), damage, knockback);
            }
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<NightsBullet>(), damage, knockback);
            SoundEngine.PlaySound(SoundID.Item11, position);

            Vector2 vel; 
            vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15)) / 2;
            Dust.NewDust(position, 1, 1, 27, vel.X, vel.Y, 0, default(Color), 1f);

            shotIndex = (shotIndex + 1) % 6;
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 40f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);

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
