using Cumpilation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Oscillation
{
    public class HediffCompProperties_ChangeSeverityOnOrgasm : HediffCompProperties
    {
        public float severityChange = 0.0f;

        public HediffCompProperties_ChangeSeverityOnOrgasm() => this.compClass = typeof(HediffComp_ChangeSeverityOnOrgasm);
    }
}
