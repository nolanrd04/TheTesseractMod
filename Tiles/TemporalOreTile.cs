using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TheTesseractMod.Tiles
{
	public class TemporalOreTile : ModTile
	{
		public override void SetStaticDefaults() {
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be highlighted by the Spelunker buff
			Main.tileOreFinderPriority[Type] = 450; // Metal Detector priority
			Main.tileShine2[Type] = true; // Modifies the brighting of the tile depending on the action
			Main.tileShine[Type] = 975; // How often tiny particles spawn
			Main.tileMergeDirt[Type] = true;
			Main.tileMerge[Type][TileID.Stone] = true;
			Main.tileMerge[TileID.Stone][Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(100, 100, 220), name); // Purple-ish color for temporal theme

			DustType = DustID.BlueTorch;
			HitSound = SoundID.Item50;

			MineResist = 1.5f; // Takes longer to mine
			MinPick = 55;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			r = 0.1f;
			g = 0.1f;
			b = 0.5f;
		}

	}
}
