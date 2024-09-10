using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;
using Cumpilation.Common;

namespace Cumpilation.Gathering
{
    public class GatheringUtility
    {
        
        public static void FillFluidGatherers(SexProps props)
        {
            Pawn pawn = props.pawn;

            if (pawn == null || pawn.Map == null || pawn.PositionHeld == null)
                return;

            if (!FluidUtility.IsSexWithFluidFlyingAround(props))
                return; 

            List<Building> gatherersInRange = FindGatherersInRangeInRoom(pawn, max_check_range: (int) Settings.MaxGatheringCheckDistance);

            foreach(Building gatherer in gatherersInRange)
            {
                FluidGatheringBuilding ext = gatherer.def.GetModExtension<FluidGatheringBuilding>();
                ModLog.Debug($"Triggering Behaviour for {gatherer.def.defName}@{gatherer.PositionHeld} (has {ext})");
                HandleFluidGatherer(gatherer, props, gatherersInRange.Count-1);
            }
        }

        // TODO update with different number of collectors ... 
        // Split "load" on all that are free. Right now we multiply.
        public static void HandleFluidGatherer(Building gatherer, SexProps props, int numberOfOtherBuildings=0)
        {

            if (!(gatherer is RimWorld.Building_Storage))
            {
                ModLog.Debug($"The Gatherer {gatherer.def.defName}@{gatherer.PositionHeld} trying to gather Cum is not a Rimworld.Building_Storage");
            }

            FluidGatheringBuilding ext = gatherer.def.GetModExtension<FluidGatheringBuilding>();

            var relevantGenitals = FluidUtility.GetGenitalsWithFluids(props.pawn, true);

            foreach (Hediff genital in relevantGenitals)
            {
                ISexPartHediff sexPartHediff = (ISexPartHediff)genital;
                var fluid = sexPartHediff.GetPartComp().Fluid;

                if (ext.supportedFluids.Contains(fluid))
                {
                    FluidGatheringDef fgDef = LookupFluidGatheringDef(fluid);
                    float fluidAmount = sexPartHediff.GetPartComp().FluidAmount;
                    if (numberOfOtherBuildings > 0)
                    {
                        fluidAmount = fluidAmount / numberOfOtherBuildings + 1;
                    }
                    

                    // Case 1: There is a FluidGatheringDef - This will have highest Priority
                    if (fgDef != null) {
                        ModLog.Debug($"Found a fitting gatherer for {fluid} - and it has a FluidGatheringDef {fgDef.defName}");

                        int toMake = (int) Math.Round( fluidAmount/fgDef.fluidRequiredForOneUnit + (fgDef.roundUp?0.5: 0), 0);
                        toMake = fgDef.canProduceMoreThanOne ? toMake : 1; 
                        Thing gatheredFluid = ThingMaker.MakeThing(fgDef.thingDef);
                        gatheredFluid.stackCount = toMake;
                        GenPlace.TryPlaceThing(gatheredFluid, gatherer.PositionHeld, gatherer.Map, ThingPlaceMode.Direct, out Thing res);
                    } 
                    // Case 2: There is no FluidGatheringDef, but there is a Consumable Def
                    else if (fluid.consumable != null)
                    {
                        /// Relevant RJW File: https://gitgud.io/Ed86/rjw/-/blob/Dev/1.5/Defs/SexFluidDefs/Example.xml?ref_type=heads
                        /// States: "consumableFluidRatio = <!-- Number of consumables to ingest per unit fluid. -->
                        /// So, with a rate of 1, 10 fluid will result in 10 ingestions. With a ratio of 0.5, 10 fluid will result in 5 etc. 
                        ModLog.Debug($"Found a fitting gatherer for {fluid} - but no FluidGatheringDef. Falling back to {fluid.consumable.defName}");

                        
                        int toMake = (int)Math.Round(fluid.consumableFluidRatio * fluidAmount, 0);
                        Thing gatheredFluid = ThingMaker.MakeThing(fluid.consumable);
                        gatheredFluid.stackCount = toMake;
                        GenPlace.TryPlaceThing(gatheredFluid, gatherer.PositionHeld, gatherer.Map, ThingPlaceMode.Direct, out Thing res);
                    }
                    // Case 3: Nothing is given, Nothing will happen.
                    else
                    {
                        // Maybe Print here?
                    }
                }
                else
                {
                    ModLog.Debug($"{gatherer} tried to gather {fluid} but was unsupported - continuing.");
                }
            }
        }

