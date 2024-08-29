using RimWorld;
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
                HandleFluidGatherer(gatherer, props, gatherersInRange.Count-1);
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

            var relevantGenitals = GetGenitalsWithFluids(props.pawn, true);

            foreach (Hediff genital in relevantGenitals)
            {
                HediffDef_SexPart sexPart = GetHediffDefSexPart(genital);
                if (ext.supportedFluids.Contains(sexPart.fluid))
                {
                    FluidGatheringDef fgDef = LookupFluidGatheringDef(sexPart.fluid);
                    // Case 1: There is a FluidGatheringDef - This will have highest Priority
                    if (fgDef != null) {
                        ModLog.Message("Found a fitting gatherer for the right fluid - and it has a FluidGatheringDef");
                        // TODO
                    } 
                    // Case 2: There is no FluidGatheringDef, but there is a Consumable Def
                    else if (sexPart.fluid.consumable != null)
                    {

                    }
                    // Case 3: Nothing is given, Nothing will happen.
                    else
                    {
                        // Maybe Print here?
                    }
                }
                else
                {
                    ModLog.Debug($"{gatherer} tried to gather {sexPart.fluid} but was unsupported - continuing.");
                }
            }
        }

        public static FluidGatheringDef? LookupFluidGatheringDef(SexFluidDef def)
        {
            if (def == null) return null;

            IEnumerable<FluidGatheringDef> defs = DefDatabase<FluidGatheringDef>.AllDefs;
            return defs.FirstOrFallback(fgDef => fgDef.fluidDef == def);
        }

        public static List<Hediff> GetGenitalsWithFluids(Pawn pawn, bool filterForShootsOnOrgasm = false)
        {
            List<Hediff> results = new List<Hediff>();
            var all_parts  = rjw.Genital_Helper.get_AllPartsHediffList(pawn);

            ModLog.Debug($"Found {all_parts.Count} parts for {pawn}");
            foreach(var part in all_parts)
            {
                var def = GetHediffDefSexPart(part);
                if (def == null) continue;
                if (def.fluid == null) continue;
                if (filterForShootsOnOrgasm && !def.produceFluidOnOrgasm) continue;

                results.Add(part);
            }
            return results;
        } 

        public static HediffDef_SexPart? GetHediffDefSexPart(Hediff hediff)
        {
            if (hediff == null) return null;
            if (hediff.def is HediffDef_SexPart)
                return (HediffDef_SexPart)(hediff.def);
            return null;
        } 

        /*
        public void AddCum(float amount, ThingDef cumDef)
        {
            Thing cum = ThingMaker.MakeThing(cumDef);
            AddCum(amount, cum);
        }

        public void AddCum(float amount, Thing cum)
        {
            storedDecimalRemainder += amount;
            totalGathered += amount;
            int num = (int)storedDecimalRemainder;

            cum.stackCount = num;
            if (cum.stackCount > 0 && !GenPlace.TryPlaceThing(cum, PositionHeld, Map, ThingPlaceMode.Direct, out Thing res))
            {
                FilthMaker.TryMakeFilth(PositionHeld, Map, RsDefOf.Thing.FilthCum, num);
            }
            storedDecimalRemainder -= num;
        }
        */

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
                if (building.PositionHeld.InHorDistOf(pawn.PositionHeld, ext.range))
                {
                    gatherers++;
                    results.Add(building);
                }
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
