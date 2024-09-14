using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class IngestionOutcomeDoer_PermanentFluidChange : IngestionOutcomeDoer_PartTargetting
    {

        SexFluidDef fluid;

        protected bool wasApplied = false; // internal note to enable the "Ingestion With Penalty on Failure"

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (pawn == null) return;
            ModLog.Debug($"{pawn} is consuming {ingestedCount} counts of {ingested}");

            foreach (ISexPartHediff sexPart in GetSexPartHediffs(pawn))
            {
                if (sexPart.GetPartComp().Fluid == fluid)
                    continue;
                
                sexPart.GetPartComp().Fluid = fluid;
                wasApplied = true;
            }
        }
    }

}
