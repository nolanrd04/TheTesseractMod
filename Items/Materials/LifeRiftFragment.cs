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
using TheTesseractMod.Dusts;

namespace TheTesseractMod.Items.Materials
{
    internal class LifeRiftFragment : ModItem
    {
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
            UpdateScalingFactor();
            Lighting.AddLight(Item.Center, Color.Pink.ToVector3());
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            //draw glow
            Texture2D glowTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/ForGlowEffect").Value;
            spriteBatch.Draw(glowTexture,
                new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f),
                new Rectangle(0, 0, glowTexture.Width, glowTexture.Height),
                Color.Pink, rotation, glowTexture.Size() * 0.5f, (scale * 0.8f) * scalingFactor, SpriteEffects.None, 0f);

            //draw texture
            Asset<Texture2D> itemTexture = ModContent.Request<Texture2D>("TheTesseractMod/Items/Materials/LifeRiftFragment");
            spriteBatch.Draw(itemTexture.Value,
                new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f),
                new Rectangle(0, 0, itemTexture.Value.Width, itemTexture.Value.Height),
                Color.White, rotation, itemTexture.Value.Size() * 0.5f, (scale * 1.2f) * scalingFactor, SpriteEffects.None, 0f);

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            UpdateScalingFactor();
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("TheTesseractMod/Items/Materials/LifeRiftFragment");
            spriteBatch.Draw(texture.Value, position, new Rectangle(0, 0, texture.Value.Width, texture.Value.Height),
               Color.White, 0, texture.Value.Size() / 2, (scale * 1.5f) * scalingFactor, SpriteEffects.None, 0f);

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
