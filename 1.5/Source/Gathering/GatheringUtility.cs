using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace Cumpilation.Gathering
{
    public class GatheringUtility
    {
        // Maximum Distance for which nearby buildings are checked. 
        const int max_check_range = 30;

        public static void FillFluidGatherers(SexProps props)
        {
            Pawn pawn = props.pawn;

            if (pawn == null || pawn.Map == null || pawn.PositionHeld == null)
                return;
            /*
            
            */

            List<Building> gatherersInRange = FindGatherersInRangeInRoom(pawn, max_check_range: max_check_range);
            foreach(Building gatherer in gatherersInRange)
            {
                FluidGatheringBuilding ext = gatherer.def.GetModExtension<FluidGatheringBuilding>();
                ModLog.Debug($"Triggering Behaviour for {gatherer.def.defName}@{gatherer.PositionHeld} (has {ext})");
            }

            // Enumerable throws System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
                /*
                List<Building_FluidGatherer> NearbyGatherers = props.pawn.GetAdjacentBuildings<Building_FluidGatherer>().ToList();

                if (NearbyGatherers?.Count > 0)
                {
                    //var initialCum = GetCumVolume(props.pawn);
                    foreach (Building_FluidGatherer gatherer in NearbyGatherers)
                    {
                        //gatherer.AddCum(initialCum / NearbyGatherers.Count);
                    }
                }

                //TODO: Fill by "fallback" if I do not have a fluid, but there might be a thingdef in the base-fluid
                */
        }

        public static void HandleFluidGatherer(Building gatherer, SexProps props, int numberOfOtherBuildings=0)
        {

            if (!(gatherer is RimWorld.Building_Storage))
            {
                ModLog.Debug($"The Gatherer {gatherer.def.defName}@{gatherer.PositionHeld} trying to gather Cum is not a Rimworld.Building_Storage");
            }

            FluidGatheringBuilding ext = gatherer.def.GetModExtension<FluidGatheringBuilding>();


        }


        public static bool IsSexWithFluidFlyingAround(SexProps props)
        {
            xxx.rjwSextype sextype = props.sexType;
            bool fluidFlyingAroundSexType =
                // Base: Fill Cumbuckets on Masturbation. Having no partner means it must be masturbation too
                sextype == xxx.rjwSextype.Masturbation || props.partner == null
                || sextype == xxx.rjwSextype.Boobjob || sextype == xxx.rjwSextype.Footjob || sextype == xxx.rjwSextype.Handjob;
            return fluidFlyingAroundSexType;
            //TODO: Return true if there is a heavy missmatch of fluid-amount and body-size, or on full cumflation etc. 
            //return false;
        }

        public static List<Building> FindGatherersInRangeInRoom(Pawn pawn, int max_check_range)
        {
            List<Building> results = new List<Building>();
            int cells = 0, buildings = 0, gatherers = 0;

            foreach (var cell in pawn.GetRoom().Cells)
            {
                if (cell == null) continue;
                if (!pawn.PositionHeld.InHorDistOf(cell, max_check_range)) continue;
                cells++;

                Building building = cell.GetFirstBuilding(pawn.Map);
                if (building == null) continue;
                buildings++;

                FluidGatheringBuilding ext = building.def.GetModExtension<FluidGatheringBuilding>();
                if (ext == null) continue;
                gatherers++;
                results.Add(building);
            }

            ModLog.Debug($"Found {cells} cells in range {max_check_range} with {gatherers} gatherers of {buildings} buildings");
            return results;
        }

        public static void PrintFluidDefInfo()
        {
            IEnumerable<FluidGatheringDef> defs = DefDatabase<FluidGatheringDef>.AllDefs;
            ModLog.Debug($"Found {defs.Count()} FluidGatheringDefs.");
            foreach (FluidGatheringDef def in defs)
            {
                ModLog.Debug($"\tFluidGatheringDef {def.defName}: {def.fluidRequiredForOneUnit} {def.fluidDef} => 1 {def.thingDef}");
            }
        }
    }
}
