using HarmonyLib;
using RimWorld;
using Verse;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using System.Reflection;

namespace Cumpilation.Gathering
{

    [HarmonyPatch(typeof(JobDriver), nameof(JobDriver.Cleanup))] 
    public class Patch_JobDriverCleaning_TrySpawnItem
    {

        public static void Postfix(JobDriver __instance)
        {
            if (!Settings.EnableFluidGatheringWhileCleaning) return;

            if (__instance is JobDriver_CleanFilth cleanJob && cleanJob.ended)
            {
                var type = cleanJob.GetType();
                if (type == null) return;

                var property = type.GetProperty("Filth", bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null) return;

                var rawValue = property.GetValue(cleanJob, null);
                if (rawValue == null) return;

                Filth filth = (Filth)rawValue;
                if (filth == null) return;

                var gDef = GatheringUtility.LookupGatheringDef(filth.def);
                if (gDef == null || gDef.filthNecessaryForOneUnit <= 0) return;
                float chance = 1.0f / gDef.filthNecessaryForOneUnit;

                //ModLog.Debug($"Running JobDriver_CleanFilth Cleanup - {cleanJob.pawn} cleaned {filth} which is supported");

                if (Rand.Chance(chance)){
                    var result = ThingMaker.MakeThing(gDef.thingDef);
                    result.stackCount = 1;
                    GenPlace.TryPlaceThing(result, __instance.pawn.PositionHeld, __instance.pawn.Map, ThingPlaceMode.Direct, out Thing res);
                }
            }
        }

    }
}
