using System;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Cumpilation.Cumflation;
using Cumpilation.Gathering;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using rjw;

namespace Cumpilation.Leaking
{
    public class Recipe_ExtractCum : Recipe_Surgery
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (thing is Pawn pawn && !(CumflationUtility.GetOrCreateCumflationHediff(pawn).Severity > 0f))
            {
                return false;
            }
            return base.AvailableOnNow(thing, part);
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Hediff cumflationHediff = CumflationUtility.GetOrCreateCumflationHediff(pawn);
            if (cumflationHediff?.Severity > 0 && billDoer != null)
            {
                List<FluidSource> sources = cumflationHediff.TryGetComp<HediffComp_SourceStorage>()?.sources;
                if (!sources.NullOrEmpty())
                {
                    float amountTotal = 0f;
                    foreach (FluidSource source in sources)
                    {
                        amountTotal += source.amount;
                    }
                    foreach (FluidSource source in sources)
                    {
                        SpawnCum(pawn, billDoer, source.fluid, cumflationHediff.Severity * (source.amount / amountTotal));
                    }
                }
                else
                {
                    SpawnCum(pawn, billDoer, DefOfs.Cum, cumflationHediff.Severity);
                }
                pawn.health.RemoveHediff(cumflationHediff);
            }
        }

        private void SpawnCum(Pawn pawn, Pawn billDoer, SexFluidDef fluid, float severity)
        {
            float fluidAmount = CumflationUtility.FluidAmountRequiredToCumflatePawn(pawn, fluid) * severity * Settings.DeflateMult;
            FluidGatheringDef fgDef = GatheringUtility.LookupFluidGatheringDef(fluid);
            Thing thing = ThingMaker.MakeThing(fgDef.thingDef);
            thing.stackCount = (int)(fluidAmount / fgDef.fluidRequiredForOneUnit);
            GenPlace.TryPlaceThing(thing, billDoer.PositionHeld, billDoer.MapHeld, ThingPlaceMode.Near);
        }
    }
}