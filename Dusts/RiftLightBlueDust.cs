using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.Dusts
{
    internal class RiftLightBlueDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale = 0.8f;
        }

        public override bool Update(Dust dust)
        {
            dust.rotation += 0.1f;
            Lighting.AddLight(dust.position, 89 / 255f, 247 / 255f, 255 / 255f);
            dust.scale *= 0.99f;

            if (dust.scale < 0.3f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
