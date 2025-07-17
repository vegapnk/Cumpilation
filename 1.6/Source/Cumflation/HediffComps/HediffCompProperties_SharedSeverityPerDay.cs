using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class HediffCompProperties_SharedSeverityPerDay : HediffCompProperties
    {
        public float severityPerDay;
        public bool showDaysToRecover;
        public bool showHoursToRecover;
        public float mechanitorFactor = 1f;
        public float reverseSeverityChangeChance;
        public FloatRange severityPerDayRange = FloatRange.Zero;
        public float minAge;
        public HediffCompProperties_SharedSeverityPerDay() => this.compClass = typeof(HediffComp_SharedSeverityPerDay);

        public float CalculateSeverityPerDay()
            {
                float severityPerDay = this.severityPerDay + this.severityPerDayRange.RandomInRange;
                if (Rand.Chance(this.reverseSeverityChangeChance))
                    severityPerDay *= -1f;
                return severityPerDay;
            }
        }

}
