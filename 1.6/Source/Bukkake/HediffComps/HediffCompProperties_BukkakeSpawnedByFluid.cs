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
        public int fluidRequiredForSeverityOne = 10; // 10 is about what a normal pawn has. So one pawn should be able to cover one bodypart (e.g. one arm, or one chest). 5 Pawns will result in a full bukkake. 

        public HediffCompProperties_BukkakeSpawnedByFluid() => this.compClass = typeof(HediffComp_BukkakeSpawnedByFluid);
    }
}
