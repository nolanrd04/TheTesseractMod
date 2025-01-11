using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueExcaliburWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons;
using TheTesseractMod.Projectiles.TerraWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TerraWeapons
{
    internal class TerraThrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 20;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }
        private static readonly int[] unwantedPrefixes = new int[] { PrefixID.Terrible, PrefixID.Dull, PrefixID.Shameful, PrefixID.Annoying, PrefixID.Broken, PrefixID.Damaged, PrefixID.Shoddy };

        public override void SetDefaults()
        {

            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 4.6f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<TerraYoyoProj>();

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
            recipe.AddIngredient(ModContent.ItemType<TrueNightsSling>());
            recipe.AddIngredient(ModContent.ItemType<TrueYeletes>());
            recipe.AddIngredient(ItemID.TheEyeOfCthulhu);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
