using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Enemy.BossProjectiles.GuardianOfTheRiftProjs
{
	public class DARK_DarkDeathRay : ModProjectile
	{
		// Rotation speed (radians per frame) - laser sweeps at constant speed
		private const float RotationSpeed = -0.035f;

		// Maximum beam length
		private const float MaxBeamLength = 550f;

		// Beam hitbox width for collision
		private const float BeamHitboxCollisionWidth = 22f;

		// Number of sample points for laser scan
		private const int NumSamplePoints = 3;

		// How quickly beam length adjusts
		private const float BeamLengthChangeFactor = 0.75f;

		// Store the current beam length
		private float BeamLength {
			get => Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
			Projectile.timeLeft = 180;
			Projectile.scale = 1.5f;
		}

        public override void SetStaticDefaults()
        {
           	ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4800;
        }
		public override void SendExtraAI(BinaryWriter writer) {
      		writer.Write(Projectile.rotation);
  		}

  		public override void ReceiveExtraAI(BinaryReader reader) {
      		Projectile.rotation = reader.ReadSingle();
  		}


		public override void AI() {
			// Get boss NPC from ai[0] (stored when projectile is created)
			int bossIndex = (int)Projectile.ai[0];
			if (bossIndex < 0 || bossIndex >= Main.maxNPCs || !Main.npc[bossIndex].active) {
				Projectile.Kill();
				return;
			}

			// Set initial rotation from ai[1] on first frame (synced with spawn)
			if (Projectile.ai[1] != 0) {
				Projectile.rotation = Projectile.ai[1];
				Projectile.ai[1] = 0; // Clear so it only applies once
			}

			NPC boss = Main.npc[bossIndex];

			// Keep beam anchored to boss center
			Projectile.Center = boss.Center;
			Projectile.velocity = Vector2.Zero;

			// Sweep laser in a constant counterclockwise direction
			// The laser sweeps independently and forces the player to move with it
			Projectile.rotation -= RotationSpeed;

			// Calculate beam length via hitscan
			float hitscanBeamLength = PerformBeamHitscan();
			BeamLength = MathHelper.Lerp(BeamLength, hitscanBeamLength, BeamLengthChangeFactor);
		}

		private float PerformBeamHitscan() {
			float[] laserScanResults = new float[NumSamplePoints];
			Collision.LaserScan(Projectile.Center, Projectile.rotation.ToRotationVector2(), 0, MaxBeamLength, laserScanResults);

			float averageLength = 0f;
			for (int i = 0; i < laserScanResults.Length; i++) {
				averageLength += laserScanResults[i];
			}
			averageLength /= NumSamplePoints;

			return averageLength;
		}

		// Collision detection for the beam
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			if (projHitbox.Intersects(targetHitbox)) {
				return true;
			}

			float _ = float.NaN;
			Vector2 beamDirection = Projectile.rotation.ToRotationVector2();
			Vector2 beamEndPos = Projectile.Center + beamDirection * BeamLength;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 beamDirection = Projectile.rotation.ToRotationVector2();
			Vector2 centerFloored = Projectile.Center.Floor() + beamDirection * Projectile.scale * 10.5f;
			Vector2 drawScale = new Vector2(Projectile.scale);

			float visualBeamLength = BeamLength - 14.5f * Projectile.scale * Projectile.scale;

			Vector2 startPosition = centerFloored - Main.screenPosition;
			Vector2 endPosition = startPosition + beamDirection * visualBeamLength;

			DrawBeam(Main.spriteBatch, texture, startPosition, endPosition, drawScale, Color.White);

			return false;
		}

		private void DrawBeam(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor) {
			var lineFraming = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);
			DelegateMethods.c_1 = beamColor;
			Utils.DrawLaser(spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
		}

		public override void CutTiles() {
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			var cut = new Utils.TileActionAttempt(DelegateMethods.CutTiles);
			Vector2 beamDirection = Projectile.rotation.ToRotationVector2();
			Vector2 beamStartPos = Projectile.Center;
			Vector2 beamEndPos = beamStartPos + beamDirection * BeamLength;
			Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
		}
	}
}