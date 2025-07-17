using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using Cumpilation.Gathering;
using rjw;
using Verse.Noise;
using System.Net.Sockets;

namespace Cumpilation.Bukkake
{
    internal class JobGiver_CleanSelfWithSink : ThinkNode_JobGiver
    {

        public const float MAX_DISTANCE_TO_BUCKET = 25.0f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            ModLog.Debug($"Trying to Give Cumpilation_CleanSelfWithSink to {pawn}");
            var splashes = pawn.health.hediffSet.hediffs.Where(x => BukkakeUtility.IsSplashHediff(x));

            if (splashes.Count() == 0 ) return null;

            IEnumerable<SexFluidDef> fluids = splashes
                .Select(splash => BukkakeUtility.SplashToFluidAmount(splash.def, splash.Severity, pawn.BodySize).Item1)
                .InRandomOrder();

            ModLog.Debug($"Found {splashes.Count()} on {pawn}");
            foreach (SexFluidDef fluid in fluids)
            {
                var sink = GatheringUtility.FindClosestFluidSink(pawn, fluid);
                if (sink == null || !GatheringUtility.IsFluidSinkFor(sink, fluid)) continue;
                if (pawn.Position.DistanceTo(sink.Position) > MAX_DISTANCE_TO_BUCKET) continue;

                ModLog.Debug($"Had a nearby sink {sink} for {fluid} near {pawn}, trying to Make Job");

                return JobMaker.MakeJob(DefOfs.Cumpilation_CleanSelfWithSink, pawn, sink);
            }

            return null;
        }
    }

}
