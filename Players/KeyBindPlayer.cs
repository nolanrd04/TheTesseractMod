using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Items.Tesseracts;
using TheTesseractMod.Systems;
using TheTesseractMod.Global.Items;

namespace TheTesseractMod.Players
{
    public class KeyBindPlayer : ModPlayer
    {
        public static int teleportCount = 0;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            Player player = Main.LocalPlayer;
            int tesseract6 = ModContent.ItemType<Tesseract6>();
            int tesseract7 = ModContent.ItemType<Tesseract7>();
            
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];

                if (item.type == tesseract6 || item.type == tesseract7)
                {
                    if (!Player.HasBuff(BuffID.ChaosState) && KeyBindSystems.keyBind.JustPressed)
                    {
                        Player.Teleport(Main.MouseWorld, TeleportationStyleID.RodOfDiscord);
                        Player.AddBuff(BuffID.ChaosState, 600);
                    }
                }
            }

            int tesseract8 = ModContent.ItemType<Tesseract8>();
            int tesseract9 = ModContent.ItemType<Tesseract9>();
            int tesseract10 = ModContent.ItemType<Tesseract10>();
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];

                if (item.type == tesseract8 || item.type == tesseract9 || item.type == tesseract10)
                {
                    if (!Player.HasBuff(BuffID.ChaosState) && KeyBindSystems.keyBind.JustPressed && teleportCount < 3)
                    {
                        Player.Teleport(Main.MouseWorld, TeleportationStyleID.RodOfDiscord);
                        teleportCount++;
                        Player.AddBuff(ModContent.BuffType<StardustRelocatorBuff>(), 60);
                    }
                }
            }

        }
    }
}
