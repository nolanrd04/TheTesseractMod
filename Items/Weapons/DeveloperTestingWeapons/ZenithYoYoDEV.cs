using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Developer;

namespace TheTesseractMod.Items.Weapons.DeveloperTestingWeapons
{
    internal class ZenithYoYoDEV : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 21;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }
        private static readonly int[] unwantedPrefixes = new int[] { PrefixID.Terrible, PrefixID.Dull, PrefixID.Shameful, PrefixID.Annoying, PrefixID.Broken, PrefixID.Damaged, PrefixID.Shoddy };

        public override void SetDefaults()
        {

            Item.damage = 140;
            Item.DamageType = DamageClass.Melee;
            Item.value = 1500000;
            Item.rare = ItemRarityID.Red;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.channel = true;
            Item.shootSpeed = 20f;
            Item.crit = 30;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<ZenithYoYoProjectileDEV>();

        }
        public override bool AllowPrefix(int pre)
        {

            if (Array.IndexOf(unwantedPrefixes, pre) > -1)
            {
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            Recipe recipe2 = CreateRecipe();
            recipe.AddIngredient(ItemID.WoodYoyo, 1);
            recipe.AddIngredient(ItemID.Rally, 1);
            recipe.AddIngredient(ItemID.CorruptYoyo, 1);
            recipe.AddIngredient(ItemID.JungleYoyo, 1);
            recipe.AddIngredient(ItemID.HiveFive, 1);
            recipe.AddIngredient(ItemID.Cascade, 1);
            recipe.AddIngredient(ItemID.Valor, 1);
            recipe.AddIngredient(ItemID.Chik, 1);
            recipe.AddIngredient(ItemID.HelFire, 1);
            recipe.AddIngredient(ItemID.Amarok, 1);
            recipe.AddIngredient(ItemID.Yelets, 1);
            recipe.AddIngredient(ItemID.Kraken, 1);
            recipe.AddIngredient(3292, 1);
            recipe.AddIngredient(ItemID.Terrarian, 1);
            recipe.AddTile(TileID.LunarCraftingStation);

            recipe2.AddIngredient(ItemID.WoodYoyo, 1);
            recipe2.AddIngredient(ItemID.Rally, 1);
            recipe2.AddIngredient(ItemID.CrimsonYoyo, 1);
            recipe2.AddIngredient(ItemID.JungleYoyo, 1);
            recipe2.AddIngredient(ItemID.HiveFive, 1);
            recipe2.AddIngredient(ItemID.Cascade, 1);
            recipe2.AddIngredient(ItemID.Valor, 1);
            recipe2.AddIngredient(ItemID.Chik, 1);
            recipe2.AddIngredient(ItemID.HelFire, 1);
            recipe2.AddIngredient(ItemID.Amarok, 1);
            recipe2.AddIngredient(ItemID.Yelets, 1);
            recipe2.AddIngredient(ItemID.Kraken, 1);
            recipe2.AddIngredient(3292, 1);
            recipe2.AddIngredient(ItemID.Terrarian, 1);
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
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {

                if (line.Mod == "Terraria" && line.Text.Contains("RED:"))
                {
                    line.Text = line.Text.Replace("RED:", $"[c/FF4A4A:RED:]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("ORANGE:"))
                {
                    line.Text = line.Text.Replace("ORANGE:", $"[c/FFA44A:ORANGE:]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("YELLOW:"))
                {
                    line.Text = line.Text.Replace("YELLOW:", $"[c/FFED4A:YELLOW:]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("GREEN:"))
                {
                    line.Text = line.Text.Replace("GREEN", $"[c/48C936:GREEN:]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("BLUE:"))
                {
                    line.Text = line.Text.Replace("BLUE:", $"[c/4775FF:BLUE:]");
                }
                if (line.Mod == "Terraria" && line.Text.Contains("PURPLE:"))
                {
                    line.Text = line.Text.Replace("PURPLE:", $"[c/9430FF:PURPLE:]");
                }
                if (line.Mod == "Terraria" && line.Text.Contains("PINK:"))
                {
                    line.Text = line.Text.Replace("PINK:", $"[c/FC72E8:PINK:]");
                }
            }
        }

    }
}
