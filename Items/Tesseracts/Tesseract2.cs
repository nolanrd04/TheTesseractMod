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
using ReLogic.Content;

namespace TheTesseractMod.Items.Tesseracts
{
    internal class Tesseract2 : ModItem
    {
        Color color = new Color(66, 155, 245);
        private float scalingFactor = 1f;
        Color colorFrontGlow = Color.White;
        public override void SetDefaults()
        {
            
            Item.CloneDefaults(ItemID.MagicMirror);
            Item.rare = ItemRarityID.Green;
            Item.value = 10;
            Item.height = 26;
            Item.width = 26;

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 6));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            
        }
        public override void UseStyle(Player player, Rectangle heldFrame)
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

        private static readonly Color[] itemNameCycleColors = {
            new Color(153, 214, 255),
            new Color (3, 152, 252),
            new Color (5, 58, 250)
        };
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int numColors = itemNameCycleColors.Length;

            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;

                    line2.OverrideColor = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
                }
            }
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
        public float Lerp(float x, float y, float amount)
        {
            amount = Math.Clamp(1, x, y);
            return x + amount * (y - x);
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AtomOfTime>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Tesseract1>(), 1);
            recipe.Register();
        }
    }
}
