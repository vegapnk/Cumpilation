using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Gathering
{
    public class GatheringUtility
    {
        public static void FillFluidGatherers(SexProps props)
        {
            xxx.rjwSextype sextype = props.sexType;

            bool sexFillsCumbuckets =
                // Base: Fill Cumbuckets on Masturbation. Having no partner means it must be masturbation too
                sextype == xxx.rjwSextype.Masturbation || props.partner == null
                || sextype == xxx.rjwSextype.Boobjob || sextype == xxx.rjwSextype.Footjob || sextype == xxx.rjwSextype.Handjob;

            if (!sexFillsCumbuckets)
                return;

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
