using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueExcaliburWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons;
using TheTesseractMod.Projectiles.TerraWeapons;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TerraWeapons
{
    internal class TerraTalonbow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 83;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 20f;
            Item.noMelee = true;
            Item.crit = 8;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Lighting.AddLight(position, Color.Lime.R / 255f, Color.Lime.G / 255f, Color.Lime.B / 255f);
            Vector2 vel;
            for (int i = 0; i < 9; i++)
            {
                vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15)) / 2;
                Dust.NewDust(position, 1, 1, ModContent.DustType<SharpRadialGlowDust>(), vel.X, vel.Y, 0, Color.Lime, 1f);
            }

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TerraArrow>(), damage, knockback);
            for (int i = 0; i < Main.rand.Next(2) + 1; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(10) - 5)), ModContent.ProjectileType<TerraArrow>(), damage, knockback);
            }

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TrueNightsLongBow>());
            recipe.AddIngredient(ItemID.ChlorophyteShotbow);
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() <= 0.50f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, 0);
        }
    }
}
