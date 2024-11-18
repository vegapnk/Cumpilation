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
    class JobDriver_DeflateBucket : JobDriver_DeflateClean
    {
        private float amountDeflated = 0f;

        public override void DoDeflate()
        {
            base.DoDeflate();
            IEnumerable<FluidSource> sources = cumflationHediff.TryGetComp<HediffComp_SourceStorage>().sources;
            if (sources.TryRandomElementByWeight(source => source.amount, out FluidSource chosenFluid))
            {
                SpawnCum(chosenFluid.fluid);
            }
            else
            {
                SpawnCum(DefOfs.Cum);
            }
        }

        private void SpawnCum(SexFluidDef fluid)
        {
            float deflateMult = TargetA.Thing?.TryGetComp<Comp_DeflateBucket>()?.deflateMult ?? 1f;
            amountDeflated += CumflationUtility.FluidAmountRequiredToCumflatePawn(pawn, fluid) * 0.01f * deflateMult * Settings.DeflateMult;
            FluidGatheringDef fgDef = GatheringUtility.LookupFluidGatheringDef(fluid);
            while (amountDeflated >= fgDef.fluidRequiredForOneUnit)
            {
                Thing thing = ThingMaker.MakeThing(fgDef.thingDef);
                GenPlace.TryPlaceThing(thing, job.GetTarget(TargetIndex.A).Cell , pawn.Map, ThingPlaceMode.Near);
                pawn.Reserve(thing, job, 1, -1, null);
                amountDeflated -= fgDef.fluidRequiredForOneUnit;
            }
        }

    }
}