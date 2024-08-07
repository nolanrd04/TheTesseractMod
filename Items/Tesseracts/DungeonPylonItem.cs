using TheTesseractMod.Items.Tesseracts;
using Terraria.Enums;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using TheTesseractMod.Items.Materials;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent;
using TheTesseractMod.Items.TileEntities;

namespace TheTesseractMod.Items.Tesseracts;

// Code mainly taken from example mod.
public class DungeonPylonItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<DungeonPylon>());
        Item.SetShopValues(ItemRarityColor.Blue1, Terraria.Item.buyPrice(gold: 10));
        Item.rare = -1;
        Item.value = 5000;
    }

    private static readonly Color[] itemNameCycleColors = {
            new Color(153, 214, 255),
            new Color (3, 152, 252),
            new Color (5, 58, 250)
        };
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        int numColors = itemNameCycleColors.Length;

        foreach (TooltipLine line2 in tooltips)
        {
            if (line2.Mod == "Terraria" && line2.Name == "ItemName")
            {
                float fade = (Main.GameUpdateCount % 60) / 60f;
                int index = (int)((Main.GameUpdateCount / 60) % numColors);
                int nextIndex = (index + 1) % numColors;

                line2.OverrideColor = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
            }
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ModContent.ItemType<AtomOfTime>(), 5);
        recipe.AddIngredient(ItemID.Bone, 10);
        recipe.Register();
    }
}
/* FOR UNIVERSAL PYLON LOCATOR
public class TesseractSystem : ModSystem
{
    public override void Load()
    {
        On_TeleportPylonsSystem.IsPlayerNearAPylon += On_TeleportPylonsSystemOnIsPlayerNearAPylon;
        On_Player.InInteractionRange += On_PlayerOnInInteractionRange;
    }

    private bool On_PlayerOnInInteractionRange(On_Player.orig_InInteractionRange orig, Player self, int interactX, int interactY, TileReachCheckSettings settings)
    {
        bool ret = orig(self, interactX, interactY, settings);

        Tile tile = Framing.GetTileSafely(interactX, interactY);

        return (TileID.Sets.CountsAsPylon.Contains(tile.TileType) && (tile.TileType == ModContent.TileType<TesseractPylon>()) &&
            (self.HasItem(ModContent.ItemType<Tesseract3>()) || self.HasItem(ModContent.ItemType<Tesseract4>())))

            || ret;
    }

    private bool On_TeleportPylonsSystemOnIsPlayerNearAPylon(On_TeleportPylonsSystem.orig_IsPlayerNearAPylon orig, Player player) =>
        player.HasItem(ModContent.ItemType<Tesseract3>()) || player.HasItem(ModContent.ItemType<Tesseract4>()) || orig(player);
}*/