        public static List<Building> FindGatherersInRangeInRoom(Pawn pawn, int max_check_range)
        {
            List<Building> results = new List<Building>();
            int cells = 0, buildings = 0, gatherers = 0;

            if (max_check_range <= 3)
            {
                ModLog.Warning($" `FindGatherersInRangeInRoom` Received a very little check-range for fining gatherers --- max_check_range = {max_check_range}");
            }

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

        public static FluidGatheringDef? LookupFluidGatheringDef(SexFluidDef def)
        {
            if (def == null) return null;

            IEnumerable<FluidGatheringDef> defs = DefDatabase<FluidGatheringDef>.AllDefs;
            return defs.FirstOrFallback(fgDef => fgDef.fluidDef == def);
        }


        public static FluidGatheringDef? LookupGatheringDef(ThingDef filth)
        {
            return DefDatabase<FluidGatheringDef>.AllDefs
                .Where(def => def.canBeRetrievedFromFilth)
                .Where(def => def.fluidDef.filth == filth || def.filth == filth)
                .FirstOrFallback();
        }


        public static void PrintFluidGatheringDefInfo()
        {
            IEnumerable<FluidGatheringDef> defs = DefDatabase<FluidGatheringDef>.AllDefs;
            ModLog.Debug($"Found {defs.Count()} FluidGatheringDefs.");
            foreach (FluidGatheringDef def in defs)
            {
                ModLog.Debug($"\tFluidGatheringDef {def.defName}: {def.fluidRequiredForOneUnit} {def.fluidDef} => 1 {def.thingDef}");
            }
        }

        public static bool CanStore(RimWorld.Building_Storage storage, SexFluidDef fluid)
        {
            // From Discord:
            // took me a moment, but from the looks of it
            // Building.MaxItemsInCell to get the capacity, and compare against GridsUtility.GetItemCount
            // (or GridsUtility.GetItems for the actual things in there)

            // There is a building_storage.Accepts(thing thing) which might help
            return false;
        }


        /// <summary>
        /// Returns a list of all Filths that are within a given range, and within the same room of this Comps parent.
        /// When `onlyFluidFilth` is true, only filth that is specified in a rjw.SexFluidDef will be considered. 
        /// </summary>
        /// <param name="onlyFluidFilth">If true, only Filth that is mentioned in rjw.SexFluidDefs are returned</param>
        /// <param name="range">Filter for the horicontal distance to the parent.</param>
        /// <returns>A list of Fitlhs in the same room of the parent, filtered for SexFluids and/or range.</returns>
        public static IEnumerable<Filth> GetNearbyFilth(Thing origin, bool onlyFluidFilth = true, int range = 50)
        {
            var results = new List<Filth>();

            if (origin.Map == null) return results;
            if (origin.GetRoom() == null) return results;
            if (origin.PositionHeld == null) return results;
            if (origin.IsBrokenDown()) return results;

            var room = origin.GetRoom();
            var knownFluids = FluidUtility.GetAllSexFluidDefs();

            // DevNote: I first was checking the cells of the room, but I checked the Cleaning Methods from BaseRW and they use the CotnainedAndAdjacentThings. 
            // So I went with that too.
            return room.ContainedAndAdjacentThings
                .OfType<Filth>()
                .Cast<Filth>()
                .Where(filth => filth.PositionHeld.InHorDistOf(origin.PositionHeld, range))
                .Where(filth => !onlyFluidFilth || knownFluids.Any(fluid => fluid.filth == filth.def));
        }

        /// <summary>
        /// Small helper that checks for a filth if the Gatherer can handle is. 
        /// This consist of checking if the filth is in a (supported) fluidDef, 
        /// or if there is a filth specified in a gatheringDef. 
        /// </summary>
        /// <param name="filth"></param>
        /// <returns></returns>
        public static bool IsSupportedFilthType(Filth filth, IEnumerable<SexFluidDef> supportedFluids)
        {
            bool isInFluidDefsAsFilth = supportedFluids.Select(fluid => fluid.filth).Any(f => f == filth.def);
            //TODO: Make this check a bit better if the gathering-defs fluid is also supported ...
            bool isInFluidGatheringDefsAsBackup = DefDatabase<FluidGatheringDef>.AllDefs
                .Where(def => def.canBeRetrievedFromFilth && def.filth != null)
                .Select(def => def.filth)
                .Any(f => f == filth.def);
            return isInFluidDefsAsFilth || isInFluidGatheringDefsAsBackup;
        }

    }
}
