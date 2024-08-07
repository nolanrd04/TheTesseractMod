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
using TheTesseractMod.Projectiles.Ranged;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    public class Culmination : ModItem
    {
        public override void SetDefaults()
        {

            Item.damage = 315;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Item.damage = 285;
                Item.damage += (int)(Item.damage * 0.105f);
            }
            Item.useAmmo = AmmoID.Arrow;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 32;
            Item.scale = 0.85f;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = 5;
            Item.knockBack = 2;
            Item.value = 1500000;
            Item.rare = 10;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CulminationArrow>();
            Item.shootSpeed = 20;

            Item.noMelee = true;

        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            Recipe recipe2 = CreateRecipe();
            recipe.AddIngredient(ItemID.WoodenBow, 1);
            recipe.AddIngredient(ItemID.DemonBow, 1);
            recipe.AddIngredient(2888, 1); //bees knees
            recipe.AddIngredient(ItemID.MoltenFury, 1);
            recipe.AddIngredient(ItemID.HellwingBow, 1);
            recipe.AddIngredient(ItemID.ShadowFlameBow, 1);
            recipe.AddIngredient(ItemID.Marrow, 1);
            recipe.AddIngredient(ItemID.DaedalusStormbow, 1);
            recipe.AddIngredient(3854, 1); //phantom pheonix
            recipe.AddIngredient(4953, 1); //eventide
            recipe.AddIngredient(ItemID.Tsunami, 1);
            recipe.AddIngredient(ItemID.Phantasm, 1);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);

            recipe2.AddIngredient(ItemID.WoodenBow, 1);
            recipe2.AddIngredient(ItemID.TendonBow, 1);
            recipe2.AddIngredient(2888, 1); //bees knees
            recipe2.AddIngredient(ItemID.MoltenFury, 1);
            recipe2.AddIngredient(ItemID.HellwingBow, 1);
            recipe2.AddIngredient(ItemID.ShadowFlameBow, 1);
            recipe2.AddIngredient(ItemID.Marrow, 1);
            recipe2.AddIngredient(ItemID.DaedalusStormbow, 1);
            recipe2.AddIngredient(3854, 1); //phantom pheonix
            recipe2.AddIngredient(4953, 1); //eventide
            recipe2.AddIngredient(ItemID.Tsunami, 1);
            recipe2.AddIngredient(ItemID.Phantasm, 1);
            recipe2.AddIngredient(ItemID.LunarBar, 5);
            recipe2.AddTile(TileID.LunarCraftingStation);

            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind<ModItem>("AuricBar", out ModItem AuricBar))
            {

                recipe.AddIngredient(AuricBar.Type, 5);
                recipe2.AddIngredient(AuricBar.Type, 5);
            }

                recipe.Register();
                recipe2.Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<CulminationArrow>();
            //velocity = velocity * 2;Projectile.NewProjectile(source, position, velocity*4f, type, damage*2, knockback * 2, player.whoAmI);

            
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*PROJECTILES THAT COME STRAIGHT OUT*/
            var random = new Random();
            int randomArrowType = random.Next(0, 4);
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Projectile.NewProjectile(source, position, velocity * 0.8f, ModContent.ProjectileType<SuperCulminationArrow>(), damage * 4, knockback * 2, player.whoAmI);
            }
            else
            {
                if (randomArrowType == 1)
                {
                    Projectile.NewProjectile(source, position, velocity * 0.8f, ModContent.ProjectileType<SuperCulminationArrow>(), damage * 4, knockback * 2, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(source, position, velocity * 0.8f, type, damage * 2, knockback, player.whoAmI);
                }
            }
            
            Projectile.NewProjectile(source, position, velocity * 1.2f, 706, damage, knockback, player.whoAmI);
            /*************************************/


            /*SET UP PROJECTILE VELOCITY + POSITION*/
            position = Main.MouseWorld;
            velocity.Y = 20;
            velocity.X = 0;
            //velocity.X += position.X;
            random = new Random();
            var randomOffsetX = random.Next(-40, 40);
            SoundEngine.PlaySound(SoundID.Item20, player.position);
            position.X = Main.MouseWorld.X + randomOffsetX;
            position.Y = player.position.Y - 650;
            /***************************************/


            /*PROJECTILES THAT RAIN FROM SKY*/
            CulminationProjectileEdit.ingoreTiles = true;
            type = 9;
            Vector2 starShot = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(-10));
            Projectile.NewProjectile(source, position, starShot * 2f, type, damage, knockback, player.whoAmI);
            starShot = new Vector2(velocity.X, velocity.Y).RotatedBy(0);
            Projectile.NewProjectile(source, position, starShot * 2f, type, damage, knockback, player.whoAmI);
            starShot = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(10));
            Projectile.NewProjectile(source, position, starShot * 1.5f, type, damage, knockback, player.whoAmI);

            type = ModContent.ProjectileType<CulminationArrow>();
            for (int i = 0; i<4; i++)
            {
                random = new Random();
                int rotation = random.Next(-10, 10);
                Vector2 shot = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(rotation * 1f));
                Projectile.NewProjectile(source, position, shot * 0.8f, type, damage*2, knockback, player.whoAmI);
            }
            type = ModContent.ProjectileType<SuperCulminationArrow>();
            //Projectile.NewProjectile(source, position, velocity*0.8f, type, damage * 4, knockback * 2, player.whoAmI);

            CulminationProjectileEdit.ingoreTiles = false;
            /********************************/
            return false;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.50f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-40f, 4f);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Item.useTime > 10)
            {
                Item.useTime -= 2;
                Item.useAnimation -= 2;
            }
        }
    }
}