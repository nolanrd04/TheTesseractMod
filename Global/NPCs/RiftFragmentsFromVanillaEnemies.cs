using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.ItemDropRulesANDConditions;


namespace TheTesseractMod.Global.NPCs
{
    internal class RiftFragmentsFromVanillaEnemies : GlobalNPC // adds fragment drops to vanilla enemies
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (AquaCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<AquaRiftFragment>(), 8, 1, 3, 1));
            }
            else if (ChloroCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<ChloroRiftFragment>(), 15, 1, 3, 1));
            }
            else if (ColdCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<ColdRiftFragment>(), 8, 1, 3, 1));
            }
            else if (DarkCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<DarkRiftFragment>(), 10, 1, 3, 1));
            }
            else if (DeathCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<DeathRiftFragment>(), 8, 1, 3, 1));
            }
            else if (DustCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<DustRiftFragment>(), 10, 1, 3, 1));
            }
            else if (ElectricCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<ElectricRiftFragment>(), 4, 1, 3, 1));
            }
            else if (GlowCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<GlowRiftFragment>(), 4, 1, 3, 1));
            }
            else if (HeatCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<HeatRiftFragment>(), 7, 1, 3, 1));
            }
            else if (LifeCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<LifeRiftFragment>(), 6, 1, 3, 1));
            }
            else if (LightCheck(npc))
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DownedMoonLord(), ModContent.ItemType<LightRiftFragment>(), 10, 1, 3));
            }
        }

        public bool AquaCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.PinkJellyfish || type == NPCID.Crab || type == NPCID.Shark || type == NPCID.BlueJellyfish || type == NPCID.AnglerFish || type == NPCID.BloodJelly || type == NPCID.BloodFeeder || type == NPCID.Arapaima
                 || type == NPCID.FungoFish || type == NPCID.Squid || type == NPCID.Piranha || type == NPCID.GreenJellyfish || type == NPCID.AngryNimbus)
            {
                return true;
            }
            return false;
        }

        public bool ChloroCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.JungleSlime || type == NPCID.JungleBat || type == NPCID.Piranha || type == NPCID.Snatcher || type == NPCID.Derpling || type == NPCID.GiantTortoise || type == NPCID.GiantFlyingFox || type == NPCID.Arapaima
                 || type == NPCID.AnglerFish || type == NPCID.AngryTrapper || type == NPCID.Lihzahrd || type == NPCID.LihzahrdCrawler || type == NPCID.FlyingSnake || type == NPCID.MossHornet || type == NPCID.Hornet || type == NPCID.SpikedJungleSlime)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool ColdCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.Flocko || type == NPCID.Yeti || type == NPCID.PresentMimic || type == NPCID.SnowBalla || type == NPCID.MisterStabby || type == NPCID.SnowmanGangsta || type == NPCID.IceGolem || type == NPCID.IceSlime
                 || type == NPCID.SpikedIceSlime || type == NPCID.ZombieEskimo || type == NPCID.ArmedZombieEskimo || type == NPCID.IceElemental || type == NPCID.Wolf || type == NPCID.IceBat || type == NPCID.SnowFlinx || type == NPCID.CyanBeetle
                  || type == NPCID.IceTortoise || type == NPCID.IcyMerman || type == NPCID.IceMimic)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DarkCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.Zombie || type == NPCID.DemonEye || type == NPCID.PossessedArmor || type == NPCID.WanderingEye || type == NPCID.TheGroom || type == NPCID.TheBride || type == NPCID.GiantFlyingFox || type == NPCID.BloodZombie
                 || type == NPCID.Drippler || type == NPCID.YellowSlime || type == NPCID.RedSlime || type == NPCID.BlackSlime || type == NPCID.MotherSlime || type == NPCID.Wraith || type == NPCID.ArmedZombieEskimo || type == NPCID.ZombieEskimo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeathCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.EyeballFlyingFish || type == NPCID.ZombieMerman || type == NPCID.GoblinShark || type == NPCID.BloodEelBody || type == NPCID.BloodSquid || type == NPCID.MaggotZombie || type == NPCID.Ghost || type == NPCID.Reaper
                 || type == NPCID.Wraith || type == NPCID.BloodCrawler || type == NPCID.BloodCrawlerWall || type == NPCID.Crimera || type == NPCID.EaterofSouls || type == NPCID.FaceMonster || type == NPCID.BloodMummy || type == NPCID.DarkMummy
                  || type == NPCID.Corruptor || type == NPCID.Clinger || type == NPCID.FloatyGross || type == NPCID.Herpling || type == 98)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DustCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.Antlion || type == NPCID.Mummy || type == NPCID.DarkMummy || type == NPCID.LightMummy || type == NPCID.BloodMummy || type == NPCID.WalkingAntlion || type == NPCID.FlyingAntlion || type == NPCID.GiantFlyingAntlion
                 || type == NPCID.GiantWalkingAntlion || type == NPCID.TombCrawlerHead || type == NPCID.DuneSplicerHead || type == NPCID.DesertGhoul || type == NPCID.DesertGhoulCorruption || type == NPCID.DesertGhoulCrimson || type == NPCID.DesertGhoulHallow
                  || type == 533 || type == NPCID.Tumbleweed || type == NPCID.SandElemental || type == NPCID.SandShark || type == NPCID.SandsharkCorrupt || type == NPCID.SandsharkCrimson || type == NPCID.SandsharkHallow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ElectricCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.MartianSaucerCore || type == NPCID.Scutlix || type == NPCID.MartianWalker || type == NPCID.MartianDrone || type == NPCID.MartianTurret || type == NPCID.GigaZapper || type == NPCID.MartianEngineer
                 || type == NPCID.MartianOfficer || type == NPCID.RayGunner || type == NPCID.GrayGrunt || type == NPCID.BrainScrambler || type == NPCID.VortexRifleman || type == NPCID.VortexSoldier || type == NPCID.VortexHornetQueen
                  || type == NPCID.MartianProbe || type == NPCID.WyvernHead)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GlowCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.AnomuraFungus || type == NPCID.FungiBulb || type == NPCID.MushiLadybug || type == NPCID.SporeBat || type == NPCID.SporeSkeleton || type == NPCID.ZombieMushroom || type == NPCID.ZombieMushroomHat
                 || type == NPCID.FungoFish || type == NPCID.GiantFungiBulb)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HeatCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.MeteorHead || type == NPCID.HellArmoredBones || type == NPCID.HellArmoredBonesMace || type == NPCID.HellArmoredBonesSpikeShield || type == NPCID.HellArmoredBonesSword || type == NPCID.Hellbat
                 || type == NPCID.Hellbat || type == NPCID.Lavabat || type == NPCID.LavaSlime || type == NPCID.Demon || type == NPCID.VoodooDemon || type == NPCID.Lavabat || type == NPCID.RedDevil || type == NPCID.SolarSolenian
                  || type == NPCID.SolarSpearman)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LifeCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.Pixie || type == NPCID.Unicorn || type == NPCID.Gastropod || type == NPCID.LightMummy || type == NPCID.IlluminantBat || type == NPCID.IlluminantSlime || type == NPCID.ChaosElemental || type == NPCID.PigronHallow
                 || type == NPCID.DesertGhoulHallow || type == NPCID.MotherSlime || type == NPCID.Pinky || type == NPCID.DungeonSpirit || type == NPCID.Mimic || type == NPCID.IceMimic || type == NPCID.Dandelion || type == NPCID.AngryNimbus
                  || type == NPCID.FlyingFish || type == NPCID.PirateGhost || type == NPCID.Mothron || type == NPCID.VortexHornetQueen || type == NPCID.StardustSpiderBig)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LightCheck(NPC npc)
        {
            int type = npc.type;

            if (type == NPCID.JungleSlime || type == NPCID.Derpling || type == NPCID.GreenSlime || type == NPCID.BlueSlime || type == NPCID.IceSlime || type == NPCID.Harpy || type == NPCID.IlluminantBat || type == NPCID.IlluminantSlime
                 || type == NPCID.ShimmerSlime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
