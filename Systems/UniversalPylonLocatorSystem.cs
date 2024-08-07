using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using TheTesseractMod.Items.Tesseracts;

namespace TheTesseractMod.Systems
{
    internal class UniversalPylonLocatorSystem : ModSystem
    {
        //FOR UNIVERSAL PYLON LOCATOR
        public override void Load()
        {
            On_TeleportPylonsSystem.IsPlayerNearAPylon += On_TeleportPylonsSystemOnIsPlayerNearAPylon;
            On_Player.InInteractionRange += On_PlayerOnInInteractionRange;
        }

        private bool On_PlayerOnInInteractionRange(On_Player.orig_InInteractionRange orig, Player self, int interactX, int interactY, TileReachCheckSettings settings)
        {
            bool ret = orig(self, interactX, interactY, settings);

            Tile tile = Framing.GetTileSafely(interactX, interactY);

            return (TileID.Sets.CountsAsPylon.Contains(tile.TileType) &&
                (self.HasItem(ModContent.ItemType<Tesseract9>()) || self.HasItem(ModContent.ItemType<Tesseract10>())))

                || ret;
        }

        private bool On_TeleportPylonsSystemOnIsPlayerNearAPylon(On_TeleportPylonsSystem.orig_IsPlayerNearAPylon orig, Player player) =>
            player.HasItem(ModContent.ItemType<Tesseract9>()) || player.HasItem(ModContent.ItemType<Tesseract10>()) || orig(player);

    }
}
