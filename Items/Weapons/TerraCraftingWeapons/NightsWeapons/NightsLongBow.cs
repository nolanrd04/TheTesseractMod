using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.EvilBossWeapons;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.JungleWeapons;
using Microsoft.Xna.Framework;
using TheTesseractMod.Items.Weapons.TerraCraftingWeapons.DungeonWeapons;
using Terraria.Audio;
using TheTesseractMod.Dusts;
using TheTesseractMod.Projectiles.NightsWeapons;

namespace TheTesseractMod.Items.Weapons.TerraCraftingWeapons.NightsWeapons
{
    internal class NightsLongBow : ModItem
    {
        private int shotIndex = 0;
        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.useAnimation = 8;
            Item.useTime = 2;
            Item.reuseDelay = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 17;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BeesKnees);
            if (!WorldGen.crimson)
            {
                recipe.AddIngredient(ItemID.DemonBow);
            }
            else
            {
                recipe.AddIngredient(ItemID.TendonBow);
            }
            recipe.AddIngredient(ModContent.ItemType<BoneBow>());
            recipe.AddIngredient(ItemID.MoltenFury);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vel;
            shotIndex = (shotIndex + 1) % 4;
            if (shotIndex == 0)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<NightsArrowProj>(), damage, knockback);
                SoundEngine.PlaySound(SoundID.Item5, position);
                for (int i = 0; i < 9; i++)
                {
                    vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(30) - 15)) / 2;
                    Dust.NewDust(position, 1, 1, ModContent.DustType<SharpRadialGlowDust>(), vel.X, vel.Y, 0, Color.Purple, 1f);
                }
            }

            return true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() <= 0.30f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, 0);
        }
    }
}
