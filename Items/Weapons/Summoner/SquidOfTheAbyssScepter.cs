using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs.MinionBuffs;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Ores;
using TheTesseractMod.Projectiles.Summoner;

namespace TheTesseractMod.Items.Weapons.Summoner
{
    public class SquidOfTheAbyssScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 145;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                Item.damage = 90;
            }
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<SquidOfTheAbyssMinion>();
            Item.buffType = ModContent.BuffType<SquidOfTheAbyssMinionBuff>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // If squid already exists, increase its minion slots instead of spawning a new one
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<SquidOfTheAbyssMinion>())
                {
                    // Always refresh the buff to keep the minion alive
                    player.AddBuff(Item.buffType, 2);

                    // Check if player has enough remaining minion slots
                    float usedSlots = 0f;
                    for (int j = 0; j < Main.maxProjectiles; j++)
                    {
                        Projectile p = Main.projectile[j];
                        if (p.active && p.owner == player.whoAmI && p.minion)
                        {
                            usedSlots += p.minionSlots;
                        }
                    }

                    if (usedSlots < player.maxMinions)
                    {
                        proj.minionSlots++;
                        proj.netUpdate = true;
                    }
                    return false;
                }
            }

            // No existing squid, only spawn one if we have room
            float totalSlots = 0f;
            for (int j = 0; j < Main.maxProjectiles; j++)
            {
                Projectile p = Main.projectile[j];
                if (p.active && p.owner == player.whoAmI && p.minion)
                {
                    totalSlots += p.minionSlots;
                }
            }

            if (totalSlots < player.maxMinions)
            {
                player.AddBuff(Item.buffType, 2);
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Starfish, 5)
            .AddIngredient(ModContent.ItemType<SoliumBar>(), 20)
            .AddIngredient(ModContent.ItemType<AquaRiftFragment>(), 15)
            .AddIngredient(ModContent.ItemType<GlowRiftFragment>(), 15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}