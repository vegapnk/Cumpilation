using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Cumflation
{
    public class FluidSource : IExposable
    {
        public Pawn pawn;
        public SexFluidDef fluid;
        public float amount;

        public void ExposeData()
        {
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn");
            Scribe_Defs.Look<SexFluidDef>(ref this.fluid, "fluid");
            Scribe_Values.Look<float>(ref this.amount, "amount");
        }
    }
}
