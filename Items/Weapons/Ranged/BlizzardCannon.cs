using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.Projectiles.Ranged;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    public class BlizzardCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 170;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 68;
            Item.height = 44;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.crit = 6;
            Item.shoot = ProjectileID.RocketI;
            Item.useAmmo = AmmoID.Rocket;
            Item.shootSpeed = 12f;
        }

        private static readonly Dictionary<int, int> RocketToSnowman = new()
        {
            { ProjectileID.RocketI,   ProjectileID.RocketSnowmanI },
            { ProjectileID.RocketII,  ProjectileID.RocketSnowmanII },
            { ProjectileID.RocketIII, ProjectileID.RocketSnowmanIII },
            { ProjectileID.RocketIV,  ProjectileID.RocketSnowmanIV },
            
        };

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Map to snowman rocket equivalent
            if (RocketToSnowman.TryGetValue(type, out int snowmanType))
                type = snowmanType;

            // Spawn the held cannon only if one doesn't already exist for this player
            // bool cannonExists = false;
            // for (int i = 0; i < Main.maxProjectiles; i++)
            // {
            //     Projectile p = Main.projectile[i];
            //     if (p.active && p.owner == player.whoAmI && p.type == ModContent.ProjectileType<Projectiles.Ranged.BlizzardCannonProj>())
            //     {
            //         cannonExists = true;
            //         break;
            //     }
            // }
            // if (!cannonExists)
            // {
            //     Projectile.NewProjectile(source, player.MountedCenter, velocity, ModContent.ProjectileType<Projectiles.Ranged.BlizzardCannonProj>(), 0, 0f, player.whoAmI);
            // }

            // Fire the bullet from the cannon's position using the fixed angle
            // float fireAngle = player.direction == 1 ? -MathHelper.ToRadians(35f) : -MathHelper.ToRadians(155f);
            // Vector2 bulletVelocity = fireAngle.ToRotationVector2() * Item.shootSpeed;
            // Vector2 cannonCenter = player.MountedCenter + new Vector2(0f, 11f);
            Vector2 bulletVelocity = velocity.SafeNormalize(Vector2.Zero) * Item.shootSpeed;
            Vector2 cannonCenter = player.MountedCenter;
            Projectile.NewProjectile(source, cannonCenter, bulletVelocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 10))) * Main.rand.NextFloat(0.6f, 2f), ProjectileID.SnowBallFriendly, damage / 2, knockback, player.whoAmI);

            if (Main.rand.NextBool(6))
            {
                Projectile.NewProjectile(source, cannonCenter, bulletVelocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 10))), type, damage, knockback, player.whoAmI);
            }

            if (Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(source, cannonCenter, bulletVelocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 10))) * Main.rand.NextFloat(1f, 2f), ModContent.ProjectileType<BlizzardSnowflake>(), damage, knockback, player.whoAmI);
            }

            Dust.NewDust(cannonCenter, 1, 1, ModContent.DustType<DustCloud>(), bulletVelocity.X, bulletVelocity.Y, 0, Color.White, 1f);

            return false; // skip default spawn
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, 0f);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.8f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SnowmanCannon, 1);
            recipe.AddIngredient(ModContent.ItemType<SoliumBar>(), 20);
            recipe.AddIngredient(ModContent.ItemType<ColdRiftFragment>(), 15);
            recipe.AddIngredient(ItemID.SnowballCannon, 1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}