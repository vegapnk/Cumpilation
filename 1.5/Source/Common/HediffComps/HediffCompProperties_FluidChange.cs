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
    public class HediffCompProperties_FluidChange : HediffCompProperties
    {
        public SexFluidDef fluid;

        public bool changePenis = false;
        public bool changeVagina = false;
        public bool changeBreast = false;
        public bool changeOther = false;

        public HediffCompProperties_FluidChange() => this.compClass = typeof(HediffComp_FluidChange);
    }
}
