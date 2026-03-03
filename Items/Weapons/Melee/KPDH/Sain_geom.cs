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
using TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Sain_geom;

using TheTesseractMod.ItemDropRulesANDConditions;
using TheTesseractMod.Buffs;

namespace TheTesseractMod.Items.Weapons.Melee.KPDH
{
    internal class Sain_geom : ModItem // adapted from ExampleMod
    {
        public int attackType = 0; // keeps track of which attack it is (up/down)
        public int swingTypeExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time
        public int attackPhase = 0; // what attack pattern is to be used
        public bool sfxPlayed = false;
        public int dashCooldown = 0;
        public override void SetDefaults()
        {
            Item.damage = 174;
            Item.width = 80;
            Item.height = 80;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.crit = 4;
            Item.shoot = ModContent.ProjectileType<GeomSwingProjectile>();
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
            if (attackPhase == 1)
            {
                int thrownSwordProjCount = 0;
                type = ModContent.ProjectileType<SainGeomSwingProjectile_Stage2>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    SoundEngine.PlaySound(SoundID.Item1, player.position);
                    if (proj.active && proj.type == ModContent.ProjectileType<Sain_geom_projThrown>() && proj.owner == player.whoAmI)
                    {
                        thrownSwordProjCount++;
                    }
                }
                if (Main.rand.Next(5) == 0 && thrownSwordProjCount < 1)
                {
                    Vector2 direction = Main.MouseWorld - player.Center;
                    direction.Normalize();
                    Projectile.NewProjectile(source, position, direction * 10f, ModContent.ProjectileType<Sain_geom_projThrown>(), damage, 0f);
                }
            }
            else if (attackPhase == 2)
            {
                int thrownSwordProjCount = 0;
                type = ModContent.ProjectileType<SainGeomSwingProjectile_Stage3>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    SoundEngine.PlaySound(SoundID.Item1, player.position);
                    if (proj.active && proj.type == ModContent.ProjectileType<Sain_geom_projThrown_stage3>() && proj.owner == player.whoAmI)
                    {
                        thrownSwordProjCount++;
                    }
                }
                if (Main.rand.Next(5) == 0 && thrownSwordProjCount < 1)
                {
                    Vector2 direction = Main.MouseWorld - player.Center;
                    direction.Normalize();
                    Projectile.NewProjectile(source, position, direction * 10f, ModContent.ProjectileType<Sain_geom_projThrown_stage3>(), damage, 0f);
                }
            }
            else
            {
                type = ModContent.ProjectileType<GeomSwingProjectile>();
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
            attackType = (attackType + 1) % 2; // Increment attackType to make sure next swing is different
            swingTypeExpireTimer = 0; // Every time the weapon is used, we reset this so the combo (swinging up or down) does not expire


            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (swingTypeExpireTimer++ >= 120)
            {
                attackType = 0;
            }

            if (GeomSwordStats.comboExpireTimer++ >= 60)
            {
                if (!player.HasBuff(ModContent.BuffType<Sealed>()))
                {
                    sfxPlayed = false;
                    attackPhase = 0;
                    GeomSwordStats.consecutiveHits = 0;
                    GeomSwordStats.canBuildConsecutiveHits = true;
                }
            }

            else
            {
                int newPhase = attackPhase;

                if (GeomSwordStats.consecutiveHits >= 17)
                {
                    GeomSwordStats.canBuildConsecutiveHits = false;
                    newPhase = 2;
                }
                else if (GeomSwordStats.consecutiveHits >= 7)
                {
                    newPhase = 1;
                }
                else
                {
                    newPhase = 0;
                }

                // Only trigger sound/text if the phase actually changes
                if (newPhase != attackPhase)
                {
                    attackPhase = newPhase;
                    SoundEngine.PlaySound(SoundID.Item29, player.position);
                    if (attackPhase == 2)
                    {
                        player.AddBuff(ModContent.BuffType<Sealed>(), 300);
                    }
                }
            }

            // dash info
            if (dashCooldown > 0)
            {
                dashCooldown--;
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            // return attackPhase == 2;
            return attackPhase == 2;
        }

        public override bool CanUseItem(Player player)
        {
            // Prevent left-click attack when right-clicking
            if (player.altFunctionUse == 2)
            {
                if (dashCooldown == 0)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Sain_geom_dashProj>(), Item.damage, Item.knockBack, player.whoAmI);

                    DashTowardsMouse(player);
                    dashCooldown = 60; // 1 second cooldown at 60 FPS
                }
                return false;
            }
            return true;
        }

        private void DashTowardsMouse(Player player)
        {
            Vector2 direction = Main.MouseWorld - player.Center;
            direction.Normalize();
            float dashSpeed = 20f;
            player.velocity = direction * dashSpeed;

            SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, player.position);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FragmentSolar, 5);
            recipe.AddIngredient(ItemID.FragmentNebula, 5);
            recipe.AddIngredient(ItemID.FragmentStardust, 5);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddIngredient(ItemID.Cutlass);
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

                if (line.Mod == "Terraria" && line.Text.Contains("ATTUNED"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("ATTUNED", $"[c/{rgbValue}:ATTUNED]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("Gets buffed after defeating the Moon Lord"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("Gets buffed after defeating the Moon Lord", $"[c/{rgbValue}:Gets buffed after defeating the Moon Lord]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("SEALED"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("SEALED", $"[c/{rgbValue}:SEALED]");
                }
            }
        }
    }

    internal class GeomSwordStats : ModPlayer // for keeping track of Geom Sword data, such as consecutive hits
    {
        public static int consecutiveHits = 0;
        public static int attackType = 0;
        public static int comboExpireTimer = 0; // keeps track of how long it has been since a consecutive hit
        public static bool canBuildConsecutiveHits; // whether or not hits will count towards the consecutive counter
    }


}
