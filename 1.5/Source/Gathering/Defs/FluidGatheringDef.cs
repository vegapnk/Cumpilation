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

        public bool canBeRetrievedFromFilth = false;
        public int filthNecessaryForOneUnit;
        public ThingDef filth; // This is meant mostly as a fall-back or to be over-specific. Common Behaviour would be to go over the FluidDefs Filth and then from there "backwards". 
    }

}
