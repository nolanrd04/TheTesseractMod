using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Utils;
using ReLogic.Content;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;
using TheTesseractMod.Items.TileEntities;
using static TheTesseractMod.TheTesseractMod;

namespace TheTesseractMod.Items.Tesseracts
{
    internal class TesseractPylon : ModPylon
    {
        public const int CrystalVerticalFrameCount = 8;
        public Asset<Texture2D> crystalTexture;
        public Asset<Texture2D> mapIcon;
        public Asset<Texture2D> crystalHighlightTexture;

        public override void Load()
        {
            crystalTexture = ModContent.Request<Texture2D>(Texture + "_PylonCrystal");
            crystalHighlightTexture = ModContent.Request<Texture2D>(Texture + "_PylonCrystalHighlight");
            mapIcon = ModContent.Request<Texture2D>(Texture + "_PylonMapIcon");
        }
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawStyleOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;

            TEModdedPylon tesseractPylon = ModContent.GetInstance<TesseractPylonEntity>();
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(tesseractPylon.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(tesseractPylon.Hook_AfterPlacement, -1, 0, false);

            TileObjectData.addTile(Type);

            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            AddToArray(ref TileID.Sets.CountsAsPylon);

            LocalizedText pylonName = CreateMapEntryName();
            AddMapEntry(Color.White, pylonName);
        }


        public override NPCShop.Entry GetNPCShopEntry()
        {
            return null;
        }
        public override void MouseOver(int i, int j)
        {
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<TesseractPylonItem>();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TesseractPylonEntity>().Kill(i, j);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = g = 0.75f;
            b = 1f;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) 
        {
			DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -12f), Color.White * 0.1f, Color.White, 4, CrystalVerticalFrameCount);
		}

		public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale) 
        {
			bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
			DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<TesseractPylonItem>().DisplayName.Key, ref mouseOverText);
		}
        public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount)
        {
            if (TesseractCheck() || Main.netMode == NetmodeID.Server)
            {
                return true;
            }
            else
            {
                Main.NewText("Level 3+ Tesseract not detected.");
                return false;
            }
        }
        public override void ValidTeleportCheck_NearbyPostCheck(TeleportPylonInfo nearbyPylonInfo, ref bool destinationPylonValid, ref bool anyNearbyValidPylon, ref string errorKey)
        {
            if (TileEntity.ByPosition[nearbyPylonInfo.PositionInTiles] is TesseractPylonEntity)
            {
                anyNearbyValidPylon = TesseractCheck() || Main.netMode == NetmodeID.Server;
            }
            else
            {
                Main.NewText("Level 3+ Tesseract not detected.");
            }
        }

        public bool TesseractCheck()
        {
            Player player = Main.LocalPlayer;
            if (player.active && !player.dead)
            {
                int tesseract3 = ModContent.ItemType<Tesseract3>();
                int tesseract4 = ModContent.ItemType<Tesseract4>();
                int tesseract5 = ModContent.ItemType<Tesseract5>();
                int tesseract6 = ModContent.ItemType<Tesseract6>();
                int tesseract7 = ModContent.ItemType<Tesseract7>();
                int tesseract8 = ModContent.ItemType<Tesseract8>();
                int tesseract9 = ModContent.ItemType<Tesseract9>();
                int tesseract10 = ModContent.ItemType<Tesseract10>();
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    Item item = player.inventory[i];

                    if (item.type == tesseract3 || item.type == tesseract4 || item.type == tesseract5 || item.type == tesseract6 || item.type == tesseract7 || item.type == tesseract8 || item.type == tesseract9 || item.type == tesseract10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
