using Cumpilation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cumpilation.Common
{
    public class HediffCompProperties_RandomlySpawnFilth : HediffCompProperties_PartTargetting
    {
        public int ticksBetweenCheck;
        public float chanceToSpawn;

        /// <summary>
        /// Scale the chance based on severity. If Chance is 50% and Severity is 50%, then there is a 25% final chance.
        /// </summary>
        public bool chanceBasedOnSeverity = false;
        /// <summary>
        /// Only try to spawn filth if the current stage is higher than the specified number.
        /// </summary>
        public int onlyAtStagesHigherThan = int.MinValue;


        public HediffCompProperties_RandomlySpawnFilth() => this.compClass = typeof(HediffComp_RandomlySpawnFilth);

    }
}
