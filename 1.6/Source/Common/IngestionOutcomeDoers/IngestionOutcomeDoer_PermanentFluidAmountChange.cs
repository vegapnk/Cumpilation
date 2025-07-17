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
    public class IngestionOutcomeDoer_PermanentFluidAmountChange : IngestionOutcomeDoer_PartTargetting
    {
        float multiplier = 1.0f;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (pawn == null) return;
            ModLog.Debug($"{pawn} is consuming {ingestedCount} counts of {ingested}");

            foreach (ISexPartHediff part in GetSexPartHediffs(pawn)){
                part.GetPartComp().partFluidMultiplier *= multiplier;
            }

        }
    }
}
