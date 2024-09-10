using HarmonyLib;
using rjw;
using rjw.Modules.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Gathering
{
    /// <summary>
    /// Changes the Position that (Player) Pawns want to Masturbate at. 
    /// If there is a supported and free Fluid-Gatherer (Sink), a position next to it will be chosen. 
    /// 
    /// Extracted and Extended behaviour from Amevarashi Sexperience:
    /// https://gitgud.io/amevarashi/rjw-sexperience/-/blob/master/Source/RJWSexperience/Patches/RJW_Patch.cs#L123
    /// </summary>
    [HarmonyPatch(typeof(CasualSex_Helper), nameof(CasualSex_Helper.FindSexLocation))]
    public static class Patch_CasualSexHelper_FindNearbyBucketToMasturbate
    {
        /// <summary>
        /// If masturbation and current map has a bucket, return location near the bucket
        /// </summary>
        /// <param name="__result">The place to stand near a bucket</param>
        /// <returns>Run original method</returns>
        public static bool Prefix(Pawn pawn, Pawn partner, ref IntVec3 __result)
        {
            if (partner != null && partner != pawn)
                return true; // Not masturbation

            if (pawn.Faction?.IsPlayer != true && !pawn.IsPrisonerOfColony)
            {
                return true;
            }

            ISexPartHediff genitalWithMostFluid = 
                Genital_Helper.get_AllPartsHediffList(pawn)
                .Where(part => part is ISexPartHediff)
                .Cast<ISexPartHediff>()
                .Where(part => GatheringUtility.HasAnySupportedFluidGatheringDefs(part))
                .OrderByDescending(f => f.GetPartComp().FluidAmount)
                .FirstOrFallback();

            Building fluidSink = GatheringUtility.FindClosestFluidSink(pawn, genitalWithMostFluid.GetPartComp().Fluid);

            if (fluidSink == null)
            {
                ModLog.Debug($"Did not find a (supported) Fluid-Sink for {pawn}s Masturbation - Continuing with normal location-detection.");
                return true;
            } else
            {
                ModLog.Debug($"Found a fluid-sink {fluidSink}@{fluidSink.Position} for {pawn} to masturbate his {genitalWithMostFluid} into.");
            }

            IntVec3 cellCandidate = GenAdjFast.AdjacentCells8Way(fluidSink.Position)
                .Where(cell => cell.Standable(fluidSink.Map))
                .OrderBy(cell => cell.DistanceTo(pawn.Position))
                .FirstOrFallback();

            if (cellCandidate != null)
            {
                __result = cellCandidate;
                ModLog.Debug($"Found good Sink {fluidSink} for {pawn}s masturbation --- setting location to {__result}");
                return false;
            } else
            {
                ModLog.Debug($"Failed to find situable location near a sink at {fluidSink.Position} for {pawn}s masturbation");
                return true;
            }

        }

    }
}
