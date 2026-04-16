using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Buffs
{
    public class WhipOfTheWestTagBuff : ModBuff
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        public static readonly int TagDamage = 22;

		public override void SetStaticDefaults() {
			BuffID.Sets.IsATagBuff[Type] = true;
		}
    }

    public class WhipOfTheWestDebuffManagement : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
			// Only player attacks should benefit from this buff, hence the NPC and trap checks.
			if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
				return;


			// SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
			var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
			if (npc.HasBuff<WhipOfTheWestTagBuff>()) {
				// Apply a flat bonus to every hit
				modifiers.FlatBonusDamage += WhipOfTheWestTagBuff.TagDamage * projTagMultiplier;
			}
		}
	}
}