using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.Projectiles.Ranged;

namespace TheTesseractMod.Items.Weapons.Ranged
{
    public class DragonsBreath : ModItem
    {
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Flamethrower);
            Item.damage = 230;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 5;
            Item.useAnimation = 10;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item34;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Flames;
            Item.shootSpeed = 14f;
            Item.crit = 10;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projectile = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[projectile].tileCollide = false;
            Main.projectile[projectile].usesLocalNPCImmunity = true;
            Main.projectile[projectile].localNPCHitCooldown = 15;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Main.projectile[projectile].localNPCHitCooldown = 45;
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20f, 0f);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Flamethrower, 1)
                .AddIngredient(ModContent.ItemType<SoliumBar>(), 20)
                .AddIngredient(ModContent.ItemType<HeatRiftFragment>(), 15)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.8f;
        }
    }
}