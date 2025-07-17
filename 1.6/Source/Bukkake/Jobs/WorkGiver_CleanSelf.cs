using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace Cumpilation.Bukkake
{
    public class WorkGiver_CleanSelf : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.InteractionCell;
            }
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
            }
        }

        //conditions for self-cleaning job to be available
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {

            if (pawn != t || !pawn.CanReserve(t, 1, -1, null, forced))
                return false;

            //TODO: Re-Add RJW Hero logic here?

            Hediff oldestSplash = pawn.health.hediffSet.hediffs
                .Where(x => BukkakeUtility.IsSplashHediff(x))
                .OrderByDescending(x => x.ageTicks)
                .FirstOrFallback(null);
            if (oldestSplash == null)
                return false;

            int minAge = 3 * 2500;//3 hours in-game must have passed
            if (!forced)
                if (!(oldestSplash.ageTicks > minAge))
                {
                    return false;
                }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefOfs.Cumpilation_CleanSelf);
        }
    }

}
