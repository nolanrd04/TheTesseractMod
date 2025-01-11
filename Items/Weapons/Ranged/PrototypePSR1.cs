using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Ranged;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    internal class PrototypePSR1 : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item157;
            Item.autoReuse = true;
            Item.shootSpeed = 2f;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<PrototypeProjectile>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;


            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);

        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-23, 0);
        }
    }
}
