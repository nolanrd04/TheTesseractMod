using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TheTesseractMod.Systems
{
    internal class KeyBindSystems :ModSystem
    {
        //for tesseract level 6
        public static ModKeybind keyBind;
        public override void Load()
        {
            keyBind = KeybindLoader.RegisterKeybind(Mod, "TesseractMouseTeleport", "Q");
        }
    }
}
