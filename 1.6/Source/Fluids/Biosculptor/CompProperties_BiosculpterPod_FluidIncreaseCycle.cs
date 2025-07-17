using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cumpilation.Fluids
{
    public class CompProperties_BiosculpterPod_FluidIncreaseCycle : CompProperties_BiosculpterPod_BaseCycle
    {

        public float increaseFactor = 1.2f;

        public CompProperties_BiosculpterPod_FluidIncreaseCycle() => this.compClass = typeof(Comp_BiosculpterPod_FluidIncreaseCycle);

    }
}

