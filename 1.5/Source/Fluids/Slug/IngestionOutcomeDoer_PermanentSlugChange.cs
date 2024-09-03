using Cumpilation.Common;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Fluids.Slug
{
    public class IngestionOutcomeDoer_PermanentSlugChange : IngestionOutcomeDoer_PermanentFluidChange
    {

        public float nonApplicationPenalityBuildUp = 0.5f;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            base.DoIngestionOutcomeSpecial(pawn,ingested,ingestedCount);

            if (!wasSuccessfullyApplied)
            {
                ModLog.Debug($"IngestionOutcomeDoer_PermanentSlugChange was not successfully applied for {pawn}, adding a penalty ToxicBuildup with severity {nonApplicationPenalityBuildUp}");
                Hediff toxicBuildup = GetOrCreateToxicBuildUpHediff(pawn);
                toxicBuildup.Severity += nonApplicationPenalityBuildUp;
            }
        }

        private Hediff GetOrCreateToxicBuildUpHediff(Pawn pawn) {

            Hediff toxicBuildup = pawn.health.hediffSet.GetFirstHediffOfDef(RimWorld.HediffDefOf.ToxicBuildup);
            if (toxicBuildup == null)
            {
                toxicBuildup = HediffMaker.MakeHediff(RimWorld.HediffDefOf.ToxicBuildup, pawn);
                toxicBuildup.Severity = 0;
                pawn.health.AddHediff(toxicBuildup);
            }
            return toxicBuildup;
        }
    }
}
