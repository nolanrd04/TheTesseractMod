using Microsoft.Xna.Framework.Graphics;
using NATUPNPLib;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using System.IO;

namespace TheTesseractMod
{
	public class TheTesseractMod : Mod
	{
        //recipe groups
        public override void AddRecipeGroups()
        {
            RecipeGroup evilItems = new RecipeGroup(() => $"Any Evil Item", ItemID.ShadowScale, ItemID.TissueSample);
            RecipeGroup.RegisterGroup("evilitem", evilItems);

            RecipeGroup bossSoul = new RecipeGroup(() => $"Any Mechanical Boss Soul", ItemID.SoulofFright, ItemID.SoulofSight, ItemID.SoulofMight);
            RecipeGroup.RegisterGroup("BossSoul", bossSoul);

            RecipeGroup mythrilBar = new RecipeGroup(() => $"Any Mythril Bar", ItemID.MythrilBar, ItemID.OrichalcumBar);
            RecipeGroup.RegisterGroup("MythrilBar", mythrilBar);

            RecipeGroup goldsword = new RecipeGroup(() => $"Any Tier 4 Metal Sword", ItemID.GoldBroadsword, ItemID.PlatinumBroadsword);
            RecipeGroup.RegisterGroup("goldsword", goldsword);
        }

    }
}