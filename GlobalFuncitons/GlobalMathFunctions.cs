using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTesseractMod.GlobalFuncitons
{
    internal class GlobalMathFunctions
    {
        public static float Lerp(float x, float y, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);
            return x + amount * (y - x);
        }
    }
}
