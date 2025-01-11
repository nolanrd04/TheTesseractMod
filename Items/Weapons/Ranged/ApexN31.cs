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
using TheTesseractMod.Projectiles.Ranged;
using TheTesseractMod.Global.Projectiles.Ranged;
using TheTesseractMod.Buffs;
using Mono.Cecil;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using TheTesseractMod.Projectiles.Ranged.ApexN31New;
using TheTesseractMod.Projectiles.TerraWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TerraWeapons;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    public class ApexN31 : ModItem
    {
        int[] arrayBulletID = { 14, 36, 89, 104, 207, 242, 279, 283, 284, 285, 286, 287, 638, 981, ModContent.ProjectileType<ApexN31Bullet>() }; //15 idx 14
        int[] arrayDustToMatchBulletID = { 6, 60, 68, 75, 61, 64, 169, 171, 177, 187, 174, 10, 302, 11, 229 };
        public override void SetDefaults()
        {

            Item.damage = 113;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Item.damage += (int)(Item.damage * 0.15f);
            }
            Item.useAmmo = AmmoID.Bullet;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 49;
            Item.height = 23;
            Item.scale = 0.85f;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 1500000;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20;
            Item.noMelee = true;
            Item.reuseDelay = 0;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.ClockworkAssaultRifle, 1);
            recipe.AddIngredient(ItemID.Uzi, 1);
            recipe.AddIngredient(ItemID.VenusMagnum, 1);
            recipe.AddIngredient(ItemID.CandyCornRifle, 1);
            recipe.AddIngredient(ModContent.ItemType<TerraTurret>());
            recipe.AddIngredient(ItemID.VortexBeater, 1);
            recipe.AddIngredient(ItemID.SDMG, 1);
            recipe.AddTile(TileID.LunarCraftingStation);

            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind("AuricBar", out ModItem AuricBar))
            {
                recipe.AddIngredient(AuricBar.Type, 5);
            }


            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            damage /= 2;
            Vector2 dustVelocity = new Vector2(2, 0);
            int randomIdx;

            // Shoot top right
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SDMGGunProj>(), damage, knockback);


            // shoot top left
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<Clockwork_Assault_RifleProj>(), damage, knockback);

            // shoot right
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<Candy_Corn_RifleGunProj>(), damage, knockback);
            Projectile.NewProjectile(source, position + new Vector2(52, 0), velocity, type, damage, knockback);

            // shoot bottom right
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<UziGunProj>(), damage, knockback);

            // shoot bottom left
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<Vortex_BeaterGunProj>(), damage, knockback);

            // shoot left
            Projectile.NewProjectile(source, position, new Vector2(-52, 0), ModContent.ProjectileType<TerraTurretProj>(), damage, knockback);
            Projectile.NewProjectile(source, position + new Vector2(-52, 0), velocity, type, damage, knockback);
            

            return false;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.66f;
        }

        public void ShootBullet(Vector2 position, int index, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int damage, float knockback)
        {
            Vector2 dustVelocity = new Vector2(2, 0);

            Projectile.NewProjectile(source, position, velocity, arrayBulletID[index], damage, knockback);

            Vector2 offset = Vector2.Zero;
            for (int i = 0; i < 10; i++)
            {
                int dustID = Dust.NewDust(position, 1, 1, arrayDustToMatchBulletID[index], dustVelocity.X, dustVelocity.Y, 0, default(Color), 1f);
                Main.dust[dustID].noGravity = true;
                dustVelocity = dustVelocity.RotatedBy(MathHelper.ToRadians(36 * i));
            }
        }
    }
}