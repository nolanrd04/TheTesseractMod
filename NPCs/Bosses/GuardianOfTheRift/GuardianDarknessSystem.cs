using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TheTesseractMod.NPCs.Bosses.GuardianOfTheRift
{
    public class GuardianDarknessSystem : ModSystem
    {
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            // Only apply color effects if the boss is actually active in the world
            if (!NPC.AnyNPCs(ModContent.NPCType<GuardianOfTheRiftBody>()))
            {
                return;
            }

            if (GuardianOfTheRiftBody.glowPhaseActive)
            {
                // --- Glow phase: dark blue tint ---
                // redScale/greenScale/blueScale control how much of each color channel survives
                // Lower = darker for that channel. blueScale is higher to keep the blue tint.
                float redScale = 0.2f;    // How much red light remains (0 = none, 1 = full)
                float greenScale = 0.2f;  // How much green light remains
                float blueScale = 0.45f;  // How much blue light remains (higher = more blue tint)

                // tileColor affects blocks/tiles. The * 2 makes tiles brighter than the background.
                tileColor = new Color(
                    (int)(tileColor.R * redScale * 2),
                    (int)(tileColor.G * greenScale * 2),
                    (int)(tileColor.B * blueScale * 2),
                    tileColor.A
                );
                // backgroundColor affects the sky and distant background layers
                backgroundColor = new Color(
                    (int)(backgroundColor.R * redScale),
                    (int)(backgroundColor.G * greenScale),
                    (int)(backgroundColor.B * blueScale),
                    backgroundColor.A
                );
            }
            else if (GuardianOfTheRiftBody.temporalLoopBackActive)
            {
                // --- Temporal Loop Back: pulse between blue tint and default ---
                float pulse = (float)(System.Math.Sin(Main.GameUpdateCount * 0.1f) + 1f) / 2f; // 0 to 1
                float blueRedScale = MathHelper.Lerp(0.4f, 1f, pulse);
                float blueGreenScale = MathHelper.Lerp(0.5f, 1f, pulse);
                float blueBlueScale = MathHelper.Lerp(1f, 1f, pulse);

                tileColor = new Color(
                    (int)(tileColor.R * blueRedScale),
                    (int)(tileColor.G * blueGreenScale),
                    (int)(tileColor.B * blueBlueScale),
                    tileColor.A
                );
                backgroundColor = new Color(
                    (int)(backgroundColor.R * blueRedScale),
                    (int)(backgroundColor.G * blueGreenScale),
                    (int)(backgroundColor.B * blueBlueScale),
                    backgroundColor.A
                );
            }
            else if (GuardianOfTheRiftBody.darkPhaseIntensity > 0f)
            {
                // --- Dark phase: darkness lerps in as the radius shrinks ---
                // intensity is 0 at the start (normal), 1 when fully charged (near pitch black)
                float intensity = GuardianOfTheRiftBody.darkPhaseIntensity;

                // Target scales at full darkness (intensity = 1). Lower = darker.
                float targetRed = 0.05f;    // How much red light remains at full darkness
                float targetGreen = 0.02f;  // How much green light remains at full darkness
                float targetBlue = 0.08f;   // How much blue light remains (slight purple feel)

                // Lerp from 1 (normal) down to the target scale based on intensity
                float redScale = MathHelper.Lerp(1f, targetRed, intensity);
                float greenScale = MathHelper.Lerp(1f, targetGreen, intensity);
                float blueScale = MathHelper.Lerp(1f, targetBlue, intensity);

                // tileColor affects blocks/tiles. The * 1.5 keeps tiles slightly more visible than the sky.
                float tileBrightness = MathHelper.Lerp(1f, 1.5f, intensity); // gradually increases tile visibility relative to background
                tileColor = new Color(
                    (int)(tileColor.R * redScale * tileBrightness),
                    (int)(tileColor.G * greenScale * tileBrightness),
                    (int)(tileColor.B * blueScale * tileBrightness),
                    tileColor.A
                );
                // backgroundColor affects the sky and distant background layers
                backgroundColor = new Color(
                    (int)(backgroundColor.R * redScale),
                    (int)(backgroundColor.G * greenScale),
                    (int)(backgroundColor.B * blueScale),
                    backgroundColor.A
                );
            }
        }

        public override void ModifyLightingBrightness(ref float scale)
        {
            // Only apply brightness effects if the boss is actually active in the world
            if (!NPC.AnyNPCs(ModContent.NPCType<GuardianOfTheRiftBody>()))
            {
                return;
            }

            if (GuardianOfTheRiftBody.glowPhaseActive)
            {
                // Reduce ambient light level. Lower = darker. 0.7 means 70% of normal brightness.
                scale *= 0.7f;
            }
            else if (GuardianOfTheRiftBody.darkPhaseIntensity > 0f)
            {
                // Lerp ambient light from normal (1.0) down to 0.15 as intensity increases.
                // At full intensity, only 15% of ambient light remains. Torches still work.
                float targetBrightness = 0.15f;
                scale *= MathHelper.Lerp(1f, targetBrightness, GuardianOfTheRiftBody.darkPhaseIntensity);
            }
        }
    }
}