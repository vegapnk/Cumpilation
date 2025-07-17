using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Cumflation
{
    public class HediffCompProperties_VomitFluidMtb : HediffCompProperties
    {

        /// <summary>
        /// "mean time between" measured in days. 
        /// If set to 1, there will be 1 vomiting per days (in random occurrence).
        /// If set to 2, there will be 0.5 vomiting per days, etc. 
        /// If set to 0.1, there will be 10 vomitings per days
        /// </summary>
        public float mtbDays;

        /// <summary>
        /// If true, the "chance" of application is scaled with the Severity of the Stuffing. 
        /// So if your stuffing is 1.0, it will have a "full mtbDays". 
        /// Otherwise, if your stuffing is 0.5, it will will result in half as much Vomiting.
        /// </summary>
        public bool scaleWithSeverity = true;
        public HediffCompProperties_VomitFluidMtb() => this.compClass = typeof(HediffComp_VomitFluidMtb);
    }
}
