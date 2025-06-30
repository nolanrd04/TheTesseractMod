using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.TrueNightsWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons
{
    internal class TrueNightsSling : ModItem
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

            Item.damage = 55;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 4.75f;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<TrueNightsSlingProj>();

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
            recipe.AddIngredient(ModContent.ItemType<NightsSling>());
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }


    }
}
