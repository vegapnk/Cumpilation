using Verse;
using Verse.AI;
using Cumpilation.Cumflation;

namespace Cumpilation.Leaking
{
    public class ThinkNode_ConditionalCumflationSeverity : ThinkNode_Conditional
    {
        public Hediff cumflationHediff;

        protected override bool Satisfied(Pawn pawn)
        {
            cumflationHediff = CumflationUtility.GetOrCreateCumflationHediff(pawn);
            if (cumflationHediff == null)
            {
                return false;
            }
            return cumflationHediff.Severity > Settings.AutoDeflateMinSeverity;
        }

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalCumflationSeverity obj = (ThinkNode_ConditionalCumflationSeverity)base.DeepCopy(resolve);
            obj.cumflationHediff = cumflationHediff;
            return obj;
        }
    }
}