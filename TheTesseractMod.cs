using Microsoft.Xna.Framework.Graphics;
using NATUPNPLib;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TheTesseractMod
{
	public class TheTesseractMod : Mod
	{
        //recipe groups
        public override void AddRecipeGroups()
        {
            RecipeGroup evilBar = new RecipeGroup(() => $"Any Evil Bar", ItemID.DemoniteBar, ItemID.CrimtaneBar); 
            RecipeGroup.RegisterGroup("EvilBar",  evilBar);

            RecipeGroup evilItems = new RecipeGroup(() => $"Any Evil Item", ItemID.ShadowScale, ItemID.TissueSample);
            RecipeGroup.RegisterGroup("evilitem", evilItems);

            RecipeGroup bossSoul = new RecipeGroup(() => $"Any Mechanical Boss Soul", ItemID.SoulofFright, ItemID.SoulofSight, ItemID.SoulofMight);
            RecipeGroup.RegisterGroup("BossSoul", bossSoul);

            RecipeGroup mythrilBar = new RecipeGroup(() => $"Any Mythril Bar", ItemID.MythrilBar, ItemID.OrichalcumBar);
            RecipeGroup.RegisterGroup("MythrilBar", mythrilBar);

            RecipeGroup goldsword = new RecipeGroup(() => $"Any Tier 3 Metal Sword", ItemID.GoldBroadsword, ItemID.PlatinumBroadsword);
            RecipeGroup.RegisterGroup("goldsword", goldsword);

            RecipeGroup dungeonStaff = new RecipeGroup(() => $"Any Dungeon Sorcerer staff", ItemID.ShadowbeamStaff, ItemID.InfernoFork, ItemID.SpectreStaff);
            RecipeGroup.RegisterGroup("DungeonStaff", dungeonStaff);
        }

        public override void PostSetupContent()
        {
            
        }

    }
}