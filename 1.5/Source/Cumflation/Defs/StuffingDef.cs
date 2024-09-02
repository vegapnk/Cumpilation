using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Cumflation
{
    public class StuffingDef : Def
    {
        public SexFluidDef fluid;
        /// <summary>
        /// Note: The Cumstuffed Hediff MUST have a `HediffComp_SharedSeverityPerDay` otherwise there will be issues. 
        /// </summary>
        public HediffDef resultingStuffedHediff;
        /// <summary>
        /// The amount of Fluid required for making a 1.0 Severity Hediff of the relative xxx-stuffing.
        /// </summary>
        public float amountOfFluidForFullStuffing = 100;

        /// <summary>
        /// How much severity does the resulting stuffing must have to give a thought?
        /// </summary>
        public float thoughtThreshold = 0.3f;
        public ThoughtDef positiveThought;
        public ThoughtDef neutralThought;
        public ThoughtDef negativeThought;
    }
}
