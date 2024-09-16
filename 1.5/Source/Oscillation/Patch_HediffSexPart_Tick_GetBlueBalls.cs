using Cumpilation.Oscillation;
using HarmonyLib;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Fluids.Cum
{

    [HarmonyPatch(typeof(Hediff), nameof(Hediff.PostTick))]
    public static class Patch_HediffSexPart_Tick_GetBlueBalls
    {
        // Check (& Increase) every 2400 ticks; Had 2000 first but if it's divisble by 3 and 4 we get a nice number for increase every bump.
        public const int BLUEBALL_TICK_INTERVALL = 2400;

        // One day has 60k ticks. I want to have "full" blue balls after 4 days. This means that after 
        public const float BLUEBALL_SEVERITY_INCREASE = 1.0f / (4.0f * 60000.0f / BLUEBALL_TICK_INTERVALL);

        public static void Postfix(Hediff __instance)
        {
            if (__instance != null && __instance is ISexPartHediff sexPart)
            {
                Pawn pawn = __instance.pawn;
                if (pawn == null || pawn.IsAnimal() || pawn.Dead) return;

                // Do only touch things on this Map and spawned --- helps to avoid ticking up hediffs in pawns outside of view.
                if (!pawn.Spawned || !pawn.Map.IsPlayerHome) return;

                if (sexPart.GetPartComp().Def == null || sexPart.GetPartComp().Def.fluid == null) 
                    return;
                
                // Requirement: The fluid must be "storable", defined as a tag. 
                // DevNote: I wanted this to be at the Genital, but 
                if (!sexPart.GetPartComp().Def.fluid.tags.Contains("CanBlueBall"))
                    return;

                if (pawn.health.hediffSet.HasHediff(DefOfs.Cumpilation_Drained))
                    return;

                if (__instance.pawn != null && __instance.pawn.IsHashIntervalTick(BLUEBALL_TICK_INTERVALL))
                {
                    //ModLog.Debug($"Postfixing Patch_HediffSexPart_Tick_GetBlueBalls for {__instance.def.defName} of {__instance.pawn}, bumping by {BLUEBALL_SEVERITY_INCREASE}");
                    var blueBallHediff = OscillationHelper.GetOrCreateBlueBallsHediff(pawn);
                    blueBallHediff.Severity += BLUEBALL_SEVERITY_INCREASE;
                }
            }
        }


    }
}
