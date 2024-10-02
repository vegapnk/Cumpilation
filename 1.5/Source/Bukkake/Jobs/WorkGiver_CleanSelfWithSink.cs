using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using Cumpilation.Gathering;

namespace Cumpilation.Bukkake
{
    public class WorkGiver_CleanSelfWithSink : WorkGiver_Scanner
    {

        //public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForUndefined();

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {

            DefDatabase<ThingDef>.AllDefs.Where(def => def.HasModExtension<FluidGatheringBuilding>());
            
            return pawn.Map.spawnedThings.Where(thing => thing.def.HasModExtension<FluidGatheringBuilding>());

            return base.PotentialWorkThingsGlobal(pawn);
        }

        public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !pawn.health.hediffSet.hediffs.Any(hed => BukkakeUtility.IsSplashHediff(hed));
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is Building potentialSink)
                if (!GatheringUtility.IsFluidSink(potentialSink))
                    return false;
                else
                {
                    return BukkakeUtility.SplashesOnPawn(pawn)
                        .Any(hed => BukkakeUtility.IsSupportedSink(hed.def, potentialSink));
                    //TODO: Check also on the "is sink already full"
                }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefOfs.Cumpilation_CleanSelfWithSink, pawn, t);
        }

        public ThingRequest RequestSink()
        {
            return ThingRequest.ForUndefined();
        }
        
    }

}
