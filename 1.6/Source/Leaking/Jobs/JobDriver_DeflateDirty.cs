using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;
using Verse.Noise;
using RimWorld.QuestGen;
using rjw;
using Cumpilation.Cumflation;
using Cumpilation;
using Cumpilation.Gathering;

namespace Cumpilation.Leaking
{
    class JobDriver_DeflateDirty : JobDriver_DeflateClean
    {
        private float amountDeflated = 0f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A).Cell, job, 1, -1, null, errorOnFailed);
        }
        public override void DoDeflate()
        {
            base.DoDeflate();
            IEnumerable<FluidSource> sources = cumflationHediff.TryGetComp<HediffComp_SourceStorage>().sources;
            if (sources.TryRandomElementByWeight(source => source.amount, out FluidSource chosenFluid))
            {
                SpawnFilth(chosenFluid.fluid);
            }
            else
            {
                SpawnFilth(DefOfs.Cum);
            }
        }

        private void SpawnFilth(SexFluidDef fluid)
        {
            amountDeflated += CumflationUtility.FluidAmountRequiredToCumflatePawn(pawn, fluid) * 0.01f * Settings.LeakMult;
            FluidGatheringDef fgDef = GatheringUtility.LookupFluidGatheringDef(fluid);
            while (amountDeflated >= fgDef.fluidRequiredForOneUnit / fgDef.filthNecessaryForOneUnit)
            {
                FilthMaker.TryMakeFilth(pawn.Position, pawn.Map, fluid.filth);
                amountDeflated -= fgDef.fluidRequiredForOneUnit / fgDef.filthNecessaryForOneUnit;
            }
        }

    }
}