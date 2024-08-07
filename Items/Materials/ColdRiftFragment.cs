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
    internal class ColdRiftFragment : ModItem
    {
        private float Rotation = 0; 
        private float scalingFactor = 0.7f;
        private bool increasing = true;
        int timer = 0;
        public override void SetDefaults()
        {
            Item.height = 16;
            Item.width = 16;
            Item.material = true;
            Item.maxStack = 9999;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.noUseGraphic = true;
            
            Item.noMelee = true;
            Item.value = 1000000;
            Item.material = true;
            Item.rare = 11;
        }

        public void UpdateScalingFactor()
        {
            if (increasing)
            {
                scalingFactor += 0.01f;
            }
            else
            {
                scalingFactor -= 0.01f;
            }
            timer++;
            if (timer == 30)
            {
                increasing = !increasing;
                timer = 0;
            }
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3());
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rotation += 0.05f;

            Color color = new Color(143, 236, 255);
            Lighting.AddLight(new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f), color.ToVector3());
            Texture2D glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect").Value;

            spriteBatch.Draw(glowTexture,
                new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Width, glowTexture.Height),
                color, rotation, glowTexture.Size() * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);

            spriteBatch.Draw(glowTexture,
                new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Width, glowTexture.Height),
                Color.White, rotation, glowTexture.Size() * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);

            Asset<Texture2D> spriteTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/Materials/ColdRiftFragment");
            spriteBatch.Draw(spriteTexture.Value,
                new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f),
                new Rectangle(0, 0, spriteTexture.Value.Width, spriteTexture.Value.Height),
                lightColor, Rotation, spriteTexture.Value.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            UpdateScalingFactor();
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("TheTesseractMod/Items/Materials/ColdRiftFragment");
            spriteBatch.Draw(texture.Value, position, null,
               Color.White, 0, texture.Size() / 2, scale * 1.2f * scalingFactor, SpriteEffects.None, 0f);

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "ItemName")
                {

                    line.OverrideColor = new Color(255, 183, 89);
                }
            }
        }
    }
}
