using Cumpilation.Cumflation;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Bukkake
{
    public class HediffCompProperties_BukkakeSpawnedByFluid : HediffCompProperties
    {
        public List<SexFluidDef> sexFluidDefs;

        public HediffCompProperties_BukkakeSpawnedByFluid() => this.compClass = typeof(HediffComp_BukkakeSpawnedByFluid);
    }
}
