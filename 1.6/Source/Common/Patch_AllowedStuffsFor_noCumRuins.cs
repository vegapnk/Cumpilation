using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace Cumpilation
{


    //Patch to Blacklist `Cumpilation_Samenstein` From being used to construct Ruins.
    [HarmonyPatch(typeof(GenStuff))]
    [HarmonyPatch(nameof(GenStuff.AllowedStuffsFor))]
    internal class Patch_AllowedStuffsFor_noCumRuins
    {
        static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> values) 
        {
            foreach (var value in values)
                if (value != DefDatabase<ThingDef>.GetNamed("Cumpilation_Samenstein"))
                    yield return value;
        }
    }


}
