using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.EvilWeapons;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons
{
    internal class PurpleHeart : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ShadowFlameHexDoll);
            Item.damage = 10;
            Item.crit = 4;
            Item.DamageType = DamageClass.Magic;
            Item.width = 28;
            Item.height = 30;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(0, 0, 27, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shootSpeed = 6;
            Item.noMelee = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(40) - 20)) * Main.rand.NextFloat(0.7f, 1f);
        }
    }
}
