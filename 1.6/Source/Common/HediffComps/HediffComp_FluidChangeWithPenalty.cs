using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffComp_FluidChangeWithPenalty : HediffComp_FluidChange
    {
        public new HediffCompProperties_FluidChangeWithPenalty Props => (HediffCompProperties_FluidChangeWithPenalty)this.props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            if (!this.WasSuccessfullyApplied() && Props.penaltyHediff != null && Props.penaltySeverity >= 0)
                Penalize();
        }

        protected void Penalize()
        {
            ModLog.Debug($"Fluid-Changes was unsuccessful, adding Penalty-Hediff {Props.penaltyHediff.defName} with Severity {Props.penaltySeverity} to Pawn {this.parent.pawn}");
            Hediff penaltyHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(Props.penaltyHediff);
            if (penaltyHediff == null)
            {
                penaltyHediff = HediffMaker.MakeHediff(Props.penaltyHediff, parent.pawn);
                penaltyHediff.Severity = 0;
                parent.pawn.health.AddHediff(penaltyHediff);
            }
            penaltyHediff.Severity += Props.penaltySeverity;
        }
    }
}
