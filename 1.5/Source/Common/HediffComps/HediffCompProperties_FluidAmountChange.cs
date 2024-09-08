using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffCompProperties_FluidAmountChange : HediffCompProperties
    {
        public float multiplier = 1.0f;

        public bool changePenis = false;
        public bool changeVagina = false;
        public bool changeBreast = false;
        public bool changeOther = false;

        public HediffCompProperties_FluidAmountChange() => this.compClass = typeof(HediffComp_FluidAmountChange);
    }
}
