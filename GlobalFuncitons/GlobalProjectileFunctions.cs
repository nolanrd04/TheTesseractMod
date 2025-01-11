using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TheTesseractMod.GlobalFuncitons
{
    internal class GlobalProjectileFunctions
    {
        public static NPC findSecondClosestTarget(Vector2 projectileCenter) // returns the second closest npc
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC && npc != findClosestTarget(projectileCenter) && npc != null)
                {
                    float distance = Vector2.Distance(projectileCenter, npc.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }
            }
            if (closestNPCIndex >= 0)
            {
                return Main.npc[closestNPCIndex];
            }
            else
            {
                return null;
            };
        }

        public static NPC findClosestTargetInRange(Vector2 projectileCenter, float range) // returns the closest npc in range
        {
            int closestNPCIndex = -1;
            float closestDistance = range;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC && npc != null && npc.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(projectileCenter, npc.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }
            }
            if (closestNPCIndex >= 0)
            {
                return Main.npc[closestNPCIndex];
            }
            else
            {
                return null;
            }

        }


        public static NPC findClosestTarget(Vector2 projectileCenter) // returns the closest npc
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.townNPC && npc != null)
                {
                    float distance = Vector2.Distance(projectileCenter, npc.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }
            }
            if (closestNPCIndex >=0)
            {
                return Main.npc[closestNPCIndex];
            }
            else
            {
                return null;
            }
            
        }
        public static NPC findClosestTarget(Vector2 projectileCenter, NPC lastHit) // returns the closest npc without taking into account the last hit NPC
        {
            int closestNPCIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (lastHit != null && npc.active && !npc.townNPC)
                {
                    if (IsTargetValid(npc, projectileCenter, 3000f, lastHit))
                    {
                        float distance = Vector2.Distance(projectileCenter, npc.position);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestNPCIndex = i;
                        }
                    }
                }
                else
                {
                    if (IsTargetValid(npc, projectileCenter, float.MaxValue))
                    {
                        float distance = Vector2.Distance(projectileCenter, npc.position);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestNPCIndex = i;
                        }
                    }
                }
                
            }
            if (closestNPCIndex >= 0)
            {
                return Main.npc[closestNPCIndex];
            }
            else
            {
                return null;
            }
        }
        public static NPC findClosestTargetInRange(Vector2 projectileCenter, NPC lastHit, float range) // returns the closest npc without taking into account the last hit NPC
        {
            int closestNPCIndex = -1;
            float closestDistance = range;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (lastHit == null)
                {
                    if (IsTargetValid(npc, projectileCenter, range))
                    {
                        float distance = Vector2.Distance(projectileCenter, npc.position);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestNPCIndex = i;
                        }
                    }
                }
                else
                {
                    if (IsTargetValid(npc, projectileCenter, range, lastHit))
                    {
                        float distance = Vector2.Distance(projectileCenter, npc.position);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestNPCIndex = i;
                        }
                    }
                }

            }
            if (closestNPCIndex >= 0)
            {
                return Main.npc[closestNPCIndex];
            }
            else
            {
                return null;
            }
        }


        public static bool IsTargetValid(NPC target, Vector2 projectileCenter, float range) // a check to make sure the target exists
        {
            return target != null && target.active && !target.friendly && (Vector2.Distance(projectileCenter, target.Center) < range) && target.CanBeChasedBy();
        }
        public static bool IsTargetValid(NPC target, Vector2 projectileCenter, float range, NPC lastHit) // a check to make sure the target exists and is not the last hit NPC
        {
            return target != null && target.active && !target.friendly && (Vector2.Distance(projectileCenter, target.Center) < range) && target != lastHit && target.CanBeChasedBy();
        }
    }
}
