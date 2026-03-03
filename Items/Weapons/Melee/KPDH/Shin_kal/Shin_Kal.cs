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
using TheTesseractMod.Projectiles.Melee.KPDH_projectiles.Shin_Kal;

namespace TheTesseractMod.Items.Weapons.Melee.KPDH.Shin_kal
{
    internal class Shin_Kal : ModItem
    {
        public int attackPhase = 0;
        public bool sfxPlayed = false;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.crit = 4;
            Item.knockBack = 7;
            Item.useTime = 15;
            Item.useAnimation = 15;

            Item.width = 80;
            Item.height = 80;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Shin_Kal_Proj_Stage1>();
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.Item39;
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
            if (attackPhase == 2)
            {
                type = ModContent.ProjectileType<Shin_Kal_Proj_Stage3>();
                float dmg = damage * 1.07f;
                damage = (int)dmg;
            }
            else if (attackPhase == 1)
            {
                type = ModContent.ProjectileType<Shin_Kal_Proj_Stage2>();
                float dmg = damage * 1.04f;
                damage = (int)dmg;
            }
            else
            {
                type = ModContent.ProjectileType<Shin_Kal_Proj_Stage1>();
            }

            if (player.altFunctionUse == 2)
            {
                int shotProjectiles;
                if (attackPhase == 2)
                {
                    shotProjectiles = 5;
                }
                else if (attackPhase == 1)
                {
                    shotProjectiles = 4;
                }
                else
                {
                    shotProjectiles = 3;
                }

                for (int i = 0; i < shotProjectiles; i++)
                {
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(30f) - 15f)), type, damage, knockback, Main.myPlayer, 0f);
                }

                if (ShinKalStats.comboExpireTimer > 15)
                {
                    ShinKalStats.comboExpireTimer -= 15;
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, 1f);
            }
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            // Increment timer
            if (attackPhase != 2 || !player.HasBuff(ModContent.BuffType<ParryReady>()))
            {
                ShinKalStats.comboExpireTimer++;
            }

            if (ShinKalStats.comboExpireTimer > 60)
            {
                if (attackPhase != 0) // only reset if we actually *leave* a phase
                {
                    attackPhase = 0;
                    sfxPlayed = false;
                }

                ShinKalStats.consecutiveHits = 0;
                GeomSwordStats.canBuildConsecutiveHits = true;
                return; // skip phase logic until we get new hits
            }

            // Work out which phase we should be in
            int newPhase = 0;
            if (ShinKalStats.consecutiveHits >= 26)
            {
                newPhase = 2;
            }
            else if (ShinKalStats.consecutiveHits >= 10)
            {
                newPhase = 1;
            }

            if (newPhase != attackPhase)
            {
                attackPhase = newPhase;

                if (attackPhase > 0)
                {
                    SoundEngine.PlaySound(SoundID.Item29, player.position);
                }
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 45;
                Item.useAnimation = 45;
                Item.autoReuse = true; 
                Item.channel = true;
            }
            else
            {
                SetDefaults();
                Item.channel = false;
            }
                return true;
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            healValue = Item.damage / 10;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FragmentSolar, 5);
            recipe.AddIngredient(ItemID.FragmentNebula, 5);
            recipe.AddIngredient(ItemID.FragmentStardust, 5);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddIngredient(ItemID.VampireKnives);
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

    internal class ShinKalStats : ModPlayer
    {
        public static int consecutiveHits = 0;
        public static int attackType = 0;
        public static int comboExpireTimer = 0; // keeps track of how long it has been since a consecutive hit
        public static bool canBuildConsecutiveHits = true; // whether or not hits will count towards the consecutive counter
    }

}
