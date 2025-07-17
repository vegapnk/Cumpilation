using Verse;

namespace Cumpilation.Bukkake
{
    public class HediffCompProperties_SpawnOrAdjustControllerHediff : HediffCompProperties
    {
        public HediffDef controller;
        /// <summary>
        /// How many of the sub-hediffs severity are required to have a 1.0 Severity of the corresponding Controller.
        /// Default 4.5 "full" (=Severity 1.0) parts to reach the full bukkake.
        /// For a normal pawn, there should be 6 or 7 bodyparts, so 4.5 is already quite a lot. 
        /// </summary>
        public float summedSeverityRequiredForFullController = 4.5f;


        public HediffCompProperties_SpawnOrAdjustControllerHediff() => this.compClass = typeof(HediffComp_SpawnOrAdjustControllerHediff);
    }

}