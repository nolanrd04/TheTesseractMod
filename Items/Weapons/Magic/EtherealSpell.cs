using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles.Magic.EtherealStaffProjectile;
using Microsoft.Xna.Framework;
using TheTesseractMod.Projectiles.Magic.EtherealTomeProjectiles;
using TheTesseractMod.Dusts;
using TheTesseractMod.Items.Materials;
using Terraria.Audio;
using TheTesseractMod.Global.Projectiles.Ranged;

namespace TheTesseractMod.Items.Weapons.Magic
{
    internal class EtherealSpell : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Magic;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(gold: 25);

            Item.rare = ItemRarityID.Red;
            //Item.UseSound = SoundID.Item54;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.RainFriendly;
            Item.shootSpeed = 15;
            Item.mana = 6;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // visual effects (dust)
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(position, 15, 15, ModContent.DustType<StormCloud1>(), velocity.X * 0.5f, velocity.Y * 0.5f, 0, Color.DarkGray, 1f);
            }
            //Dust.NewDust(position, 15, 15, ModContent.DustType<StormCloud1>(), velocity.X * 0.5f, velocity.Y * 0.5f, 0, Color.DarkGray, 1f);
            Vector2 newDustVelocity = velocity * 2 * 0.99f;
            for (int i = 0; i < 5; i++)
            {
                /*float rotation = (Main.rand.NextFloat() * 8f - 4f);
                newDustVelocity = newDustVelocity.RotatedBy(rotation);*/
                //Dust.NewDust(position, 15, 15, DustID.Shadowflame, newDustVelocity.X, newDustVelocity.Y, 0, default(Color), 1f);
                //Dust.NewDust(position, 15, 15, DustID.GemSapphire, newDustVelocity.X, newDustVelocity.Y, 0, default(Color), 1f);
            }

            // shoot rain
            for (int i = 0; i < 3; i++)
            {
                float rotation = (Main.rand.NextFloat() * 8f - 4f);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(rotation)) * 2f, ModContent.ProjectileType<RainDrop>(), Item.damage, Item.knockBack);
            }
            
            SoundEngine.PlaySound(SoundID.Item54, position);

            // shoot lightning bolt
            if (Main.rand.NextFloat() < 0.075f)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<FriendlyThunderbolt>(), Item.damage * 3, Item.knockBack);
                SoundEngine.PlaySound(SoundID.Item94, position);
            }
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(ModContent.ItemType<DustRiftFragment>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DarkRiftFragment>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AquaRiftFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ElectricRiftFragment>(), 12);

            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {

                if (line.Mod == "Terraria" && line.Text.Contains("The clouds are purely visual."))
                {
                    line.Text = line.Text.Replace("The clouds are purely visual.", $"[c/FF0000:The clouds are purely visual.]");
                }
                if (line.Mod == "Terraria" && line.Text.Contains("If they were projectiles lag would be significantly higher."))
                {
                    line.Text = line.Text.Replace("If they were projectiles lag would be significantly higher.", $"[c/FF0000: If they were projectiles lag would be significantly higher.]");
                }
            }
        }
    }
}
