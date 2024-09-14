using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffCompProperties_FluidAmountChange : HediffCompProperties_PartTargetting
    {
        public float multiplier = 1.0f;

        public HediffCompProperties_FluidAmountChange() => this.compClass = typeof(HediffComp_FluidAmountChange);
    }
}
