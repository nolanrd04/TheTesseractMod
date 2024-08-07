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
using TheTesseractMod.Projectiles.Ranged;
using TheTesseractMod.Global.Projectiles.Ranged;
using TheTesseractMod.Buffs;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    public class ApexN31 : ModItem // If you are reading this, I was not aware we should use Main.rand for randoms. Therefore a lot of the code I wrote creates new instances of randoms.
    {
        int[] arrayBulletID = { 14, 36, 89, 104, 207, 242, 279, 283, 284, 285, 286, 287, 638, 981 }; //14 idx 13
        public override void SetDefaults()
        {

            Item.damage = 130;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Item.damage += (int)(Item.damage * 0.15f);
            }
            Item.useAmmo = AmmoID.Bullet;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 49;
            Item.height = 23;
            Item.scale = 0.85f;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 1500000;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ApexN31Bullet>();
            Item.shootSpeed = 40;
            Item.noMelee = true;
            Item.reuseDelay = 0;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            Recipe recipe2 = CreateRecipe();

            recipe.AddIngredient(ItemID.Musket, 1);
            recipe.AddIngredient(ItemID.Boomstick, 1);
            recipe.AddIngredient(ItemID.Minishark, 1);
            recipe.AddIngredient(ItemID.PhoenixBlaster, 1);
            recipe.AddIngredient(ItemID.ClockworkAssaultRifle, 1);
            recipe.AddIngredient(ItemID.Megashark, 1);
            recipe.AddIngredient(ItemID.Uzi, 1);
            recipe.AddIngredient(ItemID.VenusMagnum, 1);
            recipe.AddIngredient(ItemID.CandyCornRifle, 1);
            recipe.AddIngredient(ItemID.ChainGun, 1);
            recipe.AddIngredient(ItemID.VortexBeater, 1);
            recipe.AddIngredient(ItemID.SDMG, 1);
            recipe.AddTile(TileID.LunarCraftingStation);

            recipe2.AddIngredient(ItemID.TheUndertaker, 1);
            recipe2.AddIngredient(ItemID.Boomstick, 1);
            recipe2.AddIngredient(ItemID.Minishark, 1);
            recipe2.AddIngredient(ItemID.PhoenixBlaster, 1);
            recipe2.AddIngredient(ItemID.ClockworkAssaultRifle, 1);
            recipe2.AddIngredient(ItemID.Megashark, 1);
            recipe2.AddIngredient(ItemID.Uzi, 1);
            recipe2.AddIngredient(ItemID.VenusMagnum, 1);
            recipe2.AddIngredient(ItemID.CandyCornRifle, 1);
            recipe2.AddIngredient(ItemID.ChainGun, 1);
            recipe2.AddIngredient(ItemID.VortexBeater, 1);
            recipe2.AddIngredient(ItemID.SDMG, 1);
            recipe2.AddTile(TileID.LunarCraftingStation);

            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind("AuricBar", out ModItem AuricBar))
            {
                recipe.AddIngredient(AuricBar.Type, 5);
                recipe2.AddIngredient(AuricBar.Type, 5);
            }


            recipe.Register();
            recipe2.Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<ApexN31Bullet>();
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;
            

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                //position += muzzleOffset;
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.HasBuff(ModContent.BuffType<ApexOverclockBuff>()))
            {
                damage *= 2;
            }
            
            //*****RIGHT CLICK CODE*****//
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<ApexN31Rocket>();
                SoundEngine.PlaySound(SoundID.Item61, player.position);
                Projectile.NewProjectile(source, position, velocity * 0.2f, type, damage * 4, knockback, player.whoAmI);
                EditPlayer.ApexN31BulletHits = 0;
                EditPlayer.ApexN31LoadedRocketCount = 0;
                EditPlayer.maxRocketIdx = 0;
                return false;
            }
            //*************************//

            //shoots random vanilla bullet in the center
            var random = new Random();
            var randomBullet = random.Next(0, 14);
            Projectile.NewProjectile(source, position, velocity, arrayBulletID[randomBullet], damage, knockback, player.whoAmI);


            //shoots the double beam of apex bullets
            Vector2 bulletRotate = velocity;
            bulletRotate = bulletRotate.RotatedBy(MathHelper.ToRadians(1));
            Projectile.NewProjectile(source, position, bulletRotate, type, damage, knockback, player.whoAmI);
            bulletRotate = bulletRotate.RotatedBy(MathHelper.ToRadians(-2));
            Projectile.NewProjectile(source, position, bulletRotate, type, damage, knockback, player.whoAmI);

            //randomly shoots rockets
            type = ModContent.ProjectileType<ApexN31Rocket>();
            int val = random.Next(1, 7);
            int rotation = random.Next(-20, 20);
            Vector2 shot = new Vector2(velocity.X * 0.2f, velocity.Y * 0.2f).RotatedBy(MathHelper.ToRadians(rotation * 1f));
            if (val == 1)
            {
                Projectile.NewProjectile(source, position, shot, type, damage * 3, knockback, player.whoAmI);
                SoundEngine.PlaySound(SoundID.Item61, player.position);
            }
            return false;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.66f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20f, 0f);
        }
        public override bool AltFunctionUse(Player player)
        {
            if (EditPlayer.ApexN31LoadedRocketCount > 0)
            {
                return true;
            }
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                //set stats
                Item.shoot = ModContent.ProjectileType<ApexN31Rocket>();
                Item.useTime = 5;
                Item.useAnimation = EditPlayer.ApexN31LoadedRocketCount *5;
                Item.UseSound = SoundID.Item61;

                // overlock
                if (EditPlayer.ApexN31LoadedRocketCount > 0)
                {
                    float timeToAdd = 240 * (EditPlayer.ApexN31LoadedRocketCount / 10f);
                    player.AddBuff(ModContent.BuffType<ApexOverclockBuff>(), (int)timeToAdd);
                }
            }
            else
            {
                SetDefaults();
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //tooltips.Add(new TooltipLine(Mod, "Hits", $"Enemies Hit: {ApexN31GlobalProjectile.ApexN31BulletHits}"));
            tooltips.Add(new TooltipLine(Mod, "Hits", $"[c/07f246:Loaded Rockets:] {EditPlayer.ApexN31LoadedRocketCount}"));

            foreach (TooltipLine line in tooltips)
            {

                if (line.Mod == "Terraria" && line.Text.Contains("zenith energy bolts"))
                {
                    line.Text = line.Text.Replace("zenith energy bolts", $"[c/F0F4C3:zenith energy bolts]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("RIGHT-CLICK ATTACK:"))
                {
                    line.Text = line.Text.Replace("RIGHT-CLICK ATTACK:", $"[c/FF0000:RIGHT-CLICK ATTACK:]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("1 rocket loaded"))
                {
                    line.Text = line.Text.Replace("1 rocket loaded:", $"[c/0000FF:1 rocket loaded]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("mana recharge sfx"))
                {
                    line.Text = line.Text.Replace("mana recharge sfx:", $"[c/0000FF:mana recharge sfx]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("Max rockets"))
                {
                    line.Text = line.Text.Replace("Max rockets", $"[c/CE93D8:Max rockets]");
                }
                if (line.Mod == "Terraria" && line.Text.Contains("mana crystal consume sfx"))
                {
                    line.Text = line.Text.Replace("mana crystal consume sfx", $"[c/CE93D8:mana crystal consume sfx]");
                }
            }
        }
    }
    public class EditPlayer : ModPlayer // stores the values for bullet collisions
    {
        public static int ApexN31BulletHits = 0;
        public static int ApexN31LoadedRocketCount = 0;
        public static int maxRocketIdx;
    }
}