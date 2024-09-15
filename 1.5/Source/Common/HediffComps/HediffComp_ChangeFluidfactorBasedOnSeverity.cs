using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    /// <summary>
    /// Adjusts the targetted SexParts fluid-amount based on a linear factor between a configured Max and Min value. 
    /// 
    /// Example: Max 5, Min 2
    /// Severity 100% -> 5.0
    /// Severity 50%  -> 3.5
    /// Severity 0%   -> 2.0
    /// Severity 33%  -> 3.0
    /// Severity 66%  -> 4.0
    /// Severity 250% -> 5.0
    /// </summary>
    public class HediffComp_ChangeFluidfactorBasedOnSeverity : HediffComp
    {

        protected float lastMultiplier = float.MinValue;

        public new HediffCompProperties_ChangeFluidfactorBasedOnSeverity Props => (HediffCompProperties_ChangeFluidfactorBasedOnSeverity)this.props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (parent.pawn.IsHashIntervalTick(2000)) {
                foreach (ISexPartHediff sexPart in Props.GetSexPartHediffs(parent.pawn)) {
                    // Adjust back to before this Comp
                    if ( (lastMultiplier != float.MinValue) && lastMultiplier != 0) 
                    {
                        sexPart.GetPartComp().partFluidFactor /= lastMultiplier;
                    }
                    // Re-Adjust based on the new change by this Hediff
                    sexPart.GetPartComp().partFluidFactor *= CalculateOutput();
                    lastMultiplier = CalculateOutput();
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            // DevNote: As the Orgasm removes (or is planned to remove) this Hediff, it's important to also reset the value back to what it was before. 
            if (Pawn != null && !Pawn.Dead && lastMultiplier != float.MinValue && lastMultiplier != 0.0) { 
                foreach(ISexPartHediff sexPart in Props.GetSexPartHediffs(Pawn))
                {
                    sexPart.GetPartComp().partFluidFactor /= lastMultiplier;
                }
            }
        }

        protected float CalculateOutput()
        {
            float sev = parent.Severity;

            // Calculate the value at the given percentage between min and max
            float result = Props.min + (sev) * (Props.max - Props.min);
            return result;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref lastMultiplier, "lastMultiplier", 1.0f);
        }

    }
}
