using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.TrueNightsWeapons
{
    internal class TrueNightsLongBow : ModItem
    {
        private int shotIndex = 0;
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.useAnimation = 8;
            Item.useTime = 2;
            Item.reuseDelay = 56;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4.75f;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 20;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<NightsLongBow>());
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Lighting.AddLight(position, Color.Lime.R / 255f, Color.Lime.G / 255f, Color.Lime.B / 255f);
            Vector2 vel;
            shotIndex = (shotIndex + 1) % 4;
            if (shotIndex == 0)
            {
                SoundEngine.PlaySound(SoundID.Item5, position);
                for (int i = 0; i < 9; i++)
                {
                    vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15)) / 2;
                    Dust.NewDust(position, 1, 1, ModContent.DustType<SharpRadialGlowDust>(), vel.X, vel.Y, 0, Color.Purple, 1f);
                    Dust.NewDust(position, 1, 1, ModContent.DustType<SharpRadialGlowDust>(), vel.X, vel.Y, 0, Color.Lime, 1f);
                }
            }

            if (Main.rand.Next(6) == 0)
            {
                if (Main.rand.NextBool())
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15)), ProjectileID.CursedArrow, damage, knockback);
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15)), ProjectileID.VenomArrow, damage, knockback);
                    }
                }
            }
            return true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() <= 0.40f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, 0);
        }
    }
}
