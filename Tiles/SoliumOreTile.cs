using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TheTesseractMod.Tiles
{
	public class SoliumOreTile : ModTile
	{
		public override void SetStaticDefaults() {
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be highlighted by the Spelunker buff
			Main.tileOreFinderPriority[Type] = 850; // Metal Detector priority
			Main.tileShine2[Type] = true; // Modifies the brighting of the tile depending on the action
			Main.tileShine[Type] = 975; // How often tiny particles spawn
			Main.tileMergeDirt[Type] = true;
			Main.tileMerge[Type][TileID.Stone] = true;
			Main.tileMerge[TileID.Stone][Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(255, 174, 36), name); // orange-yellow color for solium theme

			DustType = DustID.Torch;
			HitSound = SoundID.Item50;

			MineResist = 3f; // Takes longer to mine
			MinPick = 225;
		}

		public override void RandomUpdate(int i, int j) {
			if (Main.rand.NextBool(60)) { // 1 in 60 chance each random tick
				Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Torch);
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			r = (255f / 255f) * 0.5f;
			g = (174f / 255f) * 0.5f;
			b = (36f / 255f) * 0.5f;
		}

	}
}
