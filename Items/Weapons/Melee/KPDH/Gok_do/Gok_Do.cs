using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Buffs;
using TheTesseractMod.Projectiles.Melee.EtherealLanceProjectiles;
using TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Gok_do;
using TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Sain_geom;

namespace TheTesseractMod.Items.Weapons.Melee.KPDH.Gok_do
{
    internal class Gok_Do : ModItem
    {
        public int attackPhase = 0;
        public bool sfxPlayed = false;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 260;
            Item.crit = 4;
            Item.knockBack = 7;
            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.width = 80;
            Item.height = 80;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Gok_Do_Proj>();
            Item.shootSpeed = 5f;
            Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (NPC.downedMoonlord)
            {
                damage *= 1.5f;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (attackPhase == 3)
            {
                type = ModContent.ProjectileType<Gok_Do_Proj_Stage3>();
                float dmg = damage * 1.07f;
                damage = (int)dmg;
            }
            else if (attackPhase == 2)
            {
                type = ModContent.ProjectileType<Gok_Do_Proj_Stage2>();
                float dmg = damage * 1.04f;
                damage = (int)dmg;
            }
            else
            {
                type = ModContent.ProjectileType<Gok_Do_Proj>();
            }



            Projectile.NewProjectile(source, position, velocity, type, damage, knockback);
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            // Increment timer
            if (attackPhase !=3 || !player.HasBuff(ModContent.BuffType<ParryReady>()))
            {
                GokDoStats.comboExpireTimer++;
            }

            if (GokDoStats.comboExpireTimer > 60)
            {
                if (attackPhase != 0) // only reset if we actually *leave* a phase
                {
                    attackPhase = 0;
                    sfxPlayed = false;
                }

                GokDoStats.consecutiveHits = 0;
                GeomSwordStats.canBuildConsecutiveHits = true;
                return; // skip phase logic until we get new hits
            }

            // Work out which phase we should be in
            int newPhase = 0;
            if (GokDoStats.consecutiveHits >= 21)
            {
                newPhase = 3;
            }
            else if (GokDoStats.consecutiveHits >= 12)
            {
                newPhase = 2;
            }
            else if (GokDoStats.consecutiveHits >= 6)
            {
                newPhase = 1;
            }

            if (newPhase != attackPhase)
            {
                attackPhase = newPhase;

                if (attackPhase > 0)
                {
                    SoundEngine.PlaySound(SoundID.Item29, player.position);
                    player.AddBuff(ModContent.BuffType<ParryReady>(), 180);
                }
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return player.HasBuff(ModContent.BuffType<ParryReady>());
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.HasBuff(ModContent.BuffType<ParryReady>()))
            {
                if (attackPhase == 3)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Gok_Do_ParryProj_Stage3>(), (int)(Item.damage * 1.07f), Item.knockBack, player.whoAmI);
                }
                else if (attackPhase == 2)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Gok_Do_ParryProj_Stage2>(), (int)(Item.damage * 1.05f), Item.knockBack, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Gok_Do_ParryProj_Stage1>(), (int)(Item.damage * 1.03f), Item.knockBack, player.whoAmI);
                }

                player.ClearBuff(ModContent.BuffType<ParryReady>());
                GokDoStats.consecutiveHits = 0;
                return false;
            }
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FragmentSolar, 5);
            recipe.AddIngredient(ItemID.FragmentNebula, 5);
            recipe.AddIngredient(ItemID.FragmentStardust, 5);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddIngredient(3836); // Ghastly glaive
            recipe.Register();
            recipe.AddTile(TileID.LunarCraftingStation);
        }

        private static readonly Color[] itemNameCycleColors = {
            new Color(136, 255, 251),
            new Color (136, 211, 255),
            new Color (176, 136, 255),
            new Color (233, 136, 255)
        };
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int numColors = itemNameCycleColors.Length;

            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "ItemName" && NPC.downedMoonlord)
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;

                    line.OverrideColor = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
                }

                if (line.Mod == "Terraria" && line.Text.Contains("Gets buffed after defeating the Moon Lord"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("Gets buffed after defeating the Moon Lord", $"[c/{rgbValue}:Gets buffed after defeating the Moon Lord!]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("PARRY"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("PARRY", $"[c/{rgbValue}:PARRY]");
                }
            }
        }
    }

    internal class GokDoStats : ModPlayer
    {
        public static int consecutiveHits = 0;
        public static int attackType = 0;
        public static int comboExpireTimer = 0; // keeps track of how long it has been since a consecutive hit
        public static bool canBuildConsecutiveHits = true; // whether or not hits will count towards the consecutive counter
    }
}
