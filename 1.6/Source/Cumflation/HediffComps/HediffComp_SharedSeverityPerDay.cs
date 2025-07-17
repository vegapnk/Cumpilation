using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Cumpilation.Cumflation
{   
    /// <summary>
    /// Extension to the "HediffComp_SeverityPerDay" that adjusts depending on how many 
    /// Hediffs with this Comp are present. 
    /// 
    /// Example: You have 1 hediff with `-0.10` SeverityPerDay. It will behave as normal. 
    /// Example 2: You have 3 hediffs with `-0.15` SeverityPerDay each. 
    /// Because you have three, they will effectively reduce by `-0.05` each day, resulting in a "global" reduction of 0.15 amongst all. 
    /// </summary>
    public class HediffComp_SharedSeverityPerDay : HediffComp_SeverityModifierBase
    {
        public float sharedSeverityPerDay;

        private HediffCompProperties_SharedSeverityPerDay Props => (HediffCompProperties_SharedSeverityPerDay)this.props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            this.sharedSeverityPerDay = this.Props.CalculateSeverityPerDay();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<float>(ref this.sharedSeverityPerDay, "sharedSeverityPerDay");
            if (Scribe.mode != LoadSaveMode.PostLoadInit || (double)this.sharedSeverityPerDay != 0.0 || (double)this.Props.severityPerDay == 0.0 || !(this.Props.severityPerDayRange == FloatRange.Zero))
                return;
            this.sharedSeverityPerDay = this.Props.CalculateSeverityPerDay();
            Log.Warning("Hediff " + this.parent.Label + " had sharedSeverityPerDay not matching parent.");
        }

        public override float SeverityChangePerDay()
        {
            if ((double)this.Pawn.ageTracker.AgeBiologicalYearsFloat < (double)this.Props.minAge)
                return 0.0f;
            double severityPerDay = (double)this.sharedSeverityPerDay;
            HediffStage curStage = this.parent.CurStage;
            double num1 = curStage != null ? (double)curStage.severityGainFactor : 1.0;
            float num2 = (float)(severityPerDay * num1);

            return num2 / ((float)GetNumberOfHediffsWithSharedSeverity());
        }

        private int GetNumberOfHediffsWithSharedSeverity()
        {
            return this.Pawn.health
                .hediffSet.hediffs.Where(
                hediff => hediff.TryGetComp<HediffComp_SharedSeverityPerDay>() != null)
                .Count();
        }

        public override string CompLabelInBracketsExtra => this.Props.showHoursToRecover && (double)this.SeverityChangePerDay() < 0.0 ? Mathf.RoundToInt((float)((double)this.parent.Severity / (double)Mathf.Abs(this.SeverityChangePerDay()) * 24.0)).ToString() + (string)"LetterHour".Translate() : (string)null;

        public override string CompTipStringExtra => this.Props.showDaysToRecover && (double)this.SeverityChangePerDay() < 0.0 ? "DaysToRecover".Translate((NamedArgument)(this.parent.Severity / Mathf.Abs(this.SeverityChangePerDay())).ToString("0.0")).Resolve() : (string)null;

    }
}
