using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Ranged.EtherealBlaster;
using TheTesseractMod.Projectiles.Summoner.ShadowFlameDragon;

namespace TheTesseractMod.Items.Weapons.DeveloperTestingWeapons
{
    //This weapon is a dev weapon used to test the functionality of projectiles.
    internal class HomingProjectileTestingStaff : ModItem
    {
        public override void SetDefaults()
        {

            Item.staff[Item.type] = true;
            Item.damage = 100;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot; //1=swinging, 5 = shooting
            Item.knockBack = 9;
            Item.UseSound = SoundID.Item163;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShadowFlameDragonMinion>();
            Item.shootSpeed = 15;
            Item.noMelee = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Main.NewText(NPC.downedMoonlord);
            //Dust.NewDust(position, 15, 15, ModContent.DustType<StormCloud1>(), velocity.X * 0.5f, velocity.Y * 0.5f, 0, Color.DarkGray, 1f);
            return true;
        }
    }
}
