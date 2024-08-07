using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using TheTesseractMod.Items.Materials;
using Terraria.Audio;
using TheTesseractMod.Buffs;
using System.Collections;
using ReLogic.Content;

namespace TheTesseractMod.Items.Tesseracts
{
    internal class Tesseract7 : ModItem
    {
        Color color = new Color(66, 155, 245);
        private int functionStyle = 3;
        private float clarity;
        private float scalingFactor = 1f;
        Color colorFrontGlow = Color.White;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Lime;
            Item.value = 35;
            //recall
            if (functionStyle % 3 == 0)
            {
                Item.useStyle = ItemUseStyleID.HoldUp;
                Item.useTime = 90;
                Item.useAnimation = 90;
                Item.height = 26;
                Item.width = 26;
                Item.UseSound = SoundID.Item6;
                Item.autoReuse = false;

                Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 36));
                ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            }
            //time warp
            else if (functionStyle % 3 == 1)
            {
                Item.useTime = 1;
                Item.useAnimation = 1;
                Item.autoReuse = true;
                Item.rare = ItemRarityID.Blue;
                Item.height = 30;
                Item.width = 30;
                Item.UseSound = null;

                Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 36));
                ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            }
            //Temporal Dash
            else
            {
                Item.useStyle = ItemUseStyleID.HoldUp;
                Item.useTime = 20;
                Item.useAnimation = 20;
                Item.height = 26;
                Item.width = 26;
                Item.UseSound = SoundID.Item43;
                Item.autoReuse = false;

                Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 36));
                ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            }
        }
        public override void UseStyle(Player player, Rectangle heldFrame)
        {
            Lighting.AddLight(player.position, 0.1f, 0.9f, 1f);
            //recall
            if (functionStyle % 3 == 0 && player.altFunctionUse != 2)
            {
                for (int d = 0; d < 5; d++)
                {
                    Dust.NewDust(player.position, player.width, player.height, 15, 0f, 0f, 150, default(Color), 1f);
                }
                if (player.itemTime == 0)
                {
                    player.ApplyItemTime(Item);
                }
                else if (player.itemTime == player.itemTimeMax / 2)
                {
                    for (int d = 0; d < 70; d++)
                    {
                        Dust.NewDust(player.position, player.width, player.height, 15, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, default(Color), 1.5f);
                    }
                    player.RemoveAllGrapplingHooks();
                    player.Spawn(PlayerSpawnContext.RecallFromItem);
                    for (int d = 0; d < 70; d++)
                    {
                        Dust.NewDust(player.position, player.width, player.height, 15, 0f, 0f, 150, default(Color), 1.5f);
                    }
                }
            }
            //temporal dash
            if (functionStyle % 3 == 2 && player.altFunctionUse != 2)
            {
                
            }
        }

        public override bool? UseItem(Player player)
        {

            if (player.altFunctionUse != 2)
            {
                if (functionStyle % 3 == 0)
                {
                    //just uses the recall code
                }
                else if (functionStyle % 3 == 1)
                {
                    // Mode 2: Increase time of day functionality
                    Main.time += 60;
                }
                else
                {
                    // temporal dash
                    if (!player.HasBuff(ModContent.BuffType<TemporalDashCooldownDebuff>()))
                    {
                        player.AddBuff(ModContent.BuffType<TemporalDashBuff>(), 600);
                        Vector2 speed = new Vector2(10, 10);
                        for (int d = 0; d < 360; d += 5)
                        {
                            speed = speed.RotatedBy(MathHelper.ToRadians(d));
                            Dust.NewDust(player.position, player.width, player.height, 45, speed.X, speed.Y, 150, default(Color), 1.5f);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return base.UseItem(player);
        }


        public override bool AltFunctionUse(Player player)
        {
            functionStyle++;
            SetDefaults();
            if (functionStyle % 3 == 0)
            {
                Main.NewText("Recall mode activated. ");
            }
            else if ((functionStyle % 3 == 1))
            {
                Main.NewText("Time Warp mode activated. ");
                SoundEngine.PlaySound(SoundID.Item4, player.position);
            }
            else
            {
                Main.NewText("Temporal Dash mode activated. ");
                SoundEngine.PlaySound(SoundID.Item4, player.position);
            }

            for (int d = 0; d < 5; d++)
            {
                Dust.NewDust(player.position, player.width, player.height, 15, 0f, 0f, 150, default(Color), 1f);
            }

            player.itemTime = 10;
            

            return true;
        }

        private static readonly Color[] itemNameCycleColors = {
            new Color(153, 214, 255),
            new Color (3, 152, 252),
            new Color (5, 58, 250)
        };
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // This code shows using Color.Lerp,  Main.GameUpdateCount, and the modulo operator (%) to do a neat effect cycling between 4 custom colors.
            int numColors = itemNameCycleColors.Length;

            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "ItemName")
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;

                    line.OverrideColor = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
                }

                if (line.Mod == "Terraria" && line.Text.Contains("Temporal Dash"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("Temporal Dash", $"[c/{rgbValue}:Temporal Dash]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("Function 1"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("Function 1", $"[c/ff00ff:Function 1]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("Function 2"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("Function 2", $"[c/ff00ff:Function 2]");
                }

                if (line.Mod == "Terraria" && line.Text.Contains("Function 3"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
                    line.Text = line.Text.Replace("Function 3", $"[c/ff00ff:Function 3]");
                }
            }
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            int numColors = itemNameCycleColors.Length;
            float fade = (Main.GameUpdateCount % 60) / 60f;
            int index = (int)((Main.GameUpdateCount / 60) % numColors);
            int nextIndex = (index + 1) % numColors;

            color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);


            Asset<Texture2D> glowTextur = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect");

            spriteBatch.Draw(glowTextur.Value, position, null,
                color, 0, glowTextur.Size() / 2, scale, SpriteEffects.None, 0f);

            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float fade = (Main.GameUpdateCount % 60) / 60f;
            clarity = Lerp(0.2f, 1f, fade);

            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect");

            spriteBatch.Draw(glowTexture.Value, position, null,
                Color.White * clarity, 0, glowTexture.Size() / 2, scale / 2, SpriteEffects.None, 0f);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {

            int numColors = itemNameCycleColors.Length;
            float fade = (Main.GameUpdateCount % 60) / 60f;
            int index = (int)((Main.GameUpdateCount / 60) % numColors);
            int nextIndex = (index + 1) % numColors;

            color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
            scalingFactor = Lerp(1f, 1.5f, fade);


            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect");
            spriteBatch.Draw(glowTexture.Value,
                new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Value.Width, glowTexture.Value.Height),
                color, rotation, glowTexture.Size() * 0.5f, scale * 0.85f, SpriteEffects.None, 0f);


            Lighting.AddLight(Item.Center, 0f, 1f, 1f);
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AtomOfTime>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Tesseract6>(), 1);
            recipe.AddIngredient(ItemID.BeetleHusk, 5);
            recipe.Register();
        }

        public float Lerp(float x, float y, float amount)
        {
            amount = Math.Clamp(1, x, y);
            return x + amount * (y - x);
        }
    }
}
