using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Cumpilation.Cumflation
{
    /// <summary>
    /// - Stop the pawn flat / make it look like it 
    /// - Spray correct outside of it
    /// - Spray until the cumflation is at <1.0
    /// </summary>
    public class JobDriver_OverflowingCumflation : JobDriver
    {
        IntVec3 orientation = new IntVec3();
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            orientation = GetRandomOrientation();

            var toil = new Toil();
            toil.tickAction = delegate ()
            {
                if (ShouldEnd()) { 
                    base.ReadyForNextToil();
                    // TODO: TaleRecorder.RecordTale
                }
                else
                {
                    if (pawn.IsHashIntervalTick(50))
                    {
                        //ModLog.Debug("Spawning Filth from Emptying Cumflation");
                        SpawnFilth();
                        Hediff cumflationHediff = CumflationUtility.GetOrCreateCumflationHediff(pawn);
                        cumflationHediff.Severity -= 0.075f;
                    }
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            //TODO: Progress Bar could be funny, lol
            //toil.WithProgressBar
            toil.PlaySustainerOrSound(() => SoundDefOf.FleshmassBirth);
            yield return toil;
        }

        private bool ShouldEnd()
        {
            Hediff cumflationHediff = CumflationUtility.GetOrCreateCumflationHediff(pawn);
            return cumflationHediff == null || cumflationHediff.Severity <= 1.0;
        }

        private void SpawnFilth()
        {
            Hediff cumflationHediff = CumflationUtility.GetOrCreateCumflationHediff(pawn);

            IEnumerable<FluidSource> sources = cumflationHediff.TryGetComp<HediffComp_SourceStorage>().sources;
            FluidSource chosenFluid = sources.RandomElementByWeight(source => source.amount);
            if (chosenFluid.fluid.filth == null) { return; }

            // Currently, the Hediff can have atmost 3 Severity. This should also be the Max-Distance.
            int maxDistance = (int) ((cumflationHediff.Severity - 1.0f) / 0.5f);
            IntVec3 filthPosition = GetRandomNearbyPosition(maxDistance);

            FilthMaker.TryMakeFilth(filthPosition, pawn.Map, chosenFluid.fluid.filth);
        }

        private IntVec3 GetRandomNearbyPosition(int maxDistance = 3)
        {
            IntVec3 offset = orientation * (new Random()).Next(0, maxDistance);
            IntVec3 positionCandidate = pawn.PositionHeld + offset;
            // Quick Check whether the cell is in the room, not to spawn through walls etc. 
            if (pawn.GetRoom().ContainsCell(positionCandidate))
                return positionCandidate;
            else 
                return GetRandomNearbyPosition(maxDistance);
        }

        public override bool CanBeginNowWhileLyingDown() => true;

        private IntVec3 GetRandomOrientation()
        {
            var orientations = new List<IntVec3>()
            {
                new IntVec3(-1,0,-1),
                new IntVec3(-1,0, 1),
                new IntVec3(1,0,-1),
                new IntVec3(1,0,1),
            };
            return orientations.RandomElement();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<IntVec3>(ref this.orientation, "orientation");
        }

    }
}
