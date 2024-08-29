using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Gathering
{
    public class FluidGatheringDef : Def
    {
        public SexFluidDef fluidDef;
        public ThingDef thingDef;
        public float fluidRequiredForOneUnit;
        public bool canProduceMoreThanOne = true;
        public bool roundUp = false;
    }

}
