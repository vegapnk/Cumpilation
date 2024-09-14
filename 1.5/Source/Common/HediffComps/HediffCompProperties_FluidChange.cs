using Cumpilation.Cumflation;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffCompProperties_FluidChange : HediffCompProperties_PartTargetting
    {
        public SexFluidDef fluid;

        public HediffCompProperties_FluidChange() => this.compClass = typeof(HediffComp_FluidChange);
    }
}
