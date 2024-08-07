using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Melee;
using TheTesseractMod.Projectiles.NoSpecificClass;

namespace TheTesseractMod.Items.Weapons.NoSpecificClass
{
    internal class ThePeppermint : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Generic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8;
            Item.value = 50000000;
            Item.rare = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PeppermintProjectile>();
            Item.noMelee = true;
            Item.shootSpeed = 20;
            Item.noUseGraphic = true;
            Item.crit = 10;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Text == "Mint Gaming's Official Weapon!")
                {
                    line2.OverrideColor = Color.Red;
                }
            }
        }
    }
}
