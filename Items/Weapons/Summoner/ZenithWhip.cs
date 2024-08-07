using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Projectiles;

namespace TheTesseractMod.Items.Weapons.Summoner
{
    internal class ZenithWhip : ModItem
    {
        private int whipPicker = 0;
        private float whipSize = 1.3f;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            int damage = 130;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                damage = 200;
                damage += (int)(Item.damage * 0.105f);
                whipSize = 1.7f;
            }
            Item.DefaultToWhip(ProjectileID.RainbowWhip, damage, 4, 4);
            Item.autoReuse = true;
            Item.rare = 10;
            Item.value = 1500000;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BlandWhip, 1);
            recipe.AddIngredient(ItemID.ThornWhip, 1);
            recipe.AddIngredient(ItemID.BoneWhip, 1);
            recipe.AddIngredient(ItemID.FireWhip, 1);
            recipe.AddIngredient(ItemID.CoolWhip, 1);
            recipe.AddIngredient(ItemID.SwordWhip, 1);
            recipe.AddIngredient(ItemID.MaceWhip, 1);
            recipe.AddIngredient(ItemID.ScytheWhip, 1);
            recipe.AddIngredient(ItemID.RainbowWhip, 1);
            recipe.AddIngredient(ItemID.FragmentStardust, 10);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind("AuricBar", out ModItem AuricBar))
            {
                recipe.AddIngredient(AuricBar.Type, 5);
            }

            recipe.Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity * whipSize, 849, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity * whipSize, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, (velocity + (velocity * (0.5f))) * whipSize, 913, damage, knockback, player.whoAmI);

            Projectile.NewProjectile(source, position, -0.5f * velocity * whipSize, 849, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, -0.5f * velocity * whipSize, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, -0.5f * (velocity + (velocity * (0.5f))) * whipSize, 913, damage, knockback, player.whoAmI);
            

            return false;
        }
    }
}
