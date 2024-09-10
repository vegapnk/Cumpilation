using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Gathering
{
    public class PassiveFluidGathererCompProperties : CompProperties
    {
        public int range;
        public List<SexFluidDef> supportedFluids;
        public int tickIntervall;

        public float cleanChance = 0.25f;
        public bool cleanAtmostOne = false;

        public PassiveFluidGathererCompProperties()
        {
            compClass= typeof(PassiveFluidGathererComp);
        }

    
    }

}
