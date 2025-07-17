using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using Cumpilation.Gathering;
using rjw;

namespace Cumpilation.Bukkake
{
    public class JobDriver_CleanSelfWithSink : JobDriver
    {
        //TODO: Make Unit-Time dependend on the Fluid 
        protected const int UNITTIME = 240;//ticks - 120 = 2 real seconds, 3 in-game minutes
        protected float progress = 0;
        protected float severitycache = 1;
        protected Hediff hediffcache;
        protected List<Hediff_CoverageController> controllers;

        protected float CleaningTime
        {
            get
            {
                return (hediffcache != null ) ? BukkakeUtility.TicksToCleanSplashFromSelf(hediffcache, globalFactor:2.5f) : severitycache * UNITTIME;
            }
        }

        protected Building FluidSink => TargetB.Thing as Building;
        protected FluidGatheringBuilding GatherBuilding;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(pawn, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(delegate
            {
                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
                return !hediffs.Exists(x => BukkakeUtility.IsSplashHediff(x));
            });
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            Toil cleaning = new Toil
            {
                initAction = CleaningInit,
                tickAction = CleaningTick,
                defaultCompleteMode = ToilCompleteMode.Never
            };
            cleaning.AddFinishAction(Finish);
            cleaning.WithProgressBar(TargetIndex.A, () => progress / CleaningTime);

            yield return cleaning;
        }

        protected void CleaningInit()
        {
            hediffcache = pawn.health.hediffSet.hediffs.Find(x => BukkakeUtility.IsSplashHediff(x) && BukkakeUtility.IsSupportedSink(x.def,FluidSink));
            controllers = pawn.health.hediffSet.hediffs.Where(c => c is Hediff_CoverageController).Cast<Hediff_CoverageController>().ToList();

            GatherBuilding = FluidSink.def.GetModExtension<FluidGatheringBuilding>();
            if (GatherBuilding == null) pawn.jobs.EndCurrentJob(JobCondition.Errored);
            if (hediffcache == null)
            {
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            }
            else
            {
                progress = 0f;
                severitycache = hediffcache.Severity;
                if (float.IsNaN(severitycache)) //TODO: Figure out WHY NaN is here
                    severitycache = 0.1f;
            }
        }

        protected void CleaningTick()
        {
            progress++;
            if (progress > CleaningTime)
            {
                Cleaned();
                controllers.ForEach(c => c.CalculateSeverity());
            }
        }

        protected void Cleaned()
        {
            if (hediffcache != null)
            {
                (SexFluidDef, float) splashItems = BukkakeUtility.SplashToFluidAmount(hediffcache.def, hediffcache.Severity, pawn.BodySize);

                hediffcache.Severity = 0f;
                
                GatheringUtility.AddFluidToSink(splashItems.Item1, splashItems.Item2, FluidSink);
            }
            CleaningInit();
        }

        protected void Finish()
        {
            if (pawn.CurJobDef == RimWorld.JobDefOf.Wait_MaintainPosture)
            {
                pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            }
        }
    }

}
