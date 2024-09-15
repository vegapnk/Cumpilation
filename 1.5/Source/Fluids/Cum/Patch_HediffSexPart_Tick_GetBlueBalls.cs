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

                if (__instance.pawn != null && __instance.pawn.IsHashIntervalTick(1000))
                {
                    ModLog.Debug($"Postfixing Patch_HediffSexPart_Tick_GetBlueBalls for {__instance} of {__instance.pawn}");
                    // TODO: Create or Find hediff & Bump Hediff Severity 
                }
            }
        }

    }
}
