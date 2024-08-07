using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace TheTesseractMod.Dusts
{
    internal class MoltenSphereDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.scale = 1f;
        }

        private float rotationFactor = 0.2f;
        private static readonly Color[] colors = {
            new Color(255, 148, 102),
            new Color(156, 147, 146)
        };

        
        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, dust.color.R / 255f, dust.color.G / 255f, dust.color.B / 255f);
            //Lighting.AddLight(dust.position, 1f, 1f, 1f);

            dust.rotation += rotationFactor;
            dust.scale *= 0.98f;

            if (dust.scale < 0.35f)
            {
                dust.active = false;
            }
            return true;
        }
    }
}
