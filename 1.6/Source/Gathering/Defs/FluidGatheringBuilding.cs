using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Gathering
{
    /// <summary>
    /// Extension that can be added to (storage) buildings, to gather fluids on orgasm. 
    /// This is meant to manage things at `Patch_SatisfyPersonal_FillFluidGatherers`. 
    /// </summary>
    public class FluidGatheringBuilding : DefModExtension
    {
        /// <summary>
        /// The range in which fluids are gathered on Orgasm
        /// </summary>
        public int range;
        /// <summary>
        /// All supported fluids, so that a cumbucket cannot collect girl-cum.
        /// </summary>
        public List<SexFluidDef> supportedFluids;

        /// <summary>
        /// "true" will work for all Sextypes, "false" will only work for Handjob, Footjob, etc. things on the outside. 
        /// </summary>
        public bool collectsFromInternal = false;
    }
}
