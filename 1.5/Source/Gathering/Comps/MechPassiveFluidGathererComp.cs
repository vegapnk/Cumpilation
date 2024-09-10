using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Gathering
{
    /// <summary>
    /// Does basically the same as the Passive-Fluid Gatherer but checks that it's only for mechs that have power. 
    /// </summary>
    public class MechPassiveFluidGathererComp : PassiveFluidGatherer
    {

        public override void CompTick()
        {
            if (this.parent is Pawn p) 
                if (p.IsColonyMech && p.needs.TryGetNeed<Need_MechEnergy>() != null && p.needs.TryGetNeed<Need_MechEnergy>().CurLevel >= 0)
                    base.CompTick();
        }
    }
}
