using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Projectiles.Ranged.EtherealBlaster;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    public class EtherealBlaster : ModItem 
    {
        public override void SetDefaults()
        {

            Item.damage = 190;
            Item.useAmmo = AmmoID.Bullet;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 17;
            Item.scale = 2f;
            Item.useTime = 7;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 40;
            Item.noMelee = true;
            Item.reuseDelay = 40;
            Item.scale = 1f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<GlowRiftFragment>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 45f;


            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item36, player.position);
            float numberProjectiles = Main.rand.Next(2) + 3;

            position += Vector2.Normalize(velocity) * 41f;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians((float)(Main.rand.Next(16) - 8))), type, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians((float)(Main.rand.Next(16) - 8))) / 6, ModContent.ProjectileType<GlowRiftProjectileFriendly>(), damage, knockback, player.whoAmI);
            }

            if (Main.rand.Next(5) == 0)
            {
                Vector2 speed = new Vector2(15f, 0f);
                Projectile.NewProjectile(source, position, speed.RotatedBy(velocity.ToRotation()), ModContent.ProjectileType<EtherealSkullProjectile>(), damage, knockback, player.whoAmI);
            }

            return true;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.45f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, -4f);
        }
    }
}