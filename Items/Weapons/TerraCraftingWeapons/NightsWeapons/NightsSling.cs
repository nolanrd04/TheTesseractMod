using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Melee;
using TheTesseractMod.Projectiles.NightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons
{
    internal class NightsSling : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 17;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }
        private static readonly int[] unwantedPrefixes = new int[] { PrefixID.Terrible, PrefixID.Dull, PrefixID.Shameful, PrefixID.Annoying, PrefixID.Broken, PrefixID.Damaged, PrefixID.Shoddy };

        public override void SetDefaults()
        {

            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<NightsSlingProj>();

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
            recipe.AddIngredient(ItemID.JungleYoyo);
            if (!WorldGen.crimson)
            {
                recipe.AddIngredient(ItemID.CorruptYoyo);
            }
            else
            {
                recipe.AddIngredient(ItemID.CrimsonYoyo);
            }
            recipe.AddIngredient(ItemID.Valor);
            recipe.AddIngredient(ItemID.Cascade);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
