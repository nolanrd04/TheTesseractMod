using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Global.Projectiles.Magic;
using TheTesseractMod.Projectiles.Magic;

namespace TheTesseractMod.Items.Weapons.Magic
{
    public class ConjuringClimax : ModItem
    {
        //first weapon made. bear with the code...
        public override void SetDefaults()
        {

            Item.staff[Item.type] = true;
            Item.damage = 135;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Item.damage += (int)(Item.damage * 0.55f);
            }
            Item.DamageType = DamageClass.Magic;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot; //1=swinging, 5 = shooting
            Item.knockBack = 9;
            Item.value = 1500000;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item163;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ConjuringClimaxProjectile>();
            Item.shootSpeed = 7;
            Item.mana = 14;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.WandofSparking, 1);
            recipe.AddIngredient(ItemID.AmberStaff, 1);
            recipe.AddIngredient(ItemID.ThunderStaff, 1);
            recipe.AddIngredient(ItemID.MeteorStaff, 1);
            recipe.AddIngredient(ItemID.FrostStaff, 1);
            recipe.AddIngredient(ItemID.CrystalSerpent, 1);
            recipe.AddIngredient(ItemID.RainbowRod, 1);
            recipe.AddIngredient(ItemID.VenomStaff, 1);
            recipe.AddIngredient(ItemID.ShadowbeamStaff, 1);
            recipe.AddIngredient(ItemID.SpectreStaff, 1);
            recipe.AddIngredient(ItemID.InfernoFork, 1);
            recipe.AddIngredient(ItemID.Razorpine, 1);
            recipe.AddIngredient(ItemID.NebulaBlaze, 1);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);

            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind("AuricBar", out ModItem AuricBar))
            {
                recipe.AddIngredient(AuricBar.Type, 5);
            }
            recipe.Register();
        }



        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float multiplier = 1f;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                multiplier = 2.2f;
                ConjuringClimaxCalamityOverrider.shotByConjuringClimax = true;
            }
            Projectile.NewProjectile(source, position, velocity, 79, damage, knockback, player.whoAmI);

            Vector2 shot = new Vector2(velocity.X, velocity.Y);
            Projectile.NewProjectile(source, position, shot, 294, (int)(damage * multiplier), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot, type, damage, knockback, player.whoAmI);
            shot = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(-3));
            Projectile.NewProjectile(source, position, shot, 294, (int)(damage * multiplier), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot * 5, 521, (int)(damage * multiplier), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot, type, damage, knockback, player.whoAmI);
            shot = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(3)); ;
            Projectile.NewProjectile(source, position, shot, 294, (int)(damage * multiplier), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot * 5, 521, (int)(damage * multiplier), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot, type, damage, knockback, player.whoAmI);
            shot = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(7));
            Projectile.NewProjectile(source, position, shot, 294, (int)(damage * multiplier), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot, type, damage, knockback, player.whoAmI);
            shot = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(-7));
            Projectile.NewProjectile(source, position, shot, 294, (int)(damage * multiplier), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, shot, type, damage, knockback, player.whoAmI);
            ConjuringClimaxCalamityOverrider.shotByConjuringClimax = false;
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {

                if (line.Mod == "Terraria" && line.Text.Contains("Zenith Missles"))
                {
                    line.Text = line.Text.Replace("Zenith Missles", $"[c/F0F4C3:Zenith Missles]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("Crystal Breath"))
                {
                    line.Text = line.Text.Replace("Crystal Breath", $"[c/CE93D8:Crystal Breath]");
                }
                if (line.Mod == "Terraria" && line.Text.Contains("Shadow Beams"))
                {
                    line.Text = line.Text.Replace("Shadow Beams", $"[c/CE93D8:Shadow Beams]");
                }
            }
        }

    }
}