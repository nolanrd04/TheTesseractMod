using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Projectiles.Summoner;

namespace TheTesseractMod.Items.Weapons.Summoner
{
    public class ZenithSummonStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }


        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 1;
            Item.UseSound = SoundID.Item28;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 100;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Item.damage = 160;
                Item.damage += (int)(Item.damage * 0.105f);
            }
            Item.knockBack = 5f;
            Item.mana = 15;
            Item.noMelee = true;
            Item.value = 1500000;
            Item.rare = 10;
            Item.shoot = ModContent.ProjectileType<ZenithMinion>();
            Item.buffType = ModContent.BuffType<ZenithMinionBuff>();
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.AbigailsFlower, 1);
            recipe.AddIngredient(ItemID.FlinxStaff, 1);
            recipe.AddIngredient(ItemID.SlimeStaff, 1);
            recipe.AddIngredient(ItemID.HornetStaff, 1);
            recipe.AddIngredient(ItemID.ImpStaff, 1);
            recipe.AddIngredient(ItemID.SpiderStaff, 1);
            recipe.AddIngredient(ItemID.SanguineStaff, 1);
            recipe.AddIngredient(4758, 1);
            recipe.AddIngredient(ItemID.OpticStaff, 1);
            recipe.AddIngredient(ItemID.PygmyStaff, 1);
            recipe.AddIngredient(ItemID.DeadlySphereStaff, 1);
            recipe.AddIngredient(ItemID.RavenStaff, 1);
            recipe.AddIngredient(ItemID.TempestStaff, 1);
            recipe.AddIngredient(ItemID.StardustDragonStaff, 1);
            recipe.AddIngredient(5005, 1);
            recipe.AddIngredient(ItemID.RainbowCrystalStaff, 1);
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind("AuricBar", out ModItem AuricBar))
            {
                recipe.AddIngredient(AuricBar.Type, 5);
            }

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);

            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, -10f);
        }
    }
}
