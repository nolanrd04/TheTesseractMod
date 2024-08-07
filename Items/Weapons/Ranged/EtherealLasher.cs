using Microsoft.Xna.Framework;
using Mono.Cecil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Global.Projectiles.Ranged;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Projectiles.Magic.EtherealTomeProjectiles;
using TheTesseractMod.Projectiles.Melee.EtherealLanceProjectiles;
using TheTesseractMod.Projectiles.Ranged;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    public class EtherealLasher : ModItem
    {
        public int magicType = 0;
        public override void SetDefaults()
        {

            Item.damage = 150;
            Item.useAmmo = AmmoID.Arrow;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 42;
            Item.height = 96;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = 5;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20;
            Item.noMelee = true;
            Item.crit = 15;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(ModContent.ItemType<ColdRiftFragment>(), 20);
            recipe.AddIngredient(ModContent.ItemType<ChloroRiftFragment>(), 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (magicType % 2 == 0)
            {
                if (Main.rand.NextBool())
                {
                    Projectile.NewProjectile(source, position, velocity / 4.7f, ProjectileID.CrystalLeafShot, Item.damage, Item.knockBack);
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(5f)) / 4.7f, ProjectileID.CrystalLeafShot, Item.damage, Item.knockBack);
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-5f)) / 4.7f, ProjectileID.CrystalLeafShot, Item.damage, Item.knockBack);
                }
                else
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeadlyIcicleFriendly>(), Item.damage, Item.knockBack);
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(4f)), ModContent.ProjectileType<DeadlyIcicleFriendly>(), Item.damage, Item.knockBack);
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-4f)), ModContent.ProjectileType<DeadlyIcicleFriendly>(), Item.damage, Item.knockBack);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                float rotation = (Main.rand.NextFloat() * 8f - 4f);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(rotation)), type, Item.damage, Item.knockBack);
            }
            magicType++;
            return true;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.66f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-17f, 4f);
        }
    }
}