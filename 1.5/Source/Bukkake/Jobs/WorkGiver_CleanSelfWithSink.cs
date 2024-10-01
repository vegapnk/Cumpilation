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
        //public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.);
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

            return true;
            //return bucket.StoredStackCount < ThingDefOf.GatheredCum.stackLimit;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefOfs.Cumpilation_CleanSelfWithSink, pawn, t);
        }
    }

}
