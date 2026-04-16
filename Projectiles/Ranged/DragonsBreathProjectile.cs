using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheTesseractMod.Projectiles.Ranged
{
    public class DragonsBreathProjectile : ModProjectile
    {
        public override string Texture => "TheTesseractMod/Textures/empty";
        public override void SetDefaults() {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 3;
        }
    }
}