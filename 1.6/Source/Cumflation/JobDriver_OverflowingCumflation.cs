using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
        Random random = new Random();
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            orientation = GetRandomOrientation();
            Hediff cumflationHediff_for_debug = CumflationUtility.GetOrCreateCumflationHediff(pawn);
            ModLog.Debug($"{this.pawn} got over-inflated and has Cumflation Severity {cumflationHediff_for_debug.Severity} - spraying cum until reaching severity 1.0");
            ModLog.Debug($"\tSpraying from {this.pawn}@{this.pawn.PositionHeld} towards {orientation}");

            int spawned_filth = 0;

            var toil = new Toil();
            toil.tickAction = delegate ()
            {
                if (ShouldEnd()) {
                    ModLog.Debug($"Finished overflowing for {this.pawn} - severity is back down to 1.0. Spawned a total of {spawned_filth} filth.");
                    base.ReadyForNextToil();
                    // TODO: TaleRecorder.RecordTale
                }
                else
                {
                    if (pawn.IsHashIntervalTick(75))
                    {
                        //ModLog.Debug("Spawning Filth from Emptying Cumflation");
                        SpawnFilth();
                        Hediff cumflationHediff = CumflationUtility.GetOrCreateCumflationHediff(pawn);
                        cumflationHediff.Severity -= 0.075f;
                        spawned_filth++;
                    }
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.PlaySustainerOrSound(() => SoundDefOf.FleshmassBirth);
            yield return toil;
        }

        private bool ShouldEnd()
        {
            Hediff cumflationHediff = CumflationUtility.GetOrCreateCumflationHediff(pawn);
            return cumflationHediff == null || cumflationHediff.Severity <= 1.0;
        }

        private static int getGenitalBPR(Pawn pawn)
        {
            BodyPartRecord genital_bodypart = Genital_Helper.get_genitalsBPR(pawn);
            var genital_id = genital_bodypart.Index; 
            return genital_id;
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

        private IntVec3 GetRandomNearbyPosition(int maxDistance = 3, int retries = 6)
        {
            IntVec3 offset = orientation * random.Next(0, maxDistance);
            IntVec3 positionCandidate = pawn.PositionHeld + offset;
            if (retries <= 0) { 
                // In case we are unable to spawn things for too many times, we just put it below the pawn. That should always be fine. 
                return positionCandidate;
            }
            // Quick Check whether the cell is in the room, not to spawn through walls etc. 
            if (pawn.GetRoom().ContainsCell(positionCandidate))
                return positionCandidate;
            else 
                return GetRandomNearbyPosition(maxDistance, retries-1);
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
