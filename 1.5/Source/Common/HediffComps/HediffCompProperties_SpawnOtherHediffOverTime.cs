using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffCompProperties_SpawnOtherHediffOverTime : HediffCompProperties_PawnTargetting
    {

        public int tickInterval;
        public float severityIncrease;
        public HediffDef hediff;

        public bool tryToSpawnInSameBodyPart = true;
        public bool scaleWithHediffSeverity = false;
        public float applicationChance = 1.0f;

        public HediffCompProperties_SpawnOtherHediffOverTime() => this.compClass = typeof(HediffComp_SpawnOtherHediffOverTime);
    }
}
