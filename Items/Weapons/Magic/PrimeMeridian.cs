using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.ComponentModel;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Magic;
using TheTesseractMod.Projectiles.Summoner;

namespace TheTesseractMod.Items.Weapons.Magic
{
    public class PrimeMeridian : ModItem
    {

        public override void SetDefaults()
        {

            Item.damage = 235;
            Item.DamageType = DamageClass.Magic;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = 1500000;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item84;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PrimeMeridianProjectile>(); //409 is typhoon
            Item.shootSpeed = 20;
            Item.mana = 16;
            Item.noMelee = true;

        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            Recipe recipe2 = CreateRecipe();
            recipe.AddIngredient(ItemID.WaterBolt, 1);
            recipe.AddIngredient(ItemID.BookofSkulls, 1);
            recipe.AddIngredient(ItemID.CrystalStorm, 1);
            recipe.AddIngredient(ItemID.CursedFlames, 1);
            recipe.AddIngredient(ItemID.RazorbladeTyphoon, 1);
            recipe.AddIngredient(ItemID.MagnetSphere, 1);
            recipe.AddIngredient(ItemID.NebulaArcanum, 1);
            recipe.AddIngredient(ItemID.LunarFlareBook, 1);
            recipe.AddTile(TileID.LunarCraftingStation);

            recipe2.AddIngredient(ItemID.WaterBolt, 1);
            recipe2.AddIngredient(ItemID.BookofSkulls, 1);
            recipe2.AddIngredient(ItemID.CrystalStorm, 1);
            recipe2.AddIngredient(ItemID.GoldenShower, 1);
            recipe2.AddIngredient(ItemID.RazorbladeTyphoon, 1);
            recipe2.AddIngredient(ItemID.MagnetSphere, 1);
            recipe2.AddIngredient(ItemID.NebulaArcanum, 1);
            recipe2.AddIngredient(ItemID.LunarFlareBook, 1);
            recipe2.AddTile(TileID.LunarCraftingStation);

            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind("AuricBar", out ModItem AuricBar))
            {
                recipe.AddIngredient(AuricBar.Type, 5);
                recipe2.AddIngredient(AuricBar.Type, 5);
            }

            recipe.Register();
            recipe2.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            //for changing damage if the calamity mod is loaded
            int flameDamage = 250;
            int magnetsphereDamage = 235;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                flameDamage = (int)(flameDamage * 1.5f);
                magnetsphereDamage *= 2;
            }

            Vector2 shot = velocity;
            //shoots flames
            for (int i = -20; i <= 20; i += 5)
            {
                shot = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(i * 1f));
                Projectile.NewProjectile(source, position, shot * 0.5f, ModContent.ProjectileType<PrimeMeridianFlameProjectile>(), flameDamage, knockback, player.whoAmI);
            }

            int vanillaProj = GenerateProjectile();
            
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity, vanillaProj, magnetsphereDamage, knockback, player.whoAmI);

            shot = velocity.RotatedBy(MathHelper.ToRadians(-14));
            Projectile.NewProjectile(source, position, shot, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot, vanillaProj, magnetsphereDamage, knockback, player.whoAmI);

            shot = velocity.RotatedBy(MathHelper.ToRadians(14));
            Projectile.NewProjectile(source, position, shot, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot, vanillaProj, magnetsphereDamage, knockback, player.whoAmI);

            return false;
        }


        public int GenerateProjectile()
        {
            int returnValue = 254; //defaults to magnet sphere
            Random rand = new Random();
            int random = rand.Next(8);
            switch (random)
            {
                case 0:
                    returnValue = ProjectileID.BookOfSkullsSkull;
                    break;
                case 1:
                    returnValue = ProjectileID.WaterBolt;
                    break;
                case 2:
                    returnValue = ProjectileID.CrystalStorm;
                    break;
                case 3:
                    returnValue = ProjectileID.GoldenShowerFriendly;
                    break;
                case 4:
                    returnValue = ProjectileID.CursedFlameFriendly;
                    break;
                case 5:
                    returnValue = ProjectileID.MagnetSphereBall;
                    break;
                case 6:
                    returnValue = ProjectileID.NebulaArcanum;
                    break;
                case 7:
                    returnValue = ProjectileID.LunarFlare;
                    break;
                default:
                    returnValue = ProjectileID.MagnetSphereBall;
                    break;
            }

            return returnValue;
        }

    }
}
