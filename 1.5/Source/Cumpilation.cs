using Cumpilation.Cumflation;
using Cumpilation.Gathering;
using Cumpilation.Reactions;
using System;
using Verse;

namespace Cumpilation
{
    [StaticConstructorOnStartup]
    public static class Cumpilation
    {
        static Cumpilation()
        {
            ModLog.Message("Cumpilation Loaded - Let's go you cumsluts");

            GatheringUtility.PrintFluidGatheringDefInfo();
            CumflationUtility.PrintCumflatableInfo();
            ReactionUtility.PrintFluidRecordInfo();
        }
    }
}