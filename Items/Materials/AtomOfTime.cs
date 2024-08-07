using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using TheTesseractMod.Items;
using ReLogic.Content;

namespace TheTesseractMod.Items.Materials
{
    internal class AtomOfTime : ModItem
    {
        public override void SetDefaults()
        {
            Item.height = 16;
            Item.width = 16;
            Item.material = true;
            Item.maxStack = 9999;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(2, 16));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.noMelee = true;
            Item.value = 100000;
            Item.material = true;
        }

        

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.LightBlue.ToVector3());
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Color color = new Color(66, 155, 245);
            Texture2D glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect").Value;
            spriteBatch.Draw(glowTexture,
                new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f - 10f),
                new Rectangle(0, 0, glowTexture.Width, glowTexture.Height),
                color, rotation, glowTexture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
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

                if (line.Mod == "Terraria" && line.Text.Contains("Tesseract"))
                {
                    float fade = (Main.GameUpdateCount % 60) / 60f;
                    int index = (int)((Main.GameUpdateCount / 60) % numColors);
                    int nextIndex = (index + 1) % numColors;
                    Color color = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);

                    string rgbValue = color.Hex3();

                    line.Text = line.Text.Replace("Tesseract", $"[c/{rgbValue}:Tesseract]");
                }
            }
        }
    }
}
