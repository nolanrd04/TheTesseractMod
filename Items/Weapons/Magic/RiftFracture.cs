using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using Terraria;
using Microsoft.Xna.Framework;


namespace TheTesseractMod.Items.Weapons.Magic
{
    public class RiftFracture : ModItem
    {
        public override void SetDefaults()
        {
            Item.staff[Item.type] = true;
            Item.damage = 300;
            Item.DamageType = DamageClass.Magic;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 5;
            Item.useAnimation = 20;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.RiftFractureProj>();
            Item.shootSpeed = 25f;
            Item.crit = 20;
            Item.mana = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spawnPos = position + new Vector2(Main.rand.NextFloat(-70, 70f), Main.rand.NextFloat(-70, 70f));
            Vector2 direction = (Main.MouseWorld - spawnPos).SafeNormalize(Vector2.UnitX);

            for (int i = 0; i < 12; i++)
            {
                float rotation = MathHelper.ToRadians(i * 30);
                Vector2 dustVelocity = direction.RotatedBy(rotation);
                int dustIndex = Dust.NewDust(spawnPos, 0, 0, DustID.GoldFlame, dustVelocity.X, dustVelocity.Y, 0, default, 2f);
                Main.dust[dustIndex].noGravity = true;
            }

            Projectile.NewProjectile(source, spawnPos, direction * Item.shootSpeed, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.SkyFracture, 1)
                .AddIngredient(ModContent.ItemType<SoliumBar>(), 20)
                .AddIngredient(ModContent.ItemType<LightRiftFragment>(), 15)
                .AddIngredient(ItemID.SoulofLight, 20)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}