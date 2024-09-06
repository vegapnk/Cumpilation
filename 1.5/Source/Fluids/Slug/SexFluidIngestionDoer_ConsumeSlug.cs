using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Cumpilation.Fluids.Slug
{
    /// <summary>
    /// Adds up to MAX_SEVERITY (Default=0.2f) Toxic Buildup to the receiver upon consuming Slug. 
    /// </summary>
    public class SexFluidIngestionDoer_ConsumeSlug : SexFluidIngestionDoer
    {

        const float MAX_SEVERITY = 0.2f;
        const float FLUID_FOR_BODYSIZE_ONE = 50f;

        public override void Ingested(Pawn pawn, SexFluidDef fluid, float amount, ISexPartHediff fromPart, ISexPartHediff toPart = null)
        {
            if (pawn == null) return;
            if (amount <= 0 || pawn.BodySize <= 0) return;
            if (fluid == null || !fluid.defName.ToLower().Contains("slug")) return;

            float severity = Math.Min(MAX_SEVERITY,amount / (FLUID_FOR_BODYSIZE_ONE * pawn.BodySize));

            Hediff toxicBuildup = SlugUtility.GetOrCreateToxicBuildUpHediff(pawn);
            toxicBuildup.Severity += severity;
            ModLog.Debug($"{pawn} ingested {amount}{fluid.defName} - resulting in +{severity} Toxic-Buildup (capped at {MAX_SEVERITY})");
        }

    }
}
