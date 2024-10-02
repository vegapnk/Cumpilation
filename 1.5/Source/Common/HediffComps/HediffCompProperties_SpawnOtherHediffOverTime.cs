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

        public bool TryToSpawnInSameBodyPart = true;
        public float applicationChance = 1.0f;

    }
}
