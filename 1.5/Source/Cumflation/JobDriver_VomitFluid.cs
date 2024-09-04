using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Cumpilation.Cumflation
{
    public class JobDriver_VomitFluid : JobDriver_Vomit
    {
        public override bool CanBeginNowWhileLyingDown()
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                this.ticksLeft = (new System.Random()).Next(300, 600);
                int num = 0;
                IntVec3 c;
                for (; ; )
                {
                    c = this.pawn.Position + GenAdj.AdjacentCellsAndInside[Rand.Range(0, 9)];
                    num++;
                    if (num > 12)
                    {
                        break;
                    }
                    if (c.InBounds(this.pawn.Map) && c.Standable(this.pawn.Map))
                    {
                        this.job.targetA = c;
                        this.pawn.pather.StopDead();
                    }
                }
                c = this.pawn.Position;
            };

            toil.tickAction = delegate ()
            {
                if (this.ticksLeft % 150 == 149)
                {
                    ThingDef filth = LookupRandomFilthDef(pawn);
                    if (filth != null && this.job.targetA.Cell != null && this.pawn.Map != null)
                        FilthMaker.TryMakeFilth(this.job.targetA.Cell, pawn.Map,filth);
                }
                this.ticksLeft--;
                if (this.ticksLeft <= 0)
                {
                    base.ReadyForNextToil();
                    //TODO: Add TaleDef
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.WithEffect(EffecterDefOf.Vomit, TargetIndex.A, new Color(100f, 100f, 100f, 0.5f));
            toil.PlaySustainerOrSound(() => SoundDefOf.Vomit, 1f);
            yield return toil;
            yield break;
        }

        private int ticksLeft;

        /// <summary>
        /// Checks the pawns "Stuffed" Hediffs and returns a fitting FilthDef. 
        /// If there is none, it'll return the normal Filth_Vomit.
        /// </summary>
        /// <param name="pawn">The Pawn which has stuffing-hediffs</param>
        /// <returns>A filth fitting for the existing stuffing-hediffs, or a Filth_Vomit as fallback.</returns>
        protected static ThingDef LookupRandomFilthDef(Pawn pawn)
        {
            if (pawn == null) return null;

            IEnumerable<Hediff> stuffingHediffs = StuffingUtility.GetAllSharedSeverityHediffsInPawn(pawn);
            if (stuffingHediffs == null || stuffingHediffs.Count() == 0)
                return null;

            Hediff randomChosenHediff = stuffingHediffs.RandomElementByWeight(hed => hed.Severity);
            if (randomChosenHediff == null) return null;

            StuffingDef stuffDef = DefDatabase<StuffingDef>.AllDefs
                .FirstOrFallback(stuffingDef => stuffingDef.resultingStuffedHediff == randomChosenHediff.def);

            ThingDef filthDef = stuffDef != null ? stuffDef.fluid.filth : RimWorld.ThingDefOf.Filth_Vomit;

            return filthDef;
        }

    }
}
