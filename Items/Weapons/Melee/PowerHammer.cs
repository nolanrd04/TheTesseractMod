using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;

namespace TheTesseractMod.Items.Weapons.Melee
{
    public class PowerHammer : ModItem
    {
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.PaladinsHammer);
            Item.damage = 215;
            // if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            // {
            //     Item.damage = 115;
            // }
            Item.DamageType = DamageClass.Melee;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.PowerHammerProjectile>();
            Item.shootSpeed = 12f;
            Item.crit = 8;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.PaladinsHammer, 1)
                .AddIngredient(ModContent.ItemType<SoliumBar>(), 20)
                .AddIngredient(ModContent.ItemType<ElectricRiftFragment>(), 15)
                .AddIngredient(ItemID.CrystalShard, 20)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}