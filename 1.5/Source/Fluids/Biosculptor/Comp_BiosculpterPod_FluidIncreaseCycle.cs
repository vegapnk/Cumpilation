using Cumpilation.Common;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Fluids
{
    public class Comp_BiosculpterPod_FluidIncreaseCycle : CompBiosculpterPod_Cycle
    {

        public CompProperties_BiosculpterPod_FluidIncreaseCycle Props => (CompProperties_BiosculpterPod_FluidIncreaseCycle) this.props;

        public override void CycleCompleted(Pawn occupant)
        {
            if (!occupant.ageTracker.Adult)
            {
                ModLog.Debug($"Tried a fluid-cycle for an under-aged pawn {occupant} --- doing nothing");
                return;
            }

            var parts = Genital_Helper.get_AllPartsHediffList(occupant);
            foreach (var part in parts) { 
                if (part is ISexPartHediff sexPart && sexPart.GetPartComp().Fluid != null)
                {
                    sexPart.GetPartComp().partFluidFactor *= Props.increaseFactor;
                    occupant?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.Cumpilation_Juiced_Up);
                }
            }
        }

    }
}
