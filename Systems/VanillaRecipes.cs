using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Build.Tasks;
using System.Text.RegularExpressions;

namespace TheTesseractMod.Systems
{
    internal class VanillaRecipes : ModSystem // adds recipes for hard to find vanilla items
    {
        public override void AddRecipes()
        {
            Recipe terragrim = Recipe.Create(ItemID.Terragrim);
            terragrim.AddIngredient(ItemID.Emerald, 15);
            terragrim.AddRecipeGroup("goldsword");
            terragrim.AddIngredient(ItemID.DemoniteBar, 10);
            terragrim.AddTile(TileID.Anvils);
            terragrim.Register();

            Recipe rally = Recipe.Create(ItemID.Rally);
            rally.AddIngredient(ItemID.WoodYoyo);
            rally.AddIngredient(ItemID.StoneBlock, 150);
            rally.AddIngredient(ItemID.GoldChest);
            rally.AddTile(TileID.Anvils);
            rally.Register();

            Recipe cascade = Recipe.Create(ItemID.Cascade);
            cascade.AddIngredient(ItemID.WoodYoyo);
            cascade.AddIngredient(ItemID.Hellstone, 15);
            cascade.AddIngredient(ItemID.Bone, 20);
            cascade.AddTile(TileID.Anvils);
            cascade.Register();

            Recipe uzi = Recipe.Create(ItemID.Uzi);
            uzi.AddIngredient(ItemID.AngryTrapperBanner);
            uzi.AddIngredient(ItemID.ChlorophyteBar, 20);
            uzi.AddIngredient(ItemID.Megashark);
            uzi.AddTile(TileID.MythrilAnvil);
            uzi.Register();

            Recipe enchantedsword = Recipe.Create(ItemID.EnchantedSword);
            enchantedsword.AddRecipeGroup("goldsword");
            enchantedsword.AddIngredient(ItemID.FallenStar, 30);
            enchantedsword.AddIngredient(ItemID.DemoniteBar, 10);
            enchantedsword.AddTile(TileID.Bookcases);
            enchantedsword.Register();

            Recipe enchantedboomerang = Recipe.Create(ItemID.EnchantedBoomerang);
            enchantedboomerang.AddIngredient(ItemID.WoodenBoomerang);
            enchantedboomerang.AddIngredient(ItemID.FallenStar, 30);
            enchantedboomerang.AddIngredient(ItemID.DemoniteBar, 10);
            enchantedboomerang.AddTile(TileID.Bookcases);
            enchantedboomerang.Register();

            Recipe luckyhorseshoe = Recipe.Create(ItemID.LuckyHorseshoe);
            luckyhorseshoe.AddIngredient(ItemID.GoldBar, 10);
            luckyhorseshoe.AddIngredient(ItemID.Feather, 5);
            luckyhorseshoe.AddIngredient(ItemID.SunplateBlock, 20);
            luckyhorseshoe.AddTile(TileID.Anvils);
            luckyhorseshoe.Register();

            Recipe redballoon = Recipe.Create(ItemID.ShinyRedBalloon);
            redballoon.AddIngredient(ItemID.Silk, 30);
            redballoon.AddIngredient(ItemID.Feather, 5);
            redballoon.AddIngredient(ItemID.Cloud, 30);
            redballoon.AddTile(TileID.Anvils);
            redballoon.Register();

            Recipe smallwings = Recipe.Create(4978);
            smallwings.AddIngredient(ItemID.Feather, 30);
            smallwings.AddIngredient(ItemID.ShinyRedBalloon);
            smallwings.AddIngredient(ItemID.LuckyHorseshoe);
            smallwings.AddIngredient(ItemID.Cloud, 30);
            smallwings.AddTile(TileID.Hellforge);
            smallwings.Register();


            Recipe naturesgift = Recipe.Create(ItemID.NaturesGift);
            naturesgift.AddIngredient(ItemID.ManaCrystal, 5);
            naturesgift.AddIngredient(ItemID.JungleGrassSeeds);
            naturesgift.AddIngredient(ItemID.JungleSpores, 10);
            naturesgift.AddTile(TileID.Bottles);
            naturesgift.Register();
        }
    }
}
