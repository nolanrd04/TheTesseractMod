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

namespace TheTesseractMod.Items.Weapons.Melee
{
    public class DeathsSickle : ModItem // not finished IN THE SLIGHTEST
    {
        public override void SetDefaults()
        {

            Item.damage = 2000;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 60;
            Item.scale = 2f;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8;
            Item.value = 50000000;
            Item.rare = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DeathFlameMain>();
            Item.noMelee = false;
            Item.shootSpeed = 10;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            /*type = ModContent.ProjectileType<ApexN31Bullet>();
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;


            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                //position += muzzleOffset;
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);*/

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item103, player.position);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            /*
            type = ModContent.ProjectileType<ApexN31Rocket>();
            var random = new Random();
            int val = random.Next(1, 7);
            int rotation = random.Next(-20, 20);
            Vector2 shot = new Vector2(velocity.X * 0.3f, velocity.Y * 0.3f).RotatedBy(MathHelper.ToRadians(rotation * 1f));
            if (val == 1)
            {
                Projectile.NewProjectile(source, position, shot, type, damage, knockback, player.whoAmI);
                //Main.
            }*/
            return false;
        }
    }
}